using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bridge.Workflow;

public interface IWorkflowHost : IWorkflowController, IActivityController, IHostedService
{
    /// <summary>
    /// Start the workflow host, this enable execution of workflows
    /// </summary>
    void Start();

    /// <summary>
    /// Stop the workflow host
    /// </summary>
    void Stop();
        
        
    event StepErrorEventHandler OnStepError;
    event LifeCycleEventHandler OnLifeCycleEvent;
    void ReportStepError(WorkflowInstance workflow, WorkflowStep step, Exception exception);

    //public dependencies to allow for extension method access
    IPersistenceProvider PersistenceStore { get; }
    IDistributedLockProvider LockProvider { get; }
    IWorkflowRegistry Registry { get; }
    WorkflowOptions Options { get; }
    IQueueProvider QueueProvider { get; }
    ILogger Logger { get; }

}

public delegate void StepErrorEventHandler(WorkflowInstance workflow, WorkflowStep step, Exception exception);
public delegate void LifeCycleEventHandler(LifeCycleEvent evt);