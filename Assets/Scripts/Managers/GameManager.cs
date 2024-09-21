using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static event Action NextGameTick;

    static Grid grid;

    static PlayerController player;

    public static int windCounter = 0;

    [SerializeField, Range(2, 5)]
    int windCounterReset = 3;

    static Dictionary<Vector3Int, Item> activeItems = new();

    // Start is called before the first frame update
    void Awake()
    {
        grid = GameObject.FindWithTag("Grid").GetComponent<Grid>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
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
        // some logic that enables the level and disables the others

        activeItems.Clear();

        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            activeItems.Add(grid.WorldToCell(item.transform.position), item.GetComponent<Item>());
        }
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

        Vector3Int newCellLocation = grid.WorldToCell(pos) + new Vector3Int(dir.x, dir.y, 0);

        if (activeItems.ContainsKey(newCellLocation))
        {
            player.Enqueue(activeItems[newCellLocation].GetVariation());
            Destroy(activeItems[newCellLocation].gameObject);
            activeItems.Remove(newCellLocation);
        }

        return grid.CellToWorld(newCellLocation) + new Vector3(0.5f, 0.5f, 0);
    }

    public static Vector3 AlignToGrid(Vector3 pos)
    {
        return grid.CellToWorld(grid.WorldToCell(pos)) + new Vector3(0.5f, 0.5f, 0);
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
