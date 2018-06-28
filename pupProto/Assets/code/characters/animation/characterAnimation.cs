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
	}
	
	public void walk()
	{
		anim.SetBool("walkin", true);
	}
	
	public void setWalk(float speed)
	{
		anim.SetFloat("walkSpeed", Mathf.Abs(speed));
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
//		anim.SetBool("walkin", false);
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

	void Update()
	{
		anim.SetFloat("fallin", character.velocity.y);
	}
}
