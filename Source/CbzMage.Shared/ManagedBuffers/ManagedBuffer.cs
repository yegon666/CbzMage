﻿namespace CbzMage.Shared.ManagedBuffers
{
    public sealed class ManagedBuffer
    {
        internal static BufferCache Cache { get; set; }

        private readonly int _originalLength;

        public byte[] Buffer { get; private set; }

        public int Count { get; private set; }

        public ManagedBuffer()
        {
            Buffer = Cache.Get();

            _originalLength = Buffer.Length;
        }

        public ManagedBuffer(ManagedBuffer startWith, int offset, int length) : this()
        {
            System.Buffer.BlockCopy(startWith.Buffer, offset, Buffer, 0, length);

            Count = length;
        }

        public int ReadFrom(FileStream stream)
        {
            return ReadFrom(stream, useSpan: true);
        }

        public int ReadFrom(Stream stream, bool useSpan = false)
        {
            var remaining = Buffer.Length - Count;
            var readCount = useSpan ? stream.Read(Buffer.AsSpan(Count, remaining)) : stream.Read(Buffer, Count, remaining);

            if (readCount > 0)
            {
                Count += readCount;

                if (remaining < Cache.BufferRemainingThreshold)
                {
                    var newBuffer = Cache.Get(Buffer.Length + _originalLength);

                    System.Buffer.BlockCopy(Buffer, 0, newBuffer, 0, Count);

                    Buffer = newBuffer;
                }
            }

            return readCount;
        }

        public void Release()
        {
            if (Cache != null)
            {
                Cache.Release(Buffer);
            }
            else
            {
                Buffer = null;
            }
        }

        public bool StartsWith(int offset, int count, byte[] pattern)
        {
            if (count < pattern.Length)
            {
                return false;
            }

            for (int i = 0, sz = pattern.Length; i < sz; i++)
            {
                if (Buffer[offset + i] != pattern[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
