using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.Events;


public class Hittable: MonoBehaviour{

	public delegate void hitAction(Hitter hitter);

	public hitAction onHit;

	public void _onHit(Hitter hitter) {
		if(onHit != null) {
			onHit(hitter);
		}
	}
}
