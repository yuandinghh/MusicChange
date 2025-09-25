using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;

namespace MusicChange
{
	public partial class VideoCheckerForm:Form
	{
		// 支持的视频文件扩展名
		//private static readonly string[] VideoExtensions = {
		//	".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".mpeg", ".mpg",
		//	".m4v", ".rm", ".rmvb", ".3gp", ".vob", ".ts", ".mts", ".webm",
		//	".ogv", ".divx", ".xvid", ".asf", ".m2ts"
		//};

		private static readonly string[] VideoExtensions = {
			".mp4", ".avi"
		};
		private LibVLC _libVlc;
		private CancellationTokenSource _cancellationTokenSource;
		private int _totalFiles;
		private int _playableFiles;
		private int _unplayableFiles;
		private int _deletedFiles;
		private int _errorFiles;

		public VideoCheckerForm()
		{
			InitializeComponent();
			InitializeLibVLC();
			InitializeUI();
		}

		private void InitializeUI()
		{
			// 设置列表视图
			lvResults.View = View.Details;
			lvResults.Columns.Add("文件名", 300);
			lvResults.Columns.Add("状态", 150);
			lvResults.Columns.Add("路径", 500);
			lvResults.FullRowSelect = true;

			// 初始化进度条
			progressBar.Maximum = 100;
			progressBar.Value = 0;

			// 禁用删除按钮
			btnDeleteSelected.Enabled = false;
		}

		private void InitializeLibVLC()
		{
			try
			{
				// 初始化LibVLC
				Core.Initialize();
				_libVlc = new LibVLC(enableDebugLogs: false);
				lblStatus.Text = "LibVLC初始化成功";
			}
			catch(Exception ex)
			{
				lblStatus.Text = $"LibVLC初始化失败: {ex.Message}";
				btnStart.Enabled = false;
			}
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			using(var folderDialog = new FolderBrowserDialog())
			{
				folderDialog.Description = "选择要扫描的视频目录";
				if(folderDialog.ShowDialog() == DialogResult.OK)
				{
					txtDirectoryPath.Text = folderDialog.SelectedPath;
				}
			}
		}

		private async void btnStart_Click(object sender, EventArgs e)
		{
			if(string.IsNullOrEmpty(txtDirectoryPath.Text) || !Directory.Exists(txtDirectoryPath.Text))
			{
				MessageBox.Show("请选择有效的目录", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// 重置统计信息
			ResetCounters();
			lvResults.Items.Clear();

			// 禁用控件
			btnStart.Enabled = false;
			btnBrowse.Enabled = false;
			chkIncludeSubdirectories.Enabled = false;
			btnCancel.Enabled = true;

			_cancellationTokenSource = new CancellationTokenSource(); //CancellationTokenSource 提供了一种机制，用于发出取消请求并通知应该被取消的操作。

			try
			{
				// 开始扫描
				await Task.Run(() => ScanVideoFiles(
					txtDirectoryPath.Text,
					chkIncludeSubdirectories.Checked,
					_cancellationTokenSource.Token),
					_cancellationTokenSource.Token);
			}
			catch(OperationCanceledException)
			{
				lblStatus.Text = "扫描已取消";
			}
			catch(Exception ex)
			{
				lblStatus.Text = $"扫描出错: {ex.Message}";
				MessageBox.Show($"扫描过程中发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				// 更新统计信息
				UpdateStatistics();

				// 恢复控件状态
				btnStart.Enabled = true;
				btnBrowse.Enabled = true;
				chkIncludeSubdirectories.Enabled = true;
				btnCancel.Enabled = false;
				btnDeleteSelected.Enabled = lvResults.Items.Cast<ListViewItem>()
					.Any(item => item.SubItems[1].Text == "不可播放");
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			if(_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
			{
				_cancellationTokenSource.Cancel();
				lblStatus.Text = "正在取消扫描...";
				btnCancel.Enabled = false;
			}
		}

		private void btnDeleteSelected_Click(object sender, EventArgs e)
		{
			var unplayableFiles = lvResults.Items.Cast<ListViewItem>()
				.Where(item => item.SubItems[1].Text == "不可播放")
				.ToList();

			if(!unplayableFiles.Any())
			{
				MessageBox.Show("没有可删除的不可播放文件", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			var result = MessageBox.Show(
				$"确定要删除选中的 {unplayableFiles.Count} 个不可播放文件吗？\n此操作不可恢复！",
				"确认删除",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning);

			if(result == DialogResult.Yes)
			{
				DeleteUnplayableFiles(unplayableFiles);
			}
		}

		private void ResetCounters()
		{
			_totalFiles = 0;
			_playableFiles = 0;
			_unplayableFiles = 0;
			_deletedFiles = 0;
			_errorFiles = 0;
			progressBar.Value = 0;
		}

		private void UpdateStatistics()
		{
			lblStats.Text = $"总计: {_totalFiles} | 可播放: {_playableFiles} | 不可播放: {_unplayableFiles} | 已删除: {_deletedFiles} | 错误: {_errorFiles}";
		}

		private void ScanVideoFiles(string directory, bool includeSubdirectories, CancellationToken cancellationToken)
		{
			try
			{
				// 获取所有视频文件
				var searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
				var videoFiles = Directory.EnumerateFiles(directory, "*.*", searchOption)
					.Where(file => VideoExtensions.Contains(Path.GetExtension(file).ToLowerInvariant()))
					.ToList();

				_totalFiles = videoFiles.Count;

				// 更新UI
				Invoke(new Action(() =>
				{
					lblStatus.Text = $"找到 {_totalFiles} 个视频文件，开始检查...";
					progressBar.Maximum = _totalFiles;
				}));

				// 逐个检查视频文件
				for(int i = 0 ;i < videoFiles.Count ;i++)
				{
					// 检查是否取消
					if(cancellationToken.IsCancellationRequested)
					{
						cancellationToken.ThrowIfCancellationRequested();
					}

					var file = videoFiles[i];
					CheckVideoFile(file);

					// 更新进度
					Invoke(new Action(() =>
					{
						progressBar.Value = i + 1;
						lblStatus.Text = $"正在检查 {i + 1}/{_totalFiles}";
						UpdateStatistics();
					}));
				}

				Invoke(new Action(() =>
				{
					lblStatus.Text = "扫描完成";
				}));
			}
			catch(Exception ex)
			{
				Invoke(new Action(() =>
				{
					lblStatus.Text = $"扫描出错: {ex.Message}";
				}));
				throw;
			}
		}

		//private void CheckVideoFile(string filePath)
		//{
		//	try
		//	{
		//		string fileName = Path.GetFileName(filePath);
		//		bool isPlayable = IsVideoPlayable(filePath);

		//		// 更新计数器
		//		if(isPlayable)
		//		{
		//			_playableFiles++;
		//		}
		//		else
		//		{
		//			_unplayableFiles++;
		//		}

		//		// 添加到列表视图
		//		Invoke(new Action(() =>
		//		{
		//			var item = new ListViewItem(fileName);
		//			item.SubItems.Add(isPlayable ? "可播放" : "不可播放");
		//			item.SubItems.Add(filePath);
		//			item.ForeColor = isPlayable ? Color.Green : Color.Red;
		//			lvResults.Items.Add(item);
		//		}));
		//	}
		//	catch(Exception ex)
		//	{
		//		_errorFiles++;
		//		Invoke(new Action(() =>
		//		{
		//			var item = new ListViewItem(Path.GetFileName(filePath));
		//			item.SubItems.Add("错误");
		//			item.SubItems.Add(filePath);
		//			item.ForeColor = Color.Orange;
		//			lvResults.Items.Add(item);
		//		}));
		//	}
		//}
		//        错误 CS0029  无法将类型“System.Threading.Tasks.Task<LibVLCSharp.Shared.MediaParsedStatus>”隐式转换为“bool”

		//private bool IsVideoPlayable(string filePath)
		//{
		//	try
		//	{
		//		using(var media = new Media(_libVlc, new Uri(filePath)))
		//		{
		//			// 异步解析媒体信息
		//			var parseResult = await media.Parse(MediaParseOptions.ParseLocal);
		//			if(parseResult != MediaParsedStatus.Done)
		//				return false;

		//			// 检查是否有视频轨道
		//			if(media.Tracks.All(t => t.TrackType != TrackType.Video))
		//				return false;

		//			// 尝试创建媒体播放器来验证是否可以播放
		//			using(var mediaPlayer = new MediaPlayer(media))
		//			{
		//				// 尝试播放一小段来验证
		//				mediaPlayer.Play();

		//				// 短暂等待以确保播放开始
		//				await Task.Delay(300);

		//				// 检查播放状态
		//				bool isPlaying = mediaPlayer.State == VLCState.Playing ||
		//							   mediaPlayer.State == VLCState.Opening ||
		//							   mediaPlayer.State == VLCState.Buffering;

		//				// 停止播放
		//				mediaPlayer.Stop();

		//				return isPlaying;
		//			}
		//		}
		//	}
		//	catch
		//	{
		//		return false;
		//	}
		//}

		private async void CheckVideoFile(string filePath)
		{
			try
			{
				string fileName = Path.GetFileName(filePath);
				bool isPlayable = await IsVideoPlayable(filePath);

				// 更新计数器
				if(isPlayable)
				{
					_playableFiles++;
				}
				else
				{
					_unplayableFiles++;
				}

				// 添加到列表视图
				Invoke(new Action(() =>
				{
					var item = new ListViewItem(fileName);
					item.SubItems.Add(isPlayable ? "可播放" : "不可播放");
					item.SubItems.Add(filePath);
					item.ForeColor = isPlayable ? Color.Green : Color.Red;
					lvResults.Items.Add(item);
				}));
			}
			catch(Exception ex)
			{
				_errorFiles++;
				Invoke(new Action(() =>
				{
					var item = new ListViewItem(Path.GetFileName(filePath));
					item.SubItems.Add("错误");
					item.SubItems.Add(filePath);
					item.ForeColor = Color.Orange;
					lvResults.Items.Add(item);
				}));
			}
		}
		private async Task<bool> IsVideoPlayable(string filePath)
		{
			try
			{
				using(var media = new Media(_libVlc, new Uri(filePath)))
				{
					// 异步解析媒体信息
					//var parseResult = await media.Parse(MediaParseOptions.ParseLocal);
					//if(parseResult != MediaParsedStatus.Done)
					//	return false;
					//try
					//{
					//	var parseResult = await media.Parse(MediaParseOptions.ParseLocal);
					//	if(parseResult != MediaParsedStatus.Done)
					//		return false;
					//}
					//catch(Exception ex)
					//{
					//	// 记录异常信息，以便调试
					//	Console.WriteLine($"解析媒体时发生异常: {ex.Message}");
					//	return false;
					//}
					try
					{
						if(media == null)
						{
							throw new ArgumentNullException(nameof(media), "media对象未初始化");
						}

						var parseResult = await media.Parse(MediaParseOptions.ParseLocal);
						_ = parseResult == MediaParsedStatus.Done;
                        return parseResult == MediaParsedStatus.Done;
					}
					catch(Exception ex)
					{
						// 输出异常信息（实际项目中建议使用日志框架）
						Console.WriteLine($"解析失败: {ex.Message}\n堆栈跟踪: {ex.StackTrace}");
						return false;
					}


					// 检查是否有视频轨道
					if(media.Tracks.All(t => t.TrackType != TrackType.Video))
						return false;

					// 尝试创建媒体播放器来验证是否可以播放
					using var mediaPlayer = new MediaPlayer(media);
					// 尝试播放一小段来验证
					mediaPlayer.Play();

					// 短暂等待以确保播放开始
					await Task.Delay(300);

					// 检查播放状态
					bool isPlaying = mediaPlayer.State == VLCState.Playing ||
								   mediaPlayer.State == VLCState.Opening ||
								   mediaPlayer.State == VLCState.Buffering;

					// 停止播放
					mediaPlayer.Stop();

					return isPlaying;
				}
			}
			catch
			{
				return false;
			}
		}
		private void DeleteUnplayableFiles(List<ListViewItem> unplayableFiles)
		{
			int deletedCount = 0;
			int failedCount = 0;

			// 禁用按钮
			btnDeleteSelected.Enabled = false;
			btnStart.Enabled = false;

			// 逐个删除文件
			foreach(var item in unplayableFiles)
			{
				string filePath = item.SubItems[2].Text;
				try
				{
					if(File.Exists(filePath))
					{
						File.Delete(filePath);
						deletedCount++;
						item.SubItems[1].Text = "已删除";
						item.ForeColor = Color.Gray;
					}
				}
				catch(Exception ex)
				{
					failedCount++;
					item.SubItems[1].Text = $"删除失败: {ex.Message}";
					item.ForeColor = Color.DarkRed;
				}

				// 更新状态
				lblStatus.Text = $"正在删除: {deletedCount + failedCount}/{unplayableFiles.Count}";
				Application.DoEvents();
			}

			// 更新计数器
			_deletedFiles = deletedCount;
			_unplayableFiles -= deletedCount;
			_errorFiles += failedCount;
			UpdateStatistics();

			// 恢复按钮状态
			btnStart.Enabled = true;
			btnDeleteSelected.Enabled = lvResults.Items.Cast<ListViewItem>()
				.Any(i => i.SubItems[1].Text == "不可播放");

			lblStatus.Text = $"删除完成: 成功 {deletedCount} 个，失败 {failedCount} 个";
			MessageBox.Show(
				$"删除操作完成:\n成功删除 {deletedCount} 个文件\n删除失败 {failedCount} 个文件",
				"删除结果",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information);
		}

		private void VideoCheckerForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// 取消任何正在进行的操作
			if(_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
			{
				_cancellationTokenSource.Cancel();
			}

			// 释放LibVLC资源
			if(_libVlc != null)
			{
				_libVlc.Dispose();
			}
		}

		#region Windows 窗体设计器生成的代码
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.txtDirectoryPath = new System.Windows.Forms.TextBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.chkIncludeSubdirectories = new System.Windows.Forms.CheckBox();
			this.btnStart = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lvResults = new System.Windows.Forms.ListView();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.lblStatus = new System.Windows.Forms.Label();
			this.lblStats = new System.Windows.Forms.Label();
			this.btnDeleteSelected = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtDirectoryPath
			// 
			this.txtDirectoryPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtDirectoryPath.Location = new System.Drawing.Point(16, 14);
			this.txtDirectoryPath.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.txtDirectoryPath.Name = "txtDirectoryPath";
			this.txtDirectoryPath.Size = new System.Drawing.Size(1012, 25);
			this.txtDirectoryPath.TabIndex = 0;
			this.txtDirectoryPath.Text = "F:\\mp3";
			// 
			// btnBrowse
			// 
			this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowse.Location = new System.Drawing.Point(1037, 12);
			this.btnBrowse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(100, 27);
			this.btnBrowse.TabIndex = 1;
			this.btnBrowse.Text = "浏览...";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// chkIncludeSubdirectories
			// 
			this.chkIncludeSubdirectories.AutoSize = true;
			this.chkIncludeSubdirectories.Location = new System.Drawing.Point(16, 44);
			this.chkIncludeSubdirectories.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.chkIncludeSubdirectories.Name = "chkIncludeSubdirectories";
			this.chkIncludeSubdirectories.Size = new System.Drawing.Size(104, 19);
			this.chkIncludeSubdirectories.TabIndex = 2;
			this.chkIncludeSubdirectories.Text = "包含子目录";
			this.chkIncludeSubdirectories.UseVisualStyleBackColor = true;
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(16, 70);
			this.btnStart.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(100, 27);
			this.btnStart.TabIndex = 3;
			this.btnStart.Text = "开始扫描";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Enabled = false;
			this.btnCancel.Location = new System.Drawing.Point(124, 70);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(100, 27);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "取消";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// lvResults
			// 
			this.lvResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lvResults.HideSelection = false;
			this.lvResults.Location = new System.Drawing.Point(16, 104);
			this.lvResults.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.lvResults.Name = "lvResults";
			this.lvResults.Size = new System.Drawing.Size(1120, 411);
			this.lvResults.TabIndex = 6;
			this.lvResults.UseCompatibleStateImageBehavior = false;
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(13, 521);
			this.progressBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(1121, 27);
			this.progressBar.TabIndex = 7;
			// 
			// lblStatus
			// 
			this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblStatus.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lblStatus.Location = new System.Drawing.Point(16, 567);
			this.lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(1121, 22);
			this.lblStatus.TabIndex = 8;
			this.lblStatus.Text = "就绪";
			// 
			// lblStats
			// 
			this.lblStats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblStats.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lblStats.Location = new System.Drawing.Point(16, 591);
			this.lblStats.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblStats.Name = "lblStats";
			this.lblStats.Size = new System.Drawing.Size(1121, 22);
			this.lblStats.TabIndex = 9;
			this.lblStats.Text = "统计信息";
			// 
			// btnDeleteSelected
			// 
			this.btnDeleteSelected.Location = new System.Drawing.Point(250, 70);
			this.btnDeleteSelected.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.btnDeleteSelected.Name = "btnDeleteSelected";
			this.btnDeleteSelected.Size = new System.Drawing.Size(140, 27);
			this.btnDeleteSelected.TabIndex = 5;
			this.btnDeleteSelected.Text = "删除不可播放文件";
			this.btnDeleteSelected.UseVisualStyleBackColor = true;
			this.btnDeleteSelected.Click += new System.EventHandler(this.btnDeleteSelected_Click);
			// 
			// VideoCheckerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1153, 629);
			this.Controls.Add(this.lblStats);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.lvResults);
			this.Controls.Add(this.btnDeleteSelected);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnStart);
			this.Controls.Add(this.chkIncludeSubdirectories);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.txtDirectoryPath);
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.Name = "VideoCheckerForm";
			this.Text = "视频文件检查与删除工具";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VideoCheckerForm_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private System.Windows.Forms.TextBox txtDirectoryPath;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.CheckBox chkIncludeSubdirectories;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ListView lvResults;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Label lblStats;
		private System.Windows.Forms.Button btnDeleteSelected;
		#endregion
	}
}
