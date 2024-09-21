using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    GameObject queue;
    // Start is called before the first frame update
    void Awake()
    {
        queue = GameObject.FindWithTag("Queue");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void UpdateQueueVisuals(Variation[] items)
    {
        VariationType lastVariation = VariationType.NULL;
        // set to 6 for now cause that's the length of the queue. 
        // Can be changed later :shrug:
        for (int i = 0; i < 6; i++)
        {
            if (lastVariation != VariationType.NULL || lastVariation != items[i].GetVariationType())
            {
                // queue.GetChild(i).getComponent<Image>().Sprite = 
                // set the sprite to the HEAD version of this type
                lastVariation = items[i].GetVariationType();
            }
            // Shouldn't need to have the IF, but I'm including in to be safe
            else if (lastVariation == items[i].GetVariationType())
            {
                // set the sprite to the BODY version of this type
            }
        }
    }
}
