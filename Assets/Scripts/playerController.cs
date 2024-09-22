using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Inputs inputs;
    Vector2 movementInput;

    GameManager gameManager; // could just serialize this?
    Level currentLevel;

    // make queues static?
    Queue<Variation> variations = new();
    Queue<VariationType> variationTypes = new();

    // new Queue<VariationType>(
    //     new List<VariationType> { VariationType.MOVE_UP, VariationType.MOVE_RIGHT, VariationType.MOVE_DOWN, VariationType.MOVE_LEFT }
    // );

    [SerializeField, Range(2, 10)]
    int queueSize = 6;

    bool isGhost = false;

    int ghostCharges = 6;


    // Start is called before the first frame update
    void Awake()
    {
        inputs = new Inputs();
        inputs.Player.Enable();
        inputs.Player.Move.performed += Move;
        inputs.Player.Move.canceled += EndMove;

        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        currentLevel = gameManager.getLevel();

        queueSize = currentLevel.getQueueSize();
        ghostCharges = currentLevel.getGhostCharges();
        
    }

    // Update is called once per frame
    void Update() { }

    public Vector2 GetMovementInput()
    {
        return movementInput;
    }

    void Move(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        movementInput = inputs.Player.Move.ReadValue<Vector2>();
        Vector3 movePos = new(0,0,0);
        bool legalMove = true;
        // move right
        if (movementInput.x > 0 && variationTypes.Contains(VariationType.MOVE_RIGHT))
        {
            movePos = GameManager.Move(transform.position, new Vector2Int(1, 0), out legalMove);
        }
        // move left
        else if (movementInput.x < 0 && variationTypes.Contains(VariationType.MOVE_LEFT))
        {
            movePos = GameManager.Move(transform.position, new Vector2Int(-1, 0), out legalMove);
        }
        // move up
        else if (movementInput.y > 0 && variationTypes.Contains(VariationType.MOVE_UP))
        {
            movePos = GameManager.Move(transform.position, new Vector2Int(0, 1), out legalMove);
        }
        // move down
        else if (movementInput.y < 0 && variationTypes.Contains(VariationType.MOVE_DOWN))
        {
            movePos = GameManager.Move(transform.position, new Vector2Int(0, -1), out legalMove);
        }
        // else
        // {
        //     bool legalMove = true; // sometimes I hate compilers..
        // }


        if (!legalMove)
            return;
        
        transform.position = movePos;

        gameManager.TriggerNextGameTick();
    }

    void EndMove(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
    }

    void OnDestroy()
    {
        inputs.Player.Move.performed -= Move;
        inputs.Player.Move.canceled -= EndMove;
    }

    public void Enqueue(Variation variation)
    {
        for (int i = 0; i < variation.GetSize(); i++)
        {
            variations.Enqueue(variation);
            if (i == variation.GetSize() - 1) // last one is the head
                variationTypes.Enqueue(variation.GetVariationType());
            else
                variationTypes.Enqueue(Variation.GetAlternateTypes()[variation.GetVariationType()]);
        }

        // if the queue if overflowing, dequeue until the queue is no longer overfull
        if (variations.Count > queueSize)
            for (int i = variations.Count; i > queueSize; i--)
                Dequeue();

        // After we're done with the queue operations, let's manipulate the visuals!
        UIManager.UpdateQueueVisuals(variationTypes.ToArray());
    }

    public Queue<Variation> GetVariationQueue()
    {
        return variations;
    }

    public Queue<VariationType> GetVariationTypesQueue()
    {
        return variationTypes;
    }

    public void Dequeue(int n = 1)
    {
        for (int i = 0; i < n; i++)
        {
            variations.Dequeue();
            variationTypes.Dequeue();
        }
    }

    public bool GetIsGhost()
    {
        return isGhost;
    }

    public void DecrementGhostCharges(int n = 1)
    {
        ghostCharges -= n;
    }

    // TEMP DEBUG METHODS (though like, we can just, use these I guess...)

    public void EnqueueUp()
    {
        Enqueue(new Variation(VariationType.MOVE_UP));
    }

    public void EnqueueRight()
    {
        Enqueue(new Variation(VariationType.MOVE_RIGHT));
    }

    public void EnqueueDown()
    {
        Enqueue(new Variation(VariationType.MOVE_DOWN));
    }

    public void EnqueueLeft()
    {
        Enqueue(new Variation(VariationType.MOVE_LEFT));
    }

    public void EnqueueWindUp()
    {
        Enqueue(new Variation(VariationType.WIND_UP));
    }

    public void EnqueueWindRight()
    {
        Enqueue(new Variation(VariationType.WIND_RIGHT));
    }

    public void EnqueueWindDown()
    {
        Enqueue(new Variation(VariationType.WIND_DOWN));
    }

    public void EnqueueWindLeft()
    {
        Enqueue(new Variation(VariationType.WIND_LEFT));
    }

    public void EnqueueMimic()
    {
        Enqueue(new Variation(VariationType.MIMIC));
    }
}
