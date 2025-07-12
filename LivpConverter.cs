using System;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Text;

namespace MusicChange
{
	public class LivpConverter
	{
		public static void ConvertLivpToJpg(string livpPath, string outputDirectory,ListBox listBox2)
		{
			// 验证文件存在
			if (!File.Exists( livpPath ))
				throw new FileNotFoundException( "LIVP文件不存在", livpPath );
			// 准备临时解压目录
			//取得文件名
			string fileName = Path.GetFileNameWithoutExtension( livpPath );
			fileName = outputDirectory + "\\" + fileName + ".jpg";
			//文件夹有相同的文件名
			if (!File.Exists( fileName )) {
				string tempDir = Path.Combine( Path.GetTempPath(), Guid.NewGuid().ToString() );
				Directory.CreateDirectory( tempDir );
				StringBuilder failFiles = new StringBuilder();
				try {               // 步骤1: 解压LIVP文件 (本质是ZIP)
					ZipFile.ExtractToDirectory( livpPath, tempDir );
					// 步骤2: 查找JPEG文件
					string jpegPath = FindJpegFile( tempDir );
					if (jpegPath == null)
						throw new InvalidOperationException( "未找到JPEG文件" );
					// 步骤3: 准备输出路径
					string outputPath = Path.Combine( outputDirectory, Path.GetFileNameWithoutExtension( livpPath ) + ".jpg" );
					// 步骤4: 复制并重命名
					File.Copy( jpegPath, outputPath, overwrite: true );
					listBox2.Items.Add( outputPath );

				}
				catch (Exception ex) {
					ChangePictuer.failCount++;
					failFiles.AppendLine( $"{livpPath}: {ex.Message}" );
				}
				finally {
					// 清理临时文件
					Directory.Delete( tempDir, recursive: true );
				}
			}
		}
		private static string FindJpegFile(string directory)
		{
			foreach (string file in Directory.GetFiles( directory )) {
				// 通过文件头识别JPEG (FF D8 FF)
				if (IsJpegFile( file ))
					return file;
			}
			return null;
		}

		private static bool IsJpegFile(string filePath)
		{
			try {
				using (var fs = new FileStream( filePath, FileMode.Open )) {
					// JPEG文件头特征: 0xFF, 0xD8, 0xFF
					byte[] header = new byte[3];
					if (fs.Read( header, 0, 3 ) < 3)
						return false;

					return header[0] == 0xFF &&
						   header[1] == 0xD8 &&
						   header[2] == 0xFF;
				}
			}
			catch {
				return false;
			}
		}
	}
}
