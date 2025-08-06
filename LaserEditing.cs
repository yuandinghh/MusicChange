
#region ------------- 系统加载部分  无需改变的变量 -------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using NAudio.Wave;
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
		private const int HTCAPTION = 0x2;
		//private const int HT_CAPTION = 0x2;
		private bool splitContainer5mouseDown;
		int count = 0;
		bool IsOfficialMaterialSwitch = false; //官方素材开关
		bool Ismaterial = true;
		string importfile;  //imøport a file
		int gwidth, gheight, lwidth, lheight;// 获取屏幕工作区宽度
		#endregion
		private Assembly libVLCSharpWinFormsAssembly;
		private Type videoViewType;  //				 		private object videoViewInstance;
		private VideoView _videoView;
		private LibVLC _libVLC;
		private string filePath = "F:\\newipad\\跳舞3_m.MP4";
		private MediaPlayer _mediaPlayer;
		bool IsfirstPlaying = false;  //show first play
		private bool _isMuted = false;
		private int _previousVolume = 50;
		private BufferedWaveProvider waveProvider;
		private WaveOutEvent waveOut;
		private float _currentZoomFactor = 1.0f;
		private const float ZOOM_INCREMENT = 0.1f;
		private const float MIN_ZOOM = 0.5f;
		private const float MAX_ZOOM = 3.0f;
		public LaserEditing()
		{
			AutoScaleMode = AutoScaleMode.Dpi; // 根据系统DPI自动缩放
			InitializeComponent();
			IsfirstPlaying = false;
			this.DoubleBuffered = true;   //button2.FlatAppearance.BorderSize = 0; // 边框大小设为 0//qrcode1.FlatAppearance.BorderSize = 0;   // 边框大小设为 0

		}
		private void LaserEditing_Load(object sender, EventArgs e)
		{
			//splitContainer5mouseDown = false;			//splitContainer1.Panel2MinSize = 400;	//buttonx8.BackColor = System.Drawing.Color.Gray;
			Ismaterial = true;  // 默认选择当前素材
			buttonX3_Click(null, null); // 设置当前素材按钮样式	this.ClientSize = new System.Drawing.Size( 1900, 1080 );
			splitContainer5mouseDown = false;
			OfficialMaterialSwitch(); // 初始化官方素材开关状态
			gwidth = this.Width = Screen.PrimaryScreen.WorkingArea.Width; // 获取屏幕工作区宽度
			temp.Text = "屏幕工作区宽度: " + gwidth.ToString();
			temp.Text = filePath;
			temp1.Text = "屏幕工作区高度: " + Screen.PrimaryScreen.WorkingArea.Height.ToString();
			gheight = this.Height = Screen.PrimaryScreen.WorkingArea.Height; // 获取屏幕工作区高度
			this.Size = new System.Drawing.Size(1550, 900); // 设置主窗口初始大小
			lwidth = this.Size.Width;//			lheight = this.Size.Height;
			int weight = lwidth / 3; // 设置窗口宽度  
			this.sC3.Panel1MinSize = 300;
			sC3.SplitterDistance = weight + 80; // 上左
			sC4.SplitterDistance = weight+60; //上中			//LoadLibVLCSharpDynamically();  动态加载 LibVLCSharp.WinForms.dll
			InitializeLibVLC(); // 初始化 LibVLC
			InitializeUIControls();
			_mediaPlayer.TimeChanged += OnMediaPlayerTimeChanged;
			_mediaPlayer.LengthChanged += OnMediaPlayerLengthChanged;
			_mediaPlayer.Playing += OnMediaPlayerPlaying;
			_mediaPlayer.Paused += OnMediaPlayerPaused;
			_mediaPlayer.Stopped += OnMediaPlayerStopped;
			_mediaPlayer.EndReached += OnMediaPlayerEndReached;
	

	}

	#region   ------------初始化 LibVLC 核心 播放视频文件 播放 继续 停止等	 -----------------	
	/*要从视频中获取音频的左右声道数值并显示，LibVLC 本身并不直接提供左右声道音量的实时数据接口。但可以通过以下方法实现一个近似的解决方案：
实现步骤：
1.	使用 LibVLC 的音频回调功能：
•	LibVLC 提供了 AudioCallbacks，可以通过它获取音频数据。
•	通过分析音频数据，可以计算左右声道的音量值。
2.	计算左右声道音量：
•	音频数据通常是 PCM 格式，可以通过解析 PCM 数据计算左右声道的音量。
•	计算方法是对每个声道的采样值取绝对值的平均值或 RMS（均方根值）。
3.	显示左右声道音量：
•	使用 UI 控件（如 ProgressBar 或自定义绘制）显示左右声道的音量。
示例代码：
以下是一个实现左右声道音量显示的示例：  		*/
	// 音频回调函数

	//private void OnAudioPlay(IntPtr data, IntPtr samples, uint count, long pts, short[] audioData)
	//{

	//	private void OnAudioPlay(IntPtr data, IntPtr samples, uint count, long pts)
	//	{
	//	//	throw new NotImplementedException();
	//	//}
	//	// 获取音频样本数据
	//	audioData = new short[count];
	//	Marshal.Copy(samples, audioData, 0, audioData.Length);
	//          //waveProvider.AddSamples(audioData);
	//	waveProvider.AddSamples( audioData, 0, audioData.Length );

	//	// 假设音频是立体声（左右声道交替）
	//	var leftChannel = audioData.Where((_, index) => index % 2 == 0).ToArray();
	//	var rightChannel = audioData.Where((_, index) => index % 2 != 0).ToArray();

	//	// 计算左右声道音量（RMS）
	//	var leftVolume = CalculateRMS(leftChannel);
	//	var rightVolume = CalculateRMS(rightChannel);

	//	if(this.IsHandleCreated)
	//	{
	//		BeginInvoke(new Action(() =>
	//	  {
	//		  leftChannelProgressBar.Value = Math.Min((int)leftVolume, 100);
	//		  rightChannelProgressBar.Value = Math.Min((int)rightVolume, 100);
	//	  }));
	//	}
	//}
	// 修复音频回调方法签名 - 确保所有回调方法都有正确的参数签名
	// 完整修复的音频回调方法
	// 完整实现的 OnAudioPlay 方法 - 确保没有任何未实现的部分
	private void OnAudioPlay(IntPtr data, IntPtr samples, uint count, long pts)
	{
		// 不要抛出任何异常，而是静默处理所有情况
		try
		{
			// 参数验证
			if(samples == IntPtr.Zero || count == 0)
			{
				return; // 静默返回，不抛出异常
			}

			// 确保必要的组件存在
			if(waveProvider == null)
			{
				return;
			}

			// 安全地处理音频数据
			short[] audioData = null;
			try
			{
				audioData = new short[count];
				Marshal.Copy(samples, audioData, 0, (int)count);
			}
			catch
			{
				// 如果复制失败，静默返回
				return;
			}

			// 添加音频数据到缓冲区
			try
			{
				if(audioData != null && audioData.Length > 0)
				{
					// 转换 short[] 到 byte[]
					byte[] byteData = new byte[audioData.Length * 2];
					Buffer.BlockCopy(audioData, 0, byteData, 0, byteData.Length);
					waveProvider.AddSamples(byteData, 0, byteData.Length);
				}
			}
			catch
			{
				// 静默处理缓冲区添加错误
			}

			// 更新声道显示
			try
			{
				if(audioData != null && audioData.Length > 0)
				{
					UpdateChannelDisplayFromAudioData(audioData);
				}
			}
			catch
			{
				// 静默处理显示更新错误
			}
		}
		catch
		{
			// 捕获所有异常，确保不会传播到 LibVLC
			// 这是关键：音频回调中绝不能抛出异常
		}
	}
	// 完整实现的 UpdateChannelDisplayFromAudioData 方法
	private void UpdateChannelDisplayFromAudioData(short[] audioData)
	{
		try
		{
			if(audioData == null || audioData.Length == 0)
				return;

			if(leftChannelProgressBar == null || rightChannelProgressBar == null)
				return;

			// 限制处理的数据量以提高性能
			int processCount = Math.Min(audioData.Length, 100);

			// 高效计算左右声道音量
			long leftSum = 0;
			long rightSum = 0;
			int leftSamples = 0;
			int rightSamples = 0;

			for(int i = 0 ;i < processCount - 1 ;i += 2)
			{
				leftSum += Math.Abs((long)audioData[i]);
				leftSamples++;

				if(i + 1 < audioData.Length)
				{
					rightSum += Math.Abs((long)audioData[i + 1]);
					rightSamples++;
				}
			}

			// 计算平均值并转换为显示值
			int leftVolume = 0;
			int rightVolume = 0;

			if(leftSamples > 0)
			{
				long leftAverage = leftSum / leftSamples;
				leftVolume = (int)Math.Min(1, leftAverage / 100);
			}

			if(rightSamples > 0)
			{
				long rightAverage = rightSum / rightSamples;
				rightVolume = (int)Math.Min(1, rightAverage / 100);
			}

			// 安全更新UI
			SafeUpdateChannelDisplay(leftVolume, rightVolume);
		}
		catch
		{
			// 静默处理所有异常
		}
	}
	// 安全更新声道显示的方法
	private void SafeUpdateChannelDisplay(int leftVolume, int rightVolume)
	{
		try
		{
			// 检查控件状态
			if(this.IsDisposed || !this.IsHandleCreated)
				return;

			// 使用 BeginInvoke 异步更新UI
			this.BeginInvoke(new Action(() =>
			{
				try
				{
						// 更新左声道显示
						if(leftChannelProgressBar != null &&
						!leftChannelProgressBar.IsDisposed &&
						leftChannelProgressBar.IsHandleCreated)
					{
						int value = Math.Max(0, Math.Min(100, leftVolume));
						if(value != leftChannelProgressBar.Value)
						{
							leftChannelProgressBar.Value = value;
						}
					}

						// 更新右声道显示
						if(rightChannelProgressBar != null &&
						!rightChannelProgressBar.IsDisposed &&
						rightChannelProgressBar.IsHandleCreated)
					{
						int value = Math.Max(0, Math.Min(100, rightVolume));
						if(value != rightChannelProgressBar.Value)
						{
							rightChannelProgressBar.Value = value;
						}
					}
				}
				catch
				{
						// 静默处理UI更新异常
					}
			}));
		}
		catch
		{
			// 静默处理调度异常
		}
	}
	// 确保所有其他音频回调方法也都完整实现
	private void OnAudioPause(IntPtr data, long pts)
	{
		try
		{
			if(waveOut != null && waveOut.PlaybackState == PlaybackState.Playing)
			{
				waveOut.Pause();
			}
		}
		catch
		{
			// 静默处理所有异常
		}
	}
	private void OnAudioResume(IntPtr data, long pts)
	{
		try
		{
			if(waveOut != null && waveOut.PlaybackState == PlaybackState.Paused)
			{
				waveOut.Play();
			}
		}
		catch
		{
			// 静默处理所有异常
		}
	}
	private void OnAudioFlush(IntPtr data, long pts)
	{
		try
		{
			if(waveProvider != null)
			{
				waveProvider.ClearBuffer();
			}
		}
		catch
		{
			// 静默处理所有异常
		}
	}
	private void OnAudioDrain(IntPtr data)
	{
		try
		{
			if(waveOut != null)
			{
				waveOut.Stop();
			}
		}
		catch
		{
			// 静默处理所有异常
		}
	}
	// 检查您的 InitializeLibVLC 方法确保正确设置
	private void InitializeLibVLC()
	{
		try
		{
			// 初始化 LibVLC 核心
			Core.Initialize();

			// 创建 LibVLC 实例
			_libVLC = new LibVLC();  //启用硬件加速：  "--avcodec-hw=dxva2"

			if(_libVLC == null)
			{
				return;
			}

			// 创建 MediaPlayer 实例
			_mediaPlayer = new MediaPlayer(_libVLC);

			if(_mediaPlayer == null)
			{
				return;
			}

			// 设置 VideoView
			if(videoView1 != null)
			{
				videoView1.MediaPlayer = _mediaPlayer;
			}

			// 初始化 NAudio 组件
			try
			{
				waveProvider = new BufferedWaveProvider(new WaveFormat(44100, 16, 2))
				{
					BufferDuration = TimeSpan.FromMilliseconds(500),
					DiscardOnBufferOverflow = true
				};

				waveOut = new WaveOutEvent();
				waveOut.Init(waveProvider);
				// 注意：不要在这里启动播放，而是在实际需要播放时启动
			}
			catch
			{
				// 如果 NAudio 初始化失败，继续运行但不提供音频可视化
				waveProvider = null;
				waveOut = null;
			}

			// 设置音频回调 - 确保所有回调都有实现
			//_mediaPlayer.SetAudioCallbacks(
			//	OnAudioPlay,    // 必须有完整实现
			//	OnAudioPause,   // 必须有完整实现
			//	OnAudioResume,  // 必须有完整实现
			//	OnAudioFlush,   // 必须有完整实现
			//	OnAudioDrain    // 必须有完整实现
			//);
		}
		catch
		{
			// 静默处理初始化异常
			CleanupResources();
		}
	}
	// 安全更新声道显示UI
	private void UpdateChannelDisplayUI(int leftVolume, int rightVolume)
	{
		try
		{
			// 检查窗体状态
			if(this.IsDisposed || !this.IsHandleCreated)
				return;

			// 使用 BeginInvoke 异步更新UI
			this.BeginInvoke(new Action(() =>
			{
				try
				{
						// 更新左声道进度条
						if(leftChannelProgressBar != null &&
						!leftChannelProgressBar.IsDisposed &&
						leftChannelProgressBar.IsHandleCreated)
					{
						leftChannelProgressBar.Value = Math.Max(0, Math.Min(leftChannelProgressBar.Maximum, leftVolume));
					}

						// 更新右声道进度条
						if(rightChannelProgressBar != null &&
						!rightChannelProgressBar.IsDisposed &&
						rightChannelProgressBar.IsHandleCreated)
					{
						rightChannelProgressBar.Value = Math.Max(0, Math.Min(rightChannelProgressBar.Maximum, rightVolume));
					}
				}
				catch(Exception uiEx)
				{
					if(System.Diagnostics.Debugger.IsAttached)
					{
						System.Diagnostics.Debug.WriteLine($"更新声道显示UI时出错: {uiEx.Message}");
					}
				}
			}));
		}
		catch(Exception ex)
		{
			if(System.Diagnostics.Debugger.IsAttached)
			{
				System.Diagnostics.Debug.WriteLine($"调度UI更新时出错: {ex.Message}");
			}
		}
	}
	// 修复播放视频方法
	public void PlayVideo()
	{
		try
		{
			if(_libVLC == null || _mediaPlayer == null)
			{
				MessageBox.Show("播放器未正确初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if(string.IsNullOrEmpty(filePath))
			{
				MessageBox.Show("文件路径不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// 确保 NAudio 组件已启动
			if(waveOut != null && waveOut.PlaybackState != PlaybackState.Playing)
			{
				waveOut.Play();
			}

			_mediaPlayer.Stop();

				using var media = new Media(_libVLC, filePath, FromType.FromPath);
				if(media == null)
				{
					throw new InvalidOperationException("无法创建媒体对象");
				}

				progressTimer.Start();
				//_mediaPlayer.bringToF
				//_videoView.BringToFront();
				_mediaPlayer.Play(media);
			}
		catch(Exception ex)
		{
			MessageBox.Show($"播放视频失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
	private void InitializeLibVLC5()
	{
		try
		{
			// 初始化 LibVLC 核心
			Core.Initialize();

			// 创建 LibVLC 实例
			_libVLC = new LibVLC();

			// 检查 LibVLC 是否创建成功
			if(_libVLC == null)
			{
				throw new InvalidOperationException("无法创建 LibVLC 实例");
			}

			// 创建 MediaPlayer 实例
			_mediaPlayer = new MediaPlayer(_libVLC);

			// 检查 MediaPlayer 是否创建成功
			if(_mediaPlayer == null)
			{
				throw new InvalidOperationException("无法创建 MediaPlayer 实例");
			}

			// 检查 VideoView 是否创建成功
			if(videoView1 == null)
			{
				throw new InvalidOperationException("无法创建 VideoView 实例");
			}

			// 设置 MediaPlayer 到 VideoView  
			videoView1.MediaPlayer = _mediaPlayer;

			// 初始化 NAudio 组件
			waveProvider = new BufferedWaveProvider(new WaveFormat(44100, 16, 2)) // 44.1kHz, 16位, 立体声
			{
				BufferDuration = TimeSpan.FromSeconds(2) // 设置2秒缓冲区
			};

			waveOut = new WaveOutEvent();
			waveOut.Init(waveProvider);
			waveOut.Play(); // 提前启动播放器

			// 设置音频回调（必须在播放前设置）
			// 确保传递所有5个回调方法
			_mediaPlayer.SetAudioCallbacks(
				OnAudioPlay,    // play callback    (IntPtr data, IntPtr samples, uint count, long pts)
				OnAudioPause,   // pause callback   (IntPtr data, long pts)
				OnAudioResume,  // resume callback  (IntPtr data, long pts)
				OnAudioFlush,   // flush callback   (IntPtr data, long pts)
				OnAudioDrain    // drain callback   (IntPtr data)
			);
		}
		catch(Exception ex)
		{
			MessageBox.Show($"初始化 LibVLC 失败: {ex.Message}\n{ex.StackTrace}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			CleanupResources();
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
	private void InitializeLibVLC1()
	{
		try
		{
			// 初始化 LibVLC 核心
			Core.Initialize();

			// 创建 LibVLC 实例
			_libVLC = new LibVLC();

			// 检查 LibVLC 是否创建成功
			if(_libVLC == null)
			{
				throw new InvalidOperationException("无法创建 LibVLC 实例");
			}

			// 创建 MediaPlayer 实例
			_mediaPlayer = new MediaPlayer(_libVLC);

			// 检查 MediaPlayer 是否创建成功
			if(_mediaPlayer == null)
			{
				throw new InvalidOperationException("无法创建 MediaPlayer 实例");
			}

			// 检查 VideoView 是否创建成功
			if(videoView1 == null)
			{
				throw new InvalidOperationException("无法创建 VideoView 实例");
			}

			// 设置 MediaPlayer 到 VideoView  
			videoView1.MediaPlayer = _mediaPlayer;

			// 初始化 NAudio 组件
			waveProvider = new BufferedWaveProvider(new WaveFormat(44100, 16, 2)) // 44.1kHz, 16位, 立体声
			{
				BufferDuration = TimeSpan.FromSeconds(2) // 设置2秒缓冲区
			};

			waveOut = new WaveOutEvent();
			waveOut.Init(waveProvider);

			// 设置音频回调（必须在播放前设置）
			_mediaPlayer.SetAudioCallbacks(
				OnAudioPlay,    // play callback
				OnAudioPause,   // pause callback
				OnAudioResume,  // resume callback
				OnAudioFlush,   // flush callback
				OnAudioDrain    // drain callback
			);
		}
		catch(Exception ex)
		{
			MessageBox.Show($"初始化 LibVLC 失败: {ex.Message}\n{ex.StackTrace}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			CleanupResources();
		}
	}
	// 修复 InitializeLibVLC 方法
	private void InitializeLibVLC2()
	{
		try
		{
			// 初始化 LibVLC 核心
			Core.Initialize();

			// 创建 LibVLC 实例，添加音频输出选项
			string[] options = {
			"--intf", "dummy",
			"--no-video-title-show",
			"--aout=waveout"  // 指定音频输出模块
        };
			_libVLC = new LibVLC(options);

			// 检查 LibVLC 是否创建成功
			if(_libVLC == null)
			{
				throw new InvalidOperationException("无法创建 LibVLC 实例");
			}

			// 创建 MediaPlayer 实例
			_mediaPlayer = new MediaPlayer(_libVLC);

			// 检查 MediaPlayer 是否创建成功
			if(_mediaPlayer == null)
			{
				throw new InvalidOperationException("无法创建 MediaPlayer 实例");
			}

			// 检查 VideoView 是否创建成功
			if(videoView1 == null)
			{
				throw new InvalidOperationException("无法创建 VideoView 实例");
			}

			// 设置 MediaPlayer 到 VideoView  
			videoView1.MediaPlayer = _mediaPlayer;

			// 初始化 NAudio 组件
			waveProvider = new BufferedWaveProvider(new WaveFormat(44100, 16, 2)) // 44.1kHz, 16位, 立体声
			{
				BufferDuration = TimeSpan.FromSeconds(2) // 设置2秒缓冲区
			};

			waveOut = new WaveOutEvent();
			waveOut.Init(waveProvider);
			waveOut.Play(); // 提前启动播放

			// 设置音频回调（必须在播放前设置）
			_mediaPlayer.SetAudioCallbacks(
				OnAudioPlay,    // play callback
				OnAudioPause,   // pause callback
				OnAudioResume,  // resume callback
				OnAudioFlush,   // flush callback
				OnAudioDrain    // drain callback
			);
		}
		catch(Exception ex)
		{
			MessageBox.Show($"初始化 LibVLC 失败: {ex.Message}\n{ex.StackTrace}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			CleanupResources();
		}
	}

	// 确保 InitializeLibVLC 方法正确设置所有回调
	private void InitializeLibVLC3()
	{
		try
		{
			// 初始化 LibVLC 核心
			Core.Initialize();

			// 创建 LibVLC 实例
			_libVLC = new LibVLC();

			// 检查 LibVLC 是否创建成功
			if(_libVLC == null)
			{
				throw new InvalidOperationException("无法创建 LibVLC 实例");
			}

			// 创建 MediaPlayer 实例
			_mediaPlayer = new MediaPlayer(_libVLC);

			// 检查 MediaPlayer 是否创建成功
			if(_mediaPlayer == null)
			{
				throw new InvalidOperationException("无法创建 MediaPlayer 实例");
			}

			// 检查 VideoView 是否创建成功
			if(videoView1 == null)
			{
				throw new InvalidOperationException("无法创建 VideoView 实例");
			}
			// 设置 MediaPlayer 到 VideoView  
			videoView1.MediaPlayer = _mediaPlayer;

			// 初始化 NAudio 组件
			waveProvider = new BufferedWaveProvider(new WaveFormat(44100, 16, 2)) // 44.1kHz, 16位, 立体声
			{
				BufferDuration = TimeSpan.FromSeconds(2) // 设置2秒缓冲区
			};

			waveOut = new WaveOutEvent();
			waveOut.Init(waveProvider);
			waveOut.Play(); // 提前启动播放器

			// 设置音频回调（必须在播放前设置）
			// 确保传递所有5个回调方法
			_mediaPlayer.SetAudioCallbacks(
				OnAudioPlay,    // play callback    (IntPtr data, IntPtr samples, uint count, long pts)
				OnAudioPause,   // pause callback   (IntPtr data, long pts)
				OnAudioResume,  // resume callback  (IntPtr data, long pts)
				OnAudioFlush,   // flush callback   (IntPtr data, long pts)
				OnAudioDrain    // drain callback   (IntPtr data)
			);
		}
		catch(Exception ex)
		{
			MessageBox.Show($"初始化 LibVLC 失败: {ex.Message}\n{ex.StackTrace}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			CleanupResources();
		}
	}
	// 增强 CleanupResources 方法以正确释放音频资源
	private void CleanupResources()
	{
		try
		{
			progressTimer?.Stop();

			// 停止并释放 NAudio 资源
			if(waveOut != null)
			{
				try
				{
					if(waveOut.PlaybackState == NAudio.Wave.PlaybackState.Playing ||
						waveOut.PlaybackState == NAudio.Wave.PlaybackState.Paused)
					{
						waveOut.Stop();
					}
				}
				catch { }
				waveOut.Dispose();
				waveOut = null;
			}
			if(waveProvider != null)
			{
				waveProvider.ClearBuffer();
				waveProvider = null;
			}

			if(_mediaPlayer != null)
			{
				try
				{
					_mediaPlayer.Stop();
				}
				catch { }

				// 安全地取消订阅事件
				try
				{
					_mediaPlayer.TimeChanged -= OnMediaPlayerTimeChanged;
					_mediaPlayer.LengthChanged -= OnMediaPlayerLengthChanged;
					_mediaPlayer.Playing -= OnMediaPlayerPlaying;
					_mediaPlayer.Paused -= OnMediaPlayerPaused;
					_mediaPlayer.Stopped -= OnMediaPlayerStopped;
					_mediaPlayer.EndReached -= OnMediaPlayerEndReached;
				}
				catch { }

				_mediaPlayer.Dispose();
				_mediaPlayer = null;
			}

			_libVLC?.Dispose();
			_libVLC = null;

			videoView1?.Dispose();
			videoView1 = null;
		}
		catch(Exception ex)
		{
			if(System.Diagnostics.Debugger.IsAttached)
			{
				System.Diagnostics.Debug.WriteLine($"清理资源时出错: {ex.Message}");
			}
		}
	}
	// 计算 RMS（均方根值）
	//private double CalculateRMS(short[] samples)
	//{
	//	if(samples.Length == 0)
	//		return 0;
	//	return Math.Sqrt(samples.Select(sample => sample * sample).Average());
	//}
	private void PlayButton_Click(object sender, EventArgs e)
	{
		var media = new Media(_libVLC, "your_video_file.mp4", FromType.FromPath);
		_mediaPlayer.Play(media);
	}
	private void StopButton_Click1(object sender, EventArgs e)  //??????????
	{
		_mediaPlayer.Stop();
	}
	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		//_mediaPlayer.Dispose();			//_libVLC.Dispose();			CleanupResources();  // 清理资源 +++++++++ 的
		base.OnFormClosing(e);
	}
	private void InitializeUIControls()
	{
		// 初始化音量控制UI
		//_videoView.BringToFront();
		//_videoView.SendToBack();   //发送到父容器的最底层（降低其 Z 顺序，使其被其他同级控件覆盖）
		//InitializeChannelDisplay();
		// 初始化视频缩放控制UI
		//InitializeZoomControls();
		progressTimer.Start();
	}
	private void VolumeTrackBar_Scroll(object sender, EventArgs e)
	{
		int volume = volumeTrackBar.Value;
		//UpdateVolumeBar(newVolume);
		volumenum.Text = volume.ToString();
		SetVolume(volume);
		// 如果之前是静音状态，则取消静音
		if(_isMuted)
		{
			_isMuted = false;
			//_muteButton.Text = "Mute";
		}
	}
	// 静音按钮点击事件处理程序
	private void MuteButton_Click1(object sender, EventArgs e)
	{
		if(_mediaPlayer == null)
			return;
		if(!_isMuted)
		{
			// 保存当前音量并静音
			_previousVolume = _mediaPlayer.Volume;
			_mediaPlayer.Volume = 0;
			_isMuted = true;
			muteButton.SymbolColor = Color.Salmon;
			volumeTrackBar.Value = 0;
		}
		else
		{
			// 恢复之前音量
			_mediaPlayer.Volume = _previousVolume;
			_isMuted = false;
			//_muteButton.Text = "Mute";
			muteButton.SymbolColor = Color.Gray;
			volumeTrackBar.Value = _previousVolume;
		}
	}
	// 修改现有的 SetVolume 方法以更新音量显示
	public void SetVolume1(int volume)
	{
		try
		{
			if(_mediaPlayer != null)
			{
				int clampedVolume = Math.Max(0, Math.Min(100, volume));
				_mediaPlayer.Volume = clampedVolume;

				// 更新音量进度条（避免触发Scroll事件）
				if(volumeTrackBar.Value != clampedVolume)
				{
					volumeTrackBar.Value = clampedVolume;
				}
				// 同时更新声道显示（这里简化为左右声道相同）					UpdateChannelDisplay(clampedVolume, clampedVolume);
			}
		}
		catch(Exception ex)
		{
			MessageBox.Show($"设置音量失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
	private void VolumeDownButton_Click(object sender, EventArgs e)
	{
		int currentVolume = _mediaPlayer?.Volume ?? 0;
		int newVolume = Math.Max(0, currentVolume - 1);
		SetVolume(newVolume);
		//UpdateVolumeBar(newVolume);

		// 如果之前是静音状态，则取消静音
		if(_isMuted)
		{
			_isMuted = false;
			//_muteButton.Text = "Mute";
		}
	}

	// 放大按钮点击事件
	private void ZoomInButton_Click(object sender, EventArgs e)
	{
			_currentZoomFactor = _mediaPlayer.Scale;
			_currentZoomFactor += ZOOM_INCREMENT;
		if(_currentZoomFactor > MAX_ZOOM)
			_currentZoomFactor = MAX_ZOOM;

		ApplyZoom();
	}

	// 缩小按钮点击事件
	private void ZoomOutButton_Click(object sender, EventArgs e)
	{
			_currentZoomFactor = _mediaPlayer.Scale;

			_currentZoomFactor -= ZOOM_INCREMENT;
		if(_currentZoomFactor < MIN_ZOOM)
			_currentZoomFactor = MIN_ZOOM;

		ApplyZoom();
	}

	// 适应窗口按钮点击事件
	private void FitToWindowButton_Click(object sender, EventArgs e)
	{
		_currentZoomFactor = 0.5f;
		ApplyZoom();
	}
	/// <summary>
	/// 应用当前缩放因子到视频显示
	/// </summary>
	private void ApplyZoom()
	{
		if(_mediaPlayer == null || videoView1 == null)
			return;
		try
		{
			// 这里我们通过调整视频视图的大小来实现缩放效果
			// 实际的 VLC 缩放需要通过其 API 设置
			UpdateZoomLabel();

			// 如果需要通过 VLC API 实现真正的缩放，可以使用以下方法：
			_mediaPlayer.Scale = _currentZoomFactor;
		}
		catch(Exception ex)
		{
			MessageBox.Show($"应用缩放失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
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
	//-----------------------------------------------
	// 定时器更新进度
	private void ProgressTimer_Tick(object sender, EventArgs e)
	{
		if(_mediaPlayer != null && !_isSeeking)
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
			if(_mediaPlayer != null)
			{
				int clampedVolume = Math.Max(0, Math.Min(100, volume));
				_mediaPlayer.Volume = clampedVolume;

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
		if(_mediaPlayer == null)
			return;
		try
		{
			if(!_isMuted)
			{
				// 保存当前音量并静音
				_previousVolume = _mediaPlayer.Volume;
				_mediaPlayer.Volume = 0;
				_isMuted = true;
				muteButton.SymbolColor = Color.Salmon;
				volumeTrackBar.Value = 0;

				// 更新声道显示为静音状态  					UpdateChannelDisplay(0, 0);
			}
			else
			{
				// 恢复之前音量
				_mediaPlayer.Volume = _previousVolume;
				_isMuted = false;
				muteButton.SymbolColor = Color.Gray;
				volumeTrackBar.Value = _previousVolume;

				// 更新声道显示为恢复的音量 					UpdateChannelDisplay(_previousVolume, _previousVolume);
			}
		}
		catch(Exception ex)
		{
			MessageBox.Show($"切换静音状态失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	#endregion

	#region   ------------ LibVLC  添加  播放 时间 音量 播放 继续 停止等	按钮 -----------------	

	//public partial class VideoPlayerWithProgressBar : Form
	//	{
	//private TrackBar _progressBar; // 进度条
	//private Timer _progressTimer; // 定时器更新进度
	//private Label _currentTimeLabel; // 当前时间标签
	//private Label _totalTimeLabel; // 总时间标签
	private bool _isSeeking = false; // 是否正在拖拽进度条  		 // 订阅播放器事件
	private object audioData;

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
	// 进度条事件处理
	private void _progressBar_MouseDown(object sender, MouseEventArgs e)
	{
		_isSeeking = true;
	}
	private void _progressBar_MouseUp(object sender, MouseEventArgs e)
	{
		if(_isSeeking && _mediaPlayer != null)
		{
			// 计算目标时间
			long totalTime = _mediaPlayer.Length;
			if(totalTime > 0)
			{
				long targetTime = (long)(progressBar.Value * totalTime / 1000.0);
				_mediaPlayer.Time = targetTime;
			}
		}
		_isSeeking = false;
	}
	private void _progressBar_Scroll(object sender, EventArgs e)
	{
		// 拖拽时实时更新时间显示
		if(_mediaPlayer != null && _mediaPlayer.Length > 0)
		{
			long totalTime = _mediaPlayer.Length;
			long currentTime = (long)(progressBar.Value * totalTime / 1000.0);
			currentTimeLabel.Text = FormatTime(currentTime);
		}
	}
	// 定时器更新进度

	private void UpdateProgress()
	{
		if(_mediaPlayer == null)
			return;

		long currentTime = _mediaPlayer.Time;
		long totalTime = _mediaPlayer.Length;

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
	private void ResetProgress()
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
	private void playPauseButton_Click_1(object sender, EventArgs e)
	{  //			var tt = _mediaPlayer;
		if(IsfirstPlaying == false)
		{
			IsfirstPlaying = true;
			PlayVideo();
			playPauseButton.Image = Properties.Resources.pause;
			return;
		}
		else
		{
			if(_mediaPlayer == null)
			{
				return;
			}
			if(_mediaPlayer.State == VLCState.Playing)
			{
				progressTimer.Stop();
				_mediaPlayer.Pause();               //添加图片
				playPauseButton.Image = Properties.Resources.start;                 //((Button)sender).Text = "播放";
			}
			else
			{
				progressTimer.Start();
				_mediaPlayer.Play();
				playPauseButton.Image = Properties.Resources.pause;                 //((Button)sender).Text = "暂停";
					//videoView1.ba
			}
		}
			var zz = _mediaPlayer.Scale;
			temp1.Text = zz.ToString();
	}
	private void StopButton_Click(object sender, EventArgs e)
	{
		//_mediaPlayer?.Stop();
		try
		{
			if(_mediaPlayer != null)
			{
				playPauseButton.Image = Properties.Resources.start;
				progressTimer.Stop();
				_mediaPlayer.Stop();
			}
		}
		catch(Exception ex)
		{
			MessageBox.Show($"停止播放失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
	// 播放视频文件
	public void PlayVideo5()
	{
		try
		{
			if(_libVLC == null || _mediaPlayer == null)
			{
				MessageBox.Show("播放器未正确初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if(string.IsNullOrEmpty(filePath))
			{
				MessageBox.Show("文件路径不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			_mediaPlayer.Stop();
				using var media = new Media(_libVLC, filePath, FromType.FromPath);
				if(media == null)
				{
					throw new InvalidOperationException("无法创建媒体对象");
				}
				progressTimer.Start();
				_mediaPlayer.Play(media);
			}
		catch(Exception ex)
		{
			MessageBox.Show($"播放视频失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
	// 清理资源
	private void CleanupResources1()
	{
		try
		{
			progressTimer?.Stop();

			if(_mediaPlayer != null)
			{
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
		catch(Exception ex)
		{
			Console.WriteLine($"清理资源时出错: {ex.Message}");
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

	// 处理快捷键
	private void LaserEditing_KeyDown(object sender, KeyEventArgs e)
	{
		Keys key = e.KeyCode;
		//  if (e.Control != true)//如果没按Ctrl键      return;
		switch(key)
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
	{       //show cut 
			//Cut form = new Cut();
			//form.Show();
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
	//导入素材  Importing the materials
	private void buttonx7_Click(object sender, EventArgs e)
	{
		panel4.Visible = true;
		openfile.Visible = true;
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
		string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		temp.Text = "默认文档目录: " + documentsPath;
		string subDirectory = Path.Combine(documentsPath, "ResourceFolder");
		//判断是否目录存在
		if(Directory.Exists(subDirectory))
		{
			temp.Text = "子目录已存在: " + subDirectory;
		}
		else
		{
			temp1.Text = "子目录不存在，将创建: " + subDirectory;
			// 创建子目录
			try
			{
				Directory.CreateDirectory(subDirectory);
				temp1.Text = "子目录创建成功: " + subDirectory;
			}
			catch(Exception ex)
			{
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
		if(ofd.ShowDialog() == DialogResult.OK)
		{ //获取所有的选中文件存入string数组
			string[] selectedFiles = ofd.FileNames;
		}
		if(ofd.FileNames.Length > 0)
		{
			// 遍历选中的文件
			foreach(string file in ofd.FileNames)
			{
				try
				{
					// 获取文件名
					string fileName = Path.GetFileName(file);
					// 构建目标路径
					string targetPath = Path.Combine(subDirectory, fileName);
					// 复制文件到目标路径
					File.Copy(file, targetPath, true); // true表示覆盖同名文件
					temp1.Text += $"\n已导入: {fileName}";
				}
				catch(Exception ex)
				{
					temp1.Text += $"\n导入失败: {ex.Message}";
				}
			}
		}
		else
		{
			temp1.Text = "未选择任何文件";
			//button2.Visible = true;				qrcode1.Visible = true;
			panel4.Visible = true;

		}

	}

	private void videoView1_Click(object sender, EventArgs e)  //视频 播放 
	{

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
		if(panel4.Visible)
		{
			// 调整按钮和二维码的位置
			openfile.Left = (panel4.Width - openfile.Width) / 2 - 20; // 水平居中
			openfile.Top = (panel4.Height - openfile.Height) / 2; // 垂直居中
			qrcode1.Left = openfile.Left + 290; // 水平居中	
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

	#region   ------------动态加载 LibVLCSharp.WinForms	暂时不用----------------	
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
			// 创建 VideoView 实例 				videoViewInstance = Activator.CreateInstance(videoViewType);  暂时不用
			// 设置 MediaPlayer 属性（如果需要）
			// 这需要使用反射来设置属性
		}
		catch(Exception ex)
		{
			MessageBox.Show($"动态加载 LibVLCSharp 失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
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
		//_mediaPlayer.Mute = true; // 静音					  //_mediaPlayer.uiMode = "full"; // 或 "mini"
		filePath = ofd.FileName;
		PlayVideo();


	}

	#endregion
}
} //class LaserEditing









