using STSdb4.General.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STSdb4.General.Extensions;

namespace STSdb4.General.Comparers
{
    public class BigEndianByteArrayComparer : IComparer<byte[]>
    {
        public static readonly BigEndianByteArrayComparer Instance = new BigEndianByteArrayComparer();
        
        public int Compare(byte[] x, byte[] y, int length)
        {
            return Compare((ReadOnlySpan<byte>)x, (ReadOnlySpan<byte>)y, length);
        }

        public int Compare(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y, int length)
        {
            int fullLength = length & ~7;
            for (int offset = 0; offset < fullLength; offset += sizeof(ulong))
            {
                ulong v1 = ByteSpan.ReadUInt64(x, offset);
                ulong v2 = ByteSpan.ReadUInt64(y, offset);

                if (v1 != v2)
                {
                    for (int i = 0; i < sizeof(ulong); i++)
                    {
                        byte b1 = x[offset + i];
                        byte b2 = y[offset + i];
                        if (b1 < b2)
                            return -1;
                        if (b1 > b2)
                            return 1;
                    }
                }
            }

            for (int offset = fullLength; offset < length; offset++)
            {
                byte b1 = x[offset];
                byte b2 = y[offset];
                if (b1 < b2)
                    return -1;
                if (b1 > b2)
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
            int cmp = Compare(x, y, Math.Min(x.Length, y.Length));
            if (cmp != 0)
                return cmp;

            if (x.Length < y.Length)
                return -1;
            if (x.Length > y.Length)
                return 1;

            return 0;
        }
    }
}
