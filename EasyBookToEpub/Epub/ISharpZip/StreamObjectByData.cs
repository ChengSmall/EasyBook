using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Cheng.Algorithm.Compressions.SharpZipLibDLL.EasyBookToEpub
{

    /// <summary>
    /// 简单封装流数据到<see cref="IStaticDataSource"/>
    /// </summary>
    public class StreamObjectByData : IStaticDataSource
    {

        public StreamObjectByData(Stream stream)
        {
            this.stream = stream;
        }

        private readonly Stream stream;

        public Stream GetSource()
        {
            if (this.stream.CanSeek)
            {
                this.stream.Seek(0, SeekOrigin.Begin);
            }
            return new Cheng.Streams.RegulateDisposeStream(this.stream, false);
        }
    }

}
