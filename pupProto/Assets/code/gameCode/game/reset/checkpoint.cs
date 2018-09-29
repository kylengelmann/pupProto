using System;
using UnityEngine;

public class checkpoint : interactable {

    Action onCheckpointActivated;
    Action onCheckpointUnactivated;

    static checkpoint active = null;

    public void subscribeOnCheckpointActivated(Action function)
    {
        onCheckpointActivated += function;
    }

    public void unsubscribeOnCheckpointActivated(Action funciton)
    {
        onCheckpointActivated -= funciton;
    }

    public void subscribeOnCheckpointUnactivated(Action function)
    {
        onCheckpointUnactivated += function;
    }

    public void unsubscribeOnCheckpointUnactivated(Action funciton)
    {
        onCheckpointUnactivated -= funciton;
    }

    public override void interact(GameObject didInteraction)
    {
        if(active != null)
        {
            active.onCheckpointUnactivated.Invoke();
        }
        active = this;
        onCheckpointActivated.Invoke();
    }
}
