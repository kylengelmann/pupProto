using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveRight : BehaviorTreeNode {

    GameObject gameObject;
    float time = 3f;

    public moveRight(BehaviorTree tree, uint ID) : base(tree, ID)
    {
        gameObject = tree.gameObject;
    }

    protected override Status update()
    {
        gameObject.transform.position = gameObject.transform.position + Vector3.left * Time.deltaTime;
        time -= Time.deltaTime;
        return Status.running;
    }
}
