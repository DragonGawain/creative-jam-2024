using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.Tilemaps;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static event Action NextGameTick;

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

    static GameObject playerObject;

    public static int windCounter = 0;

    [SerializeField, Range(2, 5)]
    int windCounterReset = 3;


    static int walkingDamage = 1;



    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        actualGrid = GameObject.Find("Actual_Grid").GetComponent<Grid>();

        DontDestroyOnLoad(actualGrid);
        levelItemsActual = actualGrid.transform.Find("Actual_Items").GetComponent<Tilemap>(); 
        levelGroundActual = actualGrid.transform.Find("Actual_Ground").GetComponent<Tilemap>();
        levelBackgroundActual = actualGrid.transform.Find("Actual_Background").GetComponent<Tilemap>();

        playerObject = Resources.Load<GameObject>("Other/Player");


        // player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        deathScreen = GameObject.FindGameObjectWithTag("Death");

        /*
            Death screen is active by default and hidden on wake
            to be able to fetch a reference to it
        */
        //deathScreen.SetActive(false);

        NextGameTick += IncrementWind;

        Variation.InitializeVariationSprites();

    }

    private void Start()
    {
        StartNewLevel(1);
    }

    // Update is called once per frame
    void Update() { }

    public Level getLevel() { return currentLevel; }
    public static void clearLevel()
    {
        levelGroundActual.ClearAllTiles();
        levelGroundActual.RefreshAllTiles();

        levelBackgroundActual.ClearAllTiles();
        levelBackgroundActual.RefreshAllTiles();

        levelItemsActual.ClearAllTiles();
        levelItemsActual.RefreshAllTiles();
    }

    public static void StartNewLevel(int levelNb)
    {

        updateTilemaps(levelNb);
        clearLevel();
        loadLevel();

        if (GameObject.FindWithTag("Player"))
            Destroy(GameObject.FindWithTag("Player"));

        GameObject playerInstance = Instantiate(playerObject, new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
        player = playerInstance.GetComponent<PlayerController>();

        // Reset pause screen if necessary
        if (paused)
        {
            paused = false;
            deathScreen.SetActive(false);
            player.transform.position = new Vector3(0.5f, 0.5f, 0);
        }
    }
    private static void loadLevel()
    {
        loadGround();
        loadBackground();

        currentLevel = levelGrid.GetComponent<Level>();
        loadItems();

    }

    private static void updateTilemaps(int levelNb)
    {
        string levelGridName = "Level" + levelNb;
        levelGrid = GameObject.Find(levelGridName).GetComponent<Grid>();
        levelGroundBlueprint = levelGrid.transform.Find(levelGridName + "_Ground").GetComponent<Tilemap>();
        levelBackgroundBlueprint = levelGrid.transform.Find(levelGridName + "_Background").GetComponent<Tilemap>();
        levelItemsBlueprint = levelGrid.transform.Find(levelGridName + "_Items").GetComponent<Tilemap>();

        levelGroundActual.ClearAllTiles();
        levelGroundActual.RefreshAllTiles();

        levelBackgroundActual.ClearAllTiles();
        levelBackgroundActual.RefreshAllTiles();
    }

    private static void loadGround()
    {
        int durability;
        bool breakable;
        GroundTile.GroundTileType gtt;
        

        foreach(Vector3Int loc in levelGroundBlueprint.cellBounds.allPositionsWithin)
        {
            TileBase tb = levelGroundBlueprint.GetTile(loc);
            if(!tb) continue;

            durability = 1;
            breakable = false;
            gtt = GroundTile.GroundTileType.NULL;

            GroundTile gt = ScriptableObject.CreateInstance<GroundTile>();
            gt.Initialize();
            
            // check if rock
            for(int i = 0; i <= 3; i++)
            {
                if(tb.name.Length > 4 && tb.name[..5].Equals("Rock" + i))
                {
                    durability = i + 1;
                    breakable = true;
                    gtt = GroundTile.GroundTileType.ROCK;
                }
            }

            switch(tb.name)
            {
                case "Crystal":
                    gtt = GroundTile.GroundTileType.CRYSTAL;
                    break;
                case "End":
                    gtt = GroundTile.GroundTileType.END;
                    break;
            }

            levelGroundActual.SetTile(loc, gt);

            ((GroundTile) levelGroundActual.GetTile(loc)).SetValues(durability, breakable, gtt);

            levelGroundActual.RefreshTile(loc);

        }
    }

    private static void loadBackground()
    {
        foreach(Vector3Int loc in levelBackgroundBlueprint.cellBounds.allPositionsWithin)
        {
            TileBase tb = levelBackgroundBlueprint.GetTile(loc);
            if(!tb) continue;

            BackgroundTile bgt = ScriptableObject.CreateInstance<BackgroundTile>();
            bgt.Initialize();

            levelBackgroundActual.SetTile(loc, bgt);
            ((BackgroundTile) levelBackgroundActual.GetTile(loc)).SetSprite(tb.name);

            levelBackgroundActual.RefreshTile(loc);
        }
       
    }

    private static void loadItems()
    {
        foreach(Vector3Int loc in levelItemsBlueprint.cellBounds.allPositionsWithin)
        {
            TileBase tb = levelItemsBlueprint.GetTile(loc);
            if(!tb) continue;

            ItemTile itemTile = ScriptableObject.CreateInstance<ItemTile>();
            itemTile.Initialize();

            int size;
            switch(tb.name[..4])
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
            ((ItemTile) levelItemsActual.GetTile(loc)).SetSprite(tb.name, size);

            levelItemsActual.RefreshTile(loc);
        }

    }

    public void TriggerNextGameTick()
    {
        NextGameTick?.Invoke();
    }

    public static Vector3 Move(Vector3 pos, Vector2Int dir, out bool legalMove)
    {
        legalMove = true;

        // Stop player from moving when paused
        if (paused)
        {
            legalMove = false;
            return pos;
        }

        // Vector3Int cellLoc = grid.WorldToCell(pos);
        // int hp = (WorldTile)tilemap.GetTile(cellLoc).decreaseDurability();
        // if (hp <= 0)
        //     destroy(tile);

        // CellToWorld retrns bottom-left corner

        Vector3Int oldCellLocation = actualGrid.WorldToCell(pos);
        Vector3Int newCellLocation = actualGrid.WorldToCell(pos) + new Vector3Int(dir.x, dir.y, 0);


        // Walking into level border - illegal move
        if (((BackgroundTile)levelBackgroundActual.GetTile(newCellLocation)).GetIsWall())
        {
            legalMove = false;
            return pos;
        }
        // Walking into a crystal
        else if (levelGroundActual.HasTile(oldCellLocation))
        {
            // not in ghost mode - illegal
            if(
                ((GroundTile)levelGroundActual.GetTile(oldCellLocation)).GetGroundTileType() == GroundTile.GroundTileType.CRYSTAL 
                && !player.GetIsGhost()
            )
            {
                legalMove = false;
                return pos;
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
            if (levelGroundActual.GetTile(newCellLocation) == null)
                Die();

        }



        // if in ghost mode - decrement ghost move charges
        if (player.GetIsGhost())
        {
            player.DecrementGhostCharges();
        }
        // otherwise, not in ghost mode - deal damage to the ground
        else
        {
            ((GroundTile) levelGroundActual.GetTile(oldCellLocation)).decreaseDurability(walkingDamage);
        }



        if (levelItemsActual.HasTile(newCellLocation))
        {
            ItemTile itemTile = (ItemTile) levelItemsActual.GetTile(newCellLocation);
            player.Enqueue(itemTile.GetVariation());
            levelItemsActual.SetTile(newCellLocation, null);
        }

        return actualGrid.CellToWorld(newCellLocation) + new Vector3(0.5f, 0.5f, 0);
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
        deathScreen.SetActive(true);
        paused = true;
    }

    public static Vector3 AlignToGrid(Vector3 pos)
    {
        return actualGrid.CellToWorld(actualGrid.WorldToCell(pos)) + new Vector3(0.5f, 0.5f, 0);
    }

    void IncrementWind()
    {
        windCounter++;
        if (windCounter >= windCounterReset)
        {
            windCounter = 0;
            if(player.GetIsGhost()) return;
            // check if there is wind in the queue
            Queue<VariationType> varTypes = player.GetVariationTypesQueue();
            List<VariationType> varTypesList = varTypes.ToList();

            Vector3 pos = new(0,0,0);

            int tmp = Mathf.CeilToInt((float) varTypesList.Count(variant => variant == VariationType.WIND_UP) / (float) currentLevel.getWindSize() - 0.01f); // yay safety
            if(tmp > 0)
            {
                for(int i = 0; i < tmp; i++)
                {
                    pos = Move(player.transform.position, new Vector2Int(0, 1), out bool legalMove);
                    if (legalMove)
                        player.transform.position = pos;
                }
                
            }

            tmp = Mathf.CeilToInt((float) varTypesList.Count(variant => variant == VariationType.WIND_DOWN) / (float) currentLevel.getWindSize() - 0.01f);
            if(tmp > 0)
            {
                for(int i = 0; i < tmp; i++)
                {
                    pos = Move(player.transform.position, new Vector2Int(0, -1), out bool legalMove);
                    if (legalMove)
                        player.transform.position = pos;
                }
                
            }

            tmp = Mathf.CeilToInt((float) varTypesList.Count(variant => variant == VariationType.WIND_LEFT) / (float) currentLevel.getWindSize() - 0.01f);
            if(tmp > 0)
            {
                for(int i = 0; i < tmp; i++)
                {
                    pos = Move(player.transform.position, new Vector2Int(-1, 0), out bool legalMove);
                    if (legalMove)
                        player.transform.position = pos;
                }
                
            }

            tmp = Mathf.CeilToInt((float) varTypesList.Count(variant => variant == VariationType.WIND_RIGHT) / (float) currentLevel.getWindSize() - 0.01f);
            if(tmp > 0)
            {
                for(int i = 0; i < tmp; i++)
                {
                    pos = Move(player.transform.position, new Vector2Int(1, 0), out bool legalMove);
                    if (legalMove)
                        player.transform.position = pos;
                }
                
            }

        }
    }

}

