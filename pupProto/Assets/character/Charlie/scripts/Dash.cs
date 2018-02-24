using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour {

    public dashSettings settings;
    [HideInInspector]public Player player;
    [HideInInspector]public playerStateMachine stateMachine;
    public delegate void OnFinish();
    public OnFinish onFinish;
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
        if(onFinish != null) {
            onFinish();
        }
        StartCoroutine("cooldownTimer");
    }

    Vector2 startPos;
    Vector2 dashDir;
    public bool doDash(float x, float y){
        if(isCooledDown){
            startPos = transform.position;
            player.velocity = Vector2.zero;
            dashTime = 0f;
            totalDist = 0f;
            dashDir.x = x;
            dashDir.y = y;
            dashDir.Normalize();
        }
        return isCooledDown;

    }

    public void setDir(float x, float y) {
        dashDir.x = x;
        dashDir.y = y;
        dashDir.Normalize();
    }


    float dashTime;
    float totalDist;
    public void doUpdate() {
        if(!isDoneFreeze) return;
        if(totalDist > settings.dashDistance) {
            StartCoroutine("freezeTimer");
        }
        else {
            float dS = settings.dashVelocity/(-settings.dashExp) * (1 - Mathf.Exp(settings.dashExp*dashTime));
            Vector2 prevPos = transform.position;
            player.controller.movePosition(dashDir*dS);
            totalDist += dS;
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
