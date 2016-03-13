using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using System.Runtime.InteropServices;

namespace MaterialSkinExample
{
    static class Program
    {
        [DllImport("Shcore.dll", SetLastError = true)]
        static extern int SetProcessDpiAwareness(int i);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SetProcessDpiAwareness(2);

            Application.EnableVisualStyles();
            
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
