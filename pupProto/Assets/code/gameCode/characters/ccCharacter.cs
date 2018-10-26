using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ccCharacter : Character {

    public float Zcorrection = 100f;
    public float groundCheckDist = .02f;
    public LayerMask floorLayers;

    CharacterController controller;

    protected override void Start()
    {
        base.Start();
        controller = gameObject.GetComponentInHierarchy<CharacterController>();
    }

    bool wasGrounded;

    void checkGrounded(float dt)
    {
        bool didHit = false;
        if (controller.isGrounded)
        {
            Vector3 origin = transform.position + Vector3.down * controller.height / 2f;
            RaycastHit hit;
            didHit = Physics.SphereCast(origin, controller.radius * .9f, Vector3.down, out hit, controller.skinWidth + groundCheckDist, floorLayers.value, QueryTriggerInteraction.Collide);
            if (didHit)
            {
                isGrounded = true;
                groundNormal = hit.normal;
                Debug.DrawLine(hit.point, hit.point + hit.normal);
                if (!wasGrounded)
                {
                    airTime = 0f;
                    events.character.onGrounded.Invoke();
                }
            }
        }
        if (!didHit)
        {
            isGrounded = false;
            groundNormal = Vector2.up;
            airTime += dt;
            if (wasGrounded)
            {
                events.character.onLeaveGround.Invoke();
            }
        }
        wasGrounded = isGrounded;
    }

    private void LateUpdate()
    {
        float dt = Time.deltaTime;
        //Vector2 grav = groundNormal * gravity * dt / Vector2.Dot(groundNormal, Vector2.down);
        Vector2 grav = gravity * dt * Vector2.down;
        velocity += grav;

        Vector3 velocity3D = velocity;
        velocity3D -= Vector3.forward * transform.position.z * Zcorrection;

        controller.Move(velocity3D * dt);
        checkGrounded(dt);

        Debug.Log(velocity);

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        velocity += ((Vector2)hit.normal) * Vector2.Dot(hit.normal, -velocity);
    }
}
