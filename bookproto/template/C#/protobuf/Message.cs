using System;
using System.IO;
using System.Collections.Generic;

namespace bookrpg.net.protobuf
{
    public class Message : IMessage
    {
    	public int retCode;//0:correct others:wrong, if wrong exit parsing

        public Message()
        {
        }

        public virtual void ParseFrom(Stream input)
        {
            throw new InvalidOperationException("Not implemented.");
        }

        public virtual void WriteTo(Stream output)
        {
            throw new InvalidOperationException("Not implemented.");
        }

        public override string ToString()
        {
            return MessageToString(this);
        }

        private static string MessageToString(IMessage msg)
        {
        	return "";
        }
    }
}
