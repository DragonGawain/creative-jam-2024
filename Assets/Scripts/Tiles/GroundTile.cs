using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[CreateAssetMenu]
public class GroundTile: ITile
{
    // fields
    int durability = 0;
    bool breakable = false;
    GroundTileType groundTileType = GroundTileType.NULL;
    bool flipX = false;
    bool flipY = false;
    int rotationAngle = 0;
    bool open = false;

    // spritss
    Sprite currentSprite;
    Sprite[][] rockTiles;
    Sprite crystalTile;
    public void SetValues(int durability, bool breakable, GroundTileType gtt)
    {
        Debug.Log("I'm setting values!");
        this.durability = durability;
        this.breakable = breakable;
        this.groundTileType = gtt;
    }

    public void Initialize()
    {
        // initialize fields 
        // Sprite sprite1; above this method
        // use methods:
        // Sprite[] tilesAll = Resources.LoadAll<Sprite>("Sprites/TileSprites");
        // Sprite tileRedMine = Resources.Load<Sprite>("Sprites/RedMine");

        Sprite[] tilesAll = Resources.LoadAll<Sprite>("GroundTiles");
        Debug.Log("Tiles All: " + tilesAll);

        rockTiles = new Sprite[4][]; // [index = normal, x flip, y flip, xy flip] [actual tile according to durability]
        for(int i = 0; i < 4; i++)
        {
            rockTiles[i] = new Sprite[4];
        }

        foreach(Sprite s in tilesAll)
        {   
            //Debug.Log("I am placing Sprite " + s.name);
            groundTileType = GroundTileType.NULL;

            for(int i = 0; i < tilesAll.Length; i++)
            {
                if(s.name.Equals("Rock" + i))
                {
                    rockTiles[0][i] = s;
                }
                else if(s.name.Equals("Rock" + i + "_flip_X"))
                {
                    rockTiles[1][i] = s;
                }
                else if(s.name.Equals("Rock" + i + "_flip_Y"))
                {
                    rockTiles[2][i] = s;
                }
                else if(s.name.Equals("Rock" + i + "_flip_XY"))
                {
                    rockTiles[3][i] = s;
                }
            }

            switch(s.name)
            {
                case "Crystal":
                    crystalTile = s;
                    break;
                
            }

            //Debug.Log("The Tile at " + position + " has name : " + s.name);
            
        }

        flipX = Random.Range(0,1) <= 0.5f? true: false;
        flipY = Random.Range(0,1) <= 0.5f? true: false;
        rotationAngle = 90 * Mathf.FloorToInt(Random.Range(0, 3.99999f));    
    }

    // TileBase overrides    // TileBase Overrides
    // StartUp is called on the first frame of the running Scene.
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) 
    {   
        return true;
    }
    // Retrieves any tile rendering data from the scripted tile.
    // TileData: https://docs.unity3d.com/ScriptReference/Tilemaps.TileData.html
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) 
    {
        Debug.Log(groundTileType);
        // choose currentSprite
        switch(groundTileType)
        {
            case GroundTileType.ROCK:
                int index = 0;
                if(flipX && flipY) index = 3;
                else if(flipX) index = 1;
                else if(flipY) index = 2;

                Debug.Log(rockTiles);
                if(this.durability > 0) currentSprite = rockTiles[index][this.durability - 1];
                break;
            case GroundTileType.CRYSTAL:
                currentSprite = crystalTile;
                break;
            default:
                currentSprite = null;
                break;
        }

        string name = currentSprite? currentSprite.name : "Null";
        Debug.Log("Current Sprite Name: " + name + " at " + position);


        tileData.transform.SetTRS(position, Quaternion.Euler(0, 0, rotationAngle) , Vector3.one);
        // change tileData.sprite to appropriate sprite depending on what has happened

        tileData.sprite = currentSprite;

    }
    // This method is called when the tile is refreshed.
    public override void RefreshTile(Vector3Int position, ITilemap tilemap) 
    {
        // don't really need this, do we?
    }
    
    public bool isBreakable() { return breakable; }
    
    public int decreaseDurability(int n)
    {
        // instead of destroying the tile, just set the tile in this location to be null, and make a tile in the 
        // "holes" tilemap in this location. Or, just keep it all in one tilemap and change an "isBroken" flag
        // to true or something like that..
        if(breakable) durability -= n;
        return durability;
    }

    public enum GroundTileType
    {
        ROCK,
        TRAP,
        CRYSTAL,
        NULL
    }


}