using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class GroundTile: ITile
{
    // fields
    private int durability;
    private bool breakble;

    // spritss
    Sprite currentSprite;

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

    // why do you have a getter for a public variable?
    
    public bool isBreakable { return breakble; }
    
    public void decreaseDurability(int n)
    {
        // instead of destroying the tile, just set the tile in this location to be null, and make a tile in the 
        // "holes" tilemap in this location. Or, just keep it all in one tilemap and change an "isBroken" flag
        // to true or something like that..
        if(breakble) durability -= n;
        // if(durability <= 0) destroyTile();
    }

    public override void clearTile()
    {
        currentSprite = null;
        // refresh??
    }

}