using Cheng.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cheng.Algorithm;
using Cheng.Algorithm.Compressions;
using Cheng.Algorithm.Collections;
using Cheng.IO;
using System.IO;
using System.IO.Compression;
using Cheng.Algorithm.Compressions.Systems;
using Cheng.DataStructure.Collections;
using Cheng.Algorithm.Trees;
using Cheng.EasyBooks;

using PreValue = Cheng.EasyBooks.WinForms.PreferencesWindow.PreferencesValue;

using CTNodeV = Cheng.DataStructure.Collections.TreeNode<Cheng.EasyBooks.Volume>;
using CTNodeD = Cheng.DataStructure.Collections.TreeNode<Cheng.Algorithm.Trees.TreeNodeData>;

using STreeNode = System.Windows.Forms.TreeNode;
using System.Reflection;

namespace Cheng.EasyBooks.WinForms
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            f_lastInit();
            InitializeComponent();
            f_init();
        }

        #region

        #region 初始化

        private void f_lastInit()
        {
            p_easyBook = null;
            p_jsonParser = new JsonParserDefault();
            p_StreamBuffer = new MemoryStream();
        }

        private void f_init()
        {
            f_init_Par();
            f_init_eventInit();
            f_init_readWinStyle();
        }

        private void f_init_Par()
        {
            #region init

            p_preferencesWindow = new PreferencesWindow();

            #endregion
        }

        private void f_init_eventInit()
        {
            Col_MenuItem_File_Exit.Click += fe_Click_ExitButton;

            Col_Button_Test.Click += fe_Click_Button_Test;

            Col_TreeView_VolumeTree.NodeMouseDoubleClick += fe_DoubleClick_TreeNode;

            Col_MenuItem_File_Open.Click += fe_OpenFile;

            Col_MenuItem_File_Inf.Click += fe_PrintHeader;

            Col_MenuItem_Tool_Preferences.Click += fe_Click_Menu_Preferences;
            
            Col_MenuItem_Help_About.Click += fe_Click_MenuItem_About;

            Col_MenuItem_Tool_PreSaveTo.Click += fe_PreToFiles;
            Col_MenuItem_Tool_OpenFileToPre.Click += fe_OpenPreFile;

            Col_MenuItem_File_SaveTo.Click += fe_Click_Menu_SaveToEPUB;

            Col_Rtf_BookText.PreviewKeyDown += fe_KeyDown_Rtf;
        }

        private void f_init_readWinStyle()
        {
            if (f_settingReaderToPreferences())
            {
                printl("从配置文件中读取");
            }
            else
            {
                //没有配置文件

                var lit = f_readSysWinStyle();

                if (!lit)
                {
                    //暗色模式
                    PreValue value = new PreValue();
                    value.rtfFont = null;
                    value.volumeFont = null;
                    value.rtfTextColor = Color.FromArgb(220, 220, 220);
                    value.volumeTextColor = value.rtfTextColor;
                    value.rtfColor = Color.FromArgb(30, 30, 30);
                    value.volumeColor = Color.FromArgb(37, 37, 38);
                    value.backColor = Color.FromArgb(45, 45, 48);

                    SetPreferences(value);
                    printl("使用暗色主题初始化");
                }
                else
                {
                    printl("使用亮色主题初始化");
                }

            }

        }

        #endregion

        #region 参数

        private EasyBook p_easyBook;

        private JsonParserDefault p_jsonParser;

        private MemoryStream p_StreamBuffer;

        private PreferencesWindow p_preferencesWindow;

        #endregion

        #region 控件访问

        #region 菜单栏

        #region 文件菜单

        /// <summary>
        /// 菜单选项 - 文件 - 退出
        /// </summary>
        private ToolStripMenuItem Col_MenuItem_File_Exit
        {
            get => col_MenuItem_File_Exit;
        }

        /// <summary>
        /// 菜单选项 - 文件 - 打开
        /// </summary>
        private ToolStripMenuItem Col_MenuItem_File_Open
        {
            get => col_MenuItem_File_Open;
        }

        /// <summary>
        /// 菜单选项 - 文件 - 查看
        /// </summary>
        private ToolStripMenuItem Col_MenuItem_File_Inf
        {
            get => col_MenuItem_File_LookInf;
        }

        /// <summary>
        /// 菜单选项 - 文件 - 另存为EPUB
        /// </summary>
        private ToolStripMenuItem Col_MenuItem_File_SaveTo
        {
            get => col_MenuItem_File_Another;
        }

        #endregion

        #region 工具菜单

        /// <summary>
        /// 菜单选项 - 工具 - 首选项
        /// </summary>
        private ToolStripMenuItem Col_MenuItem_Tool_Preferences
        {
            get => col_MenuItem_Tool_Preferences;
        }

        /// <summary>
        /// 菜单选项 - 工具 - 导出设置
        /// </summary>
        private ToolStripMenuItem Col_MenuItem_Tool_PreSaveTo
        {
            get => col_MenuItem_Tool_PreSaveTo;
        }

        /// <summary>
        /// 菜单选项 - 工具 - 导入设置
        /// </summary>
        private ToolStripMenuItem Col_MenuItem_Tool_OpenFileToPre
        {
            get => col_MenuItem_Tool_PreRead;
        }

        #endregion

        #region 帮助菜单

        /// <summary>
        /// 菜单选项 - 帮助 - 关于
        /// </summary>
        public ToolStripMenuItem Col_MenuItem_Help_About
        {
            get => col_MenuItem_Help_About;
        }

        #endregion

        /// <summary>
        /// 菜单系统
        /// </summary>
        public MenuStrip Col_MenuStrip
        {
            get => col_menuStrip;
        }

        #endregion

        #region 小说内容

        /// <summary>
        /// 用于显示分卷和章节的树结构视图
        /// </summary>
        public TreeView Col_TreeView_VolumeTree
        {
            get => col_treeView_bookPath;
        }

        /// <summary>
        /// 显示小说内容的富文本框
        /// </summary>
        public RichTextBox Col_Rtf_BookText
        {
            get => col_richTextBox_bookText;
        }

        #endregion

        #region 对话框控件

        /// <summary>
        /// 控件参数 - 打开文件对话框
        /// </summary>
        private OpenFileDialog Col_OpenFileDialog
        {
            get => col_openFileDialog;
        }

        /// <summary>
        /// 控件参数 - 保存文件对话框
        /// </summary>
        private SaveFileDialog Col_SaveFileDialog
        {
            get => col_saveFileDialog;
        }

        #endregion

        public Button Col_Button_Test
        {
            get => col_button_test;
        }

        #endregion

        #region 释放

        private void f_disposing(bool disposing)
        {
            if (disposing)
            {
                f_tryCloseEasyBook();
                if(p_preferencesWindow != null)
                {
                    p_preferencesWindow.Dispose();
                }
            }

            InitArgs.Args.DisposeObject(disposing);

            p_preferencesWindow = null;

        }

        #endregion

        #region 事件注册

        #region 公共事件

        /// <summary>
        /// 事件注册函数 - 弹窗 - 暂无功能提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fe_ShowMeg_NotImplemented(object sender, EventArgs e)
        {
            MessageBox.Show(this, "暂无功能：）", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 事件注册函数 - 退出按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fe_Click_ExitButton(object sender, EventArgs e)
        {
            f_mainFormExit();
        }

        /// <summary>
        /// 事件注册函数 - 打开文件（打开小说）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fe_OpenFile(object sender, EventArgs e)
        {

            var openDig = Col_OpenFileDialog;
            openDig.Title = "打开小说文件或一个文件夹内的easybook.json";
            openDig.Filter = "easybook文件|*.ebk;*.easybook;*.json|所有文件|*";
            var re = Col_OpenFileDialog.ShowDialog(this);

            if(re == DialogResult.OK)
            {
                var path = Col_OpenFileDialog.FileName;
                InitNewEasyBook(path, true);
            }

        }

        /// <summary>
        /// 弹窗导出配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fe_PreToFiles(object sender, EventArgs e)
        {
            var csd = Col_SaveFileDialog;
            csd.Title = "选择你要将配置文件导出的位置";
            csd.CheckFileExists = false;
            csd.CheckPathExists = true;
            csd.Filter = "配置文件|*.json|所有文件|*";
            var re = csd.ShowDialog(this);
            if (re == DialogResult.OK)
            {
                try
                {
                    var path = csd.FileName;
                    if (File.Exists(path))
                    {
                        if(!ShowMeg_Warring("警告", "选择的位置已经存在文件，确定要覆盖吗？"))
                        {
                            return;
                        }
                    }

                    f_setPreToFile(csd.FileName);
                    ShowMeg_Inf(null, "成功导出配置");
                }
                catch (Exception ex)
                {
                    StringBuilder sb = new StringBuilder(64);
                    sb.AppendLine(ex.Message);
                    sb.AppendLine(ex.GetType().FullName);
                    sb.AppendLine(ex.StackTrace);
                    ShowMeg_Error(null, sb.ToString());
                }
            }

        }

        /// <summary>
        /// 弹窗选择导入的配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fe_OpenPreFile(object sender, EventArgs e)
        {

            var openDig = Col_OpenFileDialog;
            openDig.Title = "打开配置文件";
            openDig.Filter = "配置文件|*.json;|所有文件|*";
            var re = Col_OpenFileDialog.ShowDialog(this);

            if (re == DialogResult.OK)
            {
                var path = openDig.FileName;
                //InitNewEasyBook(path, true);
                if (!f_readFileByPre(path))
                {
                    ShowMeg_Error(null, "无法读取配置文件");
                }
            }
        }

        #endregion

        #region 菜单事件

        #region 文件

        /// <summary>
        /// 弹窗保存到EPUB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fe_Click_Menu_SaveToEPUB(object sender, EventArgs e)
        {
            f_ShowSelectToEPUB();
        }

        #endregion

        #region 工具

        /// <summary>
        /// 点击首选项菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fe_Click_Menu_Preferences(object sender, EventArgs e)
        {
            //fe_ShowMeg_NotImplemented(sender, e);

            p_preferencesWindow.SetArgs(GetPreferences());
            var re = p_preferencesWindow.ShowDialog(this);

            if(re == DialogResult.OK)
            {
                SetPreferences(p_preferencesWindow.GetArgs());
                
            }
            GC.Collect(0);
        }

        #endregion

        #region 帮助

        /// <summary>
        /// 事件注册 - 菜单 - 帮助 - 关于
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fe_Click_MenuItem_About(object sender, EventArgs e)
        {
            fe_ShowMeg_NotImplemented(sender, e);
        }

        #endregion

        #endregion

        #region 小说内容事件响应

        #region 目录树

        /// <summary>
        /// 事件注册函数 - 目录树被双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fe_DoubleClick_TreeNode(object sender, TreeNodeMouseClickEventArgs e)
        {

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                //双击左键
                f_OpenVolumeToTextByRtf(e.Node);
            }

        }


        /// <summary>
        /// 事件注册函数 - 将标头信息打印在文本框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fe_PrintHeader(object sender, EventArgs e)
        {
            if (p_easyBook is null)
            {
                ShowMeg_Error(null, "未打开书籍");
            }
            else
            {
                f_ShowToTitle();
            }
        }

        #endregion
        
        #region 文本框

        /// <summary>
        /// RTF富文本框的 KeyDown 事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fe_KeyDown_Rtf(object sender, PreviewKeyDownEventArgs e)
        {

            //var keyCode = e.KeyCode;
            //if ((keyCode  == Keys.C) && ((ModifierKeys & Keys.Control) == Keys.Control))
            //{
            //    Col_Rtf_BookText.Copy();
            //}

            if ((e.Modifiers & Keys.Control) != 0)
            {
                var key = e.KeyCode;
                if (key == Keys.C)
                {
                    //ShowMeg_Inf("消息", "按键响应");
                    Col_Rtf_BookText.Copy();
                }
                else if (key == Keys.A)
                {
                    Col_Rtf_BookText.SelectAll();
                }
            }
            

        }

        #endregion

        #endregion

        private void fe_Click_Button_Test(object sender, EventArgs e)
        {

            //Console.Clear();
            //print(Col_Rtf_BookText.Rtf);
            //STreeNode node = new STreeNode("text");
            //print($"Text:{node.Text}");
            //print($"Name:{node.Name}");

        }

        #endregion

        #region 封装

        #region epub

        private void f_ShowSelectToEPUB()
        {

            if(p_easyBook is null)
            {
                ShowMeg_Error("错误", "未打开书籍！");
                return;
            }

            var csd = Col_SaveFileDialog;
            csd.Title = "选择你要保存的位置";
            csd.CheckFileExists = false;
            csd.CheckPathExists = true;
            csd.Filter = "EPUB文件|*.epub|所有文件|*";

            var re = csd.ShowDialog(this);

            if(re == DialogResult.OK)
            {
                //ok
                var path = csd.FileName;

                if (File.Exists(path))
                {
                    if(!ShowMeg_Warring("警告", "选择的位置已存在相同名称的文件，是否覆盖？"))
                    {
                        //不覆盖
                        return;
                    }
                }

                SaveToEPUBStructure st = new SaveToEPUBStructure();
                st.mainForm = this;
                st.path = path;
                Col_Rtf_BookText.Clear();
                f_DisableAllContorl();
                Task.Run(st.f_actionSaveToEPUB);

            }

        }

        private class RTFTextWriter : Cheng.Texts.SafeReleaseTextWriter
        {
            public RTFTextWriter(MainForm mainForm)
            {
                this.mainForm = mainForm;
            }
            private MainForm mainForm;
            public override Encoding Encoding => Encoding.UTF8;

            public override void Write(string value)
            {
                mainForm.Col_Rtf_BookText.AppendText(value);
            }

            public override void Write(char[] buffer, int index, int count)
            {
                Write(new string(buffer, index, count));
            }

            public override void Write(char[] buffer)
            {
                Write(new string(buffer));
            }

            public override void WriteLine()
            {
                mainForm.Col_Rtf_BookText.AppendText(Environment.NewLine);
            }

            public override void WriteLine(string value)
            {
                mainForm.Col_Rtf_BookText.appendTextLine(value);
            }

            public override void WriteLine(char[] buffer, int index, int count)
            {
                WriteLine(new string(buffer, index, count));
            }

        }

        private class SaveToEPUBStructure
        {
            public MainForm mainForm;
            public string path;

            public void f_actionSaveToEPUB()
            {
                RTFTextWriter rtf = null;
                try
                {
                    rtf = new RTFTextWriter(mainForm);
                    using (FileStream zipFile = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                    {
                        using (ZipArchive zip = new ZipArchive(zipFile, ZipArchiveMode.Create, true, Encoding.UTF8))
                        {

                            ConvertEpub.CreateEpubByEbk(mainForm.p_easyBook, zip, rtf);
                        }

                    }
                }
                catch (Exception ex)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("错误:");
                    sb.AppendLine(ex.Message);
                    sb.AppendLine(ex.GetType().FullName);
                    sb.AppendLine(ex.StackTrace);
                    string s = sb.ToString();
                    rtf.WriteLine(s);
                    printl(s);
                }
                finally
                {
                    mainForm.f_EnableAllContorl();
                }
            }

        }

        #endregion

        #region 控件

        /// <summary>
        /// 禁用所有控件只留只读RTF
        /// </summary>
        private void f_DisableAllContorl()
        {
            Col_MenuStrip.Enabled = false;
            Col_TreeView_VolumeTree.Enabled = false;
            Col_Rtf_BookText.ReadOnly = true;
        }

        /// <summary>
        /// 启动所有控件
        /// </summary>
        private void f_EnableAllContorl()
        {
            Col_MenuStrip.Enabled = true;
            Col_TreeView_VolumeTree.Enabled = true;
        }

        #endregion

        #region 节点

        /// <summary>
        /// 获取视图分卷节点对应的数据分卷根节点
        /// </summary>
        /// <param name="volNode">视图分卷节点</param>
        /// <param name="buffer">栈缓冲区</param>
        /// <returns></returns>
        private CTNodeV f_getVolumeNodeData(STreeNode volNode, Stack<int> buffer)
        {

            //索引栈
            //Stack<int> indexBuf = buffer;
            buffer.Clear();

            STreeNode stn = volNode;

            while (stn != null)
            {
                //推入索引
                buffer.Push(stn.Index);
                //获取父节点
                stn = stn.Parent;
            }

            CTNodeV volDN = p_easyBook.VolumeRoot;
            //查找索引
            while (buffer.Count != 0)
            {
                //获取索引
                int i = buffer.Pop();
                //获取子节点
                volDN = volDN[i];
            }

            return volDN;

        }

        /// <summary>
        /// 分卷节点完整路径
        /// </summary>
        /// <param name="volNode">视图分卷节点</param>
        /// <returns></returns>
        private string f_getVolumeNodePath(STreeNode volNode)
        {
            STreeNode stn = volNode.Parent;

            const string rootPath = EasyBook.TextRootFolder;

            if (stn is null)
            {
                return rootPath; //最初的分卷节点
            }

            string path = volNode.Name;

            while (stn != null)
            {
                path = Path.Combine(stn.Name, path);
                stn = stn.Parent;
            }

            //获取分卷完整目录
            return Path.Combine(rootPath, path);
        }

        #endregion

        #region 弹窗

        /// <summary>
        /// 弹出错误对话框
        /// </summary>
        /// <param name="title"></param>
        /// <param name="meg"></param>
        private void ShowMeg_Error(string title, string meg)
        {
            MessageBox.Show(this, meg, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 弹出信息对话框
        /// </summary>
        /// <param name="title"></param>
        /// <param name="meg"></param>
        private void ShowMeg_Inf(string title, string meg)
        {
            MessageBox.Show(this, meg, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 弹出警告对话框，按钮是确定和取消
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="meg">消息</param>
        /// <returns>点击了确定返回true</returns>
        private bool ShowMeg_Warring(string title, string meg)
        {
            return MessageBox.Show(this, meg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK;
        }

        #endregion

        #region 退出

        /// <summary>
        /// 要主动退出主窗口
        /// </summary>
        private void f_mainFormExit()
        {
            this.Close();
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 尝试关闭easybook
        /// </summary>
        private void f_tryCloseEasyBook()
        {
            if (p_easyBook != null)
            {
                p_easyBook.Close();
            }
            p_easyBook = null;
        }

        /// <summary>
        /// 初始化新的easybook
        /// </summary>
        /// <param name="path">书籍full文件路径；zip文件路径或json文件路径</param>
        /// <param name="ebk">获取的新书籍</param>
        /// <returns>
        /// <para>返回1初始化zip文件，返回2初始化json根目录标头；返回-1路径有问题；返回-2路径所指向的不是一个完备的easybook</para>
        /// </returns>
        private int f_initNewEasyBook(string path, out EasyBook ebk)
        {
            ebk = null;
            if (string.IsNullOrEmpty(path)) return -1;

            if (!File.Exists(path))
            {
                return -1;
            }

            bool isJsonFile;
            try
            {
                p_jsonParser.FileToJson(path);
                isJsonFile = true;
            }
            catch (Exception)
            {
                isJsonFile = false;
            }

            BaseCompressionParser bookData;
            int res;
            if (isJsonFile)
            {
                //属于json
                //获取根目录
                var pathDire = Path.GetDirectoryName(path);
                bookData = new RootToFolderIndexer(pathDire);
                res = 2;
            }
            else
            {
                //zip文件
                FileStream fileStream = null;
                bookData = null;
                try
                {
                    fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    bookData = new ZipArchiveCompress(fileStream, ZipArchiveMode.Read, 1024 * 4, true, Encoding.UTF8);
                }
                catch (Exception)
                {
                    if(bookData != null)
                    {
                        bookData.Close();
                    }
                    else
                    {
                        fileStream?.Close();
                    }
                    //也不是zip
                    return -2;
                }
                res = 1;
            }

            try
            {
                ebk = new EasyBook(bookData, true);
            }
            catch (Exception)
            {
                ebk?.Close();
                return -2;
            }


            return res;
        }

        /// <summary>
        /// 打开书籍
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="showErrorMeg">错误会引发错误弹窗</param>
        /// <returns>是否成功打开</returns>
        public bool InitNewEasyBook(string path, bool showErrorMeg)
        {
            var i = f_initEasyBook(path);
            if (i < 0)
            {
                if (showErrorMeg)
                {
                    if (i == -1)
                    {
                        ShowMeg_Error(null, "打开的文件不存在！");
                    }
                    else
                    {
                        ShowMeg_Error(null, "打开的文件不是一个ebk文件或者不是ebk结构的标头！");
                    }
                }
                return false;
            }
            f_InitEasyBookView();
            return true;
        }

        /// <summary>
        /// 初始化新的书籍，并关闭之前的书籍；修改标题
        /// </summary>
        /// <param name="path"></param>
        /// <returns>返回1初始化zip文件，返回2初始化json根目录标头；返回-1路径有问题；返回-2路径所指向的不是一个完备的easybook</returns>
        private int f_initEasyBook(string path)
        {

            var re = f_initNewEasyBook(path, out EasyBook ebk);

            if(re < 0)
            {
                //错误
                return re;
            }
            f_tryCloseEasyBook();
            p_easyBook = ebk;

            string booktype;
            if(re == 1)
            {
                booktype = "[书籍]";
            }
            else
            {
                booktype = "[书籍文件夹]";
            }

            this.Text = ("简易书籍阅读器 - " + booktype + " - \"" + path + "\"");

            var header = p_easyBook.Header;

            return re;
        }

        private void f_InitEasyBookView()
        {
            if (p_easyBook is null)
            {
                return;
            }
            Col_Rtf_BookText.Clear();
            var volumeViewNodes = Col_TreeView_VolumeTree.Nodes;
            volumeViewNodes.Clear();
            //p_easyBook.VolumeRoot;

            var ebkVol = p_easyBook.VolumeRoot;

            if(ebkVol.Count == 0)
            {
                var list = ebkVol.Value.chapters;

                for (int i = 0; i < list.Count; i++)
                {
                    var cindex = list[i];
                    STreeNode stn = new STreeNode((i + 1).ToString() + ":" + cindex.title);
                    stn.Name = cindex.chapterFilePath;
                    volumeViewNodes.Add(stn);
                }
            }
            else
            {
                for (int i = 0; i < ebkVol.Count; i++)
                {
                    var cebk = ebkVol[i];
                    STreeNode snode = new STreeNode(cebk.Value.name);
                    snode.Name = cebk.Value.index;
                    f_createVolNodeToTreeView(cebk, snode);
                    volumeViewNodes.Add(snode);
                }
            }

           f_ShowToTitle();
        }

        /// <summary>
        /// 对视图节点创建分卷树 递归
        /// </summary>
        /// <param name="volNode">分卷节点</param>
        /// <param name="viewNode">视图节点</param>
        private void f_createVolNodeToTreeView(CTNodeV volNode, STreeNode viewNode)
        {

            var value = volNode.Value;

            if (value.volumeType == VolumeType.Node)
            {
                var vnodes = viewNode.Nodes;
                for (int i = 0; i < volNode.Count; i++)
                {
                    var st = new STreeNode(volNode[i].Value.name);
                    st.Name = volNode[i].Value.index;
                    f_createVolNodeToTreeView(volNode[i], st);
                    vnodes.Add(st);
                }
            }
            else if(value.volumeType == VolumeType.Chapters)
            {
                f_createChapterToTreeView(volNode, viewNode);
            }
        }

        /// <summary>
        /// 为视图节点初始化章节
        /// </summary>
        /// <param name="volNode">要初始化的章节节点</param>
        /// <param name="viewNode">要初始化的视图父节点</param>
        private void f_createChapterToTreeView(CTNodeV volNode, STreeNode viewNode)
        {

            var vol = volNode.Value;
            var list = vol.chapters;

            for (int i = 0; i < list.Count; i++)
            {
                var cindex = list[i];
                STreeNode stn = new STreeNode((i + 1).ToString() + ":" + cindex.title);
                stn.Name = cindex.chapterFilePath;
                viewNode.Nodes.Add(stn);
            }

        }

        /// <summary>
        /// 尝试打开系统分配的命令行路径
        /// </summary>
        private void f_tryOpenSystemOpenFile()
        {
            var args = InitArgs.Args;
            if (string.IsNullOrEmpty(args.firstInitOpenPath))
            {
                f_ShowInitView();
            }
            else
            {
                if(!InitNewEasyBook(args.firstInitOpenPath, true))
                {
                    f_ShowInitView();
                }
            }
        }

        #endregion

        #region 显示信息
        
        /// <summary>
        /// 文本框显示初始页面
        /// </summary>
        private void f_ShowInitView()
        {
            Col_TreeView_VolumeTree.Nodes.Clear();
            var rtf = Col_Rtf_BookText;
            rtf.Clear();
            rtf.appendTextLine("easybook阅读器");
            rtf.appendTextLine("请打开一个 ebk 或 json 文件");
            rtf.appendTextLine("© 不一样的惩");
        }

        private void f_ShowToTitle()
        {
            if (p_easyBook is null) return;
            var header = p_easyBook.Header;

            var rtf = Col_Rtf_BookText;
            StringBuilder sb = new StringBuilder(64);

            sb.Append('《');
            sb.Append(header.name);
            sb.Append('》');
            sb.AppendLine();

            sb.Append("作者:");
            if(string.IsNullOrEmpty(header.author))
            {
                sb.AppendLine("<无>");
            }
            else
            {
                sb.AppendLine(header.author);
            }

            sb.Append("版权信息:");
            if (string.IsNullOrEmpty(header.publisher))
            {
                sb.AppendLine("<无>");
            }
            else
            {
                sb.AppendLine(header.publisher);
            }

            if ((header.tags?.Length).GetValueOrDefault() != 0)
            {
                sb.Append("标签: ");
                var tags = header.tags;
                for (int i = 0; i < tags.Length; i++)
                {
                    sb.Append(tags[i]);
                    if(i + 1 != tags.Length)
                    {
                        sb.Append(',');
                    }
                }
                sb.AppendLine();
            }

            if (header.customs != null && header.customs.Length != 0)
            {
                foreach (var tagPair in header.customs)
                {
                    sb.Append(tagPair.Key);
                    sb.Append(':');
                    sb.AppendLine(tagPair.Value);
                }
                sb.AppendLine();
            }
            if (!string.IsNullOrEmpty(header.synopsis))
            {
                sb.AppendLine("简介:");
                sb.AppendLine(header.synopsis);
            }

            rtf.Clear();
            
            if (!string.IsNullOrEmpty(header.coverImage))
            {
                if (p_easyBook.BaseDataPack.ContainsData(header.coverImage))
                {

                    var cv = header.cover;
                    float width, height;
                    if (cv.HasValue)
                    {
                        var cvv = cv.Value;
                        width = cvv.width;
                        height = cvv.height;
                    }
                    else
                    {
                        width = 1;
                        height = 1;
                    }

                    try
                    {
                        f_appendImageLine(rtf, header.coverImage, cv.HasValue, width, height);
                    }
                    catch (Exception ex)
                    {
                        printl("打印封面出错");
                        printl(ex.Message);
                        printl(ex.GetType().FullName);
                        printl(ex.StackTrace);
                    }

                }

                
            }

            rtf.AppendText(sb.ToString());

        }

        /// <summary>
        /// 将章节分卷信息显示到富文本框
        /// </summary>
        /// <param name="volNode">章节节点</param>
        private void f_ShowToVolumeMeg(STreeNode volNode)
        {
            //volNode;
            //return;
            try
            {
                var vol = f_getVolumeNodeData(volNode, new Stack<int>(4));
                f_printToVolume(vol.Value);
            }
            catch (Exception ex)
            {
                printl("分卷信息显示异常");
                printl(ex.Message);
                printl(ex.GetType().FullName);
                printl(ex.StackTrace);
                printl();
            }
   
        }

        /// <summary>
        /// 打印分卷信息到文本框
        /// </summary>
        /// <param name="volume"></param>
        private void f_printToVolume(Volume volume)
        {
            StringBuilder sb = new StringBuilder(32);
            
            sb.AppendLine(volume.name);

            if((volume.customItems?.Length).GetValueOrDefault() != 0)
            {
                for (int i = 0; i < volume.customItems.Length; i++)
                {

                    var item = volume.customItems[i];
                    sb.Append(item.Key);
                    sb.Append(':');
                    sb.Append(' ');
                    sb.AppendLine(item.Value);
                }
            }

            if (!string.IsNullOrEmpty(volume.synopsis))
            {
                sb.AppendLine();
                sb.AppendLine("简介:");
                sb.AppendLine(volume.synopsis);
            }

            Col_Rtf_BookText.Clear();
            Col_Rtf_BookText.AppendText(sb.ToString());
        }

        #endregion

        #region 关闭

        /// <summary>
        /// 关闭小说并清空视图
        /// </summary>
        public void CloseEasyBookClearView()
        {
            f_tryCloseEasyBook();
            Col_Rtf_BookText.Clear();
            Col_TreeView_VolumeTree.Nodes.Clear();
        }

        #endregion

        #region 设置参数

        /// <summary>
        /// 从首选项设置参数
        /// </summary>
        /// <param name="value"></param>
        public void SetPreferences(PreValue value)
        {
            if (value is null) throw new ArgumentNullException();

            this.BackColor = value.backColor;
            Col_Rtf_BookText.BackColor = value.rtfColor;
            Col_Rtf_BookText.ForeColor = value.rtfTextColor;
            if (value.rtfFont != null) Col_Rtf_BookText.Font = value.rtfFont;
            Col_TreeView_VolumeTree.BackColor = value.volumeColor;
            if (value.volumeFont != null) Col_TreeView_VolumeTree.Font = value.volumeFont;
            Col_TreeView_VolumeTree.ForeColor = value.volumeTextColor;

            if (value.winSize.HasValue)
            {
                var minsize = this.MinimumSize;
                var size = value.winSize.Value;
                if (size.Width < minsize.Width) size.Width = minsize.Width;
                if (size.Height < minsize.Height) size.Height = minsize.Height;
                this.Size = size;
            }

        }

        /// <summary>
        /// 将当前窗体参数转化为首选项参数
        /// </summary>
        /// <returns></returns>
        public PreValue GetPreferences()
        {
           var value = new PreferencesWindow.PreferencesValue();
            value.backColor = this.BackColor;
            value.rtfColor = Col_Rtf_BookText.BackColor;
            value.rtfTextColor = Col_Rtf_BookText.ForeColor;
            value.rtfFont = Col_Rtf_BookText.Font;
            value.volumeColor = Col_TreeView_VolumeTree.BackColor;
            value.volumeFont = Col_TreeView_VolumeTree.Font;
            value.volumeTextColor = Col_TreeView_VolumeTree.ForeColor;
            value.winSize = this.Size;
            return value;
        }

        /// <summary>
        /// 将配置参数写入到指定文件
        /// </summary>
        /// <param name="path"></param>
        private void f_setPreToFile(string path)
        {

            var pre = GetPreferences();
            var json = pre.ToJson();
            p_jsonParser.WriterToFile(json, path, Encoding.UTF8, false);

        }

        /// <summary>
        /// 将首选项参数设置到配置文件
        /// </summary>
        public void SetPreferencesToSetting()
        {
            try
            {
                var pre = GetPreferences();
                var json = pre.ToJson();

                p_jsonParser.WriterToFile(json, InitArgs.Args.defPreferencesFilePath, Encoding.UTF8, false);
            }
            catch (Exception)
            {
                printl("保存配置文件时出错");
            }
        }

        /// <summary>
        /// 从配置文件读取首选项参数到应用
        /// </summary>
        /// <returns>是否成功读取；全部无法读取false，有读取成功参数true</returns>
        private bool f_settingReaderToPreferences()
        {
            return f_readFileByPre(InitArgs.Args.defPreferencesFilePath);
        }

        /// <summary>
        /// 从指定路径读取首选项参数到应用
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>是否成功读取；全部无法读取false，有读取成功参数true</returns>
        private bool f_readFileByPre(string filePath)
        {
            try
            {
                JsonVariable json;
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                {
                    using (StreamReader sr = new StreamReader(file, Encoding.UTF8, false, 1024 * 2, true))
                    {
                        json = p_jsonParser.ToJsonData(sr);
                    }
                }

                var value = new PreValue();

                var b = value.JsonToValue(json);

                if ((!b.backColor) && (!b.rtfColor) && (!b.rtfFont) && (!b.rtfTextColor) && (!b.volumeColor) && (!b.volumeFont) && (!b.volumeTextColor))
                {
                    return false;
                }
                var nowPre = GetPreferences();
                if (!b.backColor)
                {
                    value.backColor = nowPre.backColor;
                }
                if (!b.rtfColor)
                {
                    value.rtfColor = nowPre.rtfColor;
                }
                if (!b.rtfFont)
                {
                    value.rtfFont = nowPre.rtfFont;
                }
                if (!b.volumeColor)
                {
                    value.volumeColor = nowPre.volumeColor;
                }
                if (!b.volumeFont)
                {
                    value.volumeFont = nowPre.volumeFont;
                }
                if (!b.volumeTextColor)
                {
                    value.volumeTextColor = nowPre.volumeTextColor;
                }
                SetPreferences(value);
                return true;
            }
            catch (FileNotFoundException)
            {
                printl("没有配置文件");
            }
            catch (Exception ex)
            {
                printl("读取配置文件出错:" + ex.Message);
                printl(ex.GetType().FullName);
                printl(ex.StackTrace);
            }
            return false;
        }

        /// <summary>
        /// 从系统数据中读取并返回是否亮色还是暗色模式
        /// </summary>
        /// <returns>true亮色模式，false暗色模式</returns>
        private bool f_readSysWinStyle()
        {
            var args = InitArgs.Args;
            if (args.lightThemeApp.HasValue)
            {
                return args.lightThemeApp.Value;
            }
            else if (args.lightThemeSystem.HasValue)
            {
                return args.lightThemeSystem.Value;
            }
            return true;
        }

        #endregion

        #region 目录

        /// <summary>
        /// 判断并打开目录，显示在视图上
        /// </summary>
        /// <param name="node"></param>
        private void f_OpenVolumeToTextByRtf(STreeNode node)
        {
            if(p_easyBook is null)
            {
                printl("没有easybook数据");
                return;
            }
            //clearPrint();
            //print("FullPath:" + node.FullPath);
            //print("Name:" + node.Name);
            //print("Index:" + node.Index);

            if (node.Nodes.Count != 0)
            {
                //是分卷节点
                f_ShowToVolumeMeg(node);
                return;
            }

            //获取章节
            STreeNode stn = node;
            var path = stn.Name;
            path = path.Replace('/', '\\');

            var pack = p_easyBook.BaseDataPack;
            if (!pack.ContainsData(path))
            {
                printl("属于章节文件树视图，但不属于章节文件索引");
                return;
            }

            try
            {
                Col_Rtf_BookText.Clear();
                using (var cs = p_easyBook.CreateChapterStream(path))
                {
                    int count = 0;
                    while (true)
                    {
                        var line = cs.ReadLine();

                        if (!line.HasValue)
                        {
                            if(count == 0)
                            {
                                Col_Rtf_BookText.AppendText("<[空章节]>");
                            }
                            break;
                        }

                        f_appendTextLine(Col_Rtf_BookText, line.Value);
                        count++;
                    }
                }

            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder(64);
                sb.AppendLine(ex.Message);
                sb.AppendLine(ex.GetType().FullName);
                sb.Append(ex.StackTrace);
                MessageBox.Show(this, sb.ToString(), null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void f_appendTextLine(RichTextBox rtf, ChapterLine line)
        {

            if(line.type == ChapterLineType.Text)
            {
                rtf.appendTextLine(line.value);
            }
            else if(line.type == ChapterLineType.Image)
            {

                var imagePath = line.value;

                var ip = p_easyBook.GetFullNameByImagePath(imagePath);

                using (var openStream = p_easyBook.BaseDataPack.OpenCompressedStream(ip))
                {
                    if (openStream is null) throw new ArgumentNullException("imagePath", "错误的图像路径");

                    using (var image = Image.FromStream(openStream))
                    {

                        if (line.customImageScale && (line.width > 0.1f && line.height > 0.1f))
                        {
                            rtf.appendImageLine(image, Maths.Clamp(line.width, 0.1f, 1), Maths.Clamp(line.height, 0.1f, 1), p_StreamBuffer);
                        }
                        else
                        {

                            rtf.appendImageLine(image, 1, 1, p_StreamBuffer);
                        }

                    }

                }

        

            }

        }

        /// <summary>
        /// 添加一行图像
        /// </summary>
        /// <param name="rtf"></param>
        /// <param name="imagePath">图像所在的数据包绝对路径</param>
        /// <param name="customImageScale"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void f_appendImageLine(RichTextBox rtf, string imagePath, bool customImageScale, float width, float height)
        {

            //var ip = p_easyBook.GetFullNameByImagePath(imagePath);

            using (var openStream = p_easyBook.BaseDataPack.OpenCompressedStream(imagePath))
            {
                if (openStream is null) throw new ArgumentNullException("imagePath", "错误的图像路径");

                using (var image = Image.FromStream(openStream))
                {

                    if (customImageScale && (width > 0.1f && height > 0.1f))
                    {
                        rtf.appendImageLine(image, Maths.Clamp(width, 0.1f, 1), Maths.Clamp(height, 0.1f, 1), p_StreamBuffer);
                    }
                    else
                    {

                        rtf.appendImageLine(image, 1, 1, p_StreamBuffer);
                    }

                }

            }
        }

        #endregion

        #endregion

        #region 派生

        /// <summary>
        /// 第一次显示窗体前
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        /// <summary>
        /// 关闭窗体时
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        /// <summary>
        /// 关闭窗体后
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
        }

        /// <summary>
        /// 关闭窗体前
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var cr = e.CloseReason;

            if (cr != CloseReason.WindowsShutDown && cr != CloseReason.None)
            {
                //并非关机
                SetPreferencesToSetting();
            }

            base.OnFormClosing(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        /// <summary>
        /// 首次显示窗体时
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            f_tryOpenSystemOpenFile();
        }

        #endregion

        #region 测试

        void test()
        {
            //Col_Rtf_BookText.ScrollBars = RichTextBoxScrollBars.Both;
        }

        static void clearPrint()
        {
            //Console.Clear();
        }

        static void printl()
        {
            InitArgs.Args.DebugPrintLine();
            //Console.WriteLine();
        }

        static void printl(string meg)
        {
            InitArgs.Args.DebugPrintLine(meg);
            //Console.WriteLine(meg);
        }

        static void print(string meg)
        {
            //Console.WriteLine(meg);
            InitArgs.Args.DebugPrint(meg);
            //Console.Write(meg);
        }

        static void flushPrint()
        {
            InitArgs.Args.DebugFlush();
        }

        #endregion

        #endregion

    }
}
