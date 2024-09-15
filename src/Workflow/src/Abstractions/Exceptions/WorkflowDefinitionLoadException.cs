namespace Bridge.Workflow;

public class WorkflowDefinitionLoadException : Exception
{
    public WorkflowDefinitionLoadException(string message)
        : base (message)
    {            
    }
}