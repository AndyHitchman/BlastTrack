namespace Honeycomb.Azure
{
    using System;
    using System.Threading;

    public static class Retry
    {
        public delegate int Standoff(int attemptNumber);

        private static readonly Random variableStandoff = new Random();

        public static void Work(Action doWork, Func<Exception, bool> retryExceptions)
        {
            var tryCount = 0;
            do
            {
                tryCount += 1;

                try
                {
                    doWork();

                    //If we get here, all is good.
                    return;
                }
                catch (Exception e)
                {
                    if (tryCount > 10) throw;
                    if (!retryExceptions(e)) throw;

                    Thread.Sleep(standoff(tryCount));
                }
            } while (true);
        }

        /// <summary>
        ///   Standoff by trying again in 5..10s, 8..18s, 11..26s, etc, successively.
        /// </summary>
        /// <value> Number of previous tries </value>
        private static int standoff(int tryCount)
        {
            return 2000 + (variableStandoff.Next(3000, 8000)*tryCount);
        }
    }
}