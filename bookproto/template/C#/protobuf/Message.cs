using UnityEngine;
using System;
using System.Collections;
using System.IO;
using bookrpg.net.protobuf;
using bookrpg.utils;

namespace bookrpg.net
{
    public class Message : IMessage
    {
        #region head

        private const ushort HEAD_LENGTH = 12;

        public uint opcode  { get; set; }

        public ushort route1;

        public ushort route2;

        public uint flag;

        #endregion

        public Message()
        {
        }

        public virtual void Deserialize(ByteArray stream)
        {
            ushort headLength = stream.ReadUInt16();
            var pos = stream.position;
            opcode = stream.ReadUInt32();
            route1 = stream.ReadUInt16();
            route2 = stream.ReadUInt16();
            flag = stream.ReadUInt32();
            stream.position = pos + headLength;
        }

        public virtual ByteArray Serialize(ByteArray stream = null, bool skipHead = false)
        {
            if (stream == null)
            {
                stream = new ByteArray();
            }
            if (!skipHead)
            {
                stream.Write(HEAD_LENGTH);
                stream.Write(opcode);
                stream.Write(route1);
                stream.Write(route2);
                stream.Write(flag);
            }
            return stream;
        }
    }
}
