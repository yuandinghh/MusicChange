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

namespace MusicChange
{
	public partial class Form3 : Form
	{
		public Form3( )
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			//选择目录和webp 类型文件
			using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()) {
				folderBrowserDialog.Description = "请选择一个文件夹";

				// 如果用户选择了文件夹
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
					string selectedPath = folderBrowserDialog.SelectedPath;

					// 获取所有 .webp 文件
					string[] webpFiles = Directory.GetFiles( selectedPath, "*.webp", SearchOption.TopDirectoryOnly );
					//选择的文件存入listbox
					// 清空 ListBox
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

		private void button2_Click(object sender, EventArgs e)
		{
			//将选中的文件在listbox 内的文件转换为jpg
			if (listBox1.SelectedItem == null) {
				MessageBox.Show( "请先选择一个文件。" );
				return;
			}
			string selectedFile = listBox1.SelectedItem.ToString();
			//从 listbox 中获取所有文件转换为 jpg
			if (string.IsNullOrEmpty(selectedFile)) {
				MessageBox.Show( "请先选择一个文件。" );
				return;
			}
			if (!File.Exists(selectedFile)) {
				MessageBox.Show( "所选文件不存在。" );
				return;
			}
			// 检查文件扩展名是否为 .webp
			if (Path.GetExtension(selectedFile).ToLower() != ".webp") {
				MessageBox.Show( "请选择一个 .webp 文件。" );
				return;
			}
			if (Directory.Exists( selectedFile )) {

			}


			try {
				// 读取 .webp 文件
				using (var image = new Bitmap(selectedFile)) {
					// 构建新的文件名
					string newFileName = Path.ChangeExtension(selectedFile, ".jpg");
					// 保存为 .jpg 文件
					image.Save(newFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
					MessageBox.Show( $"转换成功！已保存为: {newFileName}" );
				}
			}
			catch (Exception ex) {
				MessageBox.Show( $"转换失败: {ex.Message}" );
			}

		}

		private void Form3_Load(object sender, EventArgs e)
		{

		}
	}
}
