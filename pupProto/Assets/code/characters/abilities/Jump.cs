using System;
using UnityEngine;
using UnityEngine.Events;

public class Jump : MonoBehaviour {

    Character character;
    [SerializeField] jumpSettings settings;
    [HideInInspector] public bool isActive = true;

	// Use this for initialization
	void Start () {
		character = GetComponent<Character>();
        character.events.wall.onWallSlide += onWallSlide;
        character.events.character.onGrounded += onGrounded;
	    character.events.jump.setActive += setActive;
	    character.events.jump.setJump += jump;
	}
    
    void setActive(bool active)
    {
        isActive = active;
    }

    void onWallSlide(bool onRightWall)
    {
        doneJumps = 1;
    }

    void onGrounded()
    {
        doneJumps = 0;
    }

    [HideInInspector] public byte doneJumps;
    bool isJumping;
    /// <summary>
    /// Causes the player to jump and drop through one way platforms. Also
    /// records information needed to make the jump height responsive.
    /// </summary>
    /// <returns>The jump.</returns>
    /// <param name="isPressed">Whether or not the jump button is pressed.</param>
    /// <param name="drop">If set to <c>true</c>, the player will drop through
    /// one way platforms.</param>
    public void jump(bool isPressed, bool drop)
    {
        if(!isActive) return;
        if (isPressed && !isJumping)
        {
            if ((character.isGrounded || character.airTime < settings.coyoteTime) && (doneJumps == 0))
            {
                if (!drop)
                {
                    doneJumps = 1;
                    character.velocity.y = settings.jumpVelocity;
                    character.events.jump.onJump.Invoke();
                }
                else
                {
                    character.controller.dropThroughOneWay = true;
                }

                
            }
            else if (doneJumps < 2)
            {
                ++doneJumps;
                character.velocity.y = settings.doubleJumpVelocity;
                character.events.jump.onJump.Invoke();
            }
        }
        isJumping = isPressed;
    }

    void setAirVel()
    {
        if (character.velocity.y > 0f)
        {
            if (!isJumping)
            {
                character.gravity = settings.endJumpAcc;
                //character.velocity.y -= settings.endJumpAcc * Time.fixedDeltaTime;
            }
            else
            {
                character.gravity = settings.jumpAcc;
                //character.velocity.y -= settings.jumpAcc * Time.fixedDeltaTime;
            }
        }
        else
        {
            character.gravity = settings.fallAcc;
            //character.velocity.y -= settings.fallAcc * Time.fixedDeltaTime;
        }
    }

    private void FixedUpdate()
    {
        if(!isActive) return;
        setAirVel();
        character.velocity.y = Mathf.Max(character.velocity.y, -settings.terminalVel);
    }
}


[Serializable]
public struct jumpSettings
{
    public float jumpVelocity;
    public float doubleJumpVelocity;
    public float jumpAcc;
    public float endJumpAcc;
    public float fallAcc;
    public float terminalVel;
    public float coyoteTime;
}

public class jumpEvents
{
    public safeAction<bool, bool> setJump = new safeAction<bool, bool>();
    public safeAction onJump = new safeAction();
    public safeAction<bool> setActive = new safeAction<bool>();
}