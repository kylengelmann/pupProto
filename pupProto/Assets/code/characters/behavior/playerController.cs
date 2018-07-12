using inputTypes;
using UnityEngine;

public class playerController : MonoBehaviour {
	
	Character character;
	contextInput input;
	
	void Start () {
		input = GameManager.current.context.input;
		input.onCheckInput += onCheckInput;
		character = GetComponent<Character>();
	}
	
	void onCheckInput()
	{
        character.events.move.setMove.Invoke(input.getAxis(axisType.moveX));
        //character.events.move.setMove.Invoke(1f);
        character.events.jump.setJump.Invoke(input.getButtonPressed(buttonType.a), input.getAxis(axisType.moveY) < -.5f);
		character.events.dash.setDash.Invoke(input.getAxis(axisType.dashX), input.getAxis(axisType.dashY));
		if(input.getButtonDown(buttonType.x))
		{
			character.events.combat.setAttack.Invoke(combatSystem.attackType.nuetral);
		}
	}
}
