﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(characterController2D))]
public class sandbag : MonoBehaviour {
	[HideInInspector] public characterController2D controller;
	[HideInInspector] public Vector2 vel;
	public float velocityTransfer = .1f;
	public float friction = 50f;
	public Animator anim;
	[HideInInspector] public bool invincible = false;
	[HideInInspector] public Hittable hittable;

	void Start () {
		controller = gameObject.GetComponent<characterController2D>();
		vel = Vector2.zero;
		hittable = gameObject.GetComponentInChildren<Hittable>();
		hittable.onHit = onHit;
	}

	void FixedUpdate () {
		vel.y -= Global.player.movement.fallAcc*Time.fixedDeltaTime;
		if(controller.grounded) {
			if(Mathf.Abs(vel.x) > Mathf.Epsilon*1000f) {
				float dV = Mathf.Sign(vel.x)*friction*Time.fixedDeltaTime;
				if(Mathf.Abs(vel.x) < Mathf.Abs(dV)) {
					vel.x = 0f;
				}
				else {
					vel.x -= dV;
				}
			}
			else {
				vel.x = 0f;
			}
		}
		controller.moveVelocity(ref vel, Time.fixedDeltaTime);

		anim.SetBool("doneMoving", vel.sqrMagnitude > Mathf.Epsilon*10f);

	}
		

	void OnCollisionStay2D(Collision2D other) {
		if(!invincible && other.gameObject.layer == LayerMask.NameToLayer("Player")) {
			vel.x += velocityTransfer*(other.gameObject.GetComponent<playerController2D>().vel.x - vel.x);
		}
	}

	private IEnumerator resetInvincible() {
		yield return new WaitForSeconds(.1f);
		anim.ResetTrigger("gotHit");
		invincible = false;
		hittable.enabled = true;
	}

	public void onHit(Hitter hitter) {
//		if(!invincible) {
		invincible = true;
		hittable.enabled = false;
		anim.SetTrigger("gotHit");
		StartCoroutine("resetInvincible");
		vel = hitter.force;
//		}
	}

}