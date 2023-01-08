using System;
using System.Windows.Forms;

namespace ShareShot.src {
    class NotificationSender {
        string url;
        public NotificationSender(string u, TrayWindow tw) {
            url = u;
            tw.ShowBalloon(30000, "Screenshot Successfully Uploaded:\n", u, ToolTipIcon.Info);
            //this.notify_icon.BalloonTipClicked += new EventHandler(notify_icon_BalloonTipClicked);
        }

        void notify_icon_BalloonTipClicked(object sender, EventArgs e) {
            System.Diagnostics.Process.Start(url);
        }
    }
}
