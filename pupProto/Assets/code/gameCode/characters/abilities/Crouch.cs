using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crouch : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public class crouchEvents
{
  public safeAction<bool> setCrouching = new safeAction<bool>();
  public safeAction<bool> onCrouchChange = new safeAction<bool>();
}
