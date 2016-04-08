using System;

namespace bookrpg.net.protobuf
{
    public class ZigZag
    {
        public static uint encode32(int n)
        {
            return (uint)(n << 1 ^ n >> 31);
        }

        public static int decode32(uint n)
        {
            return (int)(n >> 1) ^ -(int)(n & 1);
        } 

        public static ulong encode64(long n)
        {
            return (ulong)(n << 1 ^ n >> 63);
        }

        public static long decode64(ulong n)
        {
            return (long)(n >> 1) ^ -(long)(n & 1);
        }

        /*
        public static uint encode64low(uint low, int high)
        {
            return (uint)(low << 1 ^ high >> 31);
        }

        public static int encode64high(uint low, int high)
        {
            return (int)(low >> 31 ^ high << 1 ^ high >> 31);
        }

        public static uint decode64low(uint low, int high)
        {
            return (uint)(high << 31 ^ low >> 1 ^ -(low & 1));
        }

        public static int decode64high(uint low, int high)
        {
            return (int)(high >> 1) ^ -(int)(low & 1);
        }
        */

    }
}
