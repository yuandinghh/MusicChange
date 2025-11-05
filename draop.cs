using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LibVLCSharp.Shared;

namespace MediaTimelineSample
{
    public class MainForm : Form
    {
        private TimelineControl timeline;
        private Button btnAddFiles;
        private LibVLC _libVLC;

        public MainForm()
        {
            this.Text = "媒体时间线示例";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1000, 600);

            _libVLC = new LibVLC(); // 使用默认参数

            btnAddFiles = new Button { Text = "添加文件...", Dock = DockStyle.Top, Height = 36 };
            btnAddFiles.Click += BtnAddFiles_Click;

            timeline = new TimelineControl(_libVLC)
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30),
                AutoScroll = true
            };

            this.Controls.Add(timeline);
            this.Controls.Add(btnAddFiles);

            // 允许从资源管理器拖入文件
            this.AllowDrop = true;
            this.DragEnter += MainForm_DragEnter;
            this.DragDrop += MainForm_DragDrop;
        }
        
        
//        使用说明（短）
//?	把三个文件加入项目（命名空间均为 MusicChange）。确保项目引用 LibVLCSharp、LibVLCSharp.WinForms、NAudio。
//?	把 ffmpeg.exe 放到 .\ffmpeg\ffmpeg.exe（位于应用程序启动目录），或修改 FFmpegHelper 的路径引用。
//?	你的主窗体（如 LaserEditing）中已有逻辑把 new MediaItemControl(filePath, mediaType) 添加到 flowLayoutPanelMedia，因此只需替换/添加上面的控件实现即可。控件在点击缩略图或 Play 按钮时会触发 MediaPlayRequested，和你现有的 OnMediaPlayRequested 兼容。
//?	若需要更精确的峰值图（高分辨率采样、时间刻度）或把视频缩略图改为通过你已有 _libVLC1 实例获取（避免重复创建 LibVLC），我可以进一步把获取视频时长与快照方法改成接收 LibVLC 实例并复用。
//如果你同意我把 GetVideoDuration / snapshot 改为接受外部 LibVLC（避免每个控件创建实例），我会把 MediaItemControl 的构造改为 MediaItemControl(string filePath, MediaType mediaType, LibVLC libVlc = null) 并使用传入实例优先。需要我现在做这项优化吗？





        private void BtnAddFiles_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                ofd.Filter = "媒体文件|*.mp4;*.mkv;*.avi;*.mp3;*.wav;*.flac;*.mov|所有文件|*.*";
                if (ofd.ShowDialog() != DialogResult.OK) return;
                foreach (var f in ofd.FileNames)
                    timeline.AddMediaFile(f);
            }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var f in files)
                timeline.AddMediaFile(f);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _libVLC?.Dispose();
        }
    }
}

//using System;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Windows.Forms;
//using LibVLCSharp.Shared;

namespace MediaTimelineSample
{
    public class TimelineControl : FlowLayoutPanel
    {
        private LibVLC _libVLC;
        public TimelineControl(LibVLC libVLC)
        {
            _libVLC = libVLC;
            this.WrapContents = false;
            this.FlowDirection = FlowDirection.LeftToRight;
            this.AutoScroll = true;
            this.Padding = new Padding(8);
            this.AllowDrop = true;
            this.DragEnter += TimelineControl_DragEnter;
            this.DragDrop += TimelineControl_DragDrop;
        }

        private void TimelineControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }

        private void TimelineControl_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var f in files) AddMediaFile(f);
        }

        public void AddMediaFile(string path)
        {
            if (!File.Exists(path)) return;
            var ext = Path.GetExtension(path).ToLowerInvariant();
            var item = new MediaItemControl(_libVLC, path)
            {
                Width = 220,
                Height = 140,
                Margin = new Padding(6)
            };
            this.Controls.Add(item);
        }
    }
}

/*
明与注意事项（必读，简短）
?	把上述文件加入同一命名空间项目（示例使用 MediaTimelineSample，你可以改为 MusicChange）；
?	安装 NuGet 包 LibVLCSharp.WinForms、LibVLCSharp、NAudio；
?	把 ffmpeg.exe 放到 Application.StartupPath\ffmpeg\ffmpeg.exe，或修改 MediaItemControl 中 ffmpegExe 路径到你本机的 ffmpeg；
?	LibVLC 需要能加载 libvlc；若在运行时报错请把 VLC 安装目录加入 PATH 或把 libvlc 的 DLL 复制到输出目录；
?	代码为示例：波形绘制为快速实现（粗略），若需要更精确波形请改用分段采样并绘制高质量曲线；
?	Media.Parse(MediaParseOptions.ParseLocal) 在某些系统上可能耗时或工作方式不同，如遇不返回时长可使用 ffprobe 或其他媒资库代替；
?	若需要拖拽重新排序、时间轴缩放（时间坐标转换、播放位置裁剪等），可以在 TimelineControl 与 MediaItemControl 基础上扩展鼠标拖动与时间映射逻辑。
如果你要我把代码改成直接集成到你当前项目命名空间 MusicChange（并把 Avatar / 其它控件整合），或要我把 wave 渲染改为更精确的峰值图与时间刻度、或者加入视频内嵌预览（LibVLC VideoView）示例，我可以继续完善。


下面把之前的示例修改为直接集成到你当前项目命名空间 MusicChange 的实现。主要包含三份可直接加入项目的文件：
?	Controls/MediaItemControl.cs：完整实现 MediaItemControl（与你已有的 Designer partial 配合），支持视频缩略图（FFmpeg 快照或 LibVLC 备用）、音频波形（NAudio）、显示精确时长，并在点击时触发播放请求事件。
?	Helpers/FFmpegHelper.cs：调用 ffmpeg.exe 提取单帧快照并获取视频时长（可选）。
?	Helpers/WaveformRenderer.cs：用 NAudio 读取音频并渲染峰值波形位图（更精确的峰值图）。
说明（短）
?	将 ffmpeg.exe 放到 Application.StartupPath\ffmpeg\ffmpeg.exe（或修改路径）。
?	需要 NuGet：LibVLCSharp、LibVLCSharp.WinForms、NAudio。
?	这些文件使用你项目已有的 MediaPlayEventArgs、MediaType 等类型；若项目中已定义则直接集成，否则告诉我我可以再补全。
代码：

using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using NAudio.Wave;
*/
namespace MusicChange
{
    public partial class MediaItemControl : UserControl
    {
        public event EventHandler<MediaPlayEventArgs> MediaPlayRequested;

        public bool IsSelected { get; set; } = false;
        public string FilePath { get; set; }
        public string TimeLength { get; set; }
        public Image Image { get; set; }
        public string ImagePath { get; set; }
        public MediaType MediaType { get; set; }

        public MediaItemControl(string filePath, MediaType mediaType)
        {
            FilePath = filePath;
            MediaType = mediaType;
            InitializeComponent();
            InitializeRuntime();
        }

        private void InitializeRuntime()
        {
            try
            {
                lblFileName.Text = Path.GetFileName(FilePath);
                this.AllowDrop = true;
                this.pictureBoxThumbnail.Click += pictureBoxThumbnail_Click;
                this.btnPlay.Click += (s, e) => PlayMedia();
                this.LTimeLength.Text = "时长: --:--";
                // 异步加载缩略图/波形与时长
                _ = SetThumbnailAsync(FilePath, MediaType);
            }
            catch { }
        }

        private async Task SetThumbnailAsync(string filePath, MediaType mediaType)
        {
            try
            {
                if (mediaType == MediaType.Image && File.Exists(filePath))
                {
                    using (var img = Image.FromFile(filePath))
                    {
                        pictureBoxThumbnail.Image = ResizeImage(img, 200, 120);
                    }
                    LTimeLength.Visible = false;
                    TimeLength = "";
                    return;
                }

                if (mediaType == MediaType.Audio)
                {
                    // 渲染波形并显示音频时长（NAudio）
                    var bmp = await Task.Run(() => WaveformRenderer.RenderWaveform(filePath, 300, 90, Color.Black, Color.Lime));
                    if (bmp != null)
                    {
                        pictureBoxThumbnail.InvokeIfRequired(() => pictureBoxThumbnail.Image = bmp);
                    }

                    try
                    {
                        using var afr = new AudioFileReader(filePath);
                        var total = afr.TotalTime;
                        TimeLength = FormatTime(total);
                        LTimeLength.InvokeIfRequired(() => LTimeLength.Text = TimeLength);
                        LTimeLength.InvokeIfRequired(() => LTimeLength.Visible = true);
                    }
                    catch
                    {
                        LTimeLength.InvokeIfRequired(() => LTimeLength.Text = "时长: 未知");
                    }

                    return;
                }

                // 视频：先尝试用 FFmpeg 快照
                string ffmpegExe = Path.Combine(Application.StartupPath, "ffmpeg", "ffmpeg.exe");
                string tempThumb = Path.Combine(Path.GetTempPath(), $"thumb_{Guid.NewGuid():N}.jpg");
                bool extracted = false;
                if (File.Exists(ffmpegExe))
                {
                    double positionSec = 1.0; // 0~(duration) 的安全位置
                    // 如果能获取时长用更合适位置（尝试 LibVLC quick duration)
                    var durationMs = await GetVideoDurationWithLibVLCAsync(filePath);
                    if (durationMs > 2000) positionSec = Math.Max(0.5, durationMs / 1000.0 * 0.1);
                    extracted = await Task.Run(() => FFmpegHelper.ExtractFrame(ffmpegExe, filePath, positionSec, tempThumb));
                    if (extracted && File.Exists(tempThumb))
                    {
                        using var img = Image.FromFile(tempThumb);
                        var r = ResizeImage(img, 200, 120);
                        pictureBoxThumbnail.InvokeIfRequired(() => pictureBoxThumbnail.Image = r);
                        Image = (Image)r.Clone();
                        ImagePath = tempThumb;
                    }
                }

                // 如果 FFmpeg 未成功，用 LibVLC 快照/备用
                if (!extracted)
                {
                    var videoInfo = await GetVideoInfoWithLibVLCAsync(filePath);
                    if (videoInfo?.Thumbnail != null)
                    {
                        var r = ResizeImage(videoInfo.Thumbnail, 200, 120);
                        pictureBoxThumbnail.InvokeIfRequired(() => pictureBoxThumbnail.Image = r);
                        Image = videoInfo.Thumbnail;
                        ImagePath = videoInfo.FilePath;
                    }
                }

                // 获取时长（LibVLC）
                var durMs = await GetVideoDurationWithLibVLCAsync(filePath);
                if (durMs > 0)
                {
                    TimeLength = FormatTime(TimeSpan.FromMilliseconds(durMs));
                    LTimeLength.InvokeIfRequired(() => { LTimeLength.Text = TimeLength; LTimeLength.Visible = true; });
                }
                else
                {
                    LTimeLength.InvokeIfRequired(() => { LTimeLength.Text = "时长: 未知"; LTimeLength.Visible = true; });
                }
            }
            catch (Exception ex)
            {
                DebugWrite($"SetThumbnailAsync 失败: {ex.Message}");
            }
        }

        private static string FormatTime(TimeSpan t)
        {
            if (t.TotalHours >= 1)
                return t.ToString(@"hh\:mm\:ss");
            return t.ToString(@"mm\:ss");
        }

        private void PlayMedia()
        {
            try
            {
                MediaPlayRequested?.Invoke(this, new MediaPlayEventArgs(FilePath, MediaType));
            }
            catch (Exception ex)
            {
                DebugWrite($"PlayMedia 异常: {ex.Message}");
            }
        }

        private async Task<long> GetVideoDurationWithLibVLCAsync(string path)
        {
            try
            {
                // 临时创建 LibVLC 实例以查询时长（不会播放声音）
                using var lib = new LibVLC("--no-audio", "--no-video-title-show");
                using var media = new Media(lib, path, FromType.FromPath);
                // 尝试解析（同步或短超时）
                await Task.Run(() => media.Parse(MediaParseOptions.ParseLocal));
                long ms = media.Duration;
                if (ms <= 0)
                {
                    // fallback: try short parse network
                    await Task.Run(() => media.Parse(MediaParseOptions.ParseNetwork));
                    ms = media.Duration;
                }
                return Math.Max(0, ms);
            }
            catch
            {
                return 0;
            }
        }

        private static void DebugWrite(string s)
        {
            try { System.Diagnostics.Debug.WriteLine(s); } catch { }
        }

        private Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            if (image == null) return null;
            int w = image.Width;
            int h = image.Height;
            double ratio = Math.Min((double)maxWidth / w, (double)maxHeight / h);
            if (ratio >= 1.0) return new Bitmap(image);
            int nw = (int)(w * ratio);
            int nh = (int)(h * ratio);
            var bmp = new Bitmap(nw, nh);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.Clear(Color.Transparent);
                g.DrawImage(image, 0, 0, nw, nh);
            }
            return bmp;
        }

        private void pictureBoxThumbnail_Click(object sender, EventArgs e)
        {
            PlayMedia();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (pictureBoxThumbnail?.Image != null)
                    {
                        pictureBoxThumbnail.Image.Dispose();
                        pictureBoxThumbnail.Image = null;
                    }
                    if (Image != null)
                    {
                        Image.Dispose();
                        Image = null;
                    }
                    components?.Dispose();
                }
            }
            catch { }
            base.Dispose(disposing);
        }
    }

    // 扩展：安全地在任意线程更新控件
    internal static class ControlExtensions
    {
        public static void InvokeIfRequired(this Control c, Action a)
        {
            if (c == null || a == null) return;
            if (c.IsDisposed) return;
            if (c.InvokeRequired) c.Invoke(a);
            else a();
        }
    }
}




//使用说明（短）
//?	把三个文件加入项目（命名空间均为 MusicChange）。确保项目引用 LibVLCSharp、LibVLCSharp.WinForms、NAudio。
//?	把 ffmpeg.exe 放到 .\ffmpeg\ffmpeg.exe（位于应用程序启动目录），或修改 FFmpegHelper 的路径引用。
//?	你的主窗体（如 LaserEditing）中已有逻辑把 new MediaItemControl(filePath, mediaType) 添加到 flowLayoutPanelMedia，因此只需替换/添加上面的控件实现即可。控件在点击缩略图或 Play 按钮时会触发 MediaPlayRequested，和你现有的 OnMediaPlayRequested 兼容。
//?	若需要更精确的峰值图（高分辨率采样、时间刻度）或把视频缩略图改为通过你已有 _libVLC1 实例获取（避免重复创建 LibVLC），我可以进一步把获取视频时长与快照方法改成接收 LibVLC 实例并复用。
//如果你同意我把 GetVideoDuration / snapshot 改为接受外部 LibVLC（避免每个控件创建实例），我会把 MediaItemControl 的构造改为 MediaItemControl(string filePath, MediaType mediaType, LibVLC libVlc = null) 并使用传入实例优先。需要我现在做这项优化吗？


//Controls/MediaItemControl.cs
//using System;
//using System.Drawing;
//using System.IO;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using LibVLCSharp.Shared;
//using NAudio.Wave;

namespace MusicChange
{
    public partial class MediaItemControl : UserControl
    {
        public event EventHandler<MediaPlayEventArgs> MediaPlayRequested;

        public bool IsSelected { get; set; } = false;
        public string FilePath { get; set; }
        public string TimeLength { get; set; }
        public Image Image { get; set; }
        public string ImagePath { get; set; }
        public MediaType MediaType { get; set; }

        // 可选外部 LibVLC 实例（若传入则复用，避免为每个控件创建 LibVLC）
        private readonly LibVLC _sharedLibVlc;

        public MediaItemControl(string filePath, MediaType mediaType, LibVLC sharedLibVlc = null)
        {
            FilePath = filePath;
            MediaType = mediaType;
            _sharedLibVlc = sharedLibVlc;
            InitializeComponent();
            InitializeRuntime();
        }

        private void InitializeRuntime()
        {
            try
            {
                lblFileName.Text = Path.GetFileName(FilePath);
                this.AllowDrop = true;
                this.pictureBoxThumbnail.Click += pictureBoxThumbnail_Click;
                this.btnPlay.Click += (s, e) => PlayMedia();
                this.LTimeLength.Text = "时长: --:--";
                // 异步加载缩略图/波形与时长（优先使用共享 LibVLC）
                _ = SetThumbnailAsync(FilePath, MediaType);
            }
            catch { }
        }

        private async Task SetThumbnailAsync(string filePath, MediaType mediaType)
        {
            try
            {
                if (mediaType == MediaType.Image && File.Exists(filePath))
                {
                    using (var img = Image.FromFile(filePath))
                    {
                        var r = ResizeImage(img, 200, 120);
                        pictureBoxThumbnail.InvokeIfRequired(() => pictureBoxThumbnail.Image = r);
                        Image = r;
                    }
                    LTimeLength.InvokeIfRequired(() => LTimeLength.Visible = false);
                    TimeLength = "";
                    return;
                }

                if (mediaType == MediaType.Audio)
                {
                    // 渲染波形并显示音频时长（NAudio）
                    var bmp = await Task.Run(() => WaveformRenderer.RenderWaveform(filePath, 300, 90, Color.Black, Color.Lime));
                    if (bmp != null)
                    {
                        pictureBoxThumbnail.InvokeIfRequired(() => pictureBoxThumbnail.Image = bmp);
                        Image = bmp;
                    }

                    try
                    {
                        using var afr = new AudioFileReader(filePath);
                        var total = afr.TotalTime;
                        TimeLength = FormatTime(total);
                        LTimeLength.InvokeIfRequired(() => { LTimeLength.Text = TimeLength; LTimeLength.Visible = true; });
                    }
                    catch
                    {
                        LTimeLength.InvokeIfRequired(() => { LTimeLength.Text = "时长: 未知"; LTimeLength.Visible = true; });
                    }

                    return;
                }

                // 视频：优先用 FFmpeg 快照，其次使用共享 LibVLC（若提供），否则创建临时 LibVLC
                string ffmpegExe = Path.Combine(Application.StartupPath, "ffmpeg", "ffmpeg.exe");
                string tempThumb = Path.Combine(Path.GetTempPath(), $"thumb_{Guid.NewGuid():N}.jpg");
                bool extracted = false;

                if (File.Exists(ffmpegExe))
                {
                    // 尝试从视频 10% 处抽帧（若无法获取时长使用默认1s）
                    double positionSec = 1.0;
                    long dur = 0;
                    if (_sharedLibVlc != null)
                        dur = await GetVideoDurationWithLibVLCAsync(filePath, _sharedLibVlc);
                    else
                        dur = await GetVideoDurationWithLibVLCAsync(filePath, null);

                    if (dur > 2000) positionSec = Math.Max(0.5, dur / 1000.0 * 0.1);

                    extracted = await Task.Run(() => FFmpegHelper.ExtractFrame(ffmpegExe, filePath, positionSec, tempThumb));
                    if (extracted && File.Exists(tempThumb))
                    {
                        using var img = Image.FromFile(tempThumb);
                        var r = ResizeImage(img, 200, 120);
                        pictureBoxThumbnail.InvokeIfRequired(() => pictureBoxThumbnail.Image = r);
                        Image = r;
                        ImagePath = tempThumb;
                    }
                }

                // 如果 FFmpeg 没成功，尝试使用 LibVLC（优先使用共享实例）
                if (!extracted)
                {
                    LibVLC libToUse = _sharedLibVlc;
                    bool createdLocalLib = false;
                    if (libToUse == null)
                    {
                        libToUse = new LibVLC("--no-audio", "--no-video-title-show");
                        createdLocalLib = true;
                    }

                    try
                    {
                        var vi = await GetVideoInfoWithLibVLCAsync(filePath, libToUse);
                        if (vi?.Thumbnail != null)
                        {
                            var r = ResizeImage(vi.Thumbnail, 200, 120);
                            pictureBoxThumbnail.InvokeIfRequired(() => pictureBoxThumbnail.Image = r);
                            Image = r;
                            ImagePath = vi.FilePath;
                        }

                        if (vi != null && !string.IsNullOrEmpty(vi.DurationSeconds))
                        {
                            TimeLength = vi.DurationSeconds;
                            LTimeLength.InvokeIfRequired(() => { LTimeLength.Text = TimeLength; LTimeLength.Visible = true; });
                        }
                    }
                    finally
                    {
                        if (createdLocalLib)
                            libToUse.Dispose();
                    }
                }
                else
                {
                    // 用 FFmpeg 得到时长也可用 LibVLC 或 ffprobe；这里再尝试用 LibVLC 查询时长（优先共享）
                    long durMs = 0;
                    if (_sharedLibVlc != null)
                        durMs = await GetVideoDurationWithLibVLCAsync(filePath, _sharedLibVlc);
                    else
                        durMs = await GetVideoDurationWithLibVLCAsync(filePath, null);

                    if (durMs > 0)
                    {
                        TimeLength = FormatTime(TimeSpan.FromMilliseconds(durMs));
                        LTimeLength.InvokeIfRequired(() => { LTimeLength.Text = TimeLength; LTimeLength.Visible = true; });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SetThumbnailAsync 失败: {ex.Message}");
            }
        }

        private static string FormatTime(TimeSpan t)
        {
            if (t.TotalHours >= 1)
                return t.ToString(@"hh\:mm\:ss");
            return t.ToString(@"mm\:ss");
        }

        private void PlayMedia()
        {
            try
            {
                MediaPlayRequested?.Invoke(this, new MediaPlayEventArgs(FilePath, MediaType));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PlayMedia 异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 使用指定的 LibVLC（可为 null）获取视频时长（ms）。
        /// 若传入 libVlc 为 null，会临时创建一个实例并释放。
        /// </summary>
        private static async Task<long> GetVideoDurationWithLibVLCAsync(string path, LibVLC libVlc)
        {
            bool createdLocal = false;
            LibVLC lib = libVlc;
            if (lib == null)
            {
                lib = new LibVLC("--no-audio", "--no-video-title-show");
                createdLocal = true;
            }

            try
            {
                using var media = new Media(lib, path, FromType.FromPath);
                // Parse metadata (may block), do in Task.Run to avoid UI thread blocking
                await Task.Run(() => media.Parse(MediaParseOptions.ParseLocal));
                long ms = media.Duration; // ms
                return Math.Max(0, ms);
            }
            catch
            {
                return 0;
            }
            finally
            {
                if (createdLocal)
                    lib.Dispose();
            }
        }


        /// <summary>
        /// 使用给定 LibVLC（若为 null 则临时创建）通过 MediaPlayer.TakeSnapshot 获取快照并返回 VideoInfo（包含 Thumbnail 和 DurationSeconds）
        /// </summary>
        private static async Task<VideoInfo> GetVideoInfoWithLibVLCAsync(string path, LibVLC libVlc)
        {
            var result = new VideoInfo();
            bool createdLocal = false;
            LibVLC lib = libVlc;
            if (lib == null)
            {
                lib = new LibVLC("--no-audio", "--no-video-title-show");
                createdLocal = true;
            }

            Media media = null;
            MediaPlayer player = null;
            string snapshotPath = null;
            try
            {
                media = new Media(lib, path, FromType.FromPath);
                player = new MediaPlayer(media);

                // 解析获取时长
                await Task.Run(() => media.Parse(MediaParseOptions.ParseLocal));
                long dur = media.Duration;
                if (dur > 0)
                    result.DurationSeconds = (dur >= 3600 * 1000) ? TimeSpan.FromMilliseconds(dur).ToString(@"hh\:mm\:ss") : TimeSpan.FromMilliseconds(dur).ToString(@"mm\:ss");

                // 播放并取快照（静音）
                player.Mute = true;
                player.Play();

                // 等待播放器准备（短超时）
                var sw = System.Diagnostics.Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < 2000)
                {
                    if (player.Length > 0 || player.State == VLCState.Playing || player.State == VLCState.Opening)
                        break;
                    await Task.Delay(50);
                }

                snapshotPath = Path.Combine(Path.GetTempPath(), $"snapshot_{Guid.NewGuid():N}.jpg");
                bool taken = player.TakeSnapshot(0u, snapshotPath, 320u, 180u);
                if (taken)
                {
                    // 等待文件写入
                    var waitSw = System.Diagnostics.Stopwatch.StartNew();
                    while (waitSw.ElapsedMilliseconds < 3000)
                    {
                        if (File.Exists(snapshotPath) && new FileInfo(snapshotPath).Length > 0)
                        {
                            await Task.Delay(80); // ensure write finished
                            break;
                        }
                        await Task.Delay(50);
                    }

                    if (File.Exists(snapshotPath) && new FileInfo(snapshotPath).Length > 0)
                    {
                        using var fs = new FileStream(snapshotPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using var ms = new MemoryStream();
                        await fs.CopyToAsync(ms);
                        ms.Position = 0;
                        result.Thumbnail = Image.FromStream(ms);
                        result.FilePath = snapshotPath;
                    }
                }

                player.Stop();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetVideoInfoWithLibVLCAsync 失败: {ex.Message}");
            }
            finally
            {
                try { player?.Stop(); } catch { }
                player?.Dispose();
                media?.Dispose();
                if (createdLocal)
                    lib.Dispose();
            }
            return result;
        }

        private Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            if (image == null) return null;
            int w = image.Width;
            int h = image.Height;
            double ratio = Math.Min((double)maxWidth / w, (double)maxHeight / h);
            if (ratio >= 1.0) return new Bitmap(image);
            int nw = (int)(w * ratio);
            int nh = (int)(h * ratio);
            var bmp = new Bitmap(nw, nh);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.Clear(Color.Transparent);
                g.DrawImage(image, 0, 0, nw, nh);
            }
            return bmp;
        }

        private void pictureBoxThumbnail_Click(object sender, EventArgs e)
        {
            PlayMedia();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (pictureBoxThumbnail?.Image != null)
                    {
                        pictureBoxThumbnail.Image.Dispose();
                        pictureBoxThumbnail.Image = null;
                    }
                    if (Image != null)
                    {
                        Image.Dispose();
                        Image = null;
                    }
                    components?.Dispose();
                }
            }
            catch { }
            base.Dispose(disposing);
        }
    }

    // 扩展：安全地在任意线程更新控件
    internal static class ControlExtensions
    {
        public static void InvokeIfRequired(this Control c, Action a)
        {
            if (c == null || a == null) return;
            if (c.IsDisposed) return;
            if (c.InvokeRequired) c.Invoke(a);
            else a();
        }
    }
}

//更改说明（简短）
//?	新增构造器签名 MediaItemControl(string filePath, MediaType mediaType, LibVLC sharedLibVlc = null)；若传入 sharedLibVlc，控件会复用该实例来获取视频时长和拍摄快照，避免为每个控件创建或销毁 LibVLC。
//?	SetThumbnailAsync 优先使用 FFmpeg 提取快照（若可用），否则使用共享或临时的 LibVLC 快照。GetVideoInfoWithLibVLCAsync 与 GetVideoDurationWithLibVLCAsync 已改为接受外部 LibVLC。
//?	保留对音频（NAudio 波形渲染）和图片支持。
//下一步建议
//?	在 LaserEditing.InitializeLibVLC() 初始化后，把共享实例 _libVLC1（或 libVLC）传给创建 MediaItemControl 的地方：
//?	替换 new MediaItemControl(filePath, mediaType) 为 new MediaItemControl(filePath, mediaType, _libVLC1)，这样控件复用 LibVLC，性能好且不重复加载本机依赖。
//需要我现在把你项目中所有创建 MediaItemControl 的位置（比如 Click 方法中）批量替换为传入 _libVLC1 的形式并给出补丁吗？

//GitHub Copilot
//我已把 MediaItemControl 的创建调用改为传入共享的 _libVLC1 实例，避免每个控件重复创建 LibVLC，提升性能并避免库初始化开销。下面给出修改后的方法片段（直接替换你项目中对应的 Click 方法）。仅替换下面这部分即可。

private new void Click2(object sender, EventArgs e)
{
    listBox1.Items.Clear();
    if(!Directory.Exists(subDirectory))
    {
        try
        {
            Directory.CreateDirectory(subDirectory);
            Debug.WriteLine("子目录创建成功: " + subDirectory);
        }
        catch(Exception ex)
        {
            MessageBox.Show("创建子目录失败: " + ex.Message);
            return;
        }
    }
    using OpenFileDialog ofd = new()
    {
        Multiselect = true,
        Filter = "媒体文件|*.mp4;*.avi;*.jpg;*.png;*.mp3;*.wav|视频文件|*.mp4;*.avi|音频文件|*.mp3;*.wav|图片文件|*.jpg;*.png",
        Title = "请选择要导入的音频、视频或图片文件",
        InitialDirectory = subDirectory  //指定初始目录 文件夹
    };

    if(ofd.ShowDialog() != DialogResult.OK)
        return;

    var database = new db(db.dbPath);

    // 已加载文件集合（忽略大小写）
    var loadedPaths = new HashSet<string>(flowLayoutPanelMedia.Controls.OfType<MediaItemControl>().Select(mi => mi.FilePath ?? string.Empty),
        StringComparer.OrdinalIgnoreCase);

    int addedCount = 0;
    int duplicateCount = 0;

    foreach(string filePath in ofd.FileNames)
    {
        MediaType mediaType;
        if(IsVideoFile(filePath))
            mediaType = MediaType.Video;
        else if(IsAudioFile(filePath))
            mediaType = MediaType.Audio;
        else if(IsImageFile(filePath))
            mediaType = MediaType.Image;
        else
            continue;

        // 如果已存在则跳过并高亮已存在项
        if(loadedPaths.Contains(filePath))
        {
            duplicateCount++;
            var existing = flowLayoutPanelMedia.Controls.OfType<MediaItemControl>().FirstOrDefault(mi => string.Equals(mi.FilePath, filePath, StringComparison.OrdinalIgnoreCase));

            if(existing != null)
            {
                try
                {
                    existing.BringToFront();
                    var oldColor = existing.BackColor;
                    existing.BackColor = Color.LightGreen;
                    Task.Run(async () =>
                    {
                        await Task.Delay(500);
                        if(!existing.IsDisposed)
                        {
                            existing.Invoke(new Action(() =>
                            {
                                try
                                {
                                    existing.BackColor = oldColor;
                                }
                                catch { }
                            }));
                        }
                    });
                }
                catch { }
            }

            continue;
        }

        // 新增控件：改为传入共享的 _libVLC1 实例（如未初始化也可传 null）
        MediaItemControl mediaItem = new MediaItemControl(filePath, mediaType, _libVLC1);
        flowLayoutPanelMedia.Controls.Add(mediaItem);

        loadedPaths.Add(filePath);
        addedCount++;

        listBox1.Items.Add(mediaItem.MediaType);
        listBox1.Items.Add(mediaItem.FilePath ?? string.Empty);
        try
        {
            listBox1.Items.Add(mediaItem.Image?.Width + "x" + mediaItem.Image?.Height);
        }
        catch { }
        listBox1.Items.Add(mediaItem.Location.X);
        listBox1.Items.Add(mediaItem.Location.Y);

        // 写入数据库（仅对新添加项）
        try
        {
            var fi = new FileInfo(filePath);
            var asset = new MediaAsset
            {
                UserId = 1,
                Name = Path.GetFileName(filePath),
                FilePath = filePath,
                FileSize = fi.Exists ? fi.Length : 0,
                MediaType = mediaType.ToString().ToLower(),
                Duration = null,
                Width = null,
                Height = null,
                Framerate = null,
                Codec = null,
                CreatedAt = DateTime.Now
            };

            if(!string.IsNullOrEmpty(mediaItem.TimeLength))
            {
                double? seconds = ParseDurationToSeconds(mediaItem.TimeLength);
                if(seconds.HasValue)
                    asset.Duration = seconds.Value;
            }

            if(mediaItem.Image != null)
            {
                asset.Width = mediaItem.Image.Width;
                asset.Height = mediaItem.Image.Height;
            }

            int newId = database.InsertMediaAsset(asset);
            mediaItem.Tag = newId;
        }
        catch(Exception ex)
        {
            Debug.WriteLine($"写入媒体资源到数据库失败: {ex.Message}");
        }
    }

    // 更新面板可见性与状态显示
    UpdateFlowLayoutVisibility();
    UpdateStatus($"导入完成：新增 {addedCount} 个，已存在 {duplicateCount} 个，当前总计 {flowLayoutPanelMedia.Controls.Count} 个");
    string t = $"导入完成：新增 {addedCount} 个，已存在 {duplicateCount} 个。\n当前已加载：{flowLayoutPanelMedia.Controls.Count} 个";
    MessageBoxHelper.ShowAutoClose("3秒后自动关闭", t, 3000);
}


说明（短）
?	我只修改了创建 MediaItemControl 的地方，传入 _libVLC1：new MediaItemControl(filePath, mediaType, _libVLC1)。
?	确保 _libVLC1 已在 InitializeLibVLC() 中初始化（你已有代码会创建 _libVLC1），因此请在调用导入（Click）之前执行 InitializeLibVLC()（你的 LaserEditing_Load 已经如此）。
?	若你希望同时把其它创建点（若存在）全部替换，我可以再搜索并提供完整补丁。需要我继续替换其它位置并提交完整文件吗？


private new void Click(object sender, EventArgs e)
{
	listBox1.Items.Clear();
	if(!Directory.Exists(subDirectory))
	{
		try
		{
			Directory.CreateDirectory(subDirectory);
			Debug.WriteLine("子目录创建成功: " + subDirectory);
		}
		catch(Exception ex)
		{
			MessageBox.Show("创建子目录失败: " + ex.Message);
			return;
		}
	}
	using OpenFileDialog ofd = new()
	{
		Multiselect = true,
		Filter = "媒体文件|*.mp4;*.avi;*.jpg;*.png;*.mp3;*.wav|视频文件|*.mp4;*.avi|音频文件|*.mp3;*.wav|图片文件|*.jpg;*.png",
		Title = "请选择要导入的音频、视频或图片文件",
		InitialDirectory = subDirectory  //指定初始目录 文件夹
	};

	if(ofd.ShowDialog() != DialogResult.OK)
		return;

	var database = new db(db.dbPath);

	// 已加载文件集合（忽略大小写）
	var loadedPaths = new HashSet<string>(flowLayoutPanelMedia.Controls.OfType<MediaItemControl>().Select(mi => mi.FilePath ?? string.Empty),
		StringComparer.OrdinalIgnoreCase);

	int addedCount = 0;
	int duplicateCount = 0;

	foreach(string filePath in ofd.FileNames)
	{
		MediaType mediaType;
		if(IsVideoFile(filePath))
			mediaType = MediaType.Video;
		else if(IsAudioFile(filePath))
			mediaType = MediaType.Audio;
		else if(IsImageFile(filePath))
			mediaType = MediaType.Image;
		else
			continue;

		// 如果已存在则跳过并高亮已存在项
		if(loadedPaths.Contains(filePath))
		{
			duplicateCount++;
			var existing = flowLayoutPanelMedia.Controls.OfType<MediaItemControl>().FirstOrDefault(mi => string.Equals(mi.FilePath, filePath, StringComparison.OrdinalIgnoreCase));

			if(existing != null)
			{
				try
				{
					// 高亮并滚动可见，然后在短时间后恢复颜色
					existing.BringToFront();
					var oldColor = existing.BackColor;
					existing.BackColor = Color.LightGreen;
					// 异步恢复颜色（避免阻塞 UI 线程）
					Task.Run(async () =>
				   {
					   await Task.Delay(500);
					   if(!existing.IsDisposed)
					   {
						   existing.Invoke(new Action(() =>
						  {
							  try
							  {
								  existing.BackColor = oldColor;
							  }
							  catch { }
						  }));
					   }
				   });
				}
				catch { }
			}

			continue;
		}

		// 新增控件：传入共享的 _libVLC1 实例以复用 LibVLC，减少开销
		MediaItemControl mediaItem = new MediaItemControl(filePath, mediaType, _libVLC1);
		flowLayoutPanelMedia.Controls.Add(mediaItem);

		loadedPaths.Add(filePath);
		addedCount++;

		listBox1.Items.Add(mediaItem.MediaType);
		listBox1.Items.Add(mediaItem.FilePath ?? string.Empty);
		// 可选信息（防止空引用）
		try
		{
			listBox1.Items.Add(mediaItem.Image?.Width + "x" + mediaItem.Image?.Height);
		}
		catch { }
		listBox1.Items.Add(mediaItem.Location.X);
		listBox1.Items.Add(mediaItem.Location.Y);

		// 写入数据库（仅对新添加项）
		try
		{
			var fi = new FileInfo(filePath);
			var asset = new MediaAsset
			{
				UserId = 1,
				Name = Path.GetFileName(filePath),
				FilePath = filePath,
				FileSize = fi.Exists ? fi.Length : 0,
				MediaType = mediaType.ToString().ToLower(),
				Duration = null,
				Width = null,
				Height = null,
				Framerate = null,
				Codec = null,
				CreatedAt = DateTime.Now
			};

			if(!string.IsNullOrEmpty(mediaItem.TimeLength))
			{
				double? seconds = ParseDurationToSeconds(mediaItem.TimeLength);
				if(seconds.HasValue)
					asset.Duration = seconds.Value;
			}

			if(mediaItem.Image != null)
			{
				asset.Width = mediaItem.Image.Width;
				asset.Height = mediaItem.Image.Height;
			}

			int newId = database.InsertMediaAsset(asset);
			mediaItem.Tag = newId;
		}
		catch(Exception ex)
		{
			Debug.WriteLine($"写入媒体资源到数据库失败: {ex.Message}");
		}
	}


	// 更新面板可见性与状态显示
	UpdateFlowLayoutVisibility();
	UpdateStatus($"导入完成：新增 {addedCount} 个，已存在 {duplicateCount} 个，当前总计 {flowLayoutPanelMedia.Controls.Count} 个");
	string t = $"导入完成：新增 {addedCount} 个，已存在 {duplicateCount} 个。\n当前已加载：{flowLayoutPanelMedia.Controls.Count} 个";
	MessageBoxHelper.ShowAutoClose("3秒后自动关闭", t, 3000);
}