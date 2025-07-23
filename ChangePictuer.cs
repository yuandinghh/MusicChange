using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageMagick;

namespace MusicChange
{
	public partial class ChangePictuer : Form
	{
		public static int failCount = 0;
		public static int successCount;
		public ChangePictuer( )
		{
			InitializeComponent();
		}
		private void button1_Click(object sender, EventArgs e)
		{
			//选择目录和webp 类型文件
			using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()) {
				folderBrowserDialog.Description = "请选择一个文件夹";
				folderBrowserDialog.SelectedPath = $"F:\\下载";
				// 如果用户选择了文件夹
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
					string selectedPath = folderBrowserDialog.SelectedPath;
					string[] webpFiles = Directory.GetFiles( selectedPath, "*.webp", SearchOption.TopDirectoryOnly );
					//选择的文件存入listbox
					listBox1.Items.Clear();
					// 将文件路径添加到 ListBox
					foreach (string file in webpFiles) {
						listBox1.Items.Add( file );
					}
					// 显示文件列表
					if (webpFiles.Length > 0) {
						//string fileList = string.Join( Environment.NewLine, webpFiles );
						//MessageBox.Show( $"找到以下 .webp 文件:\n{fileList}", "文件列表" );
						label1.Text = $"找到 {webpFiles.Length} 个 .webp 文件。";
					}
					else {
						label1.Text = "未找到任何 .webp 文件。";
					}
				}
			}

		}
		private async void button2_Click(object sender, EventArgs e)  //批量转换webp为jpeg图片
		{
			successCount = 0;
			failCount = 0;
			textBox3.Text="";
			StringBuilder failFiles = new StringBuilder();
			foreach (var item in listBox1.Items) {
				string webpFile = item.ToString();
				if (!File.Exists( webpFile ) || Path.GetExtension( webpFile ).ToLower() != ".webp")
					continue;
				try {
					using (var image = new MagickImage( webpFile )) {
						string jpgFile = Path.ChangeExtension( webpFile, ".jpg" );
						//取得目录
						string directory = Path.GetDirectoryName( webpFile );
						image.Format = MagickFormat.Jpeg;
						//get file  name
						string fileName = Path.GetFileNameWithoutExtension( webpFile );
						//设置图片质量
						image.Quality = 100; // 设置JPEG质量为90	 //加上当前时间  显示毫秒
						string time = DateTime.Now.ToString( "HHmmss.fff" );
						fileName = directory + "\\" + fileName + time + ".jpg";
						//await Task.Run(() => LivpConverter.ConvertLivpToJpg(livpFile, directory, listBox2));
						await Task.Run( ( ) => image.Write( fileName ) );
						successCount++;
						textBox1.Text = $"转换成功为： {jpgFile}";
						//将转换后的文件添加到listbox2
						listBox2.Items.Add( fileName );
						//delete original webp file
						File.Delete( webpFile );
						textBox3.Text = $"成功删除：{webpFile}";
					}
				}
				catch (Exception ex) {
					failCount++;
					failFiles.AppendLine( $"{webpFile}: {ex.Message}" );
				}
				if (successCount > 0)
					label1.Text = $"批量转换完成！成功：{successCount}，失败：{failCount}";
				else
					label1.Text = "没有成功转换的文件。";

				if (failCount > 0)
					MessageBox.Show( $"以下文件转换失败：\n{failFiles}", "转换失败" );
			}
		}
		private void Form3_Load(object sender, EventArgs e)
		{

		}
		private async void button3_Click(object sender, EventArgs e)
		{
			if (listBox1.Items.Count == 0) {
				MessageBox.Show( "列表中没有文件。" );
				return;
			}

			successCount = 0;
			failCount = 0;
			StringBuilder failFiles = new StringBuilder();
			textBox1.Text = "";
			foreach (var item in listBox1.Items) {
				string heicFile = item.ToString();
				if (!File.Exists( heicFile ) || Path.GetExtension( heicFile ).ToLower() != ".heic")
					continue;
				try {
					using (var image = new ImageMagick.MagickImage( heicFile )) {
						string jpgFile = Path.ChangeExtension( heicFile, ".jpg" );
						image.Format = ImageMagick.MagickFormat.Jpeg;
						image.Quality = 100; // 设置JPEG质量
						await Task.Run( ( ) => image.Write( jpgFile ) );
						successCount++;
						textBox1.Text = "成功转换：" + successCount.ToString() + $"转换成功： {jpgFile}";
					}
				}
				catch (Exception ex) {
					failCount++;
					failFiles.AppendLine( $"{heicFile}: {ex.Message}" );
				}
			}

			if (successCount > 0)
				label1.Text = $"批量转换完成！成功：{successCount}，失败：{failCount}";
			else
				label1.Text = "没有成功转换的文件。";

			if (failCount > 0)
				MessageBox.Show( $"以下文件转换失败：\n{failFiles}", "转换失败" );
		}
		private void button4_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()) {
				folderBrowserDialog.Description = "请选择一个文件夹";
				// 如果用户选择了文件夹
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
					string selectedPath = folderBrowserDialog.SelectedPath;
					string[] webpFiles = Directory.GetFiles( selectedPath, "*.heic", SearchOption.TopDirectoryOnly );
					//选择的文件存入listbox
					listBox1.Items.Clear();
					// 将文件路径添加到 ListBox
					foreach (string file in webpFiles) {
						listBox1.Items.Add( file );
					}
					// 显示文件列表
					if (webpFiles.Length > 0) {
						//string fileList = string.Join( Environment.NewLine, webpFiles );
						//MessageBox.Show( $"找到以下 .webp 文件:\n{fileList}", "文件列表" );
						label1.Text = $"找到 {webpFiles.Length} 个 .Heic 文件。";
					}
					else {
						label1.Text = "未找到任何 .webp 文件。";
					}
				}
			}

		}
		private void button5_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()) {
				folderBrowserDialog.Description = "请选择一个文件夹";
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
					string selectedPath = folderBrowserDialog.SelectedPath;
					string[] livpFiles = Directory.GetFiles( selectedPath, "*.livp", SearchOption.TopDirectoryOnly );

					listBox1.Items.Clear();
					foreach (string file in livpFiles) {
						listBox1.Items.Add( file );
					}

					if (livpFiles.Length > 0)
						label1.Text = $"找到 {livpFiles.Length} 个 .livp 文件。";
					else
						label1.Text = "未找到任何 .livp 文件。";
				}
			}
		}
		private async void button6_Click(object sender, EventArgs e)
		{
			if (listBox1.Items.Count == 0) {
				MessageBox.Show( "列表中没有文件。" );
				return;
			}
			//取得listBox1.Items 文件的文件名的目录
			string directory = Path.GetDirectoryName( listBox1.Items[0].ToString() );
			int successCount = 0;

			foreach (var item in listBox1.Items) {
				string livpFile = item.ToString();
				if (!File.Exists( livpFile ) || Path.GetExtension( livpFile ).ToLower() != ".livp") {
					MessageBox.Show( "请选择一个有效的 .livp 文件。" );
					return;
				}
				//	await Task.Run( ( ) => ffmpegProcess.WaitForExit() );
				await Task.Run( ( ) => LivpConverter.ConvertLivpToJpg( livpFile, directory, listBox2 ) );
				successCount++;
				//delay 10 sec
				//System.Threading.Thread.Sleep( 10000 );
				textBox1.Text = $"转换成功：{livpFile}->为Jepg文件，共：{successCount}";

			}

			if (successCount > 0)
				label1.Text = $"批量转换完成！成功：{successCount}，失败：{failCount}";
			else
				label1.Text = "没有成功转换的文件。";

			//if (failCount > 0)
			//	MessageBox.Show( $"以下文件转换失败：\n{failFiles}", "转换失败" );
		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			textBox1.Text = $"转换成功共：{successCount}";
		}
		private void buttonX1_Click(object sender, EventArgs e)   //转换扩展名	
		{
			//转换扩展名
			if (listBox1.Items.Count == 0) {
				MessageBox.Show( "列表中没有文件。" );
				return;
			}
			foreach (var item in listBox1.Items) {
				string filePath = item.ToString();
				if (!File.Exists( filePath )) {
					MessageBox.Show( $"文件不存在: {filePath}" );
					continue;
				}
				string newFilePath = Path.ChangeExtension( filePath, textBox3.Text );
				if (filePath != newFilePath) {
					try {
						File.Move( filePath, newFilePath );
						listBox2.Items.Add( newFilePath );
					}
					catch (Exception ex) {
						MessageBox.Show( $"转换失败: {filePath}\n{ex.Message}" );
					}
				}
			}
		}
		private void buttonx2_Click(object sender, EventArgs e)  //选择目录
		{//选择文件夹
			using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()) {
				folderBrowserDialog.Description = "请选择一个文件夹";
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
					string selectedPath = folderBrowserDialog.SelectedPath;
					string[] jpgFiles = Directory.GetFiles( selectedPath, "*.txt", SearchOption.TopDirectoryOnly );
					listBox2.Items.Clear();
					foreach (string file in jpgFiles) {
						listBox1.Items.Add( file );
					}
					if (jpgFiles.Length > 0) {
						label1.Text = $"找到 {jpgFiles.Length} 个 .jpg 文件。";
					}
					else {
						label1.Text = "未找到任何 .jpg 文件。";
					}
				}
			}
		}
		private void buttonx4_Click(object sender, EventArgs e)
		{  //选择文件夹 音频
			using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()) {
				folderBrowserDialog.Description = "请选择一个文件夹";
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
					string selectedPath = folderBrowserDialog.SelectedPath;
					textBoxX2.Text = selectedPath;
					string[] jpgFiles = Directory.GetFiles( selectedPath, "*.mp4", SearchOption.TopDirectoryOnly );
					listBox1.Items.Clear();
					foreach (string file in jpgFiles) {
						listBox1.Items.Add( file );
					}
					if (jpgFiles.Length > 0) {
						label1.Text = $"找到 {jpgFiles.Length} 个 .jpg 文件。";
					}
					else {
						label1.Text = "未找到任何 .jpg 文件。";
					}
				}
			}

		}
		private async void buttonx3_Click(object sender, EventArgs e)
		{
			int count = 0;
			var path = @"G:\刀郎的主页";
			List<string> imageList = new List<string>();
			if (textBoxX2.Text == "") {
				path = @"G:\刀郎的主页";  //@"D:\音乐";
			}
			else {
				path = textBoxX2.Text;
			}
			var timagefiles = Directory.GetFiles( path, "*.*" ).Where( file => file.ToLower().EndsWith( "mp4" ) ).ToList();
			imageList.AddRange( timagefiles );
			label1.Text = "转换Mp4-》Aac：" + count.ToString();
			if (imageList.Count == 0) {
				MessageBox.Show( "没有找到符合条件的文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information );
				return;
			}
			else {       // 遍历文件列表并添加到 ListBox    string filestr = timagefiles[0].ToString();
				foreach (string file in imageList) {
					listBox2.Items.Add( file );
					// string fileName = Path.GetFileName(file);
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension( file );
					//运行程序  ffmpeg -i x.mp4 -vn -c:a copy o.aac 启动外部运行程序 ffmpeg -i input.mp4 output.wav
					//ffmpeg - i 放松轻音乐.mp4 - vn - acodec aac - b:a 192k 放松轻音乐.aac
					string arg = $"-i {path}\\{fileNameWithoutExtension}.mp4 -vn -acodec aac -b:a 192k   {path}\\{fileNameWithoutExtension}.aac";
					//判断文件是否存在，如果存在则删除
					string outputFile = Path.Combine( path, $"{fileNameWithoutExtension}.aac" );
					if (!File.Exists( outputFile )) {   //			File.Delete( outputFile ); // 删除已存在的输出文件
						try {
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
								if (!string.IsNullOrEmpty( args.Data )) {
									outputBuilder.AppendLine( args.Data );
									string timeInfo = Cut.ParseProgress( args.Data );
									if (!string.IsNullOrEmpty( timeInfo )) { // 跨线程安全更新UI
										this.Invoke( (MethodInvoker)delegate
									   {
										   textBox1.Text = $"时间: {timeInfo}";
									   } );
									}
								}
							};
							ffmpegProcess.ErrorDataReceived += (eventSender, args) =>
							{
								if (!string.IsNullOrEmpty( args.Data )) {
									outputBuilder.AppendLine( args.Data );
									this.Invoke( (MethodInvoker)delegate
								   {
									   textBox1.Text = $"" + $"进度: {args.Data}";
								   } );
								}
							};
							ffmpegProcess.Start();
							ffmpegProcess.BeginErrorReadLine();
							await Task.Run( ( ) => ffmpegProcess.WaitForExit() );
							ffmpegProcess.CancelErrorRead();
							ffmpegProcess.Close();
							textBox1.Text = outputBuilder.ToString();
							label1.Text = "提取音频完成！";

						}
						catch (Exception ex) {
							label1.Text = "剪切失败: ";
							textBox1.Text = "提取音频失败: " + ex.Message;
						}
					}
				}
			}
		}
		///根据路径和文件名，获取文件的所有信息
		private void TraverseAllFilesAndDirectories(string rootPath, List<string> allFiles)
		{
			try {
				var files = Directory.GetFiles( rootPath );// 获取当前目录下的所有文件
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
		private void GetFileInfo(string filePath)
		{
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
		private async void buttonX5_Click(object sender, EventArgs e) //将目录下的所有文件改名存到别的目录
		{
			int count = 0;
			List<string> allFiles = new List<string>();
			TraverseAllFilesAndDirectories( textBoxX3.Text, allFiles );
			foreach (string file in allFiles) {
				//get extension 
				string extension = Path.GetExtension( file );

				string fileName = Path.GetFileName( file );
				//取得目录		string path = Path.GetDirectoryName(file);
				//get current timn
				string currentTime = DateTime.Now.ToString( "mmss.fff" );  //yyyyMMddHHmmss
																		   //get file name without extension
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension( file );
				//
				string newFileName = textBoxX1.Text + "\\" + fileNameWithoutExtension + currentTime + extension;
				//将filename改名为newfilename

				await Task.Run( ( ) => File.Copy( file, newFileName, true ) );
				count++;
				textBox1.Text = "文件已复制:" + count + "个";
				//	File.Move(newFileName, textBoxX1.Text);
			}
			textBox1.Text = "文件已复制:" + count + "个" + "  完成！";


		}
		public static void CopyDirectory(string sourceDir, string targetDir, bool overwrite = true)
		{
			var source = new DirectoryInfo( sourceDir );
			var target = new DirectoryInfo( targetDir );

			// 源目录检查
			if (!source.Exists)
				throw new DirectoryNotFoundException( "源目录不存在" );

			// 创建目标目录
			Directory.CreateDirectory( target.FullName );

			// 复制所有文件
			foreach (FileInfo file in source.GetFiles()) {
				string destPath = Path.Combine( target.FullName, file.Name );
				file.CopyTo( destPath, overwrite );
			}

			// 递归处理子目录
			foreach (DirectoryInfo subDir in source.GetDirectories()) {
				DirectoryInfo nextTarget = target.CreateSubdirectory( subDir.Name );
				CopyDirectory( subDir.FullName, nextTarget.FullName, overwrite );
			}
		}
	}

}
