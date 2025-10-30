using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MusicChange.Controls
{
	public class ImageSelectorControl : UserControl
	{
		private Panel panelCanvas;
		private PictureBox pictureBoxInner;
		private Button btnLoad;
		private Button btnConfirm;
		private Label lblPath;

		private Bitmap _originalImage;
		private bool _isDragging;
		private Point _dragStart;
		private Point _scrollStart; // positive scroll offset

		public string SavedImagePath
		{
			get; private set;
		}

		// 事件：图片保存完成后通知外部
		public event EventHandler<string> ImageSaved;

		public ImageSelectorControl( )
		{
			InitializeComponents();
		}

		private void InitializeComponents( )
		{
			// Panel 用于滚动显示大图
			panelCanvas = new Panel
			{
				Dock = DockStyle.Fill,
				AutoScroll = true,
				BackColor = Color.Black
			};
			panelCanvas.Paint += PanelCanvas_Paint;
			panelCanvas.MouseDown += PanelCanvas_MouseDown;
			panelCanvas.MouseMove += PanelCanvas_MouseMove;
			panelCanvas.MouseUp += PanelCanvas_MouseUp;

			pictureBoxInner = new PictureBox
			{
				Location = Point.Empty,
				SizeMode = PictureBoxSizeMode.Normal
			};

			panelCanvas.Controls.Add( pictureBoxInner );

			// 按钮和路径标签
			btnLoad = new Button { Text = "选择图片", Dock = DockStyle.Top, Height = 30 };
			btnLoad.Click += BtnLoad_Click;

			btnConfirm = new Button { Text = "确认并保存 50×50", Dock = DockStyle.Top, Height = 30 };
			btnConfirm.Click += BtnConfirm_Click;

			lblPath = new Label { Text = "保存路径：", Dock = DockStyle.Bottom, Height = 24, AutoEllipsis = true };

			// 布局
			var rightPanel = new Panel { Dock = DockStyle.Right, Width = 140 };
			rightPanel.Controls.Add( btnConfirm );
			rightPanel.Controls.Add( btnLoad );

			this.Controls.Add( panelCanvas );
			this.Controls.Add( rightPanel );
			this.Controls.Add( lblPath );

			// 初始尺寸推荐
			this.Size = new Size( 400, 300 );
		}

		private void BtnLoad_Click(object sender, EventArgs e)
		{
			using OpenFileDialog ofd = new OpenFileDialog
			{
				Filter = "图片文件|*.png;*.jpg;*.jpeg;*.bmp;*.gif",
				Title = "选择图片"
			};
			if (ofd.ShowDialog() != DialogResult.OK)
				return;

			LoadImage( ofd.FileName );
		}

		private void LoadImage(string path)
		{
			try {
				// 释放旧图
				_originalImage?.Dispose();
				using var tmp = new Bitmap( path );
				_originalImage = new Bitmap( tmp ); // copy to allow disposing dialog file lock

				// pictureBox 显示原始像素大小，Panel.AutoScroll 负责滚动
				pictureBoxInner.Image = _originalImage;
				pictureBoxInner.Width = _originalImage.Width;
				pictureBoxInner.Height = _originalImage.Height;

				// 确保初始滚动使图片中心对齐控件中心（更直观）
				CenterImageInView();

				panelCanvas.Invalidate();
			}
			catch (Exception ex) {
				MessageBox.Show( $"加载图片失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		private void CenterImageInView( )
		{
			if (_originalImage == null)
				return;
			int scrollX = Math.Max( 0, (_originalImage.Width - panelCanvas.ClientSize.Width) / 2 );
			int scrollY = Math.Max( 0, (_originalImage.Height - panelCanvas.ClientSize.Height) / 2 );
			// 设置 AutoScrollPosition 时需要传入正的偏移值
			panelCanvas.AutoScrollPosition = new Point( scrollX, scrollY );
		}

		private void PanelCanvas_MouseDown(object sender, MouseEventArgs e)
		{
			if (_originalImage == null)
				return;
			if (e.Button != MouseButtons.Left)
				return;

			_isDragging = true;
			_dragStart = e.Location;
			// 记录当时滚动偏移（AutoScrollPosition 返回负值，因此取 -）
			_scrollStart = new Point( -panelCanvas.AutoScrollPosition.X, -panelCanvas.AutoScrollPosition.Y );
			panelCanvas.Cursor = Cursors.SizeAll;
		}

		private void PanelCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			if (!_isDragging || _originalImage == null)
				return;

			// 计算 deltas（鼠标从开始点移动多少）
			int dx = e.X - _dragStart.X;
			int dy = e.Y - _dragStart.Y;

			int newScrollX = Math.Max( 0, _scrollStart.X - dx );
			int newScrollY = Math.Max( 0, _scrollStart.Y - dy );

			// 限制不能滚出图片边界
			newScrollX = Math.Min( newScrollX, Math.Max( 0, _originalImage.Width - panelCanvas.ClientSize.Width ) );
			newScrollY = Math.Min( newScrollY, Math.Max( 0, _originalImage.Height - panelCanvas.ClientSize.Height ) );

			panelCanvas.AutoScrollPosition = new Point( newScrollX, newScrollY );
			panelCanvas.Invalidate(); // 更新中央选框绘制
		}

		private void PanelCanvas_MouseUp(object sender, MouseEventArgs e)
		{
			_isDragging = false;
			panelCanvas.Cursor = Cursors.Default;
		}

		// 中央 50x50 选框绘制
		private void PanelCanvas_Paint(object sender, PaintEventArgs e)
		{
			// 绘制中心 50x50 的半透明框，提示裁剪区域
			int selW = 50, selH = 50;
			int cx = panelCanvas.ClientSize.Width / 2;
			int cy = panelCanvas.ClientSize.Height / 2;
			Rectangle selRect = new Rectangle( cx - selW / 2, cy - selH / 2, selW, selH );

			using (Brush b = new SolidBrush( Color.FromArgb( 60, Color.Black ) )) {
				// 遮罩：四周暗化（可选），这里只绘制一个边框和半透明填充
				e.Graphics.FillRectangle( b, selRect );
			}
			using (Pen p = new Pen( Color.Yellow, 2 )) {
				e.Graphics.DrawRectangle( p, selRect );
			}
		}

		private void BtnConfirm_Click(object sender, EventArgs e)
		{
			if (_originalImage == null) {
				MessageBox.Show( "请先选择图片", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information );
				return;
			}

			try {
				// 计算源图像中要裁切的矩形（以像素为单位）
				// 当前可视区域在原图上的左上坐标：
				int scrollX = -panelCanvas.AutoScrollPosition.X;
				int scrollY = -panelCanvas.AutoScrollPosition.Y;

				// 计算中心 50x50 在可视区的左上相对位置
				int selW = 50, selH = 50;
				int selLeftInView = (panelCanvas.ClientSize.Width - selW) / 2;
				int selTopInView = (panelCanvas.ClientSize.Height - selH) / 2;

				// 转换为原图坐标
				int srcX = scrollX + selLeftInView;
				int srcY = scrollY + selTopInView;

				// 若图片小于 50x50，按边界处理（从 0,0 开始）
				if (_originalImage.Width <= selW || _originalImage.Height <= selH) {
					// 直接缩放整个图片到 50x50
					using Bitmap target = new Bitmap( selW, selH );
					using Graphics g = Graphics.FromImage( target );
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
					g.Clear( Color.Transparent );
					g.DrawImage( _originalImage, new Rectangle( 0, 0, selW, selH ),
								new Rectangle( 0, 0, _originalImage.Width, _originalImage.Height ), GraphicsUnit.Pixel );

					SaveAndShowResult( target );
					return;
				}

				// 保证 srcRect 在原图范围内
				srcX = Math.Max( 0, Math.Min( srcX, _originalImage.Width - selW ) );
				srcY = Math.Max( 0, Math.Min( srcY, _originalImage.Height - selH ) );

				var srcRect = new Rectangle( srcX, srcY, selW, selH );

				using Bitmap targetBmp = new Bitmap( selW, selH );
				using Graphics g2 = Graphics.FromImage( targetBmp );
				g2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				g2.Clear( Color.Transparent );
				g2.DrawImage( _originalImage, new Rectangle( 0, 0, selW, selH ), srcRect, GraphicsUnit.Pixel );

				SaveAndShowResult( targetBmp );
			}
			catch (Exception ex) {
				MessageBox.Show( $"裁切/保存失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		private void SaveAndShowResult(Bitmap resultBmp)
		{
			// 保存到指定文件夹（应用程序目录下的 SelectedImages）
			string folder = Path.Combine( Application.StartupPath, "SelectedImages" );
			Directory.CreateDirectory( folder );
			string fileName = $"img_{DateTime.Now:yyyyMMdd_HHmmss_fff}.png";
			string fullPath = Path.Combine( folder, fileName );

			resultBmp.Save( fullPath, System.Drawing.Imaging.ImageFormat.Png );

			// 在 pictureBoxInner 上显示缩放后的 50x50 预览（把 pictureBoxInner 替换为 result 的缩小显示）
			// 我们用 pictureBoxInner 来显示当前预览：先释放旧图再设置
			_originalImage?.Dispose();
			_originalImage = new Bitmap( resultBmp ); // 50x50 preview 也保存在 _originalImage 中

			pictureBoxInner.Image = _originalImage;
			pictureBoxInner.Width = _originalImage.Width;
			pictureBoxInner.Height = _originalImage.Height;

			// 取消滚动（因为已经是 50x50）
			panelCanvas.AutoScrollPosition = new Point( 0, 0 );

			// 保存路径与事件
			SavedImagePath = fullPath;
			lblPath.Text = "保存路径：" + fullPath;
			ImageSaved?.Invoke( this, fullPath );

			MessageBox.Show( "图片已保存并显示（50×50）", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information );
			panelCanvas.Invalidate();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				_originalImage?.Dispose();
			}
			base.Dispose( disposing );
		}
	}
}
