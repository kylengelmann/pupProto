using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.Events;


public class Hittable: MonoBehaviour{

	public delegate void hitAction(Vector2 force);

	public hitAction onHit;

    public void hitMe(Vector2 force){
        if(onHit != null) {
            onHit(force);
        }
    }
}
