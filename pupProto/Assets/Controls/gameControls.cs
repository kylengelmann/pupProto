using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameControls : MonoBehaviour {

	public playerController2D player;
	public float cameraFollowDistance = 1f/6f;

	void Awake() {
		Global.gameControls = this;
	}
	
	// Update is called once per frame
	void Update () {
    float dy = Camera.main.orthographicSize*cameraFollowDistance;
		float dx = Camera.main.aspect*Camera.main.orthographicSize*cameraFollowDistance;
		Vector3 camPos = Camera.main.transform.position;
		camPos.x = Mathf.Max(player.transform.position.x - dx, Mathf.Min(camPos.x, player.transform.position.x + dx));
		camPos.y = Mathf.Max(player.transform.position.y - dy, Mathf.Min(camPos.y, player.transform.position.y + dy));
		Camera.main.transform.position = camPos;
    if(Controls.Pause.down) {
			Global.pauseControls.enabled = true;
			this.enabled = false;
		}
		if(Controls.Jump.down) {
			player.Jump();
		}
		if (Controls.backDash.down) {
			player.backDash();
		}
		if(Controls.Kick.down) {
			player.Kick();
		}
		if(Controls.Dash.down) {
			player.Dash();
		}
	}

	void OnEnable() {
		Time.timeScale = 1f;
	}
		
	void FixedUpdate() {
		
		player.setMoveAxis(Input.GetAxisRaw("Horizontal"), Controls.Jump.pressed);
	}
		
}
