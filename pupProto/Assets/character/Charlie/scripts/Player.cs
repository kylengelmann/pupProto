using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(physicsController2D))]
public class Player : MonoBehaviour {

    [HideInInspector]public Animator anim;
    [HideInInspector]public physicsController2D controller;
    [HideInInspector]public Vector2 velocity;
    [HideInInspector]public playerSettings settings;
    public Vector2 groundBoxOffset;
    public Vector2 groundBoxSize;
    public Vector2 airBoxOffset;
    public Vector2 airBoxSize;

	void Start () {
        anim = gameObject.GetComponent<Animator>();
        controller = gameObject.GetComponent<physicsController2D>();
	}

    [HideInInspector]public bool wasGrounded;
    [HideInInspector]public float airTime;
    void checkGrounded() {
        if (controller.grounded) {
            anim.SetBool("jumpin", false);
            airTime = 0f;
            wasGrounded = true;
        }
        else {
            if(wasGrounded){
                airTime = 0f;
            }
            else {
                airTime += Time.fixedDeltaTime;
            }
            wasGrounded = false;
            anim.SetBool("jumpin", true);
            anim.SetBool("walkin", false);
        }
    }

    void FixedUpdate()
    {
        checkGrounded();
        movingPlatform moving;
        try
        {
            moving = controller.hit.y.transform.gameObject.GetComponent<movingPlatform>();
        }
        catch
        {
            moving = null;
        }
        if (moving != null)
        {
            Vector2 baseVel = moving.velocity;
            controller.moveVelocity(ref baseVel, Time.fixedDeltaTime);
        }
        controller.moveVelocity(ref velocity, Time.fixedDeltaTime);

        anim.SetFloat("fallin", velocity.y);
    }

}

[System.Serializable]
public struct playerSettings {
    public float gravity;
}
