using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(physicsController2D))]
public class Character : MonoBehaviour {

    [HideInInspector]public Animator anim;
    [HideInInspector]public physicsController2D controller;
    [HideInInspector]public Vector2 velocity;
    [HideInInspector]public playerSettings settings;

    public characterEventHandler events = new characterEventHandler();

    public physicsController2D.controllerHit hit;

	void Start () {
        anim = gameObject.GetComponent<Animator>();
        controller = gameObject.GetComponent<physicsController2D>();
	}

    bool wasGrounded;
    [HideInInspector]public bool isGrounded;
    [HideInInspector]public float airTime;
    void checkGrounded() {
        isGrounded = controller.grounded;
        if (isGrounded) {
//            anim.SetBool("jumpin", false);
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
        controller.moveVelocity(ref velocity, Time.fixedDeltaTime);
        hit = controller.hit;
        events.character.onPositionUpdate.Invoke();
        checkGrounded();
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
    public safeAction onPositionUpdate = new safeAction();
}