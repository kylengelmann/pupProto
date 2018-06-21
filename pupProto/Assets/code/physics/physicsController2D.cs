using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class physicsController2D : MonoBehaviour
{
	public float minSinSlope = 0.766f;
	public float skinWidth = 0.02f;
	public float minMove = 0.01f;
    public float hitTolerance = 0.1f;
	public float maxStairHeight = 0.1f;
	public ContactFilter2D contactFilter;
	public LayerMask oneWayPlatform;
	[HideInInspector] public bool dropThroughOneWay = false;
	[HideInInspector] public bool grounded = false;
	[HideInInspector] public BoxCollider2D box;
	[HideInInspector] public RaycastHit2D[] hits;
	[HideInInspector] public Vector2 hitNormal;

	public struct controllerHit
	{
		public RaycastHit2D x;
		public RaycastHit2D y;
	}

	public controllerHit hit;
	
	void Awake()
	{
		box = gameObject.GetComponent<BoxCollider2D>();
		hits = new RaycastHit2D[10];
	}
	

	public void movePosition(Vector2 dS)
	{
		float distance = dS.magnitude;
		hit.x = new RaycastHit2D();
		hit.y = new RaycastHit2D();
		if (distance >= minMove)
		{
			float yDist = Mathf.Abs(dS.y) + skinWidth;
			float xDist = Mathf.Abs(dS.x) + skinWidth;
			hitNormal = Vector2.zero;
			float signX = Mathf.Sign(dS.x);
			int numHits;
			if (dS.y > 0f)
			{
				numHits = box.Cast(Vector2.up, contactFilter, hits, yDist);
				for (int i = 0; i < numHits; i++)
				{
					if (hits[i].distance < yDist)
					{
						int layer = hits[i].transform.gameObject.layer;
						layer = (1 << layer);
						if (layer != oneWayPlatform.value)
						{
							yDist = hits[i].distance;
							hitNormal = hits[i].normal;	
							hit.y = hits[i];
						}
					}
				}

				
				if (numHits > 0 && hit.y.distance == 0f)
				{
					float dist = hit.y.point.y - (transform.position.y + box.offset.y + .5f * box.size.y);
					if (Mathf.Abs(dist) <= skinWidth) yDist -= dist;
					else
					{
						dist = (transform.position.y + box.offset.y - .5f * box.size.y) - hit.y.point.y;
						if (Mathf.Abs(dist) <= skinWidth) yDist += dist + 2 * skinWidth;
					}
				}

				yDist -= skinWidth;

				transform.position = transform.position + yDist*Vector3.up;

				numHits = box.Cast(Vector2.right * signX, contactFilter, hits, xDist);
				for (int i = 0; i < numHits; i++)
				{
					if (hits[i].distance < xDist)
					{
						int layer = hits[i].transform.gameObject.layer;
						layer = (1 << layer);
						if (layer != oneWayPlatform.value)
						{
							xDist = hits[i].distance;
							hitNormal = hits[i].normal;
							hit.x = hits[i];
						}
					}
				}
				
				if (numHits > 0 && hit.x.distance == 0f)
				{
					float dist = hit.x.point.x - (transform.position.x + box.offset.x + signX*.5f * box.size.x);
					dist *= signX;
					if(Mathf.Abs(dist) <= skinWidth) xDist -= dist;
					else
					{
						dist = hit.x.point.x - (transform.position.x + box.offset.x - signX*.5f * box.size.x);
						dist *= -signX;
						if (Mathf.Abs(dist) <= skinWidth) xDist += dist + 2*skinWidth;
					}
				}

				xDist -= skinWidth;

				transform.position = transform.position + xDist * signX * Vector3.right;
			}
			else
			{
				numHits = box.Cast(Vector2.right * signX, contactFilter, hits, xDist);
				for (int i = 0; i < numHits; i++)
				{
					if (hits[i].distance < xDist)
					{
						int layer = hits[i].transform.gameObject.layer;
						layer = (1 << layer);
						if (layer != oneWayPlatform.value)
						{
							xDist = hits[i].distance;
							hitNormal = hits[i].normal;
							hit.x = hits[i];
						}
					}
				}
				
				if (numHits > 0 && hit.x.distance == 0f)
				{
					float dist = hit.x.point.x - (transform.position.x + box.offset.x + signX*.5f * box.size.x);
					dist *= signX;
					if(Mathf.Abs(dist) <= skinWidth) xDist -= dist;
					else
					{
						dist = hit.x.point.x - (transform.position.x + box.offset.x - signX*.5f * box.size.x);
						dist *= -signX;
						if (Mathf.Abs(dist) <= skinWidth) xDist += dist + 2 * skinWidth;
					}
				}

				xDist -= skinWidth;
				
				transform.position = transform.position + xDist * signX*Vector3.right;
				
				numHits = box.Cast(Vector2.down, contactFilter, hits, yDist);
				for (int i = 0; i < numHits; i++)
				{
					if (hits[i].distance < yDist)
					{	
						int layer = hits[i].transform.gameObject.layer;
						layer = (1 << layer);
						if (layer != oneWayPlatform.value || (!dropThroughOneWay && hits[i].distance > Mathf.Epsilon * 1000f))
						{
							yDist = hits[i].distance;
							hitNormal = hits[i].normal;	
							hit.y = hits[i];	
						}
						else
						{
							if (hits[i].distance < Mathf.Epsilon * 1000f)
							{
								dropThroughOneWay = false;	
							}
						}
					}
				}
				if (numHits > 0 && dropThroughOneWay)
				{
					if (hit.y.transform != null)
					{
						int layer = hit.y.transform.gameObject.layer;
						layer = (1 << layer);
						if(layer != oneWayPlatform) dropThroughOneWay = false;
					}
				}
				
				
				if (numHits > 0 && hit.y.distance == 0f)
				{
					float dist = (transform.position.y + box.offset.y -.5f * box.size.y) - hit.y.point.y;
					if(Mathf.Abs(dist) <= skinWidth) xDist -= dist;
					else
					{
						dist = hit.y.point.y - (transform.position.y + box.offset.y + .5f * box.size.y);
						if (Mathf.Abs(dist) <= skinWidth) yDist += dist + 2 * skinWidth;
					}
				}

				yDist -= skinWidth;
				
				
				
				transform.position = transform.position + yDist*Vector3.down;
			}

			grounded = (hitNormal.y >= minSinSlope);

		}
	}
	
	
	
	
	
	
	
	
	
	
	public void moveVelocity(ref Vector2 velocity, float dT)
	{
		Vector2 dS = velocity * dT;
		float distance = dS.magnitude;
		hit.x = new RaycastHit2D();
		hit.y = new RaycastHit2D();
		if (distance >= minMove)
		{
            float yDist = Mathf.Abs(dS.y) + skinWidth + hitTolerance;
            float xDist = Mathf.Abs(dS.x) + skinWidth + hitTolerance;
			hitNormal = Vector2.zero;
			float signX = Mathf.Sign(velocity.x);
			int numHits;

            // If moving up, move up before moving to side
			if (dS.y > 0f)
			{   
                // Check all collisions y direction
				numHits = box.Cast(Vector2.up, contactFilter, hits, yDist);
				for (int i = 0; i < numHits; i++)
				{
                    // If this collision happens soon in the path than the current
                    // soonest path, update the info on the object hit
					if (hits[i].distance < yDist)
					{
                        // Get the hit object's layer
						int layer = hits[i].transform.gameObject.layer;
						layer = (1 << layer);

                        // If the hit object is a one way platform, ignore it becuase we're moving up
						if (layer != oneWayPlatform.value)
						{   
                            // Updates the current info on the object hit
							yDist = hits[i].distance;
							hitNormal = hits[i].normal;	
							hit.y = hits[i];
						}
					}
				}

                if(hit.y.transform != null) {
                    // If the ceiling was hit by the skin, set the y velocity to 0
                    if (hitNormal.y < -0.01f && yDist < Mathf.Abs(dS.y) + skinWidth)
                    {
                        velocity.y = 0f;
                    }

                    // Push the object this component is attached to out of other objects
                    if (hit.y.distance <= 0f)
                    {
                        float dist = hit.y.point.y - (transform.position.y + box.offset.y + .5f * box.size.y);
                        if (Mathf.Abs(dist) <= skinWidth) yDist -= dist;
                        else
                        {
                            dist = (transform.position.y + box.offset.y - .5f * box.size.y) - hit.y.point.y;
                            if (Mathf.Abs(dist) <= skinWidth) yDist += dist + 2 * skinWidth;
                        }
                    }
                }
               
                // keep this a skinWidth away from collision
				yDist -= skinWidth;

                // Ensure the hitTolerance does not affect the move distance
                yDist = Mathf.Min(yDist, Mathf.Abs(dS.y));

                //Move this yDist up
				transform.position = transform.position + yDist*Vector3.up;


                // Now handle check collisions in x direction
				numHits = box.Cast(Vector2.right * signX, contactFilter, hits, xDist);
				for (int i = 0; i < numHits; i++)
				{
					if (hits[i].distance < xDist)
					{
						int layer = hits[i].transform.gameObject.layer;
						layer = (1 << layer);
						if (layer != oneWayPlatform.value)
						{
							xDist = hits[i].distance;
							hitNormal = hits[i].normal;
							hit.x = hits[i];
						}
					}
				}


                if(hit.x.transform != null) {

                    // update velocity if a wall is hit
                    if (hitNormal.y < minSinSlope && hitNormal.x * signX < -0.01f && 
                        xDist < Mathf.Abs(dS.x) + skinWidth)
                    {
                        velocity.x = 0f;
                    }

                    // Push this out of other objects if it's inside one
                    if (hit.x.distance <= 0f)
                    {
                        float dist = hit.x.point.x - (transform.position.x + box.offset.x + signX*.5f * box.size.x);
                        dist *= signX;
                        if(Mathf.Abs(dist) <= skinWidth) xDist -= dist;
                        else
                        {
                            dist = hit.x.point.x - (transform.position.x + box.offset.x - signX*.5f * box.size.x);
                            dist *= -signX;
                            if (Mathf.Abs(dist) <= skinWidth) xDist += dist + 2*skinWidth;
                        }
                    }
                }

				xDist -= skinWidth;
                xDist = Mathf.Min(xDist, Mathf.Abs(dS.x));

				transform.position = transform.position + xDist * signX * Vector3.right;
			}

            // If not moving up, handle x movement first
			else
			{
                // Check collisions in x direction
				numHits = box.Cast(Vector2.right * signX, contactFilter, hits, xDist);
				for (int i = 0; i < numHits; i++)
				{
					if (hits[i].distance < xDist)
					{
						int layer = hits[i].transform.gameObject.layer;
						layer = (1 << layer);
						if (layer != oneWayPlatform.value)
						{
							xDist = hits[i].distance;
							hitNormal = hits[i].normal;
							hit.x = hits[i];
						}
					}
				}

                if(hit.x.transform != null) {

                    // Update velocity if hitting a wall
                    if (hitNormal.y < minSinSlope && hitNormal.x * signX < -Mathf.Epsilon*0.01f && 
                        xDist < Mathf.Abs(dS.x) + skinWidth) 
                    {
                        velocity.x = 0f;

                    }

                    // push this out of other objects
                    if (hit.x.distance <= 0f)
                    {
                        float dist = hit.x.point.x - (transform.position.x + box.offset.x + signX*.5f * box.size.x);
                        dist *= signX;
                        if(Mathf.Abs(dist) <= skinWidth) xDist -= dist;
                        else
                        {
                            dist = hit.x.point.x - (transform.position.x + box.offset.x - signX*.5f * box.size.x);
                            dist *= -signX;
                            if (Mathf.Abs(dist) <= skinWidth) xDist += dist + 2 * skinWidth;
                        }
                    }   
                }

				xDist -= skinWidth;
                xDist = Mathf.Min(xDist, Mathf.Abs(dS.x));
				
				transform.position = transform.position + xDist * signX*Vector3.right;
				
                //Check Y direction collisions
				numHits = box.Cast(Vector2.down, contactFilter, hits, yDist);
				for (int i = 0; i < numHits; i++)
				{
					if (hits[i].distance < yDist)
					{	
						int layer = hits[i].transform.gameObject.layer;
						layer = (1 << layer);
                        // If on a one way platform, check to see if we are dropping through it or not
                        if (layer != oneWayPlatform.value || (!dropThroughOneWay && hits[i].distance > Mathf.Epsilon * 1000f))
						{
							yDist = hits[i].distance;
							hitNormal = hits[i].normal;	
							hit.y = hits[i];	
						}
						else
						{
                            // Reset dropThroughOneWay if we fell through a platform
                            if (hits[i].distance < Mathf.Epsilon * 1000f)
							{
                                dropThroughOneWay = false;
							}
						}
					}
				}


                if(hit.y.transform != null) {
                    // Reset dropThroughOneWay if we land on the ground
                    if (dropThroughOneWay)
                    {
                        int layer = hit.y.transform.gameObject.layer;
                        layer = (1 << layer);
                        if(layer != oneWayPlatform) dropThroughOneWay = false;
                    }

                    if (hitNormal.y > 0.01f && yDist < Mathf.Abs(dS.y) + skinWidth)
                    {
                        velocity.y = 0f;
                    }

                    if (hit.y.distance <= 0f)
                    {
                        float dist = (transform.position.y + box.offset.y -.5f * box.size.y) - hit.y.point.y;
                        if(Mathf.Abs(dist) <= .5f * box.size.y) yDist -= dist;
                        else
                        {
                            dist = hit.y.point.y - (transform.position.y + box.offset.y + .5f * box.size.y);
                            if (Mathf.Abs(dist) <= skinWidth) yDist += dist + 2 * skinWidth;
                        }
                    }
                        
                }
				
				yDist -= skinWidth;
                yDist = Mathf.Min(yDist, Mathf.Abs(dS.y));
				
				
				
				transform.position = transform.position + yDist*Vector3.down;
			}

			grounded = (hitNormal.y >= minSinSlope);

		}
	}
	
}
