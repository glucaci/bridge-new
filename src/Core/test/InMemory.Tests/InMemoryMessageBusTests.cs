using FluentAssertions;
using Xunit;

namespace Bridge.InMemory.Tests;

public class InMemoryMessageBusTests
{
    [Fact]
    public async Task CreateTestMessage_WhenSend_ConsumerWasCalled()
    {
        // Arrange
        var host = await InMemoryBusHost<TestConsumer, TestMessage>.Create();
        var testMessage = new TestMessage();

        // Act
        await host.MessageBus.Send(testMessage, host.QueueName, default);
        await host.WaitForConsumer(host.QueueName);

        // Assert
        host.Consumer.WasCalled.Should().BeTrue();
    }
    
    [Fact]
    public async Task CreateTestMessage_WhenScheduleInFuture_ConsumerWasNotCalled()
    {
        // Arrange
        var testMessage = new TestMessage();
        var host = await InMemoryBusHost<TestConsumer, TestMessage>.Create();
        var now = host.TimeProvider.GetUtcNow();

        // Act
        await host.MessageBus.Schedule(testMessage, host.QueueName, now.AddHours(1), default);
        
        // Assert
        var queue = host.MessageBus.GetQueue(host.QueueName);
        queue.GetItems().Should().ContainSingle(i => i.EnqueueTime == now.AddHours(1));
        host.Consumer.WasCalled.Should().BeFalse();
    }
    
    [Fact]
    public async Task CreateTestMessage_WhenSchedule_ConsumerWasCalled()
    {
        // Arrange
        var testMessage = new TestMessage();
        var host = await InMemoryBusHost<TestConsumer, TestMessage>.Create();
        var now = host.TimeProvider.GetUtcNow();

        // Act
        await host.MessageBus.Schedule(testMessage, host.QueueName, now.AddHours(1), default);
        host.TimeProvider.Advance(TimeSpan.FromHours(2));
        await host.WaitForConsumer(host.QueueName);
        
        // Assert
        host.Consumer.WasCalled.Should().BeTrue();
    }

    private record TestMessage;

    private class TestConsumer : IConsumer<TestMessage>
    {
        internal bool WasCalled { get; private set; }
        
        public ValueTask Handle(TestMessage message, CancellationToken cancellationToken)
        {
            WasCalled = true;
            return ValueTask.CompletedTask;
        }
    }
}