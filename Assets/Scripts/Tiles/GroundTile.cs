using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class GroundTile: ITile
{
    // fields
    int durability;
    bool breakble;
    GroundTileType groundTileType;
    bool flipX = false;
    bool flipY = false;
    int rotationAngle = 0;

    // spritss
    Sprite currentSprite;
    Sprite[] rockTiles;
    int nRockTiles = 5;

    Sprite crystalTile;

    // TileBase overrides    // TileBase Overrides
    // StartUp is called on the first frame of the running Scene.
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) 
    {   
        // initialize fields 
        // Sprite sprite1; above this method
        // use methods:
        // Sprite[] tilesAll = Resources.LoadAll<Sprite>("Sprites/TileSprites");
        // Sprite tileRedMine = Resources.Load<Sprite>("Sprites/RedMine");

        Sprite[] tilesAll = Resources.LoadAll<Sprite>("Sprites/Ground");
        Sprite[][] rockTiles = new Sprite[4][];

        foreach(Sprite s in tilesAll)
        {
            for(int i = 0; i < nRockTiles; i++)
            {
                if(s.name.Equals("Rock" + i))
                {
                    rockTiles[0][i] = s;
                    groundTileType = GroundTileType.ROCK;
                }
                else if(s.name.Equals("Rock" + i + "_flip_X"))
                {
                    rockTiles[1][i] = s;
                    groundTileType = GroundTileType.CRYSTAL;
                }
                else if(s.name.Equals("Rock" + i + "_flip_Y"))
                {
                    rockTiles[2][i] = s;
                    groundTileType = GroundTileType.ROCK;
                }
                else if(s.name.Equals("Rock" + i + "_flip_XY"))
                {
                    rockTiles[3][i] = s;
                    groundTileType = GroundTileType.ROCK;
                }
            }

            switch(s.name)
            {
                case "Crystal":
                    crystalTile = s;
                    break;
                
            }
            
        }

        return true;
    }
    // Retrieves any tile rendering data from the scripted tile.
    // TileData: https://docs.unity3d.com/ScriptReference/Tilemaps.TileData.html
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) 
    {

        // choose currentSprite
        switch(groundTileType)
        {
            case GroundTileType.ROCK:
                currentSprite = rockTiles[this.durability];
                break;
            case GroundTileType.CRYSTAL:
                currentSprite = crystalTile;
                break;
        }




        // change tileData.sprite to appropriate sprite depending on what has happened
        tileData.sprite = currentSprite;
    }
    // This method is called when the tile is refreshed.
    public override void RefreshTile(Vector3Int position, ITilemap tilemap) 
    {
        // don't really need this, do we?
    }
    
    public bool isBreakable() { return breakble; }
    
    public int decreaseDurability(int n)
    {
        // instead of destroying the tile, just set the tile in this location to be null, and make a tile in the 
        // "holes" tilemap in this location. Or, just keep it all in one tilemap and change an "isBroken" flag
        // to true or something like that..
        if(breakble) durability -= n;
        return durability;
    }

    private enum GroundTileType
    {
        ROCK,
        TRAP,
        CRYSTAL
    }


}