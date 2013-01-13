namespace Test.Honeycomb
{
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

            string key = "test";

            domain.Apply(new RegisterDog(key, null, null));

            AggregateInfo aggregateInfo = domain.AggregateTracker[typeof (Dog), key];
            ((string) aggregateInfo.Instance.AsDynamic().earbrand).ShouldEqual(key);

            var recorded = domain.TransactionTracker[Transaction.Current].RecordedEvents;
            recorded.Count().ShouldEqual(2);
            recorded.First().EventType.ShouldEqual(typeof(DogRegistered));
            recorded.First().UntypedEvent.ShouldBeType<DogRegistered>();
            recorded.Skip(1).First().EventType.ShouldEqual(typeof(DogRequiresVaccinationWithin12Weeks));
            recorded.Skip(1).First().UntypedEvent.ShouldBeType<DogRequiresVaccinationWithin12Weeks>();
        }

//        [Test]
//        public void events_raised_whilst_receiving_events_should_be_recorded()
//        {
//            var eventStore = Substitute.For<EventStore>();
//            Domain.SetEventStore(eventStore);
//
//            var ts = new TransactionScope();
//
//            var dog = new Dog(new DogRegistered("test", null));
//
//            var resourceManager = AggregateContext.AggregateTracker[dog].ResourceManager;
//            var actual = (List<Event>) resourceManager.AsDynamic().changes;
//            actual.Count().ShouldEqual(1);
//            actual.First().ShouldBeType<DogRequiresVaccinationWithin12Weeks>();
//        }
    }
}