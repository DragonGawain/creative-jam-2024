using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    Inputs inputs;
    Vector2 movementInput;

    GameManager gameManager; // could just serialize this?

    bool canMove = true;

    // Start is called before the first frame update
    void Awake()
    {
        inputs = new Inputs();
        inputs.Player.Enable();
        inputs.Player.Move.performed += Move;
        inputs.Player.Move.canceled += EndMove;

        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Move(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (!canMove)
            return;

        movementInput = inputs.Player.Move.ReadValue<Vector2>();
        if (movementInput.x > 0)
        {
            transform.position = gameManager.Move(transform.position, new Vector2Int(1,0));
            // move right
        }
        else if (movementInput.x < 0)
        {
            transform.position = gameManager.Move(transform.position, new Vector2Int(-1,0));
            // move left
        }
        else if (movementInput.y > 0)
        {
            transform.position = gameManager.Move(transform.position, new Vector2Int(0,1));
            // move up
        }
        else if (movementInput.y < 0)
        {
            transform.position = gameManager.Move(transform.position, new Vector2Int(0,-1));
            // move down
        }

        canMove = false;

        gameManager.TriggerNextGameTick();

    }

    void EndMove(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (inputs.Player.Move.ReadValue<Vector2>().magnitude == 0)
            canMove = true;
    }

    void OnDestroy()
    {
        inputs.Player.Move.performed -= Move;
        inputs.Player.Move.canceled -= EndMove;
    }
}
