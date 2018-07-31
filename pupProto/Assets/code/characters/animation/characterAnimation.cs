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
	}
	
	public void walk()
	{
		anim.SetBool("walkin", true);
	}
	
	public void setWalk(float speed)
	{
        if(isOnWall) return;
		anim.SetFloat("walkSpeed", Mathf.Abs(speed));
        transform.localScale = new Vector3(Mathf.Sign(speed), 1f);
    }
	
	public void stopWalk()
	{
		anim.SetBool("walkin", false);
	}
	
	public void dash(float x, float y)
	{
		
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
        character.anim.SetBool("onWall", true);
        transform.localScale = new Vector3(onRightWall ? 1 : -1, 1f);
    }

    public void offWall()
    {
        isOnWall = false;
        character.anim.SetBool("onWall", false);
    }

	void Update()
	{
		anim.SetFloat("fallin", character.velocity.y);
	}
}
