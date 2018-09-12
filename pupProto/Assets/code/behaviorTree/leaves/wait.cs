using UnityEngine;


[BTENode("none", "Leaves/Wait")]
public class wait : BehaviorTreeNode
{
    [BTEField("time")]
    public float waitTime;
    float startTime;


    public wait(BehaviorTree tree, treeNode node) : base(tree, node)
    {
        waitTime = node.getProperty("time").Float;
    }

    protected override void onStart()
    {
        startTime = Time.timeSinceLevelLoad;
    }

    protected override Status update()
    {
        return Time.timeSinceLevelLoad - startTime >= waitTime ? Status.success : Status.running;
    }

}
