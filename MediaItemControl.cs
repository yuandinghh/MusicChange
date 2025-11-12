using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using NAudio.MediaFoundation;
using NAudio.Wave;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace MusicChange
{
	public partial class MediaItemControl:UserControl
	{
		//private static readonly Random _random = new(Guid.NewGuid().GetHashCode());
		private bool _isSelected = false;
		private Color _originalBackColor;
		private LibVLC libVLC;
		private string path;
		public static VideoInfo videoInfo = new();
		private static string errorOutput;

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
		public MediaItemControl(string filePath, MediaType mediaType)   // 构造函数
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

			this.AllowDrop = true;

		}
		public MediaItemControl(LibVLC libVLC, string path)  // 构造函数
		{
			this.libVLC = libVLC;
			this.path = path;
		}
		private void MediaItemControl_Click(object sender, EventArgs e)     // 点击控件切换选中状态
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
		private void PlayMedia()    // 触发播放事件
		{
			MediaPlayRequested?.Invoke(this, new MediaPlayEventArgs(FilePath, MediaType));
		}
		// 自定义事件：请求播放媒体
		public event EventHandler<MediaPlayEventArgs> MediaPlayRequested;
		private void pictureBoxThumbnail_Click(object sender, EventArgs e)      // 触发播放事件
		{
			MediaPlayRequested?.Invoke(this, new MediaPlayEventArgs(FilePath, MediaType));
		}
		private async Task SetThumbnailAsync(string filePath, MediaType mediaType)   // 设置缩略图
		{
			try
			{
				if(mediaType == MediaType.Image && File.Exists(filePath))       // 图片文件
				{
					using Image originalImage = Image.FromFile(filePath);  // 对于图片文件，尝试加载缩略图
					pictureBoxThumbnail.Image = ResizeImage(originalImage, 100, 75); // 调整为适合控件的大小
																					 //获取图片的宽高
					videoInfo.Width = originalImage.Width;
					videoInfo.Height = originalImage.Height;
					LTimeLength.Visible = false;
				}
				else if(mediaType == MediaType.Video && File.Exists(filePath))  // 视频文件
				{
					videoInfo = await ExtractFirstFrameAsync(FilePath);  // 获取视频信息
					if(videoInfo.Thumbnail != null)
					{  // 保存首帧图片（示例）
						ImagePath = videoInfo.FilePath;
						Image = videoInfo.Thumbnail;
						pictureBoxThumbnail.Image = Image;
                        LTimeLength.Visible = true;
						LTimeLength.Text = videoInfo.DurationSeconds;

					}
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
		public static async Task<VideoInfo> GetVideoInfo(string filePath)            //获取视频信息
		{
			try
			{
				// 创建 LibVLC，禁止音频输出以避免任何声音播放
				using var libVlc = new LibVLC(new[] { "--no-audio", "--no-video-title-show" });
				using var media = new Media(libVlc, filePath, FromType.FromPath);
				using var player = new MediaPlayer(media);
				try // 尝试先解析媒体元数据（可获得 duration）
				{
					// Parse 可以在不完全播放的情况下填充媒体信息（异步或同步行为依版本）
					//_ = media.Parse( MediaParseOptions.ParseNetwork );
					//media.parse_with_options(vlc.MediaParseFlag.Local, timeout = 3000); // 同步解析
					// 等一点时间让解析完成（通常很快）
					await Task.Delay(200);
					long durationFromMedia = media.Duration;
					if(durationFromMedia > 0)
					{
						var ts = TimeSpan.FromMilliseconds(durationFromMedia);
						videoInfo.DurationSeconds = ts.Hours > 0 ? ts.ToString(@"hh\:mm\:ss") : ts.ToString(@"mm\:ss");
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
				if(string.IsNullOrEmpty(videoInfo.DurationSeconds) || videoInfo.DurationSeconds == "00:00")
				{
					long lengthMs = player.Length;
					if(lengthMs > 0)
					{
						var ts = TimeSpan.FromMilliseconds(Math.Max(0, lengthMs));
						videoInfo.DurationSeconds = ts.Hours > 0 ? ts.ToString(@"hh\:mm\:ss") : ts.ToString(@"mm\:ss");
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
							videoInfo.Thumbnail = Image.FromStream(fs);
						}
						string filenanme = Path.GetFileName(snapshotPath);
						filenanme = Path.Combine(LaserEditing.subDirectory, "snapshotPath") + "\\" + filenanme;
						File.Copy(snapshotPath, filenanme, true);
						videoInfo.snapshotPath = filenanme;
					}
				}
				// 停止播放器（确保不继续播放）
				player.Stop();
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"获取视频信息时出错: {ex.Message}");
			}
			return videoInfo;
		}
		public static string GetVideoDuration(string videoPath)
		{
			try
			{
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					FileName = @"D:\C#\ffmpegbuild\bin\ffmpeg.exe",
					Arguments = $"-i \"{videoPath}\"",
					RedirectStandardError = true,
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};

				using Process process = Process.Start(startInfo);

				// 重要：读取输出流防止阻塞
				string output = process.StandardOutput.ReadToEnd();
				string errorOutput = process.StandardError.ReadToEnd();

				process.WaitForExit();

				// 时长信息通常在错误输出中
				// 正则匹配时长（格式：Duration: 00:01:23.45）
				Match match = Regex.Match(errorOutput, @"Duration:\s*(\d+:\d+:\d+\.\d+)", RegexOptions.IgnoreCase);
				if(match.Success)
				{
					string durationStr = match.Groups[1].Value;
					if(TimeSpan.TryParse(durationStr, out TimeSpan duration))
					{
						// 格式化输出
						return duration.Hours > 0 ? duration.ToString(@"hh\:mm\:ss") : duration.ToString(@"mm\:ss");
					}
				}
				return "未知";
			}
			catch(Exception ex)
			{
				Debug.WriteLine($"获取视频时长失败: {ex.Message}");
				return "未知";
			}
		}
		public static string  GetVideoDuration2(string videoPath)
		{
			try
			{
				// 构建 FFmpeg 命令：获取视频信息
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					FileName = @"D:\C#\ffmpegbuild\bin\ffmpeg.exe",
					Arguments = $"-i \"{videoPath}\"", // -i 表示输入文件
					RedirectStandardError = true, // 错误输出包含视频信息
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};

				using Process process = Process.Start(startInfo);
				string output = process.StandardError.ReadToEnd(); // 读取错误输出
				process.WaitForExit();
				// 匹配时长
				Match match = Regex.Match(errorOutput, @"Duration:\s*(\d+):(\d+):(\d+\.\d+)");
				if(match.Success)
				{
					int hours = int.Parse(match.Groups[1].Value);
					int minutes = int.Parse(match.Groups[2].Value);
					double seconds = double.Parse(match.Groups[3].Value);
					// 格式化为 HH:mm:ss
                    return $"{hours:00}:{minutes:00}:{seconds:00.00}";
				}
				return null;
			}
			catch(Exception ex)
			{
				//窗口提示
				MessageBox.Show("获取时长失败：" + ex.Message);
				return null;
			}
		}
		public static async Task<string> GetVideoDurationAsync(string videoPath)
		{
			try
			{
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					FileName = @"D:\C#\ffmpegbuild\bin\ffmpeg.exe",
					Arguments = $"-i \"{videoPath}\"",
					RedirectStandardError = true,
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};

				using Process process = new Process();
				process.StartInfo = startInfo;

				var tcs = new TaskCompletionSource<bool>();

				process.EnableRaisingEvents = true;
				process.Exited += (sender, args) => tcs.SetResult(true);

				process.Start();

				// 异步读取输出流防止阻塞
				var outputTask = process.StandardOutput.ReadToEndAsync();
				var errorTask = process.StandardError.ReadToEndAsync();

				// 等待进程完成（带超时）
				using(var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
				{
					var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(-1, cts.Token));

					if(completedTask != tcs.Task)
					{
						if(!process.HasExited)
						{
							process.Kill();
						}
						throw new TimeoutException("FFmpeg获取时长超时");
					}
				}

				string output = await outputTask;
				string errorOutput = await errorTask;

				// 正则匹配时长
				Match match = Regex.Match(errorOutput, @"Duration:\s*(\d+:\d+:\d+\.\d+)", RegexOptions.IgnoreCase);
				if(match.Success)
				{
					string durationStr = match.Groups[1].Value;
					if(TimeSpan.TryParse(durationStr, out TimeSpan duration))
					{
						return duration.Hours > 0 ? duration.ToString(@"hh\:mm\:ss") : duration.ToString(@"mm\:ss");
					}
				}
				return "未知";
			}
			catch(Exception ex)
			{
				Debug.WriteLine($"获取视频时长失败: {ex.Message}");
				return "未知";
			}
		}
		public  static async Task<VideoInfo> ExtractFirstFrameAsync(string videoPath)    //提取第一帧
		{
			 string outputImagePath = $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.jpg";
			 outputImagePath = Path.Combine(LaserEditing.subDirectory, "snapshotPath") + "\\" + outputImagePath;
			videoInfo.snapshotPath = outputImagePath;
			try
			{
				ProcessStartInfo startInfo = new()
				{
					FileName = @"D:\C#\ffmpegbuild\bin\ffmpeg.exe",
					Arguments = $"-i \"{videoPath}\" -ss 0 -vframes 1 -q:v 2 \"{outputImagePath}\"",
					RedirectStandardError = true,
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};

				using Process process = new Process();
				process.StartInfo = startInfo;
				var tcs = new TaskCompletionSource<bool>();  // 使用 TaskCompletionSource 来处理异步等待
				process.EnableRaisingEvents = true;   // 处理进程退出
				process.Exited += (sender, args) =>
				{
					tcs.SetResult(true);
				};

				//process.Exited += (sender, args) =>
				//{
				//	try
				//	{
				//		// 读取输出
				//		string outputa = process.StandardOutput.ReadToEnd();
				//		string errorOutputa = process.StandardError.ReadToEnd();
				//		tcs.SetResult(process.ExitCode == 0);
				//	}
				//	catch(Exception ex)
				//	{
				//		tcs.SetException(ex);
				//	}
				//};

				process.Start();    // 启动进程
				var outputTask = process.StandardOutput.ReadToEndAsync(); // 异步读取输出流以防止阻塞
				var errorTask = process.StandardError.ReadToEndAsync();
				// 等待进程完成
				bool result = await tcs.Task;
				// 确保读取完所有输出
				string output = await outputTask;
				string errorOutput = await errorTask;
		
				Debug.WriteLine("FFmpeg Exit Code: " + process.ExitCode);
				Debug.WriteLine("FFmpeg Output: " + output);
				Debug.WriteLine("FFmpeg Error: " + errorOutput);
				if(process.ExitCode == 0 && File.Exists(outputImagePath))
				{
					using(var fs = new FileStream(outputImagePath, FileMode.Open, FileAccess.Read))
					{
						videoInfo.Thumbnail = Image.FromStream(fs);
					}
					videoInfo.DurationSeconds = GetVideoDuration(videoPath);
					//videoInfo.DurationSeconds = await GetVideoDurationAsync(videoPath);
					//GetVideoDuration(videoPath);
					videoInfo.FilePath = videoPath;
					videoInfo.Width = videoInfo.Thumbnail.Width;
					videoInfo.Height = videoInfo.Thumbnail.Height;
				}
				//videoInfo.Thumbnail = Image.FromFile(outputImagePath);
    //            videoInfo.snapshotPath = outputImagePath;
                //videoInfo.DurationSeconds = GetVideoDuration(videoPath).ToString();
				//Thread.Sleep(1000);
				//videoInfo.FilePath = videoPath;
    //            videoInfo.Width = videoInfo.Thumbnail.Width;
    //            videoInfo.Height = videoInfo.Thumbnail.Height;

				return videoInfo;
			}
			catch(Exception ex)
			{
				MessageBox.Show("提取第一帧失败：" + ex.Message);
				Debug.WriteLine($"提取第一帧失败：{ex.Message}");
				return videoInfo;
			}
		}

		protected override void Dispose(bool disposing)   // Dispose
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
		public string GetAudioDuration(string filePath)  //获取音频时长
		{
			try
			{
				using var audioReader = new AudioFileReader(filePath);
				TimeSpan duration = audioReader.TotalTime;
				// 格式化输出为mm:ss
				TimeLength = duration.ToString(@"mm\:ss");
				videoInfo.DurationSeconds = TimeLength;
				return TimeLength;
			}
			catch(Exception ex)
			{
				// 异常处理（文件不存在/格式不支持等）
				MessageBox.Show($"获取时长失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Debug.WriteLine($"获取音频时长失败: {ex.Message}");
				return "00:00";
			}
		}
		private void MediaItemControl_DragDrop(object sender, DragEventArgs e)  //拖放文件
		{
			// 处理拖放的文件
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			if(files.Length > 0)
			{
				ProcessDroppedFile(files[0]);
			}
		}
		private void MediaItemControl_DragEnter(object sender, DragEventArgs e)
		{
			// 检查拖入的数据是否为文件
			if(e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				if(files.Length > 0 && IsMediaFile(files[0]))
				{
					e.Effect = DragDropEffects.Copy;
					return;
				}
			}
			e.Effect = DragDropEffects.None;
		}
		private void ProcessDroppedFile(string filePath)  //处理拖入的文件
		{
			try
			{
				// 获取媒体文件时长
				TimeSpan duration = GetMediaDuration(filePath);
				// 显示文件信息
				string fileName = Path.GetFileName(filePath);
				string message = $"文件: {fileName}\n时长: {duration:hh\\:mm\\:ss}";

				// 在容器中添加文件信息
				ListViewItem item = new ListViewItem(fileName);
				item.SubItems.Add(duration.ToString(@"hh\:mm\:ss"));
				item.SubItems.Add(filePath);
				//listViewDroppedMedia.Items.Add( item );

				MessageBox.Show(message, "文件已添加");
			}
			catch(Exception ex)
			{
				MessageBox.Show($"处理文件时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		private TimeSpan GetMediaDuration(string filePath)  // 获取媒体文件时长
		{
			string extension = Path.GetExtension(filePath).ToLower();

			if(extension == ".mp4" || extension == ".avi" || extension == ".mov" || extension == ".flv" || extension == ".mkv")
			{
				return GetVideoDurationo(filePath);
			}

			throw new NotSupportedException("不支持的媒体格式");
		}
		// 获取视频文件时长
		private TimeSpan GetVideoDurationo(string filePath)
		{
			// 初始化MediaFoundation
			MediaFoundationApi.Startup();

			try
			{
				using(var reader = new MediaFoundationReader(filePath))
				{
					return reader.TotalTime;
				}
			}
			finally
			{
				MediaFoundationApi.Shutdown();
			}
		}
		// 检查是否为支持的媒体文件
		private bool IsMediaFile(string filePath)
		{
			string extension = Path.GetExtension(filePath).ToLower();
			string[] supportedExtensions = { ".mp3", ".wav", ".wma", ".mp4", ".avi", ".mov", ".flv", ".mkv" };
			return Array.IndexOf(supportedExtensions, extension) >= 0;
		}
	}

	public enum MediaType       // 媒体类型枚举
	{
		Video, Audio, Image
	}
	// class  播放事件参数
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