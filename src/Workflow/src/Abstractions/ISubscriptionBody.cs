namespace Bridge.Workflow;

public interface ISubscriptionBody : IStepBody
{
    object EventData { get; set; }        
}