using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uiControl : MonoBehaviour {

	public GameObject key;
	private UnityEngine.UI.Text keyText;
	public GameObject control;
	private bool waiting = false;
	private bool wasSet = false;
	public RectTransform rect;
	private int _butt;
	private KeyCode setTo;

	public static int num;
		

	public void init(int button, string controlName) {
		control.GetComponentInChildren<UnityEngine.UI.Text>().text = controlName;
		keyText = key.GetComponentInChildren<UnityEngine.UI.Text>();
		_butt = button;
		keyText.text = Controls.butts[_butt].Key.ToString();
		rect.anchoredPosition = new Vector2(0, -35 - 40*num);
		num ++;
	}

	void Update() {
		if(wasSet) {
			for(int i = 0; i < Controls.numButts; i ++) {
				if(i == _butt) continue;
				else {
					if(Controls.butts[i].Key == setTo) {
						Controls.butts[i].Key = Controls.butts[_butt].Key;
						break;
					}
				}
			}
			Controls.butts[_butt].Key = setTo;
			saveNLoad.saveControls();
//			keyText.text = setTo.ToString();
			Global.pauseControls.enabled = true;
			wasSet = false;
			keyText.text = Controls.butts[_butt].Key.ToString();
		}
		if(Controls.butts[_butt].Key != setTo){
			keyText.text = Controls.butts[_butt].Key.ToString();
			setTo = Controls.butts[_butt].Key;
		}
	}


	void OnGUI(){
		
		if(waiting) {
			if(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != key) {
				onDeselect();
				return;
			}
			if(Event.current.type == EventType.KeyDown) {
				KeyCode current = Event.current.keyCode;
				if(current != KeyCode.W && current != KeyCode.A && current != KeyCode.S && current != KeyCode.D) {
					waiting = false;
					wasSet = true;
					setTo = current;
				}
				else {
					onDeselect();
				}
			}
		}
	}

	public void onClick() {
		Global.pauseControls.enabled = false;
		keyText.text = "Press any key to map to";
		waiting = true;
	}

	public void onDeselect() {
		keyText.text = Controls.butts[_butt].Key.ToString();
		setTo = Controls.butts[_butt].Key;
		waiting = false;
		Global.pauseControls.enabled = true;
	}


}
