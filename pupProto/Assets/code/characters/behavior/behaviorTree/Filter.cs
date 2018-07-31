public abstract class Filter : BehaviorTreeNode {

    BehaviorTreeNode child;

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
}
