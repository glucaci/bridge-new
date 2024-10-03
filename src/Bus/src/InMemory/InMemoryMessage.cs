using Azure.Messaging;

namespace Bridge.Bus.InMemory;

internal record InMemoryMessage(DateTimeOffset EnqueueTime, CloudEvent CloudEvent);