using System;
using System.Collections.Generic;

namespace bookrpg.net.protobuf
{
    public class ProtobufException : Exception
    {
        public ProtobufException(string message) : base(message)
        {
        }
    }
}

