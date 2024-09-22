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

    AudioManager audioManager;

    // Start is called before the first frame update
    void Awake()
    {
        inputs = new Inputs();
        inputs.Player.Enable();
        inputs.Player.Move.performed += Move;
        inputs.Player.GhostMode.performed += ToggleGhostMode;

        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        currentLevel = gameManager.getLevel();

        queueSize = currentLevel.getQueueSize();
        ghostCharges = currentLevel.getGhostCharges();

        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
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
        Vector3 movePos = transform.position;
        bool legalMove = false;
        // move right
        if (movementInput.x > 0 && variationTypes.Contains(VariationType.MOVE_RIGHT))
        {
            movePos = GameManager.Move(transform, new Vector2Int(1, 0), out legalMove);
        }
        // move left
        else if (movementInput.x < 0 && variationTypes.Contains(VariationType.MOVE_LEFT))
        {
            movePos = GameManager.Move(transform, new Vector2Int(-1, 0), out legalMove);
        }
        // move up
        else if (movementInput.y > 0 && variationTypes.Contains(VariationType.MOVE_UP))
        {
            movePos = GameManager.Move(transform, new Vector2Int(0, 1), out legalMove);
        }
        // move down
        else if (movementInput.y < 0 && variationTypes.Contains(VariationType.MOVE_DOWN))
        {
            movePos = GameManager.Move(transform, new Vector2Int(0, -1), out legalMove);
        }

        if (!legalMove)
            return;

        transform.position = movePos;

        if (ghostCharges <= 0)
            isGhost = false;

        gameManager.TriggerNextGameTick();
    }

    void ToggleGhostMode(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        bool oldIsGhost = isGhost;
        isGhost = !isGhost;
        if (ghostCharges <= 0)
            isGhost = false;

        // only change the audio stuff if ghost mode changed
        if (oldIsGhost != isGhost)
        {
            if (isGhost)
                audioManager.PlayGhostMusic();
            else
                audioManager.PlayNormalMusic();
        }
    }

    void OnDestroy()
    {
        inputs.Player.Move.performed -= Move;
        inputs.Player.GhostMode.performed -= ToggleGhostMode;
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

        // temp storage for things getting removed from the queue
        Variation vari;
        VariationType variT;

        // if the queue if overflowing, dequeue until the queue is no longer overfull
        if (variations.Count > queueSize)
            for (int i = variations.Count; i > queueSize; i--)
            {
                Dequeue(out vari, out variT);
                // All of that is to be able to easily check if the last piece of a mimic was removed
                if (variT == VariationType.MIMIC)
                {
                    GameManager.DespawnMimic(vari.GetMimicIndex());
                }
            }

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

    public void Dequeue(out Variation vari, out VariationType variT)
    {
        vari = variations.Dequeue();
        variT = variationTypes.Dequeue();
    }

    public bool GetIsGhost()
    {
        return isGhost;
    }

    public void DecrementGhostCharges(int n = 1)
    {
        ghostCharges -= n;
    }

    public void SetQueueSize(int q)
    {
        queueSize = q;
    }
}
