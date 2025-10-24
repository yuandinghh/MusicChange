#region ------------- 系统加载部分  无需改变的变量 -------------
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using NAudio.Wave;
using Vlc.DotNet.Forms;
using Color = System.Drawing.Color;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;
using Point = System.Drawing.Point;

#endregion
#region  ------------- 全局变量 -------------
namespace MusicChange
{
	public partial class LaserEditing:Form
	{       // Windows API 函数
		[DllImport("user32.dll")]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
		// 快捷键ID
		//private const int HOTKEY_ID = 1;
		//// 修饰符常量
		//private const uint MOD_ALT = 0x0001;
		//private const uint MOD_CONTROL = 0x0002;
		//private const uint MOD_SHIFT = 0x0004;
		// 鼠标状态变量
		private bool isDragging = false;
		private System.Drawing.Point dragStartPoint;
		private const int borderSize = 10;
		private FormWindowState previousWindowState;
		[DllImport("user32.dll")]
		public static extern bool ReleaseCapture();
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		private const int WM_NCLBUTTONDOWN = 0xA1;
		private const int HTCAPTION = 0x2;  //
		private bool splitContainer5mouseDown;
		bool IsOfficialMaterialSwitch = false; //官方素材开关
		bool Ismaterial = true;
		private readonly string importfile;  //imøport a file
		private LibVLC libVLC;
		public MediaPlayer mediaPlayer;
		private const float ZOOM_INCREMENT = 0.05f;
		private const float MIN_ZOOM = 0.2f;
		private const float MAX_ZOOM = 3.0f;
		#endregion
		private string filePath = @"F:\英语学习\MTEY0102.MP4";
		bool IsfirstPlaying = false;  //show first play
		private bool isMuted = false;
		private int previousVolume = 80;
		private float currentZoomFactor = 1.0f;
		private bool isSeeking = false;  // 是否正在拖拽进度条 		private object audioData; 		private bool isvideoView = true;
		private bool isVideoMaximized = false;
		private readonly ToolTipEx toolTipEx = new();  //private bool darkMode = false; private VlcControl vlcControl = new VlcControl();
		private ContextMenuStrip speedContextMenu;      // 在类中添加上下文菜单
		private const int FixedPanelHeight = 300;
		private const int MIN_WIDTH = 600;
		private const int MAX_WIDTH = 900;
		private const int MAX_HWIDTH = 1800;
		private VlcControl vlcControl = new();
		//int count = 0;
		private LibVLC _libVLC1, _libVLC2, _libVLC3;
		private MediaPlayer _player1, _player2, _player3;
		private VideoView _videoView1, _videoView2, _videoView3;        //private LibVLC _libVLC; // LibVLC 实例（视频播放用）
		private AudioPlayer _audioPlayer; // NAudio 播放器实例
		bool isShowOnce = false; // 是否已显示一次cut 			 //private LibVLCSharp.WinForms.VideoView videoView1;
	
		string subDirectory; // 子目录名称



		public LaserEditing()
		{
			InitializeComponent();
			IsfirstPlaying = false;
			AutoScaleMode = AutoScaleMode.Dpi; // 根据系统DPI自动缩放	EnsureVideoViewInitialized();  //动态加载 videoView1

		}
		//private void EnsureVideoViewInitialized( )
		//{
		//	this.videoView1 = new LibVLCSharp.WinForms.VideoView();
		//	((System.ComponentModel.ISupportInitialize)(this.videoView1)).BeginInit();
		//	this.sC4.Panel1.Controls.Add( this.videoView1 );

		//	this.videoView1.BackColor = System.Drawing.Color.FromArgb( ((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))) );
		//	this.videoView1.Dock = System.Windows.Forms.DockStyle.Fill;
		//	this.videoView1.Location = new System.Drawing.Point( 0, 30 );
		//	this.videoView1.MediaPlayer = null;
		//	this.videoView1.Name = "videoView1";
		//	this.videoView1.Size = new System.Drawing.Size( 717, 556 );
		//	this.videoView1.TabIndex = 13;
		//	((System.ComponentModel.ISupportInitialize)(this.videoView1)).EndInit();
		//	try {
		//		// 检查 videoView1 是否已存在
		//		if (videoView1 == null || videoView1.IsDisposed) {
		//			// 如果不存在，则动态创建
		//			videoView1 = new VideoView
		//			{
		//				Name = "videoView1",
		//				Dock = DockStyle.Fill, // 填充父容器
		//				Visible = true
		//			};

		//			// 将 videoView1 添加到父容器（如 Panel）
		//			if (panel1 != null) {
		//				panel1.Controls.Clear(); // 清空旧控件
		//				panel1.Controls.Add( videoView1 );
		//			}

		//			// 重新绑定 MediaPlayer
		//			if (mediaPlayer != null) {
		//				videoView1.MediaPlayer = mediaPlayer;
		//			}

		//			System.Diagnostics.Debug.WriteLine( "videoView1 已成功动态加载" );
		//		}
		//	}
		//	catch (Exception ex) {
		//		MessageBox.Show( $"动态加载 videoView1 时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
		//	}
		//}

		private void LaserEditing_Load(object sender, EventArgs e)
		{
			splitContainer5mouseDown = false;   //splitContainer1.Panel2MinSize = 400;	//buttonx8.BackColor = System.Drawing.Color.Gray;
			Ismaterial = true;  // 默认选择当前素材
			buttonX3_Click(null, null); // 设置当前素材按钮样式	this.ClientSize = new System.Drawing.Size( 1900, 1080 );
			OfficialMaterialSwitch(); // 初始化官方素材开关状态
			; // 获取屏幕工作区宽度
			temp.Text = "屏幕工作区宽度: " + Screen.PrimaryScreen.WorkingArea.Width.ToString();
			temp1.Text = "屏幕工作区高度: " + Screen.PrimaryScreen.WorkingArea.Height.ToString();
			this.Size = new System.Drawing.Size(1800, 1000); // 设置主窗口初始大小
			InitializeLibVLC(); // 初始化 LibVLC
			InitializeUIControls();             // 确保窗体能接收按键事件
			this.KeyPreview = true;             //darkMode = false;
			InitializeSpeedMenu();  // 初始化播放速度菜单
			ConfigureToolTip(toolTipEx);
			splitContainer1.SplitterWidth = 6;
			AdjustSplitContainer();  // 调整 SplitContainer 的宽度
			initmportfile(); // 初始化导入文件
			flowLayoutPanelMedia.HorizontalScroll.Visible = false;
			flowLayoutPanelMedia.VerticalScroll.Visible = true;
			Displayvideo(); // 显示视频
			_audioPlayer = new AudioPlayer();  // 初始化 NAudio 播放器 	//private AudioPlayer _audioPlayer;播放用）
			_audioPlayer.PlaybackCompleted += (s, e) =>     // 订阅音频播放完成事件
			{
				this.Invoke(new Action(() =>
				{
					audioTimer.Stop();
					UpdateAudioControls();
					ResetAudioProgress();
					HideAudioControls();
				}));
			};
			//pictureBox1.			sC4.Panel1.AutoScroll = false;
			ConfigureFlowLayoutPanelScrolling();
			ConfigureFlowLayoutPanel();

		}
		#region ------- ToolTip 鼠标进入悬停显示 -------

		private void ConfigureToolTip(ToolTipEx toolTip1)
		{           // 设置 ToolTip 属性
			toolTip1.AutoPopDelay = 100000; // 提示框显示 5 秒后消失
			toolTip1.InitialDelay = 500; // 鼠标悬停 1 秒后显示提示框
			toolTip1.ReshowDelay = 1000;   // 鼠标移开后再次悬停的延迟时间
			toolTip1.ShowAlways = true;   // 即使控件未激活也显示提示框
			toolTip1.IsBalloon = true;    // 使用气泡样式
			toolTip1.ToolTipIcon = ToolTipIcon.Info;      // 提示框图标  info  Warning
			toolTip1.ToolTipTitle = "提示"; // 提示框标题
			toolTip1.BackColor = Color.FromArgb(204, 200, 0); // 设置背景颜色
															  //toolTip1.TitleFont = new Font(	familyName: "微软雅黑",  emSize: 16,     // 字体大小
			toolTip1.TitleFont = new Font("微软雅黑", 16f); //	style: FontStyle.Bold | FontStyle.Italic ); // 字体样式（可组合）
			toolTip1.ForeColor = Color.Aqua; // 设置前景颜色
			toolTip1.TitleColor = Color.FromArgb(32, 200, 0); // 设置标题颜色 
			toolTip1.CornerRadius = 10; // 设置圆角半径
			toolTip1.ShadowSize = 10; // 设置阴影大小
			toolTip1.ContentFont = new Font("Segoe UI", 18f, FontStyle.Regular);   // 设置内容字体和颜色
			toolTip1.ContentColor = Color.WhiteSmoke;
			// 设置内容填充          toolTip1.ContentFill = new SolidBrush(Color.FromArgb(50, 50, 50));
			//toolTip1.Padding = new Padding( 10 ); // 上下左右各增加10像素内边距
			//toolTip1.Padding = new Padding

			toolTip1.BackColor1 = Color.FromArgb(50, 50, 80);
			toolTip1.BackColor2 = Color.FromArgb(30, 30, 50);
			toolTip1.BorderColor = Color.SteelBlue;

			toolTip1.SetToolTip(playPauseButton, "视频播放开始和停止");
			toolTip1.SetToolTip(stopButton, "停止播放视频");
			toolTip1.SetToolTip(buttonX2, "crf（Constant Rate Factor，恒定码率因子）\r\n•\t作用：控制视频压缩的画质和文件大小。\r\n•\t取值范围：0~51，常用范围为 18~28。\r\n•\t数值越小，画质越高，文件越大。\r\n•\t数值越大，画质越低，文件越小。\r\n•\t一般推荐：高质量用 18~22，普通用 23~28。");
			toolTip1.SetToolTip(color, "选择播放速度");
			toolTip1.SetToolTip(temp2, "preset（预设编码速度）\r\n•\t作用：控制编码速度与压缩效率的平衡。\r\n•\t可选值（从快到慢）：\r\n•\tultrafast, superfast, veryfast, faster, fast, medium, slow, slower, veryslow\r\n•\t说明：\r\n•\t越快（如 ultrafast），编码速度快，但文件大、画质略低。\r\n•\t越慢（如 veryslow），编码速度慢，但文件更小、画质更好。\r\n•\t默认值是 medium，一般推荐用 fast、medium 或 slow。");
			toolTip1.SetToolTip(zoomInButton, "放大视频");
			toolTip1.SetToolTip(zoomOutButton, "缩小视频画面");
			toolTip1.SetToolTip(volumeControlPanel, "音量控制");
			toolTip1.SetToolTip(volumeTrackBar, "音量控制");
			toolTip1.SetToolTip(muteButton, "静音");
			toolTip1.SetToolTip(fitToWindowButton, "适应 窗口大小");
			toolTip1.SetToolTip(vieweMax, "使用外部播放器！");
			toolTip1.SetToolTip(speed, "设置视频播放速度");
			toolTip1.SetToolTip(color, "设置视频图像属性\n亮度、对比度、饱和度和色调\n  ");

			// 为控件设置提示
			toolTip1.SetToolTip(vieweMax, "保存文档",
				"将当前文档保存到磁盘\n快捷键: Ctrl+S",
				Properties.Resources.loading);

			// 为按钮1设置提示（使用全局默认字体和颜色）
			toolTip1.SetToolTip(muteButton, "保存文档",
				"将当前文档保存到磁盘\n快捷键: Ctrl+S",
				Properties.Resources.右转1501);

			// 为按钮2设置自定义字体和颜色
			toolTip1.SetToolTip(vieweMax, "删除项目",
				"永久删除选定项目\n此操作无法撤销!",
				Properties.Resources.QRcode,
				new Font("Arial", 12, FontStyle.Bold),  // 自定义标题字体
				Color.Red,                             // 自定义标题颜色
				new Font("Consolas", 10),              // 自定义内容字体
				Color.Yellow);                         // 自定义内容颜色

			// 为文本框设置提示（无图标）
			toolTip1.SetToolTip(vieweMax, "视频在独立视频程序播放");

			// 普通文本换行控制
			toolTip1.SetToolTip(buttonX2, "这是一个较长的提示文本，\n通过换行来控制显示宽度。");

			// 或者使用 HTML 格式（需要设置 OwnerDraw 为 true）
			toolTip1.OwnerDraw = true;
			toolTip1.Draw += (sender, e) =>
			{
				e.DrawBackground();
				e.DrawBorder();
				e.Graphics.DrawString(e.ToolTipText, e.Font, System.Drawing.Brushes.Black,
									 new RectangleF(e.Bounds.X, e.Bounds.Y, 200, e.Bounds.Height));
			};
			toolTip1.SetToolTip(buttonX2, "提示", "这是一个可以自动换行的长文本提示，当达到指定宽度时会自动换行显示...");

		}

		#endregion
		#region   ------------初始化 LibVLC 核心 播放视频文件 播放 继续 停止等	 -----------------	

		private void InitializeLibVLC()
		{
			try
			{
				Core.Initialize();                  // 创建 LibVLC 实例
				libVLC = new LibVLC("--avcodec-hw=dxva2");  //启用硬件加速：  "--avcodec-hw=dxva2"
				if(libVLC == null)
				{
					MessageBox.Show("LibVLC 未正确初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				// 创建 MediaPlayer 实例
				mediaPlayer = new MediaPlayer(libVLC);
				if(mediaPlayer == null)
				{
					MessageBox.Show("播放器未正确初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				if(videoView1 != null)
				{
					videoView1.MediaPlayer = mediaPlayer;
				}

				// 创建三个独立的 VLC 实例
				_libVLC1 = new LibVLC("--no-xlib"); // Linux 参数，Windows 可以省略
				_libVLC2 = new LibVLC("--no-xlib");
				_libVLC3 = new LibVLC("--no-xlib");

				// 创建媒体播放器
				_player1 = new MediaPlayer(_libVLC1);
				_player2 = new MediaPlayer(_libVLC2);
				_player3 = new MediaPlayer(_libVLC3);


				mediaPlayer.TimeChanged += OnMediaPlayerTimeChanged;
				mediaPlayer.LengthChanged += OnMediaPlayerLengthChanged;
				mediaPlayer.Playing += OnMediaPlayerPlaying;
				mediaPlayer.Paused += OnMediaPlayerPaused;
				mediaPlayer.Stopped += OnMediaPlayerStopped;
				mediaPlayer.EndReached += OnMediaPlayerEndReached;
				/*
				事件说明
				1. mediaPlayer.TimeChanged += OnMediaPlayerTimeChanged;
				•	作用：当媒体播放位置（时间）发生变化时触发
				•	用途：更新进度条、时间标签等UI元素
				•	触发频率：播放过程中频繁触发（通常每秒多次）
				2. mediaPlayer.LengthChanged += OnMediaPlayerLengthChanged;
				•	作用：当媒体总长度确定或改变时触发
				•	用途：更新总时间显示、设置进度条最大值
				•	触发时机：媒体加载完成时或流媒体长度变化时
				3. mediaPlayer.Playing += OnMediaPlayerPlaying;
				•	作用：当媒体开始播放时触发
				•	用途：更新播放按钮状态、初始化播放相关UI
				•	触发时机：调用 Play() 方法后
				4. mediaPlayer.Paused += OnMediaPlayerPaused;
				•	作用：当媒体暂停时触发
				•	用途：更新播放按钮图标、暂停相关动画等
				•	触发时机：调用 Pause() 方法后
				5. mediaPlayer.Stopped += OnMediaPlayerStopped;
				•	作用：当媒体停止时触发
				•	用途：重置进度条、更新按钮状态
				•	触发时机：调用 Stop() 方法后
				6. mediaPlayer.EndReached += OnMediaPlayerEndReached;
				•	作用：当媒体播放结束时触发
				•	用途：自动播放下一个文件、重置播放状态
				•	触发时机：媒体播放到末尾时
				1.	线程安全：这些事件通常在后台线程触发，需要使用 InvokeRequired 和 Invoke 确保UI更新在主线程进行
2.	性能考虑：TimeChanged 事件触发非常频繁，通常使用定时器而不是直接处理该事件来更新UI
3.	资源清理：在窗体关闭时应该取消订阅这些事件以避免内存泄漏：
				*/

				// 设置第一个视频视图
				// 第一个视频视图
				_videoView1 = new VideoView
				{
					MediaPlayer = _player1,
					Dock = DockStyle.Fill
				};
				panel1.Controls.Add(_videoView1);

				// 第二个视频视图
				_videoView2 = new VideoView
				{
					MediaPlayer = _player2,
					Dock = DockStyle.Fill
				};
				panel2.Controls.Add(_videoView2);

				// 第三个视频视图
				_videoView3 = new VideoView
				{
					MediaPlayer = _player3,
					Dock = DockStyle.Fill
				};
				panel3.Controls.Add(_videoView3);


			}
			catch
			{
				// 静默处理初始化异常  				CleanupResources();
			}
		}
		// 修复播放视频方法
		public void PlayVideo()
		{
			try
			{
				if(libVLC == null || mediaPlayer == null)
				{
					MessageBox.Show("播放器未正确初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if(string.IsNullOrEmpty(filePath))
				{
					MessageBox.Show("文件路径不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				mediaPlayer.Stop();
				using var media = new Media(libVLC, filePath, FromType.FromPath);
				if(media == null)
				{
					throw new InvalidOperationException("无法创建媒体对象");
				}
				progressTimer.Start();
				mediaPlayer.Play(media);
				mediaPlayer.Mute = false;
				SetVolume(volumeTrackBar.Value);
			}
			catch(Exception ex)
			{
				MessageBox.Show($"播放视频失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		// 计算 RMS（均方根值）- 优化版本
		private double CalculateRMS(short[] samples)
		{
			if(samples == null || samples.Length == 0)
				return 0;

			try
			{
				// 优化计算，避免使用 LINQ 以提高性能
				long sum = 0;
				foreach(short sample in samples)
				{
					sum += (long)sample * sample;
				}

				double mean = (double)sum / samples.Length;
				return Math.Sqrt(mean) / 100; // 调整比例以适应进度条范围
			}
			catch
			{
				return 0;
			}
		}

		private void PlayButton_Click(object sender, EventArgs e)
		{
			var media = new Media(libVLC, "your_video_file.mp4", FromType.FromPath);
			mediaPlayer.Play(media);
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			try
			{
				// 停止音频播放
				_audioPlayer?.Stop();

				// 停止视频播放
				mediaPlayer?.Stop();
				_player1?.Stop();
				_player2?.Stop();
				_player3?.Stop();

				// 清理定时器
				audioTimer?.Stop();
				audioTimer?.Dispose();

				// 清理 NAudio 资源
				_audioPlayer?.Dispose();

				// 清理 LibVLC 资源
				mediaPlayer?.Dispose();
				libVLC?.Dispose();
				_player1?.Dispose();
				_player2?.Dispose();
				_player3?.Dispose();
				_libVLC1?.Dispose();
				_libVLC2?.Dispose();
				_libVLC3?.Dispose();
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"清理资源时出错: {ex.Message}");
			}

			base.OnFormClosing(e);
		}
		private void InitializeUIControls()
		{

			progressTimer.Start();
			volumeTrackBar.Value = 80;
			SetVolume(80);
			volumeTrackBar.Value = 80;
		}
		private void VolumeTrackBar_Scroll(object sender, EventArgs e)
		{
			int volume = volumeTrackBar.Value;
			volumenum.Text = volume.ToString();
			SetVolume(volume);
			// 如果之前是静音状态，则取消静音
			if(isMuted)
			{
				isMuted = false;
				mediaPlayer.Mute = isMuted;
				//_muteButton.Text = "Mute";
			}
		}
		// 静音按钮点击事件处理程序
		private void MuteButton_Click1(object sender, EventArgs e)
		{
			if(mediaPlayer == null)
				return;
			if(!isMuted)
			{
				// 保存当前音量并静音
				previousVolume = mediaPlayer.Volume;
				mediaPlayer.Volume = 0;
				isMuted = true;
				mediaPlayer.Mute = isMuted;
				muteButton.SymbolColor = Color.Salmon;
				volumeTrackBar.Value = 0;
			}
			else
			{
				// 恢复之前音量
				mediaPlayer.Volume = previousVolume;
				isMuted = false;
				//_muteButton.Text = "Mute";
				muteButton.SymbolColor = Color.Gray;
				volumeTrackBar.Value = previousVolume;
			}
		}
		// 修改现有的 SetVolume 方法以更新音量显示
		private void VolumeDownButton_Click(object sender, EventArgs e)
		{
			int currentVolume = mediaPlayer?.Volume ?? 0;
			int newVolume = Math.Max(0, currentVolume - 1);
			SetVolume(newVolume);
			// 如果之前是静音状态，则取消静音
			if(isMuted)
			{
				isMuted = false;
				mediaPlayer.Mute = isMuted;
				//_muteButton.Text = "Mute";
			}
		}
		// 放大按钮点击事件
		private void ZoomInButton_Click(object sender, EventArgs e)
		{
			currentZoomFactor = mediaPlayer.Scale;
			if(currentZoomFactor == 0)
			{
				currentZoomFactor = 0.4f;
				ApplyZoom();
				return;
			}
			currentZoomFactor += ZOOM_INCREMENT;
			if(currentZoomFactor > MAX_ZOOM)
				currentZoomFactor = MAX_ZOOM;
			ApplyZoom();
		}
		// 缩小按钮点击事件
		private void ZoomOutButton_Click(object sender, EventArgs e)
		{
			currentZoomFactor = mediaPlayer.Scale;
			if(currentZoomFactor <= 0)
				return;
			currentZoomFactor -= ZOOM_INCREMENT;
			if(currentZoomFactor < MIN_ZOOM)
				currentZoomFactor = MIN_ZOOM;
			ApplyZoom();
		}
		// 适应窗口按钮点击事件
		private void FitToWindowButton_Click(object sender, EventArgs e)
		{
			currentZoomFactor = 0.0f;
			ApplyZoom();
		}
		/// <summary>
		/// 应用当前缩放因子到视频显示
		/// </summary>
		private void ApplyZoom()
		{
			if(mediaPlayer == null || videoView1 == null)
				return;
			try
			{
				// 这里我们通过调整视频视图的大小来实现缩放效果
				// 实际的 VLC 缩放需要通过其 API 设置
				UpdateZoomLabel();

				// 如果需要通过 VLC API 实现真正的缩放，可以使用以下方法：
				mediaPlayer.Scale = currentZoomFactor;
			}
			catch(Exception ex)
			{
				MessageBox.Show($"应用缩放失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			temp2.Text = $"缩放: {currentZoomFactor:P0}"; // 显示为百分比
		}
		/// <summary>
		/// 更新缩放标签显示
		/// </summary>
		private void UpdateZoomLabel()
		{
			// 如果您有一个显示缩放级别的标签，可以在这里更新它
			// 例如：zoomLabel.Text = $"{_currentZoomFactor:P0}"; // 显示为百分比
		}
		// 在播放器事件处理中添加声道更新
		private void OnMediaPlayerPlaying(object sender, EventArgs e)
		{
			if(InvokeRequired)
			{
				Invoke(new Action(() =>
	  {
		  // 更新按钮状态
		  // 可以在这里初始化声道显示 						UpdateChannelDisplay(50, 50); // 默认显示
	  }));
			}
			else
			{
				//UpdateChannelDisplay(50, 50); // 默认显示
			}
		}
		// 定时器更新进度
		private void ProgressTimer_Tick(object sender, EventArgs e)
		{
			if(mediaPlayer != null && !isSeeking)
			{
				UpdateProgress();

				// 更新声道显示（实际应用中应从音频数据获取）
				//UpdateChannelDisplayFromAudio();
			}
		}
		/// <summary>
		/// 根据音量级别获取颜色
		/// </summary>
		/// <param name="level">音量级别 (0-100)</param>
		/// <returns>对应的颜色</returns>
		private Color GetChannelColor(int level)
		{
			if(level < 30)
				return Color.LimeGreen;
			else if(level < 70)
				return Color.Yellow;
			else
				return Color.Red;
		}
		public void SetVolume(int volume)
		{
			try
			{
				if(mediaPlayer != null)
				{
					int clampedVolume = Math.Max(0, Math.Min(100, volume));
					mediaPlayer.Volume = clampedVolume;

					// 更新音量进度条（避免触发Scroll事件）
					if(volumeTrackBar.Value != clampedVolume)
					{
						volumeTrackBar.Value = clampedVolume;
					}

					// 同时更新声道显示
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show($"设置音量失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		// 静音按钮点击事件处理程序
		private void MuteButton_Click(object sender, EventArgs e)
		{
			if(mediaPlayer == null)
				return;
			try
			{
				if(!isMuted)
				{                 // 保存当前音量并静音
					previousVolume = mediaPlayer.Volume;
					//mediaPlayer.Volume = 1;
					isMuted = true;
					mediaPlayer.Mute = isMuted;
					muteButton.SymbolColor = Color.Gray;

					//volumeTrackBar.Value = 1; 	// 更新声道显示为静音状态  	UpdateChannelDisplay(0, 0);
				}
				else
				{      // 恢复之前音量
					mediaPlayer.Volume = previousVolume;
					isMuted = false;
					mediaPlayer.Mute = isMuted;
					muteButton.SymbolColor = Color.Salmon;
					volumeTrackBar.Value = previousVolume;
					// 更新声道显示为恢复的音量 	UpdateChannelDisplay(previousVolume, previousVolume);
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show($"切换静音状态失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		// 播放器事件处理
		private void OnMediaPlayerTimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
		{
			// 进度更新在定时器中处理，避免频繁UI更新
		}
		private void OnMediaPlayerLengthChanged(object sender, MediaPlayerLengthChangedEventArgs e)
		{
			if(InvokeRequired)
			{
				Invoke(new Action(() => UpdateTotalTime(e.Length)));
			}
			else
			{
				UpdateTotalTime(e.Length);
			}
		}
		private void OnMediaPlayerPlaying1(object sender, EventArgs e)
		{
			if(InvokeRequired)
			{
				Invoke(new Action(() =>
	  {
		  // 更新按钮状态
	  }));
			}
		}
		private void OnMediaPlayerPaused(object sender, EventArgs e)
		{
			if(InvokeRequired)
			{
				Invoke(new Action(() =>
	  {
		  // 更新按钮状态
	  }));
			}
		}
		private void OnMediaPlayerStopped(object sender, EventArgs e)
		{
			if(InvokeRequired)
			{
				Invoke(new Action(() =>
	  {
		  ResetProgress();
	  }));
			}
			else
			{
				ResetProgress();
			}
		}
		private void OnMediaPlayerEndReached(object sender, EventArgs e)
		{
			if(InvokeRequired)
			{
				Invoke(new Action(() =>
	  {
		  ResetProgress();
	  }));
			}
			else
			{
				ResetProgress();
			}
		}
		//	进度条事件处理
		private void progressBar_MouseDown(object sender, MouseEventArgs e)
		{
			isSeeking = true;
		}
		private void progressBar_MouseUp(object sender, MouseEventArgs e)
		{
			if(isSeeking && mediaPlayer != null)
			{
				// 计算目标时间
				long totalTime = mediaPlayer.Length;
				if(totalTime > 0)
				{
					long targetTime = (long)(progressBar.Value * totalTime / 1000.0);
					mediaPlayer.Time = targetTime;
				}
			}
			isSeeking = false;
		}
		private void progressBar_Scroll(object sender, EventArgs e)
		{
			// 拖拽时实时更新时间显示
			if(mediaPlayer != null && mediaPlayer.Length > 0)
			{
				long totalTime = mediaPlayer.Length;
				long currentTime = (long)(progressBar.Value * totalTime / 1000.0);
				currentTimeLabel.Text = FormatTime(currentTime);
			}
		}
		private Control[] GetAllToolTipControls()
		{
			// 获取所有设置了工具提示的控件
			// 注意：实际实现中需要更健壮的方法
			return new Control[0]; // 简化实现
		}
		//	定时器更新进度
		private void UpdateProgress()
		{
			if(mediaPlayer == null)
				return;

			long currentTime = mediaPlayer.Time;
			long totalTime = mediaPlayer.Length;

			if(totalTime > 0)
			{
				// 更新进度条
				int progressValue = (int)(currentTime * 1000 / totalTime);
				if(progressValue >= 0 && progressValue <= 1000)
				{
					progressBar.Value = progressValue;
				}

				// 更新时间标签
				currentTimeLabel.Text = FormatTime(currentTime);
			}
		}
		private void UpdateTotalTime(long totalTime)
		{
			totalTimeLabel.Text = FormatTime(totalTime);
			if(totalTime > 0)
			{
				progressBar.Maximum = 1000;
			}
		}
		private void ResetProgress()  //结束播放时重置进度
		{
			progressBar.Value = 0;
			currentTimeLabel.Text = "00:00:00";
		}
		// 格式化时间显示 (毫秒转为 HH:MM:SS)
		private string FormatTime(long milliseconds)
		{
			if(milliseconds <= 0)
				return "00:00:00";

			TimeSpan time = TimeSpan.FromMilliseconds(milliseconds);
			return $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}:{time.Milliseconds}";
		}
		// 按钮事件处理
		private void playPauseButton_Click(object sender, EventArgs e)
		{  //			var tt = _mediaPlayer;
		   //filePath = @"F:\英语学习\MTEY0102.MP4";
			float zz;
			if(!IsfirstPlaying)
			{
				IsfirstPlaying = true;
				PlayVideo();
				playPauseButton.Image = Properties.Resources.pause;
				zz = mediaPlayer.Scale;
				temp1.Text = zz.ToString();
				temp2.Text = $"缩放: {mediaPlayer.Scale:P0}"; // 显示为百分比
				return;
			}
			else
			{
				if(mediaPlayer == null)
				{
					return;
				}
				if(mediaPlayer.State == VLCState.Playing)
				{
					progressTimer.Stop();
					mediaPlayer.Pause();               //添加图片
					playPauseButton.Image = Properties.Resources.start;
					temp2.Text = $"缩放: {mediaPlayer.Scale:P0}";
				}
				else
				{
					progressTimer.Start();
					mediaPlayer.Play();
					playPauseButton.Image = Properties.Resources.pause;                 //((Button)sender).Text = "暂停";
																						//videoView1.ba
				}
			}

			temp1.Text = mediaPlayer.Scale.ToString();
			temp2.Text = $"缩放: {mediaPlayer.Scale:P0}"; // 显示为百分比
		}
		private void StopButton_Click(object sender, EventArgs e)
		{
			//_mediaPlayer?.Stop();
			try
			{
				if(mediaPlayer != null)
				{
					playPauseButton.Image = Properties.Resources.start;
					progressTimer.Stop();
					mediaPlayer.Stop();
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show($"停止播放失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		#endregion
		#region ----------- 鼠标拖动窗口和改变大小问题 快捷键  还没解决-------------------
		private void panelEx4_MouseDown(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
			}

		}
		private void LaserEditing_MouseDown(object sender, MouseEventArgs e)
		{
			splitContainer5mouseDown = true;
			if(e.Button == MouseButtons.Left)
			{
				// 释放鼠标捕获并发送消息以模拟拖动窗口
				ReleaseCapture();
				SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
			}
			//// 只有在正常状态下才能拖动和调整大小
			//if(this.WindowState == FormWindowState.Normal)
			//{
			//	// 鼠标左键按下时开始拖动（排除边缘区域，避免与缩放冲突）
			if(e.Button == MouseButtons.Left)
			{  //&& !IsInResizeArea( e.Location )
				isDragging = true;
				dragStartPoint = new Point(e.X, e.Y);
				this.Cursor = Cursors.Hand;
			}
		}
		//if(e.Button == MouseButtons.Left)
		//{
		//	ReleaseCapture();
		//	SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0); // 欺骗系统认为在标题栏按下
		//}
		private void LaserEditing_MouseMove(object sender, MouseEventArgs e)
		{

			// 处理拖动
			if(isDragging)
			{
				this.Location = new Point(this.Location.X + e.X - dragStartPoint.X, this.Location.Y + e.Y - dragStartPoint.Y);
				//  Point newLocation = this.Location;
				//newLocation.X += e.X - dragStartPoint.X;
				//newLocation.Y += e.Y - dragStartPoint.Y;
				//this.Location = newLocation;
			}
			//// 处理边框鼠标样式
			//else if (this.WindowState == FormWindowState.Normal) {
			//	SetCursorBasedOnPosition( e.Location );
			//}
		}

		private void LaserEditing_MouseUp(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
			{
				isDragging = false;
				//this.Cursor = Cursors.Default;
			}
		}

		private void LaserEditing_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			// 双击标题栏切换最大化/正常状态
			if(e.Button == MouseButtons.Left &&
				e.Y < 50) // 假设标题栏高度为50
			{
				ToggleMaximize();
			}
		}
		private void ToggleMaximize()
		{
			if(this.WindowState == FormWindowState.Normal)
			{
				previousWindowState = FormWindowState.Normal;
				this.WindowState = FormWindowState.Maximized;
			}
			else
			{
				this.WindowState = previousWindowState;
			}
		}
		#region  ------------- protected override void WndProc(ref Message m)
		//protected override void WndProc(ref Message m)
		//{
		//	const int WM_NCHITTEST = 0x0084;
		//	const int HTCLIENT = 0x1;
		//	const int HTLEFT = 0xA;
		//	const int HTRIGHT = 0xB;
		//	const int HTTOP = 0xC;
		//	const int HTTOPLEFT = 0xD;
		//	const int HTTOPRIGHT = 0xE;
		//	const int HTBOTTOM = 0xF;
		//	const int HTBOTTOMLEFT = 0x10;
		//	const int HTBOTTOMRIGHT = 0x11;

		//	const int RESIZE_HANDLE_SIZE = 10; // 可调整大小的边缘宽度

		//	if (m.Msg == WM_NCHITTEST && this.WindowState == FormWindowState.Normal) {
		//		Point cursorPos = this.PointToClient( Cursor.Position );
		//		int hitTestResult = HTCLIENT;

		//		// 判断鼠标是否在窗体边缘
		//		if (cursorPos.X <= RESIZE_HANDLE_SIZE) {
		//			if (cursorPos.Y <= RESIZE_HANDLE_SIZE)
		//				hitTestResult = HTTOPLEFT; // 左上角
		//			else if (cursorPos.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
		//				hitTestResult = HTBOTTOMLEFT; // 左下角
		//			else
		//				hitTestResult = HTLEFT; // 左边
		//		}
		//		else if (cursorPos.X >= this.ClientSize.Width - RESIZE_HANDLE_SIZE) {
		//			if (cursorPos.Y <= RESIZE_HANDLE_SIZE)
		//				hitTestResult = HTTOPRIGHT; // 右上角
		//			else if (cursorPos.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
		//				hitTestResult = HTBOTTOMRIGHT; // 右下角
		//			else
		//				hitTestResult = HTRIGHT; // 右边
		//		}
		//		else if (cursorPos.Y <= RESIZE_HANDLE_SIZE)
		//			hitTestResult = HTTOP; // 上边
		//		else if (cursorPos.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
		//			hitTestResult = HTBOTTOM; // 下边

		//		m.Result = (IntPtr)hitTestResult;
		//		return;
		//	}

		//	base.WndProc( ref m );
		//}
		#endregion
		// 处理快捷键
		private void LaserEditing_KeyDown(object sender, KeyEventArgs e)
		{
			Keys key = e.KeyCode;
			//  if (e.Control != true)//如果没按Ctrl键      return;
			switch(key)   //功能键键 选择 
			{
				case Keys.F1:

					break;
				case Keys.F2:

					break;
				case Keys.F3:

					break;
				case Keys.F4:             //减少 图片显示时间5秒
				case Keys.F6:             //停音乐
					break;
				case Keys.F5:             //增加图片显示时间
				case Keys.F10:
					break;
				case Keys.F11:             //关闭计算机
					break;
				case Keys.F12:
					//application.Exit();
					this.Close();
					break;

			}

			// 添加播放速率控制快捷键
			if(e.Control)
			{
				switch(e.KeyCode)
				{
					case Keys.Up:  // Ctrl + Up - 增加播放速率
						IncreasePlaybackRate();
						e.Handled = true;
						break;
					case Keys.Down:  // Ctrl + Down - 降低播放速率
						DecreasePlaybackRate();
						e.Handled = true;
						break;
					case Keys.D0:  // Ctrl + 0 - 重置播放速率
						SetPlaybackRate(1.0f);
						e.Handled = true;
						break;
				}
			}

			//         if(e.KeyCode == Keys.F4 && isVideoMaximized)
			//{
			//	MaximizeVideoView();
			//}
			//if(e.KeyCode == Keys.Escape && isVideoMaximized)
			//{
			//	RestoreVideoView();
			//}
		}
		#endregion
		#region ------------- 没用的程序  -------------
		private void splitContainer5_MouseDown(object sender, MouseEventArgs e)
		{
			splitContainer5mouseDown = true;
		}
		private void splitContainer3_Panel1_Paint(object sender, PaintEventArgs e)
		{

		}
		private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
		{

		}
		private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}
		private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
		{

		}
		//private void SplitContainer5_SplitterMoving(object sender, SplitterCancelEventArgs e)
		//{
		//	// 仅处理水平分隔条（Orientation=Vertical）
		//	if (splitContainer1.Orientation == Orientation.Vertical) {
		//		// 计算 Panel2 当前宽度：总宽度 - 分隔条位置
		//		int panel2Width = splitContainer1.Width - e.SplitX;
		//		// 若 Panel2 宽度 < 400px，则取消移动
		//		if (panel2Width < 400) {
		//			e.Cancel = true;  // 阻止移动
		//			splitContainer1.SplitterDistance = splitContainer1.Width - 400; // 重置位置
		//		}
		//	}
		//}
		#endregion
		#region  -------------  窗口 close, maximize, minimize  -----------------
		private void button8_Click(object sender, EventArgs e)  //退出当前窗口
		{
			//Application.Exit();
			this.Close();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if(WindowState == FormWindowState.Maximized)
			{
				WindowState = FormWindowState.Normal; // 恢复到正常状态
			}
			else
			{
				WindowState = FormWindowState.Maximized; // 最大化窗口
			}

		}

		private void button42_Click(object sender, EventArgs e)
		{           //minimize
			if(WindowState == FormWindowState.Minimized)
			{
				WindowState = FormWindowState.Normal; // 恢复到正常状态
			}
			else
			{
				WindowState = FormWindowState.Minimized; // 最小化窗口
			}
		}

		private void buttonx6_Click(object sender, EventArgs e)
		{
			//只显示一次Cut 窗口
			//if(isShowOnce)
			//{
			//	return;
			//}
			//isShowOnce = true;

			//Cut cut = new();
			//cut.Show();

		}
		private bool IsInResizeArea(System.Drawing.Point point)
		{
			return point.X <= borderSize ||  // 左边界
				   point.X >= this.ClientSize.Width - borderSize ||  // 右边界
				   point.Y <= borderSize ||  // 上边界
				   point.Y >= this.ClientSize.Height - borderSize;   // 下边界
		}
		// 是一个根据鼠标位置设置光标类型的函数。在视频编辑软件中，这通常用于根据鼠标在时间轴上的位置来改变光标样式，以提供更好的用户体验。
		private void SetCursorBasedOnPosition(System.Drawing.Point point)
		{
		}

		private void splitContainer6_MouseMove(object sender, MouseEventArgs e)
		{
			//SetCursorBasedOnPosition( e.Location );
		}
		private void panelEx1_MouseMove(object sender, MouseEventArgs e)
		{
			SetCursorBasedOnPosition(e.Location);
		}

		/// <summary>
		/// QRCode 二维码 
		/// </summary>
		private void buttonX12_Click(object sender, EventArgs e)
		{           // 显示二维码生成界面
			QRCode form = new();
			form.Show();            // 隐藏当前界面			this.Hide();
		}
		#endregion
		#region ----------  素材  Material  -------------------
		private void buttonX3_Click(object sender, EventArgs e)  //当前选择素材
		{
			Ismaterial = true;
			AllGray();
			//this.buttonX3.BackColor = System.Drawing.Color.Black;
			//this.buttonX3.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat;
			this.material.SymbolColor = System.Drawing.Color.GreenYellow;
			//this.buttonX3.ThemeAware = true;  //这个属性很可能用于让按钮能够感知并自动适应应用程序的主题变化。当主题（如浅色 / 深色模式）发生改变时，设置为 ThemeAware=true 的控件会自动更新其外观（如颜色、样式等）以匹配当前主题，而无需手动编写额外的主题切换代码。
			inoutmatiral.Visible = true;
			personalcollection.Visible = true;
			personaMcollection.Visible = true;
			this.inoutmatiral.BackColor = System.Drawing.Color.GreenYellow;


		}
		private void buttonX2_Click(object sender, EventArgs e) //video 音频
		{
			Ismaterial = false;
			AllGray();
			this.audio.SymbolColor = System.Drawing.Color.YellowGreen;
			inoutmatiral.Visible = false;
			personalcollection.Visible = false;
			personaMcollection.Visible = false;
		}
		private void AllGray()
		{
			this.material.SymbolColor = System.Drawing.Color.Gray;
			this.audio.SymbolColor = System.Drawing.Color.Gray;
		}


		//官方素材 开关
		private void OfficialMaterialSwitch()
		{
			if(IsOfficialMaterialSwitch)
			{
				hot.Visible = true;
			}
			else
			{
				hot.Visible = false;
			}
		}
		private void buttonx5_Click(object sender, EventArgs e) //官方素材
		{
			IsOfficialMaterialSwitch = true;
			OfficialMaterialSwitch();

		}


		private void button1_Click(object sender, EventArgs e) //素材热门
		{

		}

		//导入素材  选项
		private void buttonx8_Click(object sender, EventArgs e)
		{
			IsOfficialMaterialSwitch = false;
			OfficialMaterialSwitch();
		}
		//个人收藏
		private void buttonx4_Click(object sender, EventArgs e)
		{
			IsOfficialMaterialSwitch = false;
			OfficialMaterialSwitch();
		}

		private void panel2_Paint(object sender, PaintEventArgs e)
		{

		}


		private void sC3_SplitterMoved(object sender, SplitterEventArgs e)
		{

		}

		#endregion
		#region ------------  导入：视频、音频、图片  -------------
		/// <summary>
		/// 导入：视频、音频、图片
		/// </summary>
		private void videoView1_Click(object sender, EventArgs e)  //视频 播放 
		{
			bool ff = videoView1.Visible;
			//	videoView1.VideoScale());
		}
		private void sC4_SplitterMoved(object sender, SplitterEventArgs e)
		{
			SplitContainer sc = sender as SplitContainer;

			// 确保 Panel1 不小于最小宽度
			if(sc.Panel1.Width < sc.Panel1MinSize)
			{
				sc.SplitterDistance = sc.Panel1MinSize;
			}

			// 确保 Panel2 不小于最小宽度
			if(sc.Panel2.Width < sc.Panel2MinSize)
			{
				sc.SplitterDistance = sc.Width - sc.Panel2MinSize - sc.SplitterWidth;
			}
		}

		private void sC4_SplitterMoving(object sender, SplitterCancelEventArgs e)
		{
			SplitContainer sc = sender as SplitContainer;

			// 防止 Panel1 小于最小宽度
			if(e.SplitX < sc.Panel1MinSize)
			{
				e.Cancel = true; // 取消移动
				sc.SplitterDistance = sc.Panel1MinSize;
			}

			// 防止 Panel2 小于最小宽度
			int panel2Width = sc.Width - e.SplitX - sc.SplitterWidth;
			if(panel2Width < sc.Panel2MinSize)
			{
				e.Cancel = true; // 取消移动
				sc.SplitterDistance = sc.Width - sc.Panel2MinSize - sc.SplitterWidth;
			}
		}

		private void panel4_SizeChanged(object sender, EventArgs e)  // 导入 。。dynamic resize
		{
			if(Fileupperleft.Visible)
			{
				// 调整按钮和二维码的位置
				openfile.Left = (Fileupperleft.Width - openfile.Width) / 2 - 20; // 水平居中
				openfile.Top = (Fileupperleft.Height - openfile.Height) / 2; // 垂直居中
				qrcode1.Left = openfile.Left + 290; // 水平居中	
				qrcode1.Top = (Fileupperleft.Height - qrcode1.Height) / 2; // 水平居中	
			}
		}

		#endregion
		#region   ------------动态加载 LibVLCSharp.WinForms	暂时不用----------------	
		//private void LoadLibVLCSharpDynamically( )
		//{
		//	try {
		//		// 动态加载 LibVLCSharp.WinForms 程序集 D:\Documents\Visual Studio 2022\MusicChange
		//		//string libVLCSharpWinFormsPath = Path.Combine(Application.StartupPath, "LibVLCSharp.WinForms.dll");
		//		string libVLCSharpWinFormsPath = Path.Combine( $"D:\\Documents\\Visual Studio 2022\\MusicChange", "LibVLCSharp.WinForms.dll" );

		//		libVLCSharpWinFormsAssembly = Assembly.LoadFrom( libVLCSharpWinFormsPath );
		//		// 获取 VideoView 类型
		//		videoViewType = libVLCSharpWinFormsAssembly.GetType( "LibVLCSharp.WinForms.VideoView" );
		//		// 创建 VideoView 实例 				videoViewInstance = Activator.CreateInstance(videoViewType);  暂时不用
		//		// 设置 MediaPlayer 属性（如果需要）
		//		// 这需要使用反射来设置属性
		//	}
		//	catch (Exception ex) {
		//		MessageBox.Show( $"动态加载 LibVLCSharp 失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
		//	}
		//}
		#endregion
		#region ------------  全局设置  -------------
		private void buttonx11_Click(object sender, EventArgs e)
		{
			//	db dr = new db( dbPath );			dr.dbinit();
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "视频文件|*.mp4;*.avi;*.wmv;*.mov|所有文件|*.*";
			if(ofd.ShowDialog() == DialogResult.OK)
			{
				temp.Text = ofd.FileName;
			}
			filePath = ofd.FileName;
			PlayVideo();
			var fsize = mediaPlayer.Scale;
			temp1.Text = fsize.ToString();
			temp2.Text = $"缩放: {mediaPlayer.Scale:P0}"; // 显示为百分比

		}

		#endregion
		#region   ------------ LibVLC   视频 最大化,启动外部 视频播放程序   -----------------	
		// 使用外部播放器最大化视频
		private void MaximizeWithExternalPlayer()
		{
			if(string.IsNullOrEmpty(filePath))
			{
				MessageBox.Show("没有选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			try
			{
				// 方法1: 使用系统默认播放器
				System.Diagnostics.Process.Start(filePath);
				mediaPlayer.Pause();   // 暂停播放 ???????
									   // 或者方法2: 指定特定的外部播放器  ?????????
									   // LaunchWithSpecificPlayer(filePath);
			}
			catch(Exception ex)
			{
				MessageBox.Show($"无法启动外部播放器: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				// 如果外部播放器启动失败，回退到内置最大化
				ToggleVideoMaximize();
			}
		}

		// 查找 VLC 安装路径
		private string FindVLCPath()
		{
			// 常见的 VLC 安装路径
			string[] vlcPaths = {
		@"C:\Program Files\VideoLAN\VLC\vlc.exe",
		@"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe",
		@"C:\Program Files\VLC\vlc.exe",
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

			// 检查 PATH 环境变量
			string pathEnv = Environment.GetEnvironmentVariable("PATH");
			if(!string.IsNullOrEmpty(pathEnv))
			{
				string[] paths = pathEnv.Split(';');
				foreach(string p in paths)
				{
					string fullPath = Path.Combine(p, "vlc.exe");
					if(File.Exists(fullPath))
					{
						return fullPath;
					}
				}
			}

			return string.Empty;
		}

		// 查找 MPC-HC 安装路径
		private string FindMPCPath()
		{
			string[] mpcPaths = {
		@"C:\Program Files\MPC-HC\mpc-hc64.exe",
		@"C:\Program Files (x86)\MPC-HC\mpc-hc.exe",
		@"C:\Program Files\MPC-HC\mpc-hc.exe",
		Environment.GetEnvironmentVariable("ProgramFiles") + @"\MPC-HC\mpc-hc64.exe",
		Environment.GetEnvironmentVariable("ProgramFiles(x86)") + @"\MPC-HC\mpc-hc.exe"
	};

			foreach(string path in mpcPaths)
			{
				if(File.Exists(path))
				{
					return path;
				}
			}

			return string.Empty;
		}
		// 使用外部播放器的标志
		private bool UseExternalPlayerForMaximize = false;
		// 修改原有的最大化切换方法
		private void ToggleVideoMaximize()
		{
			if(videoView1 == null)
				return;

			if(!isVideoMaximized)
			{
				if(UseExternalPlayerForMaximize)
				{
					MaximizeWithExternalPlayer();
				}
				else
				{
					//MaximizeVideoView();
				}
			}
			else
			{
				//RestoreVideoView();
			}
		}

		// 外部播放器配置设置
		public class ExternalPlayerSettings
		{
			public string PlayerPath
			{
				get; set;
			}
			public string Arguments
			{
				get; set;
			}
			public bool UseFullscreen
			{
				get; set;
			}
			public bool StartFromCurrentPosition
			{
				get; set;
			}
		}

		// 高级外部播放器启动
		private void LaunchWithAdvancedSettings(string videoPath)
		{
			try
			{
				ExternalPlayerSettings settings = GetExternalPlayerSettings();

				if(!string.IsNullOrEmpty(settings.PlayerPath) && File.Exists(settings.PlayerPath))
				{
					string arguments = settings.Arguments ?? "";

					// 如果需要从当前播放位置开始
					if(settings.StartFromCurrentPosition && mediaPlayer != null)
					{
						long currentTime = mediaPlayer.Time;
						arguments += $" --start-time={currentTime / 1000.0}";
					}

					// 如果需要全屏
					if(settings.UseFullscreen)
					{
						arguments += " --fullscreen";
					}

					arguments += $" \"{videoPath}\"";

					System.Diagnostics.Process.Start(settings.PlayerPath, arguments);
				}
				else
				{
					// 使用系统默认播放器
					System.Diagnostics.Process.Start(videoPath);
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show($"启动外部播放器失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		// 获取外部播放器设置（可以从配置文件或注册表读取）
		private ExternalPlayerSettings GetExternalPlayerSettings()
		{
			// 这里可以从配置文件、注册表或用户设置中读取
			// 示例实现：
			return new ExternalPlayerSettings
			{
				PlayerPath = FindVLCPath(), // 或从设置中读取
				Arguments = "", // 自定义参数
				UseFullscreen = true,
				StartFromCurrentPosition = true
			};
		}

		// 检测外部播放器是否正在运行
		private bool IsExternalPlayerRunning()
		{
			try
			{
				// 检查常见的视频播放器进程
				string[] playerProcesses = { "vlc", "mpc-hc", "wmplayer", "potplayer", "kmplayer" };

				foreach(string processName in playerProcesses)
				{
					var processes = System.Diagnostics.Process.GetProcessesByName(processName);
					if(processes.Length > 0)
					{
						return true;
					}
				}
			}
			catch
			{
				// 忽略异常
			}

			return false;
		}

		// 监控外部播放器状态
		private void MonitorExternalPlayer()
		{
			System.Threading.Timer playerMonitor = null;
			playerMonitor = new System.Threading.Timer((state) =>
		{
			if(!IsExternalPlayerRunning())
			{
				// 外部播放器已关闭，恢复主界面
				this.Invoke(new Action(() =>
	 {
		 // 恢复界面状态
		 if(this.WindowState == FormWindowState.Minimized)
		 {
			 this.WindowState = FormWindowState.Normal;
		 }
		 this.Activate();
	 }));

				// 停止监控
				playerMonitor?.Dispose();
			}
		}, null, 0, 1000); // 每秒检查一次
		}

		// 外部播放器按钮点击事件
		private void externalPlayerButton_Click(object sender, EventArgs e)
		{
			UseExternalPlayerForMaximize = true;
			MaximizeWithExternalPlayer();
		}
		// 修改 vieweMax_Click 方法
		private void vieweMax_Click(object sender, EventArgs e)
		{
			// 可以根据需要切换最大化方式 			// 方式1: 总是使用内置最大化			// ToggleVideoMaximize();
			// 方式2: 切换最大化方式			
			// 切换最大化方式（内置 vs 外部）
			//private void ToggleMaximizeMethod() 			{
			//if(isVideoMaximized)
			//{
			//	// 当前是内置最大化，切换到外部播放器
			//	UseExternalPlayerForMaximize = !UseExternalPlayerForMaximize;
			//	//RestoreVideoView(); // 先恢复当前最大化

			UseExternalPlayerForMaximize = !UseExternalPlayerForMaximize;
			if(UseExternalPlayerForMaximize)
			{
				MaximizeWithExternalPlayer();
			}
			else
			{
				ToggleVideoMaximize(); // 使用内置最大化
			}
			//}  			// 方式3: 根据设置决定
			if(UseExternalPlayerForMaximize)
			{
				MaximizeWithExternalPlayer();
			}
			else
			{
				ToggleVideoMaximize();
			}
		}

		// 从文件资源管理器打开
		private void OpenWithExternalPlayerFromExplorer()
		{
			if(string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
			{
				MessageBox.Show("文件不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			try
			{
				// 使用 Windows 资源管理器的"打开方式"功能
				System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
				{
					FileName = "rundll32.exe",
					Arguments = $"shell32.dll,OpenAs_RunDLL \"{filePath}\"",
					UseShellExecute = false
				};
				System.Diagnostics.Process.Start(psi);
			}
			catch(Exception ex)
			{
				MessageBox.Show($"打开方式对话框失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		// 发送文件到外部播放器（使用 Windows 的"发送到"功能）
		private void SendToExternalPlayer()
		{
			if(string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
			{
				MessageBox.Show("文件不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			try
			{
				// 创建临时的 .lnk 文件指向播放器
				string tempPath = Path.GetTempPath();
				string linkPath = Path.Combine(tempPath, "ExternalPlayer.lnk");

				// 这里简化处理，直接使用系统关联
				System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{filePath}\"");
			}
			catch(Exception ex)
			{
				MessageBox.Show($"发送到外部播放器失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		//当videoView 大小尺寸变化使调整 播放画面符合 尺寸
		private void videoView1_Resize(object sender, EventArgs e)
		{
			//int vvW = videoView1.Width;
		}

		// 保存和加载偏好设置
		private PlayerPreferences LoadPlayerPreferences()
		{
			// 从配置文件或注册表加载设置
			PlayerPreferences prefs = new()
			{
				UseExternalPlayer = Properties.Settings.Default.UseExternalPlayer,
				PreferredPlayer = Properties.Settings.Default.PreferredPlayer,
				AutoFullscreen = Properties.Settings.Default.AutoFullscreen,
				MinimizeOnExternalLaunch = Properties.Settings.Default.MinimizeOnExternalLaunch,
				CustomPlayerPath = Properties.Settings.Default.CustomPlayerPath
			};

			return prefs;
		}
		private void SavePlayerPreferences(PlayerPreferences prefs)
		{
			Properties.Settings.Default.UseExternalPlayer = prefs.UseExternalPlayer;
			Properties.Settings.Default.PreferredPlayer = prefs.PreferredPlayer;
			Properties.Settings.Default.AutoFullscreen = prefs.AutoFullscreen;
			Properties.Settings.Default.MinimizeOnExternalLaunch = prefs.MinimizeOnExternalLaunch;
			Properties.Settings.Default.CustomPlayerPath = prefs.CustomPlayerPath;
			Properties.Settings.Default.Save();
		}
		// 在设置或配置文件中保存外部播放器偏好
		public class PlayerPreferences
		{
			public bool UseExternalPlayer
			{
				get; set;
			}
			public string PreferredPlayer
			{
				get; set;
			} // "vlc", "mpc", "wmp", "default"
			public bool AutoFullscreen
			{
				get; set;
			}
			public bool MinimizeOnExternalLaunch
			{
				get; set;
			}
			public string CustomPlayerPath
			{
				get; set;
			}
		}
		#endregion
		#region   ------------ LibVLC 计算视频在指定容器内的自适应缩放比例    -----------------	
		/// <summary>
		/// 计算视频在指定容器内的自适应缩放比例 
		/// </summary>
		/// <param name="videoWidth">视频原始宽度</param>
		/// <param name="videoHeight">视频原始高度</param>
		/// <param name="containerWidth">容器宽度</param>
		/// <param name="containerHeight">容器高度</param>
		/// <param name="mode">缩放模式</param>
		/// <returns>计算出的缩放比例</returns>
		private float CalculateVideoScaleFactor(int videoWidth, int videoHeight, int containerWidth, int containerHeight, VideoScaleMode mode = VideoScaleMode.Fit)
		{
			try
			{
				// 验证输入参数
				if(videoWidth <= 0 || videoHeight <= 0 || containerWidth <= 0 || containerHeight <= 0)
				{
					return 1.0f; // 默认缩放比例
				}
				// 计算宽高比
				float videoAspectRatio = (float)videoWidth / videoHeight;
				float containerAspectRatio = (float)containerWidth / containerHeight;
				float scaleFactor = 1.0f;
				switch(mode)
				{
					case VideoScaleMode.Fit: // 适应容器（完整显示，可能有黑边）
						if(videoAspectRatio > containerAspectRatio)
						{
							// 视频更宽，以宽度为准
							scaleFactor = (float)containerWidth / videoWidth;
						}
						else
						{
							// 视频更高，以高度为准
							scaleFactor = (float)containerHeight / videoHeight;
						}
						break;

					case VideoScaleMode.Fill: // 填充容器（可能裁剪部分内容）
						if(videoAspectRatio > containerAspectRatio)
						{
							// 视频更宽，以高度为准
							scaleFactor = (float)containerHeight / videoHeight;
						}
						else
						{
							// 视频更高，以宽度为准
							scaleFactor = (float)containerWidth / videoWidth;
						}
						break;

					case VideoScaleMode.Stretch: // 拉伸填充（可能变形）
						float scaleX = (float)containerWidth / videoWidth;   // 分别计算X和Y方向的缩放比例
						float scaleY = (float)containerHeight / videoHeight;
						scaleFactor = (scaleX + scaleY) / 2;    // 这种模式下返回平均缩放比例
						break;

					case VideoScaleMode.Original: // 原始大小
						scaleFactor = 1.0f;
						break;

					case VideoScaleMode.BestFit: // 最佳适应（保持比例且完整显示）
					default:
						// 与Fit模式相同
						if(videoAspectRatio > containerAspectRatio)
						{
							scaleFactor = (float)containerWidth / videoWidth;
						}
						else
						{
							scaleFactor = (float)containerHeight / videoHeight;
						}
						break;
				}

				// 确保缩放比例在合理范围内
				scaleFactor = Math.Max(0.1f, Math.Min(10.0f, scaleFactor));
				return scaleFactor;
			}
			catch(Exception ex)
			{
				if(System.Diagnostics.Debugger.IsAttached)
				{
					System.Diagnostics.Debug.WriteLine($"计算视频缩放比例时出错: {ex.Message}");
				}
				return 1.0f;
			}
		}

		/// <summary>
		/// 获取视频的原始尺寸
		/// </summary>
		/// <returns>视频尺寸信息</returns>
		private VideoSizeInfo GetVideoOriginalSize()
		{
			try
			{
				if(mediaPlayer != null)
				{
					//	// 从 MediaPlayer 获取视频尺寸
					//	var videoTrack = mediaPlayer.VideoTrack;
					//	if (videoTrack.HasValue) {
					//		return new VideoSizeInfo
					//		{
					//			Width = (int) videoTrack.Value.Data.Width,
					//			Height = (int)	videoTrack.Value.Data.Height
					//		};
					//	}

					// 备用方法：从媒体信息获取
					var media = mediaPlayer.Media;
					if(media != null)
					{
						var videoTracks = media.Tracks.Where(t => t.TrackType == TrackType.Video).ToArray();
						if(videoTracks.Length > 0)
						{
							return new VideoSizeInfo
							{
								Width = (int)videoTracks[0].Data.Video.Width,
								Height = (int)videoTracks[0].Data.Video.Height
							};
						}
					}
				}
			}
			catch(Exception ex)
			{
				if(System.Diagnostics.Debugger.IsAttached)
				{
					System.Diagnostics.Debug.WriteLine($"获取视频原始尺寸时出错: {ex.Message}");
				}
			}

			// 返回默认值或从文件属性获取
			return GetVideoSizeFromFile();
		}

		/// <summary>
		/// 从文件获取视频尺寸（备用方法）
		/// </summary>
		/// <returns>视频尺寸信息</returns>
		private VideoSizeInfo GetVideoSizeFromFile()
		{
			try
			{
				if(!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
				{
					// 使用 MediaInfo 或其他库获取视频信息
					using(var media = new Media(libVLC, filePath, FromType.FromPath))
					{
						media.Parse(MediaParseOptions.ParseNetwork);

						var videoTracks = media.Tracks.Where(t => t.TrackType == TrackType.Video).ToArray();
						if(videoTracks.Length > 0)
						{
							return new VideoSizeInfo
							{
								Width = (int)videoTracks[0].Data.Video.Width,
								Height = (int)videoTracks[0].Data.Video.Height
							};
						}
					}
				}
			}
			catch(Exception ex)
			{
				if(System.Diagnostics.Debugger.IsAttached)
				{
					System.Diagnostics.Debug.WriteLine($"从文件获取视频尺寸时出错: {ex.Message}");
				}
			}

			return new VideoSizeInfo { Width = 1920, Height = 1080 }; // 默认 1080p
		}

		/// <summary>
		/// 调整视频以适应 videoView 尺寸
		/// </summary>
		private void AdjustVideoToViewSize()
		{
			try
			{
				if(videoView1 == null || mediaPlayer == null)
					return;

				// 获取 videoView 的当前尺寸
				int viewWidth = videoView1.Width;
				int viewHeight = videoView1.Height;

				// 如果尺寸太小，不进行调整
				if(viewWidth < 10 || viewHeight < 10)
					return;

				// 获取视频原始尺寸
				VideoSizeInfo videoSize = GetVideoOriginalSize();

				if(videoSize.Width > 0 && videoSize.Height > 0)
				{
					// 计算自适应缩放比例
					float scaleFactor = CalculateVideoScaleFactor(
						videoSize.Width, videoSize.Height,
						viewWidth, viewHeight,
						VideoScaleMode.Fit); // 使用适应模式

					// 应用缩放
					if(mediaPlayer != null)
					{
						mediaPlayer.Scale = scaleFactor;
						currentZoomFactor = scaleFactor;

						// 更新缩放显示
						UpdateZoomLabel();

						if(System.Diagnostics.Debugger.IsAttached)
						{
							System.Diagnostics.Debug.WriteLine($"视频尺寸: {videoSize.Width}x{videoSize.Height}, " +
															  $"视图尺寸: {viewWidth}x{viewHeight}, " +
															  $"缩放比例: {scaleFactor:F2}");
						}
					}
				}
			}
			catch(Exception ex)
			{
				if(System.Diagnostics.Debugger.IsAttached)
				{
					System.Diagnostics.Debug.WriteLine($"调整视频到视图尺寸时出错: {ex.Message}");
				}
			}
		}

		/// <summary>
		/// 在 videoView 尺寸改变时自动调整视频
		/// </summary>
		private void VideoView_SizeChanged(object sender, EventArgs e)
		{
			// 延迟执行调整，避免频繁调整
			System.Threading.Timer adjustTimer = null;
			adjustTimer = new System.Threading.Timer((state) =>
		{
			this.Invoke(new Action(() =>
		{
			AdjustVideoToViewSize();
			adjustTimer?.Dispose();
		}));
		}, null, 100, System.Threading.Timeout.Infinite); // 100ms 延迟
		}

		/// <summary>
		/// 视频尺寸信息结构
		/// </summary>
		public struct VideoSizeInfo
		{
			public int Width
			{
				get; set;
			}
			public int Height
			{
				get; set;
			}

			public float AspectRatio => Height > 0 ? (float)Width / Height : 1.0f;

			public override string ToString()
			{
				return $"{Width}x{Height} ({AspectRatio:F2})";
			}
		}

		/// <summary>
		/// 视频缩放模式枚举
		/// </summary>
		public enum VideoScaleMode
		{
			/// <summary>
			/// 适应模式 - 完整显示视频，可能有黑边
			/// </summary>
			Fit,

			/// <summary>
			/// 填充模式 - 填满容器，可能裁剪部分内容
			/// </summary>
			Fill,

			/// <summary>
			/// 拉伸模式 - 拉伸以填满容器，可能变形
			/// </summary>
			Stretch,

			/// <summary>
			/// 原始大小模式 - 保持视频原始尺寸
			/// </summary>
			Original,

			/// <summary>
			/// 最佳适应模式 - 保持比例且完整显示
			/// </summary>
			BestFit
		}

		/// <summary>
		/// 计算视频在容器中的显示矩形
		/// </summary>
		/// <param name="videoWidth">视频宽度</param>
		/// <param name="videoHeight">视频高度</param>
		/// <param name="containerRect">容器矩形</param>
		/// <param name="mode">缩放模式</param>
		/// <returns>视频显示矩形</returns>
		private Rectangle CalculateVideoDisplayRect(int videoWidth, int videoHeight,
			Rectangle containerRect, VideoScaleMode mode = VideoScaleMode.Fit)
		{
			try
			{
				if(videoWidth <= 0 || videoHeight <= 0)
				{
					return containerRect;
				}

				float videoAspectRatio = (float)videoWidth / videoHeight;
				float containerAspectRatio = (float)containerRect.Width / containerRect.Height;

				int displayWidth, displayHeight;
				int displayX, displayY;

				switch(mode)
				{
					case VideoScaleMode.Fit:
					case VideoScaleMode.BestFit:
						if(videoAspectRatio > containerAspectRatio)
						{
							// 视频更宽
							displayWidth = containerRect.Width;
							displayHeight = (int)(displayWidth / videoAspectRatio);
						}
						else
						{
							// 视频更高
							displayHeight = containerRect.Height;
							displayWidth = (int)(displayHeight * videoAspectRatio);
						}
						break;

					case VideoScaleMode.Fill:
						if(videoAspectRatio > containerAspectRatio)
						{
							// 视频更宽
							displayHeight = containerRect.Height;
							displayWidth = (int)(displayHeight * videoAspectRatio);
						}
						else
						{
							// 视频更高
							displayWidth = containerRect.Width;
							displayHeight = (int)(displayWidth / videoAspectRatio);
						}
						break;

					case VideoScaleMode.Stretch:
						displayWidth = containerRect.Width;
						displayHeight = containerRect.Height;
						break;

					case VideoScaleMode.Original:
					default:
						displayWidth = Math.Min(videoWidth, containerRect.Width);
						displayHeight = Math.Min(videoHeight, containerRect.Height);
						// 保持原始宽高比
						if((float)displayWidth / displayHeight != videoAspectRatio)
						{
							if(displayWidth / videoAspectRatio <= displayHeight)
							{
								displayHeight = (int)(displayWidth / videoAspectRatio);
							}
							else
							{
								displayWidth = (int)(displayHeight * videoAspectRatio);
							}
						}
						break;
				}

				// 居中显示
				displayX = containerRect.X + (containerRect.Width - displayWidth) / 2;
				displayY = containerRect.Y + (containerRect.Height - displayHeight) / 2;
				return new Rectangle(displayX, displayY, displayWidth, displayHeight);
			}
			catch
			{
				return containerRect;
			}
		}

		/// <summary>
		/// 应用自适应缩放
		/// </summary>
		private void ApplyAutoScale()
		{
			try
			{
				if(videoView1 == null || mediaPlayer == null)
					return;
				// 获取视频原始尺寸
				VideoSizeInfo videoSize = GetVideoOriginalSize();
				if(videoSize.Width > 0 && videoSize.Height > 0)
				{
					// 计算自适应缩放比例
					float scaleFactor = CalculateVideoScaleFactor(videoSize.Width, videoSize.Height, videoView1.Width, videoView1.Height, VideoScaleMode.Fit);
					// 应用缩放
					mediaPlayer.Scale = scaleFactor;
					currentZoomFactor = scaleFactor;
					// 更新显示
					UpdateZoomLabel();
				}
			}
			catch(Exception ex)
			{
				if(System.Diagnostics.Debugger.IsAttached)
				{
					System.Diagnostics.Debug.WriteLine($"应用自适应缩放时出错: {ex.Message}");
				}
			}
		}

		/// <summary>
		/// 在窗体加载时订阅 videoView 的尺寸改变事件
		/// </summary>
		private void SubscribeVideoViewEvents()
		{
			try
			{
				if(videoView1 != null)
				{
					videoView1.SizeChanged += VideoView_SizeChanged;
					videoView1.Resize += VideoView_SizeChanged;
				}
			}
			catch(Exception ex)
			{
				if(System.Diagnostics.Debugger.IsAttached)
				{
					System.Diagnostics.Debug.WriteLine($"订阅 videoView 事件时出错: {ex.Message}");
				}
			}
		}

		/// <summary>
		/// 在窗体加载完成后调用
		/// </summary>
		private void LaserEditing_Loadload(object sender, EventArgs e)
		{
			// ... 现有代码 ...

			// 订阅 videoView 事件
			SubscribeVideoViewEvents();

			// 初始调整
			AdjustVideoToViewSize();
		}

		//// 在 LaserEditing 类中添加以下方法

		//// 保存视频调整设置
		//private void SaveVideoAdjustments()
		//{
		//	if(mediaPlayer != null)
		//	{
		//		try
		//		{
		//			Properties.Settings.Default.VideoBrightness = _brightnessTrackBar.Value / 100.0f;
		//			Properties.Settings.Default.VideoContrast = _contrastTrackBar.Value / 100.0f;
		//			Properties.Settings.Default.VideoSaturation = _saturationTrackBar.Value / 100.0f;
		//			Properties.Settings.Default.VideoGamma = _gammaTrackBar.Value / 100.0f;
		//			Properties.Settings.Default.Save();
		//		}
		//		catch(Exception ex)
		//		{
		//			MessageBox.Show($"保存视频设置失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
		//		}
		//	}
		//}

		//// 加载视频调整设置
		//private void LoadVideoAdjustments()
		//{
		//	try
		//	{
		//		float brightness = Properties.Settings.Default.VideoBrightness;
		//		float contrast = Properties.Settings.Default.VideoContrast;
		//		float saturation = Properties.Settings.Default.VideoSaturation;
		//		float gamma = Properties.Settings.Default.VideoGamma;

		//		// 应用保存的设置
		//		if(mediaPlayer != null)
		//		{
		//			mediaPlayer.SetVideoAdjustment(VideoAdjustmentOptions.Enable, brightness, contrast, saturation, gamma, 0.0f);
		//		}
		//	}
		//	catch(Exception ex)
		//	{
		//		MessageBox.Show($"加载视频设置失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
		//	}
		//}

		// 在 LaserEditing.cs 文件末尾或其他合适位置添加

		#endregion
		#region  ----------------------  调整播放速度 ---------------

		// 初始化播放速度菜单
		private void InitializeSpeedMenu()
		{
			speedContextMenu = new ContextMenuStrip();

			// 定义播放速度选项
			float[] speedRates = { 0.25f, 0.5f, 0.75f, 1.0f, 1.25f, 1.5f, 1.75f, 2.0f, 2.5f, 3.0f, 4.0f };

			foreach(float rate in speedRates)
			{
				ToolStripMenuItem item = new ToolStripMenuItem($"{rate}x");
				item.Tag = rate;
				item.Click += SpeedMenuItem_Click;
				speedContextMenu.Items.Add(item);
			}

			// 添加自定义速度选项
			ToolStripSeparator separator = new ToolStripSeparator();
			speedContextMenu.Items.Add(separator);

			ToolStripMenuItem customItem = new ToolStripMenuItem("自定义速度...");
			customItem.Click += CustomSpeedMenuItem_Click;
			speedContextMenu.Items.Add(customItem);

			// 将菜单关联到 speed 按钮
			if(speed != null)
			{
				speed.ContextMenuStrip = speedContextMenu;
			}
		}
		// 播放速度菜单项点击事件
		private void SpeedMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				if(mediaPlayer == null)
				{
					MessageBox.Show("播放器未初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				if(sender is ToolStripMenuItem menuItem && menuItem.Tag is float rate)
				{
					// 设置播放速率
					mediaPlayer.SetRate(rate);

					// 更新UI
					if(SpeedT != null)
					{
						SpeedT.Text = rate.ToString("F2");
					}

					if(temp2 != null)
					{
						temp2.Text = $"播放速率: {rate:F2}x";
					}

					// 更新按钮文本或状态
					if(speed != null)
					{
						speed.Text = $"{rate}x";
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show($"设置播放速率失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		// 自定义速度菜单项点击事件
		private void CustomSpeedMenuItem_Click(object sender, EventArgs e)
		{
			// 显示输入对话框让用户输入自定义速度
			using(Form customSpeedForm = new Form())
			{
				customSpeedForm.Text = "设置自定义播放速度";
				customSpeedForm.Size = new Size(300, 150);
				customSpeedForm.StartPosition = FormStartPosition.CenterParent;
				customSpeedForm.FormBorderStyle = FormBorderStyle.FixedDialog;
				customSpeedForm.MaximizeBox = false;
				customSpeedForm.MinimizeBox = false;

				Label label = new Label
				{
					Text = "请输入播放速度 (0.25 - 4.0):",
					Location = new Point(20, 20),
					Size = new Size(200, 20)
				};

				TextBox speedTextBox = new TextBox
				{
					Text = mediaPlayer?.Rate.ToString("F2") ?? "1.00",
					Location = new Point(20, 45),
					Size = new Size(100, 20)
				};

				Button okButton = new Button
				{
					Text = "确定",
					Location = new Point(20, 80),
					Size = new Size(75, 25),
					DialogResult = DialogResult.OK
				};

				Button cancelButton = new Button
				{
					Text = "取消",
					Location = new Point(105, 80),
					Size = new Size(75, 25),
					DialogResult = DialogResult.Cancel
				};

				customSpeedForm.Controls.AddRange(new Control[] { label, speedTextBox, okButton, cancelButton });

				customSpeedForm.AcceptButton = okButton;
				customSpeedForm.CancelButton = cancelButton;

				if(customSpeedForm.ShowDialog() == DialogResult.OK)
				{
					if(float.TryParse(speedTextBox.Text, out float customRate))
					{
						// 限制范围
						customRate = Math.Max(0.25f, Math.Min(4.0f, customRate));
						SetPlaybackRate(customRate);
					}
					else
					{
						MessageBox.Show("请输入有效的数字", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}
			}
		}
		// 修改 speed 按钮点击事件
		private void speed_Click(object sender, EventArgs e)
		{
			// 如果上下文菜单已初始化，显示菜单
			if(speedContextMenu != null)
			{
				// 在按钮下方显示菜单
				speedContextMenu.Show(speed, new Point(0, speed.Height));
			}
			else
			{
				// 如果菜单未初始化，使用原来的文本框方式
				try
				{
					if(mediaPlayer == null)
					{
						MessageBox.Show("播放器未初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;
					}

					if(float.TryParse(SpeedT.Text, out float rate))
					{
						rate = Math.Max(0.25f, Math.Min(4.0f, rate));
						mediaPlayer.SetRate(rate);
						SpeedT.Text = mediaPlayer.Rate.ToString("F2");

					}
					else
					{
						MessageBox.Show("请输入有效的数字", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						SpeedT.Text = mediaPlayer.Rate.ToString("F2") + "X";
					}
				}
				catch(Exception ex)
				{
					MessageBox.Show($"设置播放速率失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
					if(mediaPlayer != null)
					{
						SpeedT.Text = mediaPlayer.Rate.ToString("F2");
					}
				}
			}
		}
		// 如果您想要为菜单项添加图标
		private void InitializeSpeedMenuWithIcons()
		{
			speedContextMenu = new ContextMenuStrip();

			float[] speedRates = { 0.25f, 0.5f, 0.75f, 1.0f, 1.25f, 1.5f, 1.75f, 2.0f, 2.5f, 3.0f, 4.0f };

			foreach(float rate in speedRates)
			{
				ToolStripMenuItem item = new ToolStripMenuItem($"{rate}x");
				item.Tag = rate;
				item.Click += SpeedMenuItem_Click;

				// 可选：为当前速率添加选中标记
				if(mediaPlayer != null && Math.Abs(rate - mediaPlayer.Rate) < 0.01f)
				{
					item.Checked = true;
				}

				speedContextMenu.Items.Add(item);
			}

			ToolStripSeparator separator = new ToolStripSeparator();
			speedContextMenu.Items.Add(separator);

			ToolStripMenuItem customItem = new ToolStripMenuItem("自定义速度...");
			customItem.Click += CustomSpeedMenuItem_Click;
			speedContextMenu.Items.Add(customItem);

			if(speed != null)
			{
				speed.ContextMenuStrip = speedContextMenu;
			}
		}
		// 设置播放速率的通用方法
		private void SetPlaybackRate(float rate)
		{
			try
			{
				if(mediaPlayer == null)
					return;

				// 限制播放速率范围
				rate = Math.Max(0.25f, Math.Min(4.0f, rate));

				// 设置播放速率
				mediaPlayer.SetRate(rate);

				// 更新UI
				if(SpeedT != null)
				{
					SpeedT.Text = rate.ToString("F2") + "X";
				}

			}
			catch(Exception ex)
			{
				MessageBox.Show($"设置播放速率失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		// 获取当前播放速率
		private float GetCurrentPlaybackRate()
		{
			try
			{
				return mediaPlayer?.Rate ?? 1.0f;
			}
			catch
			{
				return 1.0f;
			}
		}

		// 增加播放速率
		private void IncreasePlaybackRate()
		{
			float currentRate = GetCurrentPlaybackRate();
			float[] rates = { 0.25f, 0.5f, 0.75f, 1.0f, 1.25f, 1.5f, 1.75f, 2.0f, 2.5f, 3.0f, 4.0f };

			// 找到下一个更高的速率
			foreach(float rate in rates)
			{
				if(rate > currentRate)
				{
					SetPlaybackRate(rate);
					return;
				}
			}
			// 如果已经是最高速率，保持不变
			SetPlaybackRate(4.0f);
		}

		// 降低播放速率
		private void DecreasePlaybackRate()
		{
			float currentRate = GetCurrentPlaybackRate();
			float[] rates = { 0.25f, 0.5f, 0.75f, 1.0f, 1.25f, 1.5f, 1.75f, 2.0f, 2.5f, 3.0f, 4.0f };

			// 找到下一个更低的速率
			for(int i = rates.Length - 1 ;i >= 0 ;i--)
			{
				if(rates[i] < currentRate)
				{
					SetPlaybackRate(rates[i]);
					return;
				}
			}

			// 如果已经是最慢速率，保持不变
			SetPlaybackRate(0.25f);
		}

		// 预设播放速率按钮点击事件
		private void presetRateButton_Click(object sender, EventArgs e)
		{
			if(sender is Button button && button.Tag is float rate)
			{
				SetPlaybackRate(rate);
			}
		}

		// 创建预设播放速率按钮
		private void CreatePresetRateButtons()
		{
			float[] presetRates = { 0.5f, 0.75f, 1.0f, 1.25f, 1.5f, 2.0f };

			// 假设您有一个容器控件来放置这些按钮
			FlowLayoutPanel ratePanel = new FlowLayoutPanel();

			foreach(float rate in presetRates)
			{
				Button rateButton = new Button
				{
					Text = $"{rate}x",
					Tag = rate,
					Size = new Size(50, 30),
					Margin = new Padding(2)
				};
				rateButton.Click += presetRateButton_Click;
				ratePanel.Controls.Add(rateButton);
			}

			// 将面板添加到您的界面中
			// this.Controls.Add(ratePanel);
		}

		#endregion
		#region   ------------------  手动触发自适应调整  ---------------
		/// <summary>
		/// 手动触发自适应调整
		/// </summary>
		private void AutoFitButton_Click(object sender, EventArgs e)
		{
			ApplyAutoScale();
		}
		private void sC5_Panel2_Paint(object sender, PaintEventArgs e)
		{

		}
		private void sC6_Panel1_Paint(object sender, PaintEventArgs e)
		{

		}
		private void sC6_Panel2_Paint(object sender, PaintEventArgs e)
		{
		}
		private void AdjustSplitContainer()
		{
			// 确保窗体有足够的高度容纳固定面板
			if(this.ClientSize.Height > FixedPanelHeight)
			{
				textBoxX1.Text = this.ClientSize.Height.ToString();
				// 计算上半部分的高度
				int topPanelHeight = this.ClientSize.Height - FixedPanelHeight;
				// 设置分割器位置，实现上半部分可变，下半部分固定
				splitContainer2.SplitterDistance = topPanelHeight;
				// 确保分割器不会被拖到超出范围
				splitContainer2.Panel1MinSize = 500; // 上半部分最小高度
			}


		}

		private void LaserEditing_Loadlaod(object sender, EventArgs e)
		{
			// 订阅 videoView 事件以自动调整
			SubscribeVideoViewEvents();
			// 初始调整视频大小
			AdjustVideoToViewSize();
		}
		private void autoFitButton_Click(object sender, EventArgs e)
		{
			ApplyAutoScale(); // 手动触发自适应调整
		}
		#endregion
		#region   ------------------  调节视频的色彩 对比度 和 亮度  ---------------

		private void buttonX1_Click(object sender, EventArgs e)  //调节视频的色彩 对比度 和 亮度
		{
			//判断 mediaPlayer 是否有视频播放

			if(mediaPlayer == null || !mediaPlayer.IsPlaying)
			{
				MessageBox.Show("播放器未初始化或没有视频播放", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			try
			{
				//using var settingsForm = new AdjustForm( mediaPlayer );
				//settingsForm.ShowDialog();
			}
			catch(Exception ex)
			{
				MessageBox.Show($"打开视频设置失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion
		#region  ------------------  主窗口调节  似乎没解决  但可以用 2025-10-11 ------------------
		private void LaserEditing_Resize(object sender, EventArgs e)
		{
			int totalWidth = this.ClientSize.Width;
			int weight = totalWidth / 3; // 设置窗口宽度  
			int panelWidth = Math.Max(weight, MIN_WIDTH);  // 确保不小于最小宽度
			sC3.Panel1MinSize = weight;

			//this.sC3.Panel1MinSize = 300;
			//sC3.SplitterDistance = weight + 20; // 上左
			//sC4.SplitterDistance = weight + 40; //上中		

			temp2.Text = "宽度：" + this.Width.ToString();
			temp1.Text = "高度：" + this.Height.ToString();

			if(this.Height < MAX_WIDTH)
			{
				this.Height = MAX_WIDTH;
				// 防止窗口宽度小于最小宽度
				if(this.Width < MIN_WIDTH)
				{
					// 保持宽度不小于最小宽度
					this.Width = MIN_WIDTH;

				}
				else
				{
					// 记录当前有效宽度

				}
			}
			else
			{
				// 当高度恢复到1000像素以上时，允许宽度调整
				// 这里可以根据需要添加其他逻辑
			}
			if(totalWidth < MAX_HWIDTH)
			{
				this.Width = MAX_HWIDTH;
			}
		}
		private void splitContainer2_SizeChanged(object sender, EventArgs e)
		{
			AdjustSplitContainer();
		}

		#endregion
		#region  ------------------  多窗口 显示视频   ------------------
		private void buttonX2_Click_1(object sender, EventArgs e)
		{
			string videoPath1 = @"F:\newipad\sex.MP4";
			string videoPath2 = @"F:\newipad\已经压缩\让古画活起来_medium.mp4";
			string videoPath3 = @"F:\newipad\已经压缩\背部跳舞_s.mp4";

			PlayVideo(_player1, videoPath1);  // 播放第一个视频
			PlayVideo(_player2, videoPath2);   // 播放第二个视频
			PlayVideo(_player3, videoPath3);

		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			// 清理资源 			_player1?.Stop();			_player2?.Stop();			_player3?.Stop();
			try
			{
				// 显示当前滚动状态
				string scrollInfo = $"垂直滚动启用: {flowLayoutPanelMedia.VerticalScroll.Enabled}\n" +
								   $"垂直滚动可见: {flowLayoutPanelMedia.VerticalScroll.Visible}\n" +
								   $"自动滚动: {flowLayoutPanelMedia.AutoScroll}\n" +
								   $"内容大小: {flowLayoutPanelMedia.AutoScrollMinSize}\n" +
								   $"客户端大小: {flowLayoutPanelMedia.ClientSize}\n" +
								   $"控件数量: {flowLayoutPanelMedia.Controls.Count}";

				MessageBox.Show(scrollInfo, "滚动状态");

				// 强制调整
				ForceScrollAdjustment();
			}
			catch(Exception ex)
			{
				MessageBox.Show($"测试滚动时出错: {ex.Message}", "错误");
			}

		}
		private void PlayVideo(MediaPlayer player, string videoPath)
		{
			if(System.IO.File.Exists(videoPath))
			{
				using var media = new Media(_libVLC1, videoPath);
				player.Play(media);
			}
			else
			{
				MessageBox.Show($"文件不存在: {videoPath}");
			}
		}
		#endregion

		#region  ------------------  上左窗口 导入视频   ------------------
		private void ConfigureFlowLayoutPanel()  //配置 FlowLayoutPanel
		{
			if(flowLayoutPanelMedia == null)
				return;

			try
			{
				// 设置 FlowLayoutPanel 属性
				flowLayoutPanelMedia.FlowDirection = FlowDirection.LeftToRight;
				flowLayoutPanelMedia.WrapContents = true;
				flowLayoutPanelMedia.AutoScroll = true;

				// 减少内边距
				flowLayoutPanelMedia.Padding = new Padding(5);

				// 确保控件紧密排列
				//flowLayoutPanelMedia.FlowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
				flowLayoutPanelMedia.FlowDirection = FlowDirection.LeftToRight;
				flowLayoutPanelMedia.PerformLayout();  // 强制重新布局
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"配置 FlowLayoutPanel 时出错: {ex.Message}");
			}
		}

		private void ConfigureFlowLayoutPanelScrolling()  //配置 FlowLayoutPanel 滚动
		{
			if(flowLayoutPanelMedia == null)
				return;

			try
			{
				// 确保 FlowLayoutPanel 设置正确
				flowLayoutPanelMedia.AutoScroll = true;
				flowLayoutPanelMedia.WrapContents = true;
				flowLayoutPanelMedia.FlowDirection = FlowDirection.LeftToRight;

				// 强制显示垂直滚动条
				flowLayoutPanelMedia.VerticalScroll.Enabled = true;
				flowLayoutPanelMedia.VerticalScroll.Visible = true;

				// 隐藏水平滚动条
				flowLayoutPanelMedia.HorizontalScroll.Enabled = false;
				flowLayoutPanelMedia.HorizontalScroll.Visible = false;

				// 设置停靠和锚定
				flowLayoutPanelMedia.Dock = DockStyle.Fill;
				flowLayoutPanelMedia.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

				// 强制重新布局
				flowLayoutPanelMedia.PerformLayout();
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"配置 FlowLayoutPanel 滚动时出错: {ex.Message}");
			}
		}
		private void initmportfile()  //初始化导入文件
		{         
			flowLayoutPanelMedia.ControlAdded += (s, e) =>  // 订阅媒体项的“播放请求”事件
			{
				if(e.Control is MediaItemControl mediaItem)
				{
					mediaItem.MediaPlayRequested += OnMediaPlayRequested;
				}
			};
		}

		// 导入文件按钮点击事件
		//private void btnImportFiles_Click(object sender, EventArgs e)
	
		// 设置控件的停靠和锚定
		private void flowLayoutPanelMedia_ControlAdded(object sender, ControlEventArgs e)
		{
			// 当添加新控件时，确保滚动功能正常
			if(e.Control is MediaItemControl)
			{
				// 订阅媒体播放请求事件
				if(e.Control is MediaItemControl mediaItem)
				{
					mediaItem.MediaPlayRequested += OnMediaPlayRequested;
				}
				// 延迟调整滚动
				DelayedAdjustScrolling();
			}
		}

		private void flowLayoutPanelMedia_Resize(object sender, EventArgs e)
		{

			// 当面板大小改变时，重新配置滚动
			DelayedAdjustScrolling();
		}
		private void DelayedAdjustScrolling()
		{
			try
			{
				// 使用计时器延迟执行，避免频繁调整
				var timer = new System.Threading.Timer((state) =>
				{
					if(flowLayoutPanelMedia.InvokeRequired)
					{
						flowLayoutPanelMedia.Invoke(new Action(() =>
						{
							ForceScrollAdjustment();
							((System.Threading.Timer)state)?.Dispose();
						}));
					}
					else
					{
						ForceScrollAdjustment();
						((System.Threading.Timer)state)?.Dispose();
					}
				}, null, 100, System.Threading.Timeout.Infinite);
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"延迟调整滚动时出错: {ex.Message}");
			}
		}
		private void ForceScrollAdjustment()
		{
			try
			{
				if(flowLayoutPanelMedia == null)
					return;

				// 强制重新计算内容大小
				flowLayoutPanelMedia.AutoScrollMinSize = new Size(
					flowLayoutPanelMedia.Width - 20, // 减去滚动条宽度
					flowLayoutPanelMedia.Controls.Count * 230 // 假设每个控件高度约230px
				);

				// 确保滚动条可见
				flowLayoutPanelMedia.VerticalScroll.Enabled = true;
				flowLayoutPanelMedia.VerticalScroll.Visible = true;

				// 强制刷新
				flowLayoutPanelMedia.PerformLayout();
				flowLayoutPanelMedia.Invalidate();
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"强制调整滚动时出错: {ex.Message}");
			}
		}
		// 确保滚动条在需要时显示
		private void flowLayoutPanelMedia_Scroll(object sender, ScrollEventArgs e)
		{
			// 可以在这里添加滚动相关的处理逻辑
			System.Diagnostics.Debug.WriteLine($"滚动位置: {e.NewValue}");
		}
		private void importdata_Click(object sender, EventArgs e)  //导入素材文件
		{
			Click(sender, e);
		}
		private new void Click(object sender, EventArgs e)
		{
			//导入素材  Importing the materials  			int c = flowLayoutPanel1.Controls.Count;
			listBox1.Items.Clear();
			string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);  // 获取默认文档目录路径
			Debug.WriteLine("默认文档目录: " + documentsPath);
			subDirectory = Path.Combine(documentsPath, "ResourceFolder");
			if(Directory.Exists(subDirectory))      //判断是否目录存在
			{
				Debug.WriteLine("子目录已存在: " + subDirectory);
			}
			else
			{
				Debug.WriteLine("子目录不存在，将创建: " + subDirectory);
				// 创建子目录
				try
				{
					Directory.CreateDirectory(subDirectory);
					Debug.WriteLine("子目录创建成功: " + subDirectory);
				}
				catch(Exception ex)
				{
					//窗口提示
					MessageBox.Show("创建子目录失败: " + ex.Message);
				}
			}

			using OpenFileDialog ofd = new()
			{
				//Filter = "媒体文件|*.mp4;*.avi;*.jpg;*.png;*.mp3;*.wav|所有文件|*.*"
				Multiselect = true,
				Filter = "媒体文件|*.mp4;*.avi;*.jpg;*.png;*.mp3;*.wav|视频文件|*.mp4;*.avi|音频文件|*.mp3;*.wav|图片文件|*.jpg;*.png",
				InitialDirectory = documentsPath
			};
			ofd.Title = "请选择要导入的音频、视频或图片文件";    //设置缺省文档目录
			ofd.InitialDirectory = subDirectory;  //指定初始目录 文件夹
												  //ofd.InitialDirectory =   Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if(ofd.ShowDialog() == DialogResult.OK)
			{
				foreach(string filePath in ofd.FileNames)
				{
					MediaType mediaType;
					// 判断媒体类型
					if(IsVideoFile(filePath))
						mediaType = MediaType.Video;
					else if(IsAudioFile(filePath))
						mediaType = MediaType.Audio;
					else if(IsImageFile(filePath))
						mediaType = MediaType.Image;
					else
						continue; // 不支持的格式

					// 创建媒体项控件并添加到 FlowLayoutPanel
					MediaItemControl mediaItem = new(filePath, mediaType);
					flowLayoutPanelMedia.Controls.Add(mediaItem);

					listBox1.Items.Add(mediaItem.MediaType);
					listBox1.Items.Add(mediaItem.FilePath);
					listBox1.Items.Add(mediaItem.Size);
					listBox1.Items.Add(mediaItem.Location.X);
					listBox1.Items.Add(mediaItem.Location.Y);
					//if(mediaType == MediaType.Video)
					//{

					//		listBox1.Items.Add(mediaItem.ImagePath);
					//		listBox1.Items.Add(mediaItem.TimeLength);
					//}
					//else if(mediaType == MediaType.Audio)
					//{
					//		listBox1.Items.Add(mediaItem.TimeLength);
					//}
				}
			}

			bool hasControls = flowLayoutPanelMedia.Controls.Count > 0;
			if(hasControls)
			{
				Fileupperleft.Visible = false;              //dG.Visible = false;
			}
			else
			{
				Fileupperleft.Visible = true;               //dG.Visible = false;
			}
		}

		private void Displayimage()
		{
			// 停止当前音频播放
			StopCurrentAudioIfPlaying();            //buttonX1.Visible = false;
			volumeControlPanel.Visible = false;
			pictureBox1.Visible = true;
			videoView1.Visible = false;
			dispVfilename.Visible = false;

			btnAudioPlay.Visible = false;
			btnAudioPause.Visible = false;
			btnAudioStop.Visible = false;
			audioPositionTrackBar.Visible = false;
			lblAudioTime.Visible = false;


			progressBar.Visible = false;

		}
		private void Displayvideo()
		{
			// 停止当前音频播放
			StopCurrentAudioIfPlaying();
			dispVfilename.Visible = false;
			//buttonX1.Visible = false;
			volumeControlPanel.Visible = true;
			pictureBox1.Visible = false;
			videoView1.Visible = true;

			btnAudioPlay.Visible = false;
			btnAudioPause.Visible = false;
			btnAudioStop.Visible = false;
			audioPositionTrackBar.Visible = false;
			lblAudioTime.Visible = false;

			progressBar.Visible = true;
		}
		private void DisplayAudio()  //音频播放
		{
			volumeControlPanel.Visible = false;
			pictureBox1.Visible = true;
			videoView1.Visible = false;
			dispVfilename.Visible = true;
			pictureBox1.Image = Properties.Resources.music;
			btnAudioPlay.Visible = true;
			btnAudioPause.Visible = true;
			btnAudioStop.Visible = true;
			audioPositionTrackBar.Visible = true;
			lblAudioTime.Visible = true;
			btnAudioPlay.Size = new Size(28, 28);
			btnAudioPlay.Location = new Point(50, 50);
			btnAudioPlay.BringToFront();

			progressBar.Visible = false;
		}
		/// <summary>
		/// 判断当前是否有音频播放，如果有则停止
		/// </summary>
		private void StopCurrentAudioIfPlaying()
		{
			try
			{
				// 检查音频播放器是否存在且正在播放
				if(_audioPlayer != null && _audioPlayer.IsPlaying)
				{
					// 停止音频播放
					_audioPlayer.Stop();

					// 停止进度更新定时器
					audioTimer?.Stop();

					// 更新UI控件状态
					UpdateAudioControls();
					ResetAudioProgress();
					HideAudioControls();

					System.Diagnostics.Debug.WriteLine("已停止当前音频播放");
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("当前没有音频在播放");
				}
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"停止音频播放时出错: {ex.Message}");
			}
		}
		// 辅助方法：判断文件类型
		private bool IsVideoFile(string filePath) =>
			new[] { ".mp4", ".avi", ".mkv" }.Contains(Path.GetExtension(filePath).ToLower());
		private bool IsAudioFile(string filePath) =>
			new[] { ".mp3", ".wav", ".flac" }.Contains(Path.GetExtension(filePath).ToLower());
		private bool IsImageFile(string filePath) =>
			new[] { ".jpg", ".png", ".bmp" }.Contains(Path.GetExtension(filePath).ToLower());
		// 修改 OnMediaPlayRequested 方法以支持完整的音频控制
		private void OnMediaPlayRequested(object sender, MediaPlayEventArgs e)
		{
			try
			{               // 在播放新内容之前，停止当前播放的任何媒体
				StopAllMedia(); // 或者使用 StopCurrentAudioIfPlaying();
				switch(e.MediaType)
				{
					case MediaType.Video:
						Displayvideo();
						filePath = e.FilePath;
						PlayVideo();
						HideAudioControls();
						break;

					case MediaType.Audio:
						PlayAudioWithControls(e.FilePath);
						break;

					case MediaType.Image:
						Displayimage();
						pictureBox1.Image = Image.FromFile(e.FilePath);
						HideAudioControls();
						break;
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show($"播放媒体文件时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		/// <summary>
		/// 停止当前播放的任何媒体（音频或视频）
		/// </summary>
		private void StopAllMedia()
		{
			try
			{
				// 停止音频播放
				if(_audioPlayer != null && (_audioPlayer.IsPlaying || _audioPlayer.IsPaused))
				{
					_audioPlayer.Stop();
					audioTimer?.Stop();
					UpdateAudioControls();
					ResetAudioProgress();
					HideAudioControls();
					System.Diagnostics.Debug.WriteLine("已停止音频播放");
				}
				// 停止视频播放
				if(mediaPlayer != null && (mediaPlayer.State == VLCState.Playing || mediaPlayer.State == VLCState.Paused))
				{
					mediaPlayer.Stop();
					progressTimer?.Stop();
					ResetProgress();
					playPauseButton.Image = Properties.Resources.start;
					System.Diagnostics.Debug.WriteLine("已停止视频播放");
				}

				// 重置播放状态
				IsfirstPlaying = false;
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"停止媒体播放时出错: {ex.Message}");
			}
		}
		private void PlayAudioWithControls(string filePath)     // 播放音频并显示控制界面
		{
			try
			{
				// 初始化 NAudio 播放器（如果尚未初始化）
				if(_audioPlayer == null)
				{
					_audioPlayer = new AudioPlayer();
					// 订阅事件
					_audioPlayer.PlaybackCompleted += (s, e) =>
					{
						this.Invoke(new Action(() =>
						{
							audioTimer.Stop();
							UpdateAudioControls();
							ResetAudioProgress();
							HideAudioControls();
						}));
					};
				}
				_audioPlayer.Stop();    // 停止当前播放
				DisplayAudio();  // 显示音频控制界面
				ShowAudioControls();
				_audioPlayer.Play(filePath);    // 播放音频
				dispVfilename.Text = $"正在播放: {Path.GetFileName(filePath)}";      // 更新UI
				UpdateAudioControls();
				ResetAudioProgress();

				// 启动进度更新定时器
				audioTimer.Start();
			}
			catch(Exception ex)
			{
				MessageBox.Show($"播放音频失败: {ex.Message}", "错误",
							   MessageBoxButtons.OK, MessageBoxIcon.Error);
				HideAudioControls();
			}
		}
	
		private void BtnAudioPlay_Click(object sender, EventArgs e)     // 音频控制按钮事件处理
		{
			if(_audioPlayer != null)
			{
				if(_audioPlayer.IsPaused)
				{
					_audioPlayer.Resume();
				}
				else if(_audioPlayer.IsStopped)
				{
					// 需要重新播放当前音频文件
					// 这里需要保存当前播放的文件路径
				}

				UpdateAudioControls();
			}
		}

		private void BtnAudioPause_Click(object sender, EventArgs e)
		{
			_audioPlayer?.Pause();
			UpdateAudioControls();
		}
		private void BtnAudioStop_Click(object sender, EventArgs e)
		{
			_audioPlayer?.Stop();
			audioTimer.Stop();
			UpdateAudioControls();
			ResetAudioProgress();
		}
		// 进度条滚动事件
		private void AudioPositionTrackBar_Scroll(object sender, EventArgs e)
		{
			if(_audioPlayer != null && _audioPlayer.GetTotalTime().TotalSeconds > 0)
			{
				double ratio = (double)audioPositionTrackBar.Value / audioPositionTrackBar.Maximum;
				TimeSpan newPosition = TimeSpan.FromSeconds(_audioPlayer.GetTotalTime().TotalSeconds * ratio);
				_audioPlayer.SetCurrentTime(newPosition);
			}
		}

		private void Modifythephoto_Click(object sender, EventArgs e)   // 修改图片
		{

		}
		
		private void AudioTimer_Tick(object sender, EventArgs e)   //// 定时器更新音频进度
		{
			if(_audioPlayer != null)  // 更新音频进度显示
			{
				TimeSpan currentTime = _audioPlayer.GetCurrentTime();
				TimeSpan totalTime = _audioPlayer.GetTotalTime();

				if(totalTime.TotalSeconds > 0)
				{
					// 更新进度条
					int progress = (int)((currentTime.TotalSeconds / totalTime.TotalSeconds) * audioPositionTrackBar.Maximum);
					audioPositionTrackBar.Value = Math.Max(0, Math.Min(audioPositionTrackBar.Maximum, progress));

					// 更新时间标签
					lblAudioTime.Text = $"{currentTime:mm\\:ss} / {totalTime:mm\\:ss}";
				}
			}
		}
		
		private void ResetAudioProgress()  // 重置音频进度显示
		{
			audioPositionTrackBar.Value = 0;
			lblAudioTime.Text = "00:00 / 00:00";
		}
		private void UpdateAudioControls()  // 更新音频控制按钮状态
		{
			if(_audioPlayer != null)
			{
				switch(_audioPlayer.CurrentState)
				{
					case AudioPlayer.PlaybackState.Playing:
						btnAudioPlay.Text = "播放";
						btnAudioPlay.Enabled = true;
						btnAudioPause.Text = "暂停";
						btnAudioPause.Enabled = true;
						btnAudioStop.Enabled = true;
						audioTimer.Start();
						break;
					case AudioPlayer.PlaybackState.Paused:
						btnAudioPlay.Text = "继续";
						btnAudioPlay.Enabled = true;
						btnAudioPause.Text = "暂停";
						btnAudioPause.Enabled = false;
						btnAudioStop.Enabled = true;
						audioTimer.Stop();
						break;
					case AudioPlayer.PlaybackState.Stopped:
						btnAudioPlay.Text = "播放";
						btnAudioPlay.Enabled = true;
						btnAudioPause.Text = "暂停";
						btnAudioPause.Enabled = false;
						btnAudioStop.Enabled = false;
						audioTimer.Stop();
						break;
				}
			}
		}
		// 显示音频控制界面
		private void ShowAudioControls()
		{
			if(btnAudioPlay != null)
				btnAudioPlay.Visible = true;
			if(btnAudioPause != null)
				btnAudioPause.Visible = true;
			if(btnAudioStop != null)
				btnAudioStop.Visible = true;
			if(audioPositionTrackBar != null)
				audioPositionTrackBar.Visible = true;
			if(lblAudioTime != null)
				lblAudioTime.Visible = true;
		}
		// 隐藏音频控制界面
		private void HideAudioControls()
		{
			if(btnAudioPlay != null)
				btnAudioPlay.Visible = false;
			if(btnAudioPause != null)
				btnAudioPause.Visible = false;
			if(btnAudioStop != null)
				btnAudioStop.Visible = false;
			if(audioPositionTrackBar != null)
				audioPositionTrackBar.Visible = false;
			if(lblAudioTime != null)
				lblAudioTime.Visible = false;
		}
		#endregion
	}
	#region ------------ calss  AudioPlayer  VideoInfo 属性类 获取  ------------
	public class AudioPlayer:IDisposable
	{
		private WaveOutEvent _waveOut;
		private AudioFileReader _audioReader;
		private string _currentFilePath;
		private bool _isPaused = false;
		private readonly object _syncLock = new object(); // 新增：线程同步锁

		public enum PlaybackState
		{
			Stopped,
			Playing,
			Paused
		}

		public PlaybackState CurrentState { get; private set; } = PlaybackState.Stopped;

		public event EventHandler PlaybackCompleted;
		public event EventHandler PlaybackStarted;
		public event EventHandler PlaybackPaused;
		public event EventHandler PlaybackResumed;

		public AudioPlayer()
		{
			CurrentState = PlaybackState.Stopped;
		}
		public void Play(string filePath)
		{
			try
			{
				lock(_syncLock) // 加锁：确保操作原子性
				{
					// 如果正在播放相同文件，则不做任何操作
					if(_currentFilePath == filePath && CurrentState == PlaybackState.Playing)
					{
						return;
					}

					Stop(); // 停止当前播放（内部已加锁）

					_audioReader = new AudioFileReader(filePath);
					_waveOut = new WaveOutEvent();
					_waveOut.Init(_audioReader);

					// 订阅播放完成事件（注意：事件触发线程可能非UI线程）
					_waveOut.PlaybackStopped += OnPlaybackStopped;

					_waveOut.Play();
					_currentFilePath = filePath;
					CurrentState = PlaybackState.Playing;
					_isPaused = false;

					PlaybackStarted?.Invoke(this, EventArgs.Empty);
				}
			}
			catch(Exception ex)
			{
				throw new Exception($"播放音频文件失败: {ex.Message}");
			}
		}
		public void Pause()
		{
			try
			{
				lock(_syncLock) // 加锁：避免与Stop/Play同时执行
				{
					if(_waveOut != null && CurrentState == PlaybackState.Playing)
					{
						_waveOut.Pause();
						CurrentState = PlaybackState.Paused;
						_isPaused = true;
						PlaybackPaused?.Invoke(this, EventArgs.Empty);
					}
				}
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"暂停音频播放时出错: {ex.Message}");
			}
		}
		public void Resume()
		{
			try
			{
				lock(_syncLock) // 保持线程同步锁
				{
					if(_waveOut != null && CurrentState == PlaybackState.Paused)
					{
						// 关键修改：用 Play() 替代 Resume()，NAudio 中 Play() 会从暂停处继续
						_waveOut.Play();
						CurrentState = PlaybackState.Playing;
						_isPaused = false;
						PlaybackResumed?.Invoke(this, EventArgs.Empty);
					}
				}
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"恢复音频播放时出错: {ex.Message}");
			}
		}

		public void Stop()
		{
			try
			{
				lock(_syncLock)
				{
					if(_waveOut != null)
					{
						_waveOut.PlaybackStopped -= OnPlaybackStopped; // 取消事件订阅
						_waveOut.Stop();
						_waveOut.Dispose();
						_waveOut = null;
					}

					if(_audioReader != null)
					{
						//_audioReader.Dispose();     //????????????????
						_audioReader = null;
					}

					_currentFilePath = null;
					CurrentState = PlaybackState.Stopped;
					_isPaused = false;
				}
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"停止音频播放时出错: {ex.Message}");
			}
		}
	
		private void OnPlaybackStopped(object sender, StoppedEventArgs e)
		{
			// 事件可能在非UI线程触发，通过锁同步调用Stop
			Stop(); // Stop内部已加锁，确保线程安全
			PlaybackCompleted?.Invoke(this, EventArgs.Empty);
		}
		public bool IsPlaying => CurrentState == PlaybackState.Playing;
		public bool IsPaused => CurrentState == PlaybackState.Paused;
		public bool IsStopped => CurrentState == PlaybackState.Stopped;
		public TimeSpan GetTotalTime()      // 获取音频总时长
		{
			lock(_syncLock) // 加锁：避免访问时资源已释放
			{
				return _audioReader?.TotalTime ?? TimeSpan.Zero;
			}
		}
		public TimeSpan GetCurrentTime()    // 获取当前播放位置
		{
			lock(_syncLock) // 加锁：避免访问时资源已释放
			{
				return _audioReader?.CurrentTime ?? TimeSpan.Zero;
			}
		}

		// 设置播放位置
		public void SetCurrentTime(TimeSpan time)
		{
			lock(_syncLock) // 加锁：避免设置时资源被释放
			{
				if(_audioReader != null)
				{
					_audioReader.CurrentTime = time;
				}
			}
		}
		public void Dispose()
		{
			Stop(); // 释放时调用Stop，内部已加锁
		}
	}
	public class VideoInfo
	{
		public string DurationSeconds
		{
			get; set;
		} // 时长（秒）
		public Image Thumbnail
		{
			get; set;
		} // 首帧图片
		public string FilePath
		{
			get; set;
		} 
	}
	#endregion

	#region ------------     ------------


	#endregion

}





