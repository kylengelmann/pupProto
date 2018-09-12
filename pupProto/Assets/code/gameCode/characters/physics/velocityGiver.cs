using System.Collections.Generic;
using UnityEngine;

public class velocityGiver : MonoBehaviour {

    private Vector2 _velocity;
    public Vector2 velocity
    {
        get
        {
            return _velocity;
        }

        set
        {
            _velocity = value;
            updateRecievers();
        }
    }

    private List<velocityReciever> recievers;

    private void Start()
    {
        recievers = new List<velocityReciever>();
    }

    public void addReciever(velocityReciever rec)
    {
        recievers.Add(rec);
    }

    public void removeReciever(velocityReciever rec)
    {
        recievers.Remove(rec);
    }

    private void updateRecievers()
    {
        foreach(velocityReciever vr in recievers)
        {
            vr.velocity = _velocity;
        }
    }
}
