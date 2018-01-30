//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class moveSettings {
	public float speed = 5f;
	public float groundAcceleration = 35f;
	public float directionSwitchAcceleration = 50f;
	public float groundFriction = 45f;

	public enum JumpMode{
		gravitational,
		velocity1, 
		velocity2
	};

	public JumpMode jumpMode = JumpMode.gravitational;

	public float jumpVelocity = 12.5f;
	public float doubleJumpVelocity = 6f;
	public float jumpAcc = 30f;
	public float endJumpAcc = 125f;
	public float fallAcc = 30f;
	public float terminalVel = 30f;


	public float airControl = 5f;
	public float coyoteTime = .2f;
	public float minJumpTime = 0.15f;
	public float maxJumpTime = 0.3f;
	

	public float dashVelocity = 1f;
	public float dashCooldown = .1f;
	public float dashFreeze = .2f;
	public float dashExp = .18f;
  	public float dashConstant = .15f;

	public float backDashVelocity = 1f;
	public float backDashCooldown = .1f;
	public float backDashExp = .18f;

	public float airKickYVel = 7f;
	public float airKickXVel = .5f;
	public float airKickGrav = 30f;
}

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(characterController2D))]
public class playerController2D : MonoBehaviour {
	#region general variables
	public moveSettings movement;
	public Animator anim;
	public Vector2 groundBoxOffset;
	public Vector2 groundBoxSize;
	public Vector2 airBoxOffset;
	public Vector2 airBoxSize;

	private Hitter hitter;

	[HideInInspector] public Rigidbody2D rig;
	[HideInInspector] public characterController2D controller;
	[HideInInspector] public BoxCollider2D box;
	[HideInInspector] public LayerMask layer;
	private int animLayer;

	public enum action {
		free,
		kickin,
		dashin,
		backDashin,
	}
	[HideInInspector] public action currentAction = action.free;
	public Vector2 kickForce;
	#endregion
	
	#region main
	void Awake() {
		Global.player = this;
		layer = LayerMask.GetMask("Player");
		rig = gameObject.GetComponent<Rigidbody2D>();
		controller = gameObject.GetComponent<characterController2D>();
//		LayerMask contLM = new LayerMask();
//		contLM.value = ~layer.value;
//		controller.contactFilter.SetLayerMask(contLM);
		box = gameObject.GetComponent<BoxCollider2D>();

		if(!saveNLoad.loadPrefs()){
			Debug.Log("Unable to find settings file, using defaults and writing file");

			Global.player.anim.SetFloat("walkSpeed", 2.8f);

			saveNLoad.savePrefs();
		}
			
		animLayer = anim.GetLayerIndex("Base Layer");
		box.offset = airBoxOffset;
		box.size = new Vector2(airBoxSize.x - controller.skinWidth*2, airBoxSize.y - controller.skinWidth*2);

		hitter = gameObject.GetComponentInChildren<Hitter>();
		hitter.enabled = false;
//		hitter.hitSomething = hitSomething;
	}
	
	
	


	#region physics
	bool wasGrounded = false;
	float airT = 0f;

	void checkGrounded() {
		if (controller.grounded) {
			anim.SetBool("jumpin", false);
			canDash = true;
			jumped = 0;
			airT = 0f;
			if (!wasGrounded)
			{
				box.offset = groundBoxOffset;
				box.size = new Vector2(groundBoxSize.x - controller.skinWidth*2, groundBoxSize.y - controller.skinWidth*2);
			}
			wasGrounded = true;
		}
		else {
			if(wasGrounded){
				airT = 0f;
				box.offset = airBoxOffset;
				box.size = new Vector2(airBoxSize.x - controller.skinWidth*2, airBoxSize.y - controller.skinWidth*2);
			}
			else {
				airT += Time.fixedDeltaTime;
			}
			wasGrounded = false;
			anim.SetBool("jumpin", true);
			anim.SetBool("walkin", false);
		}
	}

	public void setAirVel() {
    	switch (movement.jumpMode) {
			case moveSettings.JumpMode.gravitational: {

				if (vel.y > 0f)
				{
					if (moveAxis.y <= Mathf.Epsilon && airT > movement.minJumpTime)
					{
						vel.y -= movement.endJumpAcc * Time.fixedDeltaTime;
					}
					else
					{
						vel.y -= movement.jumpAcc * Time.fixedDeltaTime;
					}
				}
				else {
					vel.y -= movement.fallAcc*Time.fixedDeltaTime;
				}
				break;
			}
			case moveSettings.JumpMode.velocity1:
			{
				if (vel.y > 0f)
				{
					vel.y -= movement.jumpAcc * Time.fixedDeltaTime;
					if (moveAxis.y <= Mathf.Epsilon && airT > movement.minJumpTime)
					{
						vel.y = 0f;
					}
				}
				else
				{
					vel.y -= movement.fallAcc*Time.fixedDeltaTime;	
				}
				break;
			}
			case moveSettings.JumpMode.velocity2:
			{
				
				if (airT > movement.minJumpTime && moveAxis.y <= Mathf.Epsilon && vel.y > 0f)
				{
					vel.y = 0f;
				}
				else if (airT > movement.maxJumpTime && vel.y > 0f)
				{
					vel.y -= movement.endJumpAcc * Time.fixedDeltaTime;
				}
				else if(vel.y <= 0f)
				{
					vel.y -= movement.fallAcc*Time.fixedDeltaTime;
				}
				break;
			}
		}
		vel.y = Mathf.Max(vel.y, -movement.terminalVel);
	}

	
	
	
	
	
	[HideInInspector]public Vector2 vel = Vector2.zero;

	bool dirChange = false;
	void FixedUpdate() {
		dashCooldown -= Time.fixedDeltaTime;
		switch(currentAction) {
			
			
			
			#region action.free
			case action.free:
				if(Mathf.Abs(vel.x) > Mathf.Epsilon && Mathf.Sign(moveAxis.x) != Mathf.Sign(vel.x)){
					dirChange = true;
				}
				float moveMod = 1f;
				if(!controller.grounded) {
					moveMod *= movement.airControl;
				}
				if(Mathf.Abs(moveAxis.x) < Mathf.Epsilon) {
					dirChange = false;
					if(Mathf.Abs(vel.x) < Mathf.Epsilon) {
						vel.x = 0f;
					}
					else {
						float dV = Mathf.Sign(vel.x)*movement.groundFriction*Time.fixedDeltaTime*moveMod;
						if(Mathf.Abs(vel.x) < Mathf.Abs(dV)) {
							vel.x = 0f;
						}
						else{
							vel.x -= dV;
						}
					}
				}
				else if(dirChange) {
					if(Mathf.Abs(vel.x) < Mathf.Abs(movement.speed*moveAxis.x) || Mathf.Sign(moveAxis.x) != Mathf.Sign(vel.x)) {
						vel.x += moveAxis.x*movement.directionSwitchAcceleration*Time.fixedDeltaTime*moveMod;
					}
					else {
						dirChange = false;
						vel.x = moveAxis.x*movement.speed;
					}
				}
				else if(Mathf.Abs(vel.x) < Mathf.Abs(movement.speed*moveAxis.x)) {
					vel.x += moveAxis.x*movement.groundAcceleration*Time.fixedDeltaTime*moveMod;	
				}
				else{
					vel.x = moveAxis.x*movement.speed;
					dirChange = false;
				}
				anim.SetFloat("walkSpeed", anim.GetFloat("animSpeed")*Mathf.Abs(vel.x)/movement.speed);
				setAirVel();
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
				controller.moveVelocity(ref vel, Time.fixedDeltaTime);

				break;
			#endregion
			
			
			
			#region action.dashin
			case action.dashin:

				if(dashFreeze > Mathf.Epsilon) {
					dashFreeze -= Time.fixedDeltaTime;
					
					if(dashFreeze <= Mathf.Epsilon) {
						anim.SetBool("dashEnd", true);
						dashFreeze = 0f;
						currentAction = action.free;
						vel.x = 0f;
						vel.y = 0f;
						setAirVel();
						controller.moveVelocity(ref vel, Time.fixedDeltaTime);
						
						
					}
				}
				else {
					float dashT = anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime;
          			float dX = movement.dashVelocity * (1 - Mathf.Exp(-movement.dashExp*(dashT+movement.dashConstant)));
					//float dX = movement.dashVelocity/movement.dashExp * (1 - Mathf.Exp(-movement.dashExp*dashT));
					if(!dashRight) {
						dX *= -1f;
					}
					controller.movePosition(Vector2.right*dX + startPos - rig.position);
				}
				break;
			#endregion

			
			
			#region action.kick
			case action.kickin:
				if(!controller.grounded || jumped > 0) {
					if(!controller.grounded) {
//						setAirVel();	
//						vel.y = 0f;
						vel.y -= movement.airKickGrav * Time.fixedDeltaTime;
					}

					controller.moveVelocity(ref vel, Time.fixedDeltaTime);
				}
				else {
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
					vel = Vector2.zero;
					setAirVel();
					controller.moveVelocity(ref vel, Time.fixedDeltaTime);
				}
				break;
			#endregion

			
			
			
			#region action.backDashin
			case action.backDashin:
				if(controller.grounded) {
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
					if(firstGrounded) {
						float dashT = anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime;
						float dX = movement.backDashVelocity*(dashT - 1/(movement.backDashExp + 1f) * Mathf.Pow(dashT, movement.backDashExp + 1f));
						vel.x = movement.backDashVelocity*(1f - Mathf.Pow(dashT, movement.backDashExp));
						if(!dashRight) {
							dX *= -1f;
						}
						controller.movePosition(Vector2.right*(dX + startPos.x - rig.position.x));
						vel.x = 0f;
						setAirVel();
						controller.moveVelocity(ref vel, Time.fixedDeltaTime);
						vel.x = movement.backDashVelocity*(1f - Mathf.Pow(dashT, movement.backDashExp));
						if(!dashRight) {
							vel.x *= -1f;
						}
					}
					else {
						vel.x -= Mathf.Sign(vel.x)*movement.groundFriction*Time.fixedDeltaTime;
						vel.y -= movement.fallAcc*Time.fixedDeltaTime;
						controller.moveVelocity(ref vel, Time.fixedDeltaTime);
					}
				}
				else {
					firstGrounded = false;
					if (Mathf.Abs(vel.x) >= movement.speed)
					{
						vel.x = movement.speed * Mathf.Sign(vel.x);
					}
					setAirVel();
					controller.moveVelocity(ref vel, Time.fixedDeltaTime);
				}

				break;
			#endregion
		}
		
		
		checkGrounded();
		anim.SetFloat("fallin", vel.y);
	}

	#endregion

	#region control 
	
	byte jumped = 0;
	public void Jump() 
	{
		if ((currentAction == action.backDashin || currentAction == action.free))
		{
			if ((controller.grounded || airT < movement.coyoteTime) && (jumped == 0))
			{
				if (Input.GetAxisRaw("Vertical") > -0.5f)
				{
					jumped = 1;
					vel.y = movement.jumpVelocity;
				}
				else
				{
					controller.dropThroughOneWay = true;
				}
			}
			else if (jumped < 2)
			{
				jumped = 2;
				vel.y = movement.doubleJumpVelocity;
			}
		}
	}

	
	
	bool dashRight;
	Vector2 startPos;
	bool canDash = true;
	public void Dash() {
		if(currentAction == action.free && canDash && dashCooldown<Mathf.Epsilon) {
			if(dashCooldown <= Mathf.Epsilon){
				startPos = rig.position;
				currentAction = action.dashin;
				if(transform.localScale.x < 0f /*sRend.flipX*/) {
					dashRight = false;
				}
				else {
					dashRight = true;
				}
				anim.SetBool("dashin", true);	
				vel.y = 0f;
			}	
		}
	}

	float dashCooldown = 0f;
	float dashFreeze = 0f;
	public void endDash() {
		if(!controller.grounded) {
			canDash = false;
		}
		anim.SetBool("dashin", false);
		anim.SetBool("dashEnd", false);
		dashCooldown = movement.dashCooldown;
		dashFreeze = movement.dashFreeze;
	}

	
	
	public void Kick() {
		if(currentAction == action.free) {
			if(controller.grounded) {
				if(jumped > 0) {
					anim.SetBool("jumpin", true);
				}
				else {
					vel = Vector2.zero;
				}
			}
			else {
				vel.y = movement.airKickYVel;
				vel.x *= movement.airKickXVel;
			}
			anim.SetBool("kickin", true);
			currentAction = action.kickin;
		}
	}

	public void endKick() {
		hitter.enabled = false;
		anim.SetBool("kickin", false);
		currentAction = action.free;
	}

//	public void hitSomething(Hittable gotHit) {
//		if(!controller.grounded) {
//			vel.y = 10f;
//		}
//	}
	
	public void startAttack() {
		hitter.enabled = true;
		hitter.force = kickForce;
		hitter.force.x *= Mathf.Sign(transform.localScale.x);
	}

	public void endAttack() {
		hitter.enabled = false;
	}

	
	
	bool firstGrounded;
	public void backDash() {
		if(currentAction == action.free && (controller.grounded && (jumped == 0))) {
			if(transform.localScale.x < 0f) {
				dashRight = true;
			}
			else {
				dashRight = false;
			}
			startPos = rig.position;
			currentAction = action.backDashin;
			firstGrounded = true;
			anim.SetBool("backDash", true);
		}
	}

	public void endBackDash() {
		anim.SetBool("backDash", false);
		currentAction = action.free;
	}

	
	
	Vector2 moveAxis = Vector2.zero;
	public void setMoveAxis(float axis, bool jumpButt) 
	{
		if(axis > 0f) {
			if(currentAction == action.free || currentAction == action.dashin) {
//				sRend.flipX = false;
				transform.localScale = new Vector3(1f, 1f);
			}
			if(controller.grounded && currentAction == action.free) {
				anim.SetBool("walkin", true);
			}
		}
		else if(axis < 0f) {
			if(currentAction == action.free || currentAction == action.dashin) {
//				sRend.flipX = true;
				transform.localScale = new Vector3(-1f, 1f);
			}
			if(controller.grounded && currentAction == action.free) {
				anim.SetBool("walkin", true);
			}
		}
		else {
			anim.SetBool("walkin", false);
		}
		moveAxis.x = axis;
		if(jumpButt) {
			moveAxis.y = 1f;
		}
		else {
			moveAxis.y = 0f;
		}
	}
	#endregion
	#endregion

}