using System;
using System.Collections.Generic;

namespace bookrpg.net.protobuf
{
	public enum WireType
    {
        Varint = 0,          //int32, int64, UInt32, UInt64, SInt32, SInt64, bool, enum
        Fixed64 = 1,         //fixed64, sfixed64, double
        LengthDelimited = 2, //string, bytes, embedded messages, packed repeated fields
        //Start = 3,         //groups (deprecated)
        //End = 4,           //groups (deprecated)
        Fixed32 = 5,         //fixed32, SFixed32, float
    }

	sealed public class Tag
	{
	    public uint number;
	    public WireType wireType;
	}
}
