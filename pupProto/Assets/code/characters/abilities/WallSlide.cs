using System;
using UnityEngine;

public class WallSlide : MonoBehaviour
{
    [HideInInspector] public bool onWall;
    public float friction = 20f;
    public float terminalVel = 10f;
    public float maxDist = .1f;
    public Vector2 jumpVel = new Vector2(10f, 10f);
    physicsController2D controller;
    BoxCollider2D box;
    Character character;
    
    [HideInInspector] public bool isActive = true;

    bool onRightWall;

    public LayerMask walls;

    private void Start()
    {
        controller = GetComponent<physicsController2D>();
        box = controller.box;
        character = GetComponent<Character>();
        character.events.jump.onJump += onJump;
        character.events.wall.setActive += setActive;
    }
    
    void setActive(bool active)
    {
        isActive = active;
        if(!isActive)
        {
            wasOnWall = false;
            if(onWall)
            {
                character.events.wall.offWall.Invoke();
                onWall = false;
            }
        }
    }

    bool wasOnWall;

    void FixedUpdate()
    {
        if(!isActive) return;
        onWall = checkOnWall();
        if (!onWall)
        {
            if (wasOnWall) character.events.wall.offWall.Invoke();
        }
        else
        {
            if (!wasOnWall) character.events.wall.onWallSlide.Invoke(onRightWall);
            applyFriction(Time.fixedDeltaTime);
        }
        wasOnWall = onWall;
    }


    bool checkOnWall()
    {
        if (character.isGrounded)
        {
            return false;
        }

        Vector2 topRight = transform.position;
        Vector2 topLeft = transform.position;
        Vector2 bottomLeft = topRight;
        Vector2 bottomRight = topRight;
        topRight.y += (box.offset.y + box.size.y * .5f) * transform.lossyScale.y;
        topLeft = topRight;
        topRight.x += box.size.x * Mathf.Abs(transform.lossyScale.x) * .5f;
        topLeft.x -= box.size.x * Mathf.Abs(transform.lossyScale.x) * .5f;
        bottomLeft.y += (box.offset.y - box.size.y * .5f) * transform.lossyScale.y;
        bottomRight = bottomLeft;
        bottomRight.x += box.size.x * Mathf.Abs(transform.lossyScale.x) * .5f;
        bottomLeft.x -= box.size.x * Mathf.Abs(transform.lossyScale.x) * .5f;


        bool top = Physics2D.Raycast(topRight, Vector2.right, maxDist, walls.value);
        bool bottom = Physics2D.Raycast(bottomRight, Vector2.right, maxDist, walls.value);
        if (top && bottom)
        {
            onRightWall = true;
            return true;
        }

        top = Physics2D.Raycast(topLeft, Vector2.left, maxDist, walls.value);
        bottom = Physics2D.Raycast(bottomLeft, Vector2.left, maxDist, walls.value);
        if (top && bottom)
        {
            onRightWall = false;
            return true;
        }

        return false;
    }

    void applyFriction(float dt)
    {
        if (character.velocity.y >= 0f)
        {
            return;
        }

        character.velocity.y += friction * dt;
        character.velocity.y = Mathf.Clamp(character.velocity.y, -terminalVel, 0f);
    }


    bool isJumping;

//    public void wallJump(bool isPressed)
//    {
//        if (isPressed && !isJumping &&  onWall && !character.isGrounded)
//        {
//            character.velocity.y = jumpVel.y;
//            character.velocity.x = onRightWall ? -jumpVel.x : jumpVel.x;
//
//            character.events.wall.onWallJump.Invoke();
//        }
//
//        isJumping = isPressed;
//    }
    
    void onJump()
    {
        if(onWall && !character.isGrounded)
        {
            character.velocity.y = jumpVel.y;
            character.velocity.x = onRightWall ? -jumpVel.x : jumpVel.x;
            character.events.wall.onWallJump.Invoke();
        }
    }
}


public class wallEvents
{
    public safeAction<bool> onWallSlide = new safeAction<bool>();
    public safeAction offWall = new safeAction();
    public safeAction onWallJump = new safeAction();
    public safeAction<bool> setActive = new safeAction<bool>();
}