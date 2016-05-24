using System;
using System.IO;

namespace bookrpg.net.protobuf
{
	public interface IMessage
	{
	    void ParseFrom(Stream input);

	    void WriteTo(Stream output);
	}
}
