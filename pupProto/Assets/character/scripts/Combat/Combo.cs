using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Combo : MonoBehaviour {

	public StateMachine stateMachine;

	public enum AttackDir {
		empty,
		none,
		up,
		down,
		left,
		right
	}


	AttackDir queuedAttack;
    Attack currentAttack;



	protected virtual void Start ()
	{
		stateMachine = new StateMachine();
		stateMachine.setValue("queued", queuedAttack);
	}

	void Update () {
        if(queuedAttack != AttackDir.empty) {
            if(currentAttack == null || currentAttack.state >= Attack.attackState.canStartNext) {
				if(currentAttack != null) {
					if(currentAttack.state != Attack.attackState.done) {
						currentAttack.endAttack();
					}
					stateMachine.setValue("queued", queuedAttack);
					stateMachine.Tick();
				}
				currentAttack = ((AttackNode)stateMachine.currentNode).attack;
                queuedAttack = AttackDir.empty;
                currentAttack.startAttack();
            }
        }
		else if(currentAttack != null) {
			if(currentAttack.state == Attack.attackState.done) {
				stateMachine.setValue("queued", AttackDir.empty);
				stateMachine.setValue("reset", true);
				stateMachine.Tick();
				stateMachine.setValue("reset", false);
				currentAttack = null;
			}
		}
	}

    public void setAttack(AttackDir attackDir) {
		if(currentAttack == null) {
			if(queuedAttack == AttackDir.empty) {
				queuedAttack = attackDir;
				return;
			}
		}
		else {
			if(currentAttack.state >= Attack.attackState.canRecordNext && queuedAttack == AttackDir.empty) {
				queuedAttack = attackDir;
			}
		}
    }
}


public class AttackNode : StateNode {
	static int num = 0;
	int myNum;
	public Attack attack;

	public AttackNode(Attack attack) {
		myNum = num;
		++num;
		this.attack = attack;
	}

	//public override void onEnter ()
	//{
	//	Debug.Log("Hello from " + myNum.ToString());
	//}

	//public override void onExit ()
	//{
	//	Debug.Log("Goodbye from " + myNum.ToString());
	//}
}