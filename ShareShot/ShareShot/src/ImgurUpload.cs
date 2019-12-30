using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Windows.Forms;


namespace ShareShot {
    class ImgurUpload {
        private ImgurLogin login = new ImgurLogin();
        // private Point point1;
        // private Point point2;
        private String path;
        private TrayWindow trayWindow;

        public ImgurUpload(TrayWindow tw) {
            // point1 = p1;
            // point2 = p2;
            trayWindow = tw;
            // Take_Screenshot();
        }

        public void Take_Screenshot(Point point1, Point point2) {
            // Create a rectangle based on the two points
            var rect = new Rectangle(Math.Min(point1.X, point2.X),
                                     Math.Min(point1.Y, point2.Y),
                                     Math.Abs(point2.X - point1.X),
                                     Math.Abs(point2.Y - point1.Y));
            // Create a bitmap using the rectangles width and height
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);

            // Copy the screen selection and save it to the appdata folder
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
            path = Path.Combine(Environment.ExpandEnvironmentVariables("%appdata%\\ShareShot\\"), DateTime.Now.ToString("MM-dd-yyyy h-mm-ss-tt") + ".png");
            bmp.Save(path, ImageFormat.Png);

            // Upload the screenshot to Imgur
            Upload_ScreenshotAsync();
        }

        private async void Upload_ScreenshotAsync() {
            // Get the access token
            string accessToken = login.GetToken();

            // Convert the screenshot to a string for upload
            string image = Convert.ToBase64String(File.ReadAllBytes(path));

            // Upload the screenshot using the imgur api upload url, await the response and use it to return the url to the Notify Icon in TrayWindow.cs
            using (HttpClient client = new HttpClient()) {
                Dictionary<string, string> data = new Dictionary<string, string>() {
                    {"image", image }
                };
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var content = new FormUrlEncodedContent(data);
                var response = await client.PostAsync("https://api.imgur.com/3/upload", content);
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic responseObjet = JsonConvert.DeserializeObject(responseString);
                string url = responseObjet.data.link;
                trayWindow.ShowBalloon(30000, "Screenshot Successfully Uploaded:\n", url, ToolTipIcon.Info);
            }
        }
    }
}
