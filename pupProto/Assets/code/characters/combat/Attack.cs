using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour {

    public enum attackState {
        busy,
        canRecordNext,
        canStartNext,
		done
    }
    [HideInInspector] public attackState state;

    public Vector2 attackForce;
	public float timeCanRecordNext;
	public float timeCanStartNext;

    public Collider2D hitArea;
    Collider2D[] colliders;


    public ContactFilter2D filter;

    protected virtual void Awake () {
        colliders = new Collider2D[8];
    }

    public void startAttack() {
		//if(enabled) {
	        state = attackState.busy;
	        onStartAttack();
	        StartCoroutine(attackTimer());
		//}
    }

    protected void startHit() {
		//if(enabled) {
			onStartHit();
			StartCoroutine(doHits());	
		//}
    }

    protected void endHit() {
		//if(enabled) {
			StopCoroutine(doHits());
			onEndHit();	
		//}
    }

    public void endAttack() {
		//if(enabled) {
			StopAllCoroutines();
			onEndAttack();
			state = attackState.done;
		//}
    }

    protected virtual void onStartAttack() {}

    protected virtual void onStartHit() {}

    protected virtual void onEndHit() {}

	protected virtual void onCanRecordNext() {}

	protected virtual void onCanStartNext() {}

    protected virtual void onEndAttack() {}

    IEnumerator attackTimer() {
        yield return new WaitForSeconds(timeCanRecordNext);
		state = attackState.canRecordNext;
		onCanRecordNext();
		yield return new WaitForSeconds(timeCanStartNext - timeCanRecordNext);
		state = attackState.canStartNext;
		onCanStartNext();
    }

    IEnumerator doHits() {
        while(true) {
            int numHits = hitArea.OverlapCollider(filter, colliders);
            for(int i = 0; i < numHits; ++i) {
                Hittable hittable;
                if(hittable = colliders[i].GetComponent<Hittable>()) {
                    hittable.onHit(attackForce);
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
