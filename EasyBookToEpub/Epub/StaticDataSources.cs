using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cheng.Streams;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Threading;

namespace Cheng.DataStructure.SharpZipLibDLL
{

    /// <summary>
    /// 从byte[]中创建数组
    /// </summary>
    public sealed class BytesDataSource : IStaticDataSource
    {
        /// <summary>
        /// 使用字节数组初始化
        /// </summary>
        /// <param name="streamData"></param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public BytesDataSource(byte[] streamData)
        {
            data = streamData ?? throw new ArgumentNullException();
        }

        private readonly byte[] data;

        public Stream GetSource()
        {
            return new MemoryStream(data);
        }
    }

    /// <summary>
    /// 封装一个可以创建流对象的委托到<see cref="IStaticDataSource"/>
    /// </summary>
    public sealed class FuncCreateDataSource : IStaticDataSource
    {

        /// <summary>
        /// 实例化一个委托创建流封装
        /// </summary>
        /// <param name="createStreamFunc"></param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public FuncCreateDataSource(Func<Stream> createStreamFunc)
        {
            p_func = createStreamFunc ?? throw new ArgumentNullException();
        }

        private readonly Func<Stream> p_func;

        public Stream GetSource()
        {
            return p_func.Invoke();
        }
    }

}
