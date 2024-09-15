namespace Bridge.Workflow;

public class Decide : StepBody
{
    public object Expression { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        return ExecutionResult.Outcome(Expression);
    }
}