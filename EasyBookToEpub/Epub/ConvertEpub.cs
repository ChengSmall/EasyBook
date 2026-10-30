using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.IO.Compression;
using System.Globalization;

using Cheng.Algorithm.Trees;
using Cheng.Algorithm.Collections;
using Cheng.Algorithm.Compressions;
using Cheng.DataStructure.Collections;
using Cheng.DataStructure;
using Cheng.Algorithm.Compressions.ZipPackagings;
using Cheng.DataStructure.Uuids;
using Cheng.Algorithm.Randoms;
using Cheng.Texts;

namespace Cheng.EasyBooks.ToEpubs
{

    /// <summary>
    /// 将easybook转换为epub的简单方法
    /// </summary>
    /// <exception cref="Exception">任何方法有可能引发任何异常，请确保参数正确</exception>
    public static class ConvertEpub
    {

        #region 创建epub结构

        #region 常量

        /// <summary>
        /// 路径分隔符
        /// </summary>
        public const char pathSeparator = '/';

        internal const string EbkToEpubRootPtah = "OEBPS";

        internal const string EbkToEpubTextPtah = "OEBPS/text";

        internal const string EbkToEpubImagePtah = "OEBPS/image";

        internal const string EpubChapterFileExtension = ".xhtml";

        internal static string EpubSynopsisText => "synopsis";

        internal const string EpubXHtmlRootNamespace = "http://www.w3.org/1999/xhtml";

        #endregion

        /// <summary>
        /// 从ebk创建epub结构到zip
        /// </summary>
        /// <param name="ebk">easybook</param>
        /// <param name="zip">zip打包器</param>
        /// <param name="outputLog" >输出日志，null表示没有</param>
        public static void CreateEpubByEbk(EasyBook ebk, ZipPackagingOperation zip, TextWriter outputLog)
        {
            if (ebk is null || zip is null) throw new ArgumentNullException();
            if (!ebk.IsNotDispose) throw new ObjectDisposedException("ebk");
            bool print = outputLog != null;

            if(print) outputLog.WriteLine("正在创建基础epub结构");
            CreateEpubZip(zip);

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
            if (!string.IsNullOrEmpty(ebk.Header.coverImage)) imageCount++;

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
                    imgNode.SetAttribute("alt", "imageLine:" + imgPath);
                    body.AppendChild(imgNode);
                }

            }

            return xml;
        }

        /// <summary>
        /// 创建一个epub章节文件
        /// </summary>
        /// <param name="chapter">要创建的ebk章节流</param>
        /// <param name="ci">与之对应的章节头</param>
        /// <returns></returns>
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
                    imgNode.SetAttribute("alt", "imageLine");
                    body.AppendChild(imgNode);
                }
            }

            return xml;
        }

        /// <summary>
        /// 为zip创建Epub基础结构
        /// </summary>
        /// <param name="zip">空zip</param>
        public static void CreateEpubZip(ZipPackagingOperation zip)
        {
            //创建头
            var bs = Encoding.ASCII.GetBytes("application/epub+zip");
            zip.SetDataItem(new GettingMemoryStream(bs), "mimetype", ZipItemPackType.Stored);

            //创建 container
            f_createEpubZipNext(zip);
        }

        static void f_createEpubZipNext(ZipPackagingOperation zip)
        {
            zip.SetDataItem(new GettingMemoryStream(CreateContainerBytes()), "META-INF/container.xml", ZipItemPackType.Deflate);
        }

        /// <summary>
        /// 创建一个epub头的zip包
        /// </summary>
        /// <param name="stream"></param>
        public static void CreateEpubHeader(Stream stream)
        {
            var zippack = new ZipPackagingOperation();
            var getbytes = new GettingMemoryStream(Encoding.ASCII.GetBytes("application/epub+zip"));
            zippack.SetDataItem(getbytes, "application/epub+zip", ZipItemPackType.Stored);
            zippack.PackTo(stream);
        }

        /// <summary>
        /// （epub基础结构）为epub创建 META-INF/container.xml
        /// </summary>
        /// <param name="zip"></param>
        public static void CreateContainer(ZipPackagingOperation zip)
        {
            if (zip is null) throw new ArgumentNullException();
            zip.SetDataItem(new GettingMemoryStream(CreateContainerBytes()), "META-INF/container.xml", ZipItemPackType.Deflate);
        }

        /// <summary>
        /// （epub基础结构）为epub包从ebk创建opf（<see cref="CreateContainer(ZipPackagingOperation)"/>之后调用）
        /// </summary>
        /// <param name="zip">epub包</param>
        /// <param name="easybook">ebk</param>
        public static void CreateEpubOPF(ZipPackagingOperation zip, EasyBook easybook)
        {
            if (zip is null || easybook is null) throw new ArgumentNullException();

            var gett = new GettingXmlDocumentStream(CreateContent(easybook));
            zip.SetDataItem(gett, "OEBPS/content.opf", ZipItemPackType.Deflate);
        }


        /// <summary>
        /// （epub基础结构）为epub创建 META-INF/toc.ncx（<see cref="CreateEpubOPF(ZipArchive, EasyBook)"/>后调用）
        /// </summary>
        /// <param name="zip"></param>
        /// <param name="ebk"></param>
        public static void CreateTocEpub(ZipPackagingOperation zip, EasyBook ebk)
        {
            if (zip is null || ebk is null) throw new ArgumentNullException();

            var getter = new GettingXmlDocumentStreamFromEasyBook(CreateNCX, ebk);
            zip.SetDataItem(getter, "OEBPS/toc.ncx", ZipItemPackType.Deflate);
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
            if (title is null) title = string.Empty;

            XmlDocument doc = new XmlDocument();

            var docType = doc.CreateDocumentType("html", "-//W3C//DTD XHTML 1.1//EN", "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd", null);

            doc.AppendChild(docType);

            var root = doc.CreateElement("html");
            
            //root.SetAttribute("xmlns", "http://www.w3.org/1999/xhtml");

            doc.AppendChild(root);

            var head = doc.CreateElement("head");
            var body = doc.CreateElement("body");

            root.AppendChild(head);
            root.AppendChild(body);

            var titleNode = doc.CreateElement("title");
            titleNode.AppendChild(doc.CreateTextNode(title));
            head.AppendChild(titleNode);

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
        public static void CreateEpubChapters(ZipPackagingOperation zip, EasyBook ebk, Action<ChapterIndex> createChapterAction, Action<Volume> createVolumeAction)
        {

            if (zip is null || ebk is null) throw new ArgumentNullException();
            var vols = ebk.VolumeRoot;

            const string rootPath = EbkToEpubRootPtah;

            {
                var ebkh = ebk.Header;
                var xmlDoc = CreateEpubChapterHtml(ebkh.name, out var body);

                if (!string.IsNullOrEmpty(ebkh.coverImage))
                {
                    //插入封面页
                    //<img src="OEBPS/image/"/>

                    var imgNode = xmlDoc.CreateElement("img");
                    //封面路径获取
                    var imgFilePath = ebkh.coverImage?.pathRepTo();
                    // epub归档内全局路径
                    var epubImgFullPath = Path.Combine(rootPath, imgFilePath);

                    // 从OEBPS目录到封面归档目录的相对位置路径
                    var textToImgPath = TextManipulation.Path.GetRelativeDirectoryByPath(rootPath, epubImgFullPath);
                    imgNode.SetAttribute("src", textToImgPath);
                    imgNode.SetAttribute("alt", "coverImage");
                    body.InsertAfter(imgNode, body.FirstChild);

                    var xmlGetter = new GettingXmlDocumentStream(xmlDoc);
                    zip.SetDataItem(xmlGetter, Path.Combine(rootPath, "cover" + EpubChapterFileExtension));

                }

                if (!string.IsNullOrEmpty(ebkh.synopsis))
                {
                    using (var reader = new NewLineStdTextReader(new StringReader(ebkh.synopsis), true, "\n"))
                    {
                        {
                            var np = xmlDoc.CreateElement("p");
                            np.AppendChild(xmlDoc.CreateTextNode(EpubSynopsisText + ":"));
                            body.AppendChild(np);
                        }
                        while (true)
                        {
                            var line = reader.ReadLine();
                            if (line is null) break;
                            var np = xmlDoc.CreateElement("p");
                            np.AppendChild(xmlDoc.CreateTextNode(line));
                            body.AppendChild(np);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(ebkh.publisher))
                {
                        var np = xmlDoc.CreateElement("p");
                        np.AppendChild(xmlDoc.CreateTextNode("© " + ebkh.publisher));
                        body.AppendChild(np);
                }

                if (!string.IsNullOrEmpty(ebkh.author))
                {
                    var np = xmlDoc.CreateElement("p");
                    np.AppendChild(xmlDoc.CreateTextNode("--- " + ebkh.author));
                    body.AppendChild(np);
                }

            }

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

                //节点分卷
                path = getRootPath(t_node);

                //属于根节点
                bool isRootPath = path == "text";
                //if (isRootPath) return;

                //路径替换 + 扩展名
                var tpath = path.pathRepTo() + EpubChapterFileExtension;
                tpath = Path.Combine(rootPath, tpath).pathRepTo();

                //创建目录html文件
                var volXml = CreateEpubChapterHtml(volValue.name, out body);


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

                var xmlGetter = new GettingXmlDocumentStream(volXml);
                zip.SetDataItem(xmlGetter, tpath);

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
                        tpath = Path.ChangeExtension(tpath, EpubChapterFileExtension);

                        using (var openC = ebk.CreateChapterStream(volValue.chapters[i]))
                        {
                            volXml = CreateEpubChapter(openC, volValue.chapters[i]);
                        }

                        xmlGetter = new GettingXmlDocumentStream(volXml);
                        zip.SetDataItem(xmlGetter, tpath);

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
        public static void CreateEpubImages(ZipPackagingOperation zip, EasyBook ebk, Action<TreeNode<TreeNodeData>> createImageAction)
        {
            if (zip is null || ebk is null) throw new ArgumentNullException();
            var vols = ebk.VolumeRoot;

            const string rootPath = EbkToEpubRootPtah;

            var header = ebk.Header;
            var pack = ebk.BaseDataPack;

            if (!string.IsNullOrEmpty(header.coverImage))
            {
                //存在封面
                var coverImgPath = Path.Combine(rootPath, header.coverImage).pathRepTo();

                zip.SetDataItem(new CompressionParserOpenStreamByPath(pack, header.coverImage), coverImgPath);

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

            bool actionTreeImage(TreeNode<TreeNodeData> t_treeNode)
            {
                var volValue = t_treeNode.Value;

                if (!volValue.isFile) return true; //不是文件

                if(volValue.dataPath is null) return false;
                //创建epub节点
                var treeAcPath = Path.Combine(rootPath, volValue.dataPath).pathRepTo();

                zip.SetDataItem(new CompressionParserOpenStreamByPath(pack, volValue.dataPath), treeAcPath);

                createImageAction?.Invoke(t_treeNode);
                return true;
            }

        }

        #endregion

        #region container.xml

        static byte[] CreateContainerBytes()
        {
            var sb = new CMStringBuilder(256 + 64);
            sb
.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>").Append('\n')
.Append("<container version=\"1.0\" xmlns=\"urn:oasis:names:tc:opendocument:xmlns:container\">").Append('\n')
.Append("   <rootfiles>").Append('\n')
.Append("      <rootfile full-path=\"OEBPS/content.opf\" media-type=\"application/oebps-package+xml\"/>").Append('\n')
.Append("   </rootfiles>").Append('\n')
.Append("</container>")
;
            var cbuf = sb.GetCharBuffer();
            return Encoding.UTF8.GetBytes(cbuf.Array, cbuf.Offset, cbuf.Count);
        }

        #endregion

        #region content.opf

        /// <summary>
        /// 通过easybook创建一个content.opf表单
        /// </summary>
        /// <param name="easybook"></param>
        /// <returns></returns>
        public static XmlDocument CreateContent(EasyBook easybook)
        {
            XmlDocument xml = new XmlDocument();

            const string rootNameSpace = "http://www.idpf.org/2007/opf";

            #region
            var root = xml.CreateElement("package", rootNameSpace);

            root.SetAttribute("unique-identifier", "chengsmall-easybook-convert-epub");
            root.SetAttribute("version", "2.0");
            root.SetAttribute("xmlns", rootNameSpace);
            xml.AppendChild(root);
            //数据头
            var metadata = xml.CreateElement("metadata");
            //资源清单
            var manifest = xml.CreateElement("manifest");
            //阅读顺序
            var spine = xml.CreateElement("spine");
            spine.SetAttribute("toc", "ncx");

            root.AppendChild(metadata);
            root.AppendChild(manifest);
            root.AppendChild(spine);

            #endregion

            #region metadata

            var eheader = easybook.Header;
            // <dc:identifier id="bookID" opf:scheme="UUID">UUID</dc:identifier>
            byte[] uuidBuf = new byte[16];
            XmlElement node;
            var rand = new XorshiftRandom(Environment.TickCount - DateTime.Now.Ticks);
            rand.NextBytes(uuidBuf);
            node = xml.CreateElement("dc", "identifier", rootNameSpace);
            node.SetAttribute("id", "chengsmall-easybook-convert-epub");
            node.SetAttribute("opf:scheme", "UUID");
            node.AppendChild(xml.CreateTextNode("urn:uuid:" + Uuid.CreateFromBytes(uuidBuf).SetToUUIDv4().ToString(false)));
            metadata.AppendChild(node);

            // <dc:title xmlns:dc=" ">easybook介绍和标准定义</dc:title>
            //书名
            node = xml.CreateElement("dc", "title", " ");
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
            node.AppendChild(xml.CreateTextNode(ToTime(DateTime.Now)));
            metadata.AppendChild(node);

            //简介
            node = xml.CreateElement("dc", "description", " ");
            if (string.IsNullOrEmpty(eheader.synopsis))
            {
                node.AppendChild(xml.CreateTextNode(string.Empty));
            }
            else
            {
                if (eheader.synopsis.IndexOf('\n') < 0)
                {
                    node.AppendChild(xml.CreateTextNode(eheader.synopsis));
                }
                else
                {
                    using (var sread = new StringReader(eheader.synopsis))
                    {
                        using (var nread = new Cheng.Texts.NewLineStdTextReader(sread, false))
                        {
                            while (true)
                            {
                                var line = nread.ReadLine();
                                if (line is null) break;
                                var p = xml.CreateElement("p");
                                p.AppendChild(xml.CreateTextNode(line));
                            }
                        }
                        

                    }
                }
              
            }
            metadata.AppendChild(node);

            #endregion

            #region 资源清单

            //OEBPS/text/ 规定小说内容
            //OEBPS/image/ 规定小说图像 -> 一比一还原easybook目录
            //OEBPS/cover.* 规定小说封面

            if (!string.IsNullOrEmpty(eheader.coverImage))
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
                    case ".jpg":
                    case ".jpeg":
                        medCov = "jpeg";
                        break;
                    case ".bmp":
                        medCov = "bitmap";
                        break;
                    default:
                        throw new ArgumentException();
                }
                //封面索引使用 cover-image

                //<item id="cover-image" href="cover.[后缀]" media-type="image/[类型]">
                var imgPath = (eheader.coverImage?.pathRepTo());
                manifest.AppendChild(CreateManifestItemNode(xml,
                    "cover-image", imgPath,
                    "image/" + medCov, null));

                //<item id="cover.html" href="cover.html" media-type="application/xhtml+xml"/>
                //封面显示文件也在OBS根目录

                manifest.AppendChild(CreateMftHtml(xml, "cover-file", "cover" + EpubChapterFileExtension));

            }

            //创建ncx
            manifest.AppendChild(CreateMftNCXNode(xml));

            //顺序遍历章节内容节点和文件
            /*
            规则:
            从 "OEBPS\text" 开始为根
            一比一获取 ebk 节点
            到达章节节点 -> *.txt <=> *.xhtml
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

            if(!string.IsNullOrEmpty(eheader.coverImage))
            {
                //封面图（如果有）
                spine.AppendChild(CreateItemRef(xml, "cover-file"));
            }

            //顺序写下集合内的文件
            for (int i = 0; i < ebf.idList.Count; i++)
            {
                spine.AppendChild(CreateItemRef(xml, ebf.idList[i]));
            }

            #endregion

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
                    var tpath = pathRepTo(path);
                    //tpath 是父级目录

                    manifest.AppendChild(CreateMftHtml(doc, pathRepIndex(tpath), tpath + EpubChapterFileExtension));
                    idList.Add(pathRepIndex(tpath));
                }
                else if(volValue.volumeType == VolumeType.Chapters)
                {
                    //章节
                    if(volValue.chapters is null) throw new ArgumentNullException();

                    //添加章节节点
                    path = getRootPath(node);
                    var tpath = path.pathRepTo();
                    manifest.AppendChild(CreateMftHtml(doc, pathRepIndex(tpath), tpath + EpubChapterFileExtension));
                    idList.Add(pathRepIndex(tpath));
                    //顺序添加章节内容
                    for (int i = 0; i < volValue.chapters.Count; i++)
                    {
                        var ci = volValue.chapters[i];
                        if(ci.chapterFilePath is null) throw new ArgumentNullException();
                        path = ci.chapterFilePath.pathRepTo();
                        //创建路径文件
                        manifest.AppendChild(CreateMftHtml(doc, pathRepIndex(path), Path.ChangeExtension(path, EpubChapterFileExtension)));
                        idList.Add(pathRepIndex(path));
                    }

                }
            }

        }

        #region 资源清单类型

#if DEBUG
        /// <summary>
        /// 创建顺序文件
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="idref">资源清单引用</param>
        /// <returns></returns>
#endif
        static XmlElement CreateItemRef(XmlDocument doc, string idref)
        {
            //<itemref idref="cover.html"/>
            var item = doc.CreateElement("itemref");
            item.SetAttribute("idref", idref);
            return item;
        }

#if DEBUG
        /// <summary>
        /// 创建一个NCX资源清单item
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
#endif
        static XmlElement CreateMftNCXNode(XmlDocument doc)
        {
            //<item id="ncx" href="toc.ncx" media-type="application/x-dtbncx+xml"/>
            var item = doc.CreateElement("item", "http://www.idpf.org/2007/opf");
            item.SetAttribute("id", "ncx");
            item.SetAttribute("href", "toc.ncx");
            item.SetAttribute("media-type", "application/x-dtbncx+xml");
            return item;
        }

#if DEBUG
        /// <summary>
        /// 创建html资源清单
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <param name="href"></param>
        /// <returns></returns>
#endif
        static XmlElement CreateMftHtml(XmlDocument doc, string id, string href)
        {
            var item = doc.CreateElement("item");
            item.SetAttribute("id", id);
            item.SetAttribute("href", href);
            item.SetAttribute("media-type", "application/xhtml+xml");
            return item;
        }

#if DEBUG
        /// <summary>
        /// 创建一个资源清单节点
        /// </summary>
        /// <param name="doc">文件</param>
        /// <param name="id">清单id</param>
        /// <param name="href">href引用</param>
        /// <param name="media_type">media-type；null表示无此属性</param>
        /// <param name="properties">properties属性；null表示无此属性</param>
        /// <returns></returns>
#endif
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

            internal XmlDocument doc;
            internal EasyBook ebk;
            internal long navPointCount;
            internal int maxDepValue;

            private void f_foreachDepVol(TreeNode<Volume> node, int nowDep)
            {
                var nv = node.Value;
                if(nv.volumeType != VolumeType.Node)
                {
                    maxDepValue = Math.Max(maxDepValue, nowDep);
                    return;
                }
                var length = node.Count;
                for (int i = 0; i < length; i++)
                {
                    var sn = node[i];
                    f_foreachDepVol(sn, nowDep + 1);
                }
            }

            private void f_getDepVolume()
            {
                var root = ebk.VolumeRoot;

                var rv = root.Value;
                if (rv.volumeType != VolumeType.Node)
                {
                    // 不存在嵌套
                    maxDepValue = 1;
                    return;
                }

                f_foreachDepVol(root, 1);
            }

            internal XmlElement CreateNCX_head()
            {
                var head = doc.CreateElement("head");
                /*
                <meta name="dtb:uid" content="urn:uuid:xxx-78933444"/>
                <meta name="dtb:depth" content="最大嵌套深度"/>
                <meta name="dtb:totalPageCount" content="0"/>
                <meta name="dtb:maxPageNumber" content="0"/>
                */
                var meta = doc.CreateElement("meta");
                meta.SetAttribute("name", "dtb:uid");
                meta.SetAttribute("content", "urn:uuid:xxx-78933444");
                head.AppendChild(meta);
                maxDepValue = 1;
                f_getDepVolume();

                meta = doc.CreateElement("meta");
                meta.SetAttribute("name", "dtb:depth");
                meta.SetAttribute("content", maxDepValue.ToString());
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

            internal XmlElement CreateNCX_navMap()
            {
                var root = ebk.VolumeRoot;
                var ms = doc.CreateElement("navMap");

                {
                    /*
                    <navPoint id="cover-file" playOrder="1">
                        <navLabel>
                            <text>标题</text>
                        </navLabel>
                        <content src="cover.xhtml" />
                    </navPoint>
                    */

                    var navPoint = doc.CreateElement("navPoint");
                    navPoint.SetAttribute("id", "cover-file");
                    navPoint.SetAttribute("playOrder", "1");

                    var label = doc.CreateElement("navLabel");
                    //分卷名
                    var labT = doc.CreateElement("text");
                    labT.AppendChild(doc.CreateTextNode(ebk.Header.name));
                    label.AppendChild(labT);
                    navPoint.AppendChild(label);

                    var ct = doc.CreateElement("content");
                    ct.SetAttribute("src", "cover" + EpubChapterFileExtension);
                    navPoint.AppendChild(ct);

                    ms.AppendChild(navPoint);

                    navPointCount = 2;
                }

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
                    navPoint.SetAttribute("playOrder", (navPointCount).ToString());

                    rootPath = rootPath + EpubChapterFileExtension;
                    if(rootPath != ("text" + EpubChapterFileExtension))
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
                    
                    rootPath = rootPath + EpubChapterFileExtension;
                    if (rootPath != ("text" + EpubChapterFileExtension))
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
                navPoint.SetAttribute("playOrder", (navPointCount).ToString());
                
                
                var lttx = doc.CreateElement("text");
                lttx.AppendChild(doc.CreateTextNode(ci.title));
                label.AppendChild(lttx);

                var cont = doc.CreateElement("content");
                var tpath = ci.chapterFilePath?.pathRepTo();
                if (tpath is null) throw new ArgumentNullException();
                tpath = Path.ChangeExtension(tpath, EpubChapterFileExtension);
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
            const string rootNameSpace = "http://www.daisy.org/z3986/2005/ncx/";
            var dr = doc.CreateDocumentType("ncx", "-//NISO//DTD ncx 2005-1//EN", "http://www.daisy.org/z3986/2005/ncx-2005-1.dtd", null);

            doc.AppendChild(dr);
            var ncx = doc.CreateElement("ncx", rootNameSpace);
            doc.AppendChild(ncx);

            ncx.SetAttribute("version", "2005-1");
            ncx.SetAttribute("xml", "lang", "en");

            var header = easybook.Header;

            #region ncx

            NCX_Data ncxdata = default;
            ncxdata.doc = doc;
            ncxdata.ebk = easybook;
            ncxdata.navPointCount = 0;
            ncxdata.maxDepValue = 1;
            var head = ncxdata.CreateNCX_head();

            var docTitle = doc.CreateElement("docTitle");

            {
                var docTitle_text = doc.CreateElement("text");
                docTitle_text.AppendChild(doc.CreateTextNode(header.name));
                docTitle.AppendChild(docTitle_text);
            }

            var navMap = ncxdata.CreateNCX_navMap();

            ncx.AppendChild(head);
            ncx.AppendChild(docTitle);
            ncx.AppendChild(navMap);
            #endregion

            return doc;
        }

        #endregion

        #region 封装

#if DEBUG
        /// <summary>
        /// 分卷查询根目录
        /// </summary>
        /// <param name="volNode"></param>
        /// <returns></returns>
#endif
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

#if DEBUG
        /// <summary>
        /// 路径替换 - 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
#endif
        static string pathRepIndex(string path)
        {
            return path?.Replace('\\', '-')?.Replace('/', '-');
        }

#if DEBUG
        /// <summary>
        /// 路径兼容性替换
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
#endif
        static string pathRepTo(this string path)
        {
            return path?.Replace('\\', pathSeparator);
        }

        #endregion

        #endregion

    }

}
