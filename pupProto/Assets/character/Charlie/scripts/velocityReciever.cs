using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Character character;

	void Start () {
        //pCtrl = gameObject.GetComponent<physicsController2D>();
        character = transform.parent.GetComponent<Character>();
        pCtrl = character.GetComponent<physicsController2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        velocityGiver velGive = other.GetComponent<velocityGiver>();
        if(velGive != null)
        {
            velGive.addReciever(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        velocityGiver velGive = other.GetComponent<velocityGiver>();
        if(velGive != null)
        {
            velGive.removeReciever(this);
            character.velocity += _vel;
        }
    }
}
