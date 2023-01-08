using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ComponentModel;

namespace ShareShot.src {
    public partial class TrayWindow : Form {
        private NotifyIcon notify_icon;
        private ContextMenu context_menu;
        private MenuItem exit, capture_area;
        private IContainer components_container;

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public TrayWindow() {
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;
            this.FormBorderStyle = FormBorderStyle.None;

            this.components_container = new System.ComponentModel.Container();
            this.context_menu = new System.Windows.Forms.ContextMenu();

            this.capture_area = new System.Windows.Forms.MenuItem("Capture Area");
            this.capture_area.Click += new System.EventHandler(this.Capture_Screenshot_Click);

            this.exit = new System.Windows.Forms.MenuItem("Exit");
            this.exit.Click += new System.EventHandler(this.Exit_Click);

            this.context_menu.MenuItems.AddRange(
                        new System.Windows.Forms.MenuItem[] { this.capture_area, this.exit });

            notify_icon = new System.Windows.Forms.NotifyIcon(this.components_container);
            notify_icon.Icon = new Icon("test.ico");
            notify_icon.ContextMenu = this.context_menu;
            notify_icon.Text = "ShareShot";
            notify_icon.Visible = true;

            this.ShowInTaskbar = false;
            RegisterHotKey(this.Handle, 1, 2, (int)Keys.F10);
        }

        private void TrayWindow_Load(object sender, EventArgs e) {

        }

        private void TrayWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            UnregisterHotKey(this.Handle, 1);
        }

        private void Exit_Click(object Sender, EventArgs e) {
            this.Close();
        }
        
        private void Capture_Screenshot_Click(object Sender, EventArgs e) {
            CaptureScreenshot capture = new CaptureScreenshot(this);
            capture.Show();
        }

        private void Balloon_Clicked(string url)
        {
            Clipboard.SetText(url);
            System.Diagnostics.Process.Start(url);
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == 1) {
                if ((Application.OpenForms["CaptureScreenshot"] as CaptureScreenshot) == null) {
                    CaptureScreenshot capture = new CaptureScreenshot(this);
                    capture.Show();
                }
            }
            base.WndProc(ref m);
        }

        public void ShowBalloon(int time, string text, string url, ToolTipIcon icon) {
            notify_icon.ShowBalloonTip(time, text, url, icon);
            notify_icon.BalloonTipClicked += new EventHandler((sender, e) => Balloon_Clicked(url));
        }
    }
}