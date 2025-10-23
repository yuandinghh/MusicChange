// MediaItemControl.cs
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using Vlc.DotNet.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using NAudio.Wave;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace MusicChange
{
	public partial class MediaItemControl:UserControl
	{
		private AudioPlayer _audioPlayer; // NAudio 播放器实例
		private static System.Windows.Media.MediaPlayer player;
		private static readonly Random _random = new(Guid.NewGuid().GetHashCode());
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
			FilePath = filePath;
			MediaType = mediaType;

			// 设置控件尺寸
			//this.Size = new Size(180, 220);
			//this.Margin = new Padding(10, 10, 10, 10);
			//this.Padding = new Padding(5);
			this.Size = new Size(120, 180); // 增加高度以容纳更多信息
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

			// 先显示默认图标
			//SetDefaultThumbnail(mediaType);
			SetThumbnailAsync(filePath, mediaType);  // 显示缩略图
			// 异步加载视频缩略图
			//if(mediaType == MediaType.Video && File.Exists(filePath))
			//{
			//	LoadVideoThumbnailAsync(filePath);
			//}

			btnPlay.Click += (s, e) => PlayMedia();     // 播放按钮点击事件
														// 订阅点击事件
			pictureBoxThumbnail.Click += (s, e) => PlayMedia();
			lblFileName.Click += (s, e) => PlayMedia();
			this.Click += (s, e) => PlayMedia();

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
		{           // 触发播放事件
			MediaPlayRequested?.Invoke(this, new MediaPlayEventArgs(FilePath, MediaType));
		}
		private Image ExtractFrameWithVLC(string videoPath)
		{
			try
			{
				// 检查 VLC 是否安装
				string vlcPath = FindVLCPath();
				if(string.IsNullOrEmpty(vlcPath) || !File.Exists(vlcPath))
				{
					System.Diagnostics.Debug.WriteLine("VLC 未安装或路径无效");
					return null;
				}

				// 创建临时目录存储截图
				string tempDir = Path.Combine(Path.GetTempPath(), "VideoThumbnails");
				Directory.CreateDirectory(tempDir);

				string screenshotPath = Path.Combine(tempDir, $"{Guid.NewGuid()}.png");

				// 使用 VLC 命令行截图
				var processInfo = new ProcessStartInfo
				{
					FileName = vlcPath,
					Arguments = $"\"{videoPath}\" --intf dummy --dummy-quiet --snapshot-path \"{tempDir}\" --snapshot-format png --snapshot-preview 0 --start-time 1 --stop-time 1 --run-time 1 vlc://quit",
					UseShellExecute = false,
					CreateNoWindow = true,
					WindowStyle = ProcessWindowStyle.Hidden
				};

				using(var process = Process.Start(processInfo))
				{
					// 等待最多10秒
					if(process.WaitForExit(10000))
					{
						// 检查截图是否生成
						if(File.Exists(screenshotPath))
						{
							using(var fs = new FileStream(screenshotPath, FileMode.Open, FileAccess.Read))
							{
								Image thumbnail = Image.FromStream(fs);
								// 清理临时文件
								try
								{
									File.Delete(screenshotPath);
									Directory.Delete(tempDir, true);
								}
								catch(Exception cleanupEx)
								{
									System.Diagnostics.Debug.WriteLine($"清理临时文件失败: {cleanupEx.Message}");
								}
								return thumbnail;
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"VLC 截图失败: {ex.Message}");
			}

			return null;
		}

		private string FindVLCPath()
		{
			// 常见的 VLC 安装路径
			string[] vlcPaths = {
		@"C:\Program Files\VideoLAN\VLC\vlc.exe",
		@"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe",
		Environment.GetEnvironmentVariable("ProgramFiles") + @"\VideoLAN\VLC\vlc.exe",
		Environment.GetEnvironmentVariable("ProgramFiles(x86)") + @"\VideoLAN\VLC\vlc.exe"
	};

			foreach(string path in vlcPaths)
			{
				if(File.Exists(path))
				{
					return path;
				}
			}

			return "vlc"; // 尝试使用 PATH 中的 vlc
		}
		private async Task SetThumbnailAsync(string filePath, MediaType mediaType)  // 设置缩略图
		{
			try
			{
				if(mediaType == MediaType.Image && File.Exists(filePath))
				{
					// 对于图片文件，尝试加载缩略图
					using(Image originalImage = Image.FromFile(filePath))
					{
						pictureBoxThumbnail.Image = ResizeImage(originalImage, 100, 75); // 调整为适合控件的大小
					}
				}
				else if(mediaType == MediaType.Video && File.Exists(filePath))
				{
					var videoInfo = await GetVideoInfo(FilePath);
					// 保存首帧图片（示例）
					if(videoInfo.Thumbnail != null)
					{
						ImagePath = videoInfo.FilePath;
						Image = videoInfo.Thumbnail;
						pictureBoxThumbnail.Image = Image;
					}
					if(videoInfo != null)
					{
						string durationText = $"时长: {videoInfo.DurationSeconds}";
						TimeLength = videoInfo.DurationSeconds;
						TimeLength = durationText;
					}
				}
				else if(mediaType == MediaType.Audio)
				{
					// 音频文件使用默认图标
					pictureBoxThumbnail.Image = Properties.Resources.music43;
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
				// 初始化 LibVLC
				using var libVlc = new LibVLC();
				using var media = new Media(libVlc, filePath, FromType.FromPath);
				using var player = new MediaPlayer(media);

				// 启动播放以便 LibVLC 填充 Length 并允许 TakeSnapshot
				player.Play();

				// 等待播放器进入 Playing 或拿到长度，最多等待 3 秒
				var sw = System.Diagnostics.Stopwatch.StartNew();
				while(sw.ElapsedMilliseconds < 5000)
				{
					if(player.Length > 0 || player.State == VLCState.Playing)
						break;
					await Task.Delay(100);
				}

				// 获取视频时长
				long lengthMs = player.Length;
				var ts = TimeSpan.FromMilliseconds(Math.Max(0, lengthMs));
				result.DurationSeconds = ts.Hours > 0 ? ts.ToString(@"hh\:mm\:ss") : ts.ToString(@"mm\:ss");

				// 保存封面到当前文件夹
				// 1. 生成当前 Unix 毫秒级时间戳（UTC时间，从1970-01-01开始）
				long unixTimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
				//string timestampStr = unixTimestamp.ToString(); 将时间戳转换为本地时间字符串精确到毫秒
				DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(unixTimestamp);
				string timestampStr = dateTimeOffset.ToLocalTime().ToString("yyyyMMddHHmmssfff");
				int randomNum;  // 2. 生成三位随机数（000-999）
				lock(_random) // 锁定随机数生成，确保多线程安全
				{
					randomNum = _random.Next(0, 1000); // 范围 [0, 999]
				}
				string randomStr = randomNum.ToString("D3"); // 确保三位，不足补零（如 5 → "005"）
				string filename = $"{timestampStr}_{randomStr}.jpg";
				// 
				string thumbnailPath = Path.Combine(Directory.GetCurrentDirectory(), filename);
				bool snapshotSuccess = player.TakeSnapshot(0u, thumbnailPath, 0u, 0u);

				if(snapshotSuccess && File.Exists(thumbnailPath))
				{
					result.Thumbnail = Image.FromFile(thumbnailPath);
					result.FilePath = thumbnailPath;
				}
				player.Stop();      // 停止播放
				//player.Dispose();
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"获取视频信息时出错: {ex.Message}");
			}

			return result;
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