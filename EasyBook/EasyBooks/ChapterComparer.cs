using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.EasyBooks
{

    /// <summary>
    /// 章节名排序
    /// </summary>
    public sealed class ChapterComparer : Comparer<string>
    {
        public ChapterComparer()
        {
        }

        /// <summary>
        /// 去后缀的章节文件比较
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int Compare(string x, string y)
        {
            return Comparer(x, y);
        }

        /// <summary>
        /// 去后缀的章节文件比较
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int Comparer(string x, string y)
        {
            if (x is null || y is null)
            {
                if ((object)x == (object)y) return 0;
                return x is null ? -1 : 1;
            }

            if (int.TryParse(x, out int left) && int.TryParse(y, out int right))
            {
                return left < right ? -1 : (left == right ? 0 : 1);
            }
            return string.CompareOrdinal(x, y);
            //int xLen = x.Length;
            //int yLen = y.Length;
            //int length = Math.Min(xLen, yLen);
            //for (int i = 0; i < length; i++)
            //{
            //    if (x[i] != y[i])
            //    {
            //        return x[i] - y[i];
            //    }
            //}
            //return xLen < yLen ? -1 : (xLen == yLen ? 0 : 1);
        }

    }

    /// <summary>
    /// 章节索引easybook排序
    /// </summary>
    public sealed class ChapterIndexComparer : Comparer<ChapterIndex>
    {

        public ChapterIndexComparer()
        {
        }

        public sealed override int Compare(ChapterIndex x, ChapterIndex y)
        {
            var nx = Path.GetFileNameWithoutExtension(x.chapterFileName);
            var ny = Path.GetFileNameWithoutExtension(y.chapterFileName);

            return ChapterComparer.Comparer(nx, ny);

        }
    }

}
