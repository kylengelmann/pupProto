using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerStateMachine : MonoBehaviour {

    Move mover;

    enum playerState {
        free,
        dashing
    }

    playerState currentState = playerState.free;

    void Start() {
        mover = gameObject.GetComponent<Move>();
        camPos = Camera.main.transform.position;
    }

    Vector3 camPos;

    void Update()
    {
        camPos.x = transform.position.x;
        camPos.y = transform.position.y;
        Camera.main.transform.position = camPos;
        switch(currentState) {
            case playerState.free:
                mover.setMoveVal(Input.GetAxisRaw(GameManager.gameButtons.xMove));
                mover.jump(Input.GetButton(GameManager.gameButtons.jump), 
                           Input.GetAxisRaw(GameManager.gameButtons.yMove) < -0.5f);
                break;
            case playerState.dashing:
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
                break;
        }
    }
}
