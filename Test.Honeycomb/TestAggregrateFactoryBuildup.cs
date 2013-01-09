namespace Test.Honeycomb
{
    using System;
    using BlastTrack.Dogs;
    using NSubstitute;
    using NUnit.Framework;
    using ReflectionMagic;
    using Should;
    using global::Honeycomb;

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
                        new UniqueEvent(Guid.NewGuid(), new DogRegistered(aggregateKey), raisedTime)
                    });
            
            var subject = new AggregateFactory<Dog, string>(eventStore);
            
            var actual = subject.Buildup(aggregateKey);

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
                        new UniqueEvent(Guid.NewGuid(), new DogRegistered(aggregateKey), raisedTime),
                        new UniqueEvent(Guid.NewGuid(), new DogNamed(aggregateKey, givenName), raisedTime),
                    });
            
            var subject = new AggregateFactory<Dog, string>(eventStore);
            
            var actual = subject.Buildup(aggregateKey);

            ((string)actual.AsDynamic().name).ShouldEqual(givenName);
        }
    }
}