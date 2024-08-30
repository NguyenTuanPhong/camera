using System;
using System.Windows.Forms;
using Vlc.DotNet.Forms;
using System.IO;
using System.Drawing;

namespace viewCamera_IP
{
    public partial class Form1 : Form
    {
        private VlcControl vlcControl;
        private Button btnTakePicture;
        private Button btnStopStream;

        public Form1()
        {
            InitializeComponent();
            InitializeVlcControl();
            InitializeControls();

        }
        private void BtnTakePicture_Click(object sender, EventArgs e)
        {
            TakePicture();
        }

        private void BtnStopStream_Click(object sender, EventArgs e)
        {
            vlcControl.Stop();
        }

        private void InitializeControls()
        {
            btnTakePicture = new Button();
            btnTakePicture.Text = "Take Picture";
            btnTakePicture.Click += BtnTakePicture_Click;
            btnTakePicture.Dock = DockStyle.Top;
            this.Controls.Add(btnTakePicture);

            btnStopStream = new Button();
            btnStopStream.Text = "Stop Stream";
            btnStopStream.Click += BtnStopStream_Click;
            btnStopStream.Dock = DockStyle.Top;
            this.Controls.Add(btnStopStream);
        }
        private void TakePicture()
        {
            try
            {
                // Stop the stream temporarily if needed
                vlcControl.Pause();

                // Get the video frame as a Bitmap
                var bitmap = GetVideoFrameAsBitmap();

                // Save the Bitmap to a file
                var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), $"Snapshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

                MessageBox.Show($"Picture saved to {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while taking the picture: {ex.Message}");
            }
            finally
            {
                // Resume the stream if needed
                vlcControl.Play();
            }
        }

        private Bitmap GetVideoFrameAsBitmap()
        {
            // This is a placeholder implementation.
            // Actual implementation might require accessing frame data directly
            // or using an additional library or API.
            // Here we create a dummy bitmap for illustration.
            int width = vlcControl.Width;
            int height = vlcControl.Height;
            var bitmap = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Black); // Just for illustration
            }
            return bitmap;
        }

        private void InitializeVlcControl()
        {
            try
            {
                // Initialize VLC control
                vlcControl = new VlcControl();
                vlcControl.Dock = DockStyle.Fill;
                this.Controls.Add(vlcControl);

                // Set VLC libraries path
                var libDirectory = @"C:\Program Files (x86)\VideoLAN\VLC"; // Adjust the path based on your VLC installation
                if (Directory.Exists(libDirectory))
                {
                    vlcControl.BeginInit();
                    vlcControl.VlcLibDirectory = new DirectoryInfo(libDirectory);
                    vlcControl.EndInit();
                }
                else
                {
                    MessageBox.Show("VLC library directory not found. Please check the VLC installation path.");
                    return;
                }

                // Construct the RTSP URL
                var username = Uri.EscapeDataString("long");
                var password = Uri.EscapeDataString("Zaq@12345");
                var hostname = "nongdanonlnine.ddns.net";
                var port = "554";
                var path = "/cam/realmonitor?channel=7&subtype=0";
                var rtspUrl = $"rtsp://{username}:{password}@{hostname}:{port}{path}";

                // Validate the URL
                try
                {
                    var uri = new Uri(rtspUrl);
                    vlcControl.Play(uri);
                }
                catch (UriFormatException ex)
                {
                    MessageBox.Show($"Invalid URI: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}\n{ex.StackTrace}");
            }
        }


        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            vlcControl.Stop();
            vlcControl.Dispose();
            base.OnFormClosed(e);
        }
    }
}
