using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    Variation variation;

    // [SerializeField, Range(1,5)] int size = 2;

    [SerializeField] VariationType type;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = GameManager.AlignToGrid(transform.position);
        variation = new(type);
    }

    // Update is called once per frame
    void Update()
    {
         
    }

    public Variation GetVariation()
    {
        return variation;
    }
}
