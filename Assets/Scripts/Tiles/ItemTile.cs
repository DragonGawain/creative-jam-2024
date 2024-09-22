using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[CreateAssetMenu]
public class ItemTile: ITile
{
    Sprite currentSprite;
    Sprite move_d, move_u, move_l, move_r;
    Sprite wind_d, wind_u, wind_l, wind_r;
    Sprite mimic;
    Sprite boots;

    public void Initialize()
    {
        Sprite[] tilesAll = Resources.LoadAll<Sprite>("QueueIcons");
        foreach(Sprite s in tilesAll)
        {
            switch(s.name)
            {
                case "wind_u":
                    wind_u = s;
                    break;
                case "wind_d":
                    wind_d = s;
                    break;
                case "wind_l":
                    wind_l = s;
                    break;
                case "wind_r":
                    wind_r = s;
                    break;
                case "move_u":
                    move_u = s;
                    break;
                case "move_d":
                    move_d = s;
                    break;
                case "move_l":
                    move_l = s;
                    break;
                case "move_r":
                    move_r = s;
                    break;
                case "mimic_head":
                    mimic = s;
                    break;
                case "boots":
                    boots = s;
                    break;
            }
        }

    }

    public void SetSprite(string name)
    {
        switch(name)
        {
            case "wind_u":
                currentSprite = wind_u;    
                break;
            case "wind_d":
                currentSprite = wind_d;
                break;
            case "wind_l":
                currentSprite = wind_l;
                break;
            case "wind_r":
                currentSprite = wind_r;
                break;
            case "move_u":
                currentSprite = move_u;
                break;
            case "move_d":
                currentSprite = move_d;
                break;
            case "move_l":
                currentSprite = move_l;
                break;
            case "move_r":
                currentSprite = move_r;
                break;
            case "mimic_head":
                currentSprite= mimic;
                break;
            case "boots":
                currentSprite=  boots;
                break;
        }

    }

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


    public enum ItemType
    {
        MOVE_D,
        MOVE_U,
        MOVE_L,
        MOVE_R,
        WIND_D,
        WIND_U,
        WIND_L,
        WIND_R,
        MIMIC,
        BOOTS,
        NULL
    }
}