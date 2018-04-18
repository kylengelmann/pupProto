using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component handles basic movements like walking and jumping. It only
/// performs physics updates while isActive is set to true.
/// </summary>
public class Move : MonoBehaviour {

    Player player;
    public moveSettings settings;

    /// <summary>
    /// Is this component performing physics updates?
    /// </summary>
    [HideInInspector]public bool isActive = false;


	void Start () {
        player = gameObject.GetComponent<Player>();
	}


	
    #region Input

    /// <summary>
    /// which controls how fast and in which direction the player moves 
    /// and accelerates.
    /// </summary>
    float moveVal;

    /// <summary>
    /// Sets the player's move value, which controls how fast and in which
    /// direction the player moves and accelerates.
    /// </summary>
    /// <param name="val">What to set the move value to</param>
    public void setMoveVal(float val) 
    {
        if(val > 0f) {
            transform.localScale = new Vector3(1f, 1f);
            player.anim.SetBool("walkin", true);
        }
        else if(val < 0f) {
            transform.localScale = new Vector3(-1f, 1f);
            if(player.isGrounded) {
                player.anim.SetBool("walkin", true);
            }
        }
        else {
            player.anim.SetBool("walkin", false);
        }
        moveVal = val;
    }

    /// <summary>
    /// The number of consecutive jumps the player has made in the air.
    /// </summary>
    byte doneJumps;

    /// <summary>
    /// Is the jump button being held?
    /// </summary>
    bool isJumping;

    /// <summary>
    /// Causes the player to jump and drop through one way platforms. Also
    /// records information needed to make the jump height responsive.
    /// </summary>
    /// <returns>The jump.</returns>
    /// <param name="isPressed">Whether or not the jump button is pressed.</param>
    /// <param name="drop">If set to <c>true</c>, the player will drop through
    /// one way platforms.</param>
    public void jump(bool isPressed, bool drop) {
        if(isPressed && !isJumping) {
            if ((player.isGrounded || player.airTime < settings.coyoteTime) && (doneJumps == 0))
            {
                if (!drop)
                {
                    player.velocity.y = settings.jumpVelocity;
                }
                else
                {
                    player.controller.dropThroughOneWay = true;
                }
            }
            else if (doneJumps < 1)
            {
                ++doneJumps;
                player.velocity.y = settings.doubleJumpVelocity;
            }
        }
        isJumping = isPressed;
    }
    #endregion



    #region Physics
    /// <summary>
    /// Sets the vertical velocity of the player while the player is in the
    /// air. Changes strength of gravity based off of the state of the jump
    /// button and whether or not the player is rising or falling.
    /// </summary>
    public void setAirVel() {
        if (player.velocity.y > 0f)
        {
            if (!isJumping)
            {
                player.velocity.y -= settings.endJumpAcc * Time.fixedDeltaTime;
            }
            else
            {
                player.velocity.y -= settings.jumpAcc * Time.fixedDeltaTime;
                player.velocity.y = Mathf.Max(player.velocity.y, -settings.terminalVel);
            }
        }
        else {
            player.velocity.y -= settings.fallAcc * Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Is the player changing directions?
    /// </summary>
    bool isChangingDir;


    public void FixedUpdate()
    {
        // If the component is not active, do not perform updates
        if(!isActive) return;

        // If the player has just hit the ground, reset doneJumps
        if(player.isGrounded && !player.wasGrounded) {
            doneJumps = 0;
        }

        // moveMod affects the acceleration of the player
        // Set move mod depending on whether or not the player is grounded
        float moveMod = 1f;
        if(!player.isGrounded) {
            moveMod *= settings.airControl;
        }

        if(player.velocity.x*moveVal < 0f || isChangingDir){ //Switching directions
            float dV = Mathf.Sign(moveVal)*settings.directionSwitchAcceleration*moveMod*Time.fixedDeltaTime;

            // Check whether or not the player his reached their desired speed.
            // If so, they are no longer switching directions, and we might as
            // well prevent overcorrection while we're at it.
            if(Mathf.Abs(player.velocity.x + dV) > Mathf.Abs(settings.speed*moveVal)) {
                player.velocity.x = settings.speed*moveVal;
                isChangingDir = false;
            }
            else {
                player.velocity.x += dV;
            }
        }
        else if(Mathf.Abs(player.velocity.x) <= Mathf.Abs(settings.speed*moveVal)) { //Speeding up
            
            player.velocity.x += Mathf.Sign(moveVal)*settings.groundAcceleration*moveMod*Time.fixedDeltaTime;

            // Cap the speed of the player
            if(Mathf.Abs(player.velocity.x) > Mathf.Abs(settings.speed*moveVal)){
                player.velocity.x = settings.speed*moveVal;
            }
        }
        else { //Slowing down
            float dV = Mathf.Sign(player.velocity.x)*settings.groundFriction*moveMod*Time.fixedDeltaTime;

            // Prevent overcorrection
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
public struct moveSettings {
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