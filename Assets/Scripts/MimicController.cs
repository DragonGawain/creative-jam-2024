using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicController : MonoBehaviour
{
    // Start is called before the first frame update
    GameManager gameManager; // could just serialize this?
    PlayerController playerController;

    Vector2 movementInput;

    void Awake()
    {
        // subscribe to game manager event
        GameManager.NextGameTick += Move;
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        playerController = GameObject.FindWithTag("PlayerController").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // TODO:: gotta change this cause I changed the GameManager.Move() logic
    void Move() {
        movementInput = playerController.GetMovementInput();

        if (movementInput.x > 0)
        {
            // move right
            transform.position = GameManager.Move(transform.position, new Vector2Int(-1,0));
        }
        else if (movementInput.x < 0)
        {
            // move left
            transform.position = GameManager.Move(transform.position, new Vector2Int(1,0));
        }
        else if (movementInput.y > 0)
        {
            // move up
            transform.position = GameManager.Move(transform.position, new Vector2Int(0,-1));
        }
        else if (movementInput.y < 0)
        {
            // move down
            transform.position = GameManager.Move(transform.position, new Vector2Int(0,1));
        }
    }

    void OnDestroy()
    {
        GameManager.NextGameTick -= Move;
    }
}
