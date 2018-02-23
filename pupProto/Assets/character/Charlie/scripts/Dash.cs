using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour {

    public dashSettings settings;
    [HideInInspector]public Player player;
    [HideInInspector]public playerStateMachine stateMachine;
	// Use this for initialization
	void Start () {
        player = gameObject.GetComponent<Player>();
        stateMachine = gameObject.GetComponent<playerStateMachine>();
	}
	

    bool isCooledDown = true;
    IEnumerator cooldownTimer() {
        isCooledDown = false;
        yield return new WaitForSeconds(settings.dashCooldown);
        isCooledDown = true;
    }

    bool isDoneFreeze = true;
    IEnumerator freezeTimer() {
        isDoneFreeze = false;
        yield return new WaitForSeconds(settings.dashFreeze);
        isDoneFreeze = true;
        player.velocity.x = 0f;
        player.velocity.y = -player.settings.gravity*Time.fixedDeltaTime;
        stateMachine.currentState = playerStateMachine.playerState.free;
        StartCoroutine("cooldownTimer");
    }

    Vector2 startPos;
    Vector2 dashDir;
    public void doDash(Vector2 direction){
        if(isCooledDown){
            stateMachine.currentState = playerStateMachine.playerState.dashing;
            startPos = transform.position;
            player.velocity = Vector2.zero;
            dashTime = 0f;
            dS = 0f;
            dashDir = direction;
        }
    }


    float dashTime;
    float dS;
    public void doUpdate() {
        if(!isDoneFreeze) return;
        if(dS > settings.dashDistance) {
            StartCoroutine("freezeTimer");
        }
        else {
            //player.velocity = dashDir*(settings.dashVelocity*Mathf.Exp(settings.dashExp*dashTime));
            //float dashT = anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime;
            dS = settings.dashVelocity/(-settings.dashExp) * (1 - Mathf.Exp(settings.dashExp*dashTime));
            //dS = settings.dashVelocity*dashTime;
            player.controller.movePosition(dashDir*dS + startPos - (Vector2)transform.position);
            dashTime += Time.fixedDeltaTime;
        }
    }
}

[System.Serializable]
public struct dashSettings {
    public float dashVelocity;
    public float dashDistance;
    public float dashCooldown;
    public float dashFreeze;
    public float dashExp;
}
