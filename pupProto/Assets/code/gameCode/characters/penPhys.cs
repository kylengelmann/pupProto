using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class penPhys : MonoBehaviour {

	new BoxCollider collider;
    Collider[] overlapping;

    private void Start()
    {
        collider = GetComponent<BoxCollider>();
        overlapping = new Collider[8];
    }

    public void move(Vector2 dS)
    {
        Vector3 movePos = transform.position + (Vector3)dS;
        int numOverlap = Physics.OverlapBoxNonAlloc(movePos, collider.size/2, overlapping, transform.rotation);
        Vector3 resolve = Vector3.zero;
        for(int i = 0; i < numOverlap; i++)
        {
            Vector3 direction;
            float distance;
            Physics.ComputePenetration(collider, movePos, transform.rotation, 
                                       overlapping[i], overlapping[i].transform.position, overlapping[i].transform.rotation, 
                                       out direction, out distance);

            float dDotR = Vector3.Dot(direction, resolve);
            resolve += direction * (distance - dDotR);

        }
        transform.position = movePos + resolve;
    }

    public void moveVelocity(ref Vector2 velocity, float dt)
    {
        Vector2 dS = velocity * dt;
        Vector3 movePos = transform.position + (Vector3)dS;
        int numOverlap = Physics.OverlapBoxNonAlloc(movePos, collider.size / 2, overlapping, transform.rotation);
        Vector3 resolve = Vector3.zero;
        for (int i = 0; i < numOverlap; i++)
        {
            if (overlapping[i] == collider) continue;
            Vector3 direction;
            float distance;
            bool isOverlapping = Physics.ComputePenetration(collider, movePos, transform.rotation,
                                                            overlapping[i], overlapping[i].transform.position, overlapping[i].transform.rotation,
                                                            out direction, out distance);
            if (isOverlapping)
            {
                float dDotR = Vector3.Dot(direction, resolve);
                resolve += direction * (distance - dDotR);

                velocity -= Vector2.Dot(velocity, direction) * (Vector2)direction;
            }
        }
        transform.position = movePos + resolve;
    }
}
