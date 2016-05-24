using System;

namespace bookrpg.net.protobuf
{
    public class ZigZag
    {
        public static uint Encode32(int n)
        {
            return (uint)(n << 1 ^ n >> 31);
        }

        public static int Decode32(uint n)
        {
            return (int)(n >> 1) ^ -(int)(n & 1);
        } 

        public static ulong Encode64(long n)
        {
            return (ulong)(n << 1 ^ n >> 63);
        }

        public static long Decode64(ulong n)
        {
            return (long)(n >> 1) ^ -(long)(n & 1);
        }

        /*
        public static uint Encode64low(uint low, int high)
        {
            return (uint)(low << 1 ^ high >> 31);
        }

        public static int Encode64high(uint low, int high)
        {
            return (int)(low >> 31 ^ high << 1 ^ high >> 31);
        }

        public static uint Decode64low(uint low, int high)
        {
            return (uint)(high << 31 ^ low >> 1 ^ -(low & 1));
        }

        public static int Decode64high(uint low, int high)
        {
            return (int)(high >> 1) ^ -(int)(low & 1);
        }
        */

    }
}
