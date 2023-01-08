using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Windows.Forms;

namespace ShareShot.src {
    class ImgurUpload {
        private Imgur_Login login = new Imgur_Login();

        private Point point1;
        private Point point2;
        private String path;
        TrayWindow trayWin;

        public ImgurUpload(Point p1, Point p2, TrayWindow tw) {
            point1 = p1;
            point2 = p2;
            trayWin = tw;
            Take_Screenshot();
        }
        
        private void Take_Screenshot() {
            var rect = new Rectangle(Math.Min(point1.X, point2.X),
                                     Math.Min(point1.Y, point2.Y),
                                     Math.Abs(point2.X - point1.X),
                                     Math.Abs(point2.Y - point1.Y));
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
            path = Path.Combine(Environment.ExpandEnvironmentVariables(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)), DateTime.Now.ToString("MM-dd-yyyy h-mm-ss-tt") + ".png");
            bmp.Save(path, ImageFormat.Png);
            Upload_ScreenshotAsync();
        }

        private async void Upload_ScreenshotAsync() {
            string access_token = login.GetToken();

            string image = Convert.ToBase64String(File.ReadAllBytes(path));
            //using (HttpClient client = new HttpClient()) {
            //    Dictionary<string, string> data = new Dictionary<string, string>() {
            //        {"image", image }
            //    };
            //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            //    var content = new FormUrlEncodedContent(data);
            //    var response = await client.PostAsync("https://api.imgur.com/3/upload", content);
            //    var responseString = await response.Content.ReadAsStringAsync();

            //    dynamic responseObjet = JsonConvert.DeserializeObject(responseString);
            //    string url = responseObjet.data.link;
            //   trayWin.ShowBalloon(30000, "Screenshot Successfully Uploaded:\n", url, ToolTipIcon.Info);
            //}
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            var response = await client.PostAsync("https://api.imgur.com/3/image", new StringContent(image));
            var responseString = await response.Content.ReadAsStringAsync();
            dynamic responseObjet = JsonConvert.DeserializeObject(responseString);
            string url = responseObjet.data.link;
            trayWin.ShowBalloon(30000, "Screenshot Successfully Uploaded:\n", url, ToolTipIcon.Info);
        }
    }
}
