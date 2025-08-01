
#region ------------- 系统加载部分  无需改变的变量 -------------
using System;
using LibVLCSharp.WinForms;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media;
using LibVLCSharp.Shared;

using SharpCompress.Common;
using static MusicChange.db;
using Application = System.Windows.Application;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;
using Point = System.Drawing.Point;
//using DevComponents.DotNetBar;  //using LibVLCSharp.WinForms; 和 using LibVLCSharp.Shared;
using System.Reflection;

#endregion

#region  ------------- 全局变量 -------------
namespace MusicChange
{
	public partial class LaserEditing : Form
	{       // Windows API 函数
		[DllImport( "user32.dll" )]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
		[DllImport( "user32.dll" )]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
		// 快捷键ID
		private const int HOTKEY_ID = 1;
		// 修饰符常量
		private const uint MOD_ALT = 0x0001;
		private const uint MOD_CONTROL = 0x0002;
		private const uint MOD_SHIFT = 0x0004;
		// 鼠标状态变量
		private bool isDragging = false;
		private System.Drawing.Point dragStartPoint;
		private const int borderSize = 10;
		private FormWindowState previousWindowState;
		[DllImport( "user32.dll" )]
		public static extern bool ReleaseCapture( );
		[DllImport( "user32.dll" )]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		private const int WM_NCLBUTTONDOWN = 0xA1;
		private const int HTCAPTION = 0x2;
		private const int HT_CAPTION = 0x2;
		private bool splitContainer5mouseDown;
		int count = 0;
		bool IsOfficialMaterialSwitch = false; //官方素材开关
		bool Ismaterial = true;
		string importfile;  //imøport a file
		int gwidth, gheight, lwidth, lheight;// 获取屏幕工作区宽度
		#endregion
		private Assembly libVLCSharpWinFormsAssembly;
		private Type videoViewType;  //		private object videoViewInstance;
		private VideoView _videoView;
		private LibVLC _libVLC;
		private string filePath = $"F:\\newipad\\sex.MP4";
		private MediaPlayer _mediaPlayer;

		public LaserEditing( )
		{
			AutoScaleMode = AutoScaleMode.Dpi; // 根据系统DPI自动缩放
			InitializeComponent();
			this.DoubleBuffered = true;   //button2.FlatAppearance.BorderSize = 0; // 边框大小设为 0//qrcode1.FlatAppearance.BorderSize = 0;   // 边框大小设为 0

		}
		private void LaserEditing_Load(object sender, EventArgs e)
		{
			//splitContainer5mouseDown = false;			//splitContainer1.Panel2MinSize = 400;	//buttonx8.BackColor = System.Drawing.Color.Gray;
			Ismaterial = true;  // 默认选择当前素材
			buttonX3_Click( null, null ); // 设置当前素材按钮样式	this.ClientSize = new System.Drawing.Size( 1900, 1080 );
			splitContainer5mouseDown = false;
			OfficialMaterialSwitch(); // 初始化官方素材开关状态
			sC4.SplitterDistance = 500; //上中
			sC3.SplitterDistance = 750; // 上左 宽度
			gwidth = this.Width = Screen.PrimaryScreen.WorkingArea.Width; // 获取屏幕工作区宽度
			temp.Text = "屏幕工作区宽度: " + gwidth.ToString();
			temp1.Text = "屏幕工作区高度: " + Screen.PrimaryScreen.WorkingArea.Height.ToString();
			gheight = this.Height = Screen.PrimaryScreen.WorkingArea.Height; // 获取屏幕工作区高度
			this.Size = new System.Drawing.Size( 1550, 900 ); // 设置主窗口初始大小
			lwidth = this.Size.Width;//			lheight = this.Size.Height;
			int weight = lwidth / 3; // 设置窗口宽度  
			this.sC3.Panel1MinSize = 300;
			sC3.SplitterDistance = weight + 80; // 上左
			sC4.SplitterDistance = weight + 50; //上中			//LoadLibVLCSharpDynamically();
			InitializeLibVLC(); // 初始化 LibVLC
			//InitializeUIControls();
			

		}

		#region   ------------动态加载 LibVLCSharp.WinForms	 -----------------	
		private void LoadLibVLCSharpDynamically()
		{
			try
			{
				// 动态加载 LibVLCSharp.WinForms 程序集 D:\Documents\Visual Studio 2022\MusicChange
				//string libVLCSharpWinFormsPath = Path.Combine(Application.StartupPath, "LibVLCSharp.WinForms.dll");
				string libVLCSharpWinFormsPath = Path.Combine($"D:\\Documents\\Visual Studio 2022\\MusicChange", "LibVLCSharp.WinForms.dll");

				libVLCSharpWinFormsAssembly = Assembly.LoadFrom(libVLCSharpWinFormsPath);

				// 获取 VideoView 类型
				videoViewType = libVLCSharpWinFormsAssembly.GetType("LibVLCSharp.WinForms.VideoView");

				// 创建 VideoView 实例
				videoViewInstance = Activator.CreateInstance(videoViewType);

				// 设置 MediaPlayer 属性（如果需要）
				// 这需要使用反射来设置属性
			}
			catch(Exception ex)
			{
				MessageBox.Show($"动态加载 LibVLCSharp 失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region   ------------初始化 LibVLC 核心 播放视频文件 播放 继续 停止等	 -----------------	

		private void InitializeLibVLC( )
		{
			try {
				// 确保在 UI 线程上初始化
				if (InvokeRequired) {
					Invoke( new Action( InitializeLibVLC ) );
					return;
				}
				// 初始化 LibVLC 核心
				Core.Initialize();
				// 添加 VLC 界面选项
				string[] options = {
						"--intf", "dummy",           // 使用虚拟接口
						"--embedded-video",          // 嵌入式视频
						"--no-video-title-show",     // 不显示视频标题
						"--no-stats",                // 不显示统计信息
						"--no-sub-autodetect-file",  // 不自动检测字幕文件
						"--verbose=0"                // 设置日志级别
					  };
				// 创建 LibVLC 实例
				_libVLC = new LibVLC( options );
				// 检查 LibVLC 是否创建成功
				if (_libVLC == null) {
					throw new InvalidOperationException( "无法创建 LibVLC 实例" );
				}
				// 创建 MediaPlayer 实例
				_mediaPlayer = new MediaPlayer( _libVLC );
				// 检查 MediaPlayer 是否创建成功
				if (_mediaPlayer == null) {
					throw new InvalidOperationException( "无法创建 MediaPlayer 实例" );
				}
                //VideoView _videoView1 = videoViewType.GetProperty( "MediaPlayer" ).GetValue( videoViewInstance ) as VideoView;
				// 检查 VideoView 是否创建成功
				if (videoView1 == null) {
					throw new InvalidOperationException( "无法创建 VideoView 实例" );
				}
				// 设置 MediaPlayer 到 VideoView  		
				videoView1.MediaPlayer = _mediaPlayer;
			}
			catch (Exception ex) {
				MessageBox.Show( $"初始化 LibVLC 失败: {ex.Message}\n{ex.StackTrace}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
				// 清理已创建的资源
				CleanupResources();
			}
		}
		/// <summary>
		/// 播放视频文件
		/// </summary>
		/// <param name="filePath">视频文件路径</param>
		public void PlayVideo1(string filePath)
		{
			try {
				// 检查必要组件是否存在
				if (_libVLC == null || _mediaPlayer == null) {
					MessageBox.Show( "播放器未正确初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}

				if (string.IsNullOrEmpty( filePath )) {
					MessageBox.Show( "文件路径不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}
				// 停止当前播放
				_mediaPlayer.Stop();
				// 创建媒体对象
				using (var media = new Media( _libVLC, filePath, FromType.FromPath )) {                 // 在这里使用 media 对象
					_mediaPlayer.Play( media );
					if (media == null) {
						throw new InvalidOperationException( "无法创建媒体对象" );
					}
					// 播放媒体
					_mediaPlayer.Play( media );
				}
				// media 对象在这里自动释放  
			}
			catch (Exception ex) {
				MessageBox.Show( $"播放视频失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		/// <summary>
		/// 播放网络流
		/// </summary>
		/// <param name="streamUrl">流媒体URL</param>
		public void PlayStream(string streamUrl)
		{
			try {
				// 检查必要组件是否存在
				if (_libVLC == null || _mediaPlayer == null) {
					MessageBox.Show( "播放器未正确初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}
				if (string.IsNullOrEmpty( streamUrl )) {
					MessageBox.Show( "流媒体URL不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}
				// 停止当前播放
				_mediaPlayer.Stop();

				// 创建媒体对象
				//using var media = new Media(_libVLC, streamUrl, FromType.FromLocation);
				using (var media = new Media( _libVLC, filePath, FromType.FromPath )) {
					// 在这里使用 media 对象
					_mediaPlayer.Play( media );
					if (media == null) {
						throw new InvalidOperationException( "无法创建媒体对象" );
					}
					// 播放媒体
					_mediaPlayer.Play( media );
					//media.AddOption(":network-caching=500");
				}
			}
			catch (Exception ex) {
				MessageBox.Show( $"播放流媒体失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		/// <summary>
		/// 暂停播放
		/// </summary>
		public void Pause( )
		{
			try {
				if (_mediaPlayer != null) {
					_mediaPlayer.Pause();
				}
			}
			catch (Exception ex) {
				MessageBox.Show( $"暂停播放失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		/// <summary>
		/// 停止播放
		/// </summary>
		public void Stop( )
		{
			try {
				if (_mediaPlayer != null) {
					_mediaPlayer.Stop();
				}
			}
			catch (Exception ex) {
				MessageBox.Show( $"停止播放失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		/// <summary>
		/// 设置音量 (0-100)
		/// </summary>
		/// <param name="volume">音量值</param>
		public void SetVolume(int volume)
		{
			try {
				if (_mediaPlayer != null) {
					int clampedVolume = Math.Max( 0, Math.Min( 100, volume ) );
					_mediaPlayer.Volume = clampedVolume;
				}
			}
			catch (Exception ex) {
				MessageBox.Show( $"设置音量失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		/// <summary>
		/// 获取当前播放位置 (毫秒)
		/// </summary>
		/// <returns>播放位置</returns>
		public long GetTime( )
		{
			try {
				return _mediaPlayer?.Time ?? 0;
			}
			catch {
				return 0;
			}
		}

		/// <summary>
		/// 设置播放位置 (毫秒)
		/// </summary>
		/// <param name="time">时间</param>
		public void SetTime(long time)
		{
			try {
				if (_mediaPlayer != null) {
					_mediaPlayer.Time = time;
				}
			}
			catch (Exception ex) {
				MessageBox.Show( $"设置播放位置失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		/// <summary>
		/// 清理资源
		/// </summary>
		private void CleanupResources1( )
		{
			try {
				// 停止播放
				if (_mediaPlayer != null) {
					_mediaPlayer.Stop();
				}

				// 清理 MediaPlayer
				if (_mediaPlayer != null) {
					_mediaPlayer.Dispose();
					_mediaPlayer = null;
				}

				// 清理 LibVLC
				if (_libVLC != null) {
					_libVLC.Dispose();
					_libVLC = null;
				}

				// 清理 VideoView
				if (videoView1 != null) {
					videoView1.Dispose();
					videoView1 = null;
				}
			}
			catch (Exception ex) {
				// 记录错误但不中断程序
				Console.WriteLine( $"清理资源时出错: {ex.Message}" );
			}
		}

		//protected override void OnFormClosing(FormClosingEventArgs e)
		//{
		//	// 清理资源
		//	CleanupResources();
		//	base.OnFormClosing( e );
		//}

		#endregion

		#region   ------------ LibVLC  添加  播放 时间 音量 播放 继续 停止等	按钮 -----------------	

		//public partial class VideoPlayerWithProgressBar : Form
		//	{
		//private TrackBar _progressBar; // 进度条
		//private Timer _progressTimer; // 定时器更新进度
		//private Label _currentTimeLabel; // 当前时间标签
		//private Label _totalTimeLabel; // 总时间标签
		private bool _isSeeking = false; // 是否正在拖拽进度条
		// 订阅播放器事件
		//_mediaPlayer.TimeChanged += OnMediaPlayerTimeChanged;
		//_mediaPlayer.LengthChanged += OnMediaPlayerLengthChanged;
		//_mediaPlayer.Playing += OnMediaPlayerPlaying;
		//_mediaPlayer.Paused += OnMediaPlayerPaused;
		//_mediaPlayer.Stopped += OnMediaPlayerStopped;
		//_mediaPlayer.EndReached += OnMediaPlayerEndReached;

		private void InitializeUIControls( )
		{
			// 创建底部控制面板 Panel controlPanel = new Panel	{Height = 60,Dock = DockStyle.Bottom, BackColor = System.Drawing.Color.FromArgb( 50, 50, 50 )	};
			// 创建进度条			_progressBar = new TrackBar			{				Minimum = 0,				Maximum = 1000, // 使用1000作为最大值以获得更平滑的控制				Height = 30,				Top = 5,				Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top			};			progressBar.MouseUp += ProgressBar_MouseUp;			_progressBar.MouseDown += ProgressBar_MouseDown;			_progressBar.Scroll += ProgressBar_Scroll;

			// 创建当前时间标签			_currentTimeLabel = new Label			{				Text = "00:00:00",				ForeColor = System.Drawing.Color.White,			Left = 10,				Top = 35,				Width = 80,				TextAlign = ContentAlignment.MiddleLeft			};
			// 创建总时间标签			_totalTimeLabel = new Label			{				Text = "00:00:00",				ForeColor = System.Drawing.Color.White,			Anchor = AnchorStyles.Right,				//Right = 10,				Top = 35,				Width = 80,				TextAlign = ontentAlignment.MiddleRight			};

			// 创建播放/暂停按钮	Button playPauseButton = new Button			{				Text = "播放",				Width = 60,				Height = 25,			Left = 100,				Top = 32			};
		//	playPauseButton.Click += PlayPauseButton_Click;

			// 创建停止按钮
			Button stopButton = new Button
			{
				Text = "停止",
				Width = 60,
				Height = 25,
				Left = 170,
				Top = 32
			};
			stopButton.Click += StopButton_Click;

			// 添加控件到控制面板
			//controlPanel.Controls.Add( _progressBar );
			//controlPanel.Controls.Add( _currentTimeLabel );
			//controlPanel.Controls.Add( _totalTimeLabel );
			//controlPanel.Controls.Add( playPauseButton );
			//controlPanel.Controls.Add( stopButton );

			//// 添加控制面板到窗体
			//this.Controls.Add( controlPanel );
			_videoView.SendToBack();

			// 创建定时器更新进度
			_progressTimer = new Timer
			{
				Interval = 100 // 每100毫秒更新一次
			};
			_progressTimer.Tick += ProgressTimer_Tick;
			_progressTimer.Start();
		}

		// 播放器事件处理
		private void OnMediaPlayerTimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
		{
			// 进度更新在定时器中处理，避免频繁UI更新
		}

		private void OnMediaPlayerLengthChanged(object sender, MediaPlayerLengthChangedEventArgs e)
		{
			if (InvokeRequired) {
				Invoke( new Action( ( ) => UpdateTotalTime( e.Length ) ) );
			}
			else {
				UpdateTotalTime( e.Length );
			}
		}

		private void OnMediaPlayerPlaying(object sender, EventArgs e)
		{
			if (InvokeRequired) {
				Invoke( new Action( ( ) => {
					// 更新按钮状态
				} ) );
			}
		}

		private void OnMediaPlayerPaused(object sender, EventArgs e)
		{
			if (InvokeRequired) {
				Invoke( new Action( ( ) => {
					// 更新按钮状态
				} ) );
			}
		}

		private void OnMediaPlayerStopped(object sender, EventArgs e)
		{
			if (InvokeRequired) {
				Invoke( new Action( ( ) => {
					ResetProgress();
				} ) );
			}
			else {
				ResetProgress();
			}
		}

		private void OnMediaPlayerEndReached(object sender, EventArgs e)
		{
			if (InvokeRequired) {
				Invoke( new Action( ( ) => {
					ResetProgress();
				} ) );
			}
			else {
				ResetProgress();
			}
		}

		// 进度条事件处理
		private void _progressBar_MouseDown(object sender, MouseEventArgs e)
		{
			_isSeeking = true;
		}

		private void _progressBar_MouseUp(object sender, MouseEventArgs e)
		{
			if (_isSeeking && _mediaPlayer != null) {
				// 计算目标时间
				long totalTime = _mediaPlayer.Length;
				if (totalTime > 0) {
					long targetTime = (long)(_progressBar.Value * totalTime / 1000.0);
					_mediaPlayer.Time = targetTime;
				}
			}
			_isSeeking = false;
		}

		private void _progressBar_Scroll(object sender, EventArgs e)
		{
			// 拖拽时实时更新时间显示
			if (_mediaPlayer != null && _mediaPlayer.Length > 0) {
				long totalTime = _mediaPlayer.Length;
				long currentTime = (long)(_progressBar.Value * totalTime / 1000.0);
				_currentTimeLabel.Text = FormatTime( currentTime );
			}
		}

		// 定时器更新进度
		private void ProgressTimer_Tick(object sender, EventArgs e)
		{
			if (_mediaPlayer != null && !_isSeeking) {
				UpdateProgress();
			}
		}

		private void UpdateProgress( )
		{
			if (_mediaPlayer == null)
				return;

			long currentTime = _mediaPlayer.Time;
			long totalTime = _mediaPlayer.Length;

			if (totalTime > 0) {
				// 更新进度条
				int progressValue = (int)(currentTime * 1000 / totalTime);
				if (progressValue >= 0 && progressValue <= 1000) {
					_progressBar.Value = progressValue;
				}

				// 更新时间标签
				_currentTimeLabel.Text = FormatTime( currentTime );
			}
		}

		private void UpdateTotalTime(long totalTime)
		{
			_totalTimeLabel.Text = FormatTime( totalTime );
			if (totalTime > 0) {
				_progressBar.Maximum = 1000;
			}
		}

		private void ResetProgress( )
		{
			_progressBar.Value = 0;
			_currentTimeLabel.Text = "00:00:00";
		}

		// 格式化时间显示 (毫秒转为 HH:MM:SS)
		private string FormatTime(long milliseconds)
		{
			if (milliseconds <= 0)
				return "00:00:00";

			TimeSpan time = TimeSpan.FromMilliseconds( milliseconds );
			return $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
		}

		// 按钮事件处理
		private void playPauseButton_Click_1(object sender, EventArgs e)
		{
			if (_mediaPlayer == null)
				return;

			if (_mediaPlayer.State == VLCState.Playing) {
				_mediaPlayer.Pause();
				((Button)sender).Text = "播放";
			}
			else {
				_mediaPlayer.Play();
				((Button)sender).Text = "暂停";
			}
		}

		private void StopButton_Click(object sender, EventArgs e)
		{
			_mediaPlayer?.Stop();
		}

		// 播放视频文件
		public void PlayVideo(string filePath)
		{
			try {
				if (_libVLC == null || _mediaPlayer == null) {
					MessageBox.Show( "播放器未正确初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}

				if (string.IsNullOrEmpty( filePath )) {
					MessageBox.Show( "文件路径不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}

				_mediaPlayer.Stop();

				using (var media = new Media( _libVLC, filePath, FromType.FromPath )) {
					if (media == null) {
						throw new InvalidOperationException( "无法创建媒体对象" );
					}

					_mediaPlayer.Play( media );
				}
			}
			catch (Exception ex) {
				MessageBox.Show( $"播放视频失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		// 清理资源
		private void CleanupResources( )
		{
			try {
				_progressTimer?.Stop();

				if (_mediaPlayer != null) {
					_mediaPlayer.Stop();
					_mediaPlayer.TimeChanged -= OnMediaPlayerTimeChanged;
					_mediaPlayer.LengthChanged -= OnMediaPlayerLengthChanged;
					_mediaPlayer.Playing -= OnMediaPlayerPlaying;
					_mediaPlayer.Paused -= OnMediaPlayerPaused;
					_mediaPlayer.Stopped -= OnMediaPlayerStopped;
					_mediaPlayer.EndReached -= OnMediaPlayerEndReached;
					_mediaPlayer.Dispose();
					_mediaPlayer = null;
				}

				_libVLC?.Dispose();
				_libVLC = null;

				_videoView?.Dispose();
				_videoView = null;
			}
			catch (Exception ex) {
				Console.WriteLine( $"清理资源时出错: {ex.Message}" );
			}
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			CleanupResources();
			base.OnFormClosing( e );
		}

#endregion

		#region ----------- 鼠标拖动窗口和改变大小问题 快捷键  还没解决-------------------
		private void panelEx4_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) {
				ReleaseCapture();
				SendMessage( this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0 );
			}

		}

		private void LaserEditing_MouseDown(object sender, MouseEventArgs e)
		{
			splitContainer5mouseDown = true;
			if (e.Button == MouseButtons.Left) {
				// 释放鼠标捕获并发送消息以模拟拖动窗口
				ReleaseCapture();
				SendMessage( this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0 );
			}
			//// 只有在正常状态下才能拖动和调整大小
			//if(this.WindowState == FormWindowState.Normal)
			//{
			//	// 鼠标左键按下时开始拖动（排除边缘区域，避免与缩放冲突）
			if (e.Button == MouseButtons.Left) {  //&& !IsInResizeArea( e.Location )
				isDragging = true;
				dragStartPoint = new Point( e.X, e.Y );
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
			if (isDragging) {
				this.Location = new Point( this.Location.X + e.X - dragStartPoint.X, this.Location.Y + e.Y - dragStartPoint.Y );
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
			if (e.Button == MouseButtons.Left) {
				isDragging = false;
				//this.Cursor = Cursors.Default;
			}
		}

		private void LaserEditing_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			// 双击标题栏切换最大化/正常状态
			if (e.Button == MouseButtons.Left &&
				e.Y < 50) // 假设标题栏高度为50
			{
				ToggleMaximize();
			}
		}
		private void ToggleMaximize( )
		{
			if (this.WindowState == FormWindowState.Normal) {
				previousWindowState = FormWindowState.Normal;
				this.WindowState = FormWindowState.Maximized;
			}
			else {
				this.WindowState = previousWindowState;
			}
		}
		protected override void WndProc(ref Message m)
		{
			const int WM_NCHITTEST = 0x0084;
			const int HTCLIENT = 0x1;
			const int HTLEFT = 0xA;
			const int HTRIGHT = 0xB;
			const int HTTOP = 0xC;
			const int HTTOPLEFT = 0xD;
			const int HTTOPRIGHT = 0xE;
			const int HTBOTTOM = 0xF;
			const int HTBOTTOMLEFT = 0x10;
			const int HTBOTTOMRIGHT = 0x11;

			const int RESIZE_HANDLE_SIZE = 10; // 可调整大小的边缘宽度

			if (m.Msg == WM_NCHITTEST && this.WindowState == FormWindowState.Normal) {
				Point cursorPos = this.PointToClient( Cursor.Position );
				int hitTestResult = HTCLIENT;

				// 判断鼠标是否在窗体边缘
				if (cursorPos.X <= RESIZE_HANDLE_SIZE) {
					if (cursorPos.Y <= RESIZE_HANDLE_SIZE)
						hitTestResult = HTTOPLEFT; // 左上角
					else if (cursorPos.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
						hitTestResult = HTBOTTOMLEFT; // 左下角
					else
						hitTestResult = HTLEFT; // 左边
				}
				else if (cursorPos.X >= this.ClientSize.Width - RESIZE_HANDLE_SIZE) {
					if (cursorPos.Y <= RESIZE_HANDLE_SIZE)
						hitTestResult = HTTOPRIGHT; // 右上角
					else if (cursorPos.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
						hitTestResult = HTBOTTOMRIGHT; // 右下角
					else
						hitTestResult = HTRIGHT; // 右边
				}
				else if (cursorPos.Y <= RESIZE_HANDLE_SIZE)
					hitTestResult = HTTOP; // 上边
				else if (cursorPos.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
					hitTestResult = HTBOTTOM; // 下边

				m.Result = (IntPtr)hitTestResult;
				return;
			}

			base.WndProc( ref m );
		}

		// 处理快捷键
		private void LaserEditing_KeyDown(object sender, KeyEventArgs e)
		{
			Keys key = e.KeyCode;
			//  if (e.Control != true)//如果没按Ctrl键      return;
			switch (key) {
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

		private void LaserEditing_Resize(object sender, EventArgs e)
		{
			splitContainer1.Panel2MinSize = 400;
		}

		private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void 修改图片_Click(object sender, EventArgs e)
		{
			ChangePictuer form = new ChangePictuer();
			form.Show();

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
			if (WindowState == FormWindowState.Maximized) {
				WindowState = FormWindowState.Normal; // 恢复到正常状态
			}
			else {
				WindowState = FormWindowState.Maximized; // 最大化窗口
			}

		}

		private void button42_Click(object sender, EventArgs e)
		{           //minimize
			if (WindowState == FormWindowState.Minimized) {
				WindowState = FormWindowState.Normal; // 恢复到正常状态
			}
			else {
				WindowState = FormWindowState.Minimized; // 最小化窗口
			}
		}

		private void buttonx6_Click(object sender, EventArgs e)
		{       //show cut 
			Cut form = new Cut();
			form.Show();
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
		//if (this.WindowState == FormWindowState.Maximized) {
		//	this.Cursor = Cursors.Default;
		//	return;
		//}

		//// 右下角
		//if (point.X >= this.ClientSize.Width - borderSize &&
		//	point.Y >= this.ClientSize.Height - borderSize) {
		//	this.Cursor = Cursors.SizeNWSE;
		//}
		//// 左下角
		//else if (point.X <= borderSize &&
		//		 point.Y >= this.ClientSize.Height - borderSize) {
		//	this.Cursor = Cursors.SizeNESW;
		//}
		//// 右上角
		//else if (point.X >= this.ClientSize.Width - borderSize &&
		//		 point.Y <= borderSize) {
		//	this.Cursor = Cursors.SizeNESW;
		//}
		//// 左上角
		//else if (point.X <= borderSize &&
		//		 point.Y <= borderSize) {
		//	this.Cursor = Cursors.SizeNWSE;
		//}
		//// 左边框
		//else if (point.X <= borderSize) {
		//	this.Cursor = Cursors.SizeWE;
		//}
		//// 右边框
		//else if (point.X >= this.ClientSize.Width - borderSize) {
		//	this.Cursor = Cursors.SizeWE;
		//}
		//// 上边框
		//else if (point.Y <= borderSize) {
		//	this.Cursor = Cursors.SizeNS;
		//}
		//// 下边框
		//else if (point.Y >= this.ClientSize.Height - borderSize) {
		//	this.Cursor = Cursors.SizeNS;
		//}
		//// 窗体内部
		//else {
		//	this.Cursor = Cursors.Default;
		//}
		//}

		private void splitContainer6_MouseMove(object sender, MouseEventArgs e)
		{
			//SetCursorBasedOnPosition( e.Location );
		}
		private void panelEx1_MouseMove(object sender, MouseEventArgs e)
		{
			SetCursorBasedOnPosition( e.Location );
		}

		/// <summary>
		/// QRCode 二维码 
		/// </summary>
		private void buttonX12_Click(object sender, EventArgs e)
		{
			// 显示二维码生成界面
			QRCode form = new QRCode();
			form.Show();
			// 隐藏当前界面			this.Hide();

		}
		#endregion

		#region ----------  素材  Material  -------------------
		private void buttonX3_Click(object sender, EventArgs e)  //当前选择素材
		{
			Ismaterial = true;
			AllGray();
			//this.buttonX3.BackColor = System.Drawing.Color.Black;
			//this.buttonX3.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat;
			this.buttonX3.SymbolColor = System.Drawing.Color.GreenYellow;
			//this.buttonX3.ThemeAware = true;  //这个属性很可能用于让按钮能够感知并自动适应应用程序的主题变化。当主题（如浅色 / 深色模式）发生改变时，设置为 ThemeAware=true 的控件会自动更新其外观（如颜色、样式等）以匹配当前主题，而无需手动编写额外的主题切换代码。
			buttonx8.Visible = true;
			buttonx4.Visible = true;
			buttonx5.Visible = true;
			this.buttonx8.BackColor = System.Drawing.Color.GreenYellow;


		}
		private void buttonX2_Click(object sender, EventArgs e) //video 音频
		{
			Ismaterial = false;
			AllGray();
			this.buttonX2.SymbolColor = System.Drawing.Color.YellowGreen;
			buttonx8.Visible = false;
			buttonx4.Visible = false;
			buttonx5.Visible = false;
		}
		private void AllGray( )
		{
			this.buttonX3.SymbolColor = System.Drawing.Color.Gray;
			this.buttonX2.SymbolColor = System.Drawing.Color.Gray;
		}


		//官方素材 开关
		private void OfficialMaterialSwitch( )
		{
			if (IsOfficialMaterialSwitch) {
				button1.Visible = true;
			}
			else {
				button1.Visible = false;
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
		//导入素材  Importing the materials
		private void buttonx7_Click(object sender, EventArgs e)
		{
			panel4.Visible = true;
			button2.Visible = true;
		}

		private void sC3_SplitterMoved(object sender, SplitterEventArgs e)
		{

		}

		#endregion

		#region ------------  导入：视频、音频、图片  -------------
		/// <summary>
		/// 导入：视频、音频、图片
		/// </summary>
		private void button2_Click(object sender, EventArgs e)
		{
			//button2.Visible = false;
			//qrcode1.Visible = false;
			panel4.Visible = false;
			// 获取默认文档目录路径
			string documentsPath = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
			temp.Text = "默认文档目录: " + documentsPath;
			string subDirectory = Path.Combine( documentsPath, "ResourceFolder" );
			//判断是否目录存在
			if (Directory.Exists( subDirectory )) {
				temp.Text = "子目录已存在: " + subDirectory;
			}
			else {
				temp1.Text = "子目录不存在，将创建: " + subDirectory;
				// 创建子目录
				try {
					Directory.CreateDirectory( subDirectory );
					temp1.Text = "子目录创建成功: " + subDirectory;
				}
				catch (Exception ex) {
					temp1.Text = "创建子目录失败: " + ex.Message;
				}
			}
			//选择目录
			OpenFileDialog ofd = new OpenFileDialog();
			//所有音频文件
			ofd.Title = "请选择要导入的音频、视频或图片文件";
			//设置缺省文档目录
			ofd.InitialDirectory = subDirectory;            //ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); //默认打开音乐文件夹
			ofd.Multiselect = true; //允许多选
			ofd.Filter = "素材|*.mp3;*.wav;*.wma;*.flac;*.aac;*.ogg;*.mp4;*.avi;*.wmv;*.mov;*.jpg;*.jpeg;*.png;*.bmp;*.gif";
			if (ofd.ShowDialog() == DialogResult.OK) { //获取所有的选中文件存入string数组
				string[] selectedFiles = ofd.FileNames;
			}
			if (ofd.FileNames.Length > 0) {
				// 遍历选中的文件
				foreach (string file in ofd.FileNames) {
					try {
						// 获取文件名
						string fileName = Path.GetFileName( file );
						// 构建目标路径
						string targetPath = Path.Combine( subDirectory, fileName );
						// 复制文件到目标路径
						File.Copy( file, targetPath, true ); // true表示覆盖同名文件
						temp1.Text += $"\n已导入: {fileName}";
					}
					catch (Exception ex) {
						temp1.Text += $"\n导入失败: {ex.Message}";
					}
				}
			}
			else {
				temp1.Text = "未选择任何文件";
				//button2.Visible = true;				qrcode1.Visible = true;
				panel4.Visible = true;

			}

		}

		private void button5_Click(object sender, EventArgs e)
		{
			Stop();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			Stop();
		}

		private void playPauseButton_Click_1(object sender, EventArgs e)
		{

		}



		/*// 递归删除子目录及其所有内容
try
{
if (Directory.Exists(subDirectory))
{
Directory.Delete(subDirectory, recursive: true); // recursive: true 表示删除所有子文件和子目录
MessageBox.Show("子目录删除成功");
}
}
catch (Exception ex)
{
MessageBox.Show("删除子目录失败: " + ex.Message);
}*/
		private void panel4_SizeChanged(object sender, EventArgs e)  // 导入 。。dynamic resize
		{
			if (panel4.Visible) {
				// 调整按钮和二维码的位置
				button2.Left = (panel4.Width - button2.Width) / 2 - 20; // 水平居中
				button2.Top = (panel4.Height - button2.Height) / 2; // 垂直居中
				qrcode1.Left = button2.Left + 290; // 水平居中	
				qrcode1.Top = (panel4.Height - qrcode1.Height) / 2; // 水平居中	
			}
		}
		//{
		//	// 调整按钮位置
		//	button2.Left = (panel4.Width - button2.Width) / 2; // 水平居中
		//	button2.Top = (panel4.Height - button2.Height) / 2; // 垂直居中
		//	qrcode1.Left = button2.Left + 250; // 水平居中	

		//}
		#endregion

		#region ------------  全局设置  -------------
		private void buttonx11_Click(object sender, EventArgs e)
		{
			//	db dr = new db( dbPath );			dr.dbinit();
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "视频文件|*.mp4;*.avi;*.wmv;*.mov|所有文件|*.*";
			if (ofd.ShowDialog() == DialogResult.OK) {
				temp.Text = ofd.FileName;
			}
			//_mediaPlayer.Mute = true; // 静音					  //_mediaPlayer.uiMode = "full"; // 或 "mini"
			PlayVideo( temp.Text );


		}
		private void PlayVideo2(string filePath)
		{
			if (File.Exists( filePath )) {
				var media = new Media( _libVLC, new Uri( filePath ) );
				_mediaPlayer.Play( media );
			}
			else {
				MessageBox.Show( "视频文件不存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		#endregion

	}

} //class LaserEditing









