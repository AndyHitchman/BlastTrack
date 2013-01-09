//namespace Honeycomb
//{
//    using System;
//
//    public abstract class Receipt
//    {
//        protected Receipt()
//        {
//            CausingCorrelationId = Guid.NewGuid();
//        }
//
//        public Guid CausingCorrelationId { get; private set; }
//    }
//
//    public class AcceptedReceipt : Receipt
//    {
//        public AcceptedReceipt(Event consequentialEvent)
//        {
//            ConsequentialEvent = consequentialEvent;
//        }
//
//        public Event ConsequentialEvent { get; private set; }
//    }
//}