using UnityEngine;
using System.IO;
using System.Collections;
using bookrpg.utils;

namespace bookrpg.net
{
    public interface IMessage
    {
        uint opcode { get; set; }

        void Deserialize(ByteArray stream);

        ByteArray Serialize(ByteArray stream = null, bool skipHead = false);
    }
}
