﻿namespace Bridge.Workflow;

public class NullSearchIndex : ISearchIndex
{
    public Task IndexWorkflow(WorkflowInstance workflow)
    {
        return Task.CompletedTask;
    }

    public Task<Page<WorkflowSearchResult>> Search(string terms, int skip, int take, params SearchFilter[] filters)
    {
        throw new NotImplementedException();
    }

    public Task Start()
    {
        return Task.CompletedTask;
    }

    public Task Stop()
    {
        return Task.CompletedTask;
    }
}