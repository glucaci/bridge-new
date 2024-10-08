﻿namespace Bridge.Workflow;

public class WorkflowNotRegisteredException : Exception
{
    public WorkflowNotRegisteredException(string workflowId, int? version)
        : base($"Workflow {workflowId} {version} is not registered")
    {
    }
}