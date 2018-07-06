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

    Vector2 lastHitNorm = Vector2.zero;

    void Awake()
	{
		box = gameObject.GetComponent<BoxCollider2D>();
		hits = new RaycastHit2D[10];
	}

    public RaycastHit2D doMove(Vector2 dS)
    {
        float distance = dS.magnitude;
        Vector2 dSNorm = dS.normalized;
        RaycastHit2D hit = new RaycastHit2D();
        if (distance < minMove) return hit;
        float minDist = distance + skinWidth;
        int numHits = box.Cast(dSNorm, contactFilter, hits, minDist, true);
        bool didHit = false;
        for (int i = 0; i < numHits; ++i)
        {
            if (hits[i].distance < minDist)
            {
                int layer = 1 << hits[i].collider.gameObject.layer;
                if ((layer & oneWayPlatform.value) == 0)
                {
                    didHit = true;
                    hit = hits[i];
                    minDist = hits[i].distance;
                }
                else if (hits[i].normal.y > 0f)
                {
                    if (!dropThroughOneWay && hits[i].distance > 0f && hits[i].collider != droppingThrough)
                    {
                        didHit = true;
                        hit = hits[i];
                        minDist = hits[i].distance;
                    }
                    else if (hits[i].collider == droppingThrough && hits[i].distance <= 0f)
                    {
                        droppingThrough = null;
                    }
                    else if (dropThroughOneWay && hits[i].distance > 0f)
                    {
                        droppingThrough = hits[i].collider;
                    }
                }
            }
        }

        if (droppingThrough != null) dropThroughOneWay = false;
        
        Vector2 moved = Vector2.zero;
        if (minDist <= 0f)
        {
            Vector2 center = box.offset + new Vector2(transform.position.x, transform.position.y);
            Vector2 hitPoint = hit.point - center;
            Vector2 skinPoint;
            float xHit = Mathf.Abs((-Mathf.Sign(hit.normal.x)*(box.size.x/2f) - hitPoint.x)/(hit.normal.x + Mathf.Epsilon));
            float yHit = Mathf.Abs((-Mathf.Sign(hit.normal.y) * (box.size.y / 2f) - hitPoint.y) / (hit.normal.y + Mathf.Epsilon));
            skinPoint = hit.normal * (2f*Mathf.Min(xHit, yHit) + skinWidth);
            
            moved = skinPoint;
        }
        else
        {
            minDist -= skinWidth;
            moved = new Vector2(dSNorm.x, dSNorm.y) * minDist;
        }
        transform.position += new Vector3(moved.x, moved.y);
        return hit;
    }
	

	public controllerHits movePosition(Vector2 dS)
	{
        Vector2 perp = Vector2.Dot(dS, lastHitNorm)*lastHitNorm;
        Vector2 par = dS - perp;
        controllerHits result = new controllerHits {
		    hit1 = doMove(par),
            hit2 = doMove(perp)
        };
        lastHitNorm = result.hit2.collider != null ? result.hit2.normal : (result.hit1.collider != null ? result.hit1.normal : Vector2.zero);

        return result;
	}
	
	
	public controllerHits moveVelocity(ref Vector2 velocity, float dT)
	{
		Vector2 dS = velocity * dT;
		controllerHits result = movePosition(dS);
        
        if(result.hit1.collider != null)
        {
            float vDotN = Vector2.Dot(velocity, result.hit1.normal);
            if(vDotN < 0f)
            {
                velocity -= vDotN*result.hit1.normal;
            }
        }
        if (result.hit2.collider != null)
        {
            float vDotN = Vector2.Dot(velocity, result.hit2.normal);
            if (vDotN < 0f)
            {
                velocity -= vDotN * result.hit2.normal;
            }
        }

        return result;
		
	}
	
}


public struct controllerHits
{
    public RaycastHit2D hit1;
    public RaycastHit2D hit2;
}
