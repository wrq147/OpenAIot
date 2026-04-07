using System;
using System.Collections;


namespace ChannelUtility.Buffers
{
    public class FastReader : IEnumerable, IByteArray
    {
        protected bool _mergeRead = false;
        public bool IsMergeRead
        {
            get
            {
                return _mergeRead;
            }
        }
        /// <summary>
        /// 标识为数据未完整
        /// </summary>
        public void MergeRead()
        {
            _mergeRead = true;
            Reset();
        }
        /// <summary>
        /// Create the instance from a byte array source
        /// </summary>
        /// <param name="source"></param>
        public FastReader(byte[] source)
        {
            _buffer = source.ToArray();
            Length = _buffer.Length;
            Reset();
        }

        public FastReader CopyTo(int start)
        {
            if (_buffer.Length <= start)
            {
                return null;
            }
            int newlen = _buffer.Length - start;
            var bytesNew = new byte[newlen];
            Buffer.BlockCopy(_buffer, start, bytesNew, 0, newlen);
            return new FastReader(bytesNew);

        }
        public FastReader Contact(FastReader reader)
        {
            var bytesNew = new byte[_buffer.Length + reader.Length];
            Buffer.BlockCopy(_buffer, 0, bytesNew, 0, _buffer.Length);
            Buffer.BlockCopy(reader._buffer, 0, bytesNew, _buffer.Length, reader.Length);
            return new FastReader(bytesNew);
        }
        public FastReader Contact(byte[] input)
        {
            var bytesNew = new byte[_buffer.Length + input.Length];
            Buffer.BlockCopy(_buffer, 0, bytesNew, 0, _buffer.Length);
            Buffer.BlockCopy(input, 0, bytesNew, _buffer.Length, input.Length);
            return new FastReader(bytesNew);
        }
        /// <summary>
        /// Create the instance from the specified part of the byte array source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public FastReader(
            byte[] source,
            int offset,
            int count)
        {
            _buffer = source
                .ToArray(offset, count);
            Length = count;
            Reset();
        }



        private readonly byte[] _buffer;

        public int Position { get; private set; }
        public int Length { get; private set; }



        /// <summary>
        /// Allow to reset the reader pointer
        /// </summary>
        public void Reset()
        {
            Position = -1;
            _bitPosition = -1;
        }



        /// <summary>
        /// Convert the reader content to a normal byte array
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return _buffer
                .ToArray();
        }



        /// <summary>
        /// Indicate whether the pointer has reached the end of the internal buffer
        /// </summary>
        public bool EndOfBuffer
        {
            get { return Position >= Length - 1; }
        }

        /// <summary>
        /// Facility for data exchange
        /// </summary>
        public byte[] Data
        {
            get { return _buffer; }
        }



        /// <summary>
        /// Tell whether the specified amount of bytes could be read from the reader
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool CanRead(int count)
        {
            int len = Length - Position - 1;
            return (count <= len);
        }



        /// <summary>
        /// Read the current pointed byte, but without moving the pointer
        /// </summary>
        /// <returns></returns>
        public byte Peek()
        {
            return _buffer[Position];
        }
        public byte Peek(int pos)
        {
            return _buffer[pos];
        }



        /// <summary>
        /// Read the current byte and move the pointer accordingly
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            this.NextBitPosition();
            return _buffer[++Position];
        }



        /// <summary>
        /// Try to read a byte, if available, and return true if succeeded
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryReadByte(out byte value)
        {
            if (CanRead(1))
            {
                value = ReadByte();
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public void Skip(int count)
        {
            int pos = Math.Min(this.Length, this.Position + count);
            this.Position = pos;
        }

        /// <summary>
        /// Read the specified amount of bytes, 
        /// and move the pointer accordingly
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public byte[] ReadBytes(int count)
        {
            this.NextBitPosition();
            var collection = new byte[count];

            for (int i = 0; i < count; i++)
            {
                collection[i] = _buffer[++Position];
            }

            return collection;
        }



        /// <summary>
        /// Read the remaining bytes up to the end, 
        /// and move the pointer accordingly
        /// </summary>
        /// <returns></returns>
        public byte[] ReadToEnd()
        {
            return ReadBytes(Length - (Position + 1));
        }



        /// <summary>
        /// Read an <see cref="System.Int16"/> (Little-endian),
        /// and move the pointer accordingly
        /// </summary>
        /// <returns></returns>
        public Int16 ReadInt16LE()
        {
            this.NextBitPosition();
            int ptr = Position + 1;
            Position += 2;
            return FastBufferHelper.ReadInt16LE(
                _buffer,
                ptr);
        }



        /// <summary>
        /// Read an <see cref="System.Int16"/> (Big-endian),
        /// and move the pointer accordingly
        /// </summary>
        /// <returns></returns>
        public Int16 ReadInt16BE()
        {
            this.NextBitPosition();
            int ptr = Position + 1;
            Position += 2;
            return FastBufferHelper.ReadInt16BE(
                _buffer,
                ptr);
        }



        /// <summary>
        /// Read an <see cref="System.UInt16"/> (Little-endian),
        /// and move the pointer accordingly
        /// </summary>
        /// <returns></returns>
        public UInt16 ReadUInt16LE()
        {
            this.NextBitPosition();
            int ptr = Position + 1;
            Position += 2;
            return FastBufferHelper.ReadUInt16LE(
                _buffer,
                ptr);
        }



        /// <summary>
        /// Read an <see cref="System.UInt16"/> (Big-endian),
        /// and move the pointer accordingly
        /// </summary>
        /// <returns></returns>
        public UInt16 ReadUInt16BE()
        {
            this.NextBitPosition();
            int ptr = Position + 1;
            Position += 2;
            return FastBufferHelper.ReadUInt16BE(
                _buffer,
                ptr);
        }



        /// <summary>
        /// Read an <see cref="System.Int32"/> (Little-endian),
        /// and move the pointer accordingly
        /// </summary>
        /// <returns></returns>
        public Int32 ReadInt32LE()
        {
            this.NextBitPosition();
            int ptr = Position + 1;
            Position += 4;
            return FastBufferHelper.ReadInt32LE(
                _buffer,
                ptr);
        }



        /// <summary>
        /// Read an <see cref="System.Int32"/> (Big-endian),
        /// and move the pointer accordingly
        /// </summary>
        /// <returns></returns>
        public Int32 ReadInt32BE()
        {
            this.NextBitPosition();
            int ptr = Position + 1;
            Position += 4;
            return FastBufferHelper.ReadInt32BE(
                _buffer,
                ptr);
        }

        public Int32 ReadInt32CDAB()
        {
            this.NextBitPosition();
            int ptr = Position + 1;
            Position += 4;
            return FastBufferHelper.ReadInt32CDAB(
                _buffer,
                ptr);
        }

        public Int32 ReadInt32BADC()
        {
            this.NextBitPosition();
            int ptr = Position + 1;
            Position += 4;
            return FastBufferHelper.ReadInt32BADC(
                _buffer,
                ptr);
        }

        /// <summary>
        /// 读取 64 位有符号整数（小端）
        /// </summary>
        public Int64 ReadInt64LE()
        {
            this.NextBitPosition();
            int ptr = Position + 1;
            Position += 8;
            return FastBufferHelper.ReadInt64LE(_buffer, ptr);
        }

        /// <summary>
        /// 读取 64 位有符号整数（大端）
        /// </summary>
        public Int64 ReadInt64BE()
        {
            this.NextBitPosition();
            int ptr = Position + 1;
            Position += 8;
            return FastBufferHelper.ReadInt64BE(_buffer, ptr);
        }
     
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _buffer
                .GetEnumerator();
        }

        // 跟踪当前字节内的位位置（0-7，-1表示未开始读取位）
        private int _bitPosition = -1;

        public void NextBitPosition()
        {
            if (_bitPosition != -1)
            {
                ++Position;
            }
            _bitPosition = -1;
        }

        /// <summary>
        /// 检查是否可以读取指定数量的位
        /// </summary>
        /// <param name="bitCount">需要读取的位数</param>
        /// <returns>是否可读取</returns>
        public bool CanReadBits(int bitCount)
        {
            if (bitCount <= 0) return true;

            // 下一个要操作的字节索引（根据Position语义）
            int nextByteIndex = Position + 1;

            // 计算当前可用字节中剩余的位数
            int remainingBits;
            if (_bitPosition == -1)
            {
                // 未开始读取位：下一个字节完整8位可用（如果存在）
                remainingBits = nextByteIndex < Length ? 8 : 0;
            }
            else
            {
                // 已开始读取位：当前操作的是nextByteIndex（未读完），剩余位为7 - _bitPosition
                remainingBits = nextByteIndex < Length ? (7 - _bitPosition) : 0;
            }

            // 当前字节剩余位足够
            if (remainingBits >= bitCount)
                return true;

            // 计算还需要的字节数
            int needMoreBits = bitCount - remainingBits;
            int needMoreBytes = (needMoreBits + 7) / 8; // 向上取整

            // 检查总字节是否足够（nextByteIndex + 需要的字节数 ≤ 总长度）
            return nextByteIndex + needMoreBytes <= Length;
        }

        /// <summary>
        /// 读取1位（0或1），并移动位指针（自动跨字节）
        /// </summary>
        public int ReadBit()
        {
            if (!CanReadBits(1))
                throw new InvalidOperationException("没有足够的位可读取");

            // 下一个要操作的字节索引
            int currentByteIndex = Position + 1;

            if (_bitPosition == -1)
            {
                // 首次读取位：从当前字节（nextByteIndex）的最高位（7）开始
                _bitPosition = 0;
                return (_buffer[currentByteIndex] >> 7) & 1;
            }

            if (_bitPosition >= 7)
            {
                // 当前字节的位已读完，切换到下一个字节
                Position++; // 字节指针推进（下一个字节变为Position+1）
                currentByteIndex = Position + 1; // 更新当前操作的字节索引
                _bitPosition = 0;
                return (_buffer[currentByteIndex] >> 7) & 1;
            }

            // 读取当前字节的下一位（从高位到低位：7→0）
            _bitPosition++;
            return (_buffer[currentByteIndex] >> (7 - _bitPosition)) & 1;
        }

        /// <summary>
        /// 尝试读取1位，成功则返回true
        /// </summary>
        public bool TryReadBit(out int bitValue)
        {
            if (CanReadBits(1))
            {
                bitValue = ReadBit();
                return true;
            }
            bitValue = 0;
            return false;
        }

        public int ReadBitLE()
        {
            if (!CanReadBits(1))
                throw new InvalidOperationException("没有足够的位可读取");

            // 下一个要操作的字节索引
            int currentByteIndex = Position + 1;

            if (_bitPosition == -1)
            {
                _bitPosition = 0;
                return _buffer[currentByteIndex] & 1;
            }

            if (_bitPosition >= 7)
            {
                Position++;
                currentByteIndex = Position + 1;
                _bitPosition = 0;
                return _buffer[currentByteIndex] & 1;
            }

            _bitPosition++;
            return (_buffer[currentByteIndex] >> _bitPosition) & 1;
        }

        /// <summary>
        /// 读取指定数量的位（1-32），返回整数（高位在前）
        /// </summary>
        public int ReadBits(int bitCount)
        {
            if (bitCount < 1 || bitCount > 32)
                throw new ArgumentOutOfRangeException(nameof(bitCount), "位数必须在1-32之间");
            if (!CanReadBits(bitCount))
                throw new InvalidOperationException("没有足够的位可读取");

            int result = 0;
            for (int i = 0; i < bitCount; i++)
            {
                result = (result << 1) | ReadBit();
            }
            return result;
        }

        /// <summary>
        /// 尝试读取指定数量的位，成功则返回true
        /// </summary>
        public bool TryReadBits(int bitCount, out int value)
        {
            value = 0;
            if (bitCount < 1 || bitCount > 32)
                return false;
            if (!CanReadBits(bitCount))
                return false;

            value = ReadBits(bitCount);
            return true;
        }

    }
}
