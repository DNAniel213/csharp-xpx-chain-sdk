﻿using System;

using System.Runtime.Serialization;


namespace ProximaX.Sirius.Chain.Sdk.Model.Exceptions
{
    public class IllegalStateException : Exception
    {
        public IllegalStateException()
        {
        }

        public IllegalStateException(string message)
            : base(message)
        {
        }

        public IllegalStateException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public IllegalStateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}