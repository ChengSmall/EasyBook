using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using Cheng.Algorithm;

namespace Cheng
{

    public static class RtfExtend
    {

        #region 图像添加

        /// <summary>
        /// （待实现函数）将给定的文本添加到富文本输入框的末端，然后换行
        /// </summary>
        /// <param name="rtb">富文本输入框</param>
        /// <param name="text">要添加的一段文本，该文本是在输入框用户界面显示的文本</param>
        public static void appendTextLine(this RichTextBox rtb, string text)
        {
            rtb.AppendText(text + Environment.NewLine);
        }

        /// <summary>
        /// 将图像输入添加到富文本输入框内文本的末端，然后换行
        /// </summary>
        /// <remarks>
        /// <para>该函数不会修改用户的剪切板</para>
        /// </remarks>
        /// <param name="rtb">富文本输入框</param>
        /// <param name="image">要添加的图像</param>
        /// <param name="boxWidthRatio">图像长度与文本框长度的比例，区间在（0-1]；越接近0图像长度越短，越接近1图像越长，等于1时图像的长度文本框长度；无论长度多少，图像的整体都会显示在文本框之内</param>
        /// <param name="boxHeightRatio">图像高度与文本框高度的比例，区间在（0-1]；越接近0图像高度越低，越接近1图像越高，等于1时图像的高度文本框高度；无论高度多少，图像的整体都会显示在文本框之内</param>
        /// <param name="memoryStream">临时数据缓冲区</param>
        public static void appendImageLine(this RichTextBox rtb, Image image, float boxWidthRatio, float boxHeightRatio, MemoryStream memoryStream)
        {
            lock (rtb)
            {
                var ro = rtb.ReadOnly;
                if (ro) rtb.ReadOnly = false;
                //rtb.appendTextLine("[图像添加功能暂时无法使用]");
                f_appendImageLine(rtb, image, boxWidthRatio, boxHeightRatio, memoryStream);
                if (ro) rtb.ReadOnly = true;
            }
            return;
            //var ro = rtb.ReadOnly;
            //if(ro) rtb.ReadOnly = false;
            //Clipboard.SetImage(image);
            //rtb.Paste();
            //Clipboard.Clear();
            //rtb.AppendText(Environment.NewLine);
            //if (ro) rtb.ReadOnly = true;
            //return;
        }

        private static void f_appendImageLine(RichTextBox rtb, Image image, float boxWidthRatio, float boxHeightRatio, MemoryStream memoryStream)
        {
            // 参数校验
            if (rtb == null || image == null || memoryStream == null) throw new ArgumentNullException();
            boxWidthRatio = boxWidthRatio.Clamp(0, 1);
            boxHeightRatio = boxHeightRatio.Clamp(0, 1);

            // 获取文本框可用宽高
            int availableWidth = rtb.ClientSize.Width;
            int availableHeight = rtb.ClientSize.Height;
            if (availableWidth <= 0 || availableHeight <= 0) return; // 无可用空间

            // 计算目标最大宽高
            float maxWidth = availableWidth * boxWidthRatio;
            float maxHeight = availableHeight * boxHeightRatio;

            // 计算等比缩放比例，确保图像整体不超出目标区域
            float widthRatio = maxWidth / image.Width;
            float heightRatio = maxHeight / image.Height;
            float scale = Math.Min(widthRatio, heightRatio);

            // 计算缩放后的像素尺寸（至少为 1 像素）
            int newWidth = (int)Math.Round(image.Width * scale);
            int newHeight = (int)Math.Round(image.Height * scale);
            if (newWidth <= 0) newWidth = 1;
            if (newHeight <= 0) newHeight = 1;

            // 创建缩放后的位图
            using (Bitmap bmp = new Bitmap(newWidth, newHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.DrawImage(image, 0, 0, newWidth, newHeight);
                }

                // 使用传入的临时流保存 PNG
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.SetLength(0);
                bmp.Save(memoryStream, ImageFormat.Png);
            }
            // 转换为十六进制字符串
            StringBuilder hexBuilder = new StringBuilder((int)(memoryStream.Length * 2));

            memoryStream.Seek(0, SeekOrigin.Begin);
            while (true)
            {
                var sri = memoryStream.ReadByte();
                if (sri < 0)
                {
                    break;
                }
                Cheng.Texts.TextManipulation.ValueToX16Char((byte)sri, true, out char left, out char r);
                hexBuilder.Append(left);
                hexBuilder.Append(r);
            }

            string hexData = hexBuilder.ToString();

            // 计算 RTF 中使用的 twips 尺寸（1 像素 = 15 twips，基于 96 DPI）
            const int twipsPerPixel = 15;
            int picwgoal = newWidth * twipsPerPixel;
            int pichgoal = newHeight * twipsPerPixel;


            // 构造当前图片对应的 RTF 片段，注意末尾只有一个 '}'，hex 后面不加空格
            string pictFragment =
                $@"{{\pict\pngblip\picw{newWidth}\pich{newHeight}\picwgoal{picwgoal}\pichgoal{pichgoal} {hexData}}}\par ";

            // 关键：取现有完整 RTF，去掉尾部 '}'，拼上片段，再补回 '}'
            string currentRtf = rtb.Rtf;

            // 去掉末尾的空白和最后一个 '}'

            int end = currentRtf.Length - 1;
            while (end >= 0 && char.IsWhiteSpace(currentRtf[end]))
            {
                end--;
            }

            if (end >= 0 && currentRtf[end] == '}') end--;
            string head = currentRtf.Substring(0, end + 1);

            rtb.Rtf = head + pictFragment + "}";
            rtb.AppendText(Environment.NewLine);
        }

        #endregion

    }

}
