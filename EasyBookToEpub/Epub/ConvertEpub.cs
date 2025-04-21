using Cheng.Algorithm.Compressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.IO.Compression;
using System.Globalization;
using Cheng.Algorithm.Trees;
using Cheng.Algorithm.Collections;
using Cheng.DataStructure.Collections;

namespace Cheng.EasyBooks
{

    /// <summary>
    /// 将easybook转换为epub的简单方法
    /// </summary>
    /// <exception cref="Exception">任何方法有可能引发任何异常，请确保参数正确</exception>
    public static class ConvertEpub
    {

        #region 创建epub结构

        /// <summary>
        /// 路径分隔符
        /// </summary>
        public const char pathSeparator = '/';

        /// <summary>
        /// 从ebk创建epub结构到zip
        /// </summary>
        /// <param name="ebk">easybook</param>
        /// <param name="zip">空zip</param>
        /// <param name="outputLog" >输出日志，null表示没有</param>
        public static void CreateEpubByEbk(EasyBook ebk, ZipArchive zip, TextWriter outputLog)
        {
            if (ebk is null || zip is null) throw new ArgumentNullException();
            if (!ebk.IsNotDispose) throw new ObjectDisposedException("ebk");
            bool print = outputLog != null;

            if(print) outputLog.WriteLine("正在创建基础epub结构");
            CreateEpubZip(zip);

            //if (print) outputLog.WriteLine("正在创建基础索引文件");
            //CreateContainer(zip);

            if (print) outputLog.WriteLine("调换资源索引");
            CreateEpubOPF(zip, ebk);

            if (print) outputLog.WriteLine("创建目录节点顺序");
            CreateTocEpub(zip, ebk);

            int cpCount = 0, volCount = 0;

            if (print) outputLog.WriteLine("准备拷贝写入书籍文件");
            CreateEpubChapters(zip, ebk, cpCreate, volCreate);

            void cpCreate(ChapterIndex cindex)
            {
                if (print) outputLog.WriteLine("创建章节:" + cindex.chapterFilePath);
                cpCount++;
            }
            void volCreate(Volume volume)
            {
                if (print) outputLog.WriteLine("创建分卷:" + volume.name);
                volCount++;
            }

            int imageCount = 0;

            if (print) outputLog.WriteLine("准备拷贝图像数据");
            CreateEpubImages(zip, ebk, imageCreateAction);
            
            void imageCreateAction(TreeNode<TreeNodeData> imageNode)
            {
                if (print) outputLog.WriteLine($"拷贝图像:\"{imageNode.Value.dataPath}\"");
                imageCount++;
            }

            //封面算一张图
            if (ebk.Header.coverImage is object) imageCount++;

            if (print) outputLog.WriteLine($"转换成功，共{cpCount}章 {volCount}卷 {imageCount}图");

        }

        #region zip

        /// <summary>
        /// 创建一个epub章节文件
        /// </summary>
        /// <param name="chapter">要创建的ebk章节流</param>
        /// <param name="ci">与之对应的章节头</param>
        public static XmlDocument CreateEpubChapter(ChapterStream chapter, ChapterIndex ci)
        {
            if (chapter is null || ci.chapterFilePath is null) throw new ArgumentNullException();

            var xml = CreateEpubChapterHtml(chapter.Title, out var body);

            var fileSp = ci.chapterFilePath.pathRepTo().Split(pathSeparator);
            if (fileSp.Length < 2) throw new ArgumentException(); //路径不对

            // 两个表示 ../ 三个表示 ../../
            //前切路径符数量
            int lastSepCount = fileSp.Length - 1;

            const string lastStr = "../";

            string lsPath = "";

            for (int i = 0; i < lastSepCount; i++)
            {
                lsPath = lsPath + lastStr;
            }

            while (true)
            {
                var line = chapter.ReadLine();

                if (line is null) break;

                var lv = line.Value;

                if(lv.type == ChapterLineType.Text)
                {
                    var np = xml.CreateElement("p");
                    np.AppendChild(xml.CreateTextNode(lv.value));
                    body.AppendChild(np);
                }
                else if(lv.type == ChapterLineType.Image)
                {
                    var imgPath = Path.Combine(lsPath, "image", lv.value).pathRepTo();
                    var imgNode = xml.CreateElement("img");
                    imgNode.SetAttribute("src", imgPath);
                    body.AppendChild(imgNode);
                }

            }

            return xml;
        }

        /// <summary>
        /// 创建一个epub章节文件
        /// </summary>
        /// <param name="xml">要写入的xml空结构</param>
        /// <param name="chapter">要创建的ebk章节</param>
        public static XmlDocument CreateEpubChapter(Chapter chapter, ChapterIndex ci)
        {
            if (chapter is null) throw new ArgumentNullException();

            var xml = CreateEpubChapterHtml(chapter.Title, out var body);

            var fileSp = ci.chapterFilePath.pathRepTo().Split(pathSeparator);
            if (fileSp.Length < 2) throw new ArgumentException(); //路径不对

            // 两个表示 ../ 三个表示 ../../
            //前切路径符数量
            int lastSepCount = fileSp.Length - 1;

            const string lastStr = "../";

            string lsPath = "";

            for (int i = 0; i < lastSepCount; i++)
            {
                lsPath = lsPath + lastStr;
            }

            foreach (var lv in chapter.ChapterLines)
            {

                if (lv.type == ChapterLineType.Text)
                {
                    var np = xml.CreateElement("p");
                    np.AppendChild(xml.CreateTextNode(lv.value));
                    body.AppendChild(np);
                }
                else if (lv.type == ChapterLineType.Image)
                {
                    var imgPath = Path.Combine(lsPath, "image", lv.value).pathRepTo();
                    var imgNode = xml.CreateElement("img");
                    imgNode.SetAttribute("src", imgPath);
                    body.AppendChild(imgNode);
                }
            }

            return xml;
        }

        /// <summary>
        /// 为zip创建Epub基础结构
        /// </summary>
        /// <param name="zip">空zip</param>
        public static void CreateEpubZip(ZipArchive zip)
        {
            //创建头
            var mimetype = zip.CreateEntry("mimetype", CompressionLevel.NoCompression);

            using (var openS = mimetype.Open())
            {
                var bs = Encoding.ASCII.GetBytes("application/epub+zip");
                openS.Write(bs, 0, bs.Length);
            }

            //创建 container
            var cont = zip.CreateEntry("META-INF/container.xml");
            XmlDocument xml;
            using (StreamWriter swr = new StreamWriter(cont.Open(), Encoding.UTF8, 1024, false))
            {
                xml = CreateContainer();
                xml.Save(swr);
            }
        }

        /// <summary>
        /// （epub基础结构）为epub包从ebk创建opf（<see cref="CreateEpubOPF(ZipArchive, EasyBook)"/>后调用）
        /// </summary>
        /// <param name="zip">epub包</param>
        /// <param name="easybook">ebk</param>
        public static void CreateEpubOPF(ZipArchive zip, EasyBook easybook)
        {
            if (zip is null || easybook is null) throw new ArgumentNullException();

            var cont = zip.CreateEntry(@"OEBPS/content.opf");

            using (StreamWriter swr = new StreamWriter(cont.Open(), Encoding.UTF8, 1024, false))
            {
                var xml = CreateContent(easybook);
                xml.Save(swr);
            }
        }

        /// <summary>
        /// （epub基础结构）为epub创建 META-INF/container.xml（<see cref="CreateEpubOPF(ZipArchive, EasyBook)"/>后调用）
        /// </summary>
        /// <param name="zip"></param>
        public static void CreateContainer(ZipArchive zip)
        {
            if (zip is null) throw new ArgumentNullException();

            var cont = zip.CreateEntry(@"META-INF/container.xml");

            using (StreamWriter swr = new StreamWriter(cont.Open(), Encoding.UTF8, 1024, false))
            {
                var xml = CreateContainer();
                xml.Save(swr);
            }
        }

        /// <summary>
        /// （epub基础结构）为epub创建 META-INF/toc.ncx（<see cref="CreateEpubOPF(ZipArchive, EasyBook)"/>后调用）
        /// </summary>
        /// <param name="zip"></param>
        /// <param name="ebk"></param>
        public static void CreateTocEpub(ZipArchive zip, EasyBook ebk)
        {
            if (zip is null || ebk is null) throw new ArgumentNullException();

            var cont = zip.CreateEntry(@"OEBPS/toc.ncx");

            using (StreamWriter swr = new StreamWriter(cont.Open(), Encoding.UTF8, 1024, false))
            {
                var xml = CreateNCX(ebk);
                xml.Save(swr);
            }
        }

        /// <summary>
        /// 将html格式的doc添加一行body
        /// </summary>
        /// <param name="doc">html格式</param>
        /// <param name="bodyElement">body节点</param>
        /// <param name="text">body的一行文本</param>
        public static void AppendTextToBody(XmlDocument doc, XmlElement bodyElement, string text)
        {
            var p = doc.CreateElement("p");
            p.AppendChild(doc.CreateTextNode(text));
            bodyElement.AppendChild(p);
        }

        /// <summary>
        /// 创建一个拥有指定标题的空html结构
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="bodyElement">body节点</param>
        /// <returns></returns>
        public static XmlDocument CreateEpubChapterHtml(string title, out XmlElement bodyElement)
        {
            if (title is null) throw new ArgumentNullException();

            XmlDocument doc = new XmlDocument();

            /*
            <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN"
                "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">

            
            */

            var docType = doc.CreateDocumentType("html", "-//W3C//DTD XHTML 1.1//EN", "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd", null);

            doc.AppendChild(docType);

            var root = doc.CreateElement("html");
            
            root.SetAttribute("xmlns", "http://www.w3.org/1999/xhtml");

            doc.AppendChild(root);

            var head = doc.CreateElement("head");
            var body = doc.CreateElement("body");

            root.AppendChild(head);
            root.AppendChild(body);

            var titleNode = doc.CreateElement("title");
            titleNode.AppendChild(doc.CreateTextNode(title));
            head.AppendChild(titleNode);

            //<link href="../css/style.css" type="text/css" rel="stylesheet"/>
            //var link = doc.CreateElement("link");
            //link.SetAttribute("href", @"..\css\style.css");
            //link.SetAttribute("type", "text\\css");
            //link.SetAttribute("rel", "stylesheet");
            //head.AppendChild(link);

            var h1Title = doc.CreateElement("h1");
            h1Title.AppendChild(doc.CreateTextNode(title));
            body.AppendChild(h1Title);
            bodyElement = body;

            return doc;
        }

        /// <summary>
        /// 将ebk的章节和分卷写入zip
        /// </summary>
        /// <param name="zip">zip文件</param>
        /// <param name="ebk">ebk书籍</param>
        /// <param name="createChapterAction">创建了zip一个章节后执行，null表示不执行</param>
        /// <param name="createVolumeAction">创建了zip一个分卷后执行，null表示不执行</param>
        public static void CreateEpubChapters(ZipArchive zip, EasyBook ebk, Action<ChapterIndex> createChapterAction, Action<Volume> createVolumeAction)
        {
            //根目录 OEBPS\text
            if (zip is null || ebk is null) throw new ArgumentNullException();
            var vols = ebk.VolumeRoot;

            const string rootPath = "OEBPS";

            foeachDepth(vols, createChapterfunc);

            void createChapterfunc(TreeNode<Volume> t_node)
            {
                
                var volValue = t_node.Value;

                if (volValue.volumeType != VolumeType.Chapters && volValue.volumeType != VolumeType.Node)
                {
                    return;
                }

                //属于根节点但是有子节点
                if (t_node.Parent is null && volValue.volumeType == VolumeType.Node) return;

                string path;
                XmlElement body;
                int i;
                ZipArchiveEntry entry;
                
                //节点分卷
                path = getRootPath(t_node);

                bool isRootPath = path == "text";
                //if (isRootPath) return;

                //路径替换 + 扩展名
                var tpath = path.pathRepTo() + ".html";
                tpath = Path.Combine(rootPath, tpath).pathRepTo();

                //创建目录html文件
                var volXml = CreateEpubChapterHtml(volValue.name, out body);

                if (isRootPath && (ebk.Header.coverImage is object))
                {
                    //插入封面
                    //<img src="../images/0001.jpeg"/>
                    var imgNode = volXml.CreateElement("img");
                    imgNode.SetAttribute("src", ebk.Header.coverImage.pathRepTo());
                    body.InsertAfter(imgNode, body.FirstChild);

                }

                if ((volValue.customItems?.Length).GetValueOrDefault() != 0)
                {
                    //拥有标签
                    for (i = 0; i < volValue.customItems.Length; i++)
                    {
                        AppendTextToBody(volXml, body, $"{volValue.customItems[i].Key}: {volValue.customItems[i].Value}");
                    }
                }

                //简介
                if (volValue.synopsis is object && volValue.synopsis.Length != 0)
                {
                    AppendTextToBody(volXml, body, "synopsis:");

                    using (StringReader strReader = new StringReader(volValue.synopsis))
                    {
                        while (true)
                        {
                            var readL = strReader.ReadLine();
                            if (readL is null) break;
                            AppendTextToBody(volXml, body, readL);
                        }
                    }
                }

                //创建目录并写入数据
                entry = zip.CreateEntry(tpath);

                using (var openStream = entry.Open())
                {
                    using (StreamWriter swr = new StreamWriter(openStream, Encoding.UTF8, 1024 * 2, true))
                    {
                        volXml.Save(swr);
                    }
                }

                createVolumeAction?.Invoke(volValue);

                if (volValue.volumeType == VolumeType.Chapters)
                {
                    //章节分卷
                    if (volValue.chapters is null) throw new ArgumentNullException();

                  
                    for (i = 0; i < volValue.chapters.Count; i++)
                    {
                        //XmlDocument volXml;

                        path = volValue.chapters[i].chapterFilePath;
                        //tpath = path.pathRepTo();
                        tpath = Path.Combine(rootPath, path).pathRepTo();
                        tpath = Path.ChangeExtension(tpath, ".html");

                        using (var openC = ebk.CreateChapterStream(volValue.chapters[i]))
                        {
                            volXml = CreateEpubChapter(openC, volValue.chapters[i]);
                        }

                        entry = zip.CreateEntry(tpath);
                        using (var openStream = entry.Open())
                        {
                            using (StreamWriter swr = new StreamWriter(openStream, Encoding.UTF8, 1024 * 2, true))
                            {
                                volXml.Save(swr);
                            }
                        }
                        createChapterAction?.Invoke(volValue.chapters[i]);

                    }

                }

            }

        }

        /// <summary>
        /// 从ebk拷贝图像到zip
        /// </summary>
        /// <param name="zip"></param>
        /// <param name="ebk"></param>
        /// <param name="createImageAction">每次成功拷贝一个图像执行，参数表示图像所在路径</param>
        public static void CreateEpubImages(ZipArchive zip, EasyBook ebk, Action<TreeNode<TreeNodeData>> createImageAction)
        {
            if (zip is null || ebk is null) throw new ArgumentNullException();
            var vols = ebk.VolumeRoot;

            const string rootPath = "OEBPS";

            //ebk.GetFullNameByImagePath("");

            var header = ebk.Header;
            var pack = ebk.BaseDataPack;

            if (header.coverImage is object)
            {
                //存在封面
                var coverT = zip.CreateEntry(Path.Combine(rootPath, header.coverImage).pathRepTo());
                using (var copen = coverT.Open())
                {
                    ebk.BaseDataPack.DeCompressionTo(header.coverImage, copen);
                }

            }

            var rootree = ebk.TreeRoot;

            var id = rootree.FindIndex(isImagePath);

            bool isImagePath(TreeNode<TreeNodeData> tdata)
            {
                return tdata.Value.name == "image";
            }
            if (id < 0) return; //没有图像路径

            //图像节点
            var imageNode = rootree[id];

            imageNode.ForeachBreadth(actionTreeImage, new Queue<TreeNode<TreeNodeData>>());

            //foeachDepth(imageNode, actionTreeImage);

            bool actionTreeImage(TreeNode<TreeNodeData> t_treeNode)
            {
                var volValue = t_treeNode.Value;

                if (!volValue.isFile) return true; //不是文件

                if(volValue.dataPath is null) return false;
                //创建epub节点
                var entry = zip.CreateEntry(Path.Combine(rootPath, volValue.dataPath).pathRepTo());
                using (var zipStream = entry.Open())
                {
                    pack.DeCompressionTo(volValue.dataPath, zipStream); //从ebk拷贝图像
                }
                createImageAction?.Invoke(t_treeNode);
                return true;
            }

        }

        #endregion

        #region container.xml

        /// <summary>
        /// （epub基础结构）创建 container.xml（<see cref="CreateEpubOPF(ZipArchive, EasyBook)"/>）后调用
        /// </summary>
        /// <returns></returns>
        public static XmlDocument CreateContainer()
        {
            XmlDocument xml = new XmlDocument();

            var root = xml.CreateElement("container", "urn:oasis:names:tc:opendocument:xmlns:container");
            root.SetAttribute("version", "1.0");
            xml.AppendChild(root);
            var rootFiles = xml.CreateElement("rootfiles");

            root.AppendChild(rootFiles);

            var rootfile = xml.CreateElement("rootfile");
            rootFiles.AppendChild(rootfile);

            rootfile.SetAttribute("full-path", @"OEBPS/content.opf".pathRepTo());
            rootfile.SetAttribute("media-type", @"application/oebps-package+xml");

            return xml;
        }

        #endregion

        /// <summary>
        /// 分卷查询根目录
        /// </summary>
        /// <param name="volNode"></param>
        /// <returns></returns>
        static string getRootPath(TreeNode<Volume> volNode)
        {
            TreeNode<Volume> tn = volNode;
            string path = "";

            while (true)
            {
                var tp = tn.Parent;
                if (tp is null)
                {
                    break;
                }
                //向前拼接
                path = Path.Combine(path, tn.Value.index);
                //父节点
                tn = tp;
            }
            //获取文件夹根节点
            return Path.Combine("text", path).pathRepTo();
        }

        /// <summary>
        /// 路径替换 - 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static string pathRepIndex(string path)
        {
            return path?.Replace('\\', '-')?.Replace('/', '-');
        }

        /// <summary>
        /// 路径兼容性替换
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static string pathRepTo(this string path)
        {
            return path?.Replace('\\', pathSeparator);
        }

        #region content.opf

        /// <summary>
        /// 通过easybook创建一个content.opf表单
        /// </summary>
        /// <param name="easybook"></param>
        /// <returns></returns>
        public static XmlDocument CreateContent(EasyBook easybook)
        {
            XmlDocument xml = new XmlDocument();

            #region

            var root = xml.CreateElement("package", "http://www.idpf.org/2007/opf");

            root.SetAttribute("unique-identifier", "bookid");
            root.SetAttribute("version", "2.0");
            root.SetAttribute("xmlns", "http://www.idpf.org/2007/opf");
            xml.AppendChild(root);
            //数据头
            var metadata = xml.CreateElement("metadata");
            //资源清单
            var manifest = xml.CreateElement("manifest");
            //阅读顺序
            var spine = xml.CreateElement("spine");
            spine.SetAttribute("toc", "ncx");

            #endregion

            #region metadata

            var eheader = easybook.Header;
            //xml.CreateElement("dc:identifier");
            //书名
            var node = xml.CreateElement("dc", "title", " ");
            node.AppendChild(xml.CreateTextNode(eheader.name));
            metadata.AppendChild(node);

            //语言
            node = xml.CreateElement("dc", "language", " ");
            node.AppendChild(xml.CreateTextNode(CultureInfo.CurrentCulture.Name));
            metadata.AppendChild(node);

            node = xml.CreateElement("dc", "creator", " ");
            node.AppendChild(xml.CreateTextNode(eheader.author ?? string.Empty));
            metadata.AppendChild(node);

            //版权信息
            node = xml.CreateElement("dc", "publisher", " ");
            node.AppendChild(xml.CreateTextNode(eheader.publisher ?? string.Empty));
            metadata.AppendChild(node);

            //时间
            node = xml.CreateElement("dc", "date", " ");
            node.AppendChild(xml.CreateTextNode(ToTime(DateTime.UtcNow)));
            metadata.AppendChild(node);

            //简介
            node = xml.CreateElement("dc", "description", " ");
            if (string.IsNullOrEmpty(eheader.synopsis))
            {
                node.AppendChild(xml.CreateTextNode(string.Empty));
            }
            else
            {
                node.AppendChild(xml.CreateTextNode(eheader.synopsis.Replace("\r", "").Replace("\n", "")));
            }
            metadata.AppendChild(node);

            #endregion

            //const string rootEpub = "OEBPS/";

            #region 资源清单

            //OEBPS/text/ 规定小说内容
            //OEBPS/image/ 规定小说图像 -> 一比一还原easybook目录
            //OEBPS/cover.* 规定小说封面

            if (eheader.coverImage is object)
            {
                //封面

                var ext = Path.GetExtension(eheader.coverImage);
                if (ext is null) throw new ArgumentNullException();
                string medCov;
                switch (ext.ToLowerInvariant())
                {
                    case ".png":
                        medCov = "png";
                        break;
                    case ".jeg":
                    case ".jpeg":
                        medCov = "jpeg";
                        break;
                    case "bmp":
                        medCov = "bitmap";
                        break;
                    default:
                        throw new ArgumentException();
                }
                //封面索引使用 cover-image

                //<item id="cover-image" href="cover.[后缀]" media-type="image/[类型]">

                manifest.AppendChild(CreateManifestItemNode(xml,
                    "cover-image", (eheader.coverImage),
                    "image/" + medCov, null));

                //<item id="cover.html" href="cover.html" media-type="application/xhtml+xml"/>
                //封面显示文件也在OBS根目录

                manifest.AppendChild(CreateMftHtml(xml, "cover-file", "cover.html"));

            }

            //创建ncx
            manifest.AppendChild(CreateMftNCXNode(xml));

            //顺序遍历章节内容节点和文件
            /*
            规则:
            从 "OEBPS\text" 开始为根
            一比一获取 ebk 节点
            到达章节节点 -> *.txt <=> *.html
            */
            //ebk章节根目录
            var ebkVolRoot = easybook.VolumeRoot;

            ebkVolForeachRef ebf = default;
            ebf.doc = xml;
            ebf.idList = new List<string>();
            ebf.manifest = manifest;

            //深度遍历，创建章节节点和章节文件item

            foeachDepth(ebkVolRoot, ebf.ForeachEBKVolNode);

            #endregion

            #region 顺序文件

            if(eheader.coverImage is object)
            {
                //封面图（如果有）
                spine.AppendChild(CreateItemRef(xml, "cover-image"));
            }

            //顺序写下集合内的文件
            for (int i = 0; i < ebf.idList.Count; i++)
            {
                spine.AppendChild(CreateItemRef(xml, ebf.idList[i]));
            }

            #endregion

            root.AppendChild(metadata);
            root.AppendChild(manifest);
            root.AppendChild(spine);

            return xml;
        }

        static void foeachDepth<T>(TreeNode<T> tree, Action<TreeNode<T>> action)
        {

            action.Invoke(tree);
            int count = tree.Count;

            for (int i = 0; i < count; i++)
            {
                var treeNode = tree[i];
                action.Invoke(treeNode);
                //ForeachEBKVolNode(treeNode);

                if (treeNode.Count != 0)
                {
                    foeachDepth(treeNode, action);
                }
            }
        }

        private struct ebkVolForeachRef
        {

            public XmlDocument doc;
            public XmlElement manifest;
            public List<string> idList;

           
            public void ForeachEBKVolNode(TreeNode<Volume> node)
            {
                var volValue = node.Value;
                if (volValue.volumeType != VolumeType.Chapters && volValue.volumeType != VolumeType.Node)
                {
                    return;
                }

                if (node.Parent is null && volValue.volumeType == VolumeType.Node) return;

                string path;
                if(volValue.volumeType == VolumeType.Node)
                {
                    //章节节点根目录
                    path = getRootPath(node);
                    //if (path == "text") return;
                    //路径替换正斜杠
                    //var tpath = path.Replace('\\', '/');
                    var tpath = pathRepTo(path);
                    //tpath 是父级目录
                    //tpath = Path.Combine(tpath, volValue.index);

                    manifest.AppendChild(CreateMftHtml(doc, pathRepIndex(tpath), tpath + ".html"));
                    idList.Add(pathRepIndex(tpath));
                }
                else if(volValue.volumeType == VolumeType.Chapters)
                {
                    //章节
                    if(volValue.chapters is null) throw new ArgumentNullException();

                    //添加章节节点
                    path = getRootPath(node);
                    var tpath = path.pathRepTo();
                    manifest.AppendChild(CreateMftHtml(doc, pathRepIndex(tpath), tpath + ".html"));
                    idList.Add(pathRepIndex(tpath));
                    //顺序添加章节内容
                    for (int i = 0; i < volValue.chapters.Count; i++)
                    {
                        var ci = volValue.chapters[i];
                        if(ci.chapterFilePath is null) throw new ArgumentNullException();
                        path = ci.chapterFilePath.pathRepTo();
                        //创建路径文件
                        manifest.AppendChild(CreateMftHtml(doc, pathRepIndex(path), Path.ChangeExtension(path, ".html")));
                        idList.Add(pathRepIndex(path));
                    }

                }
            }

        }

        #region 资源清单类型

        /// <summary>
        /// 创建顺序文件
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="idref">资源清单引用</param>
        /// <returns></returns>
        static XmlElement CreateItemRef(XmlDocument doc, string idref)
        {
            //<itemref idref="cover.html"/>
            var item = doc.CreateElement("itemref");
            item.SetAttribute("idref", idref);
            return item;
        }

        /// <summary>
        /// 创建一个NCX资源清单item
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        static XmlElement CreateMftNCXNode(XmlDocument doc)
        {
            //<item id="ncx" href="toc.ncx" media-type="application/x-dtbncx+xml"/>
            var item = doc.CreateElement("item");
            item.SetAttribute("id", "ncx");
            item.SetAttribute("href", "toc.ncx");
            item.SetAttribute("media-type", "application/x-dtbncx+xml");
            return item;
        }

  
        /// <summary>
        /// 创建png资源清单节点
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <param name="href"></param>
        /// <returns></returns>
        static XmlElement CreateManifestPngImage(XmlDocument doc, string id, string href)
        {
            var item = doc.CreateElement("item");
            item.SetAttribute("id", id);
            item.SetAttribute("href", href);
            item.SetAttribute("media-type", "image/png");
            return item;
        }

        /// <summary>
        /// 创建jpg资源清单节点
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <param name="href"></param>
        /// <returns></returns>
        static XmlElement CreateManifestJpegImage(XmlDocument doc, string id, string href)
        {
            var item = doc.CreateElement("item");
            item.SetAttribute("id", id);
            item.SetAttribute("href", href);
            item.SetAttribute("media-type", "image/jpeg");
            return item;
        }

        /// <summary>
        /// 创建bmp资源清单节点
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <param name="href"></param>
        /// <returns></returns>
        static XmlElement CreateManifestBmpImage(XmlDocument doc, string id, string href)
        {
            var item = doc.CreateElement("item");
            item.SetAttribute("id", id);
            item.SetAttribute("href", href);
            item.SetAttribute("media-type", "image/bitmap");
            return item;
        }

        /// <summary>
        /// 创建html资源清单
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <param name="href"></param>
        /// <returns></returns>
        static XmlElement CreateMftHtml(XmlDocument doc, string id, string href)
        {
            var item = doc.CreateElement("item");
            item.SetAttribute("id", id);
            item.SetAttribute("href", href);
            item.SetAttribute("media-type", "application/xhtml+xml");
            return item;
        }

        /// <summary>
        /// 创建一个资源清单节点
        /// </summary>
        /// <param name="doc">文件</param>
        /// <param name="id">清单id</param>
        /// <param name="href">href引用</param>
        /// <param name="media_type">media-type；null表示无此属性</param>
        /// <param name="properties">properties属性；null表示无此属性</param>
        /// <returns></returns>
        static XmlElement CreateManifestItemNode(XmlDocument doc, string id, string href, string media_type, string properties)
        {
            var item = doc.CreateElement("item");
            item.SetAttribute("id", id);
            item.SetAttribute("href", href);
            if (media_type is object) item.SetAttribute("media-type", media_type);
            if (properties is object) item.SetAttribute("properties", properties);
            return item;
        }

        #endregion

        static string ToTime(DateTime time)
        {
            return $"{time.Year}-{time.Month}-{time.Day}";
        }

        #endregion

        #region ncx

        private struct NCX_Data
        {

            public XmlElement CreateNCX_head()
            {
                var head = doc.CreateElement("head");
                /*
                <meta name="dtb:uid" content="urn:uuid:xxx-78933444"/>
                <meta name="dtb:depth" content="4"/>
                <meta name="dtb:totalPageCount" content="0"/>
                <meta name="dtb:maxPageNumber" content="0"/>
                */
                var meta = doc.CreateElement("meta");
                meta.SetAttribute("name", "dtb:uid");
                meta.SetAttribute("content", "urn:uuid:xxx-78933444");
                head.AppendChild(meta);

                meta = doc.CreateElement("meta");
                meta.SetAttribute("name", "dtb:depth");
                meta.SetAttribute("content", "4");
                head.AppendChild(meta);

                meta = doc.CreateElement("meta");
                meta.SetAttribute("name", "dtb:totalPageCount");
                meta.SetAttribute("content", "0");
                head.AppendChild(meta);

                meta = doc.CreateElement("meta");
                meta.SetAttribute("name", "dtb:maxPageNumber");
                meta.SetAttribute("content", "0");
                head.AppendChild(meta);

                return head;
            }

            public XmlDocument doc;
            public EasyBook ebk;
            public long navPointCount;

            public XmlElement CreateNCX_navMap()
            {
                navPointCount = 1;

                //var maps = CreateNavpoit(ebk.VolumeRoot);

                var root = ebk.VolumeRoot;
                var ms = doc.CreateElement("navMap");

                var vol = root.Value;
                if (vol.volumeType == VolumeType.Node)
                {

                    for (int i = 0; i < root.Count; i++)
                    {
                        ms.AppendChild(CreateNavpoit(root[i]));
                    }

                }
                else if (vol.volumeType == VolumeType.Chapters)
                {
                    ms.AppendChild(CreateNavpoit(root));
                }

                return ms;
            }

            XmlElement CreateNavpoit(TreeNode<Volume> ebkVol)
            {
                /*
                <navPoint> 包含
                <navLabel>节点名<navLabel/>
                引用数据 <content src="text/00003.html" />

                子类 <navPoint> 节点（可选）
                */
                var volValue = ebkVol.Value;

                var navPoint = doc.CreateElement("navPoint");

                int i;
                var label = doc.CreateElement("navLabel");
                //分卷名
                var labT = doc.CreateElement("text");
                labT.AppendChild(doc.CreateTextNode(volValue.name));
                label.AppendChild(labT);
                navPoint.AppendChild(label);

                string rootPath;
                if (volValue.volumeType == VolumeType.Node)
                {
                    //是分卷节点
                    rootPath = getRootPath(ebkVol);
                    rootPath = rootPath?.pathRepTo();
                    if (rootPath is null) throw new ArgumentNullException();
                    navPoint.SetAttribute("id", pathRepIndex(rootPath));
                    navPoint.SetAttribute("playOrder", navPointCount.ToString());


                    rootPath = rootPath + ".html";
                    if(rootPath != "text.html")
                    {
                        var volCont = doc.CreateElement("content");
                        volCont.SetAttribute("src", rootPath);
                        navPoint.AppendChild(volCont);
                    }

                    navPointCount++;
                    //分卷节点
                    for (i = 0; i < ebkVol.Count; i++)
                    {
                        //创建子节点
                        var snav = CreateNavpoit(ebkVol[i]);
                        navPoint.AppendChild(snav);
                    }
                }
                else if (volValue.volumeType == VolumeType.Chapters)
                {
                    if (volValue.chapters is null) throw new ArgumentNullException();
                    //章节分卷
                    //写入
                    rootPath = getRootPath(ebkVol);
                    rootPath = rootPath.pathRepTo();
                    if (rootPath is null) throw new ArgumentNullException();
                    navPoint.SetAttribute("id", pathRepIndex(rootPath));
                    navPoint.SetAttribute("playOrder", navPointCount.ToString());

                    rootPath = rootPath + ".html";
                    if (rootPath != "text.html")
                    {
                        var volCont = doc.CreateElement("content");
                        volCont.SetAttribute("src", rootPath);
                        navPoint.AppendChild(volCont);
                    }

                    navPointCount++;
                    for (i = 0; i < volValue.chapters.Count; i++)
                    {
                        navPoint.AppendChild(createVolNav(volValue.chapters[i]));
                    }
                }

                return navPoint;
            }

            XmlElement createVolNav(ChapterIndex ci)
            {
                var navPoint = doc.CreateElement("navPoint");
                var label = doc.CreateElement("navLabel");
                navPoint.SetAttribute("id", pathRepIndex(ci.chapterFilePath?.pathRepTo()));
                navPoint.SetAttribute("playOrder", navPointCount.ToString());
                
                
                var lttx = doc.CreateElement("text");
                lttx.AppendChild(doc.CreateTextNode(ci.title));
                label.AppendChild(lttx);

                var cont = doc.CreateElement("content");
                var tpath = ci.chapterFilePath?.pathRepTo();
                if (tpath is null) throw new ArgumentNullException();
                tpath = Path.ChangeExtension(tpath, ".html");
                cont.SetAttribute("src", tpath);
                
                navPoint.AppendChild(label);
                navPoint.AppendChild(cont);

                navPointCount++;

                return navPoint;
            }

        }

        /// <summary>
        /// 从easybook创建 epub 的 ncx 文件
        /// </summary>
        /// <param name="easybook"></param>
        /// <returns></returns>
        public static XmlDocument CreateNCX(EasyBook easybook)
        {
            XmlDocument doc = new XmlDocument();

            var dr = doc.CreateDocumentType("ncx", "-//NISO//DTD ncx 2005-1//EN", "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd", null);

            doc.AppendChild(dr);
            var ncx = doc.CreateElement("ncx", "http://www.daisy.org/z3986/2005/ncx/");
            doc.AppendChild(ncx);

            ncx.SetAttribute("version", "2005-1");

            var header = easybook.Header;

            #region ncx

            NCX_Data ncxdata;
            ncxdata.doc = doc;
            ncxdata.ebk = easybook;
            ncxdata.navPointCount = 0;
            var head = ncxdata.CreateNCX_head();

            var docTitle = doc.CreateElement("docTitle");
            var docTitle_text = doc.CreateElement("text");
            docTitle_text.AppendChild(doc.CreateTextNode(header.name));
            docTitle.AppendChild(docTitle_text);

            var navMap = ncxdata.CreateNCX_navMap();

            ncx.AppendChild(head);
            ncx.AppendChild(docTitle);
            ncx.AppendChild(navMap);
            #endregion

            return doc;
        }

        #endregion

        #endregion

    }

}
