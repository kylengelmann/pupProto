using System;
using UnityEngine;

public class checkpoint : MonoBehaviour {

    Action onGetCheckpoint;

    [SerializeField] Transform resetPosition;

    public void subscribeOnGet(Action function)
    {
        onGetCheckpoint += function;
    }

    public void unsubscribeOnGet(Action funciton)
    {
        onGetCheckpoint -= funciton;
    }

    public Vector2 getPosition()
    {
        if(onGetCheckpoint != null) onGetCheckpoint.Invoke();
        return resetPosition.position;
    }


}
