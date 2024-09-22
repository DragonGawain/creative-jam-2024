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

    int mimicIndex;

    bool firstTurn = true;

    Animator animator; 

    bool isMimicGhost = false;

    int forceDelay = 25;


    void Awake()
    {
        // snap to grid
        transform.position = GameManager.AlignToGrid(transform.position);
        // subscribe to game manager event
        GameManager.NextGameTick += Move;
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        animator = gameObject.GetComponent<Animator>();
    }

    public Animator GetMimicAnimator()
    {
        return animator;
    }

    // Update is called once per frame
    // void Update() { 

    //     if (forceDelay >= 0)
    //         forceDelay--;
    //     if (playerController.GetIsGhost() != isMimicGhost && forceDelay <= 0)
    //     {
    //         forceDelay = 25;
    //         if (playerController.GetIsGhost())
    //             animator.SetTrigger("GS"); 
    //         else
    //             animator.SetTrigger("RGS");
    //         isMimicGhost = playerController.GetIsGhost();
    //     }

    // }

    // TODO:: gotta change this cause I changed the GameManager.Move() logic
    void Move()
    {
        if (firstTurn)
        {
            firstTurn = false;
            return;
        }
        // movementInput = playerController.GetMovementInput();

        Vector3 pos = new(0, 0, 0);
        bool legalMove = true;

        // move right
        if (playerController.GetLastMove().x > 0)
        {
            GameManager.Move(transform, new Vector2Int(-1, 0), out legalMove);
            if (legalMove)
            {
                transform.localScale = new Vector3(-1,1,1);
                animator.SetTrigger("walk_left");
            }
        }
        // move left
        else if (playerController.GetLastMove().x < 0)
        {
            GameManager.Move(transform, new Vector2Int(1, 0), out legalMove);
            if (legalMove)
            {
                transform.localScale = new Vector3(1,1,1);
                animator.SetTrigger("walk_right");
            }
        }
        // move up
        else if (playerController.GetLastMove().y > 0)
        {
            GameManager.Move(transform, new Vector2Int(0, -1), out legalMove);
            if (legalMove)
                animator.SetTrigger("walk_down");
        }
        // move down
        else if (playerController.GetLastMove().y < 0)
        {
            GameManager.Move(transform, new Vector2Int(0, 1), out legalMove);
            if (legalMove)
                animator.SetTrigger("walk_up");
        }

        // if (legalMove)
        //     transform.position = pos;
    }

    void OnDestroy()
    {
        GameManager.NextGameTick -= Move;
    }

    public void SetMimicIndex(int n)
    {
        mimicIndex = n;
    }

    public int GetMimicIndex()
    {
        return mimicIndex;
    }
}
