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
			SetThumbnail(filePath, mediaType);
			// 异步加载视频缩略图
			if(mediaType == MediaType.Video && File.Exists(filePath))
			{
				LoadVideoThumbnailAsync(filePath);
			}

			btnPlay.Click += (s, e) => PlayMedia();     // 播放按钮点击事件
			// 订阅点击事件
			pictureBoxThumbnail.Click += (s, e) => PlayMedia();
			lblFileName.Click += (s, e) => PlayMedia();
			this.Click += (s, e) => PlayMedia();

		}
		private Image ExtractFrameWithFFmpeg(string videoPath)
		{
			try
			{
				// 使用 Process 调用 ffmpeg 来提取第一帧
				string tempImagePath = Path.GetTempFileName() + ".jpg";

				var processInfo = new ProcessStartInfo
				{
					FileName = "ffmpeg",
					Arguments = $"-i \"{videoPath}\" -ss 00:00:01.000 -vframes 1 -f image2 \"{tempImagePath}\"",
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				};

				using(var process = Process.Start(processInfo))
				{
					process.WaitForExit();

					if(File.Exists(tempImagePath) && new FileInfo(tempImagePath).Length > 0)
					{
						using(var fs = new FileStream(tempImagePath, FileMode.Open, FileAccess.Read))
						{
							Image thumbnail = Image.FromStream(fs);
							File.Delete(tempImagePath);
							return thumbnail;
						}
					}
				}
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"FFmpeg 提取失败: {ex.Message}");
			}

			return null;
		}
		private Image ExtractVideoThumbnail(string videoPath)
		{
			try
			{
				// 按优先级顺序尝试不同的方法

				// 方法1：使用 Windows Shell 获取缩略图
				Image shellThumbnail = ExtractThumbnailWithWindowsAPI(videoPath);
				if(shellThumbnail != null)
					return shellThumbnail;

				// 方法2：使用 FFmpeg（如果可用）
				Image ffmpegThumbnail = ExtractFrameWithFFmpeg(videoPath);
				if(ffmpegThumbnail != null)
					return ffmpegThumbnail;

				// 方法3：使用 VLC 命令行
				Image vlcThumbnail = ExtractFrameWithVLC(videoPath);
				if(vlcThumbnail != null)
					return vlcThumbnail;

				return null;
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"提取视频缩略图失败: {ex.Message}");
				return null;
			}
		}
		// 移除或注释掉有问题的 ExtractFrameUsingVLC 方法，添加以下正确的实现：

		private Image ExtractThumbnailWithWindowsAPI(string filePath)
		{
			try
			{
				// 使用 Windows Shell 获取缩略图
				// 这需要引用 System.Windows.Forms 和相关 COM 组件

				// 方法1：使用系统图标
				Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
				if(icon != null)
				{
					Image image = icon.ToBitmap();
					icon.Dispose();
					return image;
				}
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"使用 Windows API 提取缩略图失败: {ex.Message}");
			}

			return null;
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
		private async void LoadVideoThumbnailAsync(string videoPath)
		{
			try
			{
				// 在后台线程加载缩略图
				Image thumbnail = await Task.Run(() => ExtractVideoThumbnail(videoPath));

				// 在 UI 线程更新图片
				if(thumbnail != null && !this.IsDisposed)
				{
					this.Invoke(new Action(() =>
					{
						if(!pictureBoxThumbnail.IsDisposed && pictureBoxThumbnail != null)
						{
							pictureBoxThumbnail.Image = ResizeImage(thumbnail, 100, 75);
						}
						thumbnail?.Dispose();
					}));
				}
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"异步加载缩略图失败: {ex.Message}");
				// 出错时使用默认图标
				this.Invoke(new Action(() =>
				{
					if(!pictureBoxThumbnail.IsDisposed && pictureBoxThumbnail != null)
					{
						pictureBoxThumbnail.Image = Properties.Resources.DefaultVideoThumbnail;
					}
				}));
			}
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
		private void SetThumbnail(string filePath, MediaType mediaType)
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
					// 对于视频文件，异步提取缩略图
					LoadVideoThumbnailAsync(filePath);
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