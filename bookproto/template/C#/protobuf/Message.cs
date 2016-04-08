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

        public virtual void parseFrom(Stream input)
        {
            throw new InvalidOperationException("Not implemented.");
        }

        public virtual void writeTo(Stream output)
        {
            throw new InvalidOperationException("Not implemented.");
        }

        public override string ToString()
        {
            return messageToString(this);
        }

        private static string messageToString(IMessage msg)
        {
        	return "";
        }
    }
}
