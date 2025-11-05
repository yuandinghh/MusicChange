using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using NAudio.MediaFoundation;
using NAudio.Wave;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace MusicChange
{
	public partial class MediaItemControl : UserControl
	{
		//private static readonly Random _random = new(Guid.NewGuid().GetHashCode());
		private bool _isSelected = false;
		private Color _originalBackColor;
		private LibVLC libVLC;
		private string path;

		public bool IsSelected
		{
			get => _isSelected;
			set {
				_isSelected = value;
				UpdateSelectionAppearance();
			}
		}
		public string FilePath
		{
			get; private set;
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
			get; private set;
		}
		public MediaItemControl(string filePath, MediaType mediaType)   // 构造函数
		{
			InitializeComponent();
			TimeLength = "未知";
			FilePath = filePath;
			MediaType = mediaType;
			// 保存原始背景色
			_originalBackColor = this.BackColor;
			lblFileName.Text = Path.GetFileName( filePath );

			btnPlay.Visible = false;
			btnPlay.BringToFront();

			_ = SetThumbnailAsync( filePath, mediaType );  // 根据媒体类型设置初始显示

			btnPlay.Click += (s, e) => PlayMedia();     // 播放按钮点击事件
			pictureBoxThumbnail.Click += (s, e) => PlayMedia();  // 订阅点击事件
			lblFileName.Click += (s, e) => PlayMedia();
			this.Click += (s, e) => PlayMedia();
			// 添加点击事件来切换选中状态
			this.Click += MediaItemControl_Click;
			pictureBoxThumbnail.Click += MediaItemControl_Click;
			lblFileName.Click += MediaItemControl_Click;

			this.AllowDrop = true;

		}

		public MediaItemControl(LibVLC libVLC, string path)  // 构造函数
		{
			this.libVLC = libVLC;
			this.path = path;
		}

		// 点击控件切换选中状态
		private void MediaItemControl_Click(object sender, EventArgs e)
		{
			IsSelected = !IsSelected;
		}
		// 更新选中状态的外观
		private void UpdateSelectionAppearance( )
		{
			if (_isSelected) {
				// 选中状态：蓝色边框和背景
				this.BackColor = Color.LightBlue;
				this.BorderStyle = BorderStyle.FixedSingle;
			}
			else {
				// 未选中状态：恢复原始样式
				this.BackColor = _originalBackColor;
				this.BorderStyle = BorderStyle.None;
			}
		}
		public class VideoInfo    // 视频信息类
		{
			public string DurationSeconds { get; set; } = "未知";
			public Image Thumbnail
			{
				get; set;
			}
			public string FilePath
			{
				get; set;
			}
			public int Width
			{
				get; set;
			}
			public int Height
			{
				get; set;
			}
		}
		private Image ResizeImage(Image image, int maxWidth, int maxHeight)  // 缩略图缩放
		{
			// 计算缩放比例
			double ratioX = (double)maxWidth / image.Width;
			double ratioY = (double)maxHeight / image.Height;
			double ratio = Math.Min( ratioX, ratioY );

			int newWidth = (int)(image.Width * ratio);
			int newHeight = (int)(image.Height * ratio);

			// 创建缩略图
			Bitmap newImage = new Bitmap( newWidth, newHeight );
			using (Graphics graphics = Graphics.FromImage( newImage )) {
				graphics.DrawImage( image, 0, 0, newWidth, newHeight );
			}

			return newImage;
		}
		private void PlayMedia( )    // 触发播放事件
		{       
			MediaPlayRequested?.Invoke( this, new MediaPlayEventArgs( FilePath, MediaType ) );
		}
		// 自定义事件：请求播放媒体
		public event EventHandler<MediaPlayEventArgs> MediaPlayRequested;
		private void pictureBoxThumbnail_Click(object sender, EventArgs e)      // 触发播放事件
		{       
			MediaPlayRequested?.Invoke( this, new MediaPlayEventArgs( FilePath, MediaType ) );
		}

		private async Task SetThumbnailAsync(string filePath, MediaType mediaType)	 // 设置缩略图
		{

			try {
				if (mediaType == MediaType.Image && File.Exists( filePath )) {
					using Image originalImage = Image.FromFile( filePath );  // 对于图片文件，尝试加载缩略图
					pictureBoxThumbnail.Image = ResizeImage( originalImage, 100, 75 ); // 调整为适合控件的大小
					LTimeLength.Visible = false;
				}
				else if (mediaType == MediaType.Video && File.Exists( filePath )) {
					var videoInfo = await GetVideoInfo( FilePath );
					//(string inputPath, string ? outputDir = null, TimeSpan ? seek = null, int width = 320, int quality = 2, CancellationToken cancellation = default)
					//string outputDir = null;
					//TimeSpan seek = new TimeSpan(1, 1, 1, 1, 1);
					//var videoInfo = await ExtractThumbnailAsync(FilePath, outputDir, seek, 320, 2);

					if (videoInfo.Thumbnail != null) {  // 保存首帧图片（示例）
						ImagePath = videoInfo.FilePath;
						Image = videoInfo.Thumbnail;
						pictureBoxThumbnail.Image = Image;
						TimeLength = videoInfo.DurationSeconds;
						LTimeLength.Text = TimeLength;
					}
					//else  
					//{
					//	//延时一秒    ？？？？？？？？？？？？？？
					//	await Task.Delay(2000);
					//	videoInfo = await GetVideoInfo(FilePath);
					//	if(videoInfo.Thumbnail != null)
					//	{
					//		ImagePath = videoInfo.FilePath;
					//		Image = videoInfo.Thumbnail;
					//		pictureBoxThumbnail.Image = Image;
					//		TimeLength = videoInfo.DurationSeconds;
					//		LTimeLength.Text = TimeLength;
					//	}
					//}
				}
				else if (mediaType == MediaType.Audio) {
					// 音频文件使用默认图标
					pictureBoxThumbnail.Image = Properties.Resources.music43;
					LTimeLength.Text = GetAudioDuration( FilePath );
				}
			}
			catch (Exception ex) {
				// 出错时使用默认图标
				pictureBoxThumbnail.Image = Properties.Resources.DefaultVideoThumbnail;
				System.Diagnostics.Debug.WriteLine( $"加载缩略图失败: {ex.Message}" );
			}
		}
		public static async Task<VideoInfo> GetVideoInfo(string filePath)			 //获取视频信息
		{
			var result = new VideoInfo();
			try {
				// 创建 LibVLC，禁止音频输出以避免任何声音播放
				using var libVlc = new LibVLC( new[] { "--no-audio", "--no-video-title-show" } );
				using var media = new Media( libVlc, filePath, FromType.FromPath );
				using var player = new MediaPlayer( media );
				try // 尝试先解析媒体元数据（可获得 duration）
				{
					// Parse 可以在不完全播放的情况下填充媒体信息（异步或同步行为依版本）
					_ = media.Parse( MediaParseOptions.ParseNetwork );
					// 等一点时间让解析完成（通常很快）
					await Task.Delay( 200 );
					long durationFromMedia = media.Duration;
					if (durationFromMedia > 0) {
						var ts = TimeSpan.FromMilliseconds( durationFromMedia );
						result.DurationSeconds = ts.Hours > 0 ? ts.ToString( @"hh\:mm\:ss" ) : ts.ToString( @"mm\:ss" );
					}
				}
				catch {
					// 忽略解析失败，后面会尝试从 player.Length 获取
				}

				// 需要快照时，MediaPlayer.TakeSnapshot 要求 MediaPlayer 实际运行一次。
				// 启动并立即静音（LibVLC 已用 --no-audio），再请求快照并等待文件出现。
				player.Mute = true; // 额外保证不出声音
				player.Play();
				player.Mute = false; // 额外保证不出声音
				var sw = System.Diagnostics.Stopwatch.StartNew();   // 等待播放器进入可用状态或直到超时
				while (sw.ElapsedMilliseconds < 2000) {
					if (player.Length > 0 || player.State == VLCState.Playing)
						break;
					await Task.Delay( 100 );
				}

				// 如果还没有通过 media.Parse 得到时长，尝试从 player.Length 读取
				if (string.IsNullOrEmpty( result.DurationSeconds ) || result.DurationSeconds == "00:00") {
					long lengthMs = player.Length;
					if (lengthMs > 0) {
						var ts = TimeSpan.FromMilliseconds( Math.Max( 0, lengthMs ) );
						result.DurationSeconds = ts.Hours > 0 ? ts.ToString( @"hh\:mm\:ss" ) : ts.ToString( @"mm\:ss" );
					}
				}

				// 生成唯一文件名并请求快照
				string timestampStr = DateTime.Now.ToString( "yyyyMMddHHmmssfff" );
				string snapshotName = $"{timestampStr}.jpg";
				string snapshotPath = Path.Combine( Directory.GetCurrentDirectory(), snapshotName );

				// 请求快照（文件写入是异步由 VLC 完成）
				bool requestOk = player.TakeSnapshot( 0u, snapshotPath, 0u, 0u );
				if (requestOk) {
					// 等待文件被写入（最多等待 3 秒）
					var waitSw = System.Diagnostics.Stopwatch.StartNew();
					while (waitSw.ElapsedMilliseconds < 2000) {
						if (File.Exists( snapshotPath ) && new FileInfo( snapshotPath ).Length > 0)
							break;
						await Task.Delay( 100 );
					}

					if (File.Exists( snapshotPath ) && new FileInfo( snapshotPath ).Length > 0) {
						// 安全加载图像到内存，避免文件句柄被锁定
						using (var fs = new FileStream( snapshotPath, FileMode.Open, FileAccess.Read, FileShare.Read )) {
							result.Thumbnail = Image.FromStream( fs );
						}
						result.FilePath = snapshotPath;
						// 如果不想保留文件，可在此删除；当前保留文件（按你的需求决定）
						// File.Delete(snapshotPath);
					}
				}
				// 停止播放器（确保不继续播放）
				player.Stop();

			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine( $"获取视频信息时出错: {ex.Message}" );
			}

			return result;
		}
		protected override void Dispose(bool disposing)   // Dispose
		{
			if (disposing) {
				try {
					// 清理图像资源
					if (Image != null) {
						Image.Dispose();
						Image = null;
					}

					// 清理 pictureBox 中的图像
					if (pictureBoxThumbnail?.Image != null &&
						pictureBoxThumbnail.Image != Properties.Resources.DefaultVideoThumbnail &&
						pictureBoxThumbnail.Image != Properties.Resources.music43) {
						var oldImage = pictureBoxThumbnail.Image;
						pictureBoxThumbnail.Image = null;
						oldImage?.Dispose();
					}
				}
				catch (Exception ex) {
					Debug.WriteLine( $"清理 MediaItemControl 资源时出错: {ex.Message}" );
				}
			}

			base.Dispose( disposing );
		}

		public static string GetAudioDuration(string filePath)  //获取音频时长
		{
			try {
				using (var audioReader = new AudioFileReader( filePath )) {
					TimeSpan duration = audioReader.TotalTime;

					// 格式化输出为mm:ss
					string formattedDuration = duration.ToString( @"mm\:ss" );
					// 处理超过1小时的情况（可选）
					//if(duration.TotalHours > 0)
					//{
					//	formattedDuration = duration.ToString(@"hh\:mm\:ss");
					//}

					return formattedDuration;
				}
			}
			catch (Exception ex) {
				// 异常处理（文件不存在/格式不支持等）
				Debug.WriteLine( $"获取音频时长失败: {ex.Message}" );
				return "00:00";
			}
		}

		private void MediaItemControl_DragDrop(object sender, DragEventArgs e)
		{
			// 处理拖放的文件
			string[] files = (string[])e.Data.GetData( DataFormats.FileDrop );
			if (files.Length > 0) {
				ProcessDroppedFile( files[0] );
			}
		}
		private void MediaItemControl_DragEnter(object sender, DragEventArgs e)
		{
			// 检查拖入的数据是否为文件
			if (e.Data.GetDataPresent( DataFormats.FileDrop )) {
				string[] files = (string[])e.Data.GetData( DataFormats.FileDrop );
				if (files.Length > 0 && IsMediaFile( files[0] )) {
					e.Effect = DragDropEffects.Copy;
					return;
				}
			}
			e.Effect = DragDropEffects.None;
		}
		private void ProcessDroppedFile(string filePath)  //处理拖入的文件
		{
			try {
				// 获取媒体文件时长
				TimeSpan duration = GetMediaDuration( filePath );

				// 显示文件信息
				string fileName = Path.GetFileName( filePath );
				string message = $"文件: {fileName}\n时长: {duration:hh\\:mm\\:ss}";

				// 在容器中添加文件信息
				ListViewItem item = new ListViewItem( fileName );
				item.SubItems.Add( duration.ToString( @"hh\:mm\:ss" ) );
				item.SubItems.Add( filePath );
				//listViewDroppedMedia.Items.Add( item );

				MessageBox.Show( message, "文件已添加" );
			}
			catch (Exception ex) {
				MessageBox.Show( $"处理文件时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		// 获取媒体文件时长
		private TimeSpan GetMediaDuration(string filePath)
		{
			string extension = Path.GetExtension( filePath ).ToLower();

			// 音频文件处理
			//if(extension == ".mp3" || extension == ".wav" || extension == ".wma")
			//{
			//	return GetAudioDuration(filePath);
			//}
			// 视频文件处理
			if (extension == ".mp4" || extension == ".avi" || extension == ".mov" || extension == ".flv" || extension == ".mkv") {
				return GetVideoDuration( filePath );
			}

			throw new NotSupportedException( "不支持的媒体格式" );
		}
		// 获取视频文件时长
		private TimeSpan GetVideoDuration(string filePath)
		{
			// 初始化MediaFoundation
			MediaFoundationApi.Startup();

			try {
				using (var reader = new MediaFoundationReader( filePath )) {
					return reader.TotalTime;
				}
			}
			finally {
				MediaFoundationApi.Shutdown();
			}
		}
		// 检查是否为支持的媒体文件
		private bool IsMediaFile(string filePath)
		{
			string extension = Path.GetExtension( filePath ).ToLower();
			string[] supportedExtensions = { ".mp3", ".wav", ".wma", ".mp4", ".avi", ".mov", ".flv", ".mkv" };
			return Array.IndexOf( supportedExtensions, extension ) >= 0;
		}
	}


	public enum MediaType       // 媒体类型枚举
	{
		Video, Audio, Image
	}
	// class  播放事件参数
	public class MediaPlayEventArgs : EventArgs 
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