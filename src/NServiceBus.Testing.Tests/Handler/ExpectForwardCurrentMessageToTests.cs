﻿namespace NServiceBus.Testing.Tests.Handler
{
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class ExpectForwardCurrentMessageToTests
    {
        [Test]
        public void ShouldFailExpectForwardCurrentMessageToIfMessageNotForwarded()
        {
            Assert.Throws<ExpectationException>(() => Test.Handler<NotForwardingMessageHandler>()
                .ExpectForwardCurrentMessageTo()
                .OnMessage<TestMessage>());
        }

        [Test]
        public void ShouldFailExpectForwardCurrentMessageWithCheckToIfMessageNotForwarded()
        {
            Assert.Throws<ExpectationException>(() => Test.Handler<NotForwardingMessageHandler>()
                .ExpectForwardCurrentMessageTo(dest => true)
                .OnMessage<TestMessage>());
        }

        [Test]
        public void ShouldFailExpectForwardCurrentMessageToIfMessageForwardedToUnexpectedDestination()
        {
            var handler = new ForwardingMessageHandler("someOtherDestination");

            Assert.Throws<ExpectationException>(() => Test.Handler(handler)
                .ExpectForwardCurrentMessageTo(dest => dest == "expectedDestination")
                .OnMessage<TestMessage>());
        }

        [Test]
        public void ShouldPassExpectForwardCurrentMessageToIfMessageForwarded()
        {
            var handler = new ForwardingMessageHandler("somewhere");

            Test.Handler(handler)
                .ExpectForwardCurrentMessageTo()
                .OnMessage<TestMessage>();
        }

        [Test]
        public void ShouldPassExpectForwardCurrentMessageToIfMessageForwardedToExpectedDestination()
        {
            const string forwardingDestination = "expectedDestination";
            var handler = new ForwardingMessageHandler(forwardingDestination);

            Test.Handler(handler)
                .ExpectForwardCurrentMessageTo(dest => dest == forwardingDestination)
                .OnMessage<TestMessage>();
        }

        [Test]
        public void ShouldPassExpectNotForwardCurrentMessageToIfMessageNotForwarded()
        {
            Test.Handler<NotForwardingMessageHandler>()
                .ExpectNotForwardCurrentMessageTo()
                .OnMessage<TestMessage>();
        }

        [Test]
        public void ShouldPassExpectNotForwardCurrentMessageToWithCheckIfMessageNotForwarded()
        {
            Test.Handler<NotForwardingMessageHandler>()
                .ExpectNotForwardCurrentMessageTo(dest => true)
                .OnMessage<TestMessage>();
        }

        [Test]
        public void ShouldFailExpectNotForwardCurrentMessageToIfMessageForwardedToAnyDestination()
        {
            var handler = new ForwardingMessageHandler("somewhere");

            Assert.Throws<ExpectationException>(() => Test.Handler(handler)
                .ExpectNotForwardCurrentMessageTo()
                .OnMessage<TestMessage>());
        }

        [Test]
        public void ShouldFailExpectNotForwardCurrentMessageToIfMessageForwardedToExpectedDestination()
        {
            const string forwardingDestination = "expectedDestination";
            var handler = new ForwardingMessageHandler(forwardingDestination);

            Assert.Throws<ExpectationException>(() => Test.Handler(handler)
                .ExpectNotForwardCurrentMessageTo(dest => dest == forwardingDestination)
                .OnMessage<TestMessage>());
        }

        [Test]
        public void ShouldPassExpectNotForwardCurrentMessageToIfMessageForwardedToUnexpectedDestination()
        {
            var handler = new ForwardingMessageHandler("someOtherDestination");

            Test.Handler(handler)
                .ExpectNotForwardCurrentMessageTo(dest => dest == "expectedDestination")
                .OnMessage<TestMessage>();
        }

        [Test]
        public void ExpectForwardCurrentMessageToShouldSupportMultipleForwardedMessages()
        {
            Test.Handler<MultipleForwardingsMessageHandler>()
                .ExpectForwardCurrentMessageTo(dest => dest == "dest1")
                .ExpectForwardCurrentMessageTo(dest => dest == "dest2")
                .ExpectNotForwardCurrentMessageTo(dest => dest == "dest3")
                .OnMessage<TestMessage>();
        }

        [Test]
        public void ForwardCurrentMessageToShouldBeThreadsafe()
        {
            var counter = 0;

            Assert.Throws<ExpectationException>(() => Test.Handler<ConcurrentHandler>()
                .WithExternalDependencies(h =>
                {
                    h.NumberOfThreads = 100;
                    h.HandlerAction = context => context.ForwardCurrentMessageTo("destination");
                })
                .ExpectForwardCurrentMessageTo(d =>
                {
                    Interlocked.Increment(ref counter);
                    return false;
                })
                .OnMessage<MyCommand>());

            Assert.AreEqual(100, counter);
        }

        public class NotForwardingMessageHandler : IHandleMessages<TestMessage>
        {
            public Task Handle(TestMessage message, IMessageHandlerContext context)
            {
                return Task.FromResult(0);
            }
        }

        public class ForwardingMessageHandler : IHandleMessages<TestMessage>
        {
            public ForwardingMessageHandler(string destination)
            {
                this.destination = destination;
            }

            public Task Handle(TestMessage message, IMessageHandlerContext context)
            {
                return context.ForwardCurrentMessageTo(destination);
            }

            readonly string destination;
        }

        public class MultipleForwardingsMessageHandler : IHandleMessages<TestMessage>
        {
            public async Task Handle(TestMessage message, IMessageHandlerContext context)
            {
                await context.ForwardCurrentMessageTo("dest1");
                await context.ForwardCurrentMessageTo("dest2");
            }
        }
    }
}