namespace Honeycomb.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading;

    public static class Recover
    {
        public delegate int Standoff(int attemptNumber);
        private static readonly Random variableStandoff = new Random();

        public static void RetryTransaction(int attemptsAllowed, Standoff standoff, Func<Exception, bool> retry, Action doWork, Action<Exception> outOfRetries)
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
                    if (tryCount > attemptsAllowed) throw;
                    if (!retry(e)) throw;

                    Thread.Sleep(standoff(tryCount));
                }
            } while (true);
        }

        /// <summary>
        /// Standoff by trying again in 5..10s, 8..18s, 11..26s, etc, successively. 
        /// </summary>
        /// <value>Number of previous tries</value>
        public static int VariableStandoff(int tryCount)
        {
            return 2000 + (variableStandoff.Next(3000, 8000) * tryCount);
        }
    }
}