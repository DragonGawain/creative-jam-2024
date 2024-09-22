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

        Vector3 pos = new(0,0,0);
        bool legalMove = true;

        // move right
        if (movementInput.x > 0)
        {
            pos = GameManager.Move(transform.position, new Vector2Int(-1,0), out legalMove);
        }
        // move left
        else if (movementInput.x < 0)
        {
            pos = GameManager.Move(transform.position, new Vector2Int(1,0), out legalMove);
        }
        // move up
        else if (movementInput.y > 0)
        {
            pos = GameManager.Move(transform.position, new Vector2Int(0,-1), out legalMove);
        }
        // move down
        else if (movementInput.y < 0)
        {
            pos = GameManager.Move(transform.position, new Vector2Int(0,1), out legalMove);
        }
   

        if (legalMove)
            transform.position = pos;
    }

    void OnDestroy()
    {
        GameManager.NextGameTick -= Move;
    }
}
