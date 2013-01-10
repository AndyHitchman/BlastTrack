namespace Test.Honeycomb
{
    using System;
    using System.Collections.Generic;
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
    public class TestAggregrateFactoryBuildup
    {
        [Test]
        public void it_should_construct_the_aggregate_using_the_first_event()
        {
            var aggregateKey = "test";

            var eventStore = Substitute.For<EventStore>();
            var raisedTime = new DateTime(2013, 01, 09, 15, 54, 20);
            eventStore.GetEventsForAggregate(typeof(Dog), aggregateKey)
                .Returns(new[]
                    {
                        new UniqueEvent(Guid.NewGuid(), new DogRegistered(aggregateKey, null), raisedTime)
                    });
            
            var subject = new AggregateFactory<Dog, string>(eventStore);

            var ts = new TransactionScope();
            var actual = subject.Restore(aggregateKey);

            actual.ShouldNotBeNull();
            ((string)actual.AsDynamic().earbrand).ShouldEqual(aggregateKey);
        }

        [Test]
        public void it_should_buildup_the_aggregate_using_the_recorded_events()
        {
            var aggregateKey = "test";
            var givenName = "Wolfie";

            var eventStore = Substitute.For<EventStore>();
            var raisedTime = new DateTime(2013, 01, 09, 15, 54, 20);
            eventStore.GetEventsForAggregate(typeof(Dog), aggregateKey)
                .Returns(new[]
                    {
                        new UniqueEvent(Guid.NewGuid(), new DogRegistered(aggregateKey, null), raisedTime),
                        new UniqueEvent(Guid.NewGuid(), new DogNamed(aggregateKey, givenName), raisedTime),
                    });
            
            var subject = new AggregateFactory<Dog, string>(eventStore);

            var ts = new TransactionScope();
            var actual = subject.Restore(aggregateKey);

            ((string)actual.AsDynamic().name).ShouldEqual(givenName);
        }

        [Test]
        public void the_aggregate_content_should_report_live_for_the_instance_after_restore()
        {
            var aggregateKey = "test";
            var givenName = "Wolfie";

            var eventStore = Substitute.For<EventStore>();
            var raisedTime = new DateTime(2013, 01, 09, 15, 54, 20);
            eventStore.GetEventsForAggregate(typeof(Dog), aggregateKey)
                .Returns(new[]
                    {
                        new UniqueEvent(Guid.NewGuid(), new DogRegistered(aggregateKey, null), raisedTime),
                        new UniqueEvent(Guid.NewGuid(), new DogNamed(aggregateKey, givenName), raisedTime),
                    });
            
            var subject = new AggregateFactory<Dog, string>(eventStore);

            var ts = new TransactionScope();
            var actual = subject.Restore(aggregateKey);

            AggregateContext.IsLive(actual).ShouldBeTrue();
        }

        [Test]
        public void events_raised_whilst_restoring_do_not_propogate()
        {
            var aggregateKey = "test";

            var eventStore = Substitute.For<EventStore>();
            var raisedTime = new DateTime(2013, 01, 09, 15, 54, 20);
            eventStore.GetEventsForAggregate(typeof(Dog), aggregateKey)
                .Returns(new[]
                    {
                        new UniqueEvent(Guid.NewGuid(), new DogRegistered(aggregateKey, null), raisedTime)
                    });

            var subject = new AggregateFactory<Dog, string>(eventStore);

            var ts = new TransactionScope();
            var actual = subject.Restore(aggregateKey);

            var tarms =
                (Dictionary<Aggregate, AggregateResourceManager>)
                typeof (AggregateContext).GetField("trackedAggregateResourceManager", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            ((List<Event>)tarms[actual].AsDynamic().changes).ShouldBeEmpty();
        }
    }
}