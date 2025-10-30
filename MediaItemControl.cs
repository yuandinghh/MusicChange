using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using NAudio.Wave;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace MusicChange
{
	public partial class MediaItemControl:UserControl
	{
		//private static readonly Random _random = new(Guid.NewGuid().GetHashCode());
		private bool _isSelected = false;
		private Color _originalBackColor;

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
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
		public MediaItemControl(string filePath, MediaType mediaType)
		{
			InitializeComponent();
			TimeLength = "未知";
			FilePath = filePath;
			MediaType = mediaType;
			// 保存原始背景色
			_originalBackColor = this.BackColor;
			lblFileName.Text = Path.GetFileName(filePath);

			btnPlay.Visible = false;
			btnPlay.BringToFront();

			_ = SetThumbnailAsync(filePath, mediaType);  // 根据媒体类型设置初始显示

			btnPlay.Click += (s, e) => PlayMedia();     // 播放按钮点击事件
			pictureBoxThumbnail.Click += (s, e) => PlayMedia();  // 订阅点击事件
			lblFileName.Click += (s, e) => PlayMedia();
			this.Click += (s, e) => PlayMedia();
			// 添加点击事件来切换选中状态
			this.Click += MediaItemControl_Click;
			pictureBoxThumbnail.Click += MediaItemControl_Click;
			lblFileName.Click += MediaItemControl_Click;

		}

		// 点击控件切换选中状态
		private void MediaItemControl_Click(object sender, EventArgs e)
		{
			IsSelected = !IsSelected;
		}

		// 更新选中状态的外观
		private void UpdateSelectionAppearance()
		{
			if(_isSelected)
			{
				// 选中状态：蓝色边框和背景
				this.BackColor = Color.LightBlue;
				this.BorderStyle = BorderStyle.FixedSingle;
			}
			else
			{
				// 未选中状态：恢复原始样式
				this.BackColor = _originalBackColor;
				this.BorderStyle = BorderStyle.None;
			}
		}

		public class VideoInfo
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
		{       // 触发播放事件
			MediaPlayRequested?.Invoke(this, new MediaPlayEventArgs(FilePath, MediaType));
		}
		// 自定义事件：请求播放媒体
		public event EventHandler<MediaPlayEventArgs> MediaPlayRequested;
		private void pictureBoxThumbnail_Click(object sender, EventArgs e)
		{           // 触发播放事件
			MediaPlayRequested?.Invoke(this, new MediaPlayEventArgs(FilePath, MediaType));
		}

		private async Task SetThumbnailAsync(string filePath, MediaType mediaType)  // 设置缩略图
		{

			try
			{
				if(mediaType == MediaType.Image && File.Exists(filePath))
				{
					using Image originalImage = Image.FromFile(filePath);  // 对于图片文件，尝试加载缩略图
					pictureBoxThumbnail.Image = ResizeImage(originalImage, 100, 75); // 调整为适合控件的大小
					LTimeLength.Visible = false;
				}
				else if(mediaType == MediaType.Video && File.Exists(filePath))
				{
					var videoInfo = await GetVideoInfo(FilePath);
					//(string inputPath, string ? outputDir = null, TimeSpan ? seek = null, int width = 320, int quality = 2, CancellationToken cancellation = default)
					//string outputDir = null;
					//TimeSpan seek = new TimeSpan(1, 1, 1, 1, 1);
					//var videoInfo = await ExtractThumbnailAsync(FilePath, outputDir, seek, 320, 2);

					if(videoInfo.Thumbnail != null)
					{  // 保存首帧图片（示例）
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
				else if(mediaType == MediaType.Audio)
				{
					// 音频文件使用默认图标
					pictureBoxThumbnail.Image = Properties.Resources.music43;
					LTimeLength.Text = GetAudioDuration(FilePath);
				}
			}
			catch(Exception ex)
			{
				// 出错时使用默认图标
				pictureBoxThumbnail.Image = Properties.Resources.DefaultVideoThumbnail;
				System.Diagnostics.Debug.WriteLine($"加载缩略图失败: {ex.Message}");
			}
		}
		public static async Task<VideoInfo> GetVideoInfo(string filePath)   //获取视频信息
		{
			var result = new VideoInfo();
			try
			{
				// 创建 LibVLC，禁止音频输出以避免任何声音播放
				using var libVlc = new LibVLC(new[] { "--no-audio", "--no-video-title-show" });
				using var media = new Media(libVlc, filePath, FromType.FromPath);
				using var player = new MediaPlayer(media);
				try // 尝试先解析媒体元数据（可获得 duration）
				{
					// Parse 可以在不完全播放的情况下填充媒体信息（异步或同步行为依版本）
					_ = media.Parse(MediaParseOptions.ParseNetwork);
					// 等一点时间让解析完成（通常很快）
					await Task.Delay(200);
					long durationFromMedia = media.Duration;
					if(durationFromMedia > 0)
					{
						var ts = TimeSpan.FromMilliseconds(durationFromMedia);
						result.DurationSeconds = ts.Hours > 0 ? ts.ToString(@"hh\:mm\:ss") : ts.ToString(@"mm\:ss");
					}
				}
				catch
				{
					// 忽略解析失败，后面会尝试从 player.Length 获取
				}

				// 需要快照时，MediaPlayer.TakeSnapshot 要求 MediaPlayer 实际运行一次。
				// 启动并立即静音（LibVLC 已用 --no-audio），再请求快照并等待文件出现。
				player.Mute = true; // 额外保证不出声音
				player.Play();
				player.Mute = false; // 额外保证不出声音
				var sw = System.Diagnostics.Stopwatch.StartNew();   // 等待播放器进入可用状态或直到超时
				while(sw.ElapsedMilliseconds < 2000)
				{
					if(player.Length > 0 || player.State == VLCState.Playing)
						break;
					await Task.Delay(100);
				}

				// 如果还没有通过 media.Parse 得到时长，尝试从 player.Length 读取
				if(string.IsNullOrEmpty(result.DurationSeconds) || result.DurationSeconds == "00:00")
				{
					long lengthMs = player.Length;
					if(lengthMs > 0)
					{
						var ts = TimeSpan.FromMilliseconds(Math.Max(0, lengthMs));
						result.DurationSeconds = ts.Hours > 0 ? ts.ToString(@"hh\:mm\:ss") : ts.ToString(@"mm\:ss");
					}
				}

				// 生成唯一文件名并请求快照
				string timestampStr = DateTime.Now.ToString("yyyyMMddHHmmssfff");
				string snapshotName = $"{timestampStr}.jpg";
				string snapshotPath = Path.Combine(Directory.GetCurrentDirectory(), snapshotName);

				// 请求快照（文件写入是异步由 VLC 完成）
				bool requestOk = player.TakeSnapshot(0u, snapshotPath, 0u, 0u);
				if(requestOk)
				{
					// 等待文件被写入（最多等待 3 秒）
					var waitSw = System.Diagnostics.Stopwatch.StartNew();
					while(waitSw.ElapsedMilliseconds < 2000)
					{
						if(File.Exists(snapshotPath) && new FileInfo(snapshotPath).Length > 0)
							break;
						await Task.Delay(100);
					}

					if(File.Exists(snapshotPath) && new FileInfo(snapshotPath).Length > 0)
					{
						// 安全加载图像到内存，避免文件句柄被锁定
						using(var fs = new FileStream(snapshotPath, FileMode.Open, FileAccess.Read, FileShare.Read))
						{
							result.Thumbnail = Image.FromStream(fs);
						}
						result.FilePath = snapshotPath;
						// 如果不想保留文件，可在此删除；当前保留文件（按你的需求决定）
						// File.Delete(snapshotPath);
					}
				}
				// 停止播放器（确保不继续播放）
				player.Stop();

			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"获取视频信息时出错: {ex.Message}");
			}

			return result;
		}

		//public static async Task<VideoInfo> GetVideoInfo(string filePath)
		//{
		//	var result = new VideoInfo();

		//	try
		//	{
		//		// 首先尝试使用 FFmpeg 获取缩略图（更稳定）
		//		//var ffmpegThumbnail = await GetThumbnailWithFFmpeg(filePath);
		//		//if(ffmpegThumbnail != null)
		//		//{
		//		//	result.Thumbnail = ffmpegThumbnail;
		//		//	result.DurationSeconds = await GetVideoDurationWithFFmpeg(filePath);
		//		//	return result;
		//		//}

		//		// 如果 FFmpeg 不可用，使用 LibVLC 作为备用方案
		//		var vlcInfo = await GetVideoInfoWithVLC(filePath);
		//		if(vlcInfo != null)
		//		{
		//			result = vlcInfo;
		//		}
		//		else
		//		{
		//			// 最后的备用方案：只获取时长
		//			result.DurationSeconds = await GetVideoDurationWithFFmpeg(filePath) ?? "未知";
		//		}
		//	}
		//	catch(Exception ex)
		//	{
		//		Debug.WriteLine($"获取视频信息时出错: {ex.Message}");
		//		result.DurationSeconds = "未知";
		//	}

		//	return result;
		//}

		//private static async Task<Image> GetThumbnailWithFFmpeg(string videoPath)
		//{
		//	try
		//	{
		//		string ffmpegPath = FindFfmpegExecutable();
		//		if(string.IsNullOrEmpty(ffmpegPath))
		//			return null;

		//		string tempImagePath = Path.Combine(Path.GetTempPath(), $"thumb_{Guid.NewGuid():N}.jpg");

		//		// FFmpeg 命令：在1秒处获取一帧并调整大小
		//		string arguments = $"-ss 00:00:01.000 -i \"{videoPath}\" -vframes 1 -vf \"scale=320:180:force_original_aspect_ratio=decrease,pad=320:180:(ow-iw)/2:(oh-ih)/2\" -q:v 2 \"{tempImagePath}\" -y";

		//		var processInfo = new ProcessStartInfo
		//		{
		//			FileName = ffmpegPath,
		//			Arguments = arguments,
		//			UseShellExecute = false,
		//			CreateNoWindow = true,
		//			RedirectStandardOutput = true,
		//			RedirectStandardError = true
		//		};

		//		using(var process = Process.Start(processInfo))
		//		{
		//			if(process != null)
		//			{
		//				//await process.WaitForExitAsync();

		//				if(process.ExitCode == 0 && File.Exists(tempImagePath) && new FileInfo(tempImagePath).Length > 0)
		//				{
		//					using(var fs = new FileStream(tempImagePath, FileMode.Open, FileAccess.Read))
		//					{
		//						return Image.FromStream(fs);
		//					}
		//				}
		//			}
		//		}
		//	}
		//	catch(Exception ex)
		//	{
		//		Debug.WriteLine($"FFmpeg 获取缩略图失败: {ex.Message}");
		//	}

		//	return null;
		//}

		//private async Task SetThumbnailAsync(string filePath, MediaType mediaType)
		//{
		//	try
		//	{
		//		// 设置初始状态
		//		pictureBoxThumbnail.SizeMode = PictureBoxSizeMode.CenterImage;
		//		pictureBoxThumbnail.BackColor = Color.Transparent;

		//		if(mediaType == MediaType.Image && File.Exists(filePath))
		//		{
		//			// 图片文件
		//			using(Image originalImage = Image.FromFile(filePath))
		//			{
		//				pictureBoxThumbnail.Image = ResizeImage(originalImage, 100, 75);
		//			}
		//			LTimeLength.Visible = false;
		//		}
		//		else if(mediaType == MediaType.Video && File.Exists(filePath))
		//		{
		//			// 视频文件 - 显示默认图标并异步加载缩略图
		//			pictureBoxThumbnail.Image = Properties.Resources.DefaultVideoThumbnail;

		//			// 异步获取视频信息
		//			var videoInfo = await GetVideoInfo(filePath);

		//			// 在UI线程更新显示
		//			if(!this.IsDisposed && pictureBoxThumbnail != null && !pictureBoxThumbnail.IsDisposed)
		//			{
		//				this.Invoke(new Action(() =>
		//				{
		//					if(videoInfo.Thumbnail != null)
		//					{
		//						try
		//						{
		//							pictureBoxThumbnail.Image = ResizeImage(videoInfo.Thumbnail, 100, 75);
		//							pictureBoxThumbnail.SizeMode = PictureBoxSizeMode.CenterImage;
		//							Image = videoInfo.Thumbnail;
		//							ImagePath = videoInfo.FilePath;
		//						}
		//						catch(Exception ex)
		//						{
		//							Debug.WriteLine($"设置视频缩略图失败: {ex.Message}");
		//						}
		//					}

		//					TimeLength = videoInfo.DurationSeconds ?? "未知";
		//					LTimeLength.Text = TimeLength;
		//					LTimeLength.Visible = true;
		//				}));
		//			}
		//		}
		//		else if(mediaType == MediaType.Audio)
		//		{
		//			// 音频文件
		//			pictureBoxThumbnail.Image = Properties.Resources.music43;
		//			LTimeLength.Text = GetAudioDuration(filePath);
		//			LTimeLength.Visible = true;
		//		}
		//		else
		//		{
		//			// 其他类型
		//			pictureBoxThumbnail.Image = Properties.Resources.DefaultVideoThumbnail;
		//			LTimeLength.Visible = false;
		//		}
		//	}
		//	catch(Exception ex)
		//	{
		//		Debug.WriteLine($"设置缩略图失败: {ex.Message}");
		//		pictureBoxThumbnail.Image = Properties.Resources.DefaultVideoThumbnail;
		//		LTimeLength.Visible = false;
		//	}
		//}
		//private static async Task<VideoInfo> GetVideoInfoWithVLC(string filePath)
		//{
		//	var result = new VideoInfo();
		//	LibVLC libVlc = null;
		//	Media media = null;
		//	MediaPlayer player = null;

		//	try
		//	{
		//		libVlc = new LibVLC("--no-audio", "--no-video-title-show", "--verbose=0");
		//		media = new Media(libVlc, filePath, FromType.FromPath);
		//		player = new MediaPlayer(media);

		//		// 解析媒体信息
		//		await media.Parse(MediaParseOptions.ParseNetwork, 2000);
		//		long durationMs = media.Duration;

		//		if(durationMs > 0)
		//		{
		//			var ts = TimeSpan.FromMilliseconds(durationMs);
		//			result.DurationSeconds = ts.Hours > 0 ? ts.ToString(@"hh\:mm\:ss") : ts.ToString(@"mm\:ss");
		//		}

		//		// 尝试获取快照
		//		player.Mute = true;
		//		player.EnableMouseInput = false;
		//		player.EnableKeyInput = false;
		//		player.Play();

		//		// 等待播放器准备就绪
		//		await Task.Delay(500);

		//		// 生成快照
		//		string snapshotPath = Path.Combine(Path.GetTempPath(), $"snapshot_{Guid.NewGuid():N}.jpg");
		//		bool snapshotTaken = player.TakeSnapshot(0, snapshotPath, 320, 180);

		//		if(snapshotTaken && File.Exists(snapshotPath))
		//		{
		//			// 等待文件完全写入
		//			await Task.Delay(300);

		//			if(new FileInfo(snapshotPath).Length > 0)
		//			{
		//				using(var fs = new FileStream(snapshotPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		//				using(var ms = new MemoryStream())
		//				{
		//					await fs.CopyToAsync(ms);
		//					ms.Position = 0;
		//					result.Thumbnail = Image.FromStream(ms);
		//				}

		//				result.FilePath = snapshotPath;
		//			}
		//		}

		//		player.Stop();
		//	}
		//	catch(Exception ex)
		//	{
		//		Debug.WriteLine($"VLC 获取视频信息失败: {ex.Message}");
		//	}
		//	finally
		//	{
		//		try
		//		{
		//			player?.Stop();
		//			player?.Dispose();
		//			media?.Dispose();
		//			libVlc?.Dispose();
		//		}
		//		catch { }
		//	}

		//	return result;
		//}
		//public static async Task<VideoInfo> GetVideoInfo(string filePath)
		//{
		//	var result = new VideoInfo();
		//	LibVLC? libVlc = null;
		//	Media? media = null;
		//	MediaPlayer? player = null;

		//	try
		//	{
		//		// 创建 LibVLC，禁止音频输出以避免任何声音播放
		//		libVlc = new LibVLC(new[] {
		//	"--no-audio",
		//	"--no-video-title-show",
		//	"--verbose=0" // 减少日志输出
		//      });

		//		media = new Media(libVlc, filePath, FromType.FromPath);
		//		player = new MediaPlayer(media);

		//		// 先尝试解析媒体元数据
		//		try
		//		{
		//			await media.Parse(MediaParseOptions.ParseNetwork, 3000); // 3秒超时
		//			long durationFromMedia = media.Duration;
		//			if(durationFromMedia > 0)
		//			{
		//				var ts = TimeSpan.FromMilliseconds(durationFromMedia);
		//				result.DurationSeconds = ts.Hours > 0 ? ts.ToString(@"hh\:mm\:ss") : ts.ToString(@"mm\:ss");
		//			}
		//		}
		//		catch(Exception ex)
		//		{
		//			Debug.WriteLine($"解析媒体元数据失败: {ex.Message}");
		//		}

		//		// 启动播放器以获取快照
		//		player.Mute = true;
		//		player.Play();

		//		// 等待播放器进入可用状态
		//		var sw = System.Diagnostics.Stopwatch.StartNew();
		//		while(sw.ElapsedMilliseconds < 2000) // 增加到2秒超时
		//		{
		//			if(player.Length > 0 || player.State == VLCState.Playing || player.State == VLCState.Opening)
		//				break;
		//			await Task.Delay(50); // 减少延迟间隔
		//		}

		//		// 如果还没有得到时长，再次尝试
		//		if(string.IsNullOrEmpty(result.DurationSeconds) || result.DurationSeconds == "00:00")
		//		{
		//			long lengthMs = player.Length;
		//			if(lengthMs > 0)
		//			{
		//				var ts = TimeSpan.FromMilliseconds(Math.Max(0, lengthMs));
		//				result.DurationSeconds = ts.Hours > 0 ? ts.ToString(@"hh\:mm\:ss") : ts.ToString(@"mm\:ss");
		//			}
		//		}

		//		// 生成唯一文件名并请求快照
		//		string timestampStr = DateTime.Now.ToString("yyyyMMddHHmmssfff");
		//		string snapshotName = $"{timestampStr}_{Guid.NewGuid():N}.jpg";
		//		string snapshotPath = Path.Combine(Path.GetTempPath(), snapshotName);

		//		// 确保临时目录存在
		//		Directory.CreateDirectory(Path.GetDirectoryName(snapshotPath));

		//		// 请求快照
		//		bool requestOk = player.TakeSnapshot(0u, snapshotPath, 320u, 180u); // 指定尺寸

		//		if(requestOk)
		//		{
		//			// 等待文件被写入（增加等待时间）
		//			var waitSw = System.Diagnostics.Stopwatch.StartNew();
		//			while(waitSw.ElapsedMilliseconds < 3000) // 增加到3秒
		//			{
		//				if(File.Exists(snapshotPath) && new FileInfo(snapshotPath).Length > 0)
		//				{
		//					// 额外等待一小段时间确保文件写入完成
		//					await Task.Delay(100);
		//					break;
		//				}
		//				await Task.Delay(50);
		//			}

		//			if(File.Exists(snapshotPath) && new FileInfo(snapshotPath).Length > 0)
		//			{
		//				try
		//				{
		//					// 使用文件流安全地加载图像
		//					using(var fs = new FileStream(snapshotPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		//					{
		//						// 复制到内存流以避免文件锁定
		//						using(var ms = new MemoryStream())
		//						{
		//							await fs.CopyToAsync(ms);
		//							ms.Position = 0;
		//							result.Thumbnail = Image.FromStream(ms);
		//						}
		//					}
		//					result.FilePath = snapshotPath;
		//				}
		//				catch(Exception ex)
		//				{
		//					Debug.WriteLine($"加载缩略图失败: {ex.Message}");
		//					// 尝试直接从文件加载
		//					try
		//					{
		//						result.Thumbnail = Image.FromFile(snapshotPath);
		//						result.FilePath = snapshotPath;
		//					}
		//					catch(Exception ex2)
		//					{
		//						Debug.WriteLine($"直接加载缩略图也失败: {ex2.Message}");
		//					}
		//				}
		//			}
		//		}

		//		// 停止播放器
		//		player.Stop();
		//	}
		//	catch(Exception ex)
		//	{
		//		Debug.WriteLine($"获取视频信息时出错: {ex.Message}");
		//		result.DurationSeconds = "未知";
		//	}
		//	finally
		//	{
		//		// 确保资源正确释放
		//		try
		//		{
		//			player?.Stop();
		//			player?.Dispose();
		//			media?.Dispose();
		//			libVlc?.Dispose();
		//		}
		//		catch(Exception ex)
		//		{
		//			Debug.WriteLine($"释放资源时出错: {ex.Message}");
		//		}
		//	}

		//	return result;
		//}

		//private static async Task<Image?> GetVideoThumbnailFallback(string filePath)
		//{
		//	try
		//	{
		//		// 使用 FFmpeg 获取缩略图作为备用方案
		//		string? thumbnailPath = await ExtractThumbnailAsync(filePath,
		//			Path.GetTempPath(),
		//			TimeSpan.FromMilliseconds(500),
		//			320, 2);

		//		if(!string.IsNullOrEmpty(thumbnailPath) && File.Exists(thumbnailPath))
		//		{
		//			using(var fs = new FileStream(thumbnailPath, FileMode.Open, FileAccess.Read))
		//			{
		//				return Image.FromStream(fs);
		//			}
		//		}
		//	}
		//	catch(Exception ex)
		//	{
		//		Debug.WriteLine($"备用缩略图获取失败: {ex.Message}");
		//	}

		//	return null;
		//}
		//private async Task SetThumbnailAsync1(string filePath, MediaType mediaType)
		//{
		//	try
		//	{
		//		if(mediaType == MediaType.Image && File.Exists(filePath))
		//		{
		//			using Image originalImage = Image.FromFile(filePath);
		//			pictureBoxThumbnail.Image = ResizeImage(originalImage, 100, 75);
		//			LTimeLength.Visible = false;
		//		}
		//		else if(mediaType == MediaType.Video && File.Exists(filePath))
		//		{
		//			var videoInfo = await GetVideoInfo(filePath);

		//			if(videoInfo.Thumbnail != null)
		//			{
		//				ImagePath = videoInfo.FilePath;
		//				Image = videoInfo.Thumbnail;
		//				pictureBoxThumbnail.Image = ResizeImage(videoInfo.Thumbnail, 100, 75); // 确保缩放
		//			}
		//			else
		//			{
		//				// 如果无法获取缩略图，尝试备用方法
		//				var fallbackThumbnail = await GetVideoThumbnailFallback(filePath);
		//				if(fallbackThumbnail != null)
		//				{
		//					pictureBoxThumbnail.Image = ResizeImage(fallbackThumbnail, 100, 75);
		//					fallbackThumbnail.Dispose();
		//				}
		//				else
		//				{
		//					// 使用默认图标
		//					pictureBoxThumbnail.Image = Properties.Resources.DefaultVideoThumbnail;
		//				}
		//			}

		//			TimeLength = videoInfo.DurationSeconds ?? "未知";
		//			LTimeLength.Text = TimeLength;
		//			LTimeLength.Visible = true;
		//		}
		//		else if(mediaType == MediaType.Audio)
		//		{
		//			// 音频文件使用默认图标
		//			pictureBoxThumbnail.Image = Properties.Resources.music43;
		//			LTimeLength.Text = GetAudioDuration(filePath);
		//			LTimeLength.Visible = true;
		//		}
		//		else
		//		{
		//			// 其他类型使用默认图标
		//			pictureBoxThumbnail.Image = Properties.Resources.DefaultVideoThumbnail;
		//			LTimeLength.Visible = false;
		//		}
		//	}
		//	catch(Exception ex)
		//	{
		//		Debug.WriteLine($"设置缩略图失败: {ex.Message}");
		//		pictureBoxThumbnail.Image = Properties.Resources.DefaultVideoThumbnail;
		//		LTimeLength.Visible = false;
		//	}
		//}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				try
				{
					// 清理图像资源
					if(Image != null)
					{
						Image.Dispose();
						Image = null;
					}

					// 清理 pictureBox 中的图像
					if(pictureBoxThumbnail?.Image != null &&
						pictureBoxThumbnail.Image != Properties.Resources.DefaultVideoThumbnail &&
						pictureBoxThumbnail.Image != Properties.Resources.music43)
					{
						var oldImage = pictureBoxThumbnail.Image;
						pictureBoxThumbnail.Image = null;
						oldImage?.Dispose();
					}
				}
				catch(Exception ex)
				{
					Debug.WriteLine($"清理 MediaItemControl 资源时出错: {ex.Message}");
				}
			}

			base.Dispose(disposing);
		}

		public static string GetAudioDuration(string filePath)  //获取音频时长
		{
			try
			{
				using(var audioReader = new AudioFileReader(filePath))
				{
					TimeSpan duration = audioReader.TotalTime;

					// 格式化输出为mm:ss
					string formattedDuration = duration.ToString(@"mm\:ss");
					// 处理超过1小时的情况（可选）
					//if(duration.TotalHours > 0)
					//{
					//	formattedDuration = duration.ToString(@"hh\:mm\:ss");
					//}

					return formattedDuration;
				}
			}
			catch(Exception ex)
			{
				// 异常处理（文件不存在/格式不支持等）
				Debug.WriteLine($"获取音频时长失败: {ex.Message}");
				return "00:00";
			}
		}

	}  // 类结束
	public enum MediaType       // 媒体类型枚举
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