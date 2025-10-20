using Cheng.Streams;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cheng.DataStructure.SharpZipLibDLL.EasyBookToEpub
{

    /// <summary>
    /// 封装<see cref="IGettingStream"/>到<see cref="IStaticDataSource"/>
    /// </summary>
    public sealed class GettingStreamByStaticDataSource : IStaticDataSource
    {

        #region streamOnLength

        private class StreamLen : HEStream
        {
            public StreamLen(Stream stream, long length)
            {
                this.length = length;
                this.stream = stream;
            }

            public long length;
            private readonly Stream stream;

            public override bool CanRead => stream.CanRead;

            public override bool CanSeek => stream.CanSeek;

            public override bool CanTimeout => stream.CanTimeout;

            public override bool CanWrite => false;

            public override long Length => length;

            public override long Position
            {
                get => stream.Position;
                set => stream.Position = value;
            }

            public override int ReadTimeout
            {
                get => stream.ReadTimeout;
                set => stream.ReadTimeout = value;
            }

            public override int WriteTimeout
            {
                get => stream.WriteTimeout;
                set => stream.WriteTimeout = value;
            }

            public override string ToString()
            {
                return stream.ToString();
            }

            public override void Flush()
            {
                stream.Flush();
            }

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                return stream.ReadAsync(buffer, offset, count, cancellationToken);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return stream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return stream.Read(buffer, offset, count);
            }

            public override int ReadByte()
            {
                return stream.ReadByte();
            }

            protected override bool Disposing(bool disposing)
            {
                if (disposing)
                {
                    stream.Close();
                }
                return true;
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            public override void WriteByte(byte value)
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        /// <summary>
        /// 封装<see cref="IGettingStream"/>到<see cref="IStaticDataSource"/>
        /// </summary>
        /// <param name="gettingStream"></param>
        public GettingStreamByStaticDataSource(IGettingStream gettingStream)
        {
            p_get = gettingStream ?? throw new ArgumentNullException();
        }

        private IGettingStream p_get;

        public Stream GetSource()
        {
            var s = p_get.OpenStream();
            if (s is null) throw new ArgumentNullException();

            if (s.CanSeek && s.CanRead)
            {
                return s;
            }

            var len = p_get.StreamLength;
            return len < 0 ? s : new StreamLen(s, len);

        }

    }

    /// <summary>
    /// 封装<see cref="IStaticDataSource"/>到<see cref="IGettingStream"/>
    /// </summary>
    public sealed class DataSourceByGettingStream : IGettingStream
    {

        /// <summary>
        /// 封装<see cref="IStaticDataSource"/>到<see cref="IGettingStream"/>
        /// </summary>
        /// <param name="dataSource"></param>
        public DataSourceByGettingStream(IStaticDataSource dataSource)
        {
            p_data = dataSource ?? throw new ArgumentNullException();
        }

        private IStaticDataSource p_data;

        public long StreamLength => -1;

        public Stream OpenStream()
        {
            return p_data.GetSource();
        }
    }


}
