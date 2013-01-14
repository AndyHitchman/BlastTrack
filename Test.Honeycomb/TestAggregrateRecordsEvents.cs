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
            var domain = new TestableDomain(null, null, eventStore);
            domain.StartTransaction();

            var key = "test";

            domain.Apply(new RegisterDog(key, null, null));

            var aggregateInfo = domain.AggregateTracker[typeof (Dog), key];
            ((string) aggregateInfo.Instance.AsDynamic().earbrand).ShouldEqual(key);
            aggregateInfo.Lifestate.ShouldEqual(AggregateLifestate.Live);

            var recorded = domain.TransactionTracker[Transaction.Current].RecordedEvents;
            recorded.Count().ShouldEqual(2);
            recorded.First().EventType.ShouldEqual(typeof (DogRegistered));
            recorded.First().UntypedEvent.ShouldBeType<DogRegistered>();
            recorded.Skip(1).First().EventType.ShouldEqual(typeof (DogRequiresVaccinationWithin12Weeks));
            recorded.Skip(1).First().UntypedEvent.ShouldBeType<DogRequiresVaccinationWithin12Weeks>();
        }

        [Test]
        public void events_raised_whilst_consuming_events_should_be_recorded()
        {
            var eventStore = Substitute.For<EventStore>();
            var domain = new TestableDomain(null, null, eventStore);
            domain.StartTransaction();

            var key = "test";

            domain.Raise(new DogRegistered(key, null));

            var aggregateInfo = domain.AggregateTracker[typeof (Dog), key];
            ((string) aggregateInfo.Instance.AsDynamic().earbrand).ShouldEqual(key);
            aggregateInfo.Lifestate.ShouldEqual(AggregateLifestate.Live);

            var recorded = domain.TransactionTracker[Transaction.Current].RecordedEvents;
            recorded.Count().ShouldEqual(2);
            recorded.First().EventType.ShouldEqual(typeof (DogRegistered));
            recorded.First().UntypedEvent.ShouldBeType<DogRegistered>();
            recorded.Skip(1).First().EventType.ShouldEqual(typeof (DogRequiresVaccinationWithin12Weeks));
            recorded.Skip(1).First().UntypedEvent.ShouldBeType<DogRequiresVaccinationWithin12Weeks>();
        }

        [Test]
        public void events_raised_whilst_handling_commands_should_be_emitted()
        {
            var eventEmitter = Substitute.For<EventEmitter>();
            var eventStore = Substitute.For<EventStore>();
            var domain = new TestableDomain(eventEmitter, null, eventStore);
            var ts = domain.StartTransaction();

            var key = "test";
            var expected = new RegisterDog(key, null, null);
        
            domain.Apply(expected);

            ts.Complete();
            ts.Dispose();

            eventEmitter.Received().Emit(Arg.Is<Type>(_ => _ == typeof (DogRegistered)), Arg.Any<DogRegistered>());
            eventEmitter.Received().Emit(Arg.Is<Type>(_ => _ == typeof (DogRequiresVaccinationWithin12Weeks)), Arg.Any<DogRequiresVaccinationWithin12Weeks>());
        }
    }
}