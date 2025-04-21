using Cheng.Memorys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.EasyBooks
{

    /// <summary>
    /// 章节行类型
    /// </summary>
    public enum ChapterLineType : byte
    {

        /// <summary>
        /// 表示空白行
        /// </summary>
        Empty = 0,

        /// <summary>
        /// 一行文本
        /// </summary>
        Text,

        /// <summary>
        /// 图片插入行
        /// </summary>
        Image

    }

    /// <summary>
    /// 表示章节的一行内容
    /// </summary>
    public struct ChapterLine
    {

        #region

        /// <summary>
        /// 初始化一个图像插入行
        /// </summary>
        /// <param name="path">图像路径</param>
        /// <param name="customImageScale">是否采用自定义图像长宽</param>
        /// <param name="width">长度缩放比</param>
        /// <param name="height">宽度缩放比</param>
        public ChapterLine(string path, bool customImageScale, float width, float height)
        {
            this.type = ChapterLineType.Image;
            this.value = path;
            this.width = width;
            this.height = height;
            this.customImageScale = customImageScale;
        }

        /// <summary>
        /// 初始化一个文本行
        /// </summary>
        /// <param name="value">文本</param>
        public ChapterLine(string value)
        {
            this.type = ChapterLineType.Text;
            this.value = value;
            this.width = default;
            this.height = default;
            customImageScale = false;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 行数据
        /// </summary>
        /// <remarks>
        /// <para>如果行类型是<see cref="ChapterLineType.Text"/>，则该参数表示文本；如果是<see cref="ChapterLineType.Image"/>，该参数表示图像文件所在于image目录的相对路径；<see cref="ChapterLineType.Empty"/>则是无效数据</para>
        /// </remarks>
        public string value;

        /// <summary>
        /// 图像插入行的缩放宽度比
        /// </summary>
        /// <remarks>仅在行类型是<see cref="ChapterLineType.Image"/>，参数<see cref="customImageScale"/>为true时有效</remarks>
        public float width;

        /// <summary>
        /// 图像插入行的缩放高度比
        /// </summary>
        /// <remarks>仅在行类型是<see cref="ChapterLineType.Image"/>，参数<see cref="customImageScale"/>为true时有效</remarks>
        public float height;

        /// <summary>
        /// 是否拥有图像自定义缩放
        /// </summary>
        public bool customImageScale;

        /// <summary>
        /// 此行的行类型
        /// </summary>
        public ChapterLineType type;

        #endregion

        #region

        /// <summary>
        /// 表示一个空行
        /// </summary>
        public static ChapterLine EmptyLine => new ChapterLine();

        /// <summary>
        /// 返回当前行数据
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return value ?? string.Empty;
        }

        static bool isNewLine(char c, string newLine)
        {
            for (int i = 0; i < newLine.Length; i++)
            {
                if (newLine[i] == c)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断此行文本是否为easybook章节行的空行
        /// </summary>
        /// <param name="text">判断文本</param>
        /// <returns>是空行返回true，不是空行返回false</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static bool IsEmptyLine(string text)
        {
            if (text is null) throw new ArgumentNullException();
            var newL = Environment.NewLine;
            int length = text.Length;
            if (length == 0) return true;
            for (int i = 0; i < length; i++)
            {
                var c = text[i];
                if (c != ' ' && c != '\t' && (!isNewLine(c, newL)))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 判断此文本的章节行类型并返回对应的章节行数据
        /// </summary>
        /// <param name="text">一行文本</param>
        /// <returns>章节行数据</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">文本不属于章节行语法内容</exception>
        public static ChapterLine TextToChapterLine(string text)
        {
            if (text is null) throw new ArgumentNullException();

            if (IsEmptyLine(text)) return ChapterLine.EmptyLine;

            var re = text.IndexOf('\r');
            if (re != -1) throw new ArgumentException(Cheng.Properties.Resources.Exception_ArgsFormatError);

            if (text.Length > 3)
            {

                if (text[0] == '>' && text[1] == '>' && text[2] == '>')
                {

                    if (text[3] == '>')
                    {
                        //忽略第一个字符
                        return new ChapterLine(text.Substring(1));
                    }

                    /*
                    最少12个字符
                    >>>image:"s"
                    */

                    if (text.Length < 12 || text[9] != '"')
                    {
                        throw new ArgumentException(Cheng.Properties.Resources.Exception_ArgsFormatError);
                    }
                    if (text.Substring(0, 9) != ">>>image:")
                    {
                        throw new ArgumentException(Cheng.Properties.Resources.Exception_ArgsFormatError);
                    }

                    //第二个双引号
                    var endY = text.IndexOf('"', 10);

                    if (endY < 0 || endY == 12)
                    {
                        //没后引号或路径是空
                        throw new ArgumentException(Cheng.Properties.Resources.Exception_ArgsFormatError);
                    }
                    //路径长度
                    var pathLen = (endY - 10);
                    //路径
                    var path = text.Substring(10, pathLen);
                    //":[0,0]
                    if (path.Length < 5)
                    {
                        //图像没有规定格式
                        throw new ArgumentException(Cheng.Properties.Resources.Exception_ArgsFormatError);
                    }

                    //路径后一位
                    int nextY = endY + 1;

                    if (text.Length == nextY)
                    {
                        //无自定义长宽比
                        return new ChapterLine(path, false, 0, 0);
                    }

                    if (text[nextY] != ':')
                    {
                        //没冒号(￣▽￣)"
                        throw new ArgumentException(Cheng.Properties.Resources.Exception_ArgsFormatError);
                    }
                    nextY++;
                    if (text.Length == nextY)
                    {
                        throw new ArgumentException(Cheng.Properties.Resources.Exception_ArgsFormatError);
                    }
                    //从左括号开始 切下
                    var nextStr = text.Substring(nextY);
                    if (nextStr.Length < 5)
                    {
                        //缺少必备字符数量
                        throw new ArgumentException(Cheng.Properties.Resources.Exception_ArgsFormatError);

                    }
                    // :[0,0]

                    var fen = nextStr.IndexOf(',');

                    if (fen == -1)
                    {
                        //无分隔符
                        throw new ArgumentException(Cheng.Properties.Resources.Exception_ArgsFormatError);
                    }

                    //左侧width文本
                    var widthStr = nextStr.Substring(1, (fen - 1));

                    //右侧height起始索引
                    var rightIndex = fen + 1;
                    if (rightIndex + 1 == nextStr.Length)
                    {
                        //字符数不够
                        throw new ArgumentException(Cheng.Properties.Resources.Exception_ArgsFormatError);
                    }
                    var rightK = nextStr.IndexOf(']');
                    if (rightK == -1)
                    {
                        throw new ArgumentException(Cheng.Properties.Resources.Exception_ArgsFormatError);
                    }

                    var heightStr = nextStr.Substring(rightIndex, rightK - rightIndex);

                    if (float.TryParse(widthStr, out float widthF) && float.TryParse(heightStr, out float heightF))
                    {
                        return new ChapterLine(path, true, widthF, heightF);
                    }

                    if (double.TryParse(widthStr, out var widthD) && double.TryParse(heightStr, out var heightD))
                    {
                        return new ChapterLine(path, true, (float)widthD, (float)heightD);
                    }

                }

            }

            return new ChapterLine(text);

        }

        #endregion

    }

    /// <summary>
    /// 章节头
    /// </summary>
    public struct ChapterIndex
    {

        /// <summary>
        /// 初始化章节头
        /// </summary>
        /// <param name="title">章节标题</param>
        /// <param name="fileName">章节文件名</param>
        /// <param name="filePath">章节所在路径</param>
        public ChapterIndex(string title, string fileName, string filePath)
        {
            this.title = title;
            this.chapterFileName = fileName;
            this.chapterFilePath = filePath;
        }

        /// <summary>
        /// 初始化章节头
        /// </summary>
        /// <param name="title">章节标题</param>
        /// <param name="filePath">章节所在路径</param>
        public ChapterIndex(string title, string filePath)
        {
            this.title = title;
            this.chapterFilePath = filePath;
            this.chapterFileName = Path.GetFileName(filePath);
        }

        /// <summary>
        /// 章节标题
        /// </summary>
        public string title;

        /// <summary>
        /// 章节文件名
        /// </summary>
        public string chapterFileName;

        /// <summary>
        /// 章节文件所在路径
        /// </summary>
        public string chapterFilePath;

    }

    /// <summary>
    /// easybook章节
    /// </summary>
    public sealed class Chapter
    {

        #region 构造

        /// <summary>
        /// 实例化一个easybook章节实例
        /// </summary>
        public Chapter()
        {
            Title = "";
            p_lines = new List<ChapterLine>();
        }

        /// <summary>
        /// 使用章节名初始化一个easybook章节实例
        /// </summary>
        /// <param name="title">章节标题</param>
        public Chapter(string title)
        {
            p_title = title ?? string.Empty;
            p_lines = new List<ChapterLine>();
        }

        #endregion

        #region 参数

        private string p_title;

        private List<ChapterLine> p_lines;

        #endregion

        #region 功能

        #region 参数访问

        /// <summary>
        /// 访问或设置章节标题
        /// </summary>
        public string Title
        {
            get => p_title;
            set => p_title = value ?? string.Empty;
        }

        /// <summary>
        /// 获取该章节所有行数据
        /// </summary>
        public List<ChapterLine> ChapterLines
        {
            get => p_lines;
        }

        /// <summary>
        /// 访问当前章节的行数
        /// </summary>
        public int Count => p_lines.Count;

        #endregion

        #region 集合内容

        /// <summary>
        /// 添加一行文本
        /// </summary>
        /// <param name="text">要添加的行</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void AddLine(string text)
        {
            if (text is null) throw new ArgumentNullException();
            if (ChapterLine.IsEmptyLine(text))
            {
                p_lines.Add(ChapterLine.EmptyLine);
            }
            else
            {
                p_lines.Add(new ChapterLine(text));
            }
            
        }

        /// <summary>
        /// 添加一个图片插入行
        /// </summary>
        /// <param name="imagePath">图像路径</param>
        /// <exception cref="ArgumentException">路径为空</exception>
        public void AddInsertImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) throw new ArgumentException();
            p_lines.Add(new ChapterLine(imagePath, false, 0, 0));
        }

        /// <summary>
        /// 添加一个图片插入行
        /// </summary>
        /// <param name="imagePath">图像路径</param>
        /// <param name="width">长度比</param>
        /// <param name="height">高度比</param>
        /// <exception cref="ArgumentException">路径为空</exception>
        public void AddInsertImage(string imagePath, float width, int height)
        {
            if (string.IsNullOrEmpty(imagePath)) throw new ArgumentException();
            p_lines.Add(new ChapterLine(imagePath, true, width, height));
        }

        /// <summary>
        /// 清空当前章节的所有内容和标题
        /// </summary>
        public void Clear()
        {
            Title = null;
            p_lines.Clear();
        }

        #endregion

        #region 读取

        /// <summary>
        /// 从文本读取器中不断读取行数据并添加到当前章节
        /// </summary>
        /// <param name="reader">要读取文本的读取器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotImplementedException">行数据内存在非easybook标准语法</exception>
        public void ReadLinesToText(TextReader reader)
        {
            if (reader is null) throw new ArgumentNullException();

            int nowIndex = p_lines.Count - 1;
            int count = 0;
            Loop:
            var line = reader.ReadLine();

            if(line is null)
            {
                return;
            }
            ChapterLine chapLine;
            try
            {
                chapLine = ChapterLine.TextToChapterLine(line);
            }
            catch (ArgumentException aex)
            {
                if(nowIndex != -1)
                {
                    p_lines.RemoveRange(nowIndex, count);
                }
                throw new NotImplementedException(Cheng.Properties.Resources.Exception_ArgsFormatError, aex);
            }

            p_lines.Add(chapLine);
            count++;
            goto Loop;

        }

        /// <summary>
        /// 从文本读取器中不断读取行数据并添加到当前章节，排除所有空行
        /// </summary>
        /// <param name="reader">要读取文本的读取器</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="NotImplementedException">行数据内存在非easybook标准语法</exception>
        public void ReadLinesToTextRemoveEmptyLine(TextReader reader)
        {
            if (reader is null) throw new ArgumentNullException();

            int nowIndex = p_lines.Count - 1;
            int count = 0;
            Loop:
            var line = reader.ReadLine();

            if (line is null)
            {
                return;
            }
            ChapterLine chapLine;
            try
            {
                chapLine = ChapterLine.TextToChapterLine(line);
            }
            catch (ArgumentException aex)
            {
                if (nowIndex != -1)
                {
                    p_lines.RemoveRange(nowIndex, count);
                }
                throw new NotImplementedException(Cheng.Properties.Resources.Exception_ArgsFormatError, aex);
            }

            if(chapLine.type != ChapterLineType.Empty)
            {
                p_lines.Add(chapLine);
                count++;
            }

            goto Loop;
        }

        /// <summary>
        /// 从文本读取器中创建一个章节
        /// </summary>
        /// <param name="reader">文本读取器</param>
        /// <returns>创建的新章节</returns>
        /// <exception cref="ArgumentNullException">读取器是null</exception>
        /// <exception cref="ArgumentException">读取器内没有可读取的文本</exception>
        /// <exception cref="NotImplementedException">某一行不属于easybook语法文本</exception>
        public static Chapter CreateChapter(TextReader reader)
        {
            if (reader is null) throw new ArgumentNullException();

            var line = reader.ReadLine();

            if (line is null) throw new ArgumentException();

            if (ChapterLine.IsEmptyLine(line))
            {
                throw new NotImplementedException();
            }

            Chapter cp = new Chapter(line);

            cp.ReadLinesToTextRemoveEmptyLine(reader);

            return cp;
        }

        /// <summary>
        /// 从文本读取器读取章节标题和内容，写入现有章节实例
        /// </summary>
        /// <param name="reader">文本读取器</param>
        /// <param name="chapterBuffer">要写入的章节实例</param>
        /// <exception cref="ArgumentNullException">读取器是null</exception>
        /// <exception cref="ArgumentException">读取器内没有可读取的文本</exception>
        /// <exception cref="NotImplementedException">某一行不属于easybook语法文本</exception>
        public static void CreateChapter(TextReader reader, Chapter chapterBuffer)
        {
            if (reader is null || chapterBuffer is null) throw new ArgumentNullException();

            var line = reader.ReadLine();

            if (line is null) throw new ArgumentException();

            if (ChapterLine.IsEmptyLine(line))
            {
                throw new NotImplementedException();
            }

            Chapter cp = chapterBuffer;
            cp.Clear();
            cp.Title = line;
            cp.ReadLinesToTextRemoveEmptyLine(reader);

        }

        /// <summary>
        /// 创建一个枚举器用于逐行读取easybook文本行
        /// </summary>
        /// <param name="reader">读取器</param>
        /// <returns>一个逐行读取easybook章节内容的枚举器，每次枚举都会读取下行，直至读取完毕；如果中途出现错误则会返回一次空行</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static IEnumerable<ChapterLine> EnumatorReadChapter(TextReader reader)
        {
            if (reader is null) throw new ArgumentNullException();
            return f_enumatorReadChapter(reader);
        }

        internal static IEnumerable<ChapterLine> f_enumatorReadChapter(TextReader reader)
        {

            Loop:
            
            var line = reader.ReadLine();

            if (line is null)
            {
                yield break;
            }
            ChapterLine chapLine;
            try
            {
                chapLine = ChapterLine.TextToChapterLine(line);
            }
            catch (Exception)
            {
                goto Exc;
            }

            yield return chapLine;
            goto Loop;

            Exc:
            yield return ChapterLine.EmptyLine;
            goto Loop;

        }

        #endregion

        #region 派生

        /// <summary>
        /// 本章标题
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return p_title;
        }

        #endregion

        #endregion

    }

    /// <summary>
    /// easybook章节流
    /// </summary>
    /// <remarks>
    /// <para>逐行读取的章节内容的读取器</para>
    /// </remarks>
    public sealed class ChapterStream : SafreleaseUnmanagedResources
    {

        #region 初始化

        /// <summary>
        /// 使用流数据初始化一个easybook章节流
        /// </summary>
        /// <param name="reader">读取器，第一行是标题</param>
        /// <exception cref="ArgumentException">读取器没有作为标题的第一行数据</exception>
        public ChapterStream(TextReader reader) : this(reader, true)
        {
        }

        /// <summary>
        /// 使用流数据初始化一个easybook章节流
        /// </summary>
        /// <param name="reader">读取器，第一行是标题</param>
        /// <param name="onDispose">释放时是否释放内部托管对象</param>
        /// <exception cref="ArgumentException">读取器没有作为标题的第一行数据</exception>
        public ChapterStream(TextReader reader, bool onDispose)
        {
            if (reader is null) throw new ArgumentNullException();

            p_reader = reader;
            p_onDispose = onDispose;
            f_init();
        }

        private void f_init()
        {

            var line = p_reader.ReadLine();
            if (line is null || ChapterLine.IsEmptyLine(line))
            {
                throw new ArgumentException();
            }

            p_title = line;
        }

        #endregion

        #region 参数

        private TextReader p_reader;
        private string p_title;
        private bool p_onDispose;

        #endregion

        #region 功能

        #region 释放

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing && p_onDispose)
            {
                p_reader?.Close();
            }
            p_reader = null;
            return true;
        }

        #endregion

        #region

        /// <summary>
        /// 章节标题
        /// </summary>
        public string Title
        {
            get
            {
                return p_title;
            }
        }

        /// <summary>
        /// 读取下一行文本
        /// </summary>
        /// <returns>一行文本转化的easybook行数据，返回null表示已经读取完毕，没有多余的行数据可读</returns>
        /// <exception cref="NotImplementedException">读取的行文本不属于easybook</exception>
        /// <exception cref="ObjectDisposedException">已释放</exception>
        public ChapterLine? ReadLine()
        {
            ThrowObjectDisposeException();
            string line;
            line = p_reader.ReadLine();

            if(line is null)
            {
                return null;
            }
            try
            {
                return ChapterLine.TextToChapterLine(line);
            }
            catch (ArgumentException ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            
        }

        /// <summary>
        /// 本章标题
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return p_title;
        }

        #endregion

        #endregion

    }

}
