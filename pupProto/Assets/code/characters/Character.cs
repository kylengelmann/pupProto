using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(physicsController2D))]
public class Character : MonoBehaviour {

    public float gravity;

    [HideInInspector]public Animator anim;
    [HideInInspector]public physicsController2D controller;
    [HideInInspector]public Vector2 velocity;
    [HideInInspector]public playerSettings settings;

    public characterEventHandler events = new characterEventHandler();
    [HideInInspector] public Vector2 groundNormal;
    Vector2 lastHitNorm = Vector2.up;

    //public physicsController2D.controllerHit hit;

    abilitiesManager abilitiesManager = new abilitiesManager();
	void Start () {
        anim = gameObject.GetComponent<Animator>();
        controller = gameObject.GetComponent<physicsController2D>();
	    abilitiesManager.init(events);
        groundNormal = Vector2.up;
    }

    bool wasGrounded;
    [HideInInspector]public bool isGrounded;
    [HideInInspector]public float airTime;
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
            //            anim.SetBool("jumpin", false);
            //velocity.y = 0f;
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

    void FixedUpdate()
    {

        Vector2 grav = groundNormal*gravity*Time.fixedDeltaTime/Vector2.Dot(groundNormal, Vector2.down);
        velocity += grav;
        Debug.Log(groundNormal);

        Vector2 perpVel = Vector2.Dot(velocity, lastHitNorm)*lastHitNorm;
        Vector2 parVel = velocity - perpVel;

        RaycastHit2D hit1 = controller.moveVelocity(ref parVel, Time.fixedDeltaTime);
        RaycastHit2D hit2 = controller.moveVelocity(ref perpVel, Time.fixedDeltaTime);

        velocity = parVel + perpVel;

        if(hit2.collider != null)
        {
            lastHitNorm = hit2.normal;
        }
        else if(hit1.collider != null)
        {
            lastHitNorm = hit1.normal;
        }
        else
        {
            lastHitNorm = Vector2.zero;
            //lastHitNorm = Vector2.up;
        }

        checkGrounded(hit1, hit2);
        events.character.onPositionUpdate.Invoke(hit1, hit2);
    }

}

[System.Serializable]
public struct playerSettings {
    public float gravity;
}

public class characterEvents
{
    public safeAction onGrounded = new safeAction();
    public safeAction onLeaveGround = new safeAction();
    public safeAction<RaycastHit2D, RaycastHit2D> onPositionUpdate = new safeAction<RaycastHit2D, RaycastHit2D>();
}