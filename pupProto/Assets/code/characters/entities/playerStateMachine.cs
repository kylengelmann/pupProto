﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerStateMachine : MonoBehaviour {

    Move mover;
    Dash dasher;
    combatSystem combatSystem;
    WallSlide wall;
    Jump jumper;
    Character character;

    public enum playerState {
        free,
        dashing
    }

    [HideInInspector]public playerState currentState = playerState.free;

    void Start() {
        character = GetComponent<Character>();
        mover = gameObject.GetComponent<Move>();
        mover.isActive = true;
        dasher = gameObject.GetComponent<Dash>();
        //dasher.onFinish = endDash;
        character.events.dash.onDashEnd += endDash;
        combatSystem = GetComponent<combatSystem>();
        wall = GetComponent<WallSlide>();
        jumper = GetComponent<Jump>();
        
    }

    bool wasDashPressed;
    bool wasJumpPressed;
    void Update()
    {
        if(Input.GetButtonDown(GameManager.gameButtons.attack)) {
            combatSystem.setAttack(combatSystem.attackType.nuetral);
        }

        switch(currentState) {
            case playerState.free:
                float dashX = Input.GetAxisRaw(GameManager.gameButtons.xDash);
                float dashY = Input.GetAxisRaw(GameManager.gameButtons.yDash);
                bool isDashPressed = dashX*dashX + dashY*dashY > 0.3f;
                if(!wasDashPressed && isDashPressed) {
                    wasDashPressed = true;
                    if(dasher.doDash(dashX, dashY)) {
                        currentState = playerState.dashing;
                        mover.isActive = false;
                        dasher.isActive = true;
                    }
                }
                wasDashPressed = isDashPressed;
                float yMove = Input.GetAxisRaw(GameManager.gameButtons.yMove);
                //yMove = Mathf.Abs(yMove) > 0.1f ? yMove : 0f;
                float xMove = Input.GetAxisRaw(GameManager.gameButtons.xMove);
                //print(xMove);
                //xMove = Mathf.Abs(xMove) > .8f ? xMove : 0f;
                
                bool jumping = Input.GetButton(GameManager.gameButtons.jump);

                if (1.5f * Mathf.Abs(xMove) > Mathf.Abs(yMove))
                {
                    mover.setMoveVal(xMove);
                    jumper.jump(jumping, false);
                }
                else
                {
                    mover.setMoveVal(0f);
                    jumper.jump(jumping, yMove < -0.5f);
                }

                wall.wallJump(jumping);
                //if(wall.wallJump(jumping))
                //{
                //    jumper.doneJumps = 1;
                //}

                break;
            case playerState.dashing:
                dashX = Input.GetAxisRaw(GameManager.gameButtons.xDash);
                dashY = Input.GetAxisRaw(GameManager.gameButtons.yDash);
                if(dashX*dashX + dashY*dashY > 0.3f) {
                    dasher.setDir(dashX, dashY);
                }
                break;
        }
    }

    void endDash() {
        currentState = playerState.free;
        mover.isActive = true;
        dasher.isActive = false;
    }

}