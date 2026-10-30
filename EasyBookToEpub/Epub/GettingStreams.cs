using System;
using System.IO;
using System.Text;
using System.Xml;

using Cheng.DataStructure;
using Cheng.Streams;

namespace Cheng.EasyBooks.ToEpubs
{

    #region MyRegion

    /// <summary>
    /// 从xml创建读取的数据
    /// </summary>
    public sealed class GettingXmlDocumentStream : IGettingStream
    {

        /// <summary>
        /// 实例化xml数据读取
        /// </summary>
        /// <param name="xml"></param>
        public GettingXmlDocumentStream(XmlDocument xml)
        {
            this.xml = xml;
        }

        private XmlDocument xml;

        public Stream OpenStream()
        {
            try
            {
                MemoryStream ms = new MemoryStream(1024 * 4);
                using (StreamWriter swr = new StreamWriter(ms, Encoding.UTF8, 1024 * 2, true))
                {
                    xml.Save(swr);
                }
                ms.Seek(0, SeekOrigin.Begin);
                return new ReadOnlyStream(ms, true);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public OpenStreamIOSourceType SourceType => OpenStreamIOSourceType.RuntimeBytes;

        public long StreamLength => -1;
    }

    /// <summary>
    /// 从easybook创建一个xml的函数
    /// </summary>
    /// <param name="easyBook">ebk对象</param>
    /// <returns>创建的xml</returns>
    /// <exception cref="ArgumentNullException">参数是null</exception>
    public delegate XmlDocument CreateXmlFromEasyBook(EasyBook easyBook);

    /// <summary>
    /// 从easybook创建xml再创建Stream的流获取器
    /// </summary>
    public sealed class GettingXmlDocumentStreamFromEasyBook : IGettingStream
    {

        /// <summary>
        /// 实例化流对象创建器
        /// </summary>
        /// <param name="createFunc">从ebk创建xml的函数</param>
        /// <param name="easyBook">ebk数据</param>
        public GettingXmlDocumentStreamFromEasyBook(CreateXmlFromEasyBook createFunc, EasyBook easyBook)
        {
            if (createFunc is null || easyBook is null) throw new ArgumentNullException();
            p_ct = createFunc; p_ebk = easyBook;
        }

        private CreateXmlFromEasyBook p_ct;
        private EasyBook p_ebk;

        public Stream OpenStream()
        {
            try
            {
                var xml = p_ct.Invoke(p_ebk);
                MemoryStream ms = new MemoryStream(1024 * 2);
                using (StreamWriter swr = new StreamWriter(ms, Encoding.UTF8, 1024 * 2, true))
                {
                    xml.Save(swr);
                }
                ms.Seek(0, SeekOrigin.Begin);
                return new ReadOnlyStream(ms, true);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public OpenStreamIOSourceType SourceType => OpenStreamIOSourceType.RuntimeBytes;

        public long StreamLength => -1;
    }

    #endregion

}
