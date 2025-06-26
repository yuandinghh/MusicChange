using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MusicChange {
	public partial class Form1 : Form {
		public struct FileItem {
			public string FileName;      // 文件名
			public string FilePath;      // 完整路径
			public long FileSize;        // 文件大小
			public DateTime CreationTime;// 创建时间
		};
		Font newFont = new Font( "宋体", 14f ); // 创建一个新的字体对象，设置字体名称、大小等属性
		private List<FileItem> ImageList = new List<FileItem>();
		public Form1( ) {
			InitializeComponent();
			listBox1.Font = newFont;
			listBox2.Font = newFont;
		}
		//根据路径和文件名，获取文件的所有信息
		private void GetFileInfo(string filePath) {
			try {
				// 创建 FileInfo 对象
				FileInfo fileInfo = new FileInfo( filePath );
				// 检查文件是否存在
				if (!fileInfo.Exists) {
					MessageBox.Show( $"文件不存在: {filePath}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
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
				MessageBox.Show( fileDetails, "文件信息", MessageBoxButtons.OK, MessageBoxIcon.Information );
			}
			catch (Exception ex) {
				MessageBox.Show( $"获取文件信息失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
			/// <summary>
		/// 按钮1点击事件 视频提取音频
		/// </summary>
		/// <returns>两数之和</returns>
		private void Button1_Click(object sender, EventArgs e) {
			int count = 0;
			var path = @"G:\刀郎的主页";
			List<string> imageList = new List<string>();
			if (textBox1.Text == "") {
				path = @"G:\刀郎的主页";  //@"D:\音乐";
			}
			else {
				path = textBox1.Text;
			}
			try {
				var timagefiles = Directory.GetFiles( path, "*.*" ).Where( file => file.ToLower().EndsWith( "mp4" ) ).ToList();
				imageList.AddRange( timagefiles );
				label2.Text = "转换Mp4-》Aac：" + count.ToString();
				if (imageList.Count == 0) {
					MessageBox.Show( "没有找到符合条件的文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information );
					return;
				}
				else {       // 遍历文件列表并添加到 ListBox    string filestr = timagefiles[0].ToString();
					foreach (string file in imageList) {
						listBox1.Items.Add( file );
						// string fileName = Path.GetFileName(file);
						//取消文件扩展名
						string fileNameWithoutExtension = Path.GetFileNameWithoutExtension( file );
	//运行程序  ffmpeg -i x.mp4 -vn -c:a copy o.aac
						// 启动外部运行程序 ffmpeg -i input.mp4 output.wav
						string arg = $"-i {path}\\{fileNameWithoutExtension}.mp4 -vn -c:a copy  {path}\\{fileNameWithoutExtension}.aac";
						//判断文件是否存在，如果存在则删除
						string outputFile = Path.Combine( path, $"{fileNameWithoutExtension}.aac" );
						if (File.Exists( outputFile )) {   //			File.Delete( outputFile ); // 删除已存在的输出文件
																	  ////  string filenamee = "input";	   string arguments = $"-i {filenamee}.mp4 -vn -c:a copy output.wav";
																	  //ProcessStartInfo startInfo = new ProcessStartInfo {
																	  //	FileName = "ffmpeg.exe", // 确保路径正确
																	  //	Arguments = arg, // 将 input.mp4 转换为 output.wav
																	  //	UseShellExecute = false,
																	  //	RedirectStandardOutput = true,
																	  //	RedirectStandardError = true,// 重定向 stderr
																	  //	CreateNoWindow = true
																	  //};
																	  //string output, error;
																	  //Process process = null; // 定义 Process 对象
																	  //try {


							// 假设输入输出文件路径如下
							string ffmpegPath = @"D:\ffmpeg.exe"; // ffmpeg.exe 的完整路径
							//string inputFile = @"G:\刀郎的主页\video.mp4";
							//string outputFile = @"G:\刀郎的主页\video.aac";

							// 构造参数							string arguments = $"-i \"{file}\" -vn -c:a copy \"{outputFile}\"";
							string arguments = $"-i {file} -vn -c:a copy {outputFile}";

							ProcessStartInfo startInfo = new ProcessStartInfo {
								FileName = ffmpegPath,
								Arguments = arguments,
								UseShellExecute = false,
								RedirectStandardOutput = true,
								RedirectStandardError = true,
								CreateNoWindow = true
							};

							using (Process process = new Process { StartInfo = startInfo }) {
								process.Start();
								//string output = process.StandardOutput.ReadToEnd();
								string error = process.StandardError.ReadToEnd();
								process.WaitForExit();

								// 可根据 error 判断是否成功
								if (!string.IsNullOrEmpty( error )) {
									MessageBox.Show( $"FFmpeg 错误: {error}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
								}
								else {
									MessageBox.Show( "转换成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information );
								}
							}

						}


		label2.Text = "转换Mp4-》Aac：" + count.ToString();
					}
				}
			}
			catch (Exception ex) {
				Console.WriteLine( $"Processing failed: {ex.Message}" );   //LogError( e, "Shape processing failed." );
				throw;
			}  //   GetFileInfo(listBox2.GetItemText(0).ToLower());  “GetFileInfo” 通常是一个用于获取文件相关信息的函数或方法。
		}
		/// <summary>
		/// 按钮2 查找目录文件的重复文件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button2_Click(object sender, EventArgs e) {
			int count = 0;
			var path = @"D:\Documents\Camera Roll";
			//	List<string> imageList = new List<string>();
			if (textBox2.Text == "") {
				path = @"D:\Documents\Camera Roll";
			}
			else {
				path = textBox2.Text;
			}
			try {
				//		var timagefiles = Directory.GetFiles( path, "*.*" ).Where( file => file.ToLower().EndsWith( "gif" ) || file.ToLower().EndsWith( "jpeg" ) ).ToList();
				ImageList.Clear();
				List<string> allFiles = new List<string>();
				// 遍历指定目录及其子目录下的所有文件 ， 使用递归方法遍历所有文件和目录
				TraverseAllFilesAndDirectories( path, allFiles );  // 之后可以将 allFiles 添加到 listBox1 或做其他处理
				foreach (var file in allFiles) {
					FileInfo fi = new FileInfo( file );
					FileItem item = new FileItem {
						FileName = fi.Name,
						FilePath = fi.FullName,
						FileSize = fi.Length,                  //	CreationTime = fi.CreationTime
						CreationTime = fi.LastWriteTime // 这里可以根据需要选择使用 CreationTime 或 LastAccessTime
					};
					ImageList.Add( item );
					listBox1.Items.Add( file );

				}
				count = allFiles.Count();
				label2.Text = "总共找到文件数：" + count.ToString();
				int addcount = 0;
				List<string> samefile = new List<string>();
				samefile.Clear();
				if (count == 0) {
					MessageBox.Show( "没有找到符合条件的文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information );
					return;
				}
				else {
					//ImageList 内部字符串 按照文件名排序	ImageList = ImageList.OrderBy( x => x.FileName ).ToList(); // 按照文件名排序
					//ImageList = ImageList.OrderBy( x => x.FileSize ).ToList(); // 按照文件大小排序
					//ImageList = ImageList.OrderBy( x => x.FileSize ).ThenBy( x => x.CreationTime ).ToList(); // 按照文件大小和创建时间排序
					for (int j = 0; j < count; j++) {
						string file = allFiles[j]; // 获取当前文件
						var selectedItem = listBox2.SelectedItem;
						listBox2.SelectionMode = SelectionMode.MultiExtended; // 或 MultiSimple
																								//	listBox2.Items.Add( "选择" );

						for (int i = 0; i < count; i++) {
							string fileName = ImageList[i].FilePath; // 获取文件名
							long filelenght = ImageList[i].FileSize;
							DateTime timeSpan = ImageList[i].CreationTime;
							if (fileName != file) {
								//if ((filelenght == ImageList[j].FileSize) && (timeSpan == ImageList[j].CreationTime)) {
								if ((filelenght == ImageList[j].FileSize)) {
									listView1.Items.Add( fileName ); // 添加到 ListView 中
																				// 如果文件名相同，或者文件大小相同，或者创建时间相同，则认为是重复的
																				// 在这里可以执行你想要的操作 MessageBox.Show( $"发现重复文件: {fileName}" );
									addcount++;
									// 这里可以添加到一个列表中，或者执行其他操作
									//listBox2 添加选择项
									if (!listBox2.Items.Contains( fileName )) { // 检查是否已经添加过该文件名
										listBox2.Items.Add( fileName );
									}
									if (!listBox2.Items.Contains( file )) { // 检查是否已经添加过该文件名
										listBox2.Items.Add( file );
									}
								}

							}
						}
					}
				}
				addcount = addcount / 2; // 每对重复文件会被计算两次，所以除以2
				label3.Text = "总共重复的文件数：" + addcount.ToString();

			}
			catch (Exception ex) {
				Console.WriteLine( $"Processing failed: {ex.Message}" );   //LogError( e, "Shape processing failed." );
				throw;
			}
		}

		private void button3_Click(object sender, EventArgs e) {

		}
		private void TraverseAllFilesAndDirectories(string rootPath, List<string> allFiles) {
			try {
				// 获取当前目录下的所有文件
				var files = Directory.GetFiles( rootPath );
				allFiles.AddRange( files );

				// 获取当前目录下的所有子目录
				var directories = Directory.GetDirectories( rootPath );
				foreach (var dir in directories) {
					// 递归遍历子目录
					TraverseAllFilesAndDirectories( dir, allFiles );
				}
			}
			catch (Exception ex) {
				// 可根据需要处理异常
				Console.WriteLine( $"遍历失败: {ex.Message}" );
			}
		}

		private void button4_Click(object sender, EventArgs e) {
			Application.Exit(); // 退出应用程序
		}
	}
}

