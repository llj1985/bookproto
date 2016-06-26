using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using bookrpg.net;
using bookrpg.utils;

namespace bookrpg.net.protobuf
{
    sealed public class ReadUtils
    {
        public ReadUtils()
        {
        }

        public static void Skip(ByteArray stream, WireType wireType)
        {
            switch (wireType) {
                case WireType.Varint:
                    while (stream.ReadByte() > 128) {}
                    break;
                case WireType.Fixed32:
                    stream.Seek(4, SeekOrigin.Current);
                    break;
                case WireType.Fixed64:
                    stream.Seek(8, SeekOrigin.Current);
                    break;
                case WireType.LengthDelimited:
                    stream.Seek((int)ReadVarint(stream), SeekOrigin.Current);
                    break;
                default:
                    throw new NotImplementedException("Unknown wire type: " + wireType);
            }
        }

        #region varint number

        public static Tag ReadTag(ByteArray stream)
        {
            var tag = new Tag();
            var n = (uint)ReadVarint(stream);
            tag.wireType = (WireType)(n & 7);
            tag.number = n >> 3;
            return tag;
        }

        /// <summary>
        /// caution：it's ineffective when write negative number, use Read_TYPE_SINT32 instead
        /// </summary>
        public static int Read_TYPE_INT32(ByteArray stream)
        {
            return (int)ReadVarint(stream);
        }

        /// <summary>
        /// caution：it's ineffective when write negative number, use Read_TYPE_SINT64 instead
        /// </summary>
        public static long Read_TYPE_INT64(ByteArray stream)
        {
            return (long)ReadVarint(stream);
        }

        public static uint Read_TYPE_UINT32(ByteArray stream)
        {
            return (uint)ReadVarint(stream);
        }

        public static ulong Read_TYPE_UINT64(ByteArray stream)
        {
            return ReadVarint(stream);
        }

        public static int Read_TYPE_SINT32(ByteArray stream)
        {
            return ZigZag.Decode32((uint)ReadVarint(stream));
        }

        public static long Read_TYPE_SINT64(ByteArray stream)
        {
            return ZigZag.Decode64(ReadVarint(stream));
        }

        public static bool Read_TYPE_BOOL(ByteArray stream)
        {
            int b = stream.ReadByte();
            if (b < 0)
                throw new IOException("stream ended too early");
            if (b == 1)
                return true;
            if (b == 0)
                return false;
            throw new ProtobufException("Invalid boolean value");
        }

        public static int Read_TYPE_ENUM(ByteArray stream)
        {
            return (int)ReadVarint(stream);
        }

        private static ulong ReadVarint(ByteArray stream)
        {
            int b;
            ulong val = 0;

            for (int n = 0; n < 10; n++)
            {
                b = stream.ReadByte();
                if (b < 0)
                    throw new IOException("stream ended too early");

                //Check that it fits in 64 bits
                if ((n == 9) && (b & 0xFE) != 0)
                    throw new ProtobufException("Got larger VarInt than 64 bit unsigned");
                //End of check

                if ((b & 0x80) == 0)
                    return val | (ulong)b << (7 * n);

                val |= (ulong)(b & 0x7F) << (7 * n);
            }

            throw new ProtobufException("Got larger VarInt than 64 bit unsigned");
        }

        #endregion


        #region fixed number

        public static uint Read_TYPE_FIXED32(ByteArray stream)
        {
            return stream.ReadUInt32();
        }

        public static ulong Read_TYPE_FIXED64(ByteArray stream)
        {
            return stream.ReadUInt64();
        }

        public static int Read_TYPE_SFIXED32(ByteArray stream)
        {
            return stream.ReadInt32();
        }

        public static long Read_TYPE_SFIXED64(ByteArray stream)
        {
            return stream.ReadInt64();
        }

        public static float Read_TYPE_FLOAT(ByteArray stream)
        {
            return stream.ReadSingle();
        }

        public static double Read_TYPE_DOUBLE(ByteArray stream)
        {
            return stream.ReadDouble();
        }

        #endregion


        #region length delimited

        public static byte[] Read_TYPE_BYTES(ByteArray stream)
        {
            //VarInt length
            int length = (int)ReadVarint(stream);

            //Bytes
            byte[] buffer = new byte[length];
            int read = 0;
            while (read < length)
            {
                int r = stream.Read(buffer, read, length - read);
                if (r == 0)
                    throw new ProtobufException("Expected " + (length - read) + " got " + read);
                read += r;
            }
            return buffer;
        }

        public static string Read_TYPE_STRING(ByteArray stream)
        {
            var bytes = Read_TYPE_BYTES(stream);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public static IMessage Read_TYPE_MESSAGE(ByteArray stream, IMessage message)
        {
            var bytes = Read_TYPE_BYTES(stream);
            using (var ms = new ByteArray(bytes))
            {
                message.Deserialize(ms);
            }
            return message;
        }

        public static List<T> ReadPackedRepeated<T>(
            ByteArray stream, 
                Func<ByteArray, T> writeFunction, 
                List<T> value
                )
        {
            var bytes = Read_TYPE_BYTES(stream);
            using (var ms = new ByteArray(bytes))
            {
                while (ms.bytesAvailable > 0) {
                    value.Add(writeFunction(ms));
                }
            }
            return value;
        }

        #endregion

    }
}
