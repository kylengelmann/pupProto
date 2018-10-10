using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class world : MonoBehaviour {

    public static world activeWorld;

	void Start () {
        if(activeWorld != null)
		activeWorld.enabled = false;

        activeWorld = this;
	}

    private void OnEnable()
    {
        if (activeWorld != null)
            activeWorld.enabled = false;

        activeWorld = this;
    }

    public void resetWorld()
    {
        BroadcastMessage("onReset", SendMessageOptions.DontRequireReceiver);
    }
}
