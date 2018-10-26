using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anotherPhys : MonoBehaviour {
    
    public float skinWidth = .02f;
    BoxCollider box;
    BoxCollider skin;

	void Start () {
		box = GetComponent<BoxCollider>();
        skin = gameObject.AddComponent<BoxCollider>();
        skin.center = box.center;
        skin.size = box.size + new Vector3(skinWidth*2f, skinWidth*2f, 0f);
        skin.hideFlags = HideFlags.HideInInspector;
	}

    private void OnDestroy()
    {
        Destroy(skin);
    }

    public void moveVelocity(ref Vector2 velocity, float dt)
    {
        Vector2 dS = velocity*dt;
    }
}
