using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExtractor;

namespace YouTubeDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            cboResolution.SelectedIndex = 0;
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            IEnumerable<VideoInfo> videos = DownloadUrlResolver.GetDownloadUrls(txtUrl.Text);
            VideoInfo video = videos.First(x => x.VideoType == VideoType.Mp4 && x.Resolution == Convert.ToInt32(cboResolution.Text));
            if (video == null)
            {
                MessageBox.Show("Video Not found !!!");
            }
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }
            string saveLocation = ConfigurationManager.AppSettings["SaveLocation"];
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            string name = r.Replace(video.Title + video.VideoExtension, "");
            string path = Path.Combine(saveLocation, name);
            VideoDownloader downloader = new VideoDownloader(video, path);
            downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
            Thread thread = new Thread(() => { downloader.Execute(); }) { IsBackground = true };
            thread.Start();
        }

        private void Downloader_DownloadProgressChanged(object sender, ProgressEventArgs e)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                progressBar.Value = (int)e.ProgressPercentage;
                lblPercentage.Text = $"{string.Format("{0:0.##}", e.ProgressPercentage)}%";
                progressBar.Update();
            }));
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
