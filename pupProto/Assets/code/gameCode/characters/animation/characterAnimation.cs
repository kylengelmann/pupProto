﻿using UnityEngine;

public class characterAnimation : MonoBehaviour {

	Character character;
	Animator anim;
	
	void Start () {
		anim = gameObject.GetComponentInHierarchy<Animator>();
		character = gameObject.GetComponentInHierarchy<Character>();
		characterEventHandler events = character.events;
		events.move.onMove += walk;
		events.move.setMove += setWalk;
		events.move.onStopMove += stopWalk;
		events.character.onLeaveGround += jump;
		events.character.onGrounded += land;
		events.combat.onAttack += attack;
    events.wall.onWallSlide += onWall;
    events.wall.offWall += offWall;
    events.jump.onJump += actualJump;
    events.dash.onDashStart += dashStart;
    events.dash.onDashEnd += dashEnd;
  }

    bool walking;

	public void walk()
	{
		anim.SetBool("walkin", true);
        walking = true;
	}
	
	public void setWalk(float speed)
	{
        if(isOnWall) return;
		anim.SetFloat("walkSpeed", Mathf.Abs(speed));
        if(Mathf.Abs(speed) < .1f) return;
        //transform.localScale = new Vector3(Mathf.Sign(speed), 1f);

    }
	
	public void stopWalk()
	{
		anim.SetBool("walkin", false);
        walking = false;
	}

	public void dashStart()
  {
    anim.SetBool("dashin", true);
  }

  public void dashEnd()
  {
    anim.SetBool("dashin", false);
  }

	public void dash(float x, float y)
	{
		
	}
	
  public void actualJump()
  {
    anim.ResetTrigger("jumpinTrig");
    anim.SetTrigger("jumpinTrig");
  }

	public void jump()
	{
		anim.SetBool("jumpin", true);
	}
	
	public void land()
	{
		anim.SetBool("jumpin", false);
	}
	
	public void attack(combatSystem.attackType attack, int attacksDone)
	{
		anim.ResetTrigger("kickinTrig");
		anim.SetTrigger("kickinTrig");
    anim.SetInteger("comboCounter", attacksDone);
    anim.SetInteger("nextAttackType", (int) attack);
	}

    bool isOnWall;
    public void onWall(bool onRightWall)
    {
        isOnWall = true;
        anim.SetBool("onWall", true);
        transform.localScale = new Vector3(onRightWall ? 1 : -1, 1f);
    }

    public void offWall()
    {
        isOnWall = false;
        anim.SetBool("onWall", false);
    }

	void Update()
	{
    float lastFall = anim.GetFloat("fallin");
		anim.SetFloat("fallin", character.velocity.y);
    anim.SetBool("fallinFast", lastFall < -20f);
	}
}
