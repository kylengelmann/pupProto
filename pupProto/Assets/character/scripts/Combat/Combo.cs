using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo : MonoBehaviour {
	
	public Dictionary<AttackType, Attack> attacks = new Dictionary<AttackType, Attack>();
	
	public enum AttackType {
		empty = 0,
		normal = 1,
		up = 2,
		down = 3,
		left = 4,
		right = 5
	}

	AttackType queuedAttack;
    Attack currentAttack;
	int attacksDone;

	private void Start()
	{
		attacks.Add(AttackType.normal, GetComponent<Kick>());
	}

	void Update () {
        if(queuedAttack != AttackType.empty) {
            if(currentAttack == null || currentAttack.state >= Attack.attackState.canStartNext) {
				if(currentAttack != null) {
					if(currentAttack.state != Attack.attackState.done) {
						currentAttack.endAttack();
					}
				}
				currentAttack = attacks[queuedAttack];
                queuedAttack = AttackType.empty;
                currentAttack.startAttack();
	            attacksDone ++;
            }
        }
		else if(currentAttack != null) {
			if(currentAttack.state == Attack.attackState.done) {
				attacksDone = 0;
				currentAttack = null;
			}
		}
	}

    public void setAttack(AttackType attackType) {
	    Debug.Log(attacksDone);
	    if(attacksDone == 3) return;
		if(currentAttack == null) {
			if(queuedAttack == AttackType.empty) {
				queuedAttack = attackType;
			}
		}
		else {
			if(currentAttack.state >= Attack.attackState.canRecordNext && queuedAttack == AttackType.empty) {
				queuedAttack = attackType;
			}
		}
    }
}