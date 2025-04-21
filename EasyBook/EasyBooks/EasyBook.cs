using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using Cheng.Memorys;
using Cheng.IO;
using Cheng.Algorithm;
using Cheng.Algorithm.Compressions;
using Cheng.DataStructure.Collections;
using Cheng.Algorithm.Trees;
using Cheng.Json;
using System.IO;
using Cheng.Texts;

namespace Cheng.EasyBooks
{

    /// <summary>
    /// easybook读取器
    /// </summary>
    /// <remarks>
    /// <para>初始化easybook标头、分卷节点、章节路径；并提供便利的章节内容读取功能</para>
    /// </remarks>
    public sealed class EasyBook : SafreleaseUnmanagedResources
    {

        #region 结构

        /// <summary>
        /// 节点对比参数
        /// </summary>
        private struct TreeNodeFindFunc
        {
            /// <summary>
            /// 初始化对比参数
            /// </summary>
            /// <param name="value">对比值</param>
            /// <param name="equalFullPath">true对比路径，false对比名称</param>
            public TreeNodeFindFunc(string value, bool equalFullPath)
            {
                this.value = value;
                this.equalFullPath = equalFullPath;
            }

            /// <summary>
            /// 对比值
            /// </summary>
            public readonly string value;

            /// <summary>
            /// 是否对比全路径
            /// </summary>
            public bool equalFullPath;

            public bool FindToName(TreeNode<TreeNodeData> node)
            {
                if (equalFullPath)
                {
                    return node.Value.dataPath == value;
                }
                else
                {
                    return node.Value.name == value;
                }
            }

        }

        #endregion

        #region

        /// <summary>
        /// 初始化easybook读取器
        /// </summary>
        /// <param name="easybook">数据读写器</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">读取器内的结构不属于easybook定义</exception>
        public EasyBook(BaseCompressionParser easybook) : this(easybook, true)
        {
        }

        /// <summary>
        /// 初始化easybook读取器
        /// </summary>
        /// <param name="easybook">数据读写器</param>
        /// <param name="onDispose">在释放对象时是否释放内部封装对象；true释放，false不释放；该参数默认是true</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException">读取器内的结构不属于easybook定义</exception>
        public EasyBook(BaseCompressionParser easybook, bool onDispose)
        {
            if(easybook is null)
            {
                throw new ArgumentNullException();
            }

            p_pack = easybook;
            p_onDispose = onDispose;
            f_init();
        }

        private void f_init()
        {
            p_packCanOpenStream = false;
            if (p_pack.CanOpenCompressedStreamByPath)
            {
                p_packCanOpenStream = true;
            }
            else if (p_pack.CanDeCompressionByPath)
            {
                throw new NotSupportedException();
            }
            else
            {
                p_strBuffer = new StringBuilder();
                p_bufferStream = new MemoryStream(new byte[64], 0, 0, true, true);
            }

            if (!p_pack.ContainsData(HeaderPath))
            {
                throw new NotImplementedException(Cheng.Properties.Resources.Exception_EasybookStructureError);
            }

            try
            {
                p_chapterComparer = new ChapterIndexComparer();
                
                p_pathSeparator = new char[] { '\\', '/' };
                //初始化树结构
                p_root = TreeAction.ListToTreeNode(p_pack, p_pathSeparator, "\\");

                //初始化解析器
                p_jsonParser = new JsonParserDefault();

                JsonVariable headerJson;
                //读取头

                headerJson = f_getJsonDataByPath(HeaderPath);
                p_header = EasyBookHeader.JsonToEasyBookHeader(headerJson);
                p_header.coverImage = f_getCoverPath();

                //获取text节点
                TreeNodeFindFunc nodeEQfunc = new TreeNodeFindFunc(TextRootFolder, false);
                p_textRoot = p_root.Find(nodeEQfunc.FindToName);
                if (p_textRoot is null) throw new NotImplementedException(Cheng.Properties.Resources.Exception_EasybookStructureError);

                //获取image节点（如果有）
                nodeEQfunc = new TreeNodeFindFunc(ImageRootFolder, false);
                p_imageRoot = p_root.Find(nodeEQfunc.FindToName);

                Volume rootV = new Volume();
                rootV.name = p_header.name;
                rootV.index = TextRootFolder;
                rootV.synopsis = p_header.synopsis;
                //初始化分卷结构
                rootV.customItems = (p_header.customs?.Clone()) as KeyValuePair<string, string>[];
                p_volumeRoot = new TreeNode<Volume>(rootV);
                f_foreachVolumes(p_volumeRoot, 0);

            }
            catch (NotImplementedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
        }

        private string f_getCoverPath()
        {
            if (p_pack.ContainsData("cover.png")) return "cover.png";
            if (p_pack.ContainsData("cover.jpeg")) return "cover.jpeg";
            if (p_pack.ContainsData("cover.jpg")) return "cover.jpg";
            if (p_pack.ContainsData("cover.bmp")) return "cover.bmp";
            return null;
        }

        private void f_foreachVolumes(TreeNode<Volume> node, int lvl)
        {

            var volValue = node.Value;
            int volvl = lvl;

            //最终索引 
            var path = volValue.index;

            var pn = node;

            //向前拼接 获取当前分卷所在目录
            while (volvl > 0)
            {
                pn = pn.Parent;
                path = Path.Combine(pn.Value.index, path);
                volvl--;
            }
            //if(path is null)
            //{
            //    path = volRootPath;
            //}
            //else
            //{
            //    path = Path.Combine(volRootPath, path);
            //}

            //当前分卷索引文件路径
            var nowVolFile = Path.Combine(path, VolumeFileName);

            if (p_pack.ContainsData(nowVolFile))
            {
                //存在子分卷
                volValue.volumeType = VolumeType.Node;
                var volJson = f_getJsonDataByPath(nowVolFile);
                //获取子分卷枚举器
                var volEnumator = Volume.JsonToVolumes(volJson, p_strBuffer, out int volCount);

                foreach (var item in volEnumator)
                {
                    if (item.HasValue)
                    {
                        var volItem = item.Value;
                        //创建子分卷索引
                        TreeNode<Volume> chsNode = new TreeNode<Volume>(volItem);
                        //添加分卷到父节点
                        node.Add(chsNode);
                        //检索此分卷
                        f_foreachVolumes(chsNode, lvl + 1);
                    }
                }
            }
            else
            {
                //不存在子分卷
                //获取当前章节路径
                var pathToNode = f_PathGetNode(path);
                if(pathToNode is null)
                {
                    throw new NotImplementedException(Cheng.Properties.Resources.Exception_ArgsFormatError);
                }

                //当前
                int nowVolumeChapterCount = pathToNode.Count;
                if (nowVolumeChapterCount == 0)
                {
                    volValue.volumeType = VolumeType.Empty;
                    //空分卷
                }
                else
                {
                    volValue.volumeType = VolumeType.Chapters;
                    volValue.chapters = new List<ChapterIndex>(pathToNode.Count);
                    //章节内容
                    for (int i = 0; i < nowVolumeChapterCount; i++)
                    {
                        var chapterFileNode = pathToNode[i].Value;

                        if (chapterFileNode.isFile && (Path.GetExtension(chapterFileNode.name)?.ToLowerInvariant() == ".txt"))
                        {
                            //判断是章节文件并写入头
                            var title = f_readFirstLine(chapterFileNode.dataPath);
                            if(title is null)
                            {
                                throw new NotImplementedException(Cheng.Properties.Resources.Exception_ArgsFormatError);
                            }

                            volValue.chapters.Add(new ChapterIndex(title, chapterFileNode.name, chapterFileNode.dataPath));
                        }

                    }

                    volValue.chapters.Sort(p_chapterComparer);

                }
                
            }

            //修改值返回
            node.Value = volValue;

        }

        /// <summary>
        /// 读取路径数据下第一行
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string f_readFirstLine(string path)
        {
            if (p_packCanOpenStream)
            {
                using (var open = p_pack.OpenCompressedStream(path))
                {
                    using (StreamReader sr = new StreamReader(open, Encoding.UTF8, false, 3 * 32, true))
                    {
                        return sr.ReadLine();
                    }
                }
            }
            else
            {
                MemoryStream open = p_bufferStream;
                open.Seek(0, SeekOrigin.Begin);
                open.SetLength(0);
                //if ()
                //{
                    
                //}
                //else
                //{

                //}
                p_pack.DeCompressionTo(path, open);
                open.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(open, Encoding.UTF8, false, 3 * 32, true))
                {
                    return sr.ReadLine();
                }

            }
        }

        #endregion

        #region 参数

        #region 常量

        /// <summary>
        /// 图像文件根目录
        /// </summary>
        public const string ImageRootFolder = "image";

        /// <summary>
        /// 书籍内容根目录
        /// </summary>
        public const string TextRootFolder = "text";

        /// <summary>
        /// 文件头路径
        /// </summary>
        public const string HeaderPath = "easybook.json";

        /// <summary>
        /// 封面图像文件名
        /// </summary>
        public const string CoverImagePath = "cover";

        /// <summary>
        /// 分卷索引器名称
        /// </summary>
        public const string VolumeFileName = "Volume.json";

        #endregion

        #region 参数

        /// <summary>
        /// 书籍头
        /// </summary>
        private EasyBookHeader p_header;

        /// <summary>
        /// 数据读取器
        /// </summary>
        private BaseCompressionParser p_pack;

        /// <summary>
        /// 读取器数据的树状节点
        /// </summary>
        private TreeNode<TreeNodeData> p_root;

        /// <summary>
        /// 分卷结构，此实例是根分卷节点，Value为null时，不再有子分卷
        /// </summary>
        private TreeNode<Volume> p_volumeRoot;

        /// <summary>
        /// 书籍数据根目录
        /// </summary>
        private TreeNode<TreeNodeData> p_textRoot;

        /// <summary>
        /// 图像数据存储根目录，null表示不存在
        /// </summary>
        private TreeNode<TreeNodeData> p_imageRoot;

        /// <summary>
        /// json解析器
        /// </summary>
        private JsonParserDefault p_jsonParser;

        private StringBuilder p_strBuffer;

        private ChapterIndexComparer p_chapterComparer;

        /// <summary>
        /// 路径分隔符集合
        /// </summary>
        private char[] p_pathSeparator;

        /// <summary>
        /// 如果没有流打开权限，则此实例有用
        /// </summary>
        private MemoryStream p_bufferStream;

        /// <summary>
        /// true表示有打开流的权限，false只有解数据权限
        /// </summary>
        private bool p_packCanOpenStream;

        private bool p_onDispose;

        #endregion

        #endregion

        #region 释放

        protected override bool Disposeing(bool disposeing)
        {

            if (disposeing && p_onDispose && p_pack.IsNeedToReleaseResources)
            {
                if(p_onDispose && p_pack.IsNeedToReleaseResources) p_pack?.Close();
            }

            p_pack = null;

            return true;
        }

        #endregion

        #region 封装

        /// <summary>
        /// 从路径读取数据并转化为json
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private JsonVariable f_getJsonDataByPath(string path)
        {
            JsonVariable json;

            if (p_packCanOpenStream)
            {
                using (var openS = p_pack.OpenCompressedStream(path))
                {
                    using (StreamReader reader = new StreamReader(openS, Encoding.UTF8, false, 1024, true))
                    {
                        json = p_jsonParser.ToJsonData(reader);
                    }
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder(64);
                p_pack.DeCompressionToText(path, Encoding.UTF8, sb);

                using (StringBuilderReader sbreader = new StringBuilderReader(sb))
                {
                    json = p_jsonParser.ToJsonData(sbreader);
                }
            }

            return json;
        }

        /// <summary>
        /// 按照路径返回节点
        /// </summary>
        /// <param name="path">一个数据路径或文件夹路径</param>
        /// <returns>节点</returns>
        private TreeNode<TreeNodeData> f_PathGetNode(string path)
        {
            TreeNodeFindFunc nodeFind;
            var spaths = path.Split(p_pathSeparator);

            int length = spaths.Length;

            TreeNode<TreeNodeData> node = p_root;

            for (int i = 0; i < length; i++)
            {
                var sp = spaths[i];

                nodeFind = new TreeNodeFindFunc(sp, false);

                node = node.Find(nodeFind.FindToName);
                if(node is null)
                {
                    return null;
                }
            }

            return node;
        }

        #endregion

        #region 功能

        #region 参数获取

        /// <summary>
        /// 书籍数据根目录
        /// </summary>
        /// <exception cref="ObjectDisposedException">实例已释放</exception>
        public TreeNode<TreeNodeData> TreeRoot
        {
            get
            {
                ThrowObjectDisposeException();
                return p_root;
            }
        }

        /// <summary>
        /// 书籍标头
        /// </summary>
        /// <exception cref="ObjectDisposedException">实例已释放</exception>
        public EasyBookHeader Header
        {
            get
            {
                ThrowObjectDisposeException();
                return p_header;
            }
        }

        /// <summary>
        /// 书籍的根分卷
        /// </summary>
        /// <returns>
        /// <para>代表"text"根目录所在的分卷，如果有子节点则子节点代表子分卷</para>
        /// <para>该节点的<see cref="Volume.index"/>固定为"text"，且名称和自定义标签与书籍头一致</para>
        /// </returns>
        /// <exception cref="ObjectDisposedException">实例已释放</exception>
        public TreeNode<Volume> VolumeRoot
        {
            get
            {
                ThrowObjectDisposeException();
                return p_volumeRoot;
            }
        }

        /// <summary>
        /// 图像资源所在的根节点
        /// </summary>
        /// <exception cref="ObjectDisposedException">实例已释放</exception>
        public TreeNode<TreeNodeData> ImageRoot
        {
            get
            {
                ThrowObjectDisposeException();
                return p_imageRoot;
            }
        }

        /// <summary>
        /// 获取内部封装的数据管理器
        /// </summary>
        /// <exception cref="ObjectDisposedException">实例已释放</exception>
        public BaseCompressionParser BaseDataPack
        {
            get
            {
                ThrowObjectDisposeException();
                return p_pack;
            }
        }

        /// <summary>
        /// 根据路径返回节点
        /// </summary>
        /// <param name="path">数据或文件夹路径</param>
        /// <returns>一个路径节点，如果不存在则为null</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public TreeNode<TreeNodeData> GetNodeByPath(string path)
        {
            ThrowObjectDisposeException();
            if (path is null) throw new ArgumentNullException();

            return f_PathGetNode(path);
        }

        /// <summary>
        /// 按照分卷树节点获取分卷文件夹所在路径
        /// </summary>
        /// <param name="volumeNode">分卷</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">节点参数不属于easybook内部参数</exception>
        public string GetVolumePathByNode(TreeNode<Volume> volumeNode)
        {
            if (volumeNode is null) throw new ArgumentNullException();
            ThrowObjectDisposeException();
            TreeNode<Volume> tn = volumeNode;

            Stack<string> stack = new Stack<string>();

            while (tn != null)
            {
                //文件名压栈
                var tnv = tn.Value;
                stack.Push(tnv.index);

                var pn = tn.Parent;
                if(pn is null) if (tn != p_volumeRoot) throw new ArgumentException();
                tn = pn;
            }

            return Path.Combine(stack.ToArray());
        }

        #endregion

        #region 数据读取

        /// <summary>
        /// 根据图像路径获取图像所在的数据包目录
        /// </summary>
        /// <param name="image">easybook使用的图像路径</param>
        /// <returns>图像所在的数据包路径</returns>
        /// <exception cref="ArgumentException">路径不正确</exception>
        /// <exception cref="ObjectDisposedException">实例已释放</exception>
        public string GetFullNameByImagePath(string image)
        {
            ThrowObjectDisposeException();
            if (string.IsNullOrEmpty(image)) throw new ArgumentException();
            return Path.Combine(ImageRootFolder, image);
        }

        /// <summary>
        /// 按照章节头创建一个easybook章节
        /// </summary>
        /// <param name="chapterIndex">章节头</param>
        /// <returns>新的章节实例</returns>
        /// <exception cref="ArgumentNullException">路径或其它参数为null</exception>
        /// <exception cref="NotImplementedException">错误的easybook路径</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="ObjectDisposedException">实例已释放</exception>
        /// <exception cref="Exception">其它错误</exception>
        public Chapter CreateChapter(ChapterIndex chapterIndex)
        {
            ThrowObjectDisposeException();
            if (!p_pack.ContainsData(chapterIndex.chapterFilePath)) throw new NotImplementedException();

            if (p_packCanOpenStream)
            {
                using (var open = p_pack.OpenCompressedStream(chapterIndex.chapterFilePath))
                {
                    using (StreamReader sr = new StreamReader(open, Encoding.UTF8, false, 1024, true))
                    {
                        return Chapter.CreateChapter(sr);
                    }
                }
            }
            else
            {
                var open = p_bufferStream;
                lock (open)
                {
                    open.SetLength(0);
                    open.Seek(0, SeekOrigin.Begin);
                    using (StreamReader sr = new StreamReader(open, Encoding.UTF8, false, 1024, true))
                    {
                        return Chapter.CreateChapter(sr);
                    }
                }

            }
        }

        /// <summary>
        /// 按照章节头创建一个逐行读取的easybook章节
        /// </summary>
        /// <param name="chapterIndex">章节头</param>
        /// <returns>逐行读取的章节对象，当不再使用后需要手动释放</returns>
        /// <exception cref="ArgumentNullException">路径或其它参数为null</exception>
        /// <exception cref="NotImplementedException">错误的easybook路径</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">没有打开包数据流的权限</exception>
        /// <exception cref="ObjectDisposedException">实例已释放</exception>
        /// <exception cref="Exception">其它错误</exception>
        public ChapterStream CreateChapterStream(ChapterIndex chapterIndex)
        {
            ThrowObjectDisposeException();

            if (p_packCanOpenStream)
            {
                Stream open = null;
                try
                {
                    open = p_pack.OpenCompressedStream(chapterIndex.chapterFilePath);
                    if (open is null) throw new NotImplementedException();

                    StreamReader sr = new StreamReader(open, Encoding.UTF8, false, 1024, false);
                    return new ChapterStream(sr, true);
                }
                catch (Exception)
                {
                    open?.Close();
                    throw;
                }
               
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// 按照章节文件所在路径头创建一个easybook章节
        /// </summary>
        /// <param name="chapterPath">章节文件路径</param>
        /// <returns>新的章节实例</returns>
        /// <exception cref="ArgumentNullException">路径或其它参数为null</exception>
        /// <exception cref="NotImplementedException">错误的easybook路径</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="ObjectDisposedException">实例已释放</exception>
        /// <exception cref="Exception">其它错误</exception>
        public Chapter CreateChapter(string chapterPath)
        {
            ThrowObjectDisposeException();
            if (!p_pack.ContainsData(chapterPath)) throw new NotImplementedException();

            if (p_packCanOpenStream)
            {
                using (var open = p_pack.OpenCompressedStream(chapterPath))
                {
                    using (StreamReader sr = new StreamReader(open, Encoding.UTF8, false, 1024, true))
                    {
                        return Chapter.CreateChapter(sr);
                    }
                }
            }
            else
            {
                var open = p_bufferStream;
                lock (open)
                {
                    open.SetLength(0);
                    open.Seek(0, SeekOrigin.Begin);
                    using (StreamReader sr = new StreamReader(open, Encoding.UTF8, false, 1024, true))
                    {
                        return Chapter.CreateChapter(sr);
                    }
                }

            }
        }

        /// <summary>
        /// 按照章节文件所在路径头创建一个逐行读取的easybook章节
        /// </summary>
        /// <param name="chapterPath">章节文件路径</param>
        /// <returns>逐行读取的章节对象，当不再使用后需要手动释放</returns>
        /// <exception cref="ArgumentNullException">路径或其它参数为null</exception>
        /// <exception cref="NotImplementedException">错误的easybook路径</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">没有打开包数据流的权限</exception>
        /// <exception cref="ObjectDisposedException">实例已释放</exception>
        /// <exception cref="Exception">其它错误</exception>
        public ChapterStream CreateChapterStream(string chapterPath)
        {
            ThrowObjectDisposeException();
            if (p_packCanOpenStream)
            {
                Stream open = null;
                try
                {
                    open = p_pack.OpenCompressedStream(chapterPath);
                    if (open is null) throw new NotImplementedException();

                    StreamReader sr = new StreamReader(open, Encoding.UTF8, false, 1024, false);
                    return new ChapterStream(sr, true);
                }
                catch (Exception)
                {
                    open?.Close();
                    throw;
                }

            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// 使用章节分卷创建一个章节枚举器
        /// </summary>
        /// <param name="volumeNode">此书籍的章节分卷</param>
        /// <returns>从第一章到最后一章逐步推进的章节枚举器</returns>
        /// <exception cref="ArgumentNullException">路径或其它参数为null</exception>
        /// <exception cref="NotImplementedException">错误的easybook解析</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="ObjectDisposedException">实例已释放</exception>
        /// <exception cref="Exception">其它错误</exception>
        public IEnumerable<Chapter> VolumeCreateChapters(TreeNode<Volume> volumeNode)
        {
            ThrowObjectDisposeException();
            if (volumeNode is null) throw new ArgumentNullException();

            var vol = volumeNode.Value;
            if(vol.chapters is null || vol.chapters.Count == 0) throw new NotImplementedException();
            
            return f_volumeCreateChapters(vol.chapters);
        }

        private IEnumerable<Chapter> f_volumeCreateChapters(List<ChapterIndex> list)
        {
            
            for (int i = 0; i < list.Count; i++)
            {
                yield return CreateChapter(list[i]);
            }

        }

        #endregion

        #endregion

    }

}
