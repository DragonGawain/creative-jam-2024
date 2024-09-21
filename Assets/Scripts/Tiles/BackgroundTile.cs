using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class BackgroundTile : ITile
{
    // spritss
    Sprite currentSprite;
    Sprite lava;
    Sprite dark;
    Sprite[] walls;
    int nWalls = 10;

    public void Initialize()
    {
        Sprite[] tilesAll = Resources.LoadAll<Sprite>("BackgroundTiles");
        foreach(Sprite s in tilesAll)
        {
            for(int i = 0; i < nWalls; i++)
            {
               //
            }
        }

    }

    // TileBase overrides    // TileBase Overrides
    // StartUp is called on the first frame of the running Scene.
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) 
    {   
        // initialize fields 
        // Sprite sprite1; above this method
        // use methods:
        // Sprite[] tilesAll = Resources.LoadAll<Sprite>("Sprites/TileSprites");
        // Sprite tileRedMine = Resources.Load<Sprite>("Sprites/RedMine");

        return true;
    }
    // Retrieves any tile rendering data from the scripted tile.
    // TileData: https://docs.unity3d.com/ScriptReference/Tilemaps.TileData.html
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) 
    {
        // change tileData.sprite to appropriate sprite depending on what has happened
    }
    // This method is called when the tile is refreshed.
    public override void RefreshTile(Vector3Int position, ITilemap tilemap) 
    {
        // don't really need this, do we?
    }


}