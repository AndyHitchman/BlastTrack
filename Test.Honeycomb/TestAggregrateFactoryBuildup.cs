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
            var raisedTime = new DateTime(2013, 01, 09, 15, 54, 20);

            var ts = new TransactionScope();
            var actual = AggregateFactory.Restore(
                typeof (Dog),
                aggregateKey,
                new[]
                    {
                        new UniqueEvent(Guid.NewGuid(), new DogRegistered(aggregateKey, null), raisedTime)
                    });

            actual.ShouldNotBeNull();
            ((string)actual.AsDynamic().earbrand).ShouldEqual(aggregateKey);
        }

        [Test]
        public void it_should_buildup_the_aggregate_using_the_recorded_events()
        {
            var aggregateKey = "test";
            var givenName = "Wolfie";
            var raisedTime = new DateTime(2013, 01, 09, 15, 54, 20);            

            var ts = new TransactionScope();
            var actual = AggregateFactory.Restore(
                typeof (Dog),
                aggregateKey,
                new[]
                    {
                        new UniqueEvent(Guid.NewGuid(), new DogRegistered(aggregateKey, null), raisedTime),
                        new UniqueEvent(Guid.NewGuid(), new DogNamed(aggregateKey, givenName), raisedTime),
                    });

            ((string)actual.AsDynamic().name).ShouldEqual(givenName);
        }

        [Test]
        public void the_aggregate_content_should_report_live_for_the_instance_after_restore()
        {
            var aggregateKey = "test";
            var givenName = "Wolfie";
            var raisedTime = new DateTime(2013, 01, 09, 15, 54, 20);          

            var ts = new TransactionScope();
            var actual = AggregateFactory.Restore(
                typeof (Dog),
                aggregateKey,
                new[]
                    {
                        new UniqueEvent(Guid.NewGuid(), new DogRegistered(aggregateKey, null), raisedTime),
                        new UniqueEvent(Guid.NewGuid(), new DogNamed(aggregateKey, givenName), raisedTime),
                    });

            AggregateContext.GetAggregateLifestate(actual).ShouldEqual(AggregateLifestate.Live);
        }

        [Test]
        public void events_raised_whilst_restoring_do_not_propogate()
        {
            var aggregateKey = "test";
            var raisedTime = new DateTime(2013, 01, 09, 15, 54, 20);

            var ts = new TransactionScope();
            var actual = AggregateFactory.Restore(
                typeof (Dog),
                aggregateKey,
                new[]
                    {
                        new UniqueEvent(Guid.NewGuid(), new DogRegistered(aggregateKey, null), raisedTime)
                    });

            var resourceManager = AggregateContext.AggregateTracker[actual].ResourceManager;
                
            ((List<Event>)resourceManager.AsDynamic().changes).ShouldBeEmpty();
        }
    }
}