using UnityEngine;
using System;

[RequireComponent(typeof(physicsController2D))]
public class Character2D : Character {

    Vector3 nextPos;
    float dtBetweenPos;
    DateTime timeUpdatedPos;
    Vector3 lastPos;

    [HideInInspector]public physicsController2D controller;
    //[HideInInspector]public playerSettings settings;

    Vector2 lastHitNorm = Vector2.up;

    //public physicsController2D.controllerHit hit;

    abilitiesManager abilitiesManager = new abilitiesManager();
	protected override void Start () {
        base.Start();
        controller = gameObject.GetComponent<physicsController2D>();
        nextPos = transform.position;
    }

    bool wasGrounded;
    void checkGrounded(RaycastHit2D hit1, RaycastHit2D hit2) {
        if(hit2.collider != null && hit2.normal.y > .7f)
        {
            isGrounded = true;
            groundNormal = hit2.normal;
        }
        else if (hit1.collider != null && hit1.normal.y > .7f)
        {
            isGrounded = true;
            groundNormal = hit1.normal;
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector2.up;
        }
        if (isGrounded) {
            airTime = 0f;
            if (!wasGrounded)
            {
                events.character.onGrounded.Invoke();
            }
            wasGrounded = true;
        }
        else {
            if(wasGrounded){
                airTime = 0f;
                events.character.onLeaveGround.Invoke();
            }
            else {
                airTime += Time.fixedDeltaTime;
            }
            wasGrounded = false;
        }
    }

    void setGrounded()
    {

    }

    void LateUpdate()
    {
        float dt = Time.deltaTime;
        Vector2 grav = groundNormal * gravity * dt / Vector2.Dot(groundNormal, Vector2.down);
        //Vector2 grav =  Vector2.down * gravity * dt;
        velocity += grav;

        Debug.DrawLine(transform.position, transform.position + (Vector3)grav, Color.green);

        controllerHits hits = controller.moveVelocity(ref velocity, dt);

        checkGrounded(hits.hit1, hits.hit2);
        if (!isGrounded) airTime += dt;
        events.character.onPositionUpdate.Invoke(hits.hit1, hits.hit2);
    }


    public override void dropThrough()
    {
        controller.dropThroughOneWay = true;
    }

    public override void move(Vector2 dS)
    {
        controller.doMove(dS);
    }
}

//[System.Serializable]
//public struct playerSettings {
//    public float gravity;
//}

//public class characterEvents
//{
//    public safeAction onGrounded = new safeAction();
//    public safeAction onLeaveGround = new safeAction();
//    public safeAction<RaycastHit2D, RaycastHit2D> onPositionUpdate = new safeAction<RaycastHit2D, RaycastHit2D>();
//}