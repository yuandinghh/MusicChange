using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace MusicChange
{
    public class TimelineTrackC:Panel
    {
        #region 属性
        /// <summary>
        /// 缩放比例：1秒对应的像素数（如100 → 1秒=100像素）
        /// </summary>
        public float PixelsPerSecond { get; set; } = 100f;

        /// <summary>
        /// 时间轴中的媒体项列表
        /// </summary>
        public BindingList<MediaItem> MediaItems { get; } = new BindingList<MediaItem>();

        /// <summary>
        /// 当前时间轴的播放/浏览时间
        /// </summary>
        public TimeSpan CurrentTime
        {
            get => _currentTime;
            set
            {
                if(_currentTime != value)
                {
                    _currentTime = value;
                    UpdateCursorPosition(); // 更新游标位置
                    TimeChanged?.Invoke(this, value);
                }
            }
        }
        private TimeSpan _currentTime = TimeSpan.Zero;

        /// <summary>
        /// 时间变化事件（如播放时更新）
        /// </summary>
        public event EventHandler<TimeSpan> TimeChanged;

        /// <summary>
        /// 媒体项被拖入事件
        /// </summary>
        public event EventHandler<MediaItemControl> MediaItemDropped;
        #endregion

        #region 控件初始化
        private PictureBox _cursor; // 游标（红色竖线）
        private bool _isDraggingCursor = false; // 是否正在拖动游标

        public TimelineTrackC()
        {
            // 初始化游标
            _cursor = new PictureBox
            {
                BackColor = Color.Red,
                Width = 1,
                Height = 100,
                Location = new Point(0, 0),
                //BringToFront()
            };
            this.Controls.Add(_cursor);

            // 绑定事件
            this.DragEnter += TimelineTrack_DragEnter;
            this.DragOver += TimelineTrack_DragOver;
            this.Drop += TimelineTrack_Drop;
            this.Paint += TimelineTrack_Paint;
            this.Resize += TimelineTrack_Resize;
            this.MouseMove += TimelineTrack_MouseMove;
            this.MouseDown += TimelineTrack_MouseDown;
            this.MouseUp += TimelineTrack_MouseUp;

            // 监听媒体项列表变化（重绘）
            MediaItems.ListChanged += (s, e) => Invalidate();
        }
        #endregion

        #region 拖放逻辑
        private void TimelineTrack_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void TimelineTrack_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
            e.Handled = true;
        }

        private void TimelineTrack_Drop(object sender, DragEventArgs e)
        {
            if(!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            // 计算鼠标在时间轴中的位置（对应开始时间）
            Point mousePos = this.PointToClient(new Point(e.X, e.Y));
            TimeSpan startTime = TimeSpan.FromSeconds(mousePos.X / PixelsPerSecond);

            // 处理每个拖入的文件
            foreach(string file in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                MediaType type = GetMediaType(file);
                TimeSpan duration = GetMediaDuration(file, type);
                var item = new MediaItem(type, file, duration)
                {
                    StartTime = startTime // 设置开始时间为鼠标位置
                };
                MediaItems.Add(item);
                MediaItemDropped?.Invoke(this, item);
            }
        }

        /// <summary>
        /// 根据文件扩展名获取媒体类型
        /// </summary>
        private MediaType GetMediaType(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            return ext switch
            {
                ".mp4" or ".avi" or ".mov" or ".wmv" => MediaType.Video,
                ".mp3" or ".wav" or ".flac" or ".aac" => MediaType.Audio,
                ".jpg" or ".jpeg" or ".png" or ".bmp" => MediaType.Image,
                _ => MediaType.Image
            };
        }

        /// <summary>
        /// 获取媒体时长（简化版，实际需更准确）
        /// </summary>
        private TimeSpan GetMediaDuration(string filePath, MediaType type)
        {
            return type switch
            {
                MediaType.Audio => TagLib.File.Create(filePath).Properties.Duration,
                MediaType.Video => TimeSpan.FromSeconds(10), // 示例：默认10秒
                _ => TimeSpan.FromSeconds(5) // 图片默认5秒
            };
        }
        #endregion

        #region 绘制逻辑
        private void TimelineTrack_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. 绘制背景
            g.FillRectangle(Brushes.White, this.ClientRectangle);

            // 2. 绘制时间刻度
            DrawTimeRuler(g);

            // 3. 绘制媒体项
            DrawMediaItems(g);
        }

        /// <summary>
        /// 绘制时间刻度（顶部 ruler）
        /// </summary>
        private void DrawTimeRuler(Graphics g)
        {
            int width = this.Width;
            int totalSeconds = (int)(width / PixelsPerSecond); // 显示的总时长（秒）

            using(var lightPen = new Pen(Color.LightGray, 1))
            using(var boldPen = new Pen(Color.Black, 2))
            using(var font = new Font("Arial", 8))
            using(var brush = new SolidBrush(Color.Black))
            {
                for(int sec = 0 ;sec <= totalSeconds ;sec++)
                {
                    float x = sec * PixelsPerSecond;
                    if(x > width)
                        break;

                    // 大刻度（每5秒）：带时间文本
                    if(sec % 5 == 0)
                    {
                        g.DrawLine(boldPen, x, 0, x, 20);
                        string timeText = TimeSpan.FromSeconds(sec).ToString(@"mm\:ss");
                        var size = g.MeasureString(timeText, font);
                        g.DrawString(timeText, font, brush, x - size.Width / 2, 25);
                    }
                    // 小刻度（每1秒）
                    else
                    {
                        g.DrawLine(lightPen, x, 0, x, 10);
                    }
                }
            }
        }

        /// <summary>
        /// 绘制所有媒体项
        /// </summary>
        private void DrawMediaItems(Graphics g)
        {
            foreach(var item in MediaItems)
            {
                float left = (float)item.StartTime.TotalSeconds * PixelsPerSecond;
                float width = (float)item.Duration.TotalSeconds * PixelsPerSecond;

                // 跳过超出容器范围的媒体项
                if(left + width < 0 || left > this.Width)
                    continue;

                // 1. 绘制媒体项背景
                g.FillRectangle(Brushes.LightBlue, left, 30, width, this.Height - 30);
                g.DrawRectangle(Pens.Blue, left, 30, width, this.Height - 30);

                // 2. 绘制缩略图（左侧）
                if(item.Thumbnail != null)
                {
                    float thumbX = left + 5;
                    float thumbY = (this.Height - 30 - item.Thumbnail.Height) / 2 + 30;
                    g.DrawImage(item.Thumbnail, thumbX, thumbY, item.Thumbnail.Width, item.Thumbnail.Height);
                }

                // 3. 绘制媒体类型图标（顶部）
                Image icon = item.Type switch
                {
                    MediaType.Video => Properties.Resources.VideoIcon,
                    MediaType.Audio => Properties.Resources.AudioIcon,
                    _ => Properties.Resources.ImageIcon
                };
                g.DrawImage(icon, left + 5, 5);
            }
        }
        #endregion

        #region 游标逻辑
        /// <summary>
        /// 更新游标位置（根据CurrentTime）
        /// </summary>
        private void UpdateCursorPosition()
        {
            float x = (float)CurrentTime.TotalSeconds * PixelsPerSecond;
            _cursor.Left = (int)Math.Clamp(x, 0, this.Width);
        }

        /// <summary>
        /// 鼠标按下：开始拖动游标
        /// </summary>
        private void TimelineTrack_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left && e.Y < 30) // 仅在刻度区域拖动
                _isDraggingCursor = true;
        }

        /// <summary>
        /// 鼠标移动：拖动游标
        /// </summary>
        private void TimelineTrack_MouseMove(object sender, MouseEventArgs e)
        {
            if(_isDraggingCursor && e.Button == MouseButtons.Left)
            {
                float seconds = e.X / PixelsPerSecond;
                CurrentTime = TimeSpan.FromSeconds(seconds);
            }
        }

        /// <summary>
        /// 鼠标释放：停止拖动游标
        /// </summary>
        private void TimelineTrack_MouseUp(object sender, MouseEventArgs e)
        {
            _isDraggingCursor = false;
        }

        /// <summary>
        /// 容器大小变化：更新游标位置
        /// </summary>
        private void TimelineTrack_Resize(object sender, EventArgs e)
        {
            UpdateCursorPosition();
        }
        #endregion
    }
}
