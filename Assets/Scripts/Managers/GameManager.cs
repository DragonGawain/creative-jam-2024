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

    // Start is called before the first frame update
    void Awake()
    {
        grid = GameObject.FindWithTag("Grid").GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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


}
