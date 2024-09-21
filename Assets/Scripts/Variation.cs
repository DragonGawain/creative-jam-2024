using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VariationType
{
    NULL,
    MOVE_UP,
    MOVE_DOWN,
    MOVE_LEFT,
    MOVE_RIGHT,
    WIND_UP,
    WIND_DOWN,
    WIND_LEFT,
    WIND_RIGHT
}

// I don't think this needs to be a monobehaviour? It not being a monobehaviour would mean
// that any object we want to have this data struct would need a different script that is a
// monobehaviour to hold an instance of a Variation.. :shrug:
public class Variation
{

    static Dictionary<VariationType, int> typeSizes = new Dictionary<VariationType, int>() {
        {VariationType.MOVE_UP, 1},
        {VariationType.MOVE_DOWN, 1},
        {VariationType.MOVE_LEFT, 1},
        {VariationType.MOVE_RIGHT, 1},
        {VariationType.WIND_UP, 1},
        {VariationType.WIND_DOWN, 1},
        {VariationType.WIND_LEFT, 1},
        {VariationType.WIND_RIGHT, 1},
    };


    VariationType variationType;
    int size;

    static Dictionary<int, Sprite> sprites = new();

    public static void InitializeVariationSprites()
    {
        // fill the sprite dictionary with the sprites, along with a key to identify what the sprite is
        // ex: move up HEAD, wind left HEAD, wind ledt BODY (for extending the lenght of the thing)
    }


    public Variation(VariationType type)
    {
        this.variationType = type;
        this.size = typeSizes[type];
    }

    // we shouldn't really be using this one
    // public Variation(VariationType type, int size)
    // {
    //     this.variationType = type;
    //     this.size = size;
    // }

    public Variation(Variation variation)
    {
        this.variationType = variation.GetVariationType();
        this.size = variation.GetSize();
    }

    public VariationType GetVariationType()
    {
        return variationType;
    }

    public int GetSize()
    {
        return size;
    }

    public void SetVariationType(VariationType type)
    {
        variationType = type;
    }

    public void SetSize(int size)
    {
        this.size = size;
    }


    
}
