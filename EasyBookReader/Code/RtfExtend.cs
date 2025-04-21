using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace Cheng
{

    public static class RtfExtend
    {

        #region 图像添加

        /*

{\pict{\*\picprop{\sp{\sn wzDescription}{\sv windows\'d3\'a6\'d3\'c3\'cd\'bc\'b1\'ea_\'b1\'a3\'b4\'e6\'b0\'b4\'c5\'a5.png}}{\sp{\sn posv}{\sv 1}}
}\pngblip\picw342\pich342\picwgoal194\pichgoal194
89504e470d0a1a0a0000000d49484452000000100000001008060000001ff3ff61000000017352
474200aece1ce90000000467414d410000b18f0bfc610500000009704859730000127400001274
01de661f78000001d149444154384fa5523d4f1b41107d80d119dbe23720440f1229a185486968
e841221d69031290c2a6a2e1e32fa0fc8484c61d961096a0c348b1532045420829c1e66eef76f7
eecccc9c7d38d8058227cdcddeecbcb733b38bf762883f273fbd36fb308c616d0c6322b2105a47
62d63eaf39cefb9c7770383d94e912f3f9021a0d8df52f0e8706c2d72196574e318e5627020cf3
871519beef8b1f0426bbcac23506bfea27882223f15709a464dfc0b3168b0b9bff0b0441283f4a
29f1bde825bb8a8c2a08828004acec8b80a624c6cb0afac89d163ce5218a7b04ba2d68adc53306
9139e689804a5b905be0eb896360696916ab6b5528ea9313e5345ab39735d9a7c90904fab90579
07bba57a7b66668a2ae13944b8bf0fa44f4301aeaabb7e683d1099fe0dedbbd72897379277c043
e4f9294565bb06e58b1d0ea7709c518c651d129b97d359a0ef1a59a0d934787c4c06f9e3fb3eee
6e6a287ddb4471eb2b0ab92cfe796e723a55d1279094ef4ba98c0f738b640bc83919228fa290cf
a145d3e70ab4d5883b33489fb247ea4c3eae5410fdad61afb88dac33823f37bfc96770757946a4
8f30ed98f243222607c910199fd74edbcde6ad94c613e67b4ed7e20dacf124378a0d898538af1e
a5fc37027802c69a8e3a1e8e84b50000000049454e44ae426082
}\par

        bmp

\f1\lang2052{\pict{\*\picprop{\sp{\sn wzDescription}{\sv Honeyview_windows\'d3\'a6\'d3\'c3\'cd\'bc\'b1\'ea_\'b8\'b4\'d6\'c6.bmp}}{\sp{\sn posv}{\sv 1}}
}\pngblip\picw423\pich423\picwgoal240\pichgoal240
89504e470d0a1a0a0000000d494844520000001000000010080200000090916836000000017352
474200aece1ce90000000467414d410000b18f0bfc6105000000097048597300000ec300000ec3
01c76fa864000001e849444154384f95924f4854411cc7e7addb161a5e822ed5415864d5a2843c
051584ec6e270579073330c82e859d82240511f4627fe8e05949a42ebae4454191c874c1432214
6dbba6a021b88bbb6fdfcedb37f3e65fbfb76f51a325f0c3bcefbcdf6f7ebff9fd66184d29854e
8236fb390113171c945107b871adaeeed2f9d26a2520018a1cf2ee63fcd1d0d4cf5f3b65fb1f7c
82bb7b7b08a9b0853bee5c1d1a9b4e6eed96bd7fe37718836921857011056b4dc8c7ea625b5bf7
c66f14df344c5bc26af8fad9fa0b81523c4293b371af1675b86192bd7461e68be1793c86df675a
7a56d65379cff451ea40dac42a1b5bc26f170f5ecd65083d6a123828a881874d5dfd9fbe26b260
fa1973a075bd592bd280656b45727af587dbe4684c8046ae58592c9ebf5e568275f6cd2562f7fc
9c73c6c4cbf9b459a08c713852e85c0d843e6baf02cd5bd52ff440baf5d6f75472747cc3ad001f
65426f3e451c8d504668d5f23702cec10f3867e0ac55b06c1bcc689344c2adec26b4ea8f410fb9
19ed05edd76b72e699fd5c6dc660794ba4f79352b8a7f53fb91f81518a2cd333b206fae0cda6b7
b74738a494742fc3e7d9c7c963623bf27610dfbdcca28d34d2500c872c25a5926e4b151e5fe4e9
c2ce9e013f9213c9a9e44c490ab70415b697fa4efc5a2bb4f43f10fa035dc943e341fde1380000
000049454e44ae426082
}\par


        {\pict{\*\picprop{\sp{\sn wzDescription}{\sv Honeyview_windows\'d3\'a6\'d3\'c3\'cd\'bc\'b1\'ea_\'b8\'b4\'d6\'c6.jpg}}{\sp{\sn posv}{\sv 1}}
}\jpegblip\picw423\pich423\picwgoal240\pichgoal240
5acf0f96e2250739d68d24e3cd179970de1a94a9e6b83c14b1d9a617179656872d2caa9e2ead0c
2e2f091ad4e18bad4b0b2e78e029d4a74fdae329a9462a94a6e32e57f0d5ff00c13ff82857c4ef
8c3f06354f1d7ecd5f13349f0de83fb44fecdde38d7757d67e377817c43e1bf0b785be1d7c7ff8
6de3ff00156b52693e29fdbabe36ea937f677853c35ad35bdaf85fc1b73e21bdb830d8e9e562b9
b8b5b9fd5733e2ff000f70f91e6783ca336c9beb15728e20c3d3782c97ea788c5d6cc387337cab
0f87ff0084cf0c384292557138fa32954c4e323878284a7529392a73a7f1f82c878a6ae6783c4e
6184c5ba30c6e5d5651ab9855af4e8c30d9960b1752af2e378c33cd614b0f34951c3fb6949a8c2
718b9c67ffd9
}\par

        */

        /// <summary>
        /// （待实现函数）将给定的文本添加到富文本输入框的末端，然后换行
        /// </summary>
        /// <param name="rtb">富文本输入框</param>
        /// <param name="text">要添加的一段文本，该文本是在输入框用户界面显示的文本</param>
        public static void appendTextLine(this RichTextBox rtb, string text)
        {
            //待实现
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
        /// <param name="memoryStream">临时数据缓冲区；如果需要用到临时流，请尽量使用这个，而不是直接创建；该临时流可在函数体内随意更改</param>
        public static void appendImageLine(this RichTextBox rtb, Image image, float boxWidthRatio, float boxHeightRatio, MemoryStream memoryStream)
        {
            //return;
            //rtb.appendTextLine("[图像添加功能暂时无法使用]");
            //return;
            var ro = rtb.ReadOnly;
            if(ro) rtb.ReadOnly = false;
            Clipboard.SetImage(image);
            rtb.Paste();
            Clipboard.Clear();
            rtb.AppendText(Environment.NewLine);
            if (ro) rtb.ReadOnly = true;
            return;

            #region 我是傻逼
            //待实现
            // 计算目标尺寸
            int targetWidth = (int)(rtb.ClientSize.Width * boxWidthRatio);
            int targetHeight = (int)(rtb.ClientSize.Height * boxHeightRatio);

            using (Bitmap scaledImage = new Bitmap(targetWidth, targetHeight))
            {
                // 保持原图分辨率
                scaledImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (Graphics g = Graphics.FromImage(scaledImage))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, targetWidth, targetHeight);
                }


                // 重置内存流并保存为PNG
                memoryStream.SetLength(0);
                memoryStream.Seek(0, SeekOrigin.Begin);
                scaledImage.Save(memoryStream, ImageFormat.Png);
                //byte[] imageData = memoryStream.ToArray();

                // 计算RTF尺寸参数
                float horizontalDpi = scaledImage.HorizontalResolution;
                float verticalDpi = scaledImage.VerticalResolution;

                // 原始尺寸（百分之一毫米）
                int picw = (int)((targetWidth / horizontalDpi) * 25.4f * 100);
                int pich = (int)((targetHeight / verticalDpi) * 25.4f * 100);

                // 目标尺寸（twips）
                int picwgoal = (int)((targetWidth / horizontalDpi) * 1440);
                int pichgoal = (int)((targetHeight / verticalDpi) * 1440);

                // 生成十六进制字符串
                memoryStream.Seek(0, SeekOrigin.Begin);
                string hex;
                using (var reader = CreateBinTextByStream(memoryStream))
                {
                    hex = reader.ReadToEnd();
                }
                //hex = BitConverter.ToString(imageData).Replace("-", "");

                /*

                {{\sv windows\'d3\'a6\'d3\'c3\'cd\'bc\'b1\'ea_\'b1\'a3\'b4\'e6\'b0\'b4\'c5\'a5.png}}

                \f1\lang2052{\pict{\*\picprop{\sp{\sn wzDescription}{\sv Honeyview_windows\'d3\'a6\'d3\'c3\'cd\'bc\'b1\'ea_\'b8\'b4\'d6\'c6.bmp}}{\sp{\sn posv}{\sv 1}}

                {\pict{\*\picprop{\sp{\sn wzDescription}}{\sp{\sn posv}{\sv 1}}
}\pngblip\picw342\pich342\picwgoal194\pichgoal194
                */

                // 构造RTF代码
                string rtf;

                rtf = $@"\f1\lang2052{{\pict{{\*\picprop{{\sp{{\sn wzDescription}}{{\sv windows\'a5.png}}}}{{\sp{{\sn posv}}{{\sv 1}}}}}}\pngblip\picw{picw}\pich{pich}\picwgoal{picwgoal}\picwgoal{pichgoal}{Environment.NewLine}{hex}}}\par";

                //rtf = $@"{{\pict\pngblip\picw{picw}\pich{pich}\picwgoal{picwgoal}\pichgoal{pichgoal}{hex}}}\par";
                //rtb.SelectedRtf;
                // 插入到富文本框
                rtb.SelectionStart = rtb.TextLength;
                rtb.SelectedRtf = rtf;
                //rtb.Rtf += rtf;
            }
            #endregion

        }

        /// <summary>
        /// 读取流中的所有数据，并将二进制数据按顺序转化为文本模式并返回
        /// </summary>
        /// <param name="stream">要封装的流</param>
        /// <returns>创建一个文本读取器，可以将stream内的字节数据依次转化为与之相对应的二进制文本并读取，例如FEA020；对象不再使用后需要手动关闭</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="NotSupportedException"><paramref name="stream"/>没有读取权限</exception>
        static TextReader CreateBinTextByStream(Stream stream)
        {
            return new Cheng.IO.StreamBytesReader(stream, false, null, false, 1024 * 2);
        }


        #endregion

    }

}
