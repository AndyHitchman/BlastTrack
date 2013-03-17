namespace Honeycomb.Azure.Store
{
    using System;

    public class EventStamp
    {
        private const long TicksPerSecond = 10000000;
        private const int TicksPerMillisecond = 10000;
        private const string minutePad = "0000000000";
        private const string milliPad = "00000";

        public EventStamp(string partionKey, string rowKey)
        {
            TimestampMinute = partionKey;
            TimestampMilliseconds = rowKey;
        }

        public string TimestampMinute { get; private set; }
        public string TimestampMilliseconds { get; private set; }

        public static EventStamp MakeKey(long ticks)
        {
            long partOfMinuteRowKey;
            var oneMinutePartitionKey = Math.DivRem(ticks, TicksPerSecond*60, out partOfMinuteRowKey);
            var milliRowKey = partOfMinuteRowKey/TicksPerMillisecond;
            return new EventStamp(oneMinutePartitionKey.ToString(minutePad), milliRowKey.ToString(milliPad));
        }
    }
}