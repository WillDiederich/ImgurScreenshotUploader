using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareShot.src {
    public partial class CaptureScreenshot : Form {
        private SolidBrush myBrush = new SolidBrush(Color.FromArgb(255, 211, 211, 211));
        private Point point1 = new Point(-1, -1);
        private Point point2 = new Point(-1, -1);
        private bool clicked;
        TrayWindow trayWin;

        public CaptureScreenshot(TrayWindow tw) {
            trayWin = tw;
            InitializeComponent();
            this.SuspendLayout();

            // Set the WindowState to normal and remove all decoration from the form.
            // Set the size of the form to the resolution of each monitor combined.
            // Ex: Two 1080p monitors will create a form of size 3840 x 1080.
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ClientSize = new System.Drawing.Size(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);

            // Set both the Transparency
            // and the BackColor to the same color, making the form fully transparent but still clickable.
            // Set the opacity to allow a semi-transparent rectangle to be drawn.
            this.TransparencyKey = Color.LavenderBlush;
            this.BackColor = Color.LavenderBlush;
            this.Opacity = 0.20f;

            // Double buffering helps prevent flickering when the rectangle is redrawn.
            this.DoubleBuffered = true;

            // Disable the taskbar icon when the form is open.
            this.ShowInTaskbar = false;

            // Create form events.
            this.Load += new System.EventHandler(this.CaptureScreenshot_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CaptureScreenshot_Paint);
            this.Deactivate += new System.EventHandler(this.CaptureScreenshot_Deactivate);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CaptureScreenshot_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CaptureScreenshot_MouseUp);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CaptureScreenshot_Moved);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CaptureScreenshot_KeyPressed);

            this.ResumeLayout(false);
            this.clicked = false;
        }

        // Required
        private void CaptureScreenshot_Load(object sender, EventArgs e) {

        }

        // Draw the rectangle on the form when conditions are met.
        private void CaptureScreenshot_Paint(object sender, PaintEventArgs e) {
            if (clicked == true) {
                var rectangle = new Rectangle(Math.Min(point1.X, point2.X),
                                              Math.Min(point1.Y, point2.Y),
                                              Math.Abs(point2.X - point1.X),
                                              Math.Abs(point2.Y - point1.Y));
                e.Graphics.FillRectangle(myBrush, rectangle);
            }
        }

        // Force the form to regain focus if focus is lost.
        private void CaptureScreenshot_Deactivate(Object sender, EventArgs e) {
            this.Focus();
        }

        // When the mouse has been pressed, set clicked to true and set the first point.
        private void CaptureScreenshot_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
            clicked = true;
            point1 = new Point(e.X, e.Y);
        }

        // When the mouse has been released, set clicked to false and set the second point.
        // If the first and second points have different X and Y values, close the form and call TakeScreenshot.
        // If the first and second points have the same X or Y values, call Invalidate and reset both points.
        private void CaptureScreenshot_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
            clicked = false;
            point2 = new Point(e.X, e.Y);
            
            if (point1.X != point2.X && point1.Y != point2.Y) {
                this.Close();
                new ImgurUpload(point1, point2, trayWin);
            }
            else {
                Invalidate();
                point1 = new Point(-1, -1);
                point2 = new Point(-1, -1);
            }
        }

        // When the mouse has been moved, if clicked is set to true set the second point and call Invalidate to redraw the rectangle.
        private void CaptureScreenshot_Moved(object sender, System.Windows.Forms.MouseEventArgs e) {
            if (clicked == true) {
                point2 = new Point(e.X, e.Y);
                Invalidate();
            }
        }

        // If the escape key is pressed, exit the program.
        private void CaptureScreenshot_KeyPressed(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                this.Close();
            }
        }
    }
}