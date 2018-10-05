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
    [HideInInspector] public GameObject hitbox;
    Character character;

    void Start()
    {
        
        controller = gameObject.GetComponent<physicsController2D>();
        character = GetComponent<Character>();
        vel = Vector2.zero;
        character.events.combat.onGotHit += onHit;
        characterHittable hittable = GetComponentInChildren<characterHittable>();
        hitbox = hittable.gameObject;
        character.gravity = 30f;
    }

    void Update()
    {
        vel = character.velocity;
        if (character.isGrounded)
        {
            if (Mathf.Abs(vel.x) > Mathf.Epsilon * 1000f)
            {
                float dV = Mathf.Sign(vel.x) * friction * Time.deltaTime;
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

        character.velocity.x = vel.x;

        anim.SetBool("doneMoving", vel.sqrMagnitude > Mathf.Epsilon * 10f);
    }


    void OnCollisionStay2D(Collision2D other)
    {
        if (!invincible && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            character.velocity.x += velocityTransfer * (other.gameObject.GetComponent<Character>().velocity.x - character.velocity.x);
        }
    }

    private IEnumerator resetInvincible()
    {
        yield return new WaitForSeconds(timeInvincible);
        anim.ResetTrigger("gotHit");
        invincible = false;
        hitbox.SetActive(true);
    }

    public void onHit(attackData attack)
    {
//		if(!invincible) {
        invincible = true;
        hitbox.SetActive(false);
        anim.SetTrigger("gotHit");
        StartCoroutine("resetInvincible");
        character.velocity = attack.attackForce;
//		}
    }
}