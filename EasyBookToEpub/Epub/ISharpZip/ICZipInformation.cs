using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Cheng.Algorithm.Compressions.SharpZipLibDLL
{

    /// <summary>
    /// 封装<see cref="ICSharpCode.SharpZipLib.Zip.ZipEntry"/>对象信息
    /// </summary>
    public class ICZipInformation : DataInformation
    {

        /// <summary>
        /// 将一个entry封装到信息对象
        /// </summary>
        /// <param name="entry">项数据</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public ICZipInformation(ZipEntry entry)
        {
            this.entry = entry ?? throw new ArgumentNullException();
            try
            {
                p_name = Path.GetFileName(this.entry.Name);
            }
            catch (Exception)
            {
                p_name = null;
            }
            
        }

        internal readonly ZipEntry entry;

        private string p_name;

        public override string DataPath => entry.Name;

        public override string DataName
        {
            get
            {
                return p_name;
            }
        }

        public override long BeforeSize
        {
            get
            {
                return entry.Size;
            }
        }

        public override long CompressedSize
        {
            get
            {
                return entry.CompressedSize;
            }
        }

        public override DateTime? ModifiedTime
        {
            get
            {
                return entry.DateTime;
            }
        }

        public override string ToString()
        {
            return p_name;
        }

    }

}
