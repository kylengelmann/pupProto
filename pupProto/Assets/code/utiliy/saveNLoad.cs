//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;

////[System.Serializable]
////public class userControls {
////	public KeyCode[] keys = new KeyCode[Controls.numButts];
////}

//[System.Serializable]
//public class userPrefs {
//	public float aSpeed;
//	public moveSettings m;
//	public float camDist;
//}

//public static class saveNLoad {
//	//public static string controlsFile = "Assets/Resources/collabIgnore/userControls.json";
//	public static string prefsFile = "Assets/Resources/collabIgnore/userPrefs.json";
//	public static void save(object obj, string path) {
//		StreamWriter sw = new StreamWriter(path, false);
//		sw.WriteLine(JsonUtility.ToJson(obj));
//		sw.Close();
//	}

//	public static bool load<T>(string path, out T data) {
//		if(!File.Exists(path)) {
//			data = default(T);
//			return false;
//		}
//		StreamReader sr = new StreamReader(path);
//		data = JsonUtility.FromJson<T>(sr.ReadLine());
//		return true;
//	}

//	//public static void saveControls() {
//	//	userControls uc = new userControls();
//	//	for(int i = 0; i < Controls.numButts; i++) {
//	//		uc.keys[i] = Controls.butts[i].Key;
//	//	}
//	//	save(uc, controlsFile);
//	//}

//	//public static bool loadControls() {
//	//	userControls uc = new userControls();
//	//	if(!load<userControls>(controlsFile, out uc)) {
//	//		return false;
//	//	}

//	//	for(int i = 0; i < Controls.numButts; i++) {
//	//		Controls.butts[i] = new Button(uc.keys[i]);
//	//	}

//	//	return true;
//	//}

//	public static void savePrefs() {
//		userPrefs up = new userPrefs();
//		up.aSpeed = Global.player.anim.GetFloat("animSpeed");
//		up.m = Global.player.movement;
//		save(up, prefsFile);
//	}

//	public static bool loadPrefs() {
//		userPrefs up = new userPrefs();
//		if(!load<userPrefs>(prefsFile, out up)) {
//			return false;
//		}


//		Global.player.anim.SetFloat("animSpeed", up.aSpeed);

//		Global.player.movement = up.m;

//		//Global.gameControls.cameraFollowDistance = up.camDist;
//		return true;
//	}


//}
