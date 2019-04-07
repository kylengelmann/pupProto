using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crouch : MonoBehaviour {
  
  [HideInInspector] public bool isActive = true;
  bool isCrouched;
  Character character;

  // Use this for initialization
  void Start () {
    character = gameObject.GetComponentInHierarchy<Character>();
    character.events.crouch.setCrouching += setCrouching;
    character.events.crouch.setActive += setActive;
	}
  
  void setActive(bool active)
  {
    isActive = active;
    if (!active) setCrouching(false);
  }
  
  void setCrouching(bool isDown)
  {
    //first is to see if you can
    isDown &= character.isGrounded;
    
    //then is to check if the state is changing
    if (isCrouched != isDown)
    {
      character.events.crouch.onCrouchChange.Invoke(isDown);
      isCrouched = isDown;
    }
  }

  // Update is called once per frame
  void Update () {
		
	}
}

public class crouchEvents
{
  public safeAction<bool> setCrouching = new safeAction<bool>();
  public safeAction<bool> onCrouchChange = new safeAction<bool>();
  public safeAction<bool> setActive = new safeAction<bool>();
}

