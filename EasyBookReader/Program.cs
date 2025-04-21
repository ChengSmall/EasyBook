using Cheng.EasyBooks.WinForms;
using Cheng.EasyBooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Win32;

namespace Cheng
{

    public static class Program
    {


        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Control.CheckForIllegalCrossThreadCalls = false;
            InitArgs.Init(args);
           
            Application.Run(new MainForm());
            //InitArgs.Args.DisposeObject(true);

            
        }

    }
}
