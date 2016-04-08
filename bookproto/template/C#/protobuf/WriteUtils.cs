using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace bookrpg.net.protobuf
{
    sealed public class WriteUtils
    {
        public WriteUtils()
        {
        }

        private static BinaryWriter writer;

        private static BinaryWriter getWriter(Stream stream)
        {
            if (writer == null || writer.BaseStream != stream) {
                writer = new BinaryWriter(stream);
            }
            return writer;
        }

        #region varint number

        public static void writeTag(Stream stream, WireType wireType, uint number)
        {
            writeVarint(stream, number << 3 | (uint)wireType);

        }

        ///caution：it's ineffective when write negative number, use write_TYPE_SINT32 instead
        public static void write_TYPE_INT32(Stream stream, int value)
        {
            writeVarint(stream, (ulong)value);
        }

        ///caution：it's ineffective when write negative number, use write_TYPE_SINT64 instead
        public static void write_TYPE_INT64(Stream stream, long value)
        {
            writeVarint(stream, (ulong)value);
        }

        public static void write_TYPE_UINT32(Stream stream, uint value)
        {
            writeVarint(stream, value);
        }

        public static void write_TYPE_UINT64(Stream stream, ulong value)
        {
            writeVarint(stream, value);
        }

        public static void write_TYPE_SINT32(Stream stream, int value)
        {
            writeVarint(stream, ZigZag.encode32(value));
        }

        public static void write_TYPE_SINT64(Stream stream, long value)
        {
            writeVarint(stream, ZigZag.encode64(value));
        }

        public static void write_TYPE_BOOL(Stream stream, bool value)
        {
            stream.WriteByte((byte)(value ? 1 : 0));
        }

        public static void write_TYPE_ENUM(Stream stream, int value)
        {
            writeVarint(stream, (ulong)value);
        }

        private static void writeVarint(Stream stream, ulong value)
        {
            byte b;
            while (true)
            {
                b = (byte)(value & 0x7F);
                value = value >> 7;
                if (value == 0)
                {
                    stream.WriteByte(b);
                    break;
                }
                else
                {
                    b |= 0x80;
                    stream.WriteByte(b);
                }
            }
        }

        #endregion


        #region fixed number

        public static void write_TYPE_FIXED32(Stream stream, uint value)
        {
            getWriter(stream).Write(value);
        }

        public static void write_TYPE_FIXED64(Stream stream, ulong value)
        {
            getWriter(stream).Write(value);
        }

        public static void write_TYPE_SFIXED32(Stream stream, int value)
        {
            getWriter(stream).Write(value);
        }
        
        public static void write_TYPE_SFIXED64(Stream stream, long value)
        {
            getWriter(stream).Write(value);
        }

        public static void write_TYPE_FLOAT(Stream stream, float value)
        {
            getWriter(stream).Write(value);
        }

        public static void write_TYPE_DOUBLE(Stream stream, double value)
        {
            getWriter(stream).Write(value);
        }

        #endregion


        #region length delimited

        public static void write_TYPE_BYTES(Stream stream, byte[] value)
        {
            writeVarint(stream, (uint)value.Length);
            stream.Write(value, 0, value.Length);
        }

        public static void write_TYPE_STRING(Stream stream, string value)
        {
            write_TYPE_BYTES(stream, Encoding.UTF8.GetBytes(value));
        }

        public static void write_TYPE_MESSAGE(Stream stream, IMessage value)
        {
            if (value == null)
            {
                return;
            }
            using (var ms = new MemoryStream())
            {
                value.writeTo(ms);
                write_TYPE_BYTES(stream, ms.ToArray());
            }
        }

        public static void writePackedRepeated<T>(
            Stream stream, 
            Action<Stream, T> writeFunction, 
            List<T> value
            )
        {
            using (var ms = new MemoryStream())
            {
                int len = 0;
                while (len < value.Count)
                {
                    writeFunction(ms, value[len++]);
                }
                write_TYPE_BYTES(stream, ms.ToArray());
            }
        }

        #endregion
        
    }
}
