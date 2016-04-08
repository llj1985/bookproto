using System;
using System.IO;

namespace bookrpg.net.protobuf
{
	public interface IMessage
	{
	    void parseFrom(Stream input);

	    void writeTo(Stream output);
	}
}
