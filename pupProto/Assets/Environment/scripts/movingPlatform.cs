using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class movingPlatform : MonoBehaviour
{
	public Transform startPoint;
	public Transform endPoint;
	public float speed = 1f;
	public float waitTime = 1f;
	[HideInInspector] public float progress = 0f;
	[HideInInspector] public bool reverse = true;
    Vector2 __vel = Vector2.zero;
    BoxCollider2D box;
    Collider2D[] colliders;
    public float moveHeight = .2f;


	public Vector2 velocity
	{
		get { return __vel; }
	}

	void Start()
	{
        box = gameObject.GetComponent<BoxCollider2D>();
        colliders = new Collider2D[8];
	}


	float timeWaited = 0f;
	void FixedUpdate () {
		
		Vector2 center = box.offset;
		center.y += (box.size.y * transform.lossyScale.y + moveHeight)/2f + transform.position.y;
		center.x += transform.position.x;

		if (timeWaited < waitTime)
		{
			timeWaited += Time.fixedDeltaTime;
			reverse = !reverse;
			__vel = Vector2.zero;
		}
		else
		{
			Vector2 range = (endPoint.position - startPoint.position);
			float distance = range.magnitude;
			if (reverse)
			{
				__vel = range.normalized * (-speed);
				progress = Mathf.Max(progress - speed / distance * Time.fixedDeltaTime, 0f);
			}
			else
			{
				__vel = range.normalized * speed;
				progress = Mathf.Min(progress + speed / distance * Time.fixedDeltaTime, 1f);
			}
			if (progress <= 0f || progress >= 1f) timeWaited = 0f;
			transform.position = Vector3.Lerp(startPoint.position, endPoint.position, progress);
		}
		

		int numOn = Physics2D.OverlapBoxNonAlloc(center, box.size, 0f, colliders);
		for(int i = 0; i < numOn; ++i) {
			velocityReciever velRec;
			if(velRec = colliders[i].gameObject.GetComponent<velocityReciever>()) {
				velRec.velocity = __vel;
			}
		}
	}
}
