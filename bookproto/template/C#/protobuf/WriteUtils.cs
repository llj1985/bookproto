using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using bookrpg.net;
using bookrpg.utils;

namespace bookrpg.net.protobuf
{
    sealed public class WriteUtils
    {
        public WriteUtils()
        {
        }

        #region varint number

        public static void WriteTag(ByteArray stream, WireType wireType, uint number)
        {
            WriteVarint(stream, number << 3 | (uint)wireType);

        }

        ///caution：it's ineffective when write negative number, use Write_TYPE_SINT32 instead
        public static void Write_TYPE_INT32(ByteArray stream, int value)
        {
            WriteVarint(stream, (ulong)value);
        }

        ///caution：it's ineffective when write negative number, use Write_TYPE_SINT64 instead
        public static void Write_TYPE_INT64(ByteArray stream, long value)
        {
            WriteVarint(stream, (ulong)value);
        }

        public static void Write_TYPE_UINT32(ByteArray stream, uint value)
        {
            WriteVarint(stream, value);
        }

        public static void Write_TYPE_UINT64(ByteArray stream, ulong value)
        {
            WriteVarint(stream, value);
        }

        public static void Write_TYPE_SINT32(ByteArray stream, int value)
        {
            WriteVarint(stream, ZigZag.Encode32(value));
        }

        public static void Write_TYPE_SINT64(ByteArray stream, long value)
        {
            WriteVarint(stream, ZigZag.Encode64(value));
        }

        public static void Write_TYPE_BOOL(ByteArray stream, bool value)
        {
            stream.Write((byte)(value ? 1 : 0));
        }

        public static void Write_TYPE_ENUM(ByteArray stream, int value)
        {
            WriteVarint(stream, (ulong)value);
        }

        private static void WriteVarint(ByteArray stream, ulong value)
        {
            byte b;
            while (true)
            {
                b = (byte)(value & 0x7F);
                value = value >> 7;
                if (value == 0)
                {
                    stream.Write(b);
                    break;
                }
                else
                {
                    b |= 0x80;
                    stream.Write(b);
                }
            }
        }

        #endregion


        #region fixed number

        public static void Write_TYPE_FIXED32(ByteArray stream, uint value)
        {
            stream.Write(value);
        }

        public static void Write_TYPE_FIXED64(ByteArray stream, ulong value)
        {
            stream.Write(value);
        }

        public static void Write_TYPE_SFIXED32(ByteArray stream, int value)
        {
            stream.Write(value);
        }
        
        public static void Write_TYPE_SFIXED64(ByteArray stream, long value)
        {
            stream.Write(value);
        }

        public static void Write_TYPE_FLOAT(ByteArray stream, float value)
        {
            stream.Write(value);
        }

        public static void Write_TYPE_DOUBLE(ByteArray stream, double value)
        {
            stream.Write(value);
        }

        #endregion


        #region length delimited

        public static void Write_TYPE_BYTES(ByteArray stream, byte[] value)
        {
            WriteVarint(stream, (uint)value.Length);
            stream.Write(value, 0, value.Length);
        }

        public static void Write_TYPE_STRING(ByteArray stream, string value)
        {
            Write_TYPE_BYTES(stream, Encoding.UTF8.GetBytes(value));
        }

        public static void Write_TYPE_MESSAGE(ByteArray stream, IMessage value)
        {
            if (value == null)
            {
                return;
            }
            using (var ms = new ByteArray())
            {
                value.Serialize(ms, true);
                Write_TYPE_BYTES(stream, ms.ToArray());
            }
        }

        public static void WritePackedRepeated<T>(
            ByteArray stream, 
            Action<ByteArray, T> writeFunction, 
            List<T> value
            )
        {
            using (var ms = new ByteArray())
            {
                int len = 0;
                while (len < value.Count)
                {
                    writeFunction(ms, value[len++]);
                }
                Write_TYPE_BYTES(stream, ms.ToArray());
            }
        }

        #endregion
        
    }
}
