using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class combatSystem : MonoBehaviour {
	
	public moveSet moveSet;
	public Collider2D hitArea;
	public ContactFilter2D affectedLayers;
	
	attackState	state;
	attackType currentAttack;
	attackType queuedAttack;
	int attacksDone;
	Character character;
	
	Collider2D[] hits;

    enum attackState {
        busy,
        canRecordNext,
        canStartNext,
        recovery,
        done
    }
	
	public enum attackType {
		none,
		nuetral,
		up,
		down,
		side,
		special
	}


	private void Start()
	{
		character = GetComponent<Character>();
		hits = new Collider2D[16];
		character.events.combat.setAttack += setAttack;
	}

	void Update () {
        if(queuedAttack != attackType.none) {
            if(currentAttack == attackType.none || state == attackState.canStartNext) {
				if(currentAttack != attackType.none) {
					//if(state != attackState.done) {
                    hitArea.enabled = false;
                    state = attackState.done;
                    character.events.combat.onFinishAttack.Invoke(currentAttack);
                    if (attacksDone == moveSet.comboLength)
                    {
                        character.events.combat.onFinishCombo.Invoke();
                    }
                    //}
				}
                startAttack();
            }
        }
		else if(currentAttack != attackType.none) {

		    checkHits();
	        
		}
	}

    public void setAttack(attackType attackType) {
	    if(attacksDone >= moveSet.comboLength)
	    {
		    return;
	    }
	    if(!moveSet.getAttack(attackType, attacksDone).enabled)
	    {
		    if(attackType < attackType.special && attackType != attackType.nuetral)
		    {
			    setAttack(attackType.nuetral);
		    }
		    return;
	    }
		if(currentAttack == attackType.none) {
			if(queuedAttack == attackType.none) {
				queuedAttack = attackType;
			}
		}
		else {
			if(state >= attackState.canRecordNext && queuedAttack == attackType.none) {
				queuedAttack = attackType;
			}
		}
    }
	
	public void startAttack()
	{
		hitArea.enabled = true;
		currentAttack = queuedAttack;
		queuedAttack = attackType.none;
		state = attackState.busy;
		attacksDone ++;
		character.events.combat.onAttack.Invoke(currentAttack);
	}
	
	public void allowRecord()
	{
		state = attackState.canRecordNext;
	}
	
	public void allowStartNext()
	{
		state = attackState.canStartNext;
	}

    public void startRecovery()
    {
        state = attackState.recovery;
    }
	
	public void endAttack()
	{
		hitArea.enabled = false;
		state = attackState.done;
		character.events.combat.onFinishAttack.Invoke(currentAttack);
		if(attacksDone == moveSet.comboLength)
		{
			character.events.combat.onFinishCombo.Invoke();
		}
        attacksDone = 0;
        currentAttack = attackType.none;
    }
	
	void checkHits()
	{
		if(hitArea == null) return;
		int numHits = hitArea.OverlapCollider(affectedLayers, hits);
		bool landedHit = false;
		for(int i = 0; i < numHits; i++)
		{
			hittable hittable = hits[i].GetComponent<hittable>();
			if(hittable != null)
			{
                attackData data = moveSet.getAttack(currentAttack, attacksDone);
                data.attackForce.x *= Mathf.Sign(transform.lossyScale.x);
                hittable.hit(data);
				landedHit = true;
			}
		}
		if(landedHit)
		{
			character.events.combat.onLandedHit.Invoke();
		}
		
	}
	
	
}

public class combatEvents
{
    public safeAction<combatSystem.attackType> setAttack = new safeAction<combatSystem.attackType>();
	public safeAction<combatSystem.attackType> onAttack = new safeAction<combatSystem.attackType>();
	public safeAction<combatSystem.attackType> onFinishAttack = new safeAction<combatSystem.attackType>();
	public safeAction onFinishCombo = new safeAction();
	public safeAction<attackData> onGotHit = new safeAction<attackData>();
	public safeAction onLandedHit = new safeAction();
}