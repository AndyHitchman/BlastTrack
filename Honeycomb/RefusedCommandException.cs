namespace Honeycomb
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class RefusedCommandException : Exception
    {
        //TODO: Should probably be a more complex structure linking to the command property (and therefore the form input) if possible.

        public RefusedCommandException()
        {
            Reasons = new List<string>();
        }

        public RefusedCommandException(string reason) : this()
        {
            Reasons.Add(reason);
        }

        public List<string> Reasons { get; private set; }
    }
}