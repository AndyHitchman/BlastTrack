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
    public class TestAggregrateRecordsEvents
    {
        [Test]
        public void events_raised_whilst_handling_commands_should_be_recorded()
        {
            var eventStore = Substitute.For<EventStore>();

            using (var ts = new TransactionScope())
            {
                var domain = new TestableDomain(null, null, eventStore);

                var key = "test";

                domain.Apply(new RegisterDog(key, null, null));

                var aggregateInfo = domain.AggregateTracker[typeof (Dog), key];
                aggregateInfo.Lifestate.ShouldEqual(AggregateLifestate.Untracked);

                var recorded = domain.TransactionTracker[Transaction.Current].RecordedEvents;
                recorded.Count().ShouldEqual(1);
                recorded.First().EventType.ShouldEqual(typeof (DogRegistered));
                recorded.First().UntypedEvent.ShouldBeType<DogRegistered>();
            }
        }

        [Test]
        public void events_raised_whilst_consuming_events_should_be_recorded()
        {
            var eventStore = Substitute.For<EventStore>();

            using (var ts = new TransactionScope())
            {
                var domain = new TestableDomain(null, null, eventStore);

                var key = "test";

                domain.Consume(
                    new PendingEvent(
                        new RaisedEvent(
                            new DogRegistered(key, null), 
                            Transaction.Current.TransactionInformation.LocalIdentifier, 
                            DateTimeOffset.UtcNow)));

                var aggregateInfo = domain.AggregateTracker[typeof (Dog), key];
                ((string) aggregateInfo.Instance.AsDynamic().earbrand).ShouldEqual(key);
                aggregateInfo.Lifestate.ShouldEqual(AggregateLifestate.Live);

                var recorded = domain.TransactionTracker[Transaction.Current].RecordedEvents;
                recorded.Count().ShouldEqual(1);
                recorded.First().EventType.ShouldEqual(typeof (DogRequiresVaccinationWithin12Weeks));
                recorded.First().UntypedEvent.ShouldBeType<DogRequiresVaccinationWithin12Weeks>();
            }
        }

        [Test]
        public void events_raised_whilst_handling_commands_should_be_emitted()
        {
            var eventEmitter = Substitute.For<EventEmitter>();
            var eventStore = Substitute.For<EventStore>();
            var key = "test";

            using (var ts = new TransactionScope())
            {
                var domain = new TestableDomain(eventEmitter, null, eventStore);

                var expected = new RegisterDog(key, null, null);

                domain.Apply(expected);

                ts.Complete();
            }

//            eventEmitter.Received().Emit(Arg.Is<RaisedEvent>(_ => _.EventType == typeof(DogRegistered)));
            eventEmitter.Received().Emit(Arg.Is<RaisedEvent>(_ => _.EventType == typeof(DogRegistered) && ((DogRegistered)_.UntypedEvent).Earbrand == key));
        }
    }
}