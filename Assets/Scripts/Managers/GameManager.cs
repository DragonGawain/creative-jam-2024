using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static event Action NextGameTick;

    // grids & tilemaps
    // where the levels are displayed

    static Grid actualGrid;
    static Tilemap levelItemsActual;
    static Tilemap levelGroundActual;
    static Tilemap levelBackgroundActual;

    // level blueprints
    static Grid levelGrid;
    static Tilemap levelItemsBlueprint;
    static Tilemap levelGroundBlueprint;
    static Tilemap levelBackgroundBlueprint;

    public static int nWalls = 24;


    static GameObject deathScreen;

    static bool paused = false;

    static PlayerController player;

    public static int windCounter = 0;

    [SerializeField, Range(2, 5)]
    int windCounterReset = 3;

    static Dictionary<Vector3Int, Item> activeItems = new();

    static int walkingDamage = 1;



    // Start is called before the first frame update
    void Awake()
    {
        actualGrid = GameObject.Find("Actual_Grid").GetComponent<Grid>();
        levelItemsActual = actualGrid.transform.Find("Actual_Items").GetComponent<Tilemap>(); 
        levelGroundActual = actualGrid.transform.Find("Actual_Ground").GetComponent<Tilemap>();
        levelBackgroundActual = actualGrid.transform.Find("Actual_Background").GetComponent<Tilemap>();


        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
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

        activeItems.Clear();

        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            activeItems.Add(actualGrid.WorldToCell(item.transform.position), item.GetComponent<Item>());
        }

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
        loadItems();
    }

    private static void updateTilemaps(int levelNb)
    {
        string levelGridName = "Level" + levelNb;
        levelGrid = GameObject.Find(levelGridName).GetComponent<Grid>();
        levelGroundBlueprint = levelGrid.transform.Find(levelGridName + "_Ground").GetComponent<Tilemap>();
        levelBackgroundBlueprint = levelGrid.transform.Find(levelGridName + "_Background").GetComponent<Tilemap>();
        levelItemsBlueprint = levelGrid.transform.Find(levelGridName + "_Items").GetComponent<Tilemap>();
    }

    private static void loadGround()
    {
        int durability;
        bool breakable;
        GroundTile.GroundTileType gtt;
        

        foreach(Vector3Int loc in levelGroundBlueprint.cellBounds.allPositionsWithin)
        {
            durability = 1;
            breakable = false;
            gtt = GroundTile.GroundTileType.NULL;

            GroundTile gt = ScriptableObject.CreateInstance<GroundTile>();
            gt.Initialize();
            
            TileBase tb = levelGroundBlueprint.GetTile(loc);
            
            if(!tb) continue;

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
            BackgroundTile bgt = ScriptableObject.CreateInstance<BackgroundTile>();
            bgt.Initialize();
            TileBase tb = levelBackgroundBlueprint.GetTile(loc);

            if(!tb) continue;

            levelBackgroundActual.SetTile(loc, bgt);
            ((BackgroundTile) levelBackgroundActual.GetTile(loc)).SetSprite(tb.name);

            levelBackgroundActual.RefreshTile(loc);
        }
       
    }

    private static void loadItems()
    {
        foreach(Vector3Int loc in levelItemsBlueprint.cellBounds.allPositionsWithin)
        {
            ItemTile itemTile = ScriptableObject.CreateInstance<ItemTile>();
            itemTile.Initialize();
            TileBase tb = levelItemsBlueprint.GetTile(loc);

            if(!tb) continue;

            levelItemsActual.SetTile(loc, itemTile);
            ((ItemTile) levelItemsActual.GetTile(loc)).SetSprite(tb.name);

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



        if (activeItems.ContainsKey(newCellLocation))
        {
            player.Enqueue(activeItems[newCellLocation].GetVariation());
            Destroy(activeItems[newCellLocation].gameObject);
            activeItems.Remove(newCellLocation);
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
            // check if there is wind in the queue
            Queue<VariationType> varTypes = player.GetVariationTypesQueue();

            Vector3 pos = new(0,0,0);

            if (varTypes.Contains(VariationType.WIND_UP))
                {
                    pos = Move(player.transform.position, new Vector2Int(0, 1), out bool legalMove);
                    if (legalMove)
                        player.transform.position = pos;

                    // MIMIC HAS TO MOVE!!!!
                }
            if (varTypes.Contains(VariationType.WIND_DOWN))
                {
                    pos = Move(player.transform.position, new Vector2Int(0, -1), out bool legalMove);
                    if (legalMove)
                        player.transform.position = pos;

                    // MIMIC HAS TO MOVE!!!!
                }
            if (varTypes.Contains(VariationType.WIND_LEFT))
                {
                    pos = Move(player.transform.position, new Vector2Int(-1, 0), out bool legalMove);
                    if (legalMove)
                        player.transform.position = pos;

                    // MIMIC HAS TO MOVE!!!!
                }
            if (varTypes.Contains(VariationType.WIND_RIGHT))
                {
                    pos = Move(player.transform.position, new Vector2Int(1, 0), out bool legalMove);
                    if (legalMove)
                        player.transform.position = pos;

                    // MIMIC HAS TO MOVE!!!!
                }


        }
    }

}
