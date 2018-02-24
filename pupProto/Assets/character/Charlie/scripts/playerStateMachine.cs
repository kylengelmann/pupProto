using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerStateMachine : MonoBehaviour {

    Move mover;
    Dash dasher;

    public enum playerState {
        free,
        dashing
    }

    [HideInInspector]public playerState currentState = playerState.free;

    void Start() {
        mover = gameObject.GetComponent<Move>();
        dasher = gameObject.GetComponent<Dash>();
        dasher.onFinish = endDash;
        camPos = Camera.main.transform.position;
    }

    Vector3 camPos;
    bool wasDashPressed;
    void Update()
    {
        camPos.x = transform.position.x;
        camPos.y = transform.position.y;
        Camera.main.transform.position = camPos;
        switch(currentState) {
            case playerState.free:
                float dashX = Input.GetAxisRaw(GameManager.gameButtons.xDash);
                float dashY = Input.GetAxisRaw(GameManager.gameButtons.yDash);
                bool isDashPressed = dashX*dashX + dashY*dashY > 0.25f;
                if(!wasDashPressed && isDashPressed) {
                    wasDashPressed = true;
                    if(dasher.doDash(dashX, dashY)) {
                        currentState = playerState.dashing;
                    }
                }
                wasDashPressed = isDashPressed;
                mover.setMoveVal(Input.GetAxisRaw(GameManager.gameButtons.xMove));
                mover.jump(Input.GetButton(GameManager.gameButtons.jump), 
                           Input.GetAxisRaw(GameManager.gameButtons.yMove) < -0.5f);
                break;
            case playerState.dashing:
                dashX = Input.GetAxisRaw(GameManager.gameButtons.xDash);
                dashY = Input.GetAxisRaw(GameManager.gameButtons.yDash);
                if(dashX*dashX + dashY*dashY > 0.25f) {
                    dasher.setDir(dashX, dashY);
                }
                break;
        }
    }

    void FixedUpdate()
    {
        switch(currentState) {
            case playerState.free:
                mover.doUpdate();
                break;
            case playerState.dashing:
                dasher.doUpdate();
                break;
        }
    }

    void endDash() {
        currentState = playerState.free;
    }

}
