using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(physicsController2D))]
public class velocityReciever : MonoBehaviour {
    Vector2 __vel;
    bool wasSet;
    public Vector2 velocity {
        get {
            return __vel;
        }
        set {
            wasSet = true;
            __vel = value;
            pCtrl.moveVelocity(ref __vel, Time.fixedDeltaTime);
        }
    }
    physicsController2D pCtrl;

	void Start () {
        pCtrl = gameObject.GetComponent<physicsController2D>();
	}

	void FixedUpdate()
	{
        //Debug.Log(wasSet);
        //if(wasSet) {
        //    pCtrl.moveVelocity(ref __vel, Time.fixedDeltaTime);
        //}
        //__vel = Vector2.zero;
        wasSet = false;
	}
}
