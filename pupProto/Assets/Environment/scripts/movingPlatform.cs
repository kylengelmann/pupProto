using UnityEngine;

[RequireComponent(typeof(velocityGiver))]
public class movingPlatform : MonoBehaviour
{
	public Transform startPoint;
	public Transform endPoint;
	public float speed = 1f;
	public float waitTime = 1f;
	[HideInInspector] public float progress = 0f;
	[HideInInspector] public bool reverse = true;
    Vector2 __vel = Vector2.zero;
    public float moveHeight = .2f;
    velocityGiver velGiver;


	public Vector2 velocity
	{
		get { return __vel; }
	}

	void Start()
	{
        velGiver = GetComponent<velocityGiver>();
	}


	float timeWaited = 0f;
	void FixedUpdate () {

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


        velGiver.velocity = __vel;
	}
}
