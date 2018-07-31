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
    [HideInInspector]public bool isActive = true;


	void Start () {
//	    isActive = true;
        _character = gameObject.GetComponent<Character>();
	    _character.events.move.setMove += setMoveVal;
	    _character.events.move.setActive += setActive;
	}
    
    void setActive(bool active)
    {
        isActive = active;
        if(!isActive)
        {
            if(isMoving)
            {
                _character.events.move.onStopMove.Invoke();
                isMoving = false;
                moveVal = 0f;
            }
        }
    }
	
    #region Input


    float moveVal;
    bool isMoving;

    /// <summary>
    /// Sets the player's move value, which controls how fast and in which
    /// direction the player moves and accelerates.
    /// </summary>
    /// <param name="val">What to set the move value to</param>
    public void setMoveVal(float val) 
    {
        if(!isActive) return;
        if(Mathf.Abs(val) > 0.1f)
        {
            if(!isMoving)
            {
                _character.events.move.onMove.Invoke();
            }
            isMoving = true;
        }
        else {
            val = 0f;
            if(isMoving)
            {
                _character.events.move.onStopMove.Invoke();
            }
            isMoving = false;
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

        Vector2 moveDir = new Vector2(_character.groundNormal.y, -_character.groundNormal.x);
        float moveVel = Vector2.Dot(moveDir, _character.velocity);
        _character.velocity -= moveVel*moveDir;

        // moveMod affects the acceleration of the player
        // Set move mod depending on whether or not the player is grounded
        float moveMod = 1f;
        if(!_character.isGrounded) {
            moveMod *= settings.airControl;
        }

        if(moveVel*moveVal < 0f || isChangingDir) { //Switching directions
            float dV = Mathf.Sign(moveVal)*settings.directionSwitchAcceleration*moveMod*Time.fixedDeltaTime;

            // Check whether or not the player his reached their desired speed.
            // If so, they are no longer switching directions, and we might as
            // well prevent overcorrection while we're at it.
            if(Mathf.Sign(moveVal)*Mathf.Sign(moveVel) > 0f && Mathf.Abs(moveVel + dV) > Mathf.Abs(settings.speed*moveVal)) {
                moveVel = settings.speed*moveVal;
                isChangingDir = false;
            }
            else {
                moveVel += dV;
            }
        }
        else if(Mathf.Abs(moveVel) <= Mathf.Abs(settings.speed*moveVal)) { //Speeding up
            
            moveVel += Mathf.Sign(moveVal)*settings.groundAcceleration*moveMod*Time.fixedDeltaTime;

            // Cap the speed of the player
            if( Mathf.Abs(moveVel) > Mathf.Abs(settings.speed*moveVal)) {
                moveVel = settings.speed*moveVal;
            }
        }
        else { //Slowing down
            float dV = Mathf.Sign(moveVel)*settings.groundFriction*moveMod*Time.fixedDeltaTime;

            // Prevent overcorrection
            if(Mathf.Abs(dV) > Mathf.Abs(moveVel)){
                moveVel = 0f;
            }
            else {
                moveVel -= dV;
            }
        }

        _character.velocity += moveVel*moveDir;
    }
    #endregion
}


[Serializable]
public struct moveSettings {
    public float speed;
    public float groundAcceleration;
    public float directionSwitchAcceleration;
    public float groundFriction;
    public float airControl;
}

public class moveEvents
{
    public safeAction<float> setMove = new safeAction<float>();
    
    public safeAction onMove = new safeAction();
    public safeAction onStopMove = new safeAction();
    
    public safeAction<bool> setActive = new safeAction<bool>();
}