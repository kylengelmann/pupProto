using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveLeft : BehaviorTreeNode {

    GameObject gameObject;
    float speed = 3f;

    //new static public List<serializableProperty> serializableProperties = new List<serializableProperty> {
    //    new serializableProperty
    //    {
    //        Name = "Speed",
    //        Float = 3f,
    //        shownProperty = serializableProperty.Types.Float
    //    }
    //};

    public moveLeft(BehaviorTree tree, treeNode node) : base(tree, node)
    {
        gameObject = tree.gameObject;
        //speed = node.getProperty("Speed").Float;
    }

    protected override Status update()
    {
        gameObject.transform.position = gameObject.transform.position + Vector3.left * speed * Time.deltaTime;
        return Status.success;
    }
}
