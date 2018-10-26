using UnityEngine;

public class penChar : Character {

    //public float Zcorrection = 100f;
    penPhys controller;

	protected override void Start () {
		base.Start();
        controller = GetComponent<penPhys>();
	}
	
	void FixedUpdate () {
        float dt = Time.deltaTime;
        //Vector2 grav = groundNormal * gravity * dt / Vector2.Dot(groundNormal, Vector2.down);
        Vector2 grav = gravity * dt * Vector2.down;
        velocity += grav;

        controller.moveVelocity(ref velocity, dt);
    }
}
