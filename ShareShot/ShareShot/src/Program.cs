using System;
using System.Windows.Forms;

namespace ShareShot.src {
    class Program {
        [STAThread]
        static public void Main(String[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TrayWindow());
        }
    }
}