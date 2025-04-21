using Cheng.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cheng.EasyBooks.WinForms
{

    /// <summary>
    /// 首选项设置
    /// </summary>
    public partial class PreferencesWindow : Form
    {

        public PreferencesWindow()
        {
            f_lastInit();
            InitializeComponent();
            f_init();
        }

        #region

        #region 结构

        /// <summary>
        /// 首选项参数
        /// </summary>
        public sealed class PreferencesValue
        {
            public PreferencesValue()
            {
            }

            #region

            /// <summary>
            /// 富文本框字体
            /// </summary>
            public Font rtfFont;

            /// <summary>
            /// 分卷字体
            /// </summary>
            public Font volumeFont;

            /// <summary>
            /// 背景色
            /// </summary>
            public Color backColor;

            /// <summary>
            /// 富文本框背景色
            /// </summary>
            public Color rtfColor;

            /// <summary>
            /// 富文本框文本颜色
            /// </summary>
            public Color rtfTextColor;

            /// <summary>
            /// 分卷树背景色
            /// </summary>
            public Color volumeColor;

            /// <summary>
            /// 分卷树文本颜色
            /// </summary>
            public Color volumeTextColor;

            /// <summary>
            /// 主窗口大小
            /// </summary>
            public Size? winSize;

            #endregion

            #region 序列化

            /// <summary>
            /// 对字体记录json数据
            /// </summary>
            /// <param name="font"></param>
            /// <returns></returns>
            static JsonDictionary FontToJson(Font font)
            {
                
                JsonDictionary jobj = new JsonDictionary(6);
                jobj.Add("FontFamily", font.FontFamily.Name);
                jobj.Add("Size", font.Size);
                jobj.Add("Style", (int)font.Style);
                jobj.Add("Unit", (int)font.Unit);
                jobj.Add("GdiCharSet", font.GdiCharSet);
                jobj.Add("GdiVerticalFont", font.GdiVerticalFont);
                return jobj;
            }

            /// <summary>
            /// 返回字体文件，参数无效则返回null
            /// </summary>
            /// <param name="json"></param>
            /// <returns></returns>
            static Font JsonToFont(JsonVariable json)
            {
                /*
                {
                  "FontFamily": "Arial",           // 字体家族名称
                  "Size": 12.0,                    // 字体尺寸
                  "Style": ["Bold", "Italic"],     // 字体样式组合（可选，默认Regular）
                  "Unit": "Point",                 // 尺寸单位（可选，默认Point）
                  "GdiCharSet": 1,                 // 字符集（可选，默认0）
                  "GdiVerticalFont": false         // 是否为垂直字体（可选，默认false）
                }
                */
                Font font = null;
                try
                {
                    string FontFamily;
                    float Size;
                    FontStyle Style = FontStyle.Regular;
                    GraphicsUnit Unit = GraphicsUnit.Point;
                    byte GdiCharSet = 0;
                    bool GdiVerticalFont = false;

                    var jobj = json.JsonObject;

                    FontFamily = jobj["FontFamily"].String;

                    var size = jobj["Size"];
                    if(size.DataType == JsonType.RealNum)
                    {
                        Size = (float)size.RealNum;
                    }
                    else
                    {
                        Size = size.Integer;
                    }
                    
                    if (jobj.TryGetValue("Style", out json))
                    {
                        Style = (FontStyle)json.Integer;
                    }

                    if (jobj.TryGetValue("Unit", out json))
                    {
                        Unit = (GraphicsUnit)json.Integer;
                    }

                    if (jobj.TryGetValue("GdiCharSet", out json))
                    {
                        GdiCharSet = (byte)json.Integer;
                    }

                    if (jobj.TryGetValue("GdiVerticalFont", out json))
                    {
                        GdiVerticalFont = json.Boolean;
                    }

                    font = new Font(FontFamily, Size, Style, Unit, GdiCharSet, GdiVerticalFont);

                    return font;
                }
                catch (Exception)
                {
                    font?.Dispose();
                    return null;
                }
            }

            /// <summary>
            /// 将参数序列化为json
            /// </summary>
            /// <returns></returns>
            public JsonDictionary ToJson()
            {
                JsonDictionary jd = new JsonDictionary(7);

                jd.Add("backColor", (uint)backColor.ToArgb());
                jd.Add("rtfColor", (uint)rtfColor.ToArgb());
                jd.Add("rtfTextColor", (uint)rtfTextColor.ToArgb());
                jd.Add("volumeColor", (uint)volumeColor.ToArgb());
                jd.Add("volumeTextColor", (uint)volumeTextColor.ToArgb());
                if(rtfFont != null) jd.Add("rtfFont", FontToJson(rtfFont));
                if (volumeFont != null) jd.Add("volumeFont", FontToJson(volumeFont));

                if (winSize.HasValue)
                {
                    var size = winSize.Value;
                    JsonList jlist = new JsonList(2);
                    jlist.Add(size.Width);
                    jlist.Add(size.Height);
                    jd.Add("size", jlist);
                }
                return jd;
            }

            /// <summary>
            /// 将json数据转化到当前对象
            /// </summary>
            /// <param name="json"></param>
            /// <returns>各参数转化成功与否的真值</returns>
            public (bool backColor, bool rtfColor, bool rtfTextColor, bool volumeColor, bool volumeTextColor, bool rtfFont, bool volumeFont, bool size) JsonToValue(JsonVariable json)
            {
                (bool backColor, bool rtfColor, bool rtfTextColor, bool volumeColor, bool volumeTextColor, bool rtfFont, bool volumeFont, bool size) bs = default;
                var jobj = json.JsonObject;
                try
                {
                    this.backColor = Color.FromArgb(((int)jobj["backColor"].Integer));
                    bs.backColor = true;
                }
                catch (Exception)
                {
                    
                }
                try
                {
                    this.rtfColor = Color.FromArgb((int)(jobj["rtfColor"].Integer));
                    bs.rtfColor = true;
                }
                catch (Exception)
                {
                }
                try
                {
                    this.rtfTextColor = Color.FromArgb((int)jobj["rtfTextColor"].Integer);
                    bs.rtfTextColor = true;
                }
                catch (Exception)
                {
                }
                try
                {
                    this.volumeColor = Color.FromArgb((int)jobj["volumeColor"].Integer);
                    bs.volumeColor = true;
                }
                catch (Exception)
                {
                }
                try
                {
                    this.volumeTextColor = Color.FromArgb((int)jobj["volumeTextColor"].Integer);
                    bs.volumeTextColor = true;
                }
                catch (Exception)
                {
                }

                try
                {
                    var jsize = jobj["size"].Array;
                    this.winSize = new Size((int)jsize[0].Integer, (int)jsize[1].Integer);
                    bs.size = true;
                }
                catch (Exception)
                {
                }

                Font font;
                try
                {
                    
                    font = JsonToFont(jobj["rtfFont"]);
                    if (font != null)
                    {
                        this.rtfFont = font;
                        bs.rtfFont = true;
                    }
                }
                catch (Exception)
                {

                }
                try
                {
                    font = JsonToFont(jobj["volumeFont"]);
                    if (font != null)
                    {
                        this.volumeFont = font;
                        bs.volumeFont = true;
                    }
                }
                catch (Exception)
                {

                }

                return bs;
            }

            #endregion
        }

        #endregion

        #region 初始化

        private void f_lastInit()
        {
        }

        private void f_init()
        {
            col_button_backColor.Click += fe_ButtonClick_SelectColor;
            col_button_rtfColor.Click += fe_ButtonClick_SelectColor;
            col_button_rtfTextColor.Click += fe_ButtonClick_SelectColor;
            col_button_volumeTextColor.Click += fe_ButtonClick_SelectColor;
            col_button_volumeColor.Click += fe_ButtonClick_SelectColor;

            col_button_rtfFont.Click += fe_ButtonClick_SelectFont;
            col_button_volumeTextFont.Click += fe_ButtonClick_SelectFont;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 富文本框字体
        /// </summary>
        private Font p_rtfFont;

        /// <summary>
        /// 分卷结构字体
        /// </summary>
        private Font p_volumeFont;

        #endregion

        #region 控件参数访问

        #region 按钮

        private Button Col_Button_BackColor
        {
            get => col_button_backColor;
        }

        #endregion

        #region

        #endregion

        #endregion

        #region 释放

        private void f_disepose(bool disposeing)
        {

        }

        #endregion

        #region 事件注册

        #region 按钮事件

        private void fe_ButtonClick_SelectColor(object sender, EventArgs e)
        {
            Color color;
            if(sender == col_button_backColor)
            {
                col_colorDialog.Color = col_solidColorBlock_backColor.BackColor;
                if (f_ShowSelectColor(out color))
                {
                    col_solidColorBlock_backColor.BackColor = color;
                }
            }
            else if (sender == col_button_rtfTextColor)
            {
                col_colorDialog.Color = col_solidColorBlock_rtfTextColor.BackColor;
                if (f_ShowSelectColor(out color))
                {
                    col_solidColorBlock_rtfTextColor.BackColor = color;
                }
            }
            else if (sender == col_button_rtfColor)
            {
                col_colorDialog.Color = col_solidColorBlock_rtfColor.BackColor;
                if (f_ShowSelectColor(out color))
                {
                    col_solidColorBlock_rtfColor.BackColor = color;
                }
            }
            else if (sender == col_button_volumeColor)
            {
                col_colorDialog.Color = col_solidColorBlock_volumeColor.BackColor;
                if (f_ShowSelectColor(out color))
                {
                    col_solidColorBlock_volumeColor.BackColor = color;
                }
            }
            else if (sender == col_button_volumeTextColor)
            {
                col_colorDialog.Color = col_solidColorBlock_volumeTextColor.BackColor;
                if (f_ShowSelectColor(out color))
                {
                    col_solidColorBlock_volumeTextColor.BackColor = color;
                }
            }

        }

        private void fe_ButtonClick_SelectFont(object sender, EventArgs e)
        {
            Font font;
            if(sender == col_button_rtfFont)
            {
                //rtf字体
                if(p_rtfFont != null)
                {
                    col_fontDialog.Font = p_rtfFont;
                }
                if(f_ShowSelectFont(out font))
                {
                    p_rtfFont = font;
                }
            }
            else if(sender == col_button_volumeTextFont)
            {
                if(p_volumeFont != null)
                {
                    col_fontDialog.Font = p_volumeFont;
                }
                
                if (f_ShowSelectFont(out font))
                {
                    p_volumeFont = font;
                }
            }

        }

        private void fe_ButtolClick_OK(object sender, EventArgs e)
        {
            //DialogResult = DialogResult.OK;
        }

        #endregion

        #endregion

        #region 功能封装

        /// <summary>
        /// 弹窗选择颜色
        /// </summary>
        /// <param name="color"></param>
        /// <returns>是否点击确定</returns>
        private bool f_ShowSelectColor(out Color color)
        {

            var re = col_colorDialog.ShowDialog(this);
            color = col_colorDialog.Color;
            return re == DialogResult.OK;
        }

        /// <summary>
        /// 弹窗选择字体
        /// </summary>
        /// <param name="font"></param>
        /// <returns>是否点击确定</returns>
        private bool f_ShowSelectFont(out Font font)
        {
            var re = col_fontDialog.ShowDialog(this);
            if(re == DialogResult.OK)
            {
                font = col_fontDialog.Font;
                return true;
            }
            font = null;
            return false;
        }

        /// <summary>
        /// 从参数设置选单结构
        /// </summary>
        /// <param name="args"></param>
        public void SetArgs(PreferencesValue args)
        {
            f_setBackColorToWindows(args.backColor);

            col_solidColorBlock_backColor.BackColor = args.backColor;
            col_solidColorBlock_rtfColor.BackColor = args.rtfColor;

            col_solidColorBlock_rtfColor.BackColor = args.rtfColor;
            col_solidColorBlock_rtfTextColor.BackColor = args.rtfTextColor;

            col_solidColorBlock_volumeColor.BackColor = args.volumeColor;
            col_solidColorBlock_volumeTextColor.BackColor = args.volumeTextColor;

            p_rtfFont = args.rtfFont;
            p_volumeFont = args.volumeFont;

        }

        /// <summary>
        /// 从窗体获取设置参数
        /// </summary>
        /// <returns></returns>
        public PreferencesValue GetArgs()
        {
            PreferencesValue args = new PreferencesValue();
            args.backColor = col_solidColorBlock_backColor.BackColor;

            args.rtfColor = col_solidColorBlock_rtfColor.BackColor;
            args.rtfTextColor = col_solidColorBlock_rtfTextColor.BackColor;

            args.volumeColor = col_solidColorBlock_volumeColor.BackColor;
            args.volumeTextColor = col_solidColorBlock_volumeTextColor.BackColor;

            args.volumeFont = p_volumeFont;
            args.rtfFont = p_rtfFont;
            return args;
        }

        /// <summary>
        /// 设置该窗口的背景色，并按亮度指定内容颜色
        /// </summary>
        /// <param name="backColor"></param>
        private void f_setBackColorToWindows(Color backColor)
        {
            var L = backColor.GetBrightness();

            this.BackColor = backColor;

            Color setColor;
            if(L < 0.5)
            {
                setColor = Color.White;
      
            }
            else if(L > 0.51)
            {
                setColor = Color.Black;
            }
            else
            {
                return;
            }

            col_solidColorBlock_backColor.LineColor = setColor;
            col_label_backColor.ForeColor = setColor;
            col_label_rtfColor.ForeColor = setColor;
            col_label_rtfTextColor.ForeColor = setColor;
            col_label_volumeColor.ForeColor = setColor;
            col_label_volumeTextColor.ForeColor = setColor;
        }

        #endregion

        #region 重写

        #endregion

        #endregion

    }
}
