namespace Test.Honeycomb
{
    using System;
    using System.Transactions;
    using BlastTrack.BoundedContext.MemberServices.Dog;
    using BlastTrack.BoundedContext.MemberServices.Dog.Events;
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
            using (var ts = new TransactionScope())
            {
                var domain = new TestableDomain(null, null, null);

                var aggregateKey = "test";

                var ai = domain.AggregateTracker[typeof (Dog), aggregateKey];
                new TestableAggregateFactory().Buildup(
                    ai,
                    new Event[]
                        {
                            new DogRegistered(aggregateKey, null)
                        });

                domain.TransactionTracker[Transaction.Current].RecordedEvents.ShouldBeEmpty();
            }
        }

        [Test]
        public void it_should_buildup_the_aggregate_using_the_recorded_events()
        {
            using (var ts = new TransactionScope())
            {
                var domain = new TestableDomain(null, null, null);

                var aggregateKey = "test";
                var givenName = "Wolfie";
                var assignedDate = new DateTime(2013, 05, 16);

                var ai = domain.AggregateTracker[typeof (Dog), aggregateKey];
                new TestableAggregateFactory().Buildup(
                    ai,
                    new Event[]
                        {
                            new DogRegistered(aggregateKey, null),
                            new DogNamed(aggregateKey, givenName, assignedDate)
                        });

                ((string) ai.Instance.AsDynamic().name).ShouldEqual(givenName);
            }
        }

        [Test]
        public void it_should_construct_the_aggregate_using_the_first_event()
        {
            using (var ts = new TransactionScope())
            {
                var domain = new TestableDomain(null, null, null);

                var aggregateKey = "test";

                var ai = domain.AggregateTracker[typeof (Dog), aggregateKey];
                new TestableAggregateFactory().Buildup(
                    ai,
                    new Event[]
                        {
                            new DogRegistered(aggregateKey, null)
                        });

                ai.Instance.ShouldNotBeNull();
                ((string) ai.Instance.AsDynamic().earbrand).ShouldEqual(aggregateKey);
            }
        }

        [Test]
        public void the_aggregate_content_should_report_live_for_the_instance_after_restore()
        {
            using (var ts = new TransactionScope())
            {
                var domain = new TestableDomain(null, null, null);

                var aggregateKey = "test";
                var givenName = "Wolfie";
                var assignedDate = new DateTime(2013, 05, 16);

                var ai = domain.AggregateTracker[typeof (Dog), aggregateKey];
                new TestableAggregateFactory().Buildup(
                    ai,
                    new Event[]
                        {
                            new DogRegistered(aggregateKey, null),
                            new DogNamed(aggregateKey, givenName, assignedDate)
                        });

                ai.Lifestate.ShouldEqual(AggregateLifestate.Live);
                ((string) ai.Instance.AsDynamic().earbrand).ShouldEqual(aggregateKey);
                ((string) ai.Instance.AsDynamic().name).ShouldEqual(givenName);
                ((DateTime) ai.Instance.AsDynamic().nameAssignedDate).ShouldEqual(assignedDate);
            }
        }
    }
}