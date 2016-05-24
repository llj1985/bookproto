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

        private static BinaryReader GetReader(Stream stream)
        {
            if (reader == null || reader.BaseStream != stream) {
                reader = new BinaryReader(stream);
            }
            return reader;
        }

        public static void Skip(Stream stream, WireType wireType)
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

        public static Tag ReadTag(Stream stream)
        {
            var tag = new Tag();
            var n = (uint)ReadVarint(stream);
            tag.wireType = (WireType)(n & 7);
            tag.number = n >> 3;
            return tag;
        }

        public static int Read_TYPE_INT32(Stream stream)
        {
            return (int)ReadVarint(stream);
        }

        public static long Read_TYPE_INT64(Stream stream)
        {
            return (long)ReadVarint(stream);
        }

        public static uint Read_TYPE_UINT32(Stream stream)
        {
            return (uint)ReadVarint(stream);
        }

        public static ulong Read_TYPE_UINT64(Stream stream)
        {
            return ReadVarint(stream);
        }

        public static int Read_TYPE_SINT32(Stream stream)
        {
            return ZigZag.Decode32((uint)ReadVarint(stream));
        }

        public static long Read_TYPE_SINT64(Stream stream)
        {
            return ZigZag.Decode64(ReadVarint(stream));
        }

        public static bool Read_TYPE_BOOL(Stream stream)
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

        public static int Read_TYPE_ENUM(Stream stream)
        {
            return (int)ReadVarint(stream);
        }

        private static ulong ReadVarint(Stream stream)
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

        public static uint Read_TYPE_FIXED32(Stream stream)
        {
            return GetReader(stream).ReadUInt32();
        }

        public static ulong Read_TYPE_FIXED64(Stream stream)
        {
            return GetReader(stream).ReadUInt64();
        }

        public static int Read_TYPE_SFIXED32(Stream stream)
        {
            return GetReader(stream).ReadInt32();
        }

        public static long Read_TYPE_SFIXED64(Stream stream)
        {
            return GetReader(stream).ReadInt64();
        }

        public static float Read_TYPE_FLOAT(Stream stream)
        {
            return GetReader(stream).ReadSingle();
        }

        public static double Read_TYPE_DOUBLE(Stream stream)
        {
            return GetReader(stream).ReadDouble();
        }

        #endregion


        #region length delimited

        public static byte[] Read_TYPE_BYTES(Stream stream)
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

        public static string Read_TYPE_STRING(Stream stream)
        {
            var bytes = Read_TYPE_BYTES(stream);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public static IMessage Read_TYPE_MESSAGE(Stream stream, IMessage message)
        {
            var bytes = Read_TYPE_BYTES(stream);
            using (var ms = new MemoryStream(bytes))
            {
                message.ParseFrom(ms);
            }
            return message;
        }

        public static List<T> ReadPackedRepeated<T>(
            Stream stream, 
                Func<Stream, T> writeFunction, 
                List<T> value
                )
        {
            var bytes = Read_TYPE_BYTES(stream);
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
