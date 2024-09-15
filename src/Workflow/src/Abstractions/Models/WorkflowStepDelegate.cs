namespace Bridge.Workflow;

/// <summary>
/// Represents a function that executes a workflow step and returns a result.
/// </summary>
public delegate Task<ExecutionResult> WorkflowStepDelegate();