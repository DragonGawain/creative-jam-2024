using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using static UnityEngine.Tilemaps.Tile;

public abstract class ITile: TileBase
{
    protected ColliderType colliderType = ColliderType.Sprite;
    // TileBase Overrides
    // StartUp is called on the first frame of the running Scene.
    public abstract override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go);
    
    // initialize fields 
    // Sprite sprite1; above this method
    // use methods:
    // Sprite[] tilesAll = Resources.LoadAll<Sprite>("Sprites/TileSprites");
    // Sprite tileRedMine = Resources.Load<Sprite>("Sprites/RedMine");

    // return true
    
    // Retrieves any tile rendering data from the scripted tile.
    // TileData: https://docs.unity3d.com/ScriptReference/Tilemaps.TileData.html
    public abstract override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData);
    
    // change tileData.sprite to appropriate sprite depending on what has happened
    
    // This method is called when the tile is refreshed.
    public abstract override void RefreshTile(Vector3Int position, ITilemap tilemap);
    
    // don't really need this, do we?


    protected Dictionary<Directions, Vector3Int> directionDict = new Dictionary<Directions, Vector3Int>{
        {Directions.NORTH, new Vector3Int(0,1,0)},
        {Directions.EAST, new Vector3Int(1,0,0)},
        {Directions.SOUTH, new Vector3Int(0,-1,0)},
        {Directions.WEST, new Vector3Int(-1,0,0)}
    };
    // enums
    protected enum Directions
    {
        NORTH,
        EAST,
        SOUTH,
        WEST,
    }

}