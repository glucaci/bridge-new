using FluentAssertions;
using Xunit;

namespace Bridge.Bus.InMemory.Tests;

public class InMemoryMessageBusTests : IClassFixture<InMemoryBusHost<SuccessTestConsumer, TestMessage>>
{
    private readonly InMemoryBusHost<SuccessTestConsumer, TestMessage> _host;

    public InMemoryMessageBusTests(InMemoryBusHost<SuccessTestConsumer, TestMessage> host)
    {
        _host = host;
    }
    
    [Fact]
    public async Task CreateTestMessage_WhenSend_ConsumerWasCalled()
    {
        // Arrange
        var testMessage = new TestMessage();

        // Act
        await _host.MessageBus.Send(testMessage, _host.QueueName, default);
        
        // Assert
        _host.Consumer.WasCalled.Should().BeTrue();
    }
    
    [Fact]
    public async Task CreateTestMessage_WhenScheduleInFuture_ConsumerWasNotCalled()
    {
        // Arrange
        var testMessage = new TestMessage();
        var now = _host.TimeProvider.GetUtcNow();

        // Act
        await _host.MessageBus.Schedule(testMessage, _host.QueueName, now.AddHours(1), default);
        
        // Assert
        _host.Consumer.WasCalled.Should().BeFalse();
    }
    
    [Fact]
    public async Task CreateTestMessage_WhenSchedule_ConsumerWasCalled()
    {
        // Arrange
        var testMessage = new TestMessage();
        var now = _host.TimeProvider.GetUtcNow();
        _host.TimeProvider.Advance(TimeSpan.FromHours(2));

        // Act
        await _host.MessageBus.Schedule(testMessage, _host.QueueName, now.AddHours(1), default);
        
        // Assert
        _host.Consumer.WasCalled.Should().BeTrue();
    }
}

public record TestMessage();

public class SuccessTestConsumer : IConsumer<TestMessage>
{
    internal bool WasCalled { get; private set; }
        
    public ValueTask Handle(TestMessage message, CancellationToken cancellationToken)
    {
        WasCalled = true;
        return ValueTask.CompletedTask;
    }
}