using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicChange
{

	[DefaultEvent("AvatarChanged")]
	public partial class AvatarSelector:UserControl
	{
		private Image _originalImage;
		private Image _thumbnail;
		private const int TargetSize = 50;
		private PointF _dragStartPoint;
		private PointF _imageOffset = PointF.Empty;
		private bool _isDragging = false;

		// 事件定义
		public event EventHandler<Image> AvatarChanged;

		public AvatarSelector()
		{
			InitializeComponent();
			InitializeControl();
		}
		private void InitializeControl()
		{
			this.Size = new Size(TargetSize, TargetSize);
			this.DoubleBuffered = true;

			// 初始化PictureBox
			picAvatar.SizeMode = PictureBoxSizeMode.Zoom;
			picAvatar.BorderStyle = BorderStyle.FixedSingle;
			picAvatar.MouseDown += PicAvatar_MouseDown;
			picAvatar.MouseMove += PicAvatar_MouseMove;
			picAvatar.MouseUp += PicAvatar_MouseUp;

			// 初始化选择按钮
			btnSelect.Text = "选择头像";
			btnSelect.Click += BtnSelect_Click;
		}

		private void BtnSelect_Click(object sender, EventArgs e)
		{
			using var dialog = new OpenFileDialog();
			dialog.Multiselect = false;
			dialog.Title = "选择头像图片";
			dialog.Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

			if(dialog.ShowDialog() == DialogResult.OK)
			{
				LoadAvatar(dialog.FileName);
			}
		}

		private void LoadAvatar(string filePath)
		{
			try
			{
				_originalImage = Image.FromFile(filePath);
				UpdateThumbnail();
				UpdateAvatarDisplay();
				AvatarChanged?.Invoke(this, GetScaledImage());
			}
			catch(Exception ex)
			{
				MessageBox.Show($"加载图片失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void UpdateThumbnail()
		{
			if(_originalImage == null)
				return;

			// 计算缩放比例
			float scale = Math.Min(
				(float)TargetSize / _originalImage.Width,
				(float)TargetSize / _originalImage.Height
			);

			// 创建初始缩略图
			_thumbnail = new Bitmap(TargetSize, TargetSize);
			using(var g = Graphics.FromImage(_thumbnail))
			{
				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				g.DrawImage(_originalImage,
					new Rectangle(0, 0, TargetSize, TargetSize),
					new Rectangle(
						(int)((TargetSize - TargetSize * scale) / 2),
						(int)((TargetSize - TargetSize * scale) / 2),
						(int)(TargetSize * scale),
						(int)(TargetSize * scale)),
					GraphicsUnit.Pixel);
			}
		}

		private void UpdateAvatarDisplay()
		{
			if(_thumbnail == null)
				return;
			picAvatar.Image = _thumbnail.Clone() as Image;
			picAvatar.Tag = _originalImage.Clone() as Image;
			ResetImagePosition();
		}

		private void ResetImagePosition()
		{
			_imageOffset = PointF.Empty;
			Invalidate();
		}

		private void PicAvatar_MouseDown(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left && picAvatar.Image != null)
			{
				_dragStartPoint = new PointF(e.X, e.Y);
				_isDragging = true;
			}
		}

		private void PicAvatar_MouseMove(object sender, MouseEventArgs e)
		{
			if(!_isDragging)
				return;

			// 计算移动偏移量
			float dx = e.X - _dragStartPoint.X;
			float dy = e.Y - _dragStartPoint.Y;

			// 更新图像偏移量
			_imageOffset.X += dx;
			_imageOffset.Y += dy;

			// 边界限制
			_imageOffset.X = Math.Max(-_thumbnail.Width + TargetSize, Math.Min(0, _imageOffset.X));
			_imageOffset.Y = Math.Max(-_thumbnail.Height + TargetSize, Math.Min(0, _imageOffset.Y));

			_dragStartPoint = new PointF(e.X, e.Y);
			picAvatar.Invalidate();
		}

		private void PicAvatar_MouseUp(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
			{
				_isDragging = false;
			}
		}

		private Image GetScaledImage()
		{
			if(picAvatar.Image == null)
				return null;

			// 保持原始比例缩放
			float scale = Math.Min(
				(float)picAvatar.Width / picAvatar.Image.Width,
				(float)picAvatar.Height / picAvatar.Image.Height);

			return new Bitmap(
				(int)(picAvatar.Image.Width * scale),
				(int)(picAvatar.Image.Height * scale));
		}

		// 属性访问器
		[Browsable(true)]
		[Category("外观")]
		public Image SelectedAvatar => picAvatar.Image;

		[Browsable(true)]
		[Category("外观")]
		public string AvatarPath => picAvatar.Tag as string;
	}
}

