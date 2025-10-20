using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

using ICZipFile = ICSharpCode.SharpZipLib.Zip.ZipFile;

using Cheng.Algorithm.Compressions;
using Cheng.Algorithm;
using Cheng.Algorithm.Collections;
using Cheng.DataStructure.Collections;
using Cheng.Algorithm.Trees;
using ICSharpCode.SharpZipLib.Zip;
using Cheng.Memorys;
using Cheng.Texts;
using Cheng.Streams;

namespace Cheng.Algorithm.Compressions.SharpZipLibDLL.EasyBookToEpub
{

    /// <summary>
    /// 对<see cref="ICSharpCode.SharpZipLib.Zip.ZipFile"/>进行封装的只读Zip数据解析器
    /// </summary>
    public sealed class ICZipReadOnlyCompressionParser : BaseCompressionParser
    {

        #region 构造

        const int defBufferSize = 1024 * 8;

        /// <summary>
        /// 实例化一个只读zip包
        /// </summary>
        /// <param name="stream">要进行解析的zip数据</param>
        /// <exception cref="ArgumentNullException">参数对象是null</exception>
        /// <exception cref="NotSupportedException">流没有读取或查找功能</exception>
        /// <exception cref="InsufficientMemoryException">内存不足，无法获取所有数据索引</exception>
        public ICZipReadOnlyCompressionParser(Stream stream) : this(stream, true, defBufferSize)
        {
        }

        /// <summary>
        /// 实例化一个只读zip包
        /// </summary>
        /// <param name="stream">要进行解析的zip数据</param>
        /// <param name="onDispose">在释放时是否释放封装的流对象，true表示释放对象，false表示不释放<paramref name="stream"/>；默认为true</param>
        /// <exception cref="ArgumentNullException">参数对象是null</exception>
        /// <exception cref="NotSupportedException">流没有读取或查找功能</exception>
        /// <exception cref="InsufficientMemoryException">内存不足，无法获取所有数据索引</exception>
        public ICZipReadOnlyCompressionParser(Stream stream, bool onDispose) : this(stream, onDispose, defBufferSize)
        {
        }

        /// <summary>
        /// 实例化一个只读zip包
        /// </summary>
        /// <param name="stream">要进行解析的zip数据</param>
        /// <param name="onDispose">在释放时是否释放封装的流对象，true表示释放对象，false表示不释放<paramref name="stream"/>；默认为true</param>
        /// <param name="bufferSize">解压并拷贝时的缓冲区大小，不可小于或等于0；默认值为8192</param>
        /// <exception cref="ArgumentNullException">参数对象是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区小于或等于0</exception>
        /// <exception cref="NotSupportedException">流没有读取或查找功能</exception>
        /// <exception cref="InsufficientMemoryException">内存不足，无法获取所有数据索引</exception>
        public ICZipReadOnlyCompressionParser(Stream stream, bool onDispose, int bufferSize) : this(stream, onDispose, bufferSize, StringCodec.Default)
        {
        }

        /// <summary>
        /// 实例化一个只读zip包
        /// </summary>
        /// <param name="stream">要进行解析的zip数据</param>
        /// <param name="onDispose">在释放时是否释放封装的流对象，true表示释放对象，false表示不释放<paramref name="stream"/>；默认为true</param>
        /// <param name="bufferSize">解压并拷贝时的缓冲区大小，不可小于或等于0；默认值为8192</param>
        /// <param name="encoding">用于解析读取和写入字符串编码</param>
        /// <exception cref="ArgumentNullException">参数对象是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区小于或等于0</exception>
        /// <exception cref="NotSupportedException">流没有读取或查找功能</exception>
        /// <exception cref="InsufficientMemoryException">内存不足，无法获取所有数据索引</exception>
        public ICZipReadOnlyCompressionParser(Stream stream, bool onDispose, int bufferSize, Encoding encoding) : this(stream, onDispose, bufferSize, StringCodec.FromEncoding(encoding ?? throw new ArgumentNullException(nameof(encoding))))
        {
        }

        /// <summary>
        /// 实例化一个只读zip包
        /// </summary>
        /// <param name="stream">要进行解析的zip数据</param>
        /// <param name="onDispose">在释放时是否释放封装的流对象，true表示释放对象，false表示不释放<paramref name="stream"/>；默认为true</param>
        /// <param name="bufferSize">解压并拷贝时的缓冲区大小，不可小于或等于0；默认值为8192</param>
        /// <param name="codec">用于解析读取和写入字符串编码的对象</param>
        /// <exception cref="ArgumentNullException">参数对象是null</exception>
        /// <exception cref="ArgumentOutOfRangeException">缓冲区小于或等于0</exception>
        /// <exception cref="NotSupportedException">流没有读取或查找功能</exception>
        /// <exception cref="InsufficientMemoryException">内存不足，无法获取所有数据索引</exception>
        public ICZipReadOnlyCompressionParser(Stream stream, bool onDispose, int bufferSize, StringCodec codec)
        {
            if (stream is null) throw new ArgumentNullException();
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException();
            if (!(stream.CanRead && stream.CanSeek))
            {
                throw new NotSupportedException();
            }

            p_zipFile = new ICZipFile(stream, true, codec);
            try
            {
                if (p_zipFile.Count > int.MaxValue) throw new InsufficientMemoryException();
                p_buffer = new byte[bufferSize];
                f_init();
            }
            catch (Exception)
            {
                p_zipFile.Close();
                throw;
            }

            p_zipFile.IsStreamOwner = onDispose;
        }

        private void f_init()
        {
            int length = (int)p_zipFile.Count;
            p_list = new List<ICZipInformation>(length);
            p_dict = new Dictionary<string, ICZipInformation>(length, new EqualityStrNotPathSeparator(false, false));
            for (int i = 0; i < length; i++)
            {
                var entry = p_zipFile[i];
                if (entry.IsFile)
                {
                    var fentry = new ICZipInformation(entry);
                    p_list.Add(fentry);
                    p_dict.Add(getWpath(entry.Name), fentry);
                }
            }
        }

        #endregion

        #region 参数

        private ICZipFile p_zipFile;
        //private Stream p_stream;
        private List<ICZipInformation> p_list;
        private Dictionary<string, ICZipInformation> p_dict;
        private byte[] p_buffer;

        #endregion

        #region 功能

        #region

        static string getWpath(string path)
        {
            return path?.Replace('/', '\\');
        }

        #endregion

        #region 访问信息

        public override bool CanIndexOf => true;

        public override int Count
        {
            get
            {
                ThrowObjectDisposeException();
                return p_list.Count;
            }
        }

        public override DataInformation this[int index]
        {
            get
            {
                ThrowObjectDisposeException();
                return p_list[index];
            }
        }

        public override DataInformation this[string dataPath]
        {
            get
            {
                ThrowObjectDisposeException();
                return p_dict[dataPath];
            }
        }

        public override bool CanGetEnumerator => true;

        public override IEnumerable<string> EnumatorFilePath()
        {
            return p_list.ToOtherItems(toFunc);

            string toFunc(ICZipInformation t_inf)
            {
                return t_inf.DataPath;
            }
        }

        public override IEnumerable<string> EnumatorFileName()
        {
            return p_list.ToOtherItems(toFunc);

            string toFunc(ICZipInformation t_inf)
            {
                return t_inf.DataName;
            }
        }

        public override IEnumerator<DataInformation> GetEnumerator()
        {
            return p_list.GetEnumerator();
        }

        public override bool CanProbePath => true;
        public override bool CanGetEntryEnumrator => true;

        public override IEnumerator<IDataEntry> GetDataEntryEnumrator()
        {
            return p_list.GetEnumerator();
        }

        public override T GetInformation<T>(int index)
        {
            return p_list[index] as T;
        }

        public override T GetInformation<T>(string dataPath)
        {
            return this[dataPath] as T;
        }

        public override bool TryGetInformation<T>(string dataPath, out T information)
        {
            information = null;
            if(p_dict.TryGetValue(dataPath, out var obj))
            {
                information = obj as T;
                return true;
            }
            return false;
        }

        public override bool ContainsData(string dataPath)
        {
            return p_dict.ContainsKey(dataPath);
        }

        /// <summary>
        /// 获取指定索引下的项信息
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns><paramref name="index"/>索引下的信息</returns>
        /// <exception cref="ArgumentOutOfRangeException">索引超出范围</exception>
        public ICZipInformation GetZipInformation(int index)
        {
            return p_list[index];
        }

        /// <summary>
        /// 获取指定路径索引下的项信息
        /// </summary>
        /// <param name="dataPath">路径索引</param>
        /// <returns><paramref name="dataPath"/>下的信息</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="KeyNotFoundException">路径不正确</exception>
        public ICZipInformation GetZipInformation(string dataPath)
        {
            return p_dict[dataPath];
        }

        #endregion

        #region 解压缩到流

        public override bool CanBeCompressed => true;

        public override bool CanDeCompression => true;

        public override bool CanDeCompressionByIndex => true;

        public override bool CanDeCompressionByPath => true;

        public override void DeCompressionTo(int index, Stream stream)
        {
            ThrowObjectDisposeException();
            lock (p_buffer)
            {
                using (var open = p_zipFile.GetInputStream(p_list[index].entry))
                {
                    open.CopyToStream(stream, p_buffer);
                }
            }
        }

        public override void DeCompressionTo(string dataPath, Stream stream)
        {
            ThrowObjectDisposeException();
            
            var entry = p_dict.TryGetValue(dataPath, out var inf) ? inf.entry : null;
            if (entry is null || stream is null) throw new ArgumentNullException();
            lock (p_buffer)
            {
                using (var open = p_zipFile.GetInputStream(entry))
                {
                    open.CopyToStream(stream, p_buffer);
                }
            }
        }

        public override byte[] DeCompressionToData(int index)
        {
            ThrowObjectDisposeException();
            lock (p_buffer)
            {
                var entry = p_list[index].entry;
                if (entry.Size > int.MaxValue) throw new InsufficientMemoryException();
                byte[] re;
                using (var open = p_zipFile.GetInputStream(entry))
                {
                    re = new byte[entry.Size];
                    open.ReadBlock(re, 0, re.Length);
                }
                return re;
            }
           
        }

        public override byte[] DeCompressionToData(string dataPath)
        {
            ThrowObjectDisposeException();
            var entry = p_dict.TryGetValue(dataPath, out var inf) ? inf.entry : null;
            if (entry.Size > int.MaxValue) throw new InsufficientMemoryException();
            byte[] re;
            lock (p_buffer)
            {
                using (var open = p_zipFile.GetInputStream(entry))
                {
                    re = new byte[entry.Size];
                    open.ReadBlock(re, 0, re.Length);
                }
            }
            return re;
        }

        public override void DeCompressionToText(int index, Encoding encoding, TextWriter writer)
        {
            ThrowObjectDisposeException();
            lock (p_buffer)
            {
                //var entry = p_dict.TryGetValue(dataPath, out var inf) ? inf.entry : null;
                using (StreamReader sr = new StreamReader(p_zipFile.GetInputStream(p_list[index].entry), encoding))
                {
                    sr.CopyToText(writer);
                }
            }
        }

        public override void DeCompressionToText(string dataPath, Encoding encoding, TextWriter writer)
        {
            ThrowObjectDisposeException();
            var entry = p_dict.TryGetValue(dataPath, out var inf) ? inf.entry : null;
            if (entry is null || writer is null) throw new ArgumentNullException();
            lock (p_buffer)
            {
                using (StreamReader sr = new StreamReader(p_zipFile.GetInputStream(entry), encoding))
                {
                    sr.CopyToText(writer);
                }
            }
        }

        #endregion

        #region 打开流

        public override bool CanOpenCompressedStreamByIndex => true;

        public override bool CanOpenCompressedStreamByPath => true;

        public override Stream OpenCompressedStream(int index)
        {
            ThrowObjectDisposeException();
            var inf = p_list[index];
            return p_zipFile.GetInputStream(inf.entry);
        }

        public override Stream OpenCompressedStream(string dataPath)
        {
            ThrowObjectDisposeException();
            if(p_dict.TryGetValue(dataPath, out var inf))
            {
                return p_zipFile.GetInputStream(inf.entry);
            }
            return null;
        }

        public override Stream CreateOrOpenStream(string dataPath)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 获取内部对象
        /// </summary>
        /// <returns></returns>
        public ICZipFile GetBaseZipFile()
        {
            return p_zipFile;
        }

        #endregion

        #region 其它派生

        public override bool CanSortInformationIndex => true;

        public override void SortInformationIndex(IComparer<DataInformation> comparer, int index, int count)
        {
            p_list.Sort(index, count, comparer);
        }

        public override void SortInformationIndex(IComparer comparer, int index, int count)
        {
            p_list.Sort(index, count, comparer);
        }

        public override void SortInformationIndex(IComparer<DataInformation> comparer)
        {
            p_list.Sort(comparer);
        }

        public override bool IsNeedToReleaseResources => true;

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing)
            {
                p_zipFile.Close();
            }
            p_zipFile = null;
            p_list = null;
            return true;
        }

        #endregion

        #region 加密

        public override bool CanSetPassword => true;

        /// <summary>
        /// 用于解密文件的密码
        /// </summary>
        /// <value>解密文件的密码，null或空字符串表示没有设置密码</value>
        public override string Password 
        {
            get => throw new NotSupportedException();
            set
            {
                p_zipFile.Password = value;
            }
        }

        #endregion

        #endregion

    }

}
#if DEBUG

#endif