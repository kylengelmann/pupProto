using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Hitter : MonoBehaviour {

	public delegate void hitAction(Hittable gotHit);

	public hitAction hitSomething;

	public ContactFilter2D filter;
	[HideInInspector]public BoxCollider2D box;
	public Vector2 force;

	private Collider2D[] results;

	void Start() {
		box = gameObject.GetComponent<BoxCollider2D>();
		results = new Collider2D[10];
	}
	
	void FixedUpdate() {
		int numHits = box.OverlapCollider(filter, results);
		for(int i = 0; i < numHits; i++) {
			Hittable h = results[i].gameObject.GetComponent<Hittable>();
			if(h != null) {
                h.hitMe(force);
				if(hitSomething != null) {
					hitSomething(h);
				}
			}
		}
	}
}
