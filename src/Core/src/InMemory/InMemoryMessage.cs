using CloudNative.CloudEvents;

namespace Bridge.InMemory;

internal record InMemoryMessage(DateTimeOffset EnqueueTime, CloudEvent CloudEvent);