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
            character.controller.doMove(_vel*Time.fixedDeltaTime);
        }
    }
    Character character;

	void Start () {
        character = transform.GetComponent<Character>();
        character.events.character.onPositionUpdate += onPositionUpdate;
    }



    velocityGiver velGive;

    void onPositionUpdate(RaycastHit2D hit1, RaycastHit2D hit2)
    {
        velocityGiver vg = null;
        if (hit2.collider != null)
        {
            vg = hit2.collider.GetComponent<velocityGiver>();
        }
        else if(hit1.collider != null)
        {
            vg = hit1.collider.GetComponent<velocityGiver>();
        }

        if(velGive != null && vg != velGive)
        {
            velGive.removeReciever(this);
            velGive = null;
        }
        if(velGive == null && vg != null)
        {
            velGive = vg;
            velGive.addReciever(this);
        }
        
    }

}
