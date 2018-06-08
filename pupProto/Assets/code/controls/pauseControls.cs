//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class pauseControls : MonoBehaviour {

//	public Canvas canvas;

//	void Awake() {
//		Global.pauseControls = this;
//		canvas.enabled = false;
//		this.enabled = false;
//	}


//	void Update () {
//		if(Input.GetButtonDown(gameControls.buttons.pause)) {
//			Global.gameControls.enabled = true;
//			canvas.enabled = false;
//			this.enabled = false;
//		}
//	}

//	void OnEnable() {
//		Time.timeScale = 0f;
//		canvas.enabled = true;
//	}
//}
