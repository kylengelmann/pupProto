using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {


    public float gravity;
    public float Zcorrection = 100f;

    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Vector2 velocity;
    [HideInInspector] public playerSettings settings;

    public characterEventHandler events = new characterEventHandler();
    [HideInInspector] public Vector2 groundNormal;
    Vector2 lastHitNorm = Vector2.up;

    //public physicsController2D.controllerHit hit;

    abilitiesManager abilitiesManager = new abilitiesManager();
    void Start()
    {
        controller = gameObject.GetComponentInHierarchy<CharacterController>();
        abilitiesManager.init(events);
        groundNormal = Vector2.up;
    }

    bool wasGrounded;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public float airTime;
    void checkGrounded(RaycastHit2D hit1, RaycastHit2D hit2)
    {
        if (hit2.collider != null && hit2.normal.y > .7f)
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
        if (isGrounded)
        {
            airTime = 0f;
            if (!wasGrounded)
            {
                events.character.onGrounded.Invoke();
            }
            wasGrounded = true;
        }
        else
        {
            if (wasGrounded)
            {
                airTime = 0f;
                events.character.onLeaveGround.Invoke();
            }
            else
            {
                airTime += Time.fixedDeltaTime;
            }
            wasGrounded = false;
        }
    }

    //void LateUpdate()
    //{
    //    float dt = Time.deltaTime;
    //    Vector2 grav = groundNormal * gravity * dt / Vector2.Dot(groundNormal, Vector2.down);
    //    velocity += grav;

    //    controllerHits hits = controller.moveVelocity(ref velocity, dt);

    //    checkGrounded(hits.hit1, hits.hit2);
    //    if (!isGrounded) airTime += dt;
    //    events.character.onPositionUpdate.Invoke(hits.hit1, hits.hit2);
    //}

    private void LateUpdate()
    {
        float dt = Time.deltaTime;
        Vector2 grav = groundNormal * gravity * dt / Vector2.Dot(groundNormal, Vector2.down);
        velocity += grav;

        Vector3 velocity3D = velocity;
        velocity3D -= Vector3.forward * transform.position.z * Zcorrection;

        Debug.Log("Z: " + transform.position.z + ", zVel: " + velocity3D.z);

        CollisionFlags flags = controller.Move(velocity3D*dt);
        if((flags | CollisionFlags.Below) != 0)
        {
            isGrounded = true;
        }
        else {
            isGrounded = false;
            airTime += dt;
        }
        //transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        velocity += ((Vector2)hit.normal)*Vector2.Dot(hit.normal, -velocity);
    }

    //void FixedUpdate()
    //{

    //    Vector2 grav = groundNormal * gravity * Time.fixedDeltaTime / Vector2.Dot(groundNormal, Vector2.down);
    //    velocity += grav;
    //    //Debug.Log(groundNormal);

    //    controllerHits hits = controller.moveVelocity(ref velocity, Time.fixedDeltaTime);

    //    checkGrounded(hits.hit1, hits.hit2);
    //    if (!isGrounded) airTime += Time.fixedDeltaTime;
    //    events.character.onPositionUpdate.Invoke(hits.hit1, hits.hit2);
    //}

}

//[System.Serializable]
//public struct playerSettings
//{
//    public float gravity;
//}

//public class characterEvents
//{
//    public safeAction onGrounded = new safeAction();
//    public safeAction onLeaveGround = new safeAction();
//    public safeAction<RaycastHit2D, RaycastHit2D> onPositionUpdate = new safeAction<RaycastHit2D, RaycastHit2D>();
//}
