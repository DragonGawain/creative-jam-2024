using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[CreateAssetMenu]
public class ItemTile : ITile
{
    Sprite currentSprite;
    Sprite move_d,
        move_u,
        move_l,
        move_r;
    Sprite wind_d,
        wind_u,
        wind_l,
        wind_r;
    Sprite mimic;
    Sprite boots;

    Variation vt;

    int mimicIndex = 0;

    static int mimicIndexTracker = 0;

    public void Initialize()
    {
        Sprite[] tilesAll = Resources.LoadAll<Sprite>("QueueIcons");
        foreach (Sprite s in tilesAll)
        {
            switch (s.name)
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

    public void SetSprite(string name, int size)
    {
        switch (name)
        {
            case "wind_u":
                currentSprite = wind_u;
                vt = new Variation(VariationType.WIND_UP, size);
                break;
            case "wind_d":
                currentSprite = wind_d;
                vt = new Variation(VariationType.WIND_DOWN, size);
                break;
            case "wind_l":
                currentSprite = wind_l;
                vt = new Variation(VariationType.WIND_LEFT, size);
                break;
            case "wind_r":
                currentSprite = wind_r;
                vt = new Variation(VariationType.WIND_RIGHT, size);
                break;
            case "move_u":
                currentSprite = move_u;
                vt = new Variation(VariationType.MOVE_UP, size);
                break;
            case "move_d":
                currentSprite = move_d;
                vt = new Variation(VariationType.MOVE_DOWN, size);
                break;
            case "move_l":
                currentSprite = move_l;
                vt = new Variation(VariationType.MOVE_LEFT, size);
                break;
            case "move_r":
                currentSprite = move_r;
                vt = new Variation(VariationType.MOVE_RIGHT, size);
                break;
            case "mimic_head":
                currentSprite = mimic;
                vt = new Variation(VariationType.MIMIC, size);
                mimicIndex = mimicIndexTracker;
                mimicIndexTracker++;
                break;
            case "boots":
                currentSprite = boots;
                vt = new Variation(VariationType.BOOTS, size);
                break;
        }
    }

    // called on first frame
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
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

    public Variation GetVariation()
    {
        return vt;
    }

    public int GetMimicIndex()
    {
        return mimicIndex;
    }
}
