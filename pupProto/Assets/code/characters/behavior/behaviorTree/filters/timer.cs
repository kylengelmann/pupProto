using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timer : Filter {

    new static public List<serializableProperty> serializableProperties = new List<serializableProperty> {
        new serializableProperty
        {
            Name = "Switch Time",
            Float = 3f,
            shownProperty = serializableProperty.Types.Float
        }
    };

    float time;
    float timeSwitch;

    public timer(BehaviorTree tree, treeNode node) : base(tree, node) {
        timeSwitch = node.getProperty("Switch Time").Float;
        time = timeSwitch;
    }

    protected override bool condition()
    {
        time -= Time.deltaTime;
        if(time <= -timeSwitch) {
            time = timeSwitch; 
            return false;
        }

        return time > 0f;
    }

}
