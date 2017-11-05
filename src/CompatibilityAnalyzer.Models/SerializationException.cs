using System;

namespace CompatibilityAnalyzer.Models
{
    public class SerializationException : Exception
    {
        public SerializationException()
            : base()
        {
        }

        public SerializationException(string message)
            : base(message)
        {
        }
    }
}
