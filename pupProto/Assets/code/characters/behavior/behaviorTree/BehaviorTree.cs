
public class BehaviorTree
{


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

    public Status tick()
    {
        if (status != Status.running)
        {
            onStart(status);
        }
        status = update();
        if (status != Status.running)
        {
            onStop(status);
        }
        return status;
    }

    public virtual void onStart(Status status) { }
    public virtual void onStop(Status status) { }
    protected abstract Status update();

}