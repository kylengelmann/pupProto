using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Dash : MonoBehaviour {

    public dashSettings settings;
    [HideInInspector]public Character Character;

    /// <summary>
    /// Is this component performing physics updates?
    /// </summary>
    [HideInInspector]public bool isActive = false;


    public delegate void OnFinish();

    /// <summary>
    /// What to do after the dash is complete.
    /// </summary>
    public OnFinish onFinish;


	void Start () {
        Character = gameObject.GetComponent<Character>();
	}
	
    /// <summary>
    /// Has the cooldown period ended?
    /// </summary>
    bool isCooledDown = true;

    /// Resets isCooledDown
    IEnumerator cooldownTimer() {
        isCooledDown = false;
        yield return new WaitForSeconds(settings.dashCooldown);
        isCooledDown = true;
    }

    /// <summary>
    /// Has the freeze period ended?
    /// </summary>
    bool isDoneFreeze = true;

    /// Resets isDoneFreeze
    IEnumerator freezeTimer() {
        isDoneFreeze = false;
        yield return new WaitForSeconds(settings.dashFreeze);
        isDoneFreeze = true;
        Character.velocity.x = 0f;
        Character.velocity.y = -Character.settings.gravity*Time.fixedDeltaTime;
        if(onFinish != null) {
            onFinish();
        }
        Character.events.dash.onDashEnd.Invoke();
        StartCoroutine("cooldownTimer");
    }

    /// <summary>
    /// The direction in which the player is dashing.
    /// </summary>
    Vector2 dashDir;

    /// <summary>
    /// Causes the player to start dashing, and sets the initial direction to
    /// dash in. Only available outside of the cooldown period
    /// </summary>
    /// <returns><c>true</c> if dash was done, <c>false</c> otherwise.</returns>
    /// <param name="x">The x value for the dash direction.</param>
    /// <param name="y">The y value for the dash direction.</param>
    public bool doDash(float x, float y){
        if(isCooledDown){
            Character.velocity = Vector2.zero;
            dashTime = 0f;
            totalDist = 0f;
            dashDir.x = x;
            dashDir.y = y;
            dashDir.Normalize();
            Character.events.dash.onDashUpdate.Invoke(x, y);
        }
        return isCooledDown;

    }


    /// <summary>
    /// Sets the direction to dash in.
    /// </summary>
    /// <param name="x">The x value for the dash direction.</param>
    /// <param name="y">The y value for the dash direction.</param>
    public void setDir(float x, float y) {
        dashDir = Vector2.Lerp(dashDir, new Vector2(x, y), settings.dashDirChange);
        dashDir.Normalize();


    }


    float dashTime;
    float totalDist;
    public void FixedUpdate() {
        if(!isActive || !isDoneFreeze) return;
        if(totalDist > settings.dashDistance) {
            StartCoroutine("freezeTimer");
        }
        else {
            // Integrate over the dashing period to find displacement
            float dS = settings.dashVelocity/(-settings.dashExp) * (1 - Mathf.Exp(settings.dashExp*dashTime)) - totalDist;
            //Vector2 prevPos = transform.position;
            Character.controller.movePosition(dashDir*dS);
            totalDist += dS;
            dashTime += Time.fixedDeltaTime;
        }
    }
}

[Serializable]
public struct dashSettings {
    public float dashVelocity;
    public float dashDistance;
    public float dashCooldown;
    public float dashFreeze;
    public float dashExp;
    public float dashDirChange;
}

public class dashEvents
{
    public safeAction onDashStart = new safeAction();
    public safeAction<float, float> onDashUpdate = new safeAction<float, float>();
    public safeAction onDashEnd = new safeAction();
}
