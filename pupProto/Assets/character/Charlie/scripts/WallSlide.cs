using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSlide : MonoBehaviour {

    [HideInInspector] public bool onWall;
    public float friction = 20f;
    public float terminalVel = 10f;
    public float maxDist = .1f;
    public Vector2 jumpVel = new Vector2(10f, 10f);
    physicsController2D controller;
    BoxCollider2D box;
    Character character;

    bool onRightWall;

    public LayerMask walls;

    private void Start()
    {
        controller = GetComponent<physicsController2D>();
        box = controller.box;
        character = GetComponent<Character>();
    }

    void FixedUpdate () {
        onWall = checkOnWall();
        if(!onWall)
        {
            return;
        }
        applyFriction(Time.fixedDeltaTime);
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
        if(character.velocity.y >= 0f)
        {
            return;
        }

        character.velocity.y += friction*dt;
        character.velocity.y = Mathf.Clamp(character.velocity.y, -terminalVel, 0f);
    }


    bool isJumping;

    public bool wallJump(bool isPressed)
    {
        bool res = false;
        if(onWall && !character.isGrounded)
        {
            res = true;
        }


        if(isPressed && !isJumping && res)
        {
            character.velocity.y = jumpVel.y;
            character.velocity.x = onRightWall ? -jumpVel.x : jumpVel.x;
        }

        isJumping = isPressed;

        return res;
    }
}
