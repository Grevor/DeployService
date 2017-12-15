using System;
using System.Windows.Forms;
using Viking.Deployment;

namespace UserPrompt
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
            Application.Run();
            var pipe = new IPCPipe(false);
            while (true)
            {
                string msg = pipe.ReadString();
                var result = MessageBox.Show(msg, "Deployment Prompt", MessageBoxButtons.YesNo);
                pipe.Send(result == DialogResult.Yes);
            }
        }
    }
}
