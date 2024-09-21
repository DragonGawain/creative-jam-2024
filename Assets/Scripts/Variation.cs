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

    // Start is called before the first frame update
    void Start()
    {
        //
    }

    // Update is called once per frame
    void Update()
    {
        //
    }

    public VariationType GetVariationType()
    {
        return variationType;
    }

    public void SetVariationType(VariationType type)
    {
        variationType = type;
    }

    public Variation(VariationType type)
    {
        this.variationType = type;
    }
}
