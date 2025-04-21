
namespace Cheng.EasyBooks.WinForms
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            f_disposing(disposing);

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.col_button_test = new System.Windows.Forms.Button();
            this.col_menuStrip = new System.Windows.Forms.MenuStrip();
            this.col_ToolStripMenuItem_File = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_File_New = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_File_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_File_LookInf = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_File_Another = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_File_sp1 = new System.Windows.Forms.ToolStripSeparator();
            this.col_MenuItem_File_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_File_sp2 = new System.Windows.Forms.ToolStripSeparator();
            this.col_MenuItem_File_Print = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_File_PrintView = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.col_MenuItem_File_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.col_ToolStripMenuItem_Editor = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_Editor_Revoke = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_Editor_Repeat = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_Editor_sp1 = new System.Windows.Forms.ToolStripSeparator();
            this.col_MenuItem_Editor_SubTrim = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_Editor_Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_Editor_CopyTo = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_Editor_sp2 = new System.Windows.Forms.ToolStripSeparator();
            this.col_MenuItem_Editor_AllSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.col_ToolStripMenuItem_Tool = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_Tool_Preferences = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_Tool_PreSaveTo = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_Tool_PreRead = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_Tool_Custom = new System.Windows.Forms.ToolStripMenuItem();
            this.col_ToolStripMenuItem_Help = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_Help_Neirong = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_Help_Index = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_Help_Scan = new System.Windows.Forms.ToolStripMenuItem();
            this.col_MenuItem_Help_sp1 = new System.Windows.Forms.ToolStripSeparator();
            this.col_MenuItem_Help_About = new System.Windows.Forms.ToolStripMenuItem();
            this.col_richTextBox_bookText = new System.Windows.Forms.RichTextBox();
            this.col_openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.col_treeView_bookPath = new System.Windows.Forms.TreeView();
            this.col_saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.col_menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // col_button_test
            // 
            this.col_button_test.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.col_button_test.Enabled = false;
            this.col_button_test.Location = new System.Drawing.Point(1138, 613);
            this.col_button_test.Name = "col_button_test";
            this.col_button_test.Size = new System.Drawing.Size(112, 48);
            this.col_button_test.TabIndex = 1;
            this.col_button_test.Text = "test";
            this.col_button_test.UseVisualStyleBackColor = true;
            this.col_button_test.Visible = false;
            // 
            // col_menuStrip
            // 
            this.col_menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.col_menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.col_ToolStripMenuItem_File,
            this.col_ToolStripMenuItem_Editor,
            this.col_ToolStripMenuItem_Tool,
            this.col_ToolStripMenuItem_Help});
            this.col_menuStrip.Location = new System.Drawing.Point(0, 0);
            this.col_menuStrip.Name = "col_menuStrip";
            this.col_menuStrip.Size = new System.Drawing.Size(1262, 28);
            this.col_menuStrip.TabIndex = 2;
            this.col_menuStrip.Text = "menuStrip";
            // 
            // col_ToolStripMenuItem_File
            // 
            this.col_ToolStripMenuItem_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.col_MenuItem_File_New,
            this.col_MenuItem_File_Open,
            this.col_MenuItem_File_LookInf,
            this.col_MenuItem_File_Another,
            this.col_MenuItem_File_sp1,
            this.col_MenuItem_File_Save,
            this.col_MenuItem_File_sp2,
            this.col_MenuItem_File_Print,
            this.col_MenuItem_File_PrintView,
            this.toolStripSeparator2,
            this.col_MenuItem_File_Exit});
            this.col_ToolStripMenuItem_File.Name = "col_ToolStripMenuItem_File";
            this.col_ToolStripMenuItem_File.Size = new System.Drawing.Size(71, 24);
            this.col_ToolStripMenuItem_File.Text = "文件(&F)";
            // 
            // col_MenuItem_File_New
            // 
            this.col_MenuItem_File_New.Enabled = false;
            this.col_MenuItem_File_New.Image = ((System.Drawing.Image)(resources.GetObject("col_MenuItem_File_New.Image")));
            this.col_MenuItem_File_New.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.col_MenuItem_File_New.Name = "col_MenuItem_File_New";
            this.col_MenuItem_File_New.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.col_MenuItem_File_New.Size = new System.Drawing.Size(254, 26);
            this.col_MenuItem_File_New.Text = "新建(&N)";
            this.col_MenuItem_File_New.Visible = false;
            // 
            // col_MenuItem_File_Open
            // 
            this.col_MenuItem_File_Open.Image = ((System.Drawing.Image)(resources.GetObject("col_MenuItem_File_Open.Image")));
            this.col_MenuItem_File_Open.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.col_MenuItem_File_Open.Name = "col_MenuItem_File_Open";
            this.col_MenuItem_File_Open.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.col_MenuItem_File_Open.Size = new System.Drawing.Size(254, 26);
            this.col_MenuItem_File_Open.Text = "打开书籍(&O)";
            // 
            // col_MenuItem_File_LookInf
            // 
            this.col_MenuItem_File_LookInf.Name = "col_MenuItem_File_LookInf";
            this.col_MenuItem_File_LookInf.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.col_MenuItem_File_LookInf.Size = new System.Drawing.Size(254, 26);
            this.col_MenuItem_File_LookInf.Text = "查看标题界面(&L)";
            // 
            // col_MenuItem_File_Another
            // 
            this.col_MenuItem_File_Another.Name = "col_MenuItem_File_Another";
            this.col_MenuItem_File_Another.Size = new System.Drawing.Size(254, 26);
            this.col_MenuItem_File_Another.Text = "另存为EPUB";
            // 
            // col_MenuItem_File_sp1
            // 
            this.col_MenuItem_File_sp1.Name = "col_MenuItem_File_sp1";
            this.col_MenuItem_File_sp1.Size = new System.Drawing.Size(251, 6);
            // 
            // col_MenuItem_File_Save
            // 
            this.col_MenuItem_File_Save.Enabled = false;
            this.col_MenuItem_File_Save.Image = ((System.Drawing.Image)(resources.GetObject("col_MenuItem_File_Save.Image")));
            this.col_MenuItem_File_Save.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.col_MenuItem_File_Save.Name = "col_MenuItem_File_Save";
            this.col_MenuItem_File_Save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.col_MenuItem_File_Save.Size = new System.Drawing.Size(254, 26);
            this.col_MenuItem_File_Save.Text = "保存(&S)";
            this.col_MenuItem_File_Save.Visible = false;
            // 
            // col_MenuItem_File_sp2
            // 
            this.col_MenuItem_File_sp2.Name = "col_MenuItem_File_sp2";
            this.col_MenuItem_File_sp2.Size = new System.Drawing.Size(251, 6);
            this.col_MenuItem_File_sp2.Visible = false;
            // 
            // col_MenuItem_File_Print
            // 
            this.col_MenuItem_File_Print.Enabled = false;
            this.col_MenuItem_File_Print.Image = ((System.Drawing.Image)(resources.GetObject("col_MenuItem_File_Print.Image")));
            this.col_MenuItem_File_Print.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.col_MenuItem_File_Print.Name = "col_MenuItem_File_Print";
            this.col_MenuItem_File_Print.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.col_MenuItem_File_Print.Size = new System.Drawing.Size(254, 26);
            this.col_MenuItem_File_Print.Text = "打印(&P)";
            this.col_MenuItem_File_Print.Visible = false;
            // 
            // col_MenuItem_File_PrintView
            // 
            this.col_MenuItem_File_PrintView.Enabled = false;
            this.col_MenuItem_File_PrintView.Image = ((System.Drawing.Image)(resources.GetObject("col_MenuItem_File_PrintView.Image")));
            this.col_MenuItem_File_PrintView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.col_MenuItem_File_PrintView.Name = "col_MenuItem_File_PrintView";
            this.col_MenuItem_File_PrintView.Size = new System.Drawing.Size(254, 26);
            this.col_MenuItem_File_PrintView.Text = "打印预览(&V)";
            this.col_MenuItem_File_PrintView.Visible = false;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(251, 6);
            this.toolStripSeparator2.Visible = false;
            // 
            // col_MenuItem_File_Exit
            // 
            this.col_MenuItem_File_Exit.Name = "col_MenuItem_File_Exit";
            this.col_MenuItem_File_Exit.Size = new System.Drawing.Size(254, 26);
            this.col_MenuItem_File_Exit.Text = "退出(&X)";
            // 
            // col_ToolStripMenuItem_Editor
            // 
            this.col_ToolStripMenuItem_Editor.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.col_MenuItem_Editor_Revoke,
            this.col_MenuItem_Editor_Repeat,
            this.col_MenuItem_Editor_sp1,
            this.col_MenuItem_Editor_SubTrim,
            this.col_MenuItem_Editor_Copy,
            this.col_MenuItem_Editor_CopyTo,
            this.col_MenuItem_Editor_sp2,
            this.col_MenuItem_Editor_AllSelect});
            this.col_ToolStripMenuItem_Editor.Enabled = false;
            this.col_ToolStripMenuItem_Editor.Name = "col_ToolStripMenuItem_Editor";
            this.col_ToolStripMenuItem_Editor.Size = new System.Drawing.Size(71, 24);
            this.col_ToolStripMenuItem_Editor.Text = "编辑(&E)";
            this.col_ToolStripMenuItem_Editor.Visible = false;
            // 
            // col_MenuItem_Editor_Revoke
            // 
            this.col_MenuItem_Editor_Revoke.Enabled = false;
            this.col_MenuItem_Editor_Revoke.Name = "col_MenuItem_Editor_Revoke";
            this.col_MenuItem_Editor_Revoke.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.col_MenuItem_Editor_Revoke.Size = new System.Drawing.Size(198, 26);
            this.col_MenuItem_Editor_Revoke.Text = "撤消(&U)";
            this.col_MenuItem_Editor_Revoke.Visible = false;
            // 
            // col_MenuItem_Editor_Repeat
            // 
            this.col_MenuItem_Editor_Repeat.Enabled = false;
            this.col_MenuItem_Editor_Repeat.Name = "col_MenuItem_Editor_Repeat";
            this.col_MenuItem_Editor_Repeat.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.col_MenuItem_Editor_Repeat.Size = new System.Drawing.Size(198, 26);
            this.col_MenuItem_Editor_Repeat.Text = "重复(&R)";
            this.col_MenuItem_Editor_Repeat.Visible = false;
            // 
            // col_MenuItem_Editor_sp1
            // 
            this.col_MenuItem_Editor_sp1.Name = "col_MenuItem_Editor_sp1";
            this.col_MenuItem_Editor_sp1.Size = new System.Drawing.Size(195, 6);
            this.col_MenuItem_Editor_sp1.Visible = false;
            // 
            // col_MenuItem_Editor_SubTrim
            // 
            this.col_MenuItem_Editor_SubTrim.Enabled = false;
            this.col_MenuItem_Editor_SubTrim.Image = ((System.Drawing.Image)(resources.GetObject("col_MenuItem_Editor_SubTrim.Image")));
            this.col_MenuItem_Editor_SubTrim.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.col_MenuItem_Editor_SubTrim.Name = "col_MenuItem_Editor_SubTrim";
            this.col_MenuItem_Editor_SubTrim.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.col_MenuItem_Editor_SubTrim.Size = new System.Drawing.Size(198, 26);
            this.col_MenuItem_Editor_SubTrim.Text = "剪切(&T)";
            this.col_MenuItem_Editor_SubTrim.Visible = false;
            // 
            // col_MenuItem_Editor_Copy
            // 
            this.col_MenuItem_Editor_Copy.Image = ((System.Drawing.Image)(resources.GetObject("col_MenuItem_Editor_Copy.Image")));
            this.col_MenuItem_Editor_Copy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.col_MenuItem_Editor_Copy.Name = "col_MenuItem_Editor_Copy";
            this.col_MenuItem_Editor_Copy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.col_MenuItem_Editor_Copy.Size = new System.Drawing.Size(198, 26);
            this.col_MenuItem_Editor_Copy.Text = "复制(&C)";
            // 
            // col_MenuItem_Editor_CopyTo
            // 
            this.col_MenuItem_Editor_CopyTo.Enabled = false;
            this.col_MenuItem_Editor_CopyTo.Image = ((System.Drawing.Image)(resources.GetObject("col_MenuItem_Editor_CopyTo.Image")));
            this.col_MenuItem_Editor_CopyTo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.col_MenuItem_Editor_CopyTo.Name = "col_MenuItem_Editor_CopyTo";
            this.col_MenuItem_Editor_CopyTo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.col_MenuItem_Editor_CopyTo.Size = new System.Drawing.Size(198, 26);
            this.col_MenuItem_Editor_CopyTo.Text = "粘贴(&P)";
            this.col_MenuItem_Editor_CopyTo.Visible = false;
            // 
            // col_MenuItem_Editor_sp2
            // 
            this.col_MenuItem_Editor_sp2.Name = "col_MenuItem_Editor_sp2";
            this.col_MenuItem_Editor_sp2.Size = new System.Drawing.Size(195, 6);
            // 
            // col_MenuItem_Editor_AllSelect
            // 
            this.col_MenuItem_Editor_AllSelect.Name = "col_MenuItem_Editor_AllSelect";
            this.col_MenuItem_Editor_AllSelect.Size = new System.Drawing.Size(198, 26);
            this.col_MenuItem_Editor_AllSelect.Text = "全选(&A)";
            // 
            // col_ToolStripMenuItem_Tool
            // 
            this.col_ToolStripMenuItem_Tool.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.col_MenuItem_Tool_Preferences,
            this.col_MenuItem_Tool_PreSaveTo,
            this.col_MenuItem_Tool_PreRead,
            this.col_MenuItem_Tool_Custom});
            this.col_ToolStripMenuItem_Tool.Name = "col_ToolStripMenuItem_Tool";
            this.col_ToolStripMenuItem_Tool.Size = new System.Drawing.Size(72, 24);
            this.col_ToolStripMenuItem_Tool.Text = "工具(&T)";
            // 
            // col_MenuItem_Tool_Preferences
            // 
            this.col_MenuItem_Tool_Preferences.Name = "col_MenuItem_Tool_Preferences";
            this.col_MenuItem_Tool_Preferences.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.col_MenuItem_Tool_Preferences.Size = new System.Drawing.Size(211, 26);
            this.col_MenuItem_Tool_Preferences.Text = "首选项(&P)";
            // 
            // col_MenuItem_Tool_PreSaveTo
            // 
            this.col_MenuItem_Tool_PreSaveTo.Name = "col_MenuItem_Tool_PreSaveTo";
            this.col_MenuItem_Tool_PreSaveTo.Size = new System.Drawing.Size(211, 26);
            this.col_MenuItem_Tool_PreSaveTo.Text = "导出设置";
            // 
            // col_MenuItem_Tool_PreRead
            // 
            this.col_MenuItem_Tool_PreRead.Name = "col_MenuItem_Tool_PreRead";
            this.col_MenuItem_Tool_PreRead.Size = new System.Drawing.Size(211, 26);
            this.col_MenuItem_Tool_PreRead.Text = "导入设置";
            // 
            // col_MenuItem_Tool_Custom
            // 
            this.col_MenuItem_Tool_Custom.Enabled = false;
            this.col_MenuItem_Tool_Custom.Name = "col_MenuItem_Tool_Custom";
            this.col_MenuItem_Tool_Custom.Size = new System.Drawing.Size(211, 26);
            this.col_MenuItem_Tool_Custom.Text = "自定义(&C)";
            this.col_MenuItem_Tool_Custom.Visible = false;
            // 
            // col_ToolStripMenuItem_Help
            // 
            this.col_ToolStripMenuItem_Help.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.col_MenuItem_Help_Neirong,
            this.col_MenuItem_Help_Index,
            this.col_MenuItem_Help_Scan,
            this.col_MenuItem_Help_sp1,
            this.col_MenuItem_Help_About});
            this.col_ToolStripMenuItem_Help.Enabled = false;
            this.col_ToolStripMenuItem_Help.Name = "col_ToolStripMenuItem_Help";
            this.col_ToolStripMenuItem_Help.Size = new System.Drawing.Size(75, 24);
            this.col_ToolStripMenuItem_Help.Text = "帮助(&H)";
            this.col_ToolStripMenuItem_Help.Visible = false;
            // 
            // col_MenuItem_Help_Neirong
            // 
            this.col_MenuItem_Help_Neirong.Enabled = false;
            this.col_MenuItem_Help_Neirong.Name = "col_MenuItem_Help_Neirong";
            this.col_MenuItem_Help_Neirong.Size = new System.Drawing.Size(155, 26);
            this.col_MenuItem_Help_Neirong.Text = "内容(&C)";
            this.col_MenuItem_Help_Neirong.Visible = false;
            // 
            // col_MenuItem_Help_Index
            // 
            this.col_MenuItem_Help_Index.Enabled = false;
            this.col_MenuItem_Help_Index.Name = "col_MenuItem_Help_Index";
            this.col_MenuItem_Help_Index.Size = new System.Drawing.Size(155, 26);
            this.col_MenuItem_Help_Index.Text = "索引(&I)";
            this.col_MenuItem_Help_Index.Visible = false;
            // 
            // col_MenuItem_Help_Scan
            // 
            this.col_MenuItem_Help_Scan.Enabled = false;
            this.col_MenuItem_Help_Scan.Name = "col_MenuItem_Help_Scan";
            this.col_MenuItem_Help_Scan.Size = new System.Drawing.Size(155, 26);
            this.col_MenuItem_Help_Scan.Text = "搜索(&S)";
            this.col_MenuItem_Help_Scan.Visible = false;
            // 
            // col_MenuItem_Help_sp1
            // 
            this.col_MenuItem_Help_sp1.Name = "col_MenuItem_Help_sp1";
            this.col_MenuItem_Help_sp1.Size = new System.Drawing.Size(152, 6);
            this.col_MenuItem_Help_sp1.Visible = false;
            // 
            // col_MenuItem_Help_About
            // 
            this.col_MenuItem_Help_About.Name = "col_MenuItem_Help_About";
            this.col_MenuItem_Help_About.Size = new System.Drawing.Size(155, 26);
            this.col_MenuItem_Help_About.Text = "关于(&A)...";
            // 
            // col_richTextBox_bookText
            // 
            this.col_richTextBox_bookText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.col_richTextBox_bookText.BackColor = System.Drawing.SystemColors.Window;
            this.col_richTextBox_bookText.DetectUrls = false;
            this.col_richTextBox_bookText.Location = new System.Drawing.Point(367, 82);
            this.col_richTextBox_bookText.Name = "col_richTextBox_bookText";
            this.col_richTextBox_bookText.ReadOnly = true;
            this.col_richTextBox_bookText.Size = new System.Drawing.Size(883, 579);
            this.col_richTextBox_bookText.TabIndex = 3;
            this.col_richTextBox_bookText.Text = "";
            // 
            // col_openFileDialog
            // 
            this.col_openFileDialog.Filter = "easybook文件|*.ebk;*.easybook;*.json|所有文件|*";
            // 
            // col_treeView_bookPath
            // 
            this.col_treeView_bookPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.col_treeView_bookPath.Location = new System.Drawing.Point(12, 82);
            this.col_treeView_bookPath.Name = "col_treeView_bookPath";
            this.col_treeView_bookPath.Size = new System.Drawing.Size(348, 579);
            this.col_treeView_bookPath.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1262, 673);
            this.Controls.Add(this.col_treeView_bookPath);
            this.Controls.Add(this.col_button_test);
            this.Controls.Add(this.col_richTextBox_bookText);
            this.Controls.Add(this.col_menuStrip);
            this.MainMenuStrip = this.col_menuStrip;
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "简易书籍阅读器";
            this.col_menuStrip.ResumeLayout(false);
            this.col_menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button col_button_test;
        private System.Windows.Forms.MenuStrip col_menuStrip;
        private System.Windows.Forms.ToolStripMenuItem col_ToolStripMenuItem_File;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_File_New;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_File_Open;
        private System.Windows.Forms.ToolStripSeparator col_MenuItem_File_sp1;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_File_Save;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_File_Another;
        private System.Windows.Forms.ToolStripSeparator col_MenuItem_File_sp2;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_File_Print;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_File_PrintView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_File_Exit;
        private System.Windows.Forms.ToolStripMenuItem col_ToolStripMenuItem_Editor;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_Editor_Revoke;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_Editor_Repeat;
        private System.Windows.Forms.ToolStripSeparator col_MenuItem_Editor_sp1;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_Editor_SubTrim;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_Editor_Copy;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_Editor_CopyTo;
        private System.Windows.Forms.ToolStripSeparator col_MenuItem_Editor_sp2;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_Editor_AllSelect;
        private System.Windows.Forms.ToolStripMenuItem col_ToolStripMenuItem_Tool;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_Tool_Custom;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_Tool_Preferences;
        private System.Windows.Forms.ToolStripMenuItem col_ToolStripMenuItem_Help;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_Help_Neirong;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_Help_Index;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_Help_Scan;
        private System.Windows.Forms.ToolStripSeparator col_MenuItem_Help_sp1;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_Help_About;
        private System.Windows.Forms.RichTextBox col_richTextBox_bookText;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_File_LookInf;
        private System.Windows.Forms.OpenFileDialog col_openFileDialog;
        private System.Windows.Forms.TreeView col_treeView_bookPath;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_Tool_PreSaveTo;
        private System.Windows.Forms.SaveFileDialog col_saveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem col_MenuItem_Tool_PreRead;
    }
}

