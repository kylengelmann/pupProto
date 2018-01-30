using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlManager : MonoBehaviour {

	//public bool Debugging = true;

	bool movementSettings = false;
	string pSpeed;
	string aSpeed;
	string gAcc;
	string stpAcc;
	string dswAcc;

	
	bool jumpSettings = false;
	Vector2 jScrPos = Vector2.zero;
	string jVel;
	string djVel;
	string jAcc;
	string ejAcc;
	string fAcc;
	string minJump;
	string maxJump;
	string tVel;
	string ac;
	string coyote;

	
	bool dashSettings = false;
	string dVel;
	string dCool;
	string dExp;
	string dADF;
	
	string bdVel;
	string bdExp;
	string bdCool;

	bool combatSettings = false;
	string aky;
	string akx;
	string akg;


	void Awake() {
		if(!saveNLoad.loadControls()) {
			Debug.Log("Unable to find controls file, using defaults and writing file");
			Controls.Pause = new Button(KeyCode.P);
			Controls.Jump = new Button(KeyCode.Space);
			Controls.Kick = new Button(KeyCode.K);
			Controls.Dash = new Button(KeyCode.J);
			Controls.backDash = new Button(KeyCode.L);
			saveNLoad.saveControls();
		}
	}

	void Start() {
		pSpeed = Global.player.movement.speed.ToString();
		aSpeed = Global.player.anim.GetFloat("animSpeed").ToString();
		gAcc = Global.player.movement.groundAcceleration.ToString();
		stpAcc = Global.player.movement.groundFriction.ToString();
		dswAcc = Global.player.movement.directionSwitchAcceleration.ToString();
		
		jVel = Global.player.movement.jumpVelocity.ToString();
		djVel = Global.player.movement.doubleJumpVelocity.ToString();
		jAcc = Global.player.movement.jumpAcc.ToString();
		ejAcc = Global.player.movement.endJumpAcc.ToString();
		fAcc = Global.player.movement.fallAcc.ToString();
		minJump = Global.player.movement.minJumpTime.ToString();
		maxJump = Global.player.movement.maxJumpTime.ToString();
		tVel = Global.player.movement.terminalVel.ToString();
		ac = Global.player.movement.airControl.ToString();
		coyote = Global.player.movement.coyoteTime.ToString();
		
		
		dVel = Global.player.movement.dashVelocity.ToString();
		dCool = Global.player.movement.dashCooldown.ToString();
		dExp = Global.player.movement.dashExp.ToString();
		dADF = Global.player.movement.dashFreeze.ToString();
		
		bdVel = Global.player.movement.backDashVelocity.ToString();
		bdExp = Global.player.movement.backDashExp.ToString();
		bdCool = Global.player.movement.backDashCooldown.ToString();

		aky = Global.player.movement.airKickYVel.ToString();
		akx = Global.player.movement.airKickXVel.ToString();
		akg = Global.player.movement.airKickGrav.ToString();
		
	}

	void LateUpdate() {
		Controls.LateUpdate();
	}

	void makeBox(string _name, ref string dataStr, ref float data, float max = 10f) {
		GUILayout.BeginVertical("box", GUILayout.Width(200));
		GUILayout.Box(_name);
		GUILayout.BeginHorizontal("Label");
		float lastFloat = data;
		data = GUILayout.HorizontalSlider(data, 0f, max);
		if(lastFloat != data) {
			dataStr = data.ToString();
		}


		GUIStyle style = GUI.skin.box;
		style.alignment = TextAnchor.MiddleLeft;

		string lastString = dataStr;
		dataStr = GUILayout.TextField(dataStr, style, GUILayout.Width(50));
		if(lastString != dataStr) {
			try{
				data = float.Parse(dataStr);
			}
			catch {
				data = 0f;
			}
			if(data > max) {
				data = max;
				dataStr = max.ToString();
			}
		}

		style.alignment = TextAnchor.MiddleCenter;

		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
		
	void OnGUI() {
//		GUI.skin.box.contentOffset = Vector2.zero;
		GUILayout.BeginHorizontal();
		
		
		
		GUILayout.BeginVertical("box");
		
		GUILayout.BeginVertical("box", GUILayout.Width(200));
		movementSettings = GUILayout.Toggle(movementSettings, "Show Movement Settings");
		GUILayout.EndVertical();

		if (movementSettings)
		{
			makeBox("Max Player Speed", ref pSpeed, ref Global.player.movement.speed);
			makeBox("Player Start Move Acceleration", ref gAcc, ref Global.player.movement.groundAcceleration, 200f);
			makeBox("Player Stop Move Acceleration", ref stpAcc, ref Global.player.movement.groundFriction, 200f);
			makeBox("Player Switch Move Acceleration", ref dswAcc, ref Global.player.movement.directionSwitchAcceleration, 200f);
			
			float temp = Global.player.anim.GetFloat("animSpeed");
			makeBox("Animation Speed", ref aSpeed, ref temp);
			Global.player.anim.SetFloat("animSpeed", temp);
		}
		GUILayout.EndVertical();
		
		
		
		GUILayout.BeginVertical("box");
		
		GUILayout.BeginVertical("box", GUILayout.Width(216));
		jumpSettings = GUILayout.Toggle(jumpSettings, "Show Jump Settings");
		GUILayout.EndVertical();

		if (jumpSettings)
		{
			jScrPos = GUILayout.BeginScrollView(jScrPos, false, true);
			GUILayout.BeginVertical("box", GUILayout.Width(200));
			
			bool tempBool = (Global.player.movement.jumpMode == moveSettings.JumpMode.gravitational);
			tempBool = GUILayout.Toggle(tempBool, "Mario Jump");
			if (tempBool)
			{
				Global.player.movement.jumpMode = moveSettings.JumpMode.gravitational;
			}
			
			tempBool = (Global.player.movement.jumpMode == moveSettings.JumpMode.velocity1);
			tempBool = GUILayout.Toggle(tempBool, "Non Constant Velocity Jump");
			if (tempBool)
			{
				Global.player.movement.jumpMode = moveSettings.JumpMode.velocity1;
			}
			
			tempBool = (Global.player.movement.jumpMode == moveSettings.JumpMode.velocity2);
			tempBool = GUILayout.Toggle(tempBool, "Constant Velocity Jump");
			if (tempBool)
			{
				Global.player.movement.jumpMode = moveSettings.JumpMode.velocity2;
			}
			
			GUILayout.EndVertical();
			
			makeBox("Jump Velocity", ref jVel, ref Global.player.movement.jumpVelocity, 70f);
			makeBox("Double Jump Velocity", ref djVel, ref Global.player.movement.doubleJumpVelocity, 70f);
			makeBox("Jump Acceleration", ref jAcc, ref Global.player.movement.jumpAcc, 200f);
			makeBox("End Jump Acceleration", ref ejAcc, ref Global.player.movement.endJumpAcc, 200f);
			makeBox("Falling Acceleration", ref fAcc, ref Global.player.movement.fallAcc, 200f);
			makeBox("Min Jump Time", ref minJump, ref Global.player.movement.minJumpTime, 1f);
			makeBox("Max Jump Time", ref maxJump, ref Global.player.movement.maxJumpTime, 2f);
			makeBox("Terminal Velocity", ref tVel, ref Global.player.movement.terminalVel, 200f);
			makeBox("Air Control", ref ac, ref Global.player.movement.airControl, 1f);
			makeBox("CoyoteTime", ref coyote, ref Global.player.movement.coyoteTime, 2f);
			GUILayout.EndScrollView();
		}
		GUILayout.EndVertical();
		
			
		
		GUILayout.BeginVertical("box");
		
		GUILayout.BeginVertical("box", GUILayout.Width(200));
		dashSettings = GUILayout.Toggle(dashSettings, "Show Dash Settings");
		GUILayout.EndVertical();

		if (dashSettings)
		{
			makeBox("Dash Velocity", ref dVel, ref Global.player.movement.dashVelocity, 5000f);
			makeBox("Dash Cooldown", ref dCool, ref Global.player.movement.dashCooldown, 3f);
			makeBox("Dash Exponent", ref dExp, ref Global.player.movement.dashExp, 10f);
			makeBox("Dash Constant", ref dExp, ref Global.player.movement.dashConstant, 10f);
			makeBox("Dash Air Freeze", ref dADF, ref Global.player.movement.dashFreeze, 2f);
			
			makeBox("Back Dash Velocity", ref bdVel, ref Global.player.movement.backDashVelocity, 50f);
			makeBox("Back Dash Exponent", ref bdExp, ref Global.player.movement.backDashExp, 1f);
			makeBox("Back Dash Cooldown", ref bdCool, ref Global.player.movement.backDashCooldown, 3f);
		}
		GUILayout.EndVertical();



		GUILayout.BeginVertical("box");

		GUILayout.BeginVertical("box", GUILayout.Width(200));
		combatSettings = GUILayout.Toggle(combatSettings, "Show Combat Settings");
		GUILayout.EndVertical();

		if (combatSettings)
		{
			makeBox("Air Kick Y Vel (+)", ref aky, ref Global.player.movement.airKickYVel, 20f);
			makeBox("Air Kick X Vel (*)", ref akx, ref Global.player.movement.airKickXVel, 1f);
			makeBox("Air Kick Gravity", ref akg, ref Global.player.movement.airKickGrav, 100f);
		}
		GUILayout.EndVertical();


			
		GUILayout.BeginVertical("box", GUILayout.Width(100));
		if(GUILayout.Button("Save")) {
			saveNLoad.savePrefs();
		}
		GUILayout.EndVertical();

		GUILayout.EndHorizontal();
	}
}
