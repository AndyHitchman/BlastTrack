namespace Test.Honeycomb
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;
    using BlastTrack.Dogs;
    using NUnit.Framework;
    using ReflectionMagic;
    using Should;
    using global::Honeycomb;
    using global::Honeycomb.Infrastructure;

    [TestFixture]
    public class TestAggregrateFactoryBuildup
    {
        [Test]
        public void events_raised_whilst_restoring_do_not_propogate()
        {
            var domain = new TestableDomain(null);
            domain.StartTransaction();

            string aggregateKey = "test";
            var raisedTime = new DateTime(2013, 01, 09, 15, 54, 20);

            AggregateInfo ai = domain.AggregateTracker[typeof (Dog), aggregateKey];
            AggregateFactory.Buildup(
                ai,
                new Event[]
                    {
                        new DogRegistered(aggregateKey, null)
                    });

            domain.TransactionTracker[Transaction.Current].RecordedEvents.ShouldBeEmpty();
        }

        [Test]
        public void it_should_buildup_the_aggregate_using_the_recorded_events()
        {
            var domain = new TestableDomain(null);
            domain.StartTransaction();

            string aggregateKey = "test";
            string givenName = "Wolfie";
            var raisedTime = new DateTime(2013, 01, 09, 15, 54, 20);

            AggregateInfo ai = domain.AggregateTracker[typeof (Dog), aggregateKey];
            AggregateFactory.Buildup(
                ai,
                new Event[]
                    {
                        new DogRegistered(aggregateKey, null),
                        new DogNamed(aggregateKey, givenName)
                    });

            ((string) ai.Instance.AsDynamic().name).ShouldEqual(givenName);
        }

        [Test]
        public void it_should_construct_the_aggregate_using_the_first_event()
        {
            var domain = new TestableDomain(null);
            domain.StartTransaction();

            string aggregateKey = "test";
            var raisedTime = new DateTime(2013, 01, 09, 15, 54, 20);


            AggregateInfo ai = domain.AggregateTracker[typeof (Dog), aggregateKey];
            AggregateFactory.Buildup(
                ai,
                new Event[]
                    {
                        new DogRegistered(aggregateKey, null)
                    });

            ai.Instance.ShouldNotBeNull();
            ((string) ai.Instance.AsDynamic().earbrand).ShouldEqual(aggregateKey);
        }

        [Test]
        public void the_aggregate_content_should_report_live_for_the_instance_after_restore()
        {
            var domain = new TestableDomain(null);
            domain.StartTransaction();

            string aggregateKey = "test";
            string givenName = "Wolfie";
            var raisedTime = new DateTime(2013, 01, 09, 15, 54, 20);

            AggregateInfo ai = domain.AggregateTracker[typeof (Dog), aggregateKey];
            AggregateFactory.Buildup(
                ai,
                new Event[]
                    {
                        new DogRegistered(aggregateKey, null),
                        new DogNamed(aggregateKey, givenName)
                    });

            ai.Lifestate.ShouldEqual(AggregateLifestate.Live);
        }
    }
}