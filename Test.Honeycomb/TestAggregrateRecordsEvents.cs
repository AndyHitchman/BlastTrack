namespace Test.Honeycomb
{
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
            var domain = new Domain(eventStore);
            var ts = new TransactionScope();

            var key = "test";
            
            domain.Apply(new RegisterDog(key, null, null));

            var resourceManager = domain.Tracked[typeof(Dog), key].ResourceManager;
            var actual = (List<Event>) resourceManager.AsDynamic().changes;
            actual.Count().ShouldEqual(2);
            actual.First().ShouldBeType<DogRegistered>();
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