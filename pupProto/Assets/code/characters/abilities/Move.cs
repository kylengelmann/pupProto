using System;
using UnityEngine;

/// <summary>
/// This component handles basic movements like walking and jumping. It only
/// performs physics updates while isActive is set to true.
/// </summary>
public class Move : MonoBehaviour {

    Character _character;
    public moveSettings settings;

    /// <summary>
    /// Is this component performing physics updates?
    /// </summary>
    [HideInInspector]public bool isActive = false;


	void Start () {
        _character = gameObject.GetComponent<Character>();
	}


	
    #region Input


    float moveVal;

    /// <summary>
    /// Sets the player's move value, which controls how fast and in which
    /// direction the player moves and accelerates.
    /// </summary>
    /// <param name="val">What to set the move value to</param>
    public void setMoveVal(float val) 
    {
        _character.events.move.onMove.Invoke(moveVal);
        if(Mathf.Abs(val) > 0.1f)
        {
            transform.localScale = new Vector3(Mathf.Sign(val), 1f);
            _character.events.move.onMove.Invoke(moveVal);
        }
        else {
            val = 0f;
            _character.events.move.onStopMove.Invoke();
        }
        moveVal = val;

    }

    #endregion

    #region Physics

    bool isChangingDir;


    public void FixedUpdate()
    {
        // If the component is not active, do not perform updates
        if(!isActive) return;

        // If the player has just hit the ground, reset doneJumps
        //if(_character.isGrounded) {
        //    doneJumps = 0;
        //}

        // moveMod affects the acceleration of the player
        // Set move mod depending on whether or not the player is grounded
        float moveMod = 1f;
        if(!_character.isGrounded) {
            moveMod *= settings.airControl;
        }

        if(_character.velocity.x*moveVal < 0f || isChangingDir) { //Switching directions
            float dV = Mathf.Sign(moveVal)*settings.directionSwitchAcceleration*moveMod*Time.fixedDeltaTime;

            // Check whether or not the player his reached their desired speed.
            // If so, they are no longer switching directions, and we might as
            // well prevent overcorrection while we're at it.
            if(Mathf.Sign(moveVal)*Mathf.Sign(_character.velocity.x) > 0f && Mathf.Abs(_character.velocity.x + dV) > Mathf.Abs(settings.speed*moveVal)) {
                _character.velocity.x = settings.speed*moveVal;
                isChangingDir = false;
            }
            else {
                _character.velocity.x += dV;
            }
        }
        else if(Mathf.Abs(_character.velocity.x) <= Mathf.Abs(settings.speed*moveVal)) { //Speeding up
            
            _character.velocity.x += Mathf.Sign(moveVal)*settings.groundAcceleration*moveMod*Time.fixedDeltaTime;

            // Cap the speed of the player
            if( Mathf.Abs(_character.velocity.x) > Mathf.Abs(settings.speed*moveVal)) {
                _character.velocity.x = settings.speed*moveVal;
            }
        }
        else { //Slowing down
            float dV = Mathf.Sign(_character.velocity.x)*settings.groundFriction*moveMod*Time.fixedDeltaTime;

            // Prevent overcorrection
            if(Mathf.Abs(dV) > Mathf.Abs(_character.velocity.x)){
                _character.velocity.x = 0f;
            }
            else {
                _character.velocity.x -= dV;
            }
        }
    }
    #endregion
}


[Serializable]
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

public class moveEvents
{
    public safeAction<float> onMove = new safeAction<float>();
    public safeAction onStopMove = new safeAction();
}