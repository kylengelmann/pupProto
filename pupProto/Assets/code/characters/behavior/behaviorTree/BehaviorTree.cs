
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
            onStart();
        }
        status = update();
        if (status != Status.running)
        {
            onStop();
        }
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

}