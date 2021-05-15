using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineTetris.Socket
{
    public class CircularArray
    {
        private byte[] _buffer;
        private byte[] _getStringBuffer;
        private int _size;
        private int _beginIndex;
        private int _endIndex;

        private int EndIndex
        {
            get => _endIndex;
            set => _endIndex = value % _size;
        }

        public int Length => (EndIndex + _size - _beginIndex) % _size;
        public bool Empty => Length == 0;
        public bool Any => !Empty;

        public CircularArray(int size)
        {
            _buffer = new byte[size];
            _getStringBuffer = new byte[size];
            _size = size;
        }

        public void Write(byte[] data)
        {
            Write(data, 0, data.Length);
        }

        public void Write(byte[] data, int offset, int count)
        {
            for (var i = offset; i < offset + count; i++)
            {
                _buffer[EndIndex] = data[i];
                EndIndex++;
            }
        }

        public int Read(byte[] arr)
        {
            return Read(arr, 0, arr.Length);
        }

        public int Read(byte[] arr, int offset, int count)
        {
            var readCount = 0;
            for (var i = offset; Any && i < offset + count; i++)
            {
                arr[i] = _buffer[_beginIndex + readCount];
                readCount++;
            }
            _beginIndex = (_beginIndex + readCount) % _size;
            return readCount;
        }

        public string ReadString(int length)
        {
            if (_beginIndex + length <= _size)
            {
                var str = Encoding.UTF8.GetString(_buffer, _beginIndex, length);
                _beginIndex += length;
                return str;
            }
            else
            {
                var length1 = _size - _beginIndex;
                var source1 = new Memory<byte>(_buffer, _beginIndex, length1);
                var target1 = new Memory<byte>(_getStringBuffer, 0, length1);
                source1.CopyTo(target1);

                var length2 = length - length1;
                var source2 = new Memory<byte>(_buffer, 0, length2);
                var target2 = new Memory<byte>(_getStringBuffer, length1, length2);
                source2.CopyTo(target2);
                _beginIndex = (_beginIndex + length) % _size;

                var str = Encoding.UTF8.GetString(_getStringBuffer, 0, length);
                return str;
            }
        }

        public Memory<byte> GetWritableMemory()
        {
            if (_beginIndex <= EndIndex)
            {
                // 0    begin    end    size
                return new Memory<byte>(_buffer, EndIndex, _size - EndIndex);
            }
            else
            {
                // 0    end        begin   size
                return new Memory<byte>(_buffer, EndIndex, _beginIndex - EndIndex);
            }
        }

        public void ShiftEndIndex(int count)
        {
            EndIndex += count;
        }
    }
}
