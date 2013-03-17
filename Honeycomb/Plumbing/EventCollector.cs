namespace Honeycomb.Plumbing
{
    using System;

    public interface EventCollector
    {
        /// <summary>
        ///   Instructs the collector to begin serving the provided propogation domain.
        /// </summary>
        /// <param name="propogationDomain"> </param>
        void StartCollectingEvents(Domain propogationDomain);

        /// <summary>
        ///   Instructs the collector to stop collecting events.
        ///   The collector will call
        ///   <param name="stopped"> </param>
        ///   when has stopped collecting.
        /// </summary>
        void StopCollecting(Action stopped);
    }
}