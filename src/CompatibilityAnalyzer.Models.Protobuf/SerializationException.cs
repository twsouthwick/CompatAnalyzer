using System;

namespace CompatibilityAnalyzer.Models.Protobuf
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
