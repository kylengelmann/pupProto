using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class initControlSelect : MonoBehaviour {

	public GameObject prefab;

	void Awake(){
		uiControl.num = 0;
	}

	void Start () {
		RectTransform rt = gameObject.GetComponent<RectTransform>();
		rt.anchoredPosition = Vector2.zero;

		GameObject go = Instantiate<GameObject>(prefab);
		go.transform.SetParent(transform, false);
		uiControl uCon = go.GetComponent<uiControl>();
		uCon.name = "Pause";
		uCon.init(Controls.pauseIndex, "Pause");

		go = Instantiate<GameObject>(prefab);
		go.transform.SetParent(transform, false);
		uCon = go.GetComponent<uiControl>();
		uCon.name = "Jump";
		uCon.init(Controls.jumpIndex, "Jump");

		go = Instantiate<GameObject>(prefab);
		go.transform.SetParent(transform, false);
		uCon = go.GetComponent<uiControl>();
		uCon.name = "Kick";
		uCon.init(Controls.kickIndex, "Kick");

		go = Instantiate<GameObject>(prefab);
		go.transform.SetParent(transform, false);
		uCon = go.GetComponent<uiControl>();
		uCon.name = "Dash";
		uCon.init(Controls.DashIndex, "Dash");

		go = Instantiate<GameObject>(prefab);
		go.transform.SetParent(transform, false);
		uCon = go.GetComponent<uiControl>();
		uCon.name = "backDash";
		uCon.init(Controls.backDashIndex, "backDash");
	}

}
