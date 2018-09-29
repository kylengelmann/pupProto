using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpointColor : MonoBehaviour {

    public Color normalColor;
    public Color activeColor;

    SpriteRenderer sr;
    

	void Start () {
		checkpoint checkpoint = GetComponent<checkpoint>();
        checkpoint.subscribeOnCheckpointActivated(setActive);
        checkpoint.subscribeOnCheckpointUnactivated(setUnactive);

        sr = GetComponent<SpriteRenderer>();
        sr.color = normalColor;
	}
	
    void setActive()
    {
        sr.color = activeColor;
    }

    void setUnactive()
    {
        sr.color = normalColor;
    }
}
