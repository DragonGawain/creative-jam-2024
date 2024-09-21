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
    static Tilemap levelGroundActual;
    static Tilemap levelBackgroundActual;

    // level blueprints
    static Grid levelGrid;
    static Tilemap levelGroundBlueprint;
    static Tilemap levelBackgroundBlueprint;



    static GameObject deathScreen;

    static bool paused = false;

    static PlayerController player;

    public static int windCounter = 0;

    [SerializeField, Range(2, 5)]
    int windCounterReset = 3;

    static Dictionary<Vector3Int, Item> activeItems = new();



    // Start is called before the first frame update
    void Awake()
    {
        actualGrid = GameObject.FindWithTag("Acutal_Grid").GetComponent<Grid>();
        levelGroundActual = actualGrid.transform.Find("Actual_Level").GetComponent<Tilemap>();
        levelBackgroundActual = actualGrid.transform.Find("Actual_Background").GetComponent<Tilemap>();


        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        deathScreen = GameObject.FindGameObjectWithTag("Death");

        /*
            Death screen is active by default and hidden on wake
            to be able to fetch a reference to it
        */
        deathScreen.SetActive(false);

        NextGameTick += IncrementWind;

        Variation.InitializeVariationSprites();

    }

    private void Start()
    {
        StartNewLevel(0);
    }

    // Update is called once per frame
    void Update() { }

    public static void StartNewLevel(int levelNb)
    {

        int durability;
        bool breakable;
        GroundTile.GroundTileType gtt = GroundTile.GroundTileType.NULL;
        // level loading logic...
        // select the appropriate grid/tilemap
        // foreach(Vector3Int loc in tilemap.
        foreach(Vector3Int loc in levelGroundBlueprint.cellBounds.allPositionsWithin)
        {
            durability = 0;
            breakable = false;
            gtt = GroundTile.GroundTileType.NULL;

            GroundTile gt = ScriptableObject.CreateInstance<GroundTile>();
            gt.Initialize();
            
            TileBase tb = levelGroundBlueprint.GetTile(loc);
            
            if(!tb) continue;

            // check if rock
            for(int i = 0; i <= 3; i++)
            {
                if(tb.name[..5].Equals("Rock" + i))
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
                case "Indestructible":
                    gtt = GroundTile.GroundTileType.END;
                    break;
            }

            if(tb.name.Equals("Crystal"))
            {
                gtt = GroundTile.GroundTileType.CRYSTAL;
            }

            Debug.Log("In Game Manager, tile type is: " + gtt + "\n and tb.name = " + tb.name);
            levelGroundActual.SetTile(loc, gt);

            ((GroundTile) levelGroundActual.GetTile(loc)).SetValues(durability, breakable, gtt);

            levelGroundActual.RefreshTile(loc);

        }

        activeItems.Clear();

        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            activeItems.Add(actualGrid.WorldToCell(item.transform.position), item.GetComponent<Item>());
        }
    }

    private static void updateTilemaps(int levelNb)
    {
        updateTilemaps(levelNb);
        string levelGridName = "Level" + levelNb;
        levelGrid = GameObject.Find(levelGridName).GetComponent<Grid>();
        levelGroundBlueprint = levelGrid.transform.Find(levelGridName + "_Ground").GetComponent<Tilemap>();
        levelBackgroundBlueprint = levelGrid.transform.Find(levelGridName + "_Background").GetComponent<Tilemap>();

        levelGroundActual.ClearAllTiles();
        levelGroundActual.RefreshAllTiles();

        levelBackgroundActual.ClearAllTiles();
        levelBackgroundActual.RefreshAllTiles();
    }

    public void TriggerNextGameTick()
    {
        NextGameTick?.Invoke();
    }

    public static Vector3 Move(Vector3 pos, Vector2Int dir)
    {
        // Vector3Int cellLoc = grid.WorldToCell(pos);
        // int hp = (WorldTile)tilemap.GetTile(cellLoc).decreaseDurability();
        // if (hp <= 0)
        //     destroy(tile);

        // CellToWorld retrns bottom-left corner

        Vector3Int newCellLocation = actualGrid.WorldToCell(pos) + new Vector3Int(dir.x, dir.y, 0);

        // Stop player from moving when paused
        if (paused)
            return pos;

        // Whenever a player steps on lava, they Die™
        // Debug.Log("Current level tile: " + newCellLocation);
        Debug.Log("Current level tile: " + levelGroundActual.GetTile(new Vector3Int(0, 0)));
        if (levelGroundActual.GetTile(newCellLocation) == null)
            Die();

        if (activeItems.ContainsKey(newCellLocation))
        {
            player.Enqueue(activeItems[newCellLocation].GetVariation());
            Destroy(activeItems[newCellLocation].gameObject);
            activeItems.Remove(newCellLocation);
        }

        return actualGrid.CellToWorld(newCellLocation) + new Vector3(0.5f, 0.5f, 0);
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

            if (varTypes.Contains(VariationType.WIND_UP))
                player.transform.position = Move(player.transform.position, new Vector2Int(0, 1));
            if (varTypes.Contains(VariationType.WIND_DOWN))
                player.transform.position = Move(player.transform.position, new Vector2Int(0, -1));
            if (varTypes.Contains(VariationType.WIND_LEFT))
                player.transform.position = Move(player.transform.position, new Vector2Int(-1, 0));
            if (varTypes.Contains(VariationType.WIND_RIGHT))
                player.transform.position = Move(player.transform.position, new Vector2Int(1, 0));
        }
    }

}
