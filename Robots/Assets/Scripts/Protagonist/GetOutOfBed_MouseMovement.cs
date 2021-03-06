﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO change this to an IControllable interface, because there will be lots of Roan scripts
// that we will need to tell when they can be controlled by the player
public enum GameState { WAKING_UP, GAME, WON }

// which point of view are we seeing Roan and Lumi's bed from?
public enum POV { TOP_VIEW, SIDE_VIEW }

public class GetOutOfBed_MouseMovement : MonoBehaviour
{
    // when the player wins, these events are called
    public delegate void Won();
    public static event Won OnWon;

    [SerializeField] GameObject topView, sideView;
    [SerializeField] Animator roanAnimTop, roanAnimSide, lumiAnimTop, lumiAnimSide;
    [SerializeField] float maxNoiseLevel = 1, movementNoise = .05f, noiseDecreaseRate = .5f;

    Animator roanAnim, lumiAnim;
    GameState currentGameState = GameState.WAKING_UP;
    POV currentPOV = POV.TOP_VIEW;
    float mouseMovement, noiseLevel;
    int jostles;
    bool canJostle = true;

    private void Start()
    {
        roanAnim = roanAnimTop;
        lumiAnim = lumiAnimTop;
    }

    private void Update()
    {
        // if the player is permitted to move, move
        if (currentGameState == GameState.GAME) UpdatePlayerMovement();

        // else, don't move
        else roanAnim.speed = 0;
    }

    void UpdatePlayerMovement()
    {
        // mouse movement is the absolute combined values of both mouse axes -
        // this means it doesn't matter in which direction the player is moving the mouse
        mouseMovement = Mathf.Abs(Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y"));

        // animation speed is bound to movement, like SuperHot
        roanAnim.speed = mouseMovement;

        #region the simple noise gauge is also bound to movement
        noiseLevel = mouseMovement;

        if (noiseLevel >= maxNoiseLevel)
        {
            print("too loud!");
            if (canJostle)
            {
                jostles++;
                canJostle = false;

                // wait for the mouse to stop moving
                Invoke("CoolDown", noiseDecreaseRate);
            }

            // Lumi's various animations are triggered by an integer called Jostles
            lumiAnim.SetInteger("Jostles", jostles);
        }
        #endregion

        // when Roan is about halfway out of bed, switch camera angles
        if (roanAnim.GetCurrentAnimatorStateInfo(0).IsName("Roan_GetOutOfBed04"))
        {
            roanAnim = roanAnimSide;
            lumiAnim = lumiAnimSide;

            topView.SetActive(false);
            sideView.SetActive(true);
        }

        // if the empty "Roan_OutOfBed" animation is triggered, Roan is out of bed and the minigame is over
        else if (roanAnim.GetCurrentAnimatorStateInfo(0).IsName("Roan_OutOfBed"))
        {
            currentGameState = GameState.WON;

            // broadcast win event
            if (OnWon != null) OnWon();
        }
    }

    void CoolDown()
    {
        canJostle = true;
    }

    public void ChangeState(GameState state)
    {
        currentGameState = state;
    }
}
