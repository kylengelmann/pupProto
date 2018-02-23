using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

	// Use this for initialization
    Player player;
    public MoveSettings moveSettings;

	void Start () {
        player = gameObject.GetComponent<Player>();
	}
	
    #region Input
    float moveVal;
    public void setMoveVal(float val) 
    {
        if(val > 0f) {
            transform.localScale = new Vector3(1f, 1f);
            player.anim.SetBool("walkin", true);
        }
        else if(val < 0f) {
            transform.localScale = new Vector3(-1f, 1f);
            if(player.controller.grounded) {
                player.anim.SetBool("walkin", true);
            }
        }
        else {
            player.anim.SetBool("walkin", false);
        }
        moveVal = val;
    }

    byte doneJumps;
    bool isJumping;
    public void jump(bool isPressed, bool drop) {
        if(isPressed && !isJumping) {
            if ((player.controller.grounded || player.airTime < moveSettings.coyoteTime) && (doneJumps == 0))
            {
                if (!drop)
                {
                    doneJumps = 1;
                    player.velocity.y = moveSettings.jumpVelocity;
                }
                else
                {
                    player.controller.dropThroughOneWay = true;
                }
            }
            else if (doneJumps < 2)
            {
                ++doneJumps;
                player.velocity.y = moveSettings.doubleJumpVelocity;
            }
        }
        isJumping = isPressed;
    }
    #endregion



    #region Physics
    public void setAirVel() {
        if (player.velocity.y > 0f)
        {
            if (!isJumping)
            {
                player.velocity.y -= moveSettings.endJumpAcc * Time.fixedDeltaTime;
            }
            else
            {
                player.velocity.y -= moveSettings.jumpAcc * Time.fixedDeltaTime;
            }
        }
        else {
            player.velocity.y -= moveSettings.fallAcc * Time.fixedDeltaTime;
        }
    }


    bool isChangingDir;
    public void doUpdate()
    {
        if(player.controller.grounded && !player.wasGrounded) {
            doneJumps = 0;
        }
        float moveMod = 1f;
        if(!player.controller.grounded) {
            moveMod *= moveSettings.airControl;
        }
        if(player.velocity.x*moveVal < 0f || isChangingDir){ //Switching direction
            float dV = Mathf.Sign(moveVal)*moveSettings.directionSwitchAcceleration*moveMod*Time.fixedDeltaTime;
            if(Mathf.Abs(player.velocity.x + dV) > Mathf.Abs(moveSettings.speed*moveVal)) {
                player.velocity.x = moveSettings.speed*moveVal;
                isChangingDir = false;
            }
            else {
                player.velocity.x += dV;
            }
        }
        else if(Mathf.Abs(player.velocity.x) < Mathf.Abs(moveSettings.speed*moveVal)) { //Speeding up
            player.velocity.x += Mathf.Sign(moveVal)*moveSettings.groundAcceleration*moveMod*Time.fixedDeltaTime;
            if(Mathf.Abs(player.velocity.x) > Mathf.Abs(moveSettings.speed*moveVal)){
                player.velocity.x = moveSettings.speed*moveVal;
            }
        }
        else { //Slowing down
            float dV = Mathf.Sign(player.velocity.x)*moveSettings.groundFriction*moveMod*Time.fixedDeltaTime;
            if(Mathf.Abs(dV)>Mathf.Abs(player.velocity.x)){
                player.velocity.x = 0f;
            }
            else {
                player.velocity.x -= dV;
            }
        }

        player.anim.SetFloat("walkSpeed", Mathf.Abs(moveVal));
        setAirVel();
    }
    #endregion
}


[System.Serializable]
public struct MoveSettings {
    public float speed;
    public float groundAcceleration;
    public float directionSwitchAcceleration;
    public float groundFriction;

    public float jumpVelocity;
    public float doubleJumpVelocity;
    public float jumpAcc;
    public float endJumpAcc;
    public float fallAcc;
    public float terminalVel;
    public float airControl;
    public float coyoteTime;
}