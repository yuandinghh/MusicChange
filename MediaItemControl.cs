using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        } // 枚举：Video、Audio、Image

        public MediaItemControl(string filePath, MediaType mediaType)
        {
            InitializeComponent();
            FilePath = filePath;
            MediaType = mediaType;

            // 显示文件名
            lblFileName.Text = Path.GetFileName(filePath);

            // 根据媒体类型设置缩略图或默认图标
            if(mediaType == MediaType.Image)
            {
                pictureBoxThumbnail.Image = Image.FromFile(filePath);
            }
            else if(mediaType == MediaType.Video)
            {
                // 这里可通过 LibVLC 生成视频缩略图（简化示例，实际需调用 LibVLC API）
                pictureBoxThumbnail.Image = Properties.Resources.DefaultVideoThumbnail;
            }
            else // Audio
            {
                //pictureBoxThumbnail.Image = Properties.Resources.DefaultAudioThumbnail;
                pictureBoxThumbnail.Image = Properties.Resources.DefaultVideoThumbnail;
            }

            // 播放按钮点击事件（示例：触发外部播放逻辑）
            btnPlay.Click += (s, e) => PlayMedia();
        }

        private void PlayMedia()
        {
            // 触发播放事件，由父容器（Panel/FlowLayoutPanel）处理播放逻辑
            MediaPlayRequested?.Invoke(this, new MediaPlayEventArgs(FilePath, MediaType));
        }

        // 自定义事件：请求播放媒体
        public event EventHandler<MediaPlayEventArgs> MediaPlayRequested;
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
