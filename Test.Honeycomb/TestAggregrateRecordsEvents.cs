namespace Test.Honeycomb
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Transactions;
    using BlastTrack.BoundedContext.MemberServices.Dog;
    using BlastTrack.BoundedContext.MemberServices.Dog.Commands;
    using BlastTrack.BoundedContext.MemberServices.Dog.Events;
    using NSubstitute;
    using NUnit.Framework;
    using ReflectionMagic;
    using Should;
    using global::Honeycomb.Infrastructure;
    using global::Honeycomb.Plumbing;

    [TestFixture]
    public class TestAggregrateRecordsEvents
    {
        [Test]
        public void events_raised_whilst_handling_commands_should_be_recorded()
        {
            var eventStore = Substitute.For<EventStore>();

            using ( new TransactionScope())
            {
                var domain = new TestableDomain(null, null, eventStore);

                var key = "test";

                domain.Apply(new RegisterDog(key, null, null));

                var aggregateInfo = domain.AggregateTracker[typeof (Dog), key];
                aggregateInfo.Lifestate.ShouldEqual(AggregateLifestate.Untracked);

                var recorded = domain.TransactionTracker[Transaction.Current].RecordedEvents;
                recorded.Count().ShouldEqual(1);
                recorded.First().Event.ShouldBeType<DogRegistered>();
            }
        }

        [Test]
        public void events_raised_whilst_consuming_events_should_be_recorded()
        {
            var waitForConsumption = new ManualResetEventSlim(false);
            Transaction consumerTransaction = null;

            var eventStore = Substitute.For<EventStore>();

            //Because consumers run in background threads, we need to block until complete before we assert.
            eventStore.When(_ => _.LogConsumption(Arg.Any<RaisedEvent>(), Arg.Any<ConsumptionLog>())).Do(info =>
                                                                                                             {
                                                                                                                 consumerTransaction =
                                                                                                                     Transaction.Current;
                                                                                                                 waitForConsumption.Set();
                                                                                                             });

            var domain = new TestableDomain(null, null, eventStore);

            var key = "test";

            domain.Consume(
                new RaisedEvent(
                    new DogRegistered(key, null),
                    DateTimeOffset.UtcNow));

            waitForConsumption.Wait();

            var aggregateInfo = domain.AggregateTracker[typeof (Dog), key];

            ((string) aggregateInfo.Instance.AsDynamic().earbrand).ShouldEqual(key);
            aggregateInfo.Lifestate.ShouldEqual(AggregateLifestate.Live);

            var recorded = domain.TransactionTracker[consumerTransaction].RecordedEvents;
            recorded.Count().ShouldEqual(1);
            recorded.First().Event.ShouldBeType<DogIsNotVaccinated>();
        }

        [Test]
        public void events_raised_whilst_consuming_events_should_be_emitted()
        {
            var waitForConsumption = new ManualResetEventSlim(false);

            var eventEmitter = Substitute.For<EventEmitter>();
            var eventStore = Substitute.For<EventStore>();

            //Because consumers run in background threads, we need to block until complete before we assert.
            eventStore.When(_ => _.LogConsumption(Arg.Any<RaisedEvent>(), Arg.Any<ConsumptionLog>())).Do(
                info => { waitForConsumption.Set(); });

            var domain = new TestableDomain(eventEmitter, null, eventStore);

            var key = "test";

            domain.Consume(
                new RaisedEvent(
                    new DogRegistered(key, null),
                    DateTimeOffset.UtcNow));

            waitForConsumption.Wait();

            eventEmitter.Received().Emit(Arg.Is<RaisedEvent>(_ => ((DogIsNotVaccinated) _.Event).Earbrand == key));
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

            eventEmitter.Received().Emit(Arg.Is<RaisedEvent>(_ => ((DogRegistered) _.Event).Earbrand == key));
        }
    }
}