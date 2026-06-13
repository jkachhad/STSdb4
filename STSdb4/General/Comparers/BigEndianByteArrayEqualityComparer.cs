using STSdb4.General.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STSdb4.General.Extensions;

namespace STSdb4.General.Comparers
{
    public class BigEndianByteArrayEqualityComparer : IEqualityComparer<byte[]>
    {
        public static readonly BigEndianByteArrayEqualityComparer Instance = new BigEndianByteArrayEqualityComparer();
        
        public bool Equals(byte[] x, byte[] y)
        {
            return Equals((ReadOnlySpan<byte>)x, (ReadOnlySpan<byte>)y);
        }

        public bool Equals(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y)
        {
            if (x.Length != y.Length)
                return false;

            int length = x.Length;
            int fullLength = length & ~7;

            for (int offset = 0; offset < fullLength; offset += sizeof(ulong))
            {
                if (ByteSpan.ReadUInt64(x, offset) != ByteSpan.ReadUInt64(y, offset))
                    return false;
            }

            for (int offset = fullLength; offset < length; offset++)
            {
                if (x[offset] != y[offset])
                    return false;
            }

            return true;
        }

        public int GetHashCode(byte[] obj)
        {
            return obj.GetHashCodeEx();
        }
    }
}
