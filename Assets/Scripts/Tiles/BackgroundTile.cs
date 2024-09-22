using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class BackgroundTile : ITile
{
    // spritss
    Sprite currentSprite;
    Sprite lava;
    Sprite[] walls;
    bool isWall = false;

    public void Initialize()
    {
        walls = new Sprite[GameManager.nWalls];
        Sprite[] tilesAll = Resources.LoadAll<Sprite>("BackgroundTiles");
        foreach(Sprite s in tilesAll)
        {
            for(int i = 0; i < GameManager.nWalls; i++)
            {
               if(s.name.Equals("wall" + i))
               {
                    walls[i] = s;
                    isWall = true;
               }
            }
            switch(s.name)
            {
                case "Lava":
                    lava = s;
                    break;

            }
        }

    }

    public void SetSprite(string name)
    {
        for(int i = 0; i < GameManager.nWalls; i++)
        {
            if(name.Equals("wall" + i))
            {
                currentSprite = walls[i];
                isWall = true;
            }

            switch(name)
            {
                case "Lava":
                    currentSprite = lava;
                    break;

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
        tileData.sprite = currentSprite;
    }
    // This method is called when the tile is refreshed.
    public override void RefreshTile(Vector3Int position, ITilemap tilemap) 
    {
        // don't really need this, do we?
    }


}