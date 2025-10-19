// MediaItemControl.cs
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
namespace MusicChange
{
    public partial class MediaItemControl:UserControl
    {
        public string FilePath
        {
            get; private set;
        }
        public MediaType MediaType
        {
            get; private set;
        }

        public MediaItemControl(string filePath, MediaType mediaType)
        {
            InitializeComponent();
            FilePath = filePath;
            MediaType = mediaType;

            // 设置控件尺寸
            // 设置统一的控件尺寸
            //this.Size = new Size(180, 220); // 增加高度以容纳更多信息
            this.Size = new Size(120, 180); // 增加高度以容纳更多信息
            //this.Margin = new Padding(10, 10, 10, 10);
            //this.Margin = new Padding(5);
            //this.Padding = new Padding(5);

            // 显示文件名
            lblFileName.Height = 25; //40 增加标签高度
            lblFileName.Text = Path.GetFileName(filePath);
            lblFileName.Dock = DockStyle.Bottom;
            lblFileName.TextAlign = ContentAlignment.MiddleCenter;
            lblFileName.AutoEllipsis = false;
            lblFileName.Visible = true;
            lblFileName.BringToFront();
            btnPlay.Visible = false;
            btnPlay.BringToFront();

            // 根据媒体类型设置缩略图或默认图标
            SetThumbnail(filePath, mediaType);

            // 播放按钮点击事件
            btnPlay.Click += (s, e) => PlayMedia();
        }
        private void SetThumbnail(string filePath, MediaType mediaType)
        {
            try
            {
                if(mediaType == MediaType.Image && File.Exists(filePath))
                {
                    // 对于图片文件，尝试加载缩略图
                    Image originalImage = Image.FromFile(filePath);
                    pictureBoxThumbnail.Image = ResizeImage(originalImage, 120, 90);
                    originalImage.Dispose();
                }
                else if(mediaType == MediaType.Video)
                {
                    // 视频文件使用默认图标
                    //视频文件使用视频封面
                    //using(var video = new VideoPlayer(filePath))
                    //{
                    //    pictureBoxThumbnail.Image = video.GetThumbnail();
                    //}
                    //pictureBoxThumbnail.SizeMode = PictureBoxSizeMode.Zoom;

                    pictureBoxThumbnail.Image = Properties.Resources.DefaultVideoThumbnail;
                }
                else if(mediaType == MediaType.Audio)
                {
                    // 音频文件使用默认图标
                    pictureBoxThumbnail.Image = Properties.Resources.music43;  //DefaultAudioThumbnail
                }
                else
                {
                    // 其他类型使用默认图标
                    pictureBoxThumbnail.Image = Properties.Resources.music43;  //DefaultFileIcon;
                }
            }
            catch(Exception ex)
            {
                // 出错时使用默认图标
                pictureBoxThumbnail.Image = Properties.Resources.DefaultVideoThumbnail;  //DefaultFileIcon;
                System.Diagnostics.Debug.WriteLine($"加载缩略图失败: {ex.Message}");
            }
        }

        private Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            // 计算缩放比例
            double ratioX = (double)maxWidth / image.Width;
            double ratioY = (double)maxHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);

            // 创建缩略图
            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using(Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        private void PlayMedia()
        {
            // 触发播放事件
            MediaPlayRequested?.Invoke(this, new MediaPlayEventArgs(FilePath, MediaType));
        }

        // 自定义事件：请求播放媒体
        public event EventHandler<MediaPlayEventArgs> MediaPlayRequested;

		private void pictureBoxThumbnail_Click(object sender, EventArgs e)
		{
            // 触发播放事件

            MediaPlayRequested?.Invoke(this, new MediaPlayEventArgs(FilePath, MediaType));
        }

	}

	// 媒体类型枚举
	public enum MediaType
    {
        Video, Audio, Image
    }

    // 播放事件参数
    public class MediaPlayEventArgs:EventArgs
    {
        public string FilePath
        {
            get;
        }
        public MediaType MediaType
        {
            get;
        }

        public MediaPlayEventArgs(string filePath, MediaType mediaType)
        {
            FilePath = filePath;
            MediaType = mediaType;
        }
    }
}