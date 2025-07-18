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
	public partial class Form1:Form
	{
		//	private string Filestr, inputFile, outputFile, arguments;
		public struct FileItem
		{
			public string FileName;      // 文件名
			public string FilePath;      // 完整路径
			public long FileSize;        // 文件大小
			public DateTime CreationTime;// 创建时间
		};
		Font newFont = new Font("宋体", 14f); // 创建一个新的字体对象，设置字体名称、大小等属性
		private List<FileItem> ImageList = new List<FileItem>();
		public Form1()
		{
			InitializeComponent();
			listBox1.Font = newFont;
			listBox2.Font = newFont;
		}
		//根据路径和文件名，获取文件的所有信息
		private void GetFileInfo(string filePath)
		{
			try
			{
				// 创建 FileInfo 对象
				FileInfo fileInfo = new FileInfo(filePath);
				// 检查文件是否存在
				if(!fileInfo.Exists)
				{
					MessageBox.Show($"文件不存在: {filePath}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				// 获取文件的详细信息
				string fileDetails = $"文件名: {fileInfo.Name}\n" +
								$"文件路径: {fileInfo.FullName}\n" +
								$"文件大小: {fileInfo.Length} 字节\n" +
								$"创建时间: {fileInfo.CreationTime}\n" +
								$"最后访问时间: {fileInfo.LastAccessTime}\n" +
								$"最后修改时间: {fileInfo.LastWriteTime}\n" +
								$"扩展名: {fileInfo.Extension}\n" +
								$"是否只读: {fileInfo.IsReadOnly}";

				// 显示文件信息
				MessageBox.Show(fileDetails, "文件信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch(Exception ex)
			{
				MessageBox.Show($"获取文件信息失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		/// <summary>
		/// 按钮1点击事件 视频提取音频
		/// </summary>
		/// <returns>两数之和</returns>
		private async void Button1_Click(object sender, EventArgs e)
		{
			int count = 0;
			var path = @"G:\刀郎的主页";
			List<string> imageList = new List<string>();
			if(textBox1.Text == "")
			{
				path = @"G:\刀郎的主页";  //@"D:\音乐";
			}
			else
			{
				path = textBox1.Text;
			}
			var timagefiles = Directory.GetFiles(path, "*.*").Where(file => file.ToLower().EndsWith("mp4")).ToList();
			imageList.AddRange(timagefiles);
			label2.Text = "转换Mp4-》Aac：" + count.ToString();
			if(imageList.Count == 0)
			{
				MessageBox.Show("没有找到符合条件的文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			else
			{       // 遍历文件列表并添加到 ListBox    string filestr = timagefiles[0].ToString();
				foreach(string file in imageList)
				{
					listBox2.Items.Add(file);
					// string fileName = Path.GetFileName(file);
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
					//运行程序  ffmpeg -i x.mp4 -vn -c:a copy o.aac 启动外部运行程序 ffmpeg -i input.mp4 output.wav
					//ffmpeg - i 放松轻音乐.mp4 - vn - acodec aac - b:a 192k 放松轻音乐.aac
					string arg = $"-i {path}\\{fileNameWithoutExtension}.mp4 -vn -acodec aac -b:a 192k   {path}\\{fileNameWithoutExtension}.aac";
					//判断文件是否存在，如果存在则删除
					string outputFile = Path.Combine(path, $"{fileNameWithoutExtension}.aac");
					if(!File.Exists(outputFile))
					{   //			File.Delete( outputFile ); // 删除已存在的输出文件
						try
						{
							label2.Text = "开始提取音频...";
							Process ffmpegProcess = new Process();
							ffmpegProcess.StartInfo.FileName = "ffmpeg.exe";
							ffmpegProcess.StartInfo.Arguments = arg;
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
									string timeInfo =Cut.ParseProgress(args.Data);
									if(!string.IsNullOrEmpty(timeInfo))
									{ // 跨线程安全更新UI
										this.Invoke((MethodInvoker)delegate
										{
											textBoxX1.Text = $"时间: {timeInfo}";
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
										textBoxX1.Text = $"" +	$"进度: {args.Data}";
									});
								}
							};
							ffmpegProcess.Start();
							ffmpegProcess.BeginErrorReadLine();
							await Task.Run(() => ffmpegProcess.WaitForExit());
							ffmpegProcess.CancelErrorRead();
							ffmpegProcess.Close();
							textBoxX1.Text = outputBuilder.ToString();
							label1.Text = "提取音频完成！";
					
						}
						catch(Exception ex)
						{
							label1.Text = "剪切失败: ";
							textBoxX1.Text = "提取音频失败: " + ex.Message;
						}
					}
				}
			}
		}

		private void button3_Click(object sender, EventArgs e)  //选择文件夹
		{
			using(FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
			{
				folderBrowserDialog.Description = "请选择一个文件夹";
				folderBrowserDialog.SelectedPath = $"F:\\下载\\edge";
				// 如果用户选择了文件夹
				if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
				{
					textBox1.Text = folderBrowserDialog.SelectedPath;

					string[] webpFiles = Directory.GetFiles(textBox1.Text, "*.*", SearchOption.TopDirectoryOnly); // 获取当前目录下的所有文件

					listBox1.Items.Clear();//选择的文件存入listbox
					foreach(string file in webpFiles)   // 将文件路径添加到 ListBox
					{
						listBox1.Items.Add(file);
					}
					// 显示文件列表
					if(webpFiles.Length > 0)
					{
						//string fileList = string.Join( Environment.NewLine, webpFiles );
						//MessageBox.Show( $"找到以下 .webp 文件:\n{fileList}", "文件列表" );
						label2.Text = $"找到 {webpFiles.Length} 个  文件。";
					}
					else
					{
						label2.Text = "未找到任何 文件。";
					}
				}
			}
		}

		private void TraverseAllFilesAndDirectories(string rootPath, List<string> allFiles)
		{
			try
			{
				var files = Directory.GetFiles(rootPath);// 获取当前目录下的所有文件
				allFiles.AddRange(files);
				// 获取当前目录下的所有子目录
				var directories = Directory.GetDirectories(rootPath);
				foreach(var dir in directories)
				{
					// 递归遍历子目录
					TraverseAllFilesAndDirectories(dir, allFiles);
				}
			}
			catch(Exception ex)
			{
				// 可根据需要处理异常
				Console.WriteLine($"遍历失败: {ex.Message}");
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			Application.Exit(); // 退出应用程序
		}
	}
}

