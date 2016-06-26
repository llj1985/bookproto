using System;

namespace bookrpg.utils
{
    /// <summary>
    /// 字节序转换
    /// </summary>
    public static class EndianSwap
    {
        public static short SwapInt16(short value)
        {
            return (short)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        public static ushort SwapUInt16(ushort value)
        {
            return (ushort)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        public static int SwapInt32(int value)
        {
            return (int)((value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24);
        }

        public static uint SwapUInt32(uint value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }

        public static long SwapInt64(long value)
        {
            return (long)SwapUInt64((ulong)value);
        }

        public static ulong SwapUInt64(ulong value)
        {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }

        public static int FloatToInt32Bits(float value)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }

        public static float Int32BitsToFloat(int value)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
        }

        public static long DoubleToInt64Bits(double value)
        {
            return BitConverter.DoubleToInt64Bits(value);
        }

        public static double Int64BitsToDouble(long value)
        {
            return BitConverter.Int64BitsToDouble(value);
        }
    }
}