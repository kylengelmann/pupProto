using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveLeft : BehaviorTreeNode {

    GameObject gameObject;

    public moveLeft(BehaviorTree tree, uint ID) : base(tree, ID) 
    {
        gameObject = tree.gameObject;
    }

    protected override Status update()
    {
        gameObject.transform.position = gameObject.transform.position + Vector3.right * Time.deltaTime;
        return Status.success;
    }
}
