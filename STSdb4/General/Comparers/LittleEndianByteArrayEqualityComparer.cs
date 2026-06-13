using STSdb4.General.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STSdb4.General.Extensions;
using System.Runtime.InteropServices;

namespace STSdb4.General.Comparers
{
    public class LittleEndianByteArrayEqualityComparer : IEqualityComparer<byte[]>
    {
        public static readonly LittleEndianByteArrayEqualityComparer Instance = new LittleEndianByteArrayEqualityComparer();
        
        public bool Equals(byte[] x, byte[] y)
        {
            return Equals((ReadOnlySpan<byte>)x, (ReadOnlySpan<byte>)y);
        }

        public bool Equals(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y)
        {
            if (x.Length != y.Length)
                return false;

            int length = x.Length;
            int remainder = length & 7;
            int fullLength = length - remainder;

            for (int offset = 0; offset < fullLength; offset += sizeof(ulong))
            {
                if (ByteSpan.ReadUInt64(x, offset) != ByteSpan.ReadUInt64(y, offset))
                    return false;
            }

            if (remainder > 0)
            {
                if (ByteSpan.ReadUInt64Partial(x, fullLength, remainder) != ByteSpan.ReadUInt64Partial(y, fullLength, remainder))
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
