using System.Collections.Generic;
using System;
using UnityEngine;

public class BehaviorTree : MonoBehaviour
{

    BehaviorTreeNode root;
    public TreeData tree;
       
#if UNITY_EDITOR
    public Dictionary<uint, BehaviorTreeNode.Status> nodeStatus;
#endif

    void Start()
    {
        List<BehaviorTreeNode> nodes = new List<BehaviorTreeNode>();

#if UNITY_EDITOR
        nodeStatus = new Dictionary<uint, BehaviorTreeNode.Status>();
#endif
        foreach (treeNode node in tree.nodes)
        {
            Type nodeType = Type.GetType(node.Type);
            BehaviorTreeNode newNode = (BehaviorTreeNode)Activator.CreateInstance(nodeType, new object[] {this, node});
            if(node.ID == 0) root = newNode;
            nodes.Add(newNode);
#if UNITY_EDITOR
            nodeStatus.Add(node.ID, (BehaviorTreeNode.Status)(-1));
#endif
        }
        foreach (treeConnection connection in tree.connections)
        {
            BehaviorTreeNode parent = nodes.Find(delegate(BehaviorTreeNode node)
            {
                return node.ID == connection.parentID;
            });

            BehaviorTreeNode child = nodes.Find(delegate (BehaviorTreeNode node)
            {
                return node.ID == connection.childID;
            });

            parent.addChild(child);
        }
    }

    private void Update()
    {
        root.tick();
    }

}

public abstract class BehaviorTreeNode
{
    public enum Status
    {
        running,
        success,
        failure
    }

    Status status = Status.failure;
    BehaviorTree tree;
    public uint ID;

    public BehaviorTreeNode(BehaviorTree tree, treeNode node)
    {
        this.tree = tree;
        ID = node.ID;
    }

    public Status tick()
    {
        if (status != Status.running)
        {
            onStart();
        }
        status = update();
        
        if (status != Status.running)
        {
            onStop();
        }

#if UNITY_EDITOR
        tree.nodeStatus[ID] = status;
#endif
        return status;
    }

    protected virtual void onStart() { }
    protected virtual void onStop() { }
    protected abstract Status update();

    public void Stop()
    {
        if(status == Status.failure) return;
        onStop();
    }

    public virtual void addChild(BehaviorTreeNode child) { }

}

public class Root : BehaviorTreeNode
{

    BehaviorTreeNode child;

    public Root(BehaviorTree tree, treeNode node) : base(tree, node) { }

    public override void addChild(BehaviorTreeNode child)
    {
        this.child = child;
    }

    protected override Status update()
    {
        child.tick();
        return Status.success;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class BTENode : Attribute
{
    public string children { get; private set; }
    public string menuName {get; private set;}

    public BTENode(string children, string menuName)
    {
        this.children = children;
        this.menuName = menuName;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class BTEField : Attribute
{
    public string description { get; private set; }

    public BTEField(string description)
    {
        this.description = description;
    }
}
