namespace Bridge.Workflow;

public class Page<T>
{
    public ICollection<T> Data { get; set; }
    public long Total { get; set; }
}