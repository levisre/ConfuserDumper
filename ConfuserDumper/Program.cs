using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ConfuserDumper
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                MethodsDumper MethodsDumper = new MethodsDumper();
                if (MethodsDumper.DumpMethodsOf(args[0]))
                    MessageBox.Show("Success");
                else
                    MessageBox.Show(MethodsDumper.excpetion.Message);
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
