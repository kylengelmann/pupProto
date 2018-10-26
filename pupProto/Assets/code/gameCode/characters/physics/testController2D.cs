using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testController2D : MonoBehaviour {

    public float skinWidth;
    Rigidbody2D rig;
    ContactPoint2D[] contacts;

	void Start () {
		rig = GetComponent<Rigidbody2D>();
        if(rig == null)
        {
            Destroy(this);
            throw new System.Exception("No collider on test controller gameobject");
        }

        contacts = new ContactPoint2D[16];
	}

    public void move(Vector2 dS)
    {
        rig.MovePosition(rig.position + dS);
        int numContacts = rig.GetContacts(contacts);
        Vector2 resolvedPosition = rig.position;
        for(int i = 0; i < numContacts; i++)
        {
            resolvedPosition += contacts[i].normal*(skinWidth - contacts[i].separation);
        }
        rig.MovePosition(resolvedPosition);
    }

    public void moveVelocity(ref Vector2 velocity, float dt)
    {
        Vector2 dS = velocity*dt;
        dS += dS.normalized*skinWidth;
        transform.position = transform.position + (Vector3)dS;
        Physics2D.SyncTransforms();
        int numContacts = rig.GetContacts(contacts);
        Vector2 resolvedPosition = transform.position;
        for (int i = 0; i < numContacts; i++)
        {
            Debug.Log(contacts[i].separation);
            resolvedPosition += contacts[i].normal * (skinWidth - contacts[i].separation/2f);
            velocity -= Vector2.Dot(contacts[i].normal, velocity) * contacts[i].normal;
        }
        transform.position = resolvedPosition;
    }

}
