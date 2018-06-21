using System;
using UnityEngine;
using UnityEngine.Events;

public class Jump : MonoBehaviour {

    Character character;
    [SerializeField] jumpSettings settings;

	// Use this for initialization
	void Start () {
		character = GetComponent<Character>();
        character.events.wall.onWallJump += onWallJump;
        character.events.character.onGrounded += onGrounded;
	}

    void onWallJump()
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
                character.velocity.y -= settings.endJumpAcc * Time.fixedDeltaTime;
            }
            else
            {
                character.velocity.y -= settings.jumpAcc * Time.fixedDeltaTime;
                character.velocity.y = Mathf.Max(character.velocity.y, -settings.terminalVel);
            }
        }
        else
        {
            character.velocity.y -= settings.fallAcc * Time.fixedDeltaTime;
        }
    }

    private void FixedUpdate()
    {
        setAirVel();
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
    public float airControl;
    public float coyoteTime;
}

public class jumpEvents
{
    public safeAction onJump = new safeAction();

    
}