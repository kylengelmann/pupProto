public abstract class Filter : BehaviorTreeNode {

    BehaviorTreeNode child;

    public Filter(BehaviorTree tree, treeNode node) : base(tree, node) { }

    protected override Status update()
    {
        if(condition())
        {
            return child.tick();
        }
        child.Stop();
        return Status.failure;
    }

    protected abstract bool condition();

    protected override void onStop()
    {
        if(child == null) return;
        child.Stop();
    }

    public override void addChild(BehaviorTreeNode child)
    {
        this.child = child;
    }
}
