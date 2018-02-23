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

    public playerState currentState = playerState.free;

    void Start() {
        mover = gameObject.GetComponent<Move>();
        dasher = gameObject.GetComponent<Dash>();
        camPos = Camera.main.transform.position;
    }

    Vector3 camPos;
    Vector2 dashDir;
    void Update()
    {
        camPos.x = transform.position.x;
        camPos.y = transform.position.y;
        Camera.main.transform.position = camPos;
        switch(currentState) {
            case playerState.free:
                dashDir.x = Input.GetAxisRaw(GameManager.gameButtons.xDash);
                dashDir.y = Input.GetAxisRaw(GameManager.gameButtons.yDash);
                if(dashDir.sqrMagnitude > 0.25f) {
                    dasher.doDash(dashDir.normalized);
                }
                mover.setMoveVal(Input.GetAxisRaw(GameManager.gameButtons.xMove));
                mover.jump(Input.GetButton(GameManager.gameButtons.jump), 
                           Input.GetAxisRaw(GameManager.gameButtons.yMove) < -0.5f);
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
}
