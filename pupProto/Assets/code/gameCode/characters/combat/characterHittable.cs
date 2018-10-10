using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterHittable : hittable {

    Character character;

	void Start () {
		character = gameObject.GetComponentInHierarchy<Character>();
	}

    public override void hit(attackData data)
    {
        character.events.combat.onGotHit.Invoke(data);
    }
}
