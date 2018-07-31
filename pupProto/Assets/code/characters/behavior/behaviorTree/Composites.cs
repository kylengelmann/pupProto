using System.Collections.Generic;

public class Selector : BehaviorTreeNode {

    List<BehaviorTreeNode> children = new List<BehaviorTreeNode>();

    BehaviorTreeNode running;

    protected override Status update()
    {
        foreach(BehaviorTreeNode child in children)
        {
            Status status = child.tick();
            if(status == Status.failure) continue;
            if(child != running && running != null) running.Stop();
            running = child;
            return status;
        }
        running = null;
        return Status.failure;
    }

    public void addChild(BehaviorTreeNode child)
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
        if(running == null) return;
        running.Stop();
    }
}

public class Sequence : BehaviorTreeNode
{
    List<BehaviorTreeNode> children = new List<BehaviorTreeNode>();

    int running;

    bool didFail = true;

    protected override Status update()
    {
        for (; running < children.Count; ++running)
        {
            Status status = children[running].tick();
            if(status != Status.failure) didFail = false;
            if(status == Status.running) return Status.running;
            
        }
        Status result = didFail ? Status.failure : Status.success;
        didFail = true;
        running = 0;
        return result;
    }

    public void addChild(BehaviorTreeNode child)
    {
        children.Add(child);
    }

    public void insertChild(int idx, BehaviorTreeNode child)
    {
        if(idx <= running) ++running;
        children.Insert(idx, child);
    }

    public void removeChild(BehaviorTreeNode child)
    {
        int idx = children.IndexOf(child);
        child.Stop();
        if(idx < running)
        {
            --running;
        }
        else if(running >= children.Count - 1)
        {
            running = 0;
        }
        
        children.Remove(child);
    }

    protected override void onStop()
    {
        children[running].Stop();
    }
}
