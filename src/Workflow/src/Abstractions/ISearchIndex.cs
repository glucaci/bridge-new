namespace Bridge.Workflow;

public interface ISearchIndex
{
    Task IndexWorkflow(WorkflowInstance workflow);

    Task<Page<WorkflowSearchResult>> Search(string terms, int skip, int take, params SearchFilter[] filters);

    Task Start();

    Task Stop();
}