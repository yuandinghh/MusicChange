using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibVLCSharp.Shared;

namespace MusicChange
{
	public partial class Cut:Form
	{
		#region  *********  变量区  ***********
		// 引入用户32库中的方法，用于实现窗口拖动
		[DllImport("user32.dll")]
		public static extern bool ReleaseCapture();
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		private const int WM_NCLBUTTONDOWN = 0xA1;
		private const int HTCAPTION = 0x2;
		private LibVLC _libVLC;
		private MediaPlayer _mediaPlayer;
		private string Filestr, inputFile, outputFile, arguments;
		TimeSpan durationS, startTimeTs, endTimeTs;  // 视频时长、起始时间和结束时间
		int totalSeconds; // 视频总时长（秒）
		int right_rotate_angle = 0; // 视频旋转角度
		int left_rotate_angle = 0; // ++6+
		bool firstdisp = true; // 是否第一次显示
		bool clibb = false; //裁剪 时间正确
		bool isCutting = false; // 是否正在裁剪
		bool isCompress = false; // 是否正在压缩
		int timeStamp = 0; // 时间戳
		private bool waitingForBuffer = false; // 是否等待缓冲完成
		string startTime = "00:00:00"; // 起始时间
		string endTime = "00:00:00"; // 结束时间
		string duration = "00:00:30"; // 持续时间
		public Cut()
		{  //类的初始化 函数
			InitializeComponent();
			videoView1.Size = new Size(700, 580); // 设置视频视图控件大小
			videoView1.Location = new Point(678, 146);
			comboBoxSpeed.SelectedIndex = 3; // 默认1.0x	_mediaPlayer.SetRate(rate) = 0.1; 
			comboBoxSpeed.SelectedIndexChanged += comboBoxSpeed_SelectedIndexChanged; //示例：为按钮和组合框添加说明
			#endregion

		#region ------- ToolTip 鼠标进入悬停显示 -------
			ToolTipEx toolTip1 = new ToolTipEx();   // 创建自定义 ToolTipEx 实例 ，鼠标悬停时显示提示信息
			toolTip1.TipFont = new Font("微软雅黑", 20); // 这里设置字体和大小
			ConfigureToolTip(toolTip1);
			// 初始化 LibVLC

			Core.Initialize(@"D:\Documents\Visual Studio 2022\MusicChange");
			//Core.Initialize( );
			_libVLC = new LibVLC();
			_mediaPlayer = new MediaPlayer(_libVLC);
			videoView1.MediaPlayer = _mediaPlayer;  // 将 MediaPlayer 绑定到 VideoView 控件


		}

		private void ConfigureToolTip(ToolTipEx toolTip1)
		{

			// 设置 ToolTip 属性
			toolTip1.AutoPopDelay = 90000; // 提示框显示 5 秒后消失
			toolTip1.InitialDelay = 500; // 鼠标悬停 1 秒后显示提示框
			toolTip1.ReshowDelay = 1000;   // 鼠标移开后再次悬停的延迟时间
			toolTip1.ShowAlways = true;   // 即使控件未激活也显示提示框
			toolTip1.IsBalloon = true;    // 使用气泡样式
			toolTip1.ToolTipIcon = ToolTipIcon.Info; // 提示框图标
			toolTip1.ToolTipTitle = "提示"; // 提示框标题

			toolTip1.SetToolTip(textBox1, "选择要裁剪的视频文件");
			toolTip1.SetToolTip(textBox1, "选择要播放的视频文件");
			toolTip1.SetToolTip(label28, "crf（Constant Rate Factor，恒定码率因子）\r\n•\t作用：控制视频压缩的画质和文件大小。\r\n•\t取值范围：0~51，常用范围为 18~28。\r\n•\t数值越小，画质越高，文件越大。\r\n•\t数值越大，画质越低，文件越小。\r\n•\t一般推荐：高质量用 18~22，普通用 23~28。");
			toolTip1.SetToolTip(comboBoxSpeed, "选择播放速度");
			toolTip1.SetToolTip(label27, "preset（预设编码速度）\r\n•\t作用：控制编码速度与压缩效率的平衡。\r\n•\t可选值（从快到慢）：\r\n•\tultrafast, superfast, veryfast, faster, fast, medium, slow, slower, veryslow\r\n•\t说明：\r\n•\t越快（如 ultrafast），编码速度快，但文件大、画质略低。\r\n•\t越慢（如 veryslow），编码速度慢，但文件更小、画质更好。\r\n•\t默认值是 medium，一般推荐用 fast、medium 或 slow。");
		}
		#endregion

		#region -------------- 视频播放相关 ------------
		// 选择要播放的视频文件
		private void button3_Click(object sender, EventArgs e)
		{  
			if(textBox1 != null)
			{
				OpenFileDialog ofd = new OpenFileDialog();
				ofd.Filter = "视频文件|*.mp4;*.avi;*.wmv;*.mov|所有文件|*.*";
				if(ofd.ShowDialog() == DialogResult.OK)
				{
					//_mediaPlayer.uiMode = "full"; // 或 "mini"
					Filestr = ofd.FileName;
					textBox1.Text = Filestr;
					_mediaPlayer.Play();
					timer1.Start(); // 播放时启动定时器
				}
			}
			else
			{
				PlayVideo(textBox1.Text);
				//_mediaPlayer.uiMode = "full"; 				_mediaPlayer.Play();
				timer1.Start(); // 播放时启动定时器
			}
		}
		private void buttonStop_Click(object sender, EventArgs e)
		{
			_mediaPlayer.Stop();
			timer1.Stop(); // 停止进度条刷新
			label1.Text = "00:00:00 / 00:00:00";
			progressBar1.Value = 0;
			//firstdisp = true;
		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			if(_mediaPlayer.IsPlaying)
			{
				double duration = _mediaPlayer.Time / 1000.0; // 当前播放进度，单位：秒
				double position = _mediaPlayer.Length / 1000.0;
				if(duration > 0)
				{
					progressBar1.Maximum = (int)duration;
					progressBar1.Value = Math.Min((int)position, progressBar1.Maximum);
					// 显示当前时间和总时长
					TimeSpan pos = TimeSpan.FromSeconds(duration);
					TimeSpan dur = TimeSpan.FromSeconds(position);
					if(firstdisp)
					{
						label15.Text = $"{dur:hh\\:mm\\:ss\\.fff}";
						totalSeconds = (int)dur.TotalSeconds; // 获取总时长的秒数
						firstdisp = false;
					}
					else
					{
						label1.Text = $"{pos:hh\\:mm\\:ss\\.fff}";
						//	firstdisp = true;
					}

				}
				else
				{
					label1.Text = "00:00:00 ";
				}
			}
			else
			{
				label1.Text = "00:00:00";
			}
		}
		private void button5_Click(object sender, EventArgs e)
		{
			_mediaPlayer.Pause();
		}
		private void button6_Click(object sender, EventArgs e)
		{
			_mediaPlayer.Stop();
			//	firstdisp = true;
		}
		private void button4_Click(object sender, EventArgs e)
		{
			button9_Click(null, null); // 调用播放按钮事件
			_mediaPlayer.Mute = true; // 静音
			PlayVideo(textBox1.Text);
			//	_mediaPlayer.uiMode = "full"; // 或 "mini"
			_mediaPlayer.Play();
			//	_mediaPlayer.Mute = false; // 取消静音
			timer1.Start(); // 播放时启动定时器
		}
		private void button7_Click(object sender, EventArgs e)
		{
			_mediaPlayer.Play();
			timer1.Start(); // 恢复进度条刷新
		}
		private void button8_Click(object sender, EventArgs e)
		{
			//退出当前窗口
			Application.Exit();
			this.Close();
		//释放资源

		}
		//选择目录
		private void button2_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "视频文件|*.mp4;*.avi;*.wmv;*.mov|所有文件|*.*";
			if(ofd.ShowDialog() == DialogResult.OK)
			{
				Filestr = ofd.FileName;
				textBox1.Text = Filestr;
			}
		}
		#endregion

		#region  ------- 开始压缩 -------
		//	按目录所有视频文件压缩  yy
		private async void button22_Click(object sender, EventArgs e)
		{
			killProcess();
			if(isCompress)
			{
				MessageBox.Show("正在压缩中，请稍后再试！");
				return;
			}
			isCompress = true; // 设置压缩状态为 true
			int count = 1;
			var path = @"F:\newipad";
			List<string> imageList = new List<string>();
			using(FolderBrowserDialog folderDialog = new FolderBrowserDialog())
			{
				folderDialog.Description = "请选择一个文件夹";
				folderDialog.ShowNewFolderButton = true; // 是否允许新建文件夹
				if(folderDialog.ShowDialog() == DialogResult.OK)
				{
					path = folderDialog.SelectedPath;
					textBox5.Text = path; // 显示选择的路径
				}
			}
			var timagefiles = Directory.GetFiles(path, "*.*").Where(file => file.ToLower().EndsWith("mp4") || file.ToLower().EndsWith("mov") || file.ToLower().EndsWith("avi") || file.ToLower().EndsWith("mkv") || file.ToLower().EndsWith("wmv")).ToList();
			imageList.AddRange(timagefiles);
			label24.Text = "视频压缩文件数量：" + count.ToString();
			if(imageList.Count == 0)
			{
				MessageBox.Show("没有找到符合条件的文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			else
			{       // 遍历文件列表并添加到 ListBox    string filestr = timagefiles[0].ToString();
				listBox1.Items.Clear(); // 清空ListBox中的所有项
				listBox1.HorizontalScrollbar = true; // 设置水平滚动条

				foreach(string file in imageList)
				{
					listBox1.Items.Add(file);
					textBox1.Text = file;
					if(File.Exists(file))
					{  //					selefile_Click( null, null );
						inputFile = textBox1.Text;
						if(string.IsNullOrEmpty(inputFile) || !File.Exists(inputFile))
						{
							MessageBox.Show("请选择有效的视频文件！");
							return;
						}           //Remove the carriage return and spaces at the end of the string.
						inputFile = inputFile.TrimEnd('\r', '\n', ' ');
						string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFile);
						fileNameWithoutExtension = fileNameWithoutExtension + "_s";
						outputFile = Path.Combine(textBox3.Text, $"{fileNameWithoutExtension}.mp4");
						if(File.Exists(outputFile))
						{     //如果存在 删除文件outputFile
							File.Delete(outputFile); // 删除文件
						}
						arguments = $"-i {inputFile} -c:v libx264 -crf 23 -preset medium -c:a aac -b:a 128k -movflags +faststart {outputFile} ";
						setlisbox(); // 设置ListBox内容
						try
						{
							label3.Text = "压缩中。。。";
							Process ffmpegProcess = new Process();
							ffmpegProcess.StartInfo.FileName = "ffmpeg.exe";
							ffmpegProcess.StartInfo.Arguments = arguments;
							ffmpegProcess.StartInfo.UseShellExecute = false;    // 启用重定向
							ffmpegProcess.StartInfo.RedirectStandardError = true;
							ffmpegProcess.StartInfo.CreateNoWindow = true;
							var outputBuilder = new StringBuilder();
							//注册输出数据处理事件
							ffmpegProcess.ErrorDataReceived += (eventSender, args) =>
							{
								if(!string.IsNullOrEmpty(args.Data))
								{
									outputBuilder.AppendLine(args.Data);
									string timeInfo = ParseProgress(args.Data);
									if(!string.IsNullOrEmpty(timeInfo))
									{
										// 跨线程安全更新UI
										this.Invoke((MethodInvoker)delegate
										{
											textBox9.Text = $"压缩时间: {timeInfo}";
										});
									}
								}
							};
							// 启动进程并开始异步读取输出
							ffmpegProcess.Start();
							ffmpegProcess.BeginErrorReadLine();
							// 等待进程完成
							await Task.Run(() => ffmpegProcess.WaitForExit());
							//string output = ffmpegProcess.StandardError.ReadToEnd(); // FFmpeg信息输出在StandardError
							string output = outputBuilder.ToString();
							if(!string.IsNullOrEmpty(output))
							{
								textBox4.Text = output;
							}
							ffmpegProcess.CancelErrorRead();
							ffmpegProcess.Close();
							label3.Text = "压缩完成！";
						}
						catch(Exception ex)
						{
							label3.Text = "压缩失败: " + ex.Message;
						}
					}
					textBox8.Text = count.ToString();
					count++;
				}
			}
			//} catch (Exception ex) {
			//	MessageBox.Show( $"Processing failed: {ex.Message}" );   //LogError( e, "Shape processing failed." );
			//	throw;
			//}  //   GetFileInfo(listBox2.GetItemText(0).ToLower());  “GetFileInfo” 通常是一个用于获取文件相关信息的函数或方法。
		}
		private async void selefile_Click(object sender, EventArgs e)// 压缩 单个视频文件
		{
			killProcess();
			if(isCompress)
			{
				MessageBox.Show("正在压缩中，请稍后再试！");
				return;
			}
			isCompress = true; // 设置压缩状态为 true
			label3.Text = "准备压缩";
			textBox4.Text = "";
			inputFile = textBox1.Text;
			if(string.IsNullOrEmpty(inputFile) || !File.Exists(inputFile))
			{
				MessageBox.Show("请选择有效的视频文件！");
				return;
			}           //Remove the carriage return and spaces at the end of the string.
			inputFile = inputFile.TrimEnd('\r', '\n', ' ');
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFile);

			if(checkBox1.Checked)
			{ //自定义压缩参数
				fileNameWithoutExtension = fileNameWithoutExtension + DateTime.Now.ToString("_HHmmss");
				outputFile = Path.Combine(textBox3.Text, $"{fileNameWithoutExtension}.mp4");
				arguments = $"-i {inputFile} -c:v libx264 -crf {comboBox3.Text} -preset {comboBox2.Text} -c:a aac -b:a 128k -movflags +faststart {outputFile} ";

			}
			else
			{
				if(radioButton1.Checked)
				{            // 高质量压缩参数
					fileNameWithoutExtension = fileNameWithoutExtension + "_h";
				}
				else if(radioButton2.Checked)
				{             // 高压缩比参数
					fileNameWithoutExtension = fileNameWithoutExtension + "_m";
				}
				else if(radioButton4.Checked)
				{             // 高压缩比参数
					fileNameWithoutExtension = fileNameWithoutExtension + "_l";
				}
				else
				{            // 默认参数
					fileNameWithoutExtension = fileNameWithoutExtension + "_low";
				}
				outputFile = Path.Combine(textBox3.Text, $"{fileNameWithoutExtension}.mp4");
				if(radioButton1.Checked)
				{            // 高质量压缩参数
					arguments = $"-i \"{inputFile}\" -c:v libx264 -crf 18 -preset fast -c:a aac -b:a 128k -movflags +faststart \"{outputFile}\" ";
				}
				else if(radioButton2.Checked)
				{          // 高压缩比参数
					arguments = $"-i \"{inputFile}\" -c:v libx264 -crf 23 -preset medium -c:a aac -b:a 128k -movflags +faststart \"{outputFile}\" ";
				}
				else if(radioButton4.Checked)
				{          // 高压缩比参数
					arguments = $"-i \"{inputFile}\" -c:v libx264 -crf 48 -preset veryslow -c:a aac -b:a 64k -movflags +faststart \"{outputFile}\" ";
				}
				else
				{            // 默认参数
					arguments = $"-i \"{inputFile}\" -c:v libx264 -crf 28 -preset slower -c:a aac -b:a 128k -movflags +faststart \"{outputFile}\"";
				}
			}
			if(File.Exists(outputFile))
			{     //如果存在 删除文件outputFile
				File.Delete(outputFile); // 删除文件
			}
			setlisbox(); // 设置ListBox内容
			try
			{
				label3.Text = "压缩中。。。";
				Process ffmpegProcess = new Process();
				ffmpegProcess.StartInfo.FileName = "ffmpeg.exe";
				ffmpegProcess.StartInfo.Arguments = arguments;
				ffmpegProcess.StartInfo.UseShellExecute = false;    // 启用重定向
				ffmpegProcess.StartInfo.RedirectStandardError = true;
				ffmpegProcess.StartInfo.CreateNoWindow = true;
				var outputBuilder = new StringBuilder();    //注册输出数据处理事件
				ffmpegProcess.ErrorDataReceived += (eventSender, args) =>
				{
					if(!string.IsNullOrEmpty(args.Data))
					{
						outputBuilder.AppendLine(args.Data);
						string timeInfo = ParseProgress(args.Data);
						if(!string.IsNullOrEmpty(timeInfo))
						{ // 跨线程安全更新UI
							this.Invoke((MethodInvoker)delegate
							{
								textBox9.Text = $"压缩时间: {timeInfo}";
							});
						}
					}
				};
				// 启动进程并开始异步读取输出
				ffmpegProcess.Start();
				ffmpegProcess.BeginErrorReadLine();         // 等待进程完成
				await Task.Run(() => ffmpegProcess.WaitForExit());
				string output = outputBuilder.ToString();   // FFmpeg信息输出在StandardError
				if(!string.IsNullOrEmpty(output))
				{
					textBox4.Text = output;
				}
				ffmpegProcess.CancelErrorRead();
				ffmpegProcess.Close();
				label3.Text = "压缩完成！";
			}
			catch(Exception ex)
			{
				label3.Text = "压缩失败: " + ex.Message;
			}
			isCompress = false;
		}
		private void setlisbox()
		{
			listBox1.DrawMode = DrawMode.OwnerDrawFixed;
			listBox1.DrawItem += listBox1_DrawItem;
			listBox1.MeasureItem += listBox1_MeasureItem;
			listBox1.Items.Clear(); // 清空ListBox中的所有项
			listBox1.HorizontalScrollbar = true; // 设置水平滚动条	 //listBox1.ItemHeight = 14; //在属性窗口中设置ListBox的高度
			listBox1.Items.Add($"开始压缩");
			listBox1.Items.Add($"压缩命令: ffmpeg {arguments}");
			listBox1.Items.Add($"被压缩文件:{inputFile}");
			listBox1.Items.Add($"输出文件:{outputFile}");
		}
		// 解析进度信息中的时间字段
		public static string ParseProgress(string line)
		{
			// 匹配out_time字段（格式：out_time=00:00:10.50）
			Match timeMatch = Regex.Match(line, @"^out_time=(\d+:\d+:\d+\.\d+)$");
			Match timeMatch2 = Regex.Match(line, @"time=(\d+:\d+:\d+\.\d+)");
			if(timeMatch2.Success)
			{
				return timeMatch2.Groups[1].Value;
				//return $"找到时间: {timeValue}";
			}
			if(timeMatch.Success)
			{
				return timeMatch.Groups[1].Value;
			}
			return "";
		}
		#endregion

		#region ------- 视频裁剪 -------
		private void button1_Click(object sender, EventArgs e)
		{
			if(Clip())
			{
				if(totalSeconds != 0)
				{
					CutVideoSegment(startTime, endTime, inputFile, outputFile);
				}
			}
			//TimeSpan endS = dateTimePicker3.Value.TimeOfDay;
			//int endSeconds = (int)endS.TotalSeconds;
		}
		private void button27_Click(object sender, EventArgs e)  //裁剪开始部分视频
		{
			if(dateTimePicker1.Value.TimeOfDay.TotalSeconds != 0)
			{
				MessageBox.Show("起始时间必须 0 ！");
				return;
			}
			if(Clip())
			{
				if(totalSeconds != 0)
				{        //获得整个视频时间
					double duration = _mediaPlayer.Time;
					TimeSpan dur = TimeSpan.FromSeconds(duration);
					string endss = $"{dur:hh\\:mm\\:ss\\.fff}";
					CutVideoSegment(endTime, endss, inputFile, outputFile);
				}
			}
		}
		bool Clip()  //Clip the video  裁剪视频
		{
			clibb = false;
			killProcess();
			_mediaPlayer.Pause();  // 暂停视频播放
			textBox6.Text = "";
			inputFile = textBox1.Text;
			if(string.IsNullOrEmpty(inputFile) || !File.Exists(inputFile))
			{
				MessageBox.Show("请选择有效的视频文件！");
				return clibb;
			}
			inputFile = inputFile.TrimEnd('\r', '\n', ' ');
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFile);
			//当前时间 转换为字符串  F:\newipad\已经压缩\Aigirl3.mp4
			fileNameWithoutExtension = fileNameWithoutExtension + DateTime.Now.ToString("_HHmmss");
			// 获取精确到毫秒的起始时间、持续时间和结束时间
			// 例如：startTime = "00:00:10.500", duration = "00:00:20.750", endTime = "00:00:30.000"
			// 将 DateTimePicker 的值转换为字符串格式
			// 注意：DateTimePicker 的 Value 属性返回的是 DateTime 类型，包含日期和时间
			// 如果需要精确到毫秒，可以使用 ToString("HH:mm:ss.fff")
			startTime = dateTimePicker1.Value.ToString("HH:mm:ss");
			duration = dateTimePicker2.Value.ToString("HH:mm:ss");
			endTime = dateTimePicker3.Value.ToString("HH:mm:ss");
			int totaltime = (int)durationS.TotalSeconds;  // 转换为总秒数
			durationS = dateTimePicker2.Value.TimeOfDay;
			startTimeTs = dateTimePicker1.Value.TimeOfDay;
			endTimeTs = dateTimePicker3.Value.TimeOfDay;
			int di = ((int)durationS.TotalSeconds);
			int startSeconds = (int)startTimeTs.TotalSeconds;
			int endSeconds = (int)endTimeTs.TotalSeconds;

			if(di == 0)
			{  //持续时间为0
				if(startSeconds >= endSeconds)
				{
					MessageBox.Show("起始时间不能大于等于结束时间 ！");
					return clibb;
				}
			}
			else
			{
				if(endSeconds != 0)
				{
					MessageBox.Show("持续时间非 0，结束时间必须为 0 ！");
					return clibb;
				}

				if(startSeconds >= endSeconds)
				{
					MessageBox.Show("起始时间不能大于等于结束时间！");
					return clibb;
				}

				if(di < 0)
				{
					MessageBox.Show("持续时间不能小于 0！");
					return clibb;
				}
			}
			outputFile = Path.Combine(textBox11.Text, $"{fileNameWithoutExtension}");
			outputFile = outputFile.TrimEnd('\r', '\n', ' ');
			if(!Directory.Exists(textBox11.Text))
			{
				MessageBox.Show("输出目录不存在，请先创建目录！");
				return clibb;
			}
			outputFile = outputFile + ".mp4";
			//??????????????
			if(totaltime == 0)
			{
				durationS = dateTimePicker3.Value.TimeOfDay;
				totaltime = (int)durationS.TotalSeconds;  // 转换为总秒数
				startTime = startTime + label19.Text;
				endTime = endTime + label21.Text;

			}
			return true; // 返回裁剪成功标志
		}
		/*使用FFmpeg精确到毫秒裁剪视频段，可以使用以下命令：
``
ffmpeg -ss 起始时间 -to 结束时间 -accurate_seek -i 输入文件 -c:v copy -c:a copy 输出文件
```
- **参数解释**：
    - `-ss`：指定裁剪的起始时间，格式为`HH:MM:SS.MILLISECONDS`，例如`00:00:10.500`表示从第10秒500毫秒开始。
    - `-to`：指定裁剪的结束时间，格式与`-ss`相同，如`00:00:20.750`表示裁剪到第20秒750毫秒结束。
    - `-accurate_seek`：该参数可使裁剪时间更加精确，必须放在`-i`参数之前。
    - `-i`：用于指定输入视频文件。
    - `-c:v copy -c:a copy`：表示使用原始视频和音频的编码格式进行裁剪，保持视频和音频质量无损。

例如，要从`input.mp4`视频文件的第15秒200毫秒处开始，裁剪到第30秒500毫秒处，命令如下：
```
ffmpeg -ss 00:00:15.200 -to 00:00:30.500 -accurate_seek -i input.mp4 -c:v copy -c:a copy output.mp4
另外，如果想通过指定起始时间和持续时间来裁剪视频，可以使用`-t`参数，例如：
ffmpeg -ss 00:00:10.000 -t 00:00:20.500 -accurate_seek -i input.mp4 -c:v copy -c:a copy output.mp4
上述命令表示从`input.mp4`的第10秒开始，裁剪一个时长为20秒500毫秒的视频片段。
为了避免出现一些潜在的时间戳问题，特别是在使用`-c copy`时，还可以加上`-avoid_negative_ts 1`参数。例如：
ffmpeg -ss 00:00:15.200 -to 00:00:30.500 -accurate_seek -i input.mp4 -c:v copy -c:a copy -avoid_negative_ts 1 output.mp4
		*/
		private async void CutVideoSegment(string startTime, string endTime, string inputFile, string outputFile)
		{
			if(isCutting)
			{ // 如果选中则复制到指定目录
				MessageBox.Show("剪切视频正在运行！");
			}
			if(checkBox2.Checked)
			{ // 如果选中则复制到指定目录
				if(!Directory.Exists(textBox7.Text))
				{
					MessageBox.Show("剪切视频备份目录不存在，请先创建或选择目录！");
					return;
				}
			}
			isCutting = true; // 设置正在裁剪标志
			textBox10.Text = ""; // 清空裁剪时间文本框
			textBox6.Text = "";
			string arguments = $"-ss {startTime} -to {endTime}  -accurate_seek -i {inputFile}  -c:v copy -c:a copy -avoid_negative_ts 1 {outputFile}";
			try
			{
				label6.Text = "剪切中...";
				Process ffmpegProcess = new Process();
				ffmpegProcess.StartInfo.FileName = "ffmpeg.exe";
				ffmpegProcess.StartInfo.Arguments = arguments;
				ffmpegProcess.StartInfo.UseShellExecute = false;
				ffmpegProcess.StartInfo.RedirectStandardError = true;
				ffmpegProcess.StartInfo.CreateNoWindow = true;
				// 注册输出数据处理事件
				var outputBuilder = new StringBuilder();
				ffmpegProcess.ErrorDataReceived += (eventSender, args) =>
				{
					if(!string.IsNullOrEmpty(args.Data))
					{
						outputBuilder.AppendLine(args.Data);
						string timeInfo = ParseProgress(args.Data);
						if(!string.IsNullOrEmpty(timeInfo))
						{ // 跨线程安全更新UI
							this.Invoke((MethodInvoker)delegate
							{
								textBox10.Text = $"裁剪时间: {timeInfo}";
							});
						}
					}
				};
				ffmpegProcess.ErrorDataReceived += (eventSender, args) =>
				{
					if(!string.IsNullOrEmpty(args.Data))
					{
						outputBuilder.AppendLine(args.Data);
						this.Invoke((MethodInvoker)delegate
						{
							textBox6.Text = $"剪切进度: {args.Data}";
						});
					}
				};
				ffmpegProcess.Start();
				ffmpegProcess.BeginErrorReadLine();
				await Task.Run(() => ffmpegProcess.WaitForExit());
				ffmpegProcess.CancelErrorRead();
				ffmpegProcess.Close();
				textBox6.Text = outputBuilder.ToString();
				label6.Text = "剪切完成！";
				if(checkBox2.Checked)
				{        //copy file
					checkBox2.Checked = false;
					// 如果选中则复制到指定目录
					CopyFile(outputFile, textBox7.Text);
				}
			}
			catch(Exception ex)
			{
				label6.Text = "剪切失败: ";
				textBox6.Text = "剪切失败: " + ex.Message;
			}
			isCutting = false;
		}
		private void CopyFile(string sourceFilePath, string destinationDirectory) //copy file
		{
			try
			{
				// 检查源文件是否存在
				if(!File.Exists(sourceFilePath))
				{
					MessageBox.Show("源文件不存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// 检查目标目录是否存在，不存在则创建
				if(!Directory.Exists(destinationDirectory))
				{
					Directory.CreateDirectory(destinationDirectory);
				}

				// 获取源文件的文件名
				string fileName = Path.GetFileName(sourceFilePath);

				// 构造目标文件路径
				string destinationFilePath = Path.Combine(destinationDirectory, fileName);

				// 复制文件，覆盖已存在的文件
				File.Copy(sourceFilePath, destinationFilePath, true);
				//MessageBox.Show( $"文件已成功复制到：{destinationFilePath}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information );
			}
			catch(Exception ex)
			{
				MessageBox.Show($"文件复制失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		//播放选择视频
		private void button10_Click(object sender, EventArgs e)
		{  // 播放选择视频
			if(textBox1 == null || string.IsNullOrEmpty(textBox1.Text))
			{
				MessageBox.Show("请先选择视频文件！");
				return;
			}
			_mediaPlayer.Mute = true; // 静音
			button9_Click(null, null); // 调用播放按钮事件
			PlayVideo(textBox1.Text);
			//_mediaPlayer.uiMode = "full"; // 或 "mini"
			_mediaPlayer.Play();
			waitingForBuffer = true;
			_mediaPlayer.Mute = false; // 静音
			timer1.Start(); // 播放时启动定时器
							//}
		}
		private void button11_Click(object sender, EventArgs e)
		{
			_mediaPlayer.Pause();
		}
		//取得时间戳
		private void button12_Click(object sender, EventArgs e)
		{
			double position;                  // i转换为时间标准时间格式0:00:00.000
			position = _mediaPlayer.Time;
			TimeSpan pos = TimeSpan.FromSeconds(position);
			if(timeStamp == 0)
			{
				label10.Text = pos.ToString(@"hh\:mm\:ss"); // 显示到毫秒
				label16.Text = pos.ToString(@"\.fff"); // 显示到毫秒
			}
			if(timeStamp == 1)
			{
				label11.Text = pos.ToString(@"hh\:mm\:ss"); // 显示到毫秒
				label17.Text = pos.ToString(@"\.fff"); // 显示到毫秒
			}
			if(timeStamp == 2)
			{
				label12.Text = pos.ToString(@"hh\:mm\:ss"); // 显示到毫秒
				label18.Text = pos.ToString(@"\.fff"); // 显示到毫秒
			}
			timeStamp++;

		}

		private void Cut_Load(object sender, EventArgs e)
		{
			//_mediaPlayer.PlayStateChange += _mediaPlayer_PlayStateChange;
		}
		private void _mediaPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
		{
			// 3 = Playing, 6 = Buffering
			if(waitingForBuffer && e.newState == 3)
			{
				waitingForBuffer = false;
				_mediaPlayer.Play();
			}
		}
		private void comboBoxSpeed_SelectedIndexChanged(object sender, EventArgs e)
		{
			float rate = 1.0f; // 默认速度为1.0x
			switch(comboBoxSpeed.SelectedItem.ToString())
			{
				case "0.1x":
					rate = 0.1f;
					break;
				case "0.25x":
					rate = 0.25f;
					break;
				case "0.5x":
					rate = 0.5f;
					break;
				case "1.0x":
					rate = 1.0f;
					break;
				case "2.0x":
					rate = 2.0f;
					break;
				case "5.0x":
					rate = 5.0f;
					break;
				case "10.0x":
					rate = 10.0f;
					break;
			}
			//string rateStr = rate.ToString( "F1" );
			float rataf = (float)Convert.ToDouble(rate.ToString("F1"));
			//_mediaPlayer.SetRate( rataf ) ;
			//_mediaPlayer.Mute = true; // 静音
		}
		private void button14_Click(object sender, EventArgs e)
		{
			startTime = label10.Text;
			DateTime endDateTime = DateTime.Parse("2023-01-01 " + startTime);
			dateTimePicker1.Value = endDateTime;
			label19.Text = label16.Text;
		}
		//选择结束时间
		private void button15_Click(object sender, EventArgs e)
		{
			endTime = label10.Text;
			dateTimePicker3.Value = DateTime.Parse("2023-01-01 " + endTime);
			label21.Text = label16.Text;
		}
		private void button13_Click(object sender, EventArgs e)
		{
			_mediaPlayer.Play();
			timer1.Start(); // 恢复进度条刷新
		}
		private void button20_Click(object sender, EventArgs e)
		{   //stop show
			_mediaPlayer.Stop();
			//firstdisp = true;
		}
		private void button17_Click(object sender, EventArgs e)
		{
			startTime = label11.Text;
			dateTimePicker1.Value = DateTime.Parse("2023-01-01 " + startTime);
			label19.Text = label17.Text;
		}
		private void button16_Click(object sender, EventArgs e)
		{
			endTime = label11.Text;
			dateTimePicker3.Value = DateTime.Parse("2023-01-01 " + endTime);
			label21.Text = label17.Text;
		}
		private void button19_Click(object sender, EventArgs e)
		{  //作为起始时间
			startTime = label12.Text;
			dateTimePicker1.Value = DateTime.Parse("2023-01-01 " + startTime);
			label19.Text = label18.Text;
		}
		private void button18_Click(object sender, EventArgs e)
		{
			endTime = label12.Text;
			dateTimePicker3.Value = DateTime.Parse("2023-01-01 " + endTime);
			label21.Text = label18.Text;
		}
		private void button21_Click(object sender, EventArgs e)
		{
			// 跳转到指定时间（秒）
			_mediaPlayer.Time = (long)dateTimePicker4.Value.TimeOfDay.TotalSeconds;
		}
		private void tabPage6_Paint(object sender, PaintEventArgs e)
		{           // 创建画笔  画一条线
					//using (Pen pen = new Pen( Color.Red, 1 )) // 红色，线宽2
					//{  // 画一条从 (10, 10) 到 (10, 100) 的直线
					//	e.Graphics.DrawLine( pen, 6, 430, 500, 430 );
					//}
		}
		private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
		{  // 绘制 ListBox 的项 。改变 ListBox 字体大小和颜色
			if(e.Index < 0)
				return;            // 获取当前项的文本
			string text = listBox1.Items[e.Index].ToString();

			Font font = new Font("微软雅黑", 12, FontStyle.Bold);
			Brush textBrush = Brushes.Black; // 默认黑色字体
			Brush backgroundBrush = Brushes.White; // 默认白色背景
												   // 根据条件设置不同的颜色（可选）
			if(e.Index % 2 == 0) // 偶数项
			{
				textBrush = Brushes.Blue;
				backgroundBrush = Brushes.LightGray;
			}
			// 绘制背景
			e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
			// 绘制文本
			e.Graphics.DrawString(text, font, textBrush, e.Bounds);
			// 绘制边框
			e.DrawFocusRectangle();
		}
		private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			// 这里用和 DrawItem 一致的字体
			//Font font = new Font( "微软雅黑", 20, FontStyle.Bold );
			//e.ItemHeight = (int)font.GetHeight() + 8; // 适当加点padding
		}
		private void button23_Click(object sender, EventArgs e)
		{  //视频后退
		   //判断当前视频是否正在播放
			if(!_mediaPlayer.IsPlaying)
			{
				MessageBox.Show("请先播放视频！");
				return;
			}
			double Time = _mediaPlayer.Time;
			if(Time < 0)
				return;
			double newPosition = Time - dateTimePicker5.Value.TimeOfDay.TotalSeconds;
			; // 后退5秒
			if(newPosition < 0)
				return;
			_mediaPlayer.Time = (long)newPosition;
		}
		private void button24_Click(object sender, EventArgs e)
		{
			//判断当前视频是否正在播放
			if(!_mediaPlayer.IsPlaying)
			{
				MessageBox.Show("请先播放视频！");
				return;
			}
			double Time = _mediaPlayer.Time;
			//取得当前视频长度
			double duration = _mediaPlayer.Time;
			Time = dateTimePicker5.Value.TimeOfDay.TotalSeconds + Time; // 前进5秒
			if(Time > duration)
				return;
			_mediaPlayer.Time = (long)Time;
		}
		private void button25_Click(object sender, EventArgs e)
		{  //打开视频声音
			_mediaPlayer.Mute = false; // 取消静音
		}
		private void button26_Click(object sender, EventArgs e)
		{
			_mediaPlayer.Mute = true; // 取消静音
		}
		private void button28_Click(object sender, EventArgs e)  //裁剪去结尾部分
		{
			if(dateTimePicker3.Value.TimeOfDay.TotalSeconds != 0)
			{
				MessageBox.Show("结束时间必须 0 ！");
				return;
			}
			if(Clip())
			{
				if(totalSeconds != 0)
				{        //获得整个视频时间
					double duration = _mediaPlayer.Time;
					TimeSpan dur = TimeSpan.FromSeconds(duration);
					string endss = $"{dur:hh\\:mm\\:ss\\.fff}";
					CutVideoSegment(endTime, endss, inputFile, outputFile);
				}
			}
		}

		void killProcess()
		{
			Process[] processes = Process.GetProcessesByName("ffmpeg");
			if(processes.Length > 0)
			{
				foreach(Process process in processes)
				{
					process.Kill();
				}
			}
		}

		private void tabPage1_Click(object sender, EventArgs e)
		{

		}

		//时间清零 为选择一个时间段做准备
		private void button9_Click(object sender, EventArgs e)
		{
			dateTimePicker1.Value = new DateTime(2023, 1, 1, 0, 0, 0);
			dateTimePicker2.Value = new DateTime(2023, 1, 1, 0, 0, 0);
			dateTimePicker3.Value = new DateTime(2023, 1, 1, 0, 0, 0);
			dateTimePicker4.Value = new DateTime(2025, 7, 1, 0, 0, 10);
			dateTimePicker5.Value = new DateTime(2023, 1, 1, 0, 0, 5);
			dateTimePicker6.Value = new DateTime(2025, 7, 1, 0, 0, 5);
			timeStamp = 0;
			startTime = "00:00:00"; // 起始时间
			endTime = "00:00:00"; // 结束时间
			duration = "00:00:00"; // 持续时间
			firstdisp = true;
		}
		private string CalcDuration(string startTime, string endTime)
		{
			TimeSpan start = TimeSpan.Parse(startTime);
			TimeSpan end = TimeSpan.Parse(endTime);
			if(end < start)
			{
				MessageBox.Show("结束时间必须大于起始时间！");
				return null;
			}
			TimeSpan durationSpan = end - start;
			return durationSpan.ToString(@"hh\:mm\:ss");
		}

		private void button30_Click(object sender, EventArgs e)  //选择裁剪输出目录
		{
			using(FolderBrowserDialog folderDialog = new FolderBrowserDialog())
			{
				folderDialog.Description = "请选择一个文件夹";
				folderDialog.ShowNewFolderButton = true; // 是否允许新建文件夹
				if(folderDialog.ShowDialog() == DialogResult.OK)
				{
					textBox11.Text = folderDialog.SelectedPath;
				}
			}
		}



		#endregion

		#region ------- 删除ffmpeg进程 -------		
		private void button39_Click(object sender, EventArgs e) //删除ffmpeg 进程
		{       //扫描所有进程

			Process[] processes;            // 检查管理员权限
											//if (!IsRunningAsAdmin()) {
											//	MessageBox.Show( "请以管理员身份运行程序" );
											//	RestartAsAdmin();
											//	return;
											//}
											//TerminateProcess( "AlibabaProtect" );
											//TerminateProcess( "AliPaladin" );  // 可能存在的关联进程

			//// 清空 ListBox 或其他显示控件（假设有一个 listBox1 用于显示）
			//listBox3.Items.Clear();
			//_mediaPlayer.Visible = false;
			//try {       // 获取所有正在运行的进程
			//	processes = Process.GetProcesses();
			//	listBox3.Items.Clear();
			//	// 遍历所有进程
			//	foreach (Process process in processes) {
			//		try {
			//			// 获取进程名称和 ID
			//			string processInfo = $"名称: {process.ProcessName}, ID: {process.Id}";
			//			if (process.ProcessName != "svchost")
			//				listBox3.Items.Add( processInfo );
			//		}
			//		catch (Exception ex) {
			//			// 如果无法获取某些进程信息，记录错误
			//			listBox3.Items.Add( $"无法获取进程信息: {ex.Message}" );
			//		}
			//	}
			//	//listBox3 排序	
			//	var items = new List<string>();
			//	foreach (var item in listBox3.Items) {
			//		items.Add( item.ToString() );
			//	}
			//	items.Sort(); // 默认按字符串升序排序
			//	listBox3.Items.Clear();
			//	//创建文件，将数据写入文件中
			//	//string filePath = Path.Combine( Application.StartupPath, "process_list.txt" );
			//	//try {
			//	//	File.WriteAllLines( filePath, items );
			//	//	label9.Text = $"进程信息已写入文件：{filePath}" + "写入成功";
			//	//}
			//	//catch (Exception ex) {
			//	//	label9.Text = $"写入文件失败: {ex.Message}" + "错误";
			//	//}

			//	foreach (var item in items) {
			//		listBox3.Items.Add( item );
			//	}
			//}
			//catch (Exception ex) {
			//	MessageBox.Show( $"扫描进程时出错: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
			//}

			processes = Process.GetProcessesByName("ffmpeg");
			if(processes.Length > 0)
			{
				foreach(Process process in processes)
				{
					process.Kill();
				}
				label9.Text = "已删除 ffmpeg 进程！";
			}
			else
			{
				label9.Text = "没有找到ffmpeg 进程！";
			}

			processes = Process.GetProcessesByName("KanKan");  //AlibabaProtect
			try
			{
				if(processes.Length > 0)
				{
					foreach(Process process in processes)
					{
						process.Kill();
					}
					label9.Text = "已删除 KanKan 进程！";
				}
				else
				{
					label9.Text = "KanKan 进程！";
				}
			}
			catch(Win32Exception ex)
			{
				MessageBox.Show($"Failed to terminate process : {ex.Message}");
			}
		}
		private void button29_Click(object sender, EventArgs e)
		{
			using(FolderBrowserDialog folderDialog = new FolderBrowserDialog())
			{
				folderDialog.Description = "请选择一个文件夹";
				folderDialog.ShowNewFolderButton = true; // 是否允许新建文件夹
				if(folderDialog.ShowDialog() == DialogResult.OK)
				{
					textBox7.Text = folderDialog.SelectedPath;
				}
			}
		}
		static void TerminateProcess(string processName)
		{
			try
			{
				// 获取所有匹配进程
				var processes = Process.GetProcessesByName(processName);

				if(processes.Length == 0)
				{

					MessageBox.Show($"未找到进程: {processName}");
					return;
				}

				MessageBox.Show($"发现 {processes.Length} 个{processName}进程");

				// 结束所有匹配进程
				foreach(var process in processes)
				{
					try
					{
						MessageBox.Show($"正在终止进程: {process.Id}");
						process.Kill();
						process.WaitForExit(3000);  // 等待进程退出
						MessageBox.Show($"成功终止: {process.Id}");
					}
					catch(Exception ex)
					{
						MessageBox.Show($"终止失败({process.Id}): {ex.Message}");
					}
				}
			}
			catch(Exception globalEx)
			{
				MessageBox.Show($"全局错误: {globalEx.Message}");
			}
		}

		// 检查管理员权限
		static bool IsRunningAsAdmin()
		{
			var identity = WindowsIdentity.GetCurrent();
			var principal = new WindowsPrincipal(identity);
			return principal.IsInRole(WindowsBuiltInRole.Administrator);
		}

		// 重启程序并申请管理员权限
		static void RestartAsAdmin()
		{
			var exePath = Process.GetCurrentProcess().MainModule.FileName;
			var startInfo = new ProcessStartInfo
			{
				FileName = exePath,
				UseShellExecute = true,
				Verb = "runas"  // 触发UAC提权
			};

			try
			{
				Process.Start(startInfo);
			}
			catch(Exception ex)
			{
				MessageBox.Show($"重启失败: {ex.Message}");
			}
			Environment.Exit(0);
		}
		#endregion

		#region ------- 视频旋转 -------
		/*1. 顺时针旋转 90 度
		bash
		ffmpeg -i input.mp4 -vf "rotate=90*(PI/180)" output.mp4
		2. 逆时针旋转 90 度
		bash
		ffmpeg -i input.mp4 -vf "rotate=270*(PI/180)" output.mp4
		3. 旋转 180 度
		bash
		ffmpeg -i input.mp4 -vf "rotate=180*(PI/180)" output.mp4
		4. 依据视频元数据自动旋转（适合手机拍摄的视频）
		bash
		ffmpeg -i input.mp4 -metadata:s:v:0 rotate=0 -c:v copy -c:a copy output.mp4
		命令参数说明
-i input.mp4：用于指定输入视频的文件路径。
-vf：即-filter:v的简写，是用来设置视频滤镜的参数。
rotate=90*(PI/180)：此表达式中角度需以弧度为单位，所以用角度值乘以PI/180来完成转换。
-metadata:s:v:0 rotate=0：该参数的作用是移除视频元数据里的旋转标记。
-c:v copy和-c:a copy：它们能让视频和音频直接复制，无需重新编码，从而加快处理速度。
		要将视频向右旋转 1 度（顺时针），可以使用以下命令：
ffmpeg -i input.mp4 -vf "rotate=1*PI/180:c=black" -preset medium -crf 23 output.mp4
参数说明：
rotate=1*PI/180：将视频顺时针旋转 1 度（FFmpeg 使用弧度制，因此将角度转换为弧度）
c=black：指定旋转后填充边缘的颜色为黑色（可选参数）
-preset medium：编码速度与质量的平衡设置（可选）
-crf 23：视频质量参数，默认值 23，越小质量越高（可选）
		如果需要更精确的控制（如旋转中心点、插值算法），可以扩展rotate滤镜的参数：
ffmpeg -i input.mp4 -vf "rotate=1*PI/180:ow=ceil(iw*1.01):oh=ceil(ih*1.01):bilinear=1:c=black" output.mp4
这个扩展命令中：
ow=ceil(iw*1.01)：输出宽度增加 1%，避免内容被裁剪
oh=ceil(ih*1.01)：输出高度增加 1%
bilinear=1：使用双线性插值提高旋转后的画质
要将视频向左旋转 1 度（逆时针），只需将旋转角度改为 -1 度（或等效为 359 度）即可。以下是具体命令：

bash
ffmpeg -i input.mp4 -vf "rotate=-1*PI/180:c=black" -preset medium -crf 23 output.mp4

参数说明：
rotate=-1*PI/180：将视频逆时针旋转 1 度（负号表示逆时针方向）
c=black：指定旋转后填充边缘的颜色为黑色（可选）
-preset medium：编码速度与质量的平衡设置（可选）
-crf 23：视频质量参数，默认值 23，越小质量越高（可选）
等效写法：
如果你更喜欢使用正数角度，可以将 -1度 转换为 359度（360-1）：
bash
ffmpeg -i input.mp4 -vf "rotate=359*PI/180:c=black" output.mp4

注意事项：
逆时针旋转会使视频内容向左倾斜，边缘可能出现黑边
如需避免黑边，可以结合使用 scale 或 crop 滤镜
旋转操作需要重新编码视频，建议保持足够的输出码率

如果需要更精确的控制（如旋转中心点、插值算法），可以扩展 rotate 滤镜的参数：
		这个扩展命令中：

ow=ceil(iw*1.01)：输出宽度增加 1%，避免内容被裁剪
oh=ceil(ih*1.01)：输出高度增加 1%
bilinear=1：使用双线性插值提高旋转后的画质

		*/
		private void button32_Click(object sender, EventArgs e)
		{
			right_rotate_angle++; // 右旋转角度增加
			if(right_rotate_angle >= 360)
			{
				right_rotate_angle = 0; // 重置角度
			}
			label8.Text = "右旋转角度:" + right_rotate_angle.ToString() + "°";
		}
		private void button34_Click(object sender, EventArgs e)
		{
			right_rotate_angle = right_rotate_angle + 10; // 右旋转角度增加
			if(right_rotate_angle >= 360)
			{
				right_rotate_angle = 0; // 重置角度
			}
			label8.Text = "右旋转角度:" + right_rotate_angle.ToString() + "°";
		}
		private void button33_Click(object sender, EventArgs e)
		{

		}
		private void button38_Click(object sender, EventArgs e)
		{
			right_rotate_angle = 0;
			left_rotate_angle = 0;
			label8.Text = "视频旋转为 0°"; // 重置角度显示
		}
		private void button31_Click(object sender, EventArgs e) //旋转视频
		{
			clibb = false;
			_mediaPlayer.Pause();  // 暂停视频播放
			textBox6.Text = "";
			inputFile = textBox1.Text;
			if(string.IsNullOrEmpty(inputFile) || !File.Exists(inputFile))
			{
				MessageBox.Show("请选择有效的视频文件！");
				return;
			}
			inputFile = inputFile.TrimEnd('\r', '\n', ' ');
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFile);
			//当前时间 转换为字符串  F:\newipad\已经压缩\Aigirl3.mp4
			fileNameWithoutExtension = fileNameWithoutExtension + DateTime.Now.ToString("_HHmmss");
		}


		#endregion

		#region  --------------  VLC 视频播放   -------------
		private void PlayVideo(string filePath)
		{
			if(File.Exists(filePath))
			{
				var media = new Media(_libVLC, new Uri(filePath));
				_mediaPlayer.Play(media);
			}
			else
			{
				MessageBox.Show("视频文件不存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}


		private void button41_Click(object sender, EventArgs e)
		{
			button9_Click(null, null); // 调用播放按钮事件
			_mediaPlayer.Mute = true; // 静音
									  //_mediaPlayer.uiMode = "full"; // 或 "mini"
			_mediaPlayer.Mute = false; // 取消静音
			PlayVideo(textBox1.Text);
			timer1.Start(); // 播放时启动定时器
		}

		private void Cut_MouseDown(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
			{
				// 释放鼠标捕获并发送消息以模拟拖动窗口
				ReleaseCapture();
				SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
			}
		}

		private void button3_Click_1(object sender, EventArgs e)
		{
			//maximize
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
		{
			//minimize
			if(WindowState == FormWindowState.Minimized)
			{
				WindowState = FormWindowState.Normal; // 恢复到正常状态
			}
			else
			{
				WindowState = FormWindowState.Minimized; // 最小化窗口
			}
		}

		private void buttonX1_Click(object sender, EventArgs e)
		{
			//跳转到changepictuer窗口
			//this.Hide();
			ChangePictuer change = new ChangePictuer();
			change.Show();

		}

		// 暂停
		private void PauseVideo()
		{
			_mediaPlayer.Pause();
		}

		// 停止
		private void StopVideo()
		{
			_mediaPlayer.Stop();
		}

		// 调整音量（0~100）
		private void SetVolume(int volume)
		{
			_mediaPlayer.Volume = volume;
		}

		// 设置播放进度（单位：秒）
		private void SetPosition(double seconds)
		{
			if(_mediaPlayer.Length > 0)
				_mediaPlayer.Time = (long)(seconds * 1000); // 毫秒
		}

		// 获取当前进度（单位：秒）
		private double GetPosition()
		{
			return _mediaPlayer.Time / 1000.0;
		}


		#endregion

	}


}


