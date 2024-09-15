namespace Bridge.Workflow;

public interface ISearchable
{
    IEnumerable<string> GetSearchTokens();
}