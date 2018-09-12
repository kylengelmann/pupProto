using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BTENode("one", "Filters/Timer")]
public class timer : Filter {


    float time;

    [BTEField("Switch Time")]
    public float timeSwitch;

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
