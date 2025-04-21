
namespace Cheng.EasyBooks.WinForms
{
    partial class PreferencesWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            f_disepose(disposing);

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.col_colorDialog = new System.Windows.Forms.ColorDialog();
            this.col_fontDialog = new System.Windows.Forms.FontDialog();
            this.col_solidColorBlock_backColor = new Cheng.EasyBooks.Controls.SolidColorBlock();
            this.col_button_backColor = new System.Windows.Forms.Button();
            this.col_label_backColor = new System.Windows.Forms.Label();
            this.col_label_volumeColor = new System.Windows.Forms.Label();
            this.col_button_volumeColor = new System.Windows.Forms.Button();
            this.col_solidColorBlock_volumeColor = new Cheng.EasyBooks.Controls.SolidColorBlock();
            this.col_label_rtfColor = new System.Windows.Forms.Label();
            this.col_button_rtfColor = new System.Windows.Forms.Button();
            this.col_solidColorBlock_rtfColor = new Cheng.EasyBooks.Controls.SolidColorBlock();
            this.col_label_volumeTextColor = new System.Windows.Forms.Label();
            this.col_button_volumeTextColor = new System.Windows.Forms.Button();
            this.col_solidColorBlock_volumeTextColor = new Cheng.EasyBooks.Controls.SolidColorBlock();
            this.col_label_rtfTextColor = new System.Windows.Forms.Label();
            this.col_button_rtfTextColor = new System.Windows.Forms.Button();
            this.col_solidColorBlock_rtfTextColor = new Cheng.EasyBooks.Controls.SolidColorBlock();
            this.col_button_volumeTextFont = new System.Windows.Forms.Button();
            this.col_button_rtfFont = new System.Windows.Forms.Button();
            this.col_button_ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // col_solidColorBlock_backColor
            // 
            this.col_solidColorBlock_backColor.LineColor = System.Drawing.Color.Black;
            this.col_solidColorBlock_backColor.LineWidth = 4F;
            this.col_solidColorBlock_backColor.Location = new System.Drawing.Point(131, 41);
            this.col_solidColorBlock_backColor.Name = "col_solidColorBlock_backColor";
            this.col_solidColorBlock_backColor.Size = new System.Drawing.Size(56, 44);
            this.col_solidColorBlock_backColor.TabIndex = 0;
            // 
            // col_button_backColor
            // 
            this.col_button_backColor.Location = new System.Drawing.Point(31, 46);
            this.col_button_backColor.Name = "col_button_backColor";
            this.col_button_backColor.Size = new System.Drawing.Size(87, 34);
            this.col_button_backColor.TabIndex = 1;
            this.col_button_backColor.Text = "选择";
            this.col_button_backColor.UseVisualStyleBackColor = true;
            // 
            // col_label_backColor
            // 
            this.col_label_backColor.AutoSize = true;
            this.col_label_backColor.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.col_label_backColor.Location = new System.Drawing.Point(204, 56);
            this.col_label_backColor.Name = "col_label_backColor";
            this.col_label_backColor.Size = new System.Drawing.Size(76, 17);
            this.col_label_backColor.TabIndex = 2;
            this.col_label_backColor.Text = "背景色调";
            // 
            // col_label_volumeColor
            // 
            this.col_label_volumeColor.AutoSize = true;
            this.col_label_volumeColor.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.col_label_volumeColor.Location = new System.Drawing.Point(203, 137);
            this.col_label_volumeColor.Name = "col_label_volumeColor";
            this.col_label_volumeColor.Size = new System.Drawing.Size(110, 17);
            this.col_label_volumeColor.TabIndex = 5;
            this.col_label_volumeColor.Text = "目录页面颜色";
            // 
            // col_button_volumeColor
            // 
            this.col_button_volumeColor.Location = new System.Drawing.Point(31, 128);
            this.col_button_volumeColor.Name = "col_button_volumeColor";
            this.col_button_volumeColor.Size = new System.Drawing.Size(87, 34);
            this.col_button_volumeColor.TabIndex = 4;
            this.col_button_volumeColor.Text = "选择";
            this.col_button_volumeColor.UseVisualStyleBackColor = true;
            // 
            // col_solidColorBlock_volumeColor
            // 
            this.col_solidColorBlock_volumeColor.LineColor = System.Drawing.Color.Black;
            this.col_solidColorBlock_volumeColor.LineWidth = 4F;
            this.col_solidColorBlock_volumeColor.Location = new System.Drawing.Point(131, 123);
            this.col_solidColorBlock_volumeColor.Name = "col_solidColorBlock_volumeColor";
            this.col_solidColorBlock_volumeColor.Size = new System.Drawing.Size(56, 44);
            this.col_solidColorBlock_volumeColor.TabIndex = 3;
            // 
            // col_label_rtfColor
            // 
            this.col_label_rtfColor.AutoSize = true;
            this.col_label_rtfColor.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.col_label_rtfColor.Location = new System.Drawing.Point(203, 294);
            this.col_label_rtfColor.Name = "col_label_rtfColor";
            this.col_label_rtfColor.Size = new System.Drawing.Size(127, 17);
            this.col_label_rtfColor.TabIndex = 8;
            this.col_label_rtfColor.Text = "文本框页面颜色";
            // 
            // col_button_rtfColor
            // 
            this.col_button_rtfColor.Location = new System.Drawing.Point(31, 285);
            this.col_button_rtfColor.Name = "col_button_rtfColor";
            this.col_button_rtfColor.Size = new System.Drawing.Size(87, 34);
            this.col_button_rtfColor.TabIndex = 7;
            this.col_button_rtfColor.Text = "选择";
            this.col_button_rtfColor.UseVisualStyleBackColor = true;
            // 
            // col_solidColorBlock_rtfColor
            // 
            this.col_solidColorBlock_rtfColor.LineColor = System.Drawing.Color.Black;
            this.col_solidColorBlock_rtfColor.LineWidth = 4F;
            this.col_solidColorBlock_rtfColor.Location = new System.Drawing.Point(131, 280);
            this.col_solidColorBlock_rtfColor.Name = "col_solidColorBlock_rtfColor";
            this.col_solidColorBlock_rtfColor.Size = new System.Drawing.Size(56, 44);
            this.col_solidColorBlock_rtfColor.TabIndex = 6;
            // 
            // col_label_volumeTextColor
            // 
            this.col_label_volumeTextColor.AutoSize = true;
            this.col_label_volumeTextColor.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.col_label_volumeTextColor.Location = new System.Drawing.Point(203, 217);
            this.col_label_volumeTextColor.Name = "col_label_volumeTextColor";
            this.col_label_volumeTextColor.Size = new System.Drawing.Size(144, 17);
            this.col_label_volumeTextColor.TabIndex = 11;
            this.col_label_volumeTextColor.Text = "目录页面文字颜色";
            // 
            // col_button_volumeTextColor
            // 
            this.col_button_volumeTextColor.Location = new System.Drawing.Point(31, 208);
            this.col_button_volumeTextColor.Name = "col_button_volumeTextColor";
            this.col_button_volumeTextColor.Size = new System.Drawing.Size(87, 34);
            this.col_button_volumeTextColor.TabIndex = 10;
            this.col_button_volumeTextColor.Text = "选择";
            this.col_button_volumeTextColor.UseVisualStyleBackColor = true;
            // 
            // col_solidColorBlock_volumeTextColor
            // 
            this.col_solidColorBlock_volumeTextColor.LineColor = System.Drawing.Color.Black;
            this.col_solidColorBlock_volumeTextColor.LineWidth = 4F;
            this.col_solidColorBlock_volumeTextColor.Location = new System.Drawing.Point(131, 203);
            this.col_solidColorBlock_volumeTextColor.Name = "col_solidColorBlock_volumeTextColor";
            this.col_solidColorBlock_volumeTextColor.Size = new System.Drawing.Size(56, 44);
            this.col_solidColorBlock_volumeTextColor.TabIndex = 9;
            // 
            // col_label_rtfTextColor
            // 
            this.col_label_rtfTextColor.AutoSize = true;
            this.col_label_rtfTextColor.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.col_label_rtfTextColor.Location = new System.Drawing.Point(203, 365);
            this.col_label_rtfTextColor.Name = "col_label_rtfTextColor";
            this.col_label_rtfTextColor.Size = new System.Drawing.Size(161, 17);
            this.col_label_rtfTextColor.TabIndex = 14;
            this.col_label_rtfTextColor.Text = "文本框页面文字颜色";
            // 
            // col_button_rtfTextColor
            // 
            this.col_button_rtfTextColor.Location = new System.Drawing.Point(31, 356);
            this.col_button_rtfTextColor.Name = "col_button_rtfTextColor";
            this.col_button_rtfTextColor.Size = new System.Drawing.Size(87, 34);
            this.col_button_rtfTextColor.TabIndex = 13;
            this.col_button_rtfTextColor.Text = "选择";
            this.col_button_rtfTextColor.UseVisualStyleBackColor = true;
            // 
            // col_solidColorBlock_rtfTextColor
            // 
            this.col_solidColorBlock_rtfTextColor.LineColor = System.Drawing.Color.Black;
            this.col_solidColorBlock_rtfTextColor.LineWidth = 4F;
            this.col_solidColorBlock_rtfTextColor.Location = new System.Drawing.Point(131, 351);
            this.col_solidColorBlock_rtfTextColor.Name = "col_solidColorBlock_rtfTextColor";
            this.col_solidColorBlock_rtfTextColor.Size = new System.Drawing.Size(56, 44);
            this.col_solidColorBlock_rtfTextColor.TabIndex = 12;
            // 
            // col_button_volumeTextFont
            // 
            this.col_button_volumeTextFont.Location = new System.Drawing.Point(396, 208);
            this.col_button_volumeTextFont.Name = "col_button_volumeTextFont";
            this.col_button_volumeTextFont.Size = new System.Drawing.Size(87, 34);
            this.col_button_volumeTextFont.TabIndex = 15;
            this.col_button_volumeTextFont.Text = "选择字体";
            this.col_button_volumeTextFont.UseVisualStyleBackColor = true;
            // 
            // col_button_rtfFont
            // 
            this.col_button_rtfFont.Location = new System.Drawing.Point(396, 356);
            this.col_button_rtfFont.Name = "col_button_rtfFont";
            this.col_button_rtfFont.Size = new System.Drawing.Size(87, 34);
            this.col_button_rtfFont.TabIndex = 16;
            this.col_button_rtfFont.Text = "选择字体";
            this.col_button_rtfFont.UseVisualStyleBackColor = true;
            // 
            // col_button_ok
            // 
            this.col_button_ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.col_button_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.col_button_ok.Location = new System.Drawing.Point(244, 425);
            this.col_button_ok.Name = "col_button_ok";
            this.col_button_ok.Size = new System.Drawing.Size(109, 33);
            this.col_button_ok.TabIndex = 17;
            this.col_button_ok.Text = "确定";
            this.col_button_ok.UseVisualStyleBackColor = true;
            // 
            // PreferencesWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(582, 484);
            this.Controls.Add(this.col_button_ok);
            this.Controls.Add(this.col_button_rtfFont);
            this.Controls.Add(this.col_button_volumeTextFont);
            this.Controls.Add(this.col_label_rtfTextColor);
            this.Controls.Add(this.col_button_rtfTextColor);
            this.Controls.Add(this.col_solidColorBlock_rtfTextColor);
            this.Controls.Add(this.col_label_volumeTextColor);
            this.Controls.Add(this.col_button_volumeTextColor);
            this.Controls.Add(this.col_solidColorBlock_volumeTextColor);
            this.Controls.Add(this.col_label_rtfColor);
            this.Controls.Add(this.col_button_rtfColor);
            this.Controls.Add(this.col_solidColorBlock_rtfColor);
            this.Controls.Add(this.col_label_volumeColor);
            this.Controls.Add(this.col_button_volumeColor);
            this.Controls.Add(this.col_solidColorBlock_volumeColor);
            this.Controls.Add(this.col_label_backColor);
            this.Controls.Add(this.col_button_backColor);
            this.Controls.Add(this.col_solidColorBlock_backColor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 480);
            this.Name = "PreferencesWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "首选项";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColorDialog col_colorDialog;
        private System.Windows.Forms.FontDialog col_fontDialog;
        private Controls.SolidColorBlock col_solidColorBlock_backColor;
        private System.Windows.Forms.Button col_button_backColor;
        private System.Windows.Forms.Label col_label_backColor;
        private System.Windows.Forms.Label col_label_volumeColor;
        private System.Windows.Forms.Button col_button_volumeColor;
        private Controls.SolidColorBlock col_solidColorBlock_volumeColor;
        private System.Windows.Forms.Label col_label_rtfColor;
        private System.Windows.Forms.Button col_button_rtfColor;
        private Controls.SolidColorBlock col_solidColorBlock_rtfColor;
        private System.Windows.Forms.Label col_label_volumeTextColor;
        private System.Windows.Forms.Button col_button_volumeTextColor;
        private Controls.SolidColorBlock col_solidColorBlock_volumeTextColor;
        private System.Windows.Forms.Label col_label_rtfTextColor;
        private System.Windows.Forms.Button col_button_rtfTextColor;
        private Controls.SolidColorBlock col_solidColorBlock_rtfTextColor;
        private System.Windows.Forms.Button col_button_volumeTextFont;
        private System.Windows.Forms.Button col_button_rtfFont;
        private System.Windows.Forms.Button col_button_ok;
    }
}