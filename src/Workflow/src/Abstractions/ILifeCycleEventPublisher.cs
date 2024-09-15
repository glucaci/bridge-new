namespace Bridge.Workflow;

public interface ILifeCycleEventPublisher : IBackgroundTask
{
    void PublishNotification(LifeCycleEvent evt);
}