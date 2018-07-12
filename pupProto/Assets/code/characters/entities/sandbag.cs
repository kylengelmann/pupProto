using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(physicsController2D))]
public class sandbag : MonoBehaviour
{
    [HideInInspector] public physicsController2D controller;
    [HideInInspector] public Vector2 vel;
    public float velocityTransfer = .1f;
    public float friction = 50f;
    public float timeInvincible = .2f;
    public Animator anim;
    [HideInInspector] public bool invincible = false;
    [HideInInspector] public hittable hittable;
    Character character;

    void Start()
    {
        controller = gameObject.GetComponent<physicsController2D>();
        character = GetComponent<Character>();
        vel = Vector2.zero;
        hittable = gameObject.GetComponentInChildren<hittable>();
        GetComponentInChildren<hittable>().hit += data =>
        {
            character.events.combat.onGotHit.Invoke(data);
        };
        character.events.combat.onGotHit += onHit;
    }

    void FixedUpdate()
    {
        vel.y -= 30f * Time.fixedDeltaTime;
        if (character.isGrounded)
        {
            if (Mathf.Abs(vel.x) > Mathf.Epsilon * 1000f)
            {
                float dV = Mathf.Sign(vel.x) * friction * Time.fixedDeltaTime;
                if (Mathf.Abs(vel.x) < Mathf.Abs(dV))
                {
                    vel.x = 0f;
                }
                else
                {
                    vel.x -= dV;
                }
            }
            else
            {
                vel.x = 0f;
            }
        }

        character.velocity = vel;

//		controller.moveVelocity(ref vel, Time.fixedDeltaTime);

        anim.SetBool("doneMoving", vel.sqrMagnitude > Mathf.Epsilon * 10f);
    }


    void OnCollisionStay2D(Collision2D other)
    {
        if (!invincible && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            vel.x += velocityTransfer * (other.gameObject.GetComponent<Character>().velocity.x - vel.x);
        }
    }

    private IEnumerator resetInvincible()
    {
        yield return new WaitForSeconds(timeInvincible);
        anim.ResetTrigger("gotHit");
        invincible = false;
        hittable.gameObject.SetActive(true);
    }

    public void onHit(attackData attack)
    {
//		if(!invincible) {
        invincible = true;
        hittable.gameObject.SetActive(false);
        anim.SetTrigger("gotHit");
        StartCoroutine("resetInvincible");
        vel = attack.attackForce;
//		}
    }
}