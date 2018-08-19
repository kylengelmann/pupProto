using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timer : Filter {

    float time = 3f;

    public timer(BehaviorTree tree, uint ID) : base(tree, ID) {}

    protected override bool condition()
    {
        time -= Time.deltaTime;
        if(time <= -3f) {
            time = 3f; 
            return false;
        }

        return time > 0f;
    }

}
