using System;
using System.Buffers.Binary;

namespace STSdb4.General.Buffers
{
    internal static class ByteSpan
    {
        public static int GetUInt64Length(int byteCount)
        {
            return (byteCount + sizeof(ulong) - 1) / sizeof(ulong);
        }

        public static uint ReadUInt32(ReadOnlySpan<byte> buffer, int offset)
        {
            return BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(offset, sizeof(uint)));
        }

        public static uint ReadUInt32Partial(ReadOnlySpan<byte> buffer, int offset, int length)
        {
            uint value = 0;

            for (int i = 0; i < length; i++)
                value |= (uint)buffer[offset + i] << (i * 8);

            return value;
        }

        public static ulong ReadUInt64(ReadOnlySpan<byte> buffer, int offset)
        {
            return BinaryPrimitives.ReadUInt64LittleEndian(buffer.Slice(offset, sizeof(ulong)));
        }

        public static ulong ReadUInt64Partial(ReadOnlySpan<byte> buffer, int offset, int length)
        {
            ulong value = 0;

            for (int i = 0; i < length; i++)
                value |= (ulong)buffer[offset + i] << (i * 8);

            return value;
        }

        public static long ReadInt64(ReadOnlySpan<byte> buffer, int offset)
        {
            return BinaryPrimitives.ReadInt64LittleEndian(buffer.Slice(offset, sizeof(long)));
        }

        public static void WriteInt64(Span<byte> buffer, int offset, long value)
        {
            BinaryPrimitives.WriteInt64LittleEndian(buffer.Slice(offset, sizeof(long)), value);
        }
    }
}
