using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VariationType
{
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
    VariationType variationType;
    int size;

    public Variation(VariationType type)
    {
        this.variationType = type;
        this.size = 1;
    }

    public Variation(VariationType type, int size)
    {
        this.variationType = type;
        this.size = size;
    }

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
