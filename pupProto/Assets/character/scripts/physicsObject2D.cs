using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class physicsObject2D : MonoBehaviour {

    public Vector2 boxOffset;
    public Vector2 boxSize;
    public float minSinSlope = 0.766f;
    public float skinWidth = 0.04f;
    public float minMove = 0.01f;
    public float hitTolerance = 0.08f;
    public float maxStairHeight = 0.1f;
    public LayerMask layerMask;
    public LayerMask oneWayPlatform;
    [HideInInspector] public bool dropThroughOneWay = false;
    [HideInInspector] public bool grounded = false;
    [HideInInspector] public BoxCollider2D box;
    [HideInInspector] public RaycastHit2D[] hits;

    public struct objectHit
    {
        public RaycastHit2D x;
        public RaycastHit2D y;
    }

	// Use this for initialization
	void Start () {
        hits = new RaycastHit2D[10];
	}
	
    public objectHit movePosition(Vector2 dS) {
        objectHit hit;
        hit.x = new RaycastHit2D();
        hit.y = new RaycastHit2D();

        if(dS.sqrMagnitude < minMove*minMove) {
            return hit;
        }

        // Handle each direction separately
        float dX = Mathf.Abs(dS.x) + skinWidth;
        float dY = Mathf.Abs(dS.y) + skinWidth;
        Vector2 hitNormal;

        Vector2 origin = transform.position;
        origin += boxOffset;

        // If moving up, handle up motion first
        if(dS.y > 0f) {
            int numHits = Physics2D.BoxCastNonAlloc(origin, boxSize, 0f, Vector2.up, hits, dY, layerMask);

            // Only do if we hit something
            if(numHits > 0) {

                // Find the closest hit
                for(int i = 0; i < numHits; ++i) {
                    // See if this is closer than our current closest hit
                    if(hits[i].distance <= dY) {
                        int layer = 1 << hits[i].transform.gameObject.layer;
                        //While moving up, ignore one way platforms
                        if(layer != oneWayPlatform.value) {
                            dY = hits[i].distance;
                            hit.y = hits[i];
                            hitNormal = hit.y.normal;
                        }
                    }
                }

                // Check for objects inside of this and push this out of them
                if(dY <= 0 && hit.y.transform != null) {
                    float distIn = hit.y.point.y - boxOffset.y + boxSize.y*.5f;
                }

            }
        }



        return hit;
    }
}
