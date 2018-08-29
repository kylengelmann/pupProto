using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Dash : MonoBehaviour {

    public dashSettings settings;
    [HideInInspector]public Character character;

    /// <summary>
    /// Is this component performing physics updates?
    /// </summary>
    [HideInInspector]public bool isActive = true;

    public int numWinds = 3;

    int windsLeft;


	void Start () {
        character = gameObject.GetComponent<Character>();
	    character.events.dash.setAcive += setActive;
	    character.events.dash.setDash += doDash;
	}
    
    void setActive(bool active)
    {
        isActive = active;
        if(!isActive && isDashing)
        {
            StopCoroutine(freezeTimer());
            StartCoroutine(cooldownTimer());
        }
    }
	
    /// <summary>
    /// Has the cooldown period ended?
    /// </summary>
    bool isCooledDown = true;

    /// Resets isCooledDown
    IEnumerator cooldownTimer() {
        isCooledDown = false;
        yield return new WaitForSeconds(settings.dashCooldown);
        character.events.dash.onCooldownEnd.Invoke();
        isCooledDown = true;
    }

    /// <summary>
    /// Has the freeze period ended?
    /// </summary>
    bool isDoneFreeze = true;

    /// Resets isDoneFreeze
    IEnumerator freezeTimer() {
        isDoneFreeze = false;
        character.velocity = Vector2.zero;
        yield return new WaitForSeconds(settings.dashFreeze);
        isDoneFreeze = true;
        character.velocity.x = 0f;
        //character.velocity.y = -character.settings.gravity*Time.fixedDeltaTime;
        isDashing = false;
        character.events.dash.onDashEnd.Invoke();
        StartCoroutine(cooldownTimer());
    }

    /// <summary>
    /// The direction in which the player is dashing.
    /// </summary>
    Vector2 dashDir;
    bool isDashing;
    /// <summary>
    /// Causes the player to start dashing, and sets the initial direction to
    /// dash in. Only available outside of the cooldown period
    /// </summary>
    /// <returns><c>true</c> if dash was done, <c>false</c> otherwise.</returns>
    /// <param name="x">The x value for the dash direction.</param>
    /// <param name="y">The y value for the dash direction.</param>
    public void doDash(float x, float y){
        if(x*x + y*y < .3f || !isCooledDown)
        {
            return;
        }
        if(isCooledDown){
            dashDir.x = x;
            dashDir.y = y;
            dashDir.Normalize();
            if(!isDashing)
            {
                isDashing = true;
                windsLeft --;
                character.velocity = Vector2.zero;
                dashTime = 0f;
                totalDist = 0f;
                character.gravity = 0f;
                character.events.dash.onDashStart.Invoke();
            }
        }
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
        if(!isActive || !isDoneFreeze || !isDashing) return;
        if(totalDist > settings.dashDistance) {
            StartCoroutine(freezeTimer());
        }
        else {
            // Integrate over the dashing period to find displacement
            float dS = settings.dashVelocity/(settings.dashExp) * (1 - Mathf.Exp(-settings.dashExp*dashTime)) - totalDist;
            //Vector2 prevPos = transform.position;
            character.velocity = dashDir*dS/Time.fixedDeltaTime;
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
    public safeAction<float, float> setDash = new safeAction<float, float>();
    
    public safeAction onDashStart = new safeAction();
    public safeAction onDashEnd = new safeAction();
    public safeAction onCooldownEnd = new safeAction();
    
    public safeAction<bool> setAcive = new safeAction<bool>();
}
