using UnityEngine;

public class characterAnimation : MonoBehaviour {

	Character character;
	Animator anim;
	
	void Start () {
		anim = GetComponent<Animator>();
		character = GetComponent<Character>();
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
        transform.localScale = new Vector3(Mathf.Sign(speed), 1f);

    }
	
	public void stopWalk()
	{
		anim.SetBool("walkin", false);
        walking = false;
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
	
	public void attack(combatSystem.attackType attack)
	{
		anim.ResetTrigger("kickinTrig");
		anim.SetTrigger("kickinTrig");
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
		anim.SetFloat("fallin", character.velocity.y);
	}
}
