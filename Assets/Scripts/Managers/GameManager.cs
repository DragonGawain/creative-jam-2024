using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static event Action NextGameTick;

    Grid grid;

    PlayerController player;

    public static int windCounter = 0;

    [SerializeField, Range(2, 5)]
    int windCounterReset = 3;

    // Start is called before the first frame update
    void Awake()
    {
        grid = GameObject.FindWithTag("Grid").GetComponent<Grid>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        NextGameTick += IncrementWind;
    }

    // Update is called once per frame
    void Update() { }

    public void TriggerNextGameTick()
    {
        NextGameTick?.Invoke();
    }

    public Vector3 Move(Vector3 pos, Vector2Int dir)
    {
        // CellToWorld retrns bottom-left corner
        return grid.CellToWorld(grid.WorldToCell(pos) + new Vector3Int(dir.x, dir.y, 0))
            + new Vector3(0.5f, 0.5f, 0);
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
