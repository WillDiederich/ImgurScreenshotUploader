using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ShareShot {
    public partial class TrayWindow : Form {
        private NotifyIcon notifyIcon;
        private ContextMenu contextMenu;
        private MenuItem exit, captureArea;
        private IContainer componentsContainer;
        private Capture capture;

        // Used in the creation of hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public TrayWindow() {
            InitializeComponent();

            // Set the window state to minimized with no border style
            this.WindowState = FormWindowState.Minimized;
            this.FormBorderStyle = FormBorderStyle.None;

            // Setup a components container and context menu that will hold the buttons we are about to create below
            this.componentsContainer = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenu();

            // Setup a button that initiates screenshot capture
            this.captureArea = new System.Windows.Forms.MenuItem("Capture Area");
            this.captureArea.Click += new System.EventHandler(this.Capture_Screenshot_Click);

            // Setup a button that will capture the entire desktop

            // We will add a view last uploaded image button here

            // Setup an exit button
            this.exit = new System.Windows.Forms.MenuItem("Exit");
            this.exit.Click += new System.EventHandler(this.Exit_Click);

            // Add the buttons to the context menu
            this.contextMenu.MenuItems.AddRange(
                        new System.Windows.Forms.MenuItem[] {
                            this.captureArea,
                            this.exit
                        }
                );

            // Setup the notify icon, this will sit in the system tray
            notifyIcon = new System.Windows.Forms.NotifyIcon(this.componentsContainer);
            notifyIcon.Icon = new Icon("C:\\Users\\Desktop-PC\\Desktop\\test.ico");
            notifyIcon.ContextMenu = this.contextMenu;
            notifyIcon.Text = "ShareShot";
            notifyIcon.Visible = true;

            // Disable the taskbar icon
            this.ShowInTaskbar = false;

            // Setup a hotkey that will initiate screenshot capture when pressed
            // Currently bound to (Ctrl + F10)
            // Will add feature in the future to allow the user to select their own hotkeys
            RegisterHotKey(this.Handle, 1, 2, (int)Keys.F10);

            capture = new Capture(this);
        }

        private void TrayWindow_Load(object sender, EventArgs e) {

        }

        // When the program shuts down, remove the hotkey
        private void TrayWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            UnregisterHotKey(this.Handle, 1);
        }

        // Shut down the program when called
        private void Exit_Click(object Sender, EventArgs e) {
            this.Close();
        }

        // Create an instance of CaptureScreenshot and show the window
        private void Capture_Screenshot_Click(object Sender, EventArgs e) {
            capture.Show();
        }

        // Detects when the hotkey is pressed, and creates a new instance of CaptureScreenshot if one does not currently exist
        protected override void WndProc(ref Message m) {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == 1) {
                if ((Application.OpenForms["CaptureScreenshot"] as Capture) == null) {
                    capture.Show();
                }
            }
            base.WndProc(ref m);
        }

        // Creates a toast notification showing the user that the screenshot was successfully uploaded
        public void ShowBalloon(int time, string text, string url, ToolTipIcon icon) {
            notifyIcon.BalloonTipClicked += delegate (object sender, EventArgs e) {
                notify_icon_BalloonTipClicked(sender, e, url);
            };
            notifyIcon.ShowBalloonTip(time, text, url, icon);
        }

        // If the toast notification is clicked, we open the image in the users default browser
        private void notify_icon_BalloonTipClicked(object sender, EventArgs e, string url) {
            System.Diagnostics.Process.Start(url);
        }
    }
}
