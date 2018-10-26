using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterPhysics : MonoBehaviour {
    public float skinWidth = 0.02f;
    public float minMove = 0.01f;
    public float minGroundY = .6f;
    public float maxOneWayPenetration = .05f;
    public ContactFilter2D contactFilter;
    public LayerMask oneWayPlatform;
    [HideInInspector] public bool dropThroughOneWay = false;
    [HideInInspector] public BoxCollider2D box;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public RaycastHit2D groundHit;
    RaycastHit2D[] hits;



    Character character;

    Collider2D droppingThrough = null;

    Vector2 lastHitNorm = Vector2.zero;
    bool didHit = false;

    void Awake()
    {
        box = gameObject.GetComponent<BoxCollider2D>();
        character = gameObject.GetComponentInHierarchy<Character>();
        hits = new RaycastHit2D[10];
    }

    public RaycastHit2D checkFloor(float checkDist, bool snapToGround = false)
    {
        isGrounded = false;
        Vector3 position = transform.position;
        RaycastHit2D hit = doMove(Vector2.down * checkDist);
        if (hit.distance <= 0f) {
            hit = new RaycastHit2D();
        }
        if (hit.collider == null) {
            if (hit.normal.y > minGroundY)
            {
                isGrounded = true;
                groundHit = hit;
            }
            transform.position = position;
        }
        else if(!snapToGround)
        {
            transform.position = position;
        }
        return hit;
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
                    bool isHit = hits[i].distance > Mathf.Epsilon;
                    if (!isHit)
                    {
                        Vector2 pen = calcPenetration(hits[i]);
                        isHit = pen.y <= maxOneWayPenetration;
                    }
                    if (!dropThroughOneWay && isHit && hits[i].collider != droppingThrough)
                    {
                            didHit = true;
                            hit = hits[i];
                            minDist = hits[i].distance;
                    }
                    else if (hits[i].collider == droppingThrough && !isHit)
                    {
                        droppingThrough = null;
                    }
                    else if (dropThroughOneWay && isHit)
                    {
                        droppingThrough = hits[i].collider;
                    }
                }
            }
        }

        if (droppingThrough != null) dropThroughOneWay = false;
        if (didHit)
        {
            dropThroughOneWay = false;
        }

        Vector2 moved = Vector2.zero;
        if (minDist <= 0f)
        {
            //Vector2 center = box.offset + new Vector2(transform.position.x, transform.position.y);
            //Vector2 hitPoint = hit.point - center;
            //Vector2 skinPoint;
            //float xHit = Mathf.Abs((-Mathf.Sign(hit.normal.x) * (box.size.x / 2f) - hitPoint.x) / (hit.normal.x + Mathf.Epsilon));
            //float yHit = Mathf.Abs((-Mathf.Sign(hit.normal.y) * (box.size.y / 2f) - hitPoint.y) / (hit.normal.y + Mathf.Epsilon));
            //skinPoint = hit.normal * (2f * Mathf.Min(xHit, yHit) + skinWidth);

            moved = calcPenetration(hit);
        }
        else if (didHit)
        {
            moved = new Vector2(dSNorm.x, dSNorm.y) * minDist + hit.normal * skinWidth;
            //moved = new Vector2(dSNorm.x, dSNorm.y) * (minDist - skinWidth);
        }
        else
        {
            moved = new Vector2(dSNorm.x, dSNorm.y) * distance;
        }
        transform.position += (Vector3) moved;
        return hit;
    }

    Vector2 calcPenetration(RaycastHit2D hit)
    {
        Vector2 center = box.offset + new Vector2(transform.position.x, transform.position.y);
        Vector2 hitPoint = hit.point - center;
        Vector2 skinPoint;
        float xHit = Mathf.Abs((-Mathf.Sign(hit.normal.x) * (box.size.x / 2f) - hitPoint.x) / (hit.normal.x + Mathf.Epsilon));
        float yHit = Mathf.Abs((-Mathf.Sign(hit.normal.y) * (box.size.y / 2f) - hitPoint.y) / (hit.normal.y + Mathf.Epsilon));
        skinPoint = hit.normal * (2f * Mathf.Min(xHit, yHit) + skinWidth);

        return skinPoint;
    }

    RaycastHit2D slideAlongSurface(Vector2 dS, float distTraveled, Vector2 surfaceNormal)
    {
        dS -= dS.normalized*distTraveled;
        dS -= Vector2.Dot(surfaceNormal, dS) * surfaceNormal;
        return doMove(dS);
    }
        
    public void movePosition(Vector2 dS)
    {
        isGrounded = false;
        RaycastHit2D hit = doMove(dS);
        if(hit.collider != null)
        {
            if (hit.normal.y > minGroundY)
            {
                isGrounded = true;
                groundHit = hit;
            }

            character.events.physics.onPhysicsHit.Invoke(hit);
            hit = slideAlongSurface(dS, hit.distance, hit.normal);

            if(hit.collider != null)
            {
                if (hit.normal.y > minGroundY)
                {
                    isGrounded = true;
                    groundHit = hit;
                }
                character.events.physics.onPhysicsHit.Invoke(hit);
            }
        }
    }


    public void moveVelocity(ref Vector2 velocity, float dT)
    {
        isGrounded = false;
        Vector2 dS = velocity * dT;
        RaycastHit2D hit = doMove(dS);
        if (hit.collider != null)
        {
            if (hit.normal.y > minGroundY)
            {
                isGrounded = true;
                groundHit = hit;
            }

            float vDotN = Vector2.Dot(velocity, hit.normal);
            if (vDotN < 0f)
            {
                velocity -= vDotN * hit.normal;
            }
            character.events.physics.onPhysicsHit.Invoke(hit);
            hit = slideAlongSurface(dS, hit.distance, hit.normal);

            if (hit.collider != null)
            {
                if (hit.normal.y > minGroundY)
                {
                    isGrounded = true;
                    groundHit = hit;
                }

                vDotN = Vector2.Dot(velocity, hit.normal);
                if (vDotN < 0f)
                {
                    velocity -= vDotN * hit.normal;
                }
                character.events.physics.onPhysicsHit.Invoke(hit);
            }
        }

    }
}

public class physicsEvents
{
    public safeAction<RaycastHit2D> onPhysicsHit = new safeAction<RaycastHit2D>();
}
