using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(physicsController2D))]
public class velocityReciever : MonoBehaviour {
    Vector2 _vel;
//    bool wasSet;
    public Vector2 velocity {
        get {
            return _vel;
        }
        set {
//            wasSet = true;
            _vel = value;
            pCtrl.moveVelocity(ref _vel, Time.fixedDeltaTime);
        }
    }
    physicsController2D pCtrl;

	void Start () {
        pCtrl = gameObject.GetComponent<physicsController2D>();
	}

//	void FixedUpdate()
//	{
//        //Debug.Log(wasSet);
//        //if(wasSet) {
//        //    pCtrl.moveVelocity(ref __vel, Time.fixedDeltaTime);
//        //}
//        //__vel = Vector2.zero;
//        wasSet = false;
//	}
}
