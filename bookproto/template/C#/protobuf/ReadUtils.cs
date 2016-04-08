using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace bookrpg.net.protobuf
{
    sealed public class ReadUtils
    {
        public ReadUtils()
        {
        }

        private static BinaryReader reader;

        private static BinaryReader getReader(Stream stream)
        {
            if (reader == null || reader.BaseStream != stream) {
                reader = new BinaryReader(stream);
            }
            return reader;
        }

        public static void skip(Stream stream, WireType wireType)
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
                    stream.Seek((int)readVarint(stream), SeekOrigin.Current);
                    break;
                default:
                    throw new NotImplementedException("Unknown wire type: " + wireType);
            }
        }

        #region varint number

        public static Tag readTag(Stream stream)
        {
            var tag = new Tag();
            var n = (uint)readVarint(stream);
            tag.wireType = (WireType)(n & 7);
            tag.number = n >> 3;
            return tag;
        }

        public static int read_TYPE_INT32(Stream stream)
        {
            return (int)readVarint(stream);
        }

        public static long read_TYPE_INT64(Stream stream)
        {
            return (long)readVarint(stream);
        }

        public static uint read_TYPE_UINT32(Stream stream)
        {
            return (uint)readVarint(stream);
        }

        public static ulong read_TYPE_UINT64(Stream stream)
        {
            return readVarint(stream);
        }

        public static int read_TYPE_SINT32(Stream stream)
        {
            return ZigZag.decode32((uint)readVarint(stream));
        }

        public static long read_TYPE_SINT64(Stream stream)
        {
            return ZigZag.decode64(readVarint(stream));
        }

        public static bool read_TYPE_BOOL(Stream stream)
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

        public static int read_TYPE_ENUM(Stream stream)
        {
            return (int)readVarint(stream);
        }

        private static ulong readVarint(Stream stream)
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

        public static uint read_TYPE_FIXED32(Stream stream)
        {
            return getReader(stream).ReadUInt32();
        }

        public static ulong read_TYPE_FIXED64(Stream stream)
        {
            return getReader(stream).ReadUInt64();
        }

        public static int read_TYPE_SFIXED32(Stream stream)
        {
            return getReader(stream).ReadInt32();
        }

        public static long read_TYPE_SFIXED64(Stream stream)
        {
            return getReader(stream).ReadInt64();
        }

        public static float read_TYPE_FLOAT(Stream stream)
        {
            return getReader(stream).ReadSingle();
        }

        public static double read_TYPE_DOUBLE(Stream stream)
        {
            return getReader(stream).ReadDouble();
        }

        #endregion


        #region length delimited

        public static byte[] read_TYPE_BYTES(Stream stream)
        {
            //VarInt length
            int length = (int)readVarint(stream);

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

        public static string read_TYPE_STRING(Stream stream)
        {
            var bytes = read_TYPE_BYTES(stream);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public static IMessage read_TYPE_MESSAGE(Stream stream, IMessage message)
        {
            var bytes = read_TYPE_BYTES(stream);
            using (var ms = new MemoryStream(bytes))
            {
                message.parseFrom(ms);
            }
            return message;
        }

        public static List<T> readPackedRepeated<T>(
            Stream stream, 
                Func<Stream, T> writeFunction, 
                List<T> value
                )
        {
            var bytes = read_TYPE_BYTES(stream);
            using (var ms = new MemoryStream(bytes))
            {
                while (ms.Position < ms.Length) {
                    value.Add(writeFunction(ms));
                }
            }
            return value;
        }

        #endregion

    }
}
