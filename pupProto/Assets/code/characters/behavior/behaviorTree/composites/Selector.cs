using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : BehaviorTreeNode
{

    List<BehaviorTreeNode> children = new List<BehaviorTreeNode>();

    BehaviorTreeNode running;

    public Selector(BehaviorTree tree, treeNode node) : base(tree, node) { }

    protected override Status update()
    {
        foreach (BehaviorTreeNode child in children)
        {
            Status status = child.tick();
            if (status == Status.failure) continue;
            if (child != running && running != null) running.Stop();
            running = child;
            return status;
        }
        running = null;
        return Status.failure;
    }

    public override void addChild(BehaviorTreeNode child)
    {
        children.Add(child);
    }

    public void removeChild(BehaviorTreeNode child)
    {
        child.Stop();
        children.Remove(child);
    }

    protected override void onStop()
    {
        if (running == null) return;
        running.Stop();
    }
}
