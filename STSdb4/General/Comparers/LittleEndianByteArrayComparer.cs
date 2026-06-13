using STSdb4.General.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STSdb4.General.Extensions;

namespace STSdb4.General.Comparers
{
    public class LittleEndianByteArrayComparer : IComparer<byte[]>
    {
        public static readonly LittleEndianByteArrayComparer Instance = new LittleEndianByteArrayComparer();
        
        public int Compare(byte[] x, byte[] y, int length)
        {
            return Compare((ReadOnlySpan<byte>)x, (ReadOnlySpan<byte>)y, length);
        }

        public int Compare(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y, int length)
        {
            int remainder = length & 7;
            int offset = length - remainder;

            if (remainder > 0)
            {
                ulong v1 = ByteSpan.ReadUInt64Partial(x, offset, remainder);
                ulong v2 = ByteSpan.ReadUInt64Partial(y, offset, remainder);
                if (v1 < v2)
                    return -1;
                if (v1 > v2)
                    return 1;
            }

            for (offset -= sizeof(ulong); offset >= 0; offset -= sizeof(ulong))
            {
                ulong v1 = ByteSpan.ReadUInt64(x, offset);
                ulong v2 = ByteSpan.ReadUInt64(y, offset);
                if (v1 < v2)
                    return -1;
                if (v1 > v2)
                    return 1;
            }

            return 0;
        }

        public int Compare(byte[] x, byte[] y)
        {
            return Compare((ReadOnlySpan<byte>)x, (ReadOnlySpan<byte>)y);
        }

        public int Compare(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y)
        {
            if (x.Length == y.Length)
                return Compare(x, y, x.Length);

            for (int i = x.Length - 1, j = y.Length - 1, len = Math.Min(x.Length, y.Length); len > 0; i--, j--, len--)
            {
                if (x[i] < y[j])
                    return -1;
                if (x[i] > y[j])
                    return 1;
            }

            if (x.Length < y.Length)
                return -1;
            if (x.Length > y.Length)
                return 1;

            return 0;
        }
    }
}
