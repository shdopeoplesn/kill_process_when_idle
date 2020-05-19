using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace kill_process_when_idle
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new kill_process_when_idle());
        }
    }
}
