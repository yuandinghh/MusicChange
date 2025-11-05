using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using NAudio.Wave;

namespace MusicChange
{
	public partial class MediaItemControg : UserControl
	{
		public event EventHandler<MediaPlayEventArgs> MediaPlayRequested;

		public bool IsSelected { get; set; } = false;
		public string FilePath
		{
			get; set;
		}
		public string TimeLength
		{
			get; set;
		}
		public Image Image
		{
			get; set;
		}
		public string ImagePath
		{
			get; set;
		}
		public MediaType MediaType
		{
			get; set;
		}

		public MediaItemControg(string filePath, MediaType mediaType)
		{
			FilePath = filePath;
			MediaType = mediaType;
			InitializeComponent();
			//InitializeRuntime();
		}

		private void InitializeRuntime( )
		{
			try {
				lblFileName.Text = Path.GetFileName( FilePath );
				this.AllowDrop = true;
				this.pictureBoxThumbnail.Click += pictureBoxThumbnail_Click;
				this.btnPlay.Click += (s, e) => PlayMedia();
				this.LTimeLength.Text = "时长: --:--";
				// 异步加载缩略图/波形与时长
				_ = SetThumbnailAsync( FilePath, MediaType );
			}
			catch(Exception ex)
			{
				//窗口显示 异常错误
				DebugWrite( "初始化控件失败" );
				MessageBox.Show($"初始化控件失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				//this.Dispose();

			}
		}

		private async Task SetThumbnailAsync(string filePath, MediaType mediaType)
		{
			try {
				if (mediaType == MediaType.Image && File.Exists( filePath )) {
					using (var img = Image.FromFile( filePath )) {
						pictureBoxThumbnail.Image = ResizeImage( img, 200, 120 );
					}
					LTimeLength.Visible = false;
					TimeLength = "";
					return;
				}

				if (mediaType == MediaType.Audio) {
					// 渲染波形并显示音频时长（NAudio）
					var bmp = await Task.Run( ( ) => WaveformRenderer.RenderWaveform( filePath, 300, 90, Color.Black, Color.Lime ) );
					if (bmp != null) {
						pictureBoxThumbnail.InvokeIfRequired( ( ) => pictureBoxThumbnail.Image = bmp );
					}

					try {
						using var afr = new AudioFileReader( filePath );
						var total = afr.TotalTime;
						TimeLength = FormatTime( total );
						LTimeLength.InvokeIfRequired( ( ) => LTimeLength.Text = TimeLength );
						LTimeLength.InvokeIfRequired( ( ) => LTimeLength.Visible = true );
					}
					catch {
						LTimeLength.InvokeIfRequired( ( ) => LTimeLength.Text = "时长: 未知" );
					}

					return;
				}

				// 视频：先尝试用 FFmpeg 快照
				string ffmpegExe = Path.Combine( Application.StartupPath, "ffmpeg", "ffmpeg.exe" );
				string tempThumb = Path.Combine( Path.GetTempPath(), $"thumb_{Guid.NewGuid():N}.jpg" );
				bool extracted = false;
				if (File.Exists( ffmpegExe )) {
					double positionSec = 1.0; // 0~(duration) 的安全位置
											  // 如果能获取时长用更合适位置（尝试 LibVLC quick duration)
					var durationMs = await GetVideoDurationWithLibVLCAsync( filePath );
					if (durationMs > 2000)
						positionSec = Math.Max( 0.5, durationMs / 1000.0 * 0.1 );
					extracted = await Task.Run( ( ) => FFmpegHelper.ExtractFrame( ffmpegExe, filePath, positionSec, tempThumb ) );
					if (extracted && File.Exists( tempThumb )) {
						using var img = Image.FromFile( tempThumb );
						var r = ResizeImage( img, 200, 120 );
						pictureBoxThumbnail.InvokeIfRequired( ( ) => pictureBoxThumbnail.Image = r );
						Image = (Image)r.Clone();
						ImagePath = tempThumb;
					}
				}

				// 如果 FFmpeg 未成功，用 LibVLC 快照/备用
				if (!extracted) {
					var videoInfo = GetVideoInfoWithLibVLC( filePath );
					if (videoInfo?.Thumbnail != null) {
						var r = ResizeImage( videoInfo.Thumbnail, 200, 120 );
						pictureBoxThumbnail.InvokeIfRequired( ( ) => pictureBoxThumbnail.Image = r );
						Image = videoInfo.Thumbnail;
						ImagePath = videoInfo.FilePath;
					}
				}

				// 获取时长（LibVLC）
				var durMs = await GetVideoDurationWithLibVLCAsync( filePath );
				if (durMs > 0) {
					TimeLength = FormatTime( TimeSpan.FromMilliseconds( durMs ) );
					LTimeLength.InvokeIfRequired( ( ) => { LTimeLength.Text = TimeLength; LTimeLength.Visible = true; } );
				}
				else {
					LTimeLength.InvokeIfRequired( ( ) => { LTimeLength.Text = "时长: 未知"; LTimeLength.Visible = true; } );
				}
			}
			catch (Exception ex) {
				DebugWrite( $"SetThumbnailAsync 失败: {ex.Message}" );
			}
		}

		//private void GetVideoInfoWithLibVLC(string filePath)   // LibVLC
		//{
		//	return null;
		//	//throw new NotImplementedException();
		//}

		private VideoInfo GetVideoInfoWithLibVLC(string filePath)   // LibVLC
		{
			var result = new VideoInfo();
			LibVLC libVlc = null;
			Media media = null;
			MediaPlayer player = null;

			try
			{
				// 创建 LibVLC 实例，禁用音频输出和视频标题显示
				libVlc = new LibVLC("--no-audio", "--no-video-title-show");
				media = new Media(libVlc, filePath, FromType.FromPath);
				player = new MediaPlayer(media);

				// 解析媒体以获取元数据
				media.Parse(MediaParseOptions.ParseNetwork);

				// 等待解析完成
				System.Threading.Thread.Sleep(500);

				// 获取视频时长
				long durationMs = media.Duration;
				if(durationMs > 0)
				{
					var ts = TimeSpan.FromMilliseconds(durationMs);
					result.DurationSeconds = ts.Hours > 0 ? ts.ToString(@"hh\:mm\:ss") : ts.ToString(@"mm\:ss");
				}

				// 为获取快照，需要播放媒体
				player.Mute = true; // 确保静音
				player.Play();

				// 等待播放器准备好
				var sw = System.Diagnostics.Stopwatch.StartNew();
				while(sw.ElapsedMilliseconds < 2000)
				{
					if(player.State == VLCState.Playing || player.State == VLCState.Opening)
						break;
					System.Threading.Thread.Sleep(50);
				}

				// 生成快照文件路径
				string snapshotPath = Path.Combine(Path.GetTempPath(), $"snapshot_{Guid.NewGuid():N}.jpg");

				// 尝试获取快照
				bool snapshotTaken = player.TakeSnapshot(0, snapshotPath, 0, 0);

				if(snapshotTaken)
				{
					// 等待文件写入完成
					sw.Restart();
					while(sw.ElapsedMilliseconds < 2000)
					{
						if(File.Exists(snapshotPath) && new FileInfo(snapshotPath).Length > 0)
						{
							System.Threading.Thread.Sleep(100);
							break;
						}
						System.Threading.Thread.Sleep(50);
					}

					// 如果快照文件存在且有效，加载为图像
					if(File.Exists(snapshotPath) && new FileInfo(snapshotPath).Length > 0)
					{
						using(var fs = new FileStream(snapshotPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
						{
							result.Thumbnail = Image.FromStream(fs);
						}
						result.FilePath = snapshotPath;
					}
				}

				player.Stop();
			}
			catch(Exception ex)
			{
				DebugWrite($"GetVideoInfoWithLibVLC 失败: {ex.Message}");
			}
			finally
			{
				// 释放资源
				player?.Dispose();
				media?.Dispose();
				libVlc?.Dispose();
			}

			return result;
		}
		private static string FormatTime(TimeSpan t)
		{
			if (t.TotalHours >= 1)
				return t.ToString( @"hh\:mm\:ss" );
			return t.ToString( @"mm\:ss" );
		}

		private void PlayMedia( )
		{
			try {
				MediaPlayRequested?.Invoke( this, new MediaPlayEventArgs( FilePath, MediaType ) );
			}
			catch (Exception ex) {
				DebugWrite( $"PlayMedia 异常: {ex.Message}" );
			}
		}

		private async Task<long> GetVideoDurationWithLibVLCAsync(string path)
		{
			try {
				// 临时创建 LibVLC 实例以查询时长（不会播放声音）
				using var lib = new LibVLC( "--no-audio", "--no-video-title-show" );
				using var media = new Media( lib, path, FromType.FromPath );
				// 尝试解析（同步或短超时）
				await Task.Run( ( ) => media.Parse( MediaParseOptions.ParseLocal ) );
				long ms = media.Duration;
				if (ms <= 0) {
					// fallback: try short parse network
					await Task.Run( ( ) => media.Parse( MediaParseOptions.ParseNetwork ) );
					ms = media.Duration;
				}
				return Math.Max( 0, ms );
			}
			catch {
				return 0;
			}
		}

		private static void DebugWrite(string s)
		{
			try {
				System.Diagnostics.Debug.WriteLine( s );
			}
			catch { }
		}

		private Image ResizeImage(Image image, int maxWidth, int maxHeight)
		{
			if (image == null)
				return null;
			int w = image.Width;
			int h = image.Height;
			double ratio = Math.Min( (double)maxWidth / w, (double)maxHeight / h );
			if (ratio >= 1.0)
				return new Bitmap( image );
			int nw = (int)(w * ratio);
			int nh = (int)(h * ratio);
			var bmp = new Bitmap( nw, nh );
			using (var g = Graphics.FromImage( bmp )) {
				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				g.Clear( Color.Transparent );
				g.DrawImage( image, 0, 0, nw, nh );
			}
			return bmp;
		}

		private void pictureBoxThumbnail_Click(object sender, EventArgs e)
		{
			PlayMedia();
		}

		protected override void Dispose(bool disposing)
		{
			try {
				if (disposing) {
					if (pictureBoxThumbnail?.Image != null) {
						pictureBoxThumbnail.Image.Dispose();
						pictureBoxThumbnail.Image = null;
					}
					if (Image != null) {
						Image.Dispose();
						Image = null;
					}
					components?.Dispose();
				}
			}
			catch { }
			base.Dispose( disposing );
		}
	}

	// 扩展：安全地在任意线程更新控件
	internal static class ControlExtensions   // 扩展方法  public static void InvokeIfRequired(this Control c, Action a)
	{
		public static void InvokeIfRequired(this Control c, Action a)
		{
			if (c == null || a == null)
				return;
			if (c.IsDisposed)
				return;
			if (c.InvokeRequired)
				c.Invoke( a );
			else
				a();
		}
	}
}