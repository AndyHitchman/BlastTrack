namespace Test.Honeycomb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
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
            Repository.SetEventStore(eventStore);

            var ts = new TransactionScope();

            var dog = new Dog(new RegisterDog("test", null, null));

            var tarms =
                (Dictionary<Aggregate, AggregateResourceManager>)
                typeof (AggregateContext).GetField("trackedAggregateResourceManager", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            var actual = ((List<Event>) tarms[dog].AsDynamic().changes);
            actual.Count().ShouldEqual(1);
            actual.First().ShouldBeType<DogRegistered>();
        }

        [Test]
        public void events_raised_whilst_receiving_events_should_be_recorded()
        {
            var eventStore = Substitute.For<EventStore>();
            Repository.SetEventStore(eventStore);

            var ts = new TransactionScope();

            var dog = new Dog(new DogRegistered("test", null));

            var tarms =
                (Dictionary<Aggregate, AggregateResourceManager>)
                typeof (AggregateContext).GetField("trackedAggregateResourceManager", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            var actual = ((List<Event>) tarms[dog].AsDynamic().changes);
            actual.Count().ShouldEqual(1);
            actual.First().ShouldBeType<DogRequiresVaccinationWithin12Weeks>();
        }
    }
}