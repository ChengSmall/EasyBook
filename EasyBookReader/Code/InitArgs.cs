using Cheng.Streams.Parsers;
using Cheng.Streams.Parsers.Default;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Cheng
{

    /// <summary>
    /// C#程序基本参数
    /// </summary>
    public unsafe class InitArgs
    {

        #region

        static InitArgs sp_initArgs;

        public static InitArgs Args
        {
            get
            {
                return sp_initArgs;
            }
        }

        public static void Init(string[] args)
        {
            sp_initArgs = new InitArgs(args);
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="args">命令行参数</param>
        public InitArgs(string[] args)
        {
            commandArgs = args;

            currentDomain = AppDomain.CurrentDomain;
            
            applicationName = currentDomain.FriendlyName;

            isX64 = sizeof(void*) == 8;

            appConfigName = applicationName + ".config";

            rootDirectory = currentDomain.BaseDirectory;
            isOpenDebug = false;
            debugLogPrint = null;

            commandLine = Environment.CommandLine;
            //setUpFilePath = Path.Combine(rootDirectory, "state.sav");

            defPreferencesFilePath = Path.Combine(rootDirectory, defPreferencesFileName);

            f_init();
        }

        private void f_init()
        {

            f_initCmdLine();
            f_initFirstPath();
            f_initDebugStream();

            try
            {
                appConfigXml = new XmlDocument();
                using (StreamReader sr = new StreamReader(Path.Combine(rootDirectory, appConfigName), Encoding.UTF8, true, 1024 * 4))
                {
                    appConfigXml.Load(sr);
                }
            }
            catch (Exception)
            {
            }

            try
            {
                int sysi = IsSystemInDarkTheme();
                if (sysi == 1) lightThemeSystem = true;
                else if (sysi == 0) lightThemeSystem = false;
                else lightThemeSystem = null;
            }
            catch (Exception)
            {
                lightThemeSystem = null;
            }

            try
            {
                int ai = IsAppsInDarkTheme();
                if (ai == 1) lightThemeApp = true;
                else if (ai == 0) lightThemeApp = false;
                else lightThemeApp = null;
            }
            catch (Exception)
            {
                lightThemeApp = null;
            }

        }

        private void f_initCmdLine()
        {
            try
            {
                var re = commandLine.IndexOf(currentDomain.FriendlyName);
                int i;
                for (i = re + 1; i < commandLine.Length; i++)
                {
                    if (char.IsWhiteSpace(commandLine[i]))
                    {
                        break;
                    }
                }

                for ( ; i < commandLine.Length; i++)
                {
                    if (!char.IsWhiteSpace(commandLine[i]))
                    {
                        break;
                    }
                }
                if(i != commandLine.Length)
                {
                    //到达第二个行
                    notHeadCommandLine = commandLine.Substring(i);
                }
                else
                {
                    notHeadCommandLine = string.Empty;
                }
            }
            catch (Exception)
            {
            }
        }

        private void f_initFirstPath()
        {

            var nline = notHeadCommandLine;
            if (string.IsNullOrEmpty(nline))
            {
                firstInitOpenPath = null;
            }
            else
            {
                string re;
                var par = TrimLastStr(nline, out re);

                if (par?.ToLowerInvariant() == "-debug")
                {
                    nline = re.TrimStart(null);
                    isOpenDebug = true;
                }
                //先使用双引号获取路径
                var path = TrimLastPath(nline, out re);
                if(path is null)
                {
                    //没有双引号
                    //获取全部文本
                    path = nline.Trim(null);
                }
                
                firstInitOpenPath = path;

            }

        }

        private void f_initDebugStream()
        {
            //debugLogPrint = Console.Out;
            //return;

            if (!isOpenDebug) return;

            StreamWriter swr = null;
            FileStream file = null;
            try
            {
                //初始化日志文件路径
                var logPath = Path.Combine(rootDirectory, "debug.log");

                file = new FileStream(logPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                swr = new StreamWriter(file, Encoding.UTF8, 1024, false);
            }
            catch (Exception)
            {
                if (swr != null)
                {
                    swr.Close();
                }
                else file?.Close();
                return;
            }

            debugLogPrint = swr;

        }

        #endregion

        #region 参数

        /// <summary>
        /// 命令行参数
        /// </summary>
        public readonly string[] commandArgs;

        /// <summary>
        /// 当前应用程序域
        /// </summary>
        public AppDomain currentDomain;

        /// <summary>
        /// 程序根目录
        /// </summary>
        public readonly string rootDirectory;

        /// <summary>
        /// 该程序文件名称
        /// </summary>
        public readonly string applicationName;

        /// <summary>
        /// 该程序公共配置文件名称
        /// </summary>
        public readonly string appConfigName;

        /// <summary>
        /// 原始命令行参数
        /// </summary>
        public string commandLine;

        /// <summary>
        /// 去头命令行参数
        /// </summary>
        public string notHeadCommandLine;

        /// <summary>
        /// 该程序的配置文件xml文档
        /// </summary>
        public XmlDocument appConfigXml;

        /// <summary>
        /// DEBUG日志打印时调用对象
        /// </summary>
        public TextWriter debugLogPrint;

        /// <summary>
        /// 第一次打开时的文件路径
        /// </summary>
        public string firstInitOpenPath;

        /// <summary>
        /// 默认的首选项配置文件路径
        /// </summary>
        public readonly string defPreferencesFilePath;

        public const string defPreferencesFileName = "preferences.json";

        /// <summary>
        /// 是否为64位环境
        /// </summary>
        public readonly bool isX64;

        /// <summary>
        /// 系统亮暗色主题，true亮色，false暗色；null表示无法获取
        /// </summary>
        public bool? lightThemeSystem;

        /// <summary>
        /// 应用亮暗色主题，true亮色，false暗色；null表示无法获取
        /// </summary>
        public bool? lightThemeApp;

        /// <summary>
        /// 开启Debug模式
        /// </summary>
        public bool isOpenDebug;

        #endregion

        #region 功能

        #region 释放

        public void DisposeObject(bool disposing)
        {
            if (disposing)
            {
                debugLogPrint?.Close();
            }
            debugLogPrint = null;
        }

        #endregion

        #region sys

        /// <summary>
        /// 读取系统页面主题
        /// </summary>
        /// <returns>1表示亮色，0表示暗色</returns>
        static int IsSystemInDarkTheme()
        {
            //系统主题设置
            object systemValue = Registry.GetValue(
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                "SystemUsesLightTheme",
                1);
            // 1（亮色）
            if (systemValue is int i32)
            {
                return i32;
            }
            if (systemValue is uint ui32)
            {
                return (int)ui32;
            }
            if (systemValue is short i16)
            {
                return i16;
            }
            if (systemValue is ushort ui16)
            {
                return (int)ui16;
            }
            if (systemValue is short i8)
            {
                return i8;
            }
            if (systemValue is ushort ui8)
            {
                return (int)ui8;
            }
            throw new ArgumentException();
        }

        /// <summary>
        /// 读取应用页面主题
        /// </summary>
        /// <returns>1表示亮色，0表示暗色</returns>
        static int IsAppsInDarkTheme()
        {
            // 读取应用主题设置
            object appsValue = Registry.GetValue(
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                "AppsUseLightTheme",
                1); // 默认值为1（亮色）
            if (appsValue is int i32)
            {
                return i32;
            }
            if (appsValue is uint ui32)
            {
                return (int)ui32;
            }
            if (appsValue is short i16)
            {
                return i16;
            }
            if (appsValue is ushort ui16)
            {
                return (int)ui16;
            }
            if (appsValue is short i8)
            {
                return i8;
            }
            if (appsValue is ushort ui8)
            {
                return (int)ui8;
            }
            throw new ArgumentException();
        }

        #endregion

        #region DEBUG

        /// <summary>
        /// 清理debug流缓冲区
        /// </summary>
        public void DebugFlush()
        {
            debugLogPrint?.Flush();
        }

        /// <summary>
        /// 打印一行日志
        /// </summary>
        /// <param name="message"></param>
        public void DebugPrintLine(string message)
        {
            debugLogPrint?.WriteLine(message);
        }

        /// <summary>
        /// 打印一个换行符
        /// </summary>
        public void DebugPrintLine()
        {
            debugLogPrint?.WriteLine();
        }

        /// <summary>
        /// 打印一段日志信息
        /// </summary>
        /// <param name="message"></param>
        public void DebugPrint(string message)
        {
            debugLogPrint?.Write(message);
        }

        #endregion

        #region

        /// <summary>
        /// 切割字符串，按空白字符切割
        /// </summary>
        /// <remarks>
        /// 切割从起始位开始的第一个非空白字符，到第一个空白字符前，作为返回值；再向后第一个非空白字符，到<paramref name="value"/>的结尾，作为<paramref name="reValue"/>的返回值
        /// </remarks>
        /// <param name="value">要切割的字符串</param>
        /// <param name="reValue">切割后第一个非空白字符到结尾的字符串；如果整个字符串没有中间空白字符，则<paramref name="value"/>会被直接切割到返回值，此值为空</param>
        /// <returns>返回从起始位开始向后的第一个非空白字符，到第一个空白字符前的字符串</returns>
        public static string TrimLastStr(string value, out string reValue)
        {
            //空白字符使用 char.IsWhiteSpace 函数做判断
            if (value is null) throw new ArgumentNullException();

            if (value.Length == 0)
            {
                //空字符
                reValue = string.Empty;
                return value;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                //全是空白
                reValue = string.Empty;
                return value;
            }

            int i;

            //移除前导空白
            var str = value.TrimStart((char[])null);

            //char c;
            //搜索str向后第一个空白
            for (i = 0; i < str.Length; i++)
            {

                if (char.IsWhiteSpace(str[i]))
                {
                    //是空白
                    break;
                }
            }

            if (i == str.Length)
            {
                //没有空白
                reValue = string.Empty;
                return str.TrimStart();
                //reValue = str.TrimStart((char[])null);
                //return null;
            }

            // i表示后第一个空白
            //获取返回值
            string res = str.Substring(0, i);

            reValue = str.Substring(i).TrimStart((char[])null);

            return res;

        }

        /// <summary>
        /// 切割字符串，按引号切割
        /// </summary>
        /// <remarks>
        /// <para>按引号切割字符串，返回从第一个 '"'到第二个'"'间的值，<paramref name="reValue"/>表示引号后再空白后的字符串</para>
        /// </remarks>
        /// <param name="value"></param>
        /// <param name="reValue">引号后的空白后字符串</param>
        /// <returns>从第一个 '"'到第二个'"'间的值，若没找到双引号则为null</returns>
        public static string TrimLastPath(string value, out string reValue)
        {
            //空白字符使用 char.IsWhiteSpace 函数做判断
            if (value is null) throw new ArgumentNullException();

            int length = value.Length;

            if (length == 0)
            {
                //空字符
                reValue = value;
                return null;
            }

            int left;

            //检索第一个双引号
            left = value.IndexOf('"');
            if (left == -1)
            {
                //没有双引号
                reValue = value;
                return null;
            }


            int right = left + 1;

            if (right == length)
            {
                //没有完整双引号
                reValue = value;
                return null;
            }

            right = value.IndexOf('"', right);

            if (right == -1)
            {
                //没有完整双引号
                reValue = value;
                return null;
            }

            //返回引号内值
            string re;
            if (right - left <= 1)
            {
                re = string.Empty;
            }
            else
            {
                re = value.Substring(left + 1, (right - left) - 1);
            }


            //赋值后继字符
            if (right + 1 == length)
            {
                reValue = string.Empty;
            }
            else
            {
                reValue = value.Substring(right + 1).TrimStart(null);
            }

            return re;
        }


        #endregion

        #endregion

    }

}
#if DEBUG

#endif