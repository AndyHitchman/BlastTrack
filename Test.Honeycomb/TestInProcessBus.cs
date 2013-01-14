namespace Test.Honeycomb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using BlastTrack.Dogs;
    using NSubstitute;
    using NUnit.Framework;
    using ReflectionMagic;
    using Should;
    using global::Honeycomb;
    using global::Honeycomb.Infrastructure;
    using global::Honeycomb.Plumbing;

    [TestFixture]
    public class TestInProcessBus
    {
        [Test]
        public void events_emitted_should_be_collected()
        {
            var domain = Substitute.For<Domain>(null, null, null);
            var inProcessBus = new InProcessEventBus();

            var expected = new DogRegistered("test", "wibble");

            inProcessBus.Emit(new RaisedEvent(expected, null, DateTimeOffset.UtcNow));
            inProcessBus.StartCollectingEvents(domain);
            inProcessBus.StopCollecting(() => domain.Received().Consume(Arg.Is<PendingEvent>(_ => _.Event == expected)));
        }
    }
}