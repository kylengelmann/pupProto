using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerStateMachine : MonoBehaviour {

    Move mover;
    Dash dasher;
    Combo combo;

    public enum playerState {
        free,
        dashing
    }

    [HideInInspector]public playerState currentState = playerState.free;

    void Start() {
        mover = gameObject.GetComponent<Move>();
        mover.isActive = true;
        dasher = gameObject.GetComponent<Dash>();
        dasher.onFinish = endDash;
        combo = GetComponent<Combo>();
        camPos = Camera.main.transform.position;
    }

    Vector3 camPos;
    bool wasDashPressed;
    void Update()
    {
        if(Input.GetButtonDown(GameManager.gameButtons.attack)) {
            combo.setAttack(Combo.AttackType.normal);
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
                float xMove = Input.GetAxisRaw(GameManager.gameButtons.xMove);
                if(1.5f*Mathf.Abs(xMove) > Mathf.Abs(yMove)) {
                    mover.setMoveVal(xMove);
                    mover.jump(Input.GetButton(GameManager.gameButtons.jump), false);
                }
                else {
                    mover.setMoveVal(0f);
                    mover.jump(Input.GetButton(GameManager.gameButtons.jump), yMove < -0.5f);
                }
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
