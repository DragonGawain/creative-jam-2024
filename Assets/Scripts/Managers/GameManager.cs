using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.Tilemaps;
using System.Linq;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
#region Fields
    public static event Action NextGameTick;
    public static event Action AltNextGameTick;

    // grids & tilemaps
    // where the levels are displayed

    static Grid actualGrid;
    static Tilemap levelItemsActual;
    static Tilemap levelGroundActual;
    static Tilemap levelBackgroundActual;
    static Level currentLevel;

    // level blueprints
    static Grid levelGrid;
    static Tilemap levelItemsBlueprint;
    static Tilemap levelGroundBlueprint;
    static Tilemap levelBackgroundBlueprint;

    public static int nWalls = 24;

    static GameObject deathScreen;

    static bool paused = false;

    static PlayerController player;

    // I'm setting the hypthetical limit to 10 mimics. I don't think we'll ever have more than like, 2 at once, but just in case..
    static MimicController[] mimics = new MimicController[10];

    static int mimicIndex = 0;

    static GameObject playerObject;
    static GameObject mimicObject;

    public static int windCounter = 0;

    [SerializeField, Range(2, 5)]
    static int windCounterReset = 3;

    static int walkingDamage = 1;

    static GameManager singleton;

    static bool isFirstLoad;

#endregion

#region Unity Methods

    // Start is called before the first frame update
    void Awake()
    {
        if (!singleton)
            singleton = this;

        if (this != singleton)
            Destroy(this.gameObject);

        DontDestroyOnLoad(gameObject);

        isFirstLoad = false;
    }

    private void Start()
    {
        // StartNewLevel(1);
    }

    // Update is called once per frame
    void Update() { }

#endregion

#region Level Utils
    public Level getLevel()
    {
        return currentLevel;
    }

    public static void StartNewLevel(int levelNb)
    {
        if (!isFirstLoad)
        {
            initializeFirstLoad();
            isFirstLoad = true;
        }

        updateTilemaps(levelNb);
        clearLevel();
        loadLevel();
        reloadPlayer();
    }

    static void initializeFirstLoad()
    {
        deathScreen = GameObject.FindGameObjectWithTag("Death");
        deathScreen.transform.parent.GetChild(2).gameObject.SetActive(false);
        ShowDeathScreen(false);

        actualGrid = GameObject.Find("Actual_Grid").GetComponent<Grid>();

        DontDestroyOnLoad(actualGrid);
        levelItemsActual = actualGrid.transform.Find("Actual_Items").GetComponent<Tilemap>();
        levelGroundActual = actualGrid.transform.Find("Actual_Ground").GetComponent<Tilemap>();
        levelBackgroundActual = actualGrid.transform
            .Find("Actual_Background")
            .GetComponent<Tilemap>();

        playerObject = Resources.Load<GameObject>("Other/Player");
        mimicObject = Resources.Load<GameObject>("Other/Mimic");

        // make sure that the mimic array is clear
        Array.Clear(mimics, 0, mimics.Length);

        // player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        // deathScreen = GameObject.FindGameObjectWithTag("Death");
        // ShowDeathScreen(false);


        NextGameTick += IncrementWind;

        Variation.InitializeVariationSprites();
    }

    public static void ResetCurrentLevel()
    {
        if(!currentLevel) 
        {
            StartNewLevel(1);
        }
        else
        {
            StartNewLevel(currentLevel.getLevelNum());
        }
    }

    public static void ShowDeathScreen(bool showDeath)
    {
        deathScreen.SetActive(showDeath);
        paused = showDeath;
    }

    private static void updateTilemaps(int levelNb)
    {
        string levelGridName = "Level" + levelNb;
        levelGrid = GameObject.Find(levelGridName).GetComponent<Grid>();
        levelGroundBlueprint = levelGrid.transform
            .Find(levelGridName + "_Ground")
            .GetComponent<Tilemap>();
        levelBackgroundBlueprint = levelGrid.transform
            .Find(levelGridName + "_Background")
            .GetComponent<Tilemap>();
        levelItemsBlueprint = levelGrid.transform
            .Find(levelGridName + "_Items")
            .GetComponent<Tilemap>();

        levelGroundActual.ClearAllTiles();
        levelGroundActual.RefreshAllTiles();

        levelBackgroundActual.ClearAllTiles();
        levelBackgroundActual.RefreshAllTiles();
    }
    public static void clearLevel()
    {
        levelGroundActual.ClearAllTiles();
        levelGroundActual.RefreshAllTiles();

        levelBackgroundActual.ClearAllTiles();
        levelBackgroundActual.RefreshAllTiles();

        levelItemsActual.ClearAllTiles();
        levelItemsActual.RefreshAllTiles();
    }
    private static void loadLevel()
    {
        loadGround();
        loadBackground();

        currentLevel = levelGrid.GetComponent<Level>();
        loadItems();
    }

    private static void reloadPlayer()
    {
        if (GameObject.FindWithTag("Player"))
            Destroy(GameObject.FindWithTag("Player"));

        if (GameObject.FindGameObjectsWithTag("Mimic").Length > 0)
        {
            GameObject[] mimis = GameObject.FindGameObjectsWithTag("Mimic");
            foreach (GameObject mimi in mimis)
                Destroy(mimi);
        }

        // make sure that the mimic array is clear
        Array.Clear(mimics, 0, mimics.Length);

        mimicIndex = 0;

        GameObject playerInstance = Instantiate(
            playerObject,
            actualGrid.CellToWorld(currentLevel.getPlayerStartLoc()) + new Vector3(0.5f, 0.5f, 0),
            Quaternion.identity
        );
        player = playerInstance.GetComponent<PlayerController>();
        player.SetQueueSize(currentLevel.getQueueSize());
        UIManager.SetQueueSize(currentLevel.getQueueSize());

        loadInitialVariations();

        // Reset pause screen if necessary
        if (paused)
        {
            paused = false;
            deathScreen.SetActive(false);
            //player.transform.position = new Vector3(0.5f, 0.5f, 0);
        }
    }


    private static void loadGround()
    {
        int durability;
        bool breakable;
        GroundTile.GroundTileType gtt;

        foreach (Vector3Int loc in levelGroundBlueprint.cellBounds.allPositionsWithin)
        {
            TileBase tb = levelGroundBlueprint.GetTile(loc);
            if (!tb)
                continue;

            durability = 1;
            breakable = false;
            gtt = GroundTile.GroundTileType.NULL;

            GroundTile gt = ScriptableObject.CreateInstance<GroundTile>();
            gt.Initialize();

            // check if rock
            for (int i = 0; i <= 3; i++)
            {
                if (tb.name.Length > 4 && tb.name[..5].Equals("Rock" + i))
                {
                    durability = i + 1;
                    breakable = true;
                    gtt = GroundTile.GroundTileType.ROCK;
                }
            }

            switch (tb.name)
            {
                case "Crystal":
                    gtt = GroundTile.GroundTileType.CRYSTAL;
                    break;
                case "End":
                    gtt = GroundTile.GroundTileType.END;
                    break;
            }

            levelGroundActual.SetTile(loc, gt);

            ((GroundTile)levelGroundActual.GetTile(loc)).SetValues(durability, breakable, gtt);

            levelGroundActual.RefreshTile(loc);
        }
    }

    private static void loadBackground()
    {
        foreach (Vector3Int loc in levelBackgroundBlueprint.cellBounds.allPositionsWithin)
        {
            TileBase tb = levelBackgroundBlueprint.GetTile(loc);
            if (!tb)
                continue;

            BackgroundTile bgt = ScriptableObject.CreateInstance<BackgroundTile>();
            bgt.Initialize();

            levelBackgroundActual.SetTile(loc, bgt);
            ((BackgroundTile)levelBackgroundActual.GetTile(loc)).SetSprite(tb.name);

            levelBackgroundActual.RefreshTile(loc);
        }
    }

    private static void loadItems()
    {
        foreach (Vector3Int loc in levelItemsBlueprint.cellBounds.allPositionsWithin)
        {
            TileBase tb = levelItemsBlueprint.GetTile(loc);
            if (!tb)
                continue;

            ItemTile itemTile = ScriptableObject.CreateInstance<ItemTile>();
            itemTile.Initialize();

            int size;
            switch (tb.name[..4])
            {
                case "wind":
                    size = currentLevel.getWindSize();
                    break;
                case "move":
                    size = currentLevel.getMoveSize();
                    break;
                case "mimi":
                    size = currentLevel.getMimicSize();
                    break;
                case "boot":
                    size = currentLevel.getBootSize();
                    break;
                default:
                    size = 1;
                    break;
            }
            levelItemsActual.SetTile(loc, itemTile);
            ((ItemTile)levelItemsActual.GetTile(loc)).SetSprite(tb.name, size);

            levelItemsActual.RefreshTile(loc);
        }
    }

#endregion

#region Variations
    static void loadInitialVariations()
    {
        // set move size
        Variation.SetVariationTypeSize(VariationType.MOVE_UP, currentLevel.getMoveSize());
        Variation.SetVariationTypeSize(VariationType.MOVE_DOWN, currentLevel.getMoveSize());
        Variation.SetVariationTypeSize(VariationType.MOVE_LEFT, currentLevel.getMoveSize());
        Variation.SetVariationTypeSize(VariationType.MOVE_RIGHT, currentLevel.getMoveSize());

        // set wind size
        Variation.SetVariationTypeSize(VariationType.WIND_UP, currentLevel.getWindSize());
        Variation.SetVariationTypeSize(VariationType.WIND_DOWN, currentLevel.getWindSize());
        Variation.SetVariationTypeSize(VariationType.WIND_LEFT, currentLevel.getWindSize());
        Variation.SetVariationTypeSize(VariationType.WIND_RIGHT, currentLevel.getWindSize());

        // set mimic size
        Variation.SetVariationTypeSize(VariationType.MIMIC, currentLevel.getMimicSize());

        // set boots size
        Variation.SetVariationTypeSize(VariationType.BOOTS, currentLevel.getBootSize());

        foreach (VariationType vari in currentLevel.getStartVariations())
        {
            player.Enqueue(new Variation(vari, Variation.GetVariationSize(vari)));
        }
    }
#endregion


#region Movement

    public static Vector3 Move(Transform trans, Vector2Int dir, out bool legalMove)
    {
        Vector3 pos = trans.position;
        legalMove = true;

        // Stop player from moving when paused
        if (paused)
        {
            legalMove = false;
            return pos;
        }

        // CellToWorld retrns bottom-left corner
        Vector3Int oldCellLocation = actualGrid.WorldToCell(pos);
        Vector3Int newCellLocation = actualGrid.WorldToCell(pos) + new Vector3Int(dir.x, dir.y, 0);

        int legality = CheckMoveLegality(oldCellLocation, newCellLocation);

        switch (legality)
        {
            case 0:
                break;
            case 1:
                legalMove = false;
                return pos;
            case 2:
                // death of the player
                if (trans.TryGetComponent<MimicController>(out MimicController deadMimic))
                {
                    DespawnMimic(deadMimic.GetMimicIndex());
                }
                // if it's not a mimic, it's the player
                else
                {
                    Die();
                }
                break;
        }

        // if in ghost mode - decrement ghost move charges
        if (
            player.GetIsGhost()
            && trans.TryGetComponent<PlayerController>(out PlayerController iDontNeedThis)
        )
        {
            player.DecrementGhostCharges();
        }

        // otherwise, not in ghost mode - deal damage to the ground
        if (!player.GetIsGhost())
        {
            GroundTile gt = (GroundTile)levelGroundActual.GetTile(oldCellLocation);
            int durabilityRemaining = gt.decreaseDurability(walkingDamage);
            if(durabilityRemaining <= 0) levelGroundActual.SetTile(oldCellLocation, null);
            levelGroundActual.RefreshTile(oldCellLocation);
        }

        if (levelItemsActual.HasTile(newCellLocation))
        {
            ItemTile itemTile = (ItemTile)levelItemsActual.GetTile(newCellLocation);
            if (itemTile.GetVariation().GetVariationType() == VariationType.MIMIC)
            {
                // itemTile.GetVariation().SetMimicIndex(mimicIndex);
                mimicIndex = itemTile.GetMimicIndex();
                SpawnMimic();
            }

            player.Enqueue(itemTile.GetVariation());

            levelItemsActual.SetTile(newCellLocation, null);
            levelItemsActual.RefreshTile(newCellLocation);
        }

        return actualGrid.CellToWorld(newCellLocation) + new Vector3(0.5f, 0.5f, 0);

        checkWin();
    }

    static int CheckMoveLegality(Vector3Int oldCellLocation, Vector3Int newCellLocation)
    {
        // 0 => legal
        // 1 => illegal
        // 2 => dead

        int legal = 0;

        // Walking into level border - illegal move
        if (((BackgroundTile)levelBackgroundActual.GetTile(newCellLocation)).GetIsWall())
        {
            legal = 1;
        }
        // Walking into a crystal
        else if (levelGroundActual.HasTile(newCellLocation))
        {
            // not in ghost mode - illegal
            if (
                ((GroundTile)levelGroundActual.GetTile(newCellLocation)).GetGroundTileType()
                    == GroundTile.GroundTileType.CRYSTAL
                && !player.GetIsGhost()
            )
            {
                legal = 1;
            }
            // otherwise, it's a normal ground tile - legality checks end
        }
        // Trying to walk onto lava (no ground tile, background is not a wall) - kill player/mimic
        else
        {
            // TODO:: add logic to detect if it's a mimic and not the player

            // Whenever a player steps on lava, they Die™
            // Debug.Log("Current level tile: " + newCellLocation);
            // Debug.Log("Current level tile: " + levelGroundActual.GetTile(new Vector3Int(0, 0)));
            if (levelGroundActual.GetTile(newCellLocation) == null && !player.GetIsGhost())
                legal = 2;
        }

        return legal;
    }

#endregion

#region Mimic
    public static void SpawnMimic()
    {
        // check if there are any mimics currently alive to determine where to spawn the mimic
        GameObject mimicInstance = Instantiate(
            mimicObject,
            currentLevel.getMimicStartLocs()[mimicIndex],
            Quaternion.identity
        );
        MimicController mimicController = mimicInstance.GetComponent<MimicController>();
        mimicController.SetMimicIndex(mimicIndex);
        mimics[mimicIndex] = mimicController;
    }

    public static void DespawnMimic(int index)
    {
        // gotta check if it's null cause there's multiple ways that a mimic can die
        if (mimics[index] != null)
            Destroy(mimics[index].gameObject);
        mimics[index] = null;
    }

#endregion

#region Misc
    public void TriggerNextGameTick()
    {
        NextGameTick?.Invoke();
    }
    public static void SetWalkingDamage(int d)
    {
        walkingDamage = d;
    }

    static void Die()
    {
        // Die™
        Debug.Log("Dying™");

        // Show death ui
        ShowDeathScreen(true);
    }

    public static Vector3 AlignToGrid(Vector3 pos)
    {
        return actualGrid.CellToWorld(actualGrid.WorldToCell(pos)) + new Vector3(0.5f, 0.5f, 0);
    }

    public static void SafeLanding()
    {
        Vector3Int playerPos = actualGrid.WorldToCell(player.transform.position);
        GroundTile gt = ((GroundTile) levelGroundActual.GetTile(playerPos));
        GroundTile.GroundTileType gtt = gt.getGroundTileType();
        if(gtt != GroundTile.GroundTileType.ROCK
        && gtt != GroundTile.GroundTileType.END)
        {
            //TODO kill player
        }

    }

    public static void checkWin()
    {
        List<GroundTile> groundTiles = tilemapToList(levelGroundActual);
        int endTiles = groundTiles.Count(gt => gt.getGroundTileType() == GroundTile.GroundTileType.END);
        int allTiles = groundTiles.Count(gt => gt.getGroundTileType() != GroundTile.GroundTileType.CRYSTAL);
        if(endTiles == allTiles)
        {
            // TODO WIN
        }
    }

    private static List<GroundTile> tilemapToList(Tilemap tm)
    {
        List<GroundTile> tmpList = new();
        GroundTile tmpGt;
        foreach(Vector3Int loc in tm.cellBounds.allPositionsWithin)
        {
            tmpGt = (GroundTile) tm.GetTile(loc);
            if(!tmpGt) continue;

            tmpList.Add(tmpGt);
        }

        return tmpList;
    }
#endregion

#region Wind
    static void IncrementWind()
    {
        windCounter++;
        if (windCounter >= windCounterReset)
        {
            windCounter = 0;
            if (player.GetIsGhost())
                return;
            // check if there is wind in the queue
            Queue<VariationType> varTypes = player.GetVariationTypesQueue();
            List<VariationType> varTypesList = varTypes.ToList();

            Vector3 pos = new(0, 0, 0);

            Animator pa = player.GetPlayerAnimator();

            int tmp = Mathf.CeilToInt(
                (float)varTypesList.Count(variant => variant == VariationType.WIND_UP)
                    / (float)currentLevel.getWindSize()
                    - 0.01f
            ); // yay safety
            if (tmp > 0)
            {
                for (int i = 0; i < tmp; i++)
                {
                    Move(player.transform, new Vector2Int(0, 1), out bool legalMove);
                    if (legalMove)
                    {
                        pa.SetTrigger("walk_up");
                        player.SetLastMove(0,-1);
                        AltNextGameTick?.Invoke();
                    }
                    
                }
            }

            tmp = Mathf.CeilToInt(
                (float)varTypesList.Count(variant => variant == VariationType.WIND_DOWN)
                    / (float)currentLevel.getWindSize()
                    - 0.01f
            );
            if (tmp > 0)
            {
                for (int i = 0; i < tmp; i++)
                {
                    Move(player.transform, new Vector2Int(0, -1), out bool legalMove);
                    if (legalMove)
                    {
                        pa.SetTrigger("walk_down");
                        player.SetLastMove(0,1);
                        AltNextGameTick?.Invoke();
                    }

                    
                }
            }

            tmp = Mathf.CeilToInt(
                (float)varTypesList.Count(variant => variant == VariationType.WIND_LEFT)
                    / (float)currentLevel.getWindSize()
                    - 0.01f
            );
            if (tmp > 0)
            {
                for (int i = 0; i < tmp; i++)
                {
                    Move(player.transform, new Vector2Int(-1, 0), out bool legalMove);
                    if (legalMove)
                    {
                        pa.SetTrigger("walk_left");
                        player.SetLastMove(1,0);
                        AltNextGameTick?.Invoke();

                    }
                }
            }

            tmp = Mathf.CeilToInt(
                (float)varTypesList.Count(variant => variant == VariationType.WIND_RIGHT)
                    / (float)currentLevel.getWindSize()
                    - 0.01f
            );
            if (tmp > 0)
            {
                for (int i = 0; i < tmp; i++)
                {
                    Move(player.transform, new Vector2Int(1, 0), out bool legalMove);
                    if (legalMove)
                    {
                        pa.SetTrigger("walk_right");
                        player.SetLastMove(-1,0);
                        AltNextGameTick?.Invoke();
                    }
                }
            }
        }
    }
#endregion
}
