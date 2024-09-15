namespace Bridge.Workflow;

public interface ILifeCycleEventHub
{
    Task PublishNotification(LifeCycleEvent evt);
    void Subscribe(Action<LifeCycleEvent> action);
    Task Start();
    Task Stop();
}