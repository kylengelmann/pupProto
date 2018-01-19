using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resolutionFix : MonoBehaviour {

	// Use this for initialization
	private Camera cam;

	public float orthoSize = 5;
	public int ppu = 8;
	
	void Start () {
		cam = Camera.main;

	}
	
	// Update is called once per frame
	void Update()
	{
		float x = Mathf.Round(Screen.height / (2f * orthoSize * ppu));
		cam.orthographicSize = (Screen.height - 2f*orthoSize*x*ppu)/(x*ppu)/2f + orthoSize;
//		cam.aspect = 1;
		Debug.Log(new Vector2(x, cam.orthographicSize));
	}
}
