using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VariationType {
    MOVE_UP,
    MOVE_DOWN,
    MOVE_LEFT,
    MOVE_RIGHT,
    WIND_UP,
    WIND_DOWN,
    WIND_LEFT,
    WIND_RIGHT
}

public class PlayerController : MonoBehaviour
{

    Inputs inputs;
    Vector2 movementInput;

    GameManager gameManager; // could just serialize this?

    Queue variations = new();

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
        Debug.Log("perf");



        movementInput = inputs.Player.Move.ReadValue<Vector2>();
        if (movementInput.x > 0)
        {
            // move right
            transform.position = gameManager.Move(transform.position, new Vector2Int(1,0));
        }
        else if (movementInput.x < 0)
        {
            // move left
            transform.position = gameManager.Move(transform.position, new Vector2Int(-1,0));
        }
        else if (movementInput.y > 0)
        {
            // move up
            transform.position = gameManager.Move(transform.position, new Vector2Int(0,1));
        }
        else if (movementInput.y < 0)
        {
            // move down
            transform.position = gameManager.Move(transform.position, new Vector2Int(0,-1));
        }


        gameManager.TriggerNextGameTick();


    }

    void EndMove(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        Debug.Log("cance");
    }

    void OnDestroy()
    {
        inputs.Player.Move.performed -= Move;
        inputs.Player.Move.canceled -= EndMove;
    }
}
