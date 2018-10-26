using UnityEngine;
using System;

//[RequireComponent(typeof(physicsController2D))]
public class Char2DNew : Character {

    Vector3 nextPos;
    float dtBetweenPos;
    DateTime timeUpdatedPos;
    Vector3 lastPos;
    public float groundCheckDist = .05f;

    [HideInInspector]public characterPhysics controller;
    //[HideInInspector]public playerSettings settings;

    Vector2 lastHitNorm = Vector2.up;

    //public physicsController2D.controllerHit hit;

    abilitiesManager abilitiesManager = new abilitiesManager();
	protected override void Start () {
        base.Start();
        controller = gameObject.GetComponent<characterPhysics>();
        nextPos = transform.position;
    }

    GameObject prevFloor;
    GameObject floor;
    Vector2 floorPosition;
    Vector2 floorVelocity;

    bool wasGrounded;
    void handleGroundState(float dt) {
        isGrounded = controller.isGrounded;
        if (!isGrounded)
        {
            isGrounded = controller.checkFloor(groundCheckDist);
        }
        prevFloor = floor;
        if (isGrounded) {
            floor = controller.groundHit.collider.gameObject;
            floorPosition = floor.transform.position;
            airTime = 0f;
            groundNormal = controller.groundHit.normal;
            if (!wasGrounded)
            {
                events.character.onGrounded.Invoke();
            }
            wasGrounded = true;
        }
        else {
            floor = null;
            groundNormal = Vector2.up;
            if(wasGrounded){
                airTime = 0f;
                events.character.onLeaveGround.Invoke();
            }
            else {
                airTime += dt;
            }
            wasGrounded = false;
        }
    }

    void addFloorMovement(float dt)
    {

        if (floor == null)
        {
            if (prevFloor != null)
            {
                velocity += floorVelocity;
            }
            floorVelocity = Vector2.zero;
        }
        else
        {
            Vector2 floorDS = (Vector2)floor.transform.position - floorPosition;
            transform.position += (Vector3)floorDS;
            floorVelocity = floorDS / dt;
        }
    }

    void LateUpdate()
    {

        float dt = Time.deltaTime;
        addFloorMovement(dt);
        Vector2 grav = groundNormal * gravity * dt / Vector2.Dot(groundNormal, Vector2.down);
        //Vector2 grav = Vector2.down * gravity * dt;
        velocity += grav;

        controller.moveVelocity(ref velocity, dt);
        handleGroundState(dt);
        //events.character.onPositionUpdate.Invoke(hits.hit1, hits.hit2);
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