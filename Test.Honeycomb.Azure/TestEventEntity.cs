namespace Test.Honeycomb.Azure
{
    using System;
    using NUnit.Framework;
    using Should;
    using global::Honeycomb.Azure.Store;

    [TestFixture]
    public class TestEventEntity
    {
        [Test]
        public void min_timestamp_should_resolve_to_partition_key()
        {
            var timestamp = DateTimeOffset.MinValue;
            var key = EventStamp.MakeKey(timestamp.Ticks);
            key.TimestampMinute.ShouldEqual("0000000000");
            key.TimestampMilliseconds.ShouldEqual("00000");
        }

        [Test]
        public void max_timestamp_should_resolve_to_partition_key()
        {
            var timestamp = DateTimeOffset.MaxValue;
            var key = EventStamp.MakeKey(timestamp.Ticks);
            key.TimestampMinute.ShouldEqual("5258964959");
            key.TimestampMilliseconds.ShouldEqual("59999");
        }

        [Test]
        public void timestamp_should_resolve_to_partition_key()
        {
            var timestamp = new DateTimeOffset(2013, 01, 16, 11, 59, 44, 635, TimeZoneInfo.Utc.BaseUtcOffset);
            var key = EventStamp.MakeKey(timestamp.Ticks);
            key.TimestampMinute.ShouldEqual("1058232239");
            key.TimestampMilliseconds.ShouldEqual("44635");
        }

        [Test]
        public void min_row_key_should_be_zero()
        {
            var timestamp = new DateTimeOffset(2013, 01, 16, 11, 59, 00, 000, TimeZoneInfo.Utc.BaseUtcOffset);
            var key = EventStamp.MakeKey(timestamp.Ticks);
            key.TimestampMinute.ShouldEqual("1058232239");
            key.TimestampMilliseconds.ShouldEqual("00000");
        }

        [Test]
        public void max_row_key_should_be_zero()
        {
            var timestamp = new DateTimeOffset(2013, 01, 16, 11, 59, 59, 999, TimeZoneInfo.Utc.BaseUtcOffset);
            var key = EventStamp.MakeKey(timestamp.Ticks);
            key.TimestampMinute.ShouldEqual("1058232239");
            key.TimestampMilliseconds.ShouldEqual("59999");
        }

        [Test]
        public void next_tick_should_move_to_new_partition()
        {
            var timestamp1 = new DateTimeOffset(2013, 01, 16, 11, 59, 59, 999, TimeZoneInfo.Utc.BaseUtcOffset);
            var timestamp2 = new DateTimeOffset(2013, 01, 16, 12, 00, 00, 000, TimeZoneInfo.Utc.BaseUtcOffset);
            var key1 = EventStamp.MakeKey(timestamp1.Ticks);
            var key2 = EventStamp.MakeKey(timestamp2.Ticks);
            key1.TimestampMinute.ShouldEqual("1058232239");
            key1.TimestampMilliseconds.ShouldEqual("59999");
            key2.TimestampMinute.ShouldEqual("1058232240");
            key2.TimestampMilliseconds.ShouldEqual("00000");
        }
    }
}