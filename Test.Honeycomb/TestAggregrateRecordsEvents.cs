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

    [TestFixture]
    public class TestAggregrateRecordsEvents
    {
        [Test]
        public void events_raised_whilst_handling_commands_should_be_recorded()
        {
            var eventStore = Substitute.For<EventStore>();
            var domain = new TestableDomain(eventStore);
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
            var domain = new TestableDomain(eventStore);
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
        public void events_raised_whilst_handling_commands_should_be_recorded_to_an_event_store()
        {
            var eventStore = Substitute.For<EventStore>();
            var domain = new TestableDomain(eventStore);
            var ts = domain.StartTransaction();

            var key = "test";
            var affectedAggregates = new List<Dictionary<AggregateInfo, UniqueEvent.ConsumptionRecord>>();
            var expected = new RegisterDog(key, null, null);

            eventStore.RecordEvent(
                          Arg.Any<Type>(),
                          Arg.Any<Event>(),
                          Arg.Do<Dictionary<AggregateInfo, UniqueEvent.ConsumptionRecord>>(p => affectedAggregates.Add(p)));
            
            domain.Apply(expected);

            ts.Complete();
            ts.Dispose();

            eventStore.Received()
                      .RecordEvent(
                          typeof (DogRegistered),
                          Arg.Any<DogRegistered>(),
                          Arg.Any<Dictionary<AggregateInfo, UniqueEvent.ConsumptionRecord>>());

            eventStore.Received()
                      .RecordEvent(
                          typeof (DogRequiresVaccinationWithin12Weeks),
                          Arg.Any<DogRequiresVaccinationWithin12Weeks>(),
                          Arg.Any<Dictionary<AggregateInfo, UniqueEvent.ConsumptionRecord>>());

            affectedAggregates.Count.ShouldEqual(2);
            affectedAggregates[0].Count.ShouldEqual(1);
            affectedAggregates[1].Count.ShouldEqual(0);

            affectedAggregates[0][domain.AggregateTracker[typeof(Dog), key]].ConsumptionException.ShouldBeNull();
            affectedAggregates[0][domain.AggregateTracker[typeof(Dog), key]].ExecutionTime.CompareTo(TimeSpan.Zero).ShouldBeInRange(1, 1);
        }
    }
}