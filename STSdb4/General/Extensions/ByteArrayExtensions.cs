using STSdb4.General.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STSdb4.General.Extensions
{
    public static class ByteArrayExtensions
    {
        private static readonly string[] hexValues = BitConverter.ToString(Enumerable.Range(0, 256).Select(x => (byte)x).ToArray()).Split('-');

        public static int GetHashCodeEx(this byte[] buffer)
        {
            return GetHashCodeEx((ReadOnlySpan<byte>)buffer);
        }

        public static int GetHashCodeEx(this ReadOnlySpan<byte> buffer)
        {
            const int CONSTANT = 17;
            int hashCode = 37;

            int length = buffer.Length;
            int fullLength = length & ~3;

            for (int offset = 0; offset < fullLength; offset += sizeof(int))
                hashCode = CONSTANT * hashCode + unchecked((int)ByteSpan.ReadUInt32(buffer, offset));

            int remainder = length - fullLength;
            if (remainder > 0)
                hashCode = CONSTANT * hashCode + unchecked((int)ByteSpan.ReadUInt32Partial(buffer, fullLength, remainder));

            return hashCode;
        }

        /// <summary>
        /// http://en.wikipedia.org/wiki/MurmurHash
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static int MurMurHash3(this byte[] buffer, int seed = 37)
        {
            return MurMurHash3((ReadOnlySpan<byte>)buffer, seed);
        }

        public static int MurMurHash3(this ReadOnlySpan<byte> buffer, int seed = 37)
        {
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;
            const int r1 = 15;
            const int r2 = 13;
            const uint m = 5;
            const uint n = 0xe6546b64;

            uint hash = (uint)seed;

            int length = buffer.Length;
            int fullLength = length & ~3;

            for (int offset = 0; offset < fullLength; offset += sizeof(uint))
            {
                uint k = ByteSpan.ReadUInt32(buffer, offset);

                k *= c1;
                k = (k << r1) | (k >> (32 - r1)); //k = rotl32(k, r1);
                k *= c2;

                hash ^= k;
                hash = (hash << r2) | (hash >> (32 - r2)); //hash = rotl32(hash, r2);
                hash = hash * m + n;
            }

            int remainder = length - fullLength;
            if (remainder > 0)
            {
                uint k = ByteSpan.ReadUInt32Partial(buffer, fullLength, remainder);

                k *= c1;
                k = (k << r1) | (k >> (32 - r1)); //k = rotl32(k, r1);
                k *= c2;

                hash ^= k;
            }

            hash ^= (uint)length;

            //hash = fmix(hash);
            hash ^= hash >> 16;
            hash *= 0x85ebca6b;
            hash ^= hash >> 13;
            hash *= 0xc2b2ae35;
            hash ^= hash >> 16;

            return (int)hash;
        }

        public static byte[] Middle(this byte[] buffer, int offset, int length)
        {
            byte[] middle = new byte[length];
            Buffer.BlockCopy(buffer, offset, middle, 0, length);
            return middle;
        }

        public static byte[] Left(this byte[] buffer, int length)
        {
            return buffer.Middle(0, length);
        }

        public static byte[] Right(this byte[] buffer, int length)
        {
            return buffer.Middle(buffer.Length - length, length);
        }

        /// <summary>
        /// Convert byte array to hex string
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string ToHex(this byte[] buffer)
        {
            return ToHex((ReadOnlySpan<byte>)buffer);
        }

        public static string ToHex(this ReadOnlySpan<byte> buffer)
        {
            StringBuilder sb = new StringBuilder(2 * buffer.Length);

            for (int i = 0; i < buffer.Length; i++)
                sb.Append(hexValues[buffer[i]]);

            return sb.ToString();
        }

        public static int GetBit(this byte[] map, int bitIndex)
        {
            return (map[bitIndex >> 3] >> (bitIndex & 7)) & 1;
        }

        public static void SetBit(this byte[] map, int bitIndex, int value)
        {
            int bitMask = 1 << (bitIndex & 7);
            if (value != 0)
                map[bitIndex >> 3] |= (byte)bitMask;
            else
                map[bitIndex >> 3] &= (byte)(~bitMask);
        }
    }
}
 