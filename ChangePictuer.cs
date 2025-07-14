using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
				//	folderBrowserDialog.SelectedPath = $"F:\\下载\\edge";
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


		private async void button2_Click(object sender, EventArgs e)
		{
			successCount = 0;
			failCount = 0;
			StringBuilder failFiles = new StringBuilder();
			foreach (var item in listBox1.Items) {
				string webpFile = item.ToString();
				if (!File.Exists( webpFile ) || Path.GetExtension( webpFile ).ToLower() != ".webp")
					continue;

				try {
					using (var image = new ImageMagick.MagickImage( webpFile )) {
						string jpgFile = Path.ChangeExtension( webpFile, ".jpg" );
						image.Format = ImageMagick.MagickFormat.Jpeg;
						//get file  name
						string fileName = Path.GetFileNameWithoutExtension( webpFile );
						//设置图片质量
						image.Quality = 100; // 设置JPEG质量为90
						//加上当前时间
						//显示毫秒

						string time = DateTime.Now.ToString( "HHmmss.fff" );
						fileName = fileName + time + ".jpg";
						//await Task.Run(() => LivpConverter.ConvertLivpToJpg(livpFile, directory, listBox2));
						await  Task.Run( () =>	image.Write( fileName ));
						successCount++;
						textBox1.Text = $"转换成功：{webpFile} -> {jpgFile}" + successCount.ToString();
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

		private void buttonX1_Click(object sender, EventArgs e)
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
				string newFilePath = Path.ChangeExtension( filePath, ".jpg" );
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

		private void buttonx2_Click(object sender, EventArgs e)
		{
	
			//选择文件夹
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
	}
}

