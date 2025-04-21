using Cheng.Algorithm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cheng.EasyBooks.Controls
{
    public partial class SolidColorBlock : UserControl
    {

        public SolidColorBlock()
        {
            f_lastInit();
            InitializeComponent();
            f_init();
        }

        #region

        #region 初始化

        private void f_lastInit()
        {
        }

        private void f_init()
        {
            p_lineColorPen = new Pen(Color.Black, 1);
        }

        #endregion

        #region 参数

        private Pen p_lineColorPen;

        #endregion

        #region 控件参数访问

        /// <summary>
        /// 色块边界线的颜色
        /// </summary>
        [AmbientValue(typeof(Color), "0, 0, 0")]
        [Category("Appearance")]
        [Description("色块边界线的颜色")]
        public Color LineColor
        {
            get => p_lineColorPen.Color;
            set
            {
                p_lineColorPen.Color = value;
            }
        }

        [AmbientValue(1f)]
        [Category("Appearance")]
        [Description("色块边界线的宽度")]
        public float LineWidth
        {
            get
            {
                return p_lineColorPen.Width;
            }
            set
            {
                if (value <= 0) value = 1E-5f;
                p_lineColorPen.Width = value;
            }
        }

        #endregion

        #region 释放

        private void f_disepose(bool disposeing)
        {
            if (disposeing)
            {
                p_lineColorPen?.Dispose();
            }
        }

        #endregion

        #region 控件参数

        #endregion

        #region 事件注册

        #endregion

        #region 功能封装

        #endregion

        #region 重写

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawRectangle(p_lineColorPen, e.ClipRectangle);
        }

        #endregion

        #endregion


    }
}
