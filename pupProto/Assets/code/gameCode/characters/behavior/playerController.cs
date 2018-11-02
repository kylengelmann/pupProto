using inputTypes;
using UnityEngine;

public class playerController : MonoBehaviour {
	
	Character character;
	contextInput input;
	
	void Start () {
		input = GameManager.current.context.input;
		input.onCheckInput += onCheckInput;
		character = gameObject.GetComponentInHierarchy<Character>();
	}
	
	void onCheckInput()
	{
    character.events.move.setMove.Invoke(input.getAxis(axisType.moveX));
    character.events.jump.setJump.Invoke(input.getButtonPressed(buttonType.a), input.getAxis(axisType.moveY) < -.5f);
		character.events.dash.setDash.Invoke(input.getAxis(axisType.dashX), input.getAxis(axisType.dashY));
    character.events.crouch.setCrouching.Invoke(input.getAxis(axisType.moveY) < -.5f);

    if (input.getButtonDown(buttonType.x))
		{
      combatSystem.attackType nextAttackType = combatSystem.attackType.neutral;

      if (Mathf.Abs(input.getAxis(axisType.moveX)) > .1f)
      {
        nextAttackType = combatSystem.attackType.side;
      }

      if (input.getAxis(axisType.moveY) < -.5f)
      {
        nextAttackType = combatSystem.attackType.down;
      }
      else if (input.getAxis(axisType.moveY) > .5f)
      {
        nextAttackType = combatSystem.attackType.up;
      }

      character.events.combat.setAttack.Invoke(nextAttackType);
		}
    if(input.getButtonDown(buttonType.b))
    {
        character.events.interaction.doInteract.Invoke();
    }

    if(Input.GetKeyDown(KeyCode.R))
    {
        world.activeWorld.resetWorld();
    }

	}
}
