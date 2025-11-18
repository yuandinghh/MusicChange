using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
		//private LibVLC libVLC;
		//private string path;
		//private static string errorOutput;
		//private MediaAssetsRepository mediaRepo;

		private bool _isSelected = false;
		private Color _originalBackColor;
		public static VideoInfo videoInfo = new();


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
		public bool IsAdded
		{
			get;
			internal set;
		}

		// 1. 使用 event 关键字定义事件
		// EventHandler<TEventArgs> 是标准的事件处理委托
		// 这里我们使用自定义的事件参数类来传递更多信息
		//public event EventHandler<ItemDraggedOutEventArgs> ItemDraggedOut;
		// 简化：使用标准的 EventHandler，不传递自定义参数
		public event EventHandler ItemDraggedOut;

		//// 触发事件的方法
		//protected virtual void OnItemDraggedOut(DragDropEffects effect)
		//{
		//	bool success = effect == DragDropEffects.Copy;
		//	var args = new ItemDraggedOutEventArgs(FilePath, success);
		//	//ItemDraggedOut?.Invoke(this, args);
		//	ItemDraggedOut?.Invoke(this, EventArgs.Empty);
		//}
	
		private void MediaItemControl_MouseDown_1(object sender, MouseEventArgs e)    // 点击控件触发拖放操作
		{
			// 当鼠标左键按下并移动时，开始拖放操作
			if(e.Button == MouseButtons.Left)
			{
				// 创建一个数据对象，用于传递拖放的数据。
				// 我们使用自定义格式 "MediaItemData"，并将文件路径和控件的其他属性（如Name）传递过去。
				DataObject dragData = new DataObject();
				dragData.SetData("MediaItemData", this); // 直接传递整个控件引用，方便获取所有属性

				// 开始拖放操作，效果为 Copy（复制）
				DragDropEffects effect = DoDragDrop(dragData, DragDropEffects.Copy);

				// 拖放操作完成后，可以根据效果做一些事情
				if(effect == DragDropEffects.Copy)
				{
					// 例如，触发一个事件，通知主界面该项已被成功拖出并放置
					//object p = ItemDraggedOut?.Invoke(this, EventArgs.Empty);
					//ItemDraggedOut?.Invoke(this, new ItemDraggedOutEventArgs() { Success = true, FilePath = FilePath, DragTime = TimeLength });
					Debug.WriteLine($"文件已拖出并放置: {FilePath}");
					OnItemDraggedOut(); // 只在不带参数的情况下触发
				}
			}
		}
		// 关键：将事件参数类设置为 public
		private void OnItemDraggedOut()
		{
			ItemDraggedOut?.Invoke(this, EventArgs.Empty);
			Debug.WriteLine($"文件已拖出: {FilePath}");
		}

		public MediaItemControl(string filePath, MediaType mediaType)		  // 构造函数
		{
			InitializeComponent();
			TimeLength = "未知";
			FilePath = filePath;
			MediaType = mediaType;

			_originalBackColor = this.BackColor;   // 保存原始背景色
			lblFileName.Text = Path.GetFileName(filePath);

			btnPlay.Visible = false;
			btnPlay.BringToFront();
			hadadd.Visible = false;

			_ = SetThumbnailAsync(filePath, mediaType);  //  // 根据媒体类型选择不同 处理方式 

			btnPlay.Click += (s, e) => PlayMedia();     // 播放按钮点击事件
			pictureBoxThumbnail.Click += (s, e) => PlayMedia();  // 订阅点击事件
			lblFileName.Click += (s, e) => PlayMedia();
			this.Click += (s, e) => PlayMedia();
			// 添加点击事件来切换选中状态
			this.Click += MediaItemControl_Click;
			pictureBoxThumbnail.Click += MediaItemControl_Click;
			lblFileName.Click += MediaItemControl_Click;            //mediaRepo = new MediaAssetsRepository(db.dbPath);

			// 添加鼠标事件订阅
			//this.MouseDown += MediaItemControl_MouseDown;  // 添加这行
			//this.MouseMove += MediaItemControl_MouseMove;  // 添加这行
			//this.MouseLeave += MediaItemControl_MouseLeave;  // 添加这行
			//this.MouseEnter += MediaItemControl_MouseEnter;  // 添加这行


		}
		//public MediaItemControl(LibVLC libVLC, string path)  // 构造函数
		//{
		//	this.libVLC = libVLC;
		//	this.path = path;
		//	hadadd.Visible = false;
		//	butadd.Visible = false;
		//}
		private void MediaItemControl_Click(object sender, EventArgs e)     // 点击控件切换选中状态
		{
			IsSelected = !IsSelected;
		}
		// 更新选中状态的外观
		private void UpdateSelectionAppearance()                           // 更新选中状态的外观
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
		private void PlayMedia()                                                 // 触发播放事件
		{
			MediaPlayRequested?.Invoke(this, new MediaPlayEventArgs(FilePath, MediaType));
		}
		// 自定义事件：请求播放媒体
		public event EventHandler<MediaPlayEventArgs> MediaPlayRequested;          // 自定义事件
		private void pictureBoxThumbnail_Click(object sender, EventArgs e)      // 触发播放事件
		{
			MediaPlayRequested?.Invoke(this, new MediaPlayEventArgs(FilePath, MediaType));
		}
		private async Task SetThumbnailAsync(string filePath, MediaType mediaType)  // 根据媒体类型选择不同 处理方式 设置缩略图
		{
			videoInfo.DurationSeconds = "";
			try
			{
				if(mediaType == MediaType.Image && File.Exists(filePath))       // 图片文件
				{
					using Image originalImage = Image.FromFile(filePath);        // 对于图片文件，尝试加载缩略图
					pictureBoxThumbnail.Image = ResizeImage(originalImage, 100, 75); // 调整为适合控件的大小
					videoInfo.Width = originalImage.Width;   //获取图片的宽高
					videoInfo.Height = originalImage.Height;
					LTimeLength.Visible = false;
				}
				else if(mediaType == MediaType.Video && File.Exists(filePath))              // 视频文件
				{
					videoInfo = await ExtractFirstFrameAsync(FilePath);                      // 获取视频信息
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
		public static string GetVideoDuration(string videoPath)                        //获取视频时长  ，使用
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
		public static async Task<VideoInfo> ExtractFirstFrameAsync(string videoPath)    //提取第一帧
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
					//		videoInfo.DurationSeconds = GetVideoDuration(videoPath); // 获取视频时长ffmpeg
					TimeSpan v = GetMediaDuration(videoPath);			//			获取视频时长 NAudio
					DateTime dt = DateTime.Today.Add(v); // 今天的 00:05:03
					videoInfo.DurationSeconds = dt.ToString("mm:ss"); // 输出: "05:03"
					//videoInfo.DurationSeconds = await GetVideoDurationAsync(videoPath);
					videoInfo.FilePath = videoPath;
					videoInfo.Width = videoInfo.Thumbnail.Width;
					videoInfo.Height = videoInfo.Thumbnail.Height;
					videoInfo.snapshotPath = outputImagePath;
					int id = 0;
					////string fileName = Path.GetFileName(videoPath);
					//id = LaserEditing._mediaRepo.GetIdByMediaTypeAndCodec("video", LaserEditing.PubmediaAsset.Codec);
					//MediaAsset target = LaserEditing.MediaAssets.FirstOrDefault(s => s.Codec == outputImagePath);
					//               if(target != null)
					//{
					//	id = target.Id;
					//}
					//target.Duration = videoInfo.DurationSeconds;
					//target.Width = videoInfo.Width;
					//target.Height = videoInfo.Height;
					////将更新的target 存入 LaserEditing.MediaAssets
					//LaserEditing.MediaAssets[id] = target;
					//LaserEditing.MediaAssets.Add(id, target);
					//	id = LaserEditing._mediaRepo.Create(LaserEditing.PubmediaAsset);
					//  LaserEditing.MediaAssets 
					//if(LaserEditing.PubmediaAssetid != 0 && id != 0)   // 如果存在PubmediaAssetid，则更新数据库
					//{
					//	// 更新MediaAsset数据库
					//	LaserEditing.PubmediaAsset.Duration = videoInfo.DurationSeconds;
					//	LaserEditing.PubmediaAsset.Width = videoInfo.Width;
					//	LaserEditing.PubmediaAsset.Height = videoInfo.Height;
					////	//LaserEditing.PubmediaAsset.Codec = videoInfo.snapshotPath;
					////	//LaserEditing.PubmediaAsset.Id = LaserEditing.PubmediaAssetid;
					//	LaserEditing.PubmediaAsset.Id = LaserEditing.PubmediaAssetid;
					//	LaserEditing._mediaRepo.Update(LaserEditing.PubmediaAsset);        // 更新MediaAsset数据库 根据id
					//}
				}
				return videoInfo;
			}
			catch(Exception ex)
			{
				MessageBox.Show("提取第一帧失败：" + ex.Message);
				Debug.WriteLine($"提取第一帧失败：{ex.Message}");
				return videoInfo;
			}
		}
		protected override void Dispose(bool disposing)                              // Dispose
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
		public string GetAudioDuration(string filePath)                              //获取Audio音频时长
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
		//private void MediaItemControl_DragDrop(object sender, DragEventArgs e)       //拖放文件
		//{
		//	// 处理拖放的文件
		//	string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
		//	if(files.Length > 0)
		//	{
		//		ProcessDroppedFile(files[0]);
		//	}
		//}
		//private void MediaItemControl_DragEnter(object sender, DragEventArgs e)
		//{
		//	// 检查拖入的数据是否为文件
		//	if(e.Data.GetDataPresent(DataFormats.FileDrop))
		//	{
		//		string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
		//		if(files.Length > 0 && IsMediaFile(files[0]))
		//		{
		//			e.Effect = DragDropEffects.Copy;
		//			return;
		//		}
		//	}
		//	e.Effect = DragDropEffects.None;
		//}

		public static TimeSpan GetMediaDuration(string filePath)                              // 获取视频文件时长
		{
			string extension = Path.GetExtension(filePath).ToLower();

			if(extension == ".mp4" || extension == ".avi" || extension == ".mov" || extension == ".flv" || extension == ".mkv")
			{
				//return GetVideoDurationo(filePath);
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

			throw new NotSupportedException("不支持的媒体格式");
		}
		private bool IsMediaFile(string filePath)
		{
			string extension = Path.GetExtension(filePath).ToLower();
			string[] supportedExtensions = { ".mp3", ".wav", ".wma", ".mp4", ".avi", ".mov", ".flv", ".mkv" };
			return Array.IndexOf(supportedExtensions, extension) >= 0;
		}
		private void MediaItemControl_MouseMove(object sender, MouseEventArgs e)         //鼠标移动
		{
			hadadd.Visible = true;
			hadadd.BringToFront();
			butadd.Visible = true;
			butadd.BringToFront();
		}
		private void butadd_Click(object sender, EventArgs e)                            //添加文件到编辑区
		{

		}
		private void MediaItemControl_MouseLeave(object sender, EventArgs e)  //鼠标离开
		{
			hadadd.Visible = false;
			butadd.Visible = false;
		}
		private void MediaItemControl_MouseEnter(object sender, EventArgs e)    //鼠标进入
		{
			hadadd.Visible = true;
			hadadd.BringToFront();
			butadd.Visible = true;
			butadd.BringToFront();
		}

		internal class ItemDraggedOutEventArgs
		{
			public ItemDraggedOutEventArgs(string filePath, bool success)
			{
				FilePath = filePath;
				Success = success;
			}

			public bool Success
			{
				get;
				internal set;
			}
			public object FilePath
			{
				get;
				internal set;
			}
			public object DragTime
			{
				get;
				internal set;
			}
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