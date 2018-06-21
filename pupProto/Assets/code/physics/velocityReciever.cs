using UnityEngine;

public class velocityReciever : MonoBehaviour {
    Vector2 _vel;
//    bool wasSet;
    public Vector2 velocity {
        get {
            return _vel;
        }
        set {
            _vel = value;
            character.controller.moveVelocity(ref _vel, Time.fixedDeltaTime);
        }
    }
    Character character;

	void Start () {
        character = transform.GetComponent<Character>();
        character.events.character.onPositionUpdate += onPositionUpdate;
    }

    velocityGiver velGive;
    void onPositionUpdate()
    {
        if(character.hit.y.collider == null)
        {
            return;
        }
        velocityGiver vg = character.hit.y.collider.GetComponent<velocityGiver>();
        if(vg != null && velGive == null)
        {
            vg.addReciever(this);
        }
        else if(vg == null && velGive != null)
        {
            velGive.removeReciever(this);
            character.velocity = _vel;
        }
        velGive = vg;
    }

}
