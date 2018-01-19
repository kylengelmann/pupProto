using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Button {
	public KeyCode Key;
	public bool down {
		get{
			return Input.GetKeyDown(this.Key);
		}
	}
	public bool up {
		get {
			return Input.GetKeyUp(this.Key);
		}
	}
	public bool pressed {
		get {
			return Input.GetKey(this.Key);
		}
	}
	public bool held {
		get {
			if(heldTime < holdLength) {
				return false;
			}
			return true;
		}
	}
	public static float holdLength = 1f;
	public float heldTime = 0f;
	public float heldPercent {
		get {
			return Mathf.Min(heldTime/holdLength, 1f);
		}
	}
	public Button() {
		
	}
	public Button(KeyCode key) {
		Key = key;
	}
	public Button(string key) {
		Key = (KeyCode)System.Enum.Parse(typeof(KeyCode), key, true);
	}
}

public static class Controls {

	public const int numButts = 5;
	
	public static Button[] butts = new Button[numButts];


	public const int pauseIndex = 0;
	public static Button Pause {
		get{
			return butts[pauseIndex];
		}
		set {
			butts[pauseIndex] = value;
		}
	}

	public const int jumpIndex = 1;
	public static Button Jump {
		get{
			return butts[jumpIndex];
		}
		set {
			butts[jumpIndex] = value;
		}
	}

	public const int kickIndex = 2;
	public static Button Kick {
		get {
			return butts[kickIndex];
		}
		set {
			butts[kickIndex] = value;
		}
	}

	public const int DashIndex = 3;
	public static Button Dash {
		get {
			return butts[DashIndex];
		}
		set {
			butts[DashIndex] = value;
		}
	}

	public const int backDashIndex = 4;
	public static Button backDash {
		get {
			return butts[backDashIndex];
		}
		set {
			butts[backDashIndex] = value;
		}
	}




	public static void LateUpdate() {
		for(int i = 0; i < butts.Length; i++){
			if(butts[i].pressed) {
				butts[i].heldTime += Time.deltaTime;
			}
			else if(butts[i].up) {
				butts[i].heldTime = 0f;
			}
		}
	}
}
