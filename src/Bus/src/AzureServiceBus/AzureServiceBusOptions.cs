namespace Bridge.Bus.AzureServiceBus;

public sealed class AzureServiceBusOptions
{
    public AuthenticationType AuthenticationType { get; set; } = AuthenticationType.AccessKeys;
    public string? ConnectionString { get; set; }
    public string? FullyQualifiedNamespace { get; set; }
}

public enum AuthenticationType
{
    AccessKeys,
    EntraId
}