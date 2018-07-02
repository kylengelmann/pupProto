using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class physicsController2D : MonoBehaviour
{
	public float skinWidth = 0.02f;
	public float minMove = 0.01f;
	public ContactFilter2D contactFilter;
	public LayerMask oneWayPlatform;
	[HideInInspector] public bool dropThroughOneWay = false;
	[HideInInspector] public BoxCollider2D box;
	[HideInInspector] public RaycastHit2D[] hits;

    Collider2D droppingThrough = null;

    void Awake()
	{
		box = gameObject.GetComponent<BoxCollider2D>();
		hits = new RaycastHit2D[10];
	}
	

	public RaycastHit2D movePosition(Vector2 dS)
	{
		float distance = dS.magnitude;
        Vector2 dSNorm = dS.normalized;
		RaycastHit2D hit = new RaycastHit2D();
        if(distance < minMove) return hit;
        float minDist = distance + skinWidth;
        int numHits = box.Cast(dSNorm, contactFilter, hits, minDist, true);
        bool didHit = false;
        for(int i = 0; i < numHits; ++i)
        {
            if(hits[i].distance < minDist)
            {
                int layer = 1 << hits[i].collider.gameObject.layer;
                if((layer & oneWayPlatform.value) == 0)
                {
                    didHit = true;
                    hit = hits[i];
                    minDist = hits[i].distance;
                }
                else if(hits[i].normal.y > 0f)
                {
                    if(!dropThroughOneWay && hits[i].distance > 0f && hits[i].collider != droppingThrough)
                    {
                        didHit = true;
                        hit = hits[i];
                        minDist = hits[i].distance;
                    }
                    else if(hits[i].collider == droppingThrough && hits[i].distance <= 0f)
                    {
                        droppingThrough = null;
                    }
                    else if(dropThroughOneWay && hits[i].distance > 0f)
                    {
                        droppingThrough = hits[i].collider;
                    }
                }
            }
        }

        if(droppingThrough != null) dropThroughOneWay = false;

        if(minDist <= 0f)
        {
            Vector2 center = box.offset + new Vector2(transform.position.x, transform.position.y);
            Vector2 hitPoint = hit.point - center;
            Vector2 skinPoint;
            float yX = box.size.y/(2f*Mathf.Abs(dSNorm.y)) * dSNorm.x;
            float xX = box.size.x/2f;
            if(Mathf.Abs(yX) < xX)
            {
                skinPoint = new Vector2(yX, box.size.y/2f * Mathf.Sign(dSNorm.y));
            }
            else
            {
                skinPoint = new Vector2(xX*Mathf.Sign(dSNorm.x), box.size.x/(2f*Mathf.Abs(dSNorm.x)) * dSNorm.y);
            }
            minDist -= Vector2.Dot(hitPoint - skinPoint, dSNorm);
        }

        minDist -= skinWidth;
        Vector2 moved = new Vector2(dSNorm.x, dSNorm.y)*minDist;
        transform.position += new Vector3(moved.x, moved.y);

        return hit;
	}
	
	
	public RaycastHit2D moveVelocity(ref Vector2 velocity, float dT)
	{
		Vector2 dS = velocity * dT;
		RaycastHit2D hit = movePosition(dS);
        
        if(hit.collider != null)
        {
            float vDotN = Vector2.Dot(velocity, hit.normal);
            if(vDotN < 0f)
            {
                velocity -= vDotN*hit.normal;
            }
        }

        return hit;
		
	}
	
}
