using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MusicChange
{
	public partial class user:Form
	{
		private bool usernamenull = false;
		private int newId = 0;
		public static string logopic = null;
		public static Users luser = new();
		//private Bitmap _originalImage;
		//private bool _isDragging;
		//private Point _dragStart;
		//private Point _scrollStart; // positive scroll offset

		public string SavedImagePath
		{
			get; private set;
		}

		public user()
		{
			// 使用 db.dbPath（确保已初始化）
			InitializeComponent();
			logopic = null;
			if(LaserEditing.Pubuser != null)
			{
				if(LaserEditing.Pubuser.AvatarPath != null)
				{
					pictureBox.Image = Image.FromFile(LaserEditing.Pubuser.AvatarPath); // 显示用户头像
				}
				DateTime r = LaserEditing.Pubuser.CreatedAt;
				userimage.Image = Image.FromFile(LaserEditing.Pubuser.AvatarPath); // 显示用户头像
				username.Text = "你好：" + LaserEditing.Pubuser.Username;
				CultureInfo chineseCulture = new CultureInfo("zh-CN");  //`中文
				registertime.Text = r.ToString("yyyy年MM月dd日 HH:mm:ss", chineseCulture);  // 注册时间
				if(LaserEditing.Pubuser.IsActive == true)
				{
					register.Text = "应用程序已经注册";
				}
				else
				{
					register.Text = "应用程序未注册";
				}
				//versions.Text = "当前版本：" + LaserEditing.AppVersion;
				//将当前时间减去注册时间 ，获得天数
				days.Text = "你已经使用：  " + (DateTime.Now - LaserEditing.Pubuser.CreatedAt).Days + " 天";

			}
			else
			{
				if(!db.IsTableEmpty("Users"))
				{


					LaserEditing.usersRepo = new UsersRepository(db.dbPath);    //读第一条LaserEditing.db数据库的User 存入Pubuser 类中
					LaserEditing.Pubuser = LaserEditing.usersRepo.GetById(15);  //???????????
					if(LaserEditing.Pubuser != null)
					{

						if(LaserEditing.Pubuser.AvatarPath != null)
						{
							userimage.Image = Image.FromFile(LaserEditing.Pubuser.AvatarPath); // 显示用户头像
						}
					}
				}
				else
				{
					SetStatus.Text = "你好：" + "请先注册";
					username.Text = "";
					labelX1.Text = "";
				}
			}
			// Panel 用于滚动显示大图
			//panelCanvas = new Panel
			//{
			//	Dock = DockStyle.Fill,
			//	AutoScroll = true,
			//	BackColor = Color.Black
			//};
			//panelCanvas.Paint += PanelCanvas_Paint;
			//panelCanvas.MouseDown += PanelCanvas_MouseDown;
			//panelCanvas.MouseMove += PanelCanvas_MouseMove;
			//panelCanvas.MouseUp += PanelCanvas_MouseUp;

			//pictureBoxInner = new PictureBox
			//{
			//	Location = Point.Empty,
			//	SizeMode = PictureBoxSizeMode.Normal
			//};

			//panelCanvas.Controls.Add( pictureBoxInner );

			//// 按钮和路径标签
			//btnLoad = new Button { Text = "选择图片", Dock = DockStyle.Top, Height = 30 };
			//btnLoad.Click += BtnLoad_Click;

			//btnConfirm = new Button { Text = "确认并保存 50×50", Dock = DockStyle.Top, Height = 30 };
			//btnConfirm.Click += BtnConfirm_Click;

			//lblPath = new Label { Text = "保存路径：", Dock = DockStyle.Bottom, Height = 24, AutoEllipsis = true };

			//布局
			//  var rightPanel = new Panel { Dock = DockStyle.Right, Width = 140 };
			//rightPanel.Controls.Add(btnConfirm);
			//rightPanel.Controls.Add(btnLoad);

			//this.Controls.Add( panelCanvas );
			//this.Controls.Add( rightPanel );
			//this.Controls.Add( lblPath );

		}

		//private void BtnLoad_Click(object sender, EventArgs e)
		//{
		//	using OpenFileDialog ofd = new OpenFileDialog
		//	{
		//		Filter = "图片文件|*.png;*.jpg;*.jpeg;*.bmp;*.gif",
		//		Title = "选择图片"
		//	};
		//	if (ofd.ShowDialog() != DialogResult.OK)
		//		return;

		//	LoadImage( ofd.FileName );
		//}

		//private void LoadImage(string path)
		//{
		//	try {
		//		// 释放旧图
		//		_originalImage?.Dispose();
		//		using var tmp = new Bitmap( path );
		//		_originalImage = new Bitmap( tmp ); // copy to allow disposing dialog file lock

		//		// pictureBox 显示原始像素大小，Panel.AutoScroll 负责滚动
		//		pictureBoxInner.Image = _originalImage;
		//		pictureBoxInner.Width = _originalImage.Width;
		//		pictureBoxInner.Height = _originalImage.Height;

		//		// 确保初始滚动使图片中心对齐控件中心（更直观）
		//		CenterImageInView();

		//		panelCanvas.Invalidate();
		//	}
		//	catch (Exception ex) {
		//		MessageBox.Show( $"加载图片失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
		//	}
		//}

		//private void CenterImageInView( )
		//{
		//	if (_originalImage == null)
		//		return;
		//	int scrollX = Math.Max( 0, (_originalImage.Width - panelCanvas.ClientSize.Width) / 2 );
		//	int scrollY = Math.Max( 0, (_originalImage.Height - panelCanvas.ClientSize.Height) / 2 );
		//	// 设置 AutoScrollPosition 时需要传入正的偏移值
		//	panelCanvas.AutoScrollPosition = new Point( scrollX, scrollY );
		//}

		//private void PanelCanvas_MouseDown(object sender, MouseEventArgs e)
		//{
		//	if (_originalImage == null)
		//		return;
		//	if (e.Button != MouseButtons.Left)
		//		return;

		//	_isDragging = true;
		//	_dragStart = e.Location;
		//	// 记录当时滚动偏移（AutoScrollPosition 返回负值，因此取 -）
		//	_scrollStart = new Point( -panelCanvas.AutoScrollPosition.X, -panelCanvas.AutoScrollPosition.Y );
		//	panelCanvas.Cursor = Cursors.SizeAll;
		//}

		//private void PanelCanvas_MouseMove(object sender, MouseEventArgs e)
		//{
		//	if (!_isDragging || _originalImage == null)
		//		return;

		//	// 计算 deltas（鼠标从开始点移动多少）
		//	int dx = e.X - _dragStart.X;
		//	int dy = e.Y - _dragStart.Y;

		//	int newScrollX = Math.Max( 0, _scrollStart.X - dx );
		//	int newScrollY = Math.Max( 0, _scrollStart.Y - dy );

		//	// 限制不能滚出图片边界
		//	newScrollX = Math.Min( newScrollX, Math.Max( 0, _originalImage.Width - panelCanvas.ClientSize.Width ) );
		//	newScrollY = Math.Min( newScrollY, Math.Max( 0, _originalImage.Height - panelCanvas.ClientSize.Height ) );

		//	panelCanvas.AutoScrollPosition = new Point( newScrollX, newScrollY );
		//	panelCanvas.Invalidate(); // 更新中央选框绘制
		//}

		//private void PanelCanvas_MouseUp(object sender, MouseEventArgs e)
		//{
		//	_isDragging = false;
		//	panelCanvas.Cursor = Cursors.Default;
		//}

		//// 中央 50x50 选框绘制
		//private void PanelCanvas_Paint(object sender, PaintEventArgs e)
		//{
		//	// 绘制中心 50x50 的半透明框，提示裁剪区域
		//	int selW = 50, selH = 50;
		//	int cx = panelCanvas.ClientSize.Width / 2;
		//	int cy = panelCanvas.ClientSize.Height / 2;
		//	Rectangle selRect = new Rectangle( cx - selW / 2, cy - selH / 2, selW, selH );

		//	using (Brush b = new SolidBrush( Color.FromArgb( 60, Color.Black ) )) {
		//		// 遮罩：四周暗化（可选），这里只绘制一个边框和半透明填充
		//		e.Graphics.FillRectangle( b, selRect );
		//	}
		//	using (Pen p = new Pen( Color.Yellow, 2 )) {
		//		e.Graphics.DrawRectangle( p, selRect );
		//	}
		//}

		//private void BtnConfirm_Click(object sender, EventArgs e)  //
		//{
		//	if (_originalImage == null) {
		//		MessageBox.Show( "请先选择图片", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information );
		//		return;
		//	}

		//	try {
		//		// 计算源图像中要裁切的矩形（以像素为单位）
		//		// 当前可视区域在原图上的左上坐标：
		//		int scrollX = -panelCanvas.AutoScrollPosition.X;
		//		int scrollY = -panelCanvas.AutoScrollPosition.Y;

		//		// 计算中心 50x50 在可视区的左上相对位置
		//		int selW = 50, selH = 50;
		//		int selLeftInView = (panelCanvas.ClientSize.Width - selW) / 2;
		//		int selTopInView = (panelCanvas.ClientSize.Height - selH) / 2;

		//		// 转换为原图坐标
		//		int srcX = scrollX + selLeftInView;
		//		int srcY = scrollY + selTopInView;

		//		// 若图片小于 50x50，按边界处理（从 0,0 开始）
		//		if (_originalImage.Width <= selW || _originalImage.Height <= selH) {
		//			// 直接缩放整个图片到 50x50
		//			using Bitmap target = new Bitmap( selW, selH );
		//			using Graphics g = Graphics.FromImage( target );
		//			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
		//			g.Clear( Color.Transparent );
		//			g.DrawImage( _originalImage, new Rectangle( 0, 0, selW, selH ),
		//						new Rectangle( 0, 0, _originalImage.Width, _originalImage.Height ), GraphicsUnit.Pixel );

		//			SaveAndShowResult( target );
		//			return;
		//		}

		//		// 保证 srcRect 在原图范围内
		//		srcX = Math.Max( 0, Math.Min( srcX, _originalImage.Width - selW ) );
		//		srcY = Math.Max( 0, Math.Min( srcY, _originalImage.Height - selH ) );

		//		var srcRect = new Rectangle( srcX, srcY, selW, selH );

		//		using Bitmap targetBmp = new Bitmap( selW, selH );
		//		using Graphics g2 = Graphics.FromImage( targetBmp );
		//		g2.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
		//		g2.Clear( Color.Transparent );
		//		g2.DrawImage( _originalImage, new Rectangle( 0, 0, selW, selH ), srcRect, GraphicsUnit.Pixel );

		//		SaveAndShowResult( targetBmp );
		//	}
		//	catch (Exception ex) {
		//		MessageBox.Show( $"裁切/保存失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
		//	}
		//}

		//private void SaveAndShowResult(Bitmap resultBmp)
		//{
		//	// 保存到指定文件夹（应用程序目录下的 SelectedImages）
		//	string folder = Path.Combine( Application.StartupPath, "SelectedImages" );
		//	Directory.CreateDirectory( folder );
		//	string fileName = $"img_{DateTime.Now:yyyyMMdd_HHmmss_fff}.png";
		//	string fullPath = Path.Combine( folder, fileName );

		//	resultBmp.Save( fullPath, System.Drawing.Imaging.ImageFormat.Png );

		//	// 在 pictureBoxInner 上显示缩放后的 50x50 预览（把 pictureBoxInner 替换为 result 的缩小显示）
		//	// 我们用 pictureBoxInner 来显示当前预览：先释放旧图再设置
		//	_originalImage?.Dispose();
		//	_originalImage = new Bitmap( resultBmp ); // 50x50 preview 也保存在 _originalImage 中

		//	pictureBoxInner.Image = _originalImage;
		//	pictureBoxInner.Width = _originalImage.Width;
		//	pictureBoxInner.Height = _originalImage.Height;

		//	// 取消滚动（因为已经是 50x50）
		//	panelCanvas.AutoScrollPosition = new Point( 0, 0 );

		//	// 保存路径与事件
		//	SavedImagePath = fullPath;
		//	lblPath.Text = "保存路径：" + fullPath;
		//	//ImageSaved?.Invoke( this, fullPath );

		//	MessageBox.Show( "图片已保存并显示（50×50）", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information );
		//	panelCanvas.Invalidate();
		//}

		private void btnRegister_Click(object sender, EventArgs e)  // 注册
		{
			try
			{
				if(!ValidateInput(out string username, out string email, out string password))
				{
					if(usernamenull)
					{
						SetStatus.Text = "未注册";
						luser.Username = username;
						luser.Email = "110959751@qq.com";
						luser.PasswordHash = "123456789";
						luser.Iphone = txtPhone.Text.Trim();
						luser.FullName = txtFullName.Text.Trim();
						luser.AvatarPath = "";
						luser.IsActive = true;
						luser.CreatedAt = DateTime.Now;
						luser.UpdatedAt = DateTime.Now;

						bool same = LaserEditing.usersRepo.ExistsByUsername(username);
						int newIdw = LaserEditing.usersRepo.Create(luser);

						if(newIdw > 0)
						{
							//SetStatus.Text = "未注册用户成功";
							MessageBox.Show("未注册用户成功。", "注册完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
							this.DialogResult = DialogResult.OK;
							this.Close();
						}
						else
						{
							SetStatus.Text = "注册失败，请重试";
						}

					}
					return;
				}
				//获取当前 users 记录数  				int count = _usersRepo.GetAll().Count;
				if(!db.IsTableEmpty("Users"))
				{
					if(LaserEditing.usersRepo.ExistsByUsername(username))   //检查用户名是否存在
					{
						SetStatus.Text = $"用户名已存在，请更换";
						return;
					}
				}
				else
				{
					//db.ClearTableAndResetId("Users");
					db.ClearUserTable();
				}
				string passwordHash = PasswordHelper.HashPassword(password);  // 生成密码哈希（格式： iterations.salt.hash）
				SetStatus.Text = "未注册";
				luser.Username = username;
				luser.Email = "110959751@qq.com";
				luser.PasswordHash = passwordHash;
				luser.Iphone = txtPhone.Text.Trim();
				luser.FullName = txtFullName.Text.Trim();
				luser.AvatarPath = logopic;
				luser.IsActive = true;
				luser.CreatedAt = DateTime.Now;
				luser.UpdatedAt = DateTime.Now;
				luser.Draftposition = "";
				luser.IsLocked = false;
				luser.IsDeleted = false;
				luser.IsModified = false;
				newId = LaserEditing.usersRepo.Create(luser);  // 创建用户
				if(newId > 0)
				{
					SetStatus.Text = "注册成功";
					//写 main 数据库

					MessageBox.Show("用户注册成功。", "注册完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
					this.DialogResult = DialogResult.OK;
					this.Close();
				}
				else
				{
					SetStatus.Text = "注册失败，请重试";
				}
			}
			catch(Exception ex)
			{
				SetStatus.Text = ex.Message;
				MessageBox.Show("发生错误: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

			}
		}
		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
			this.Dispose();
		}
		private bool ValidateInput(out string username, out string email, out string password)
		{
			username = txtUsername.Text.Trim();
			email = txtEmail.Text.Trim();
			password = txtPassword.Text;
			usernamenull = false;

			if(string.IsNullOrEmpty(username))
			{
				SetStatus.Text = "用户名不能为空";
				usernamenull = true;
				txtUsername.Focus();
				return false;
			}

			if(username.Length < 3)
			{
				SetStatus.Text = "用户名至少 3 个字符";
				txtUsername.Focus();
				return false;
			}

			if(string.IsNullOrEmpty(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			{
				SetStatus.Text = "请输入有效邮箱地址";
				txtEmail.Focus();
				return false;
			}

			if(string.IsNullOrEmpty(password) || password.Length < 6)
			{
				SetStatus.Text = "密码至少 6 个字符";
				txtPassword.Focus();
				return false;
			}

			if(password != txtConfirm.Text)
			{
				SetStatus.Text = "两次输入密码不匹配";
				txtConfirm.Focus();
				return false;
			}
			return true;
		}

		// 选择图片按钮点击事件
		private void SelectButton_Click(object sender, EventArgs e)
		{
			Image originalImage, resizedImage;
			string savePath;
			using(OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif|所有文件|*.*";
				openFileDialog.Title = "选择图片";

				if(openFileDialog.ShowDialog() == DialogResult.OK)
				{
					try
					{                       // // 加载图片并显示
						originalImage = Image.FromFile(openFileDialog.FileName);
						{
							// 在PictureBox中显示原始图片
							pictureBox.Image = new Bitmap(originalImage);
							savePath = GetSavePath(openFileDialog.SafeFileName);
							// 检查图片是否需要压缩
							if(originalImage.Width > 50 || originalImage.Height > 50)
							{                       // 等比例压缩图片
								resizedImage = ResizeImage(originalImage, 50, 50);
								resizedImage.Save(savePath);
								string filenanme = Path.GetFileName(savePath);
								logopic = Path.Combine(LaserEditing.subDirectory, "User") + "\\" + filenanme;
								File.Copy(savePath, logopic, true);
								SetStatus.Text = "图片已压缩并保存";
								resizedImage.Dispose();
							}
							else
							{
								originalImage = Image.FromFile(openFileDialog.FileName);
								originalImage.Save(savePath);
								string filenanme = Path.GetFileName(savePath);
								logopic = Path.Combine(LaserEditing.subDirectory, "User") + "\\" + filenanme;
								File.Copy(savePath, logopic, true);
								SetStatus.Text = "图片尺寸小于等于50×50";
							}
						}
					}
					catch(Exception ex)
					{
						_ = MessageBox.Show($"处理图片时出错: {ex.Message}", "错误");
					}
				}
			}
		}
		// 等比例压缩图片
		private Image ResizeImage(Image image, int maxWidth, int maxHeight)
		{
			// 计算新尺寸，保持原始比例
			int newWidth = image.Width;
			int newHeight = image.Height;

			// 如果宽度超过最大限制，按比例缩小
			if(newWidth > maxWidth)
			{
				float ratio = (float)maxWidth / newWidth;
				newWidth = maxWidth;
				newHeight = (int)(newHeight * ratio);
			}

			// 如果高度超过最大限制，按比例缩小
			if(newHeight > maxHeight)
			{
				float ratio = (float)maxHeight / newHeight;
				newHeight = maxHeight;
				newWidth = (int)(newWidth * ratio);
			}

			// 创建新的位图并绘制缩小后的图像
			Bitmap resizedBitmap = new Bitmap(newWidth, newHeight);
			using(Graphics graphics = Graphics.FromImage(resizedBitmap))
			{
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.DrawImage(image, 0, 0, newWidth, newHeight);
			}

			return resizedBitmap;
		}
		private string GetSavePath(string originalFileName)  // 获取保存路径，避免覆盖原文件
		{
			string directory = Application.StartupPath; // 当前应用程序所在文件夹
			string fileName = Path.GetFileNameWithoutExtension(originalFileName);  // 文件名
			string extension = Path.GetExtension(originalFileName);  // 扩展名

			// 生成新文件名，添加"_resized"后缀
			string newFileName = $"{fileName}_resized{extension}";
			string savePath = Path.Combine(directory, newFileName);

			// 如果文件已存在，添加数字编号
			int count = 1;
			while(File.Exists(savePath))
			{
				newFileName = $"{fileName}_resized_{count}{extension}";
				savePath = Path.Combine(directory, newFileName);
				count++;
			}

			return savePath;
		}
	}
}

#region ------------ 调用 用户控件 ImageSelectorControl     没用 ------------
//private void AddInlineImageSelector( )
//{
//	var selector = new ImageSelectorControl
//	{
//		Width = 300,
//		Height = 220,
//		Location = new Point( 10, 10 )
//	};
//	selector.ImageSaved += (s, path) =>
//	{
//		// 处理保存结果（同上）
//		if (this.Controls.Find( "pictureBoxAvatar", true ).FirstOrDefault() is PictureBox pb) {
//			pb.Image?.Dispose();
//			using (var img = Image.FromFile( (string)path )) {
//				pb.Image = new Bitmap( img );
//			}
//		}
//		// 可以把路径写入文本框或字段
//	};
//	this.Controls.Add( selector );
//}
// 在 user 窗体中使用：打开 ImageSelectorDialog 并获取保存路径  如何 在 user 窗口 中调用 用户控件 ImageSelectorControl

//private void OpenImageSelectorAndSetAvatar( )
//{
//	using (var dlg = new ImageSelectorControl()) {
//		if (dlg.ShowDialog( this ) == DialogResult.OK) {
//			string savedPath = dlg.SavedImagePath;
//			if (!string.IsNullOrEmpty( savedPath )) {
//				// 假设 user 窗体有一个 pictureBoxAvatar 和 txtAvatarPath 控件
//				try {
//					// 更新 UI 预览
//					if (this.Controls.Find( "pictureBoxAvatar", true ).Length > 0) {
//						var pb = this.Controls.Find( "pictureBoxAvatar", true )[0] as PictureBox;
//						pb.Image?.Dispose();
//						pb.Image = System.Drawing.Image.FromFile( savedPath );
//					}

//					// 存储路径到文本框或成员变量
//					if (this.Controls.Find( "txtAvatarPath", true ).Length > 0) {
//						var tb = this.Controls.Find( "txtAvatarPath", true )[0] as TextBox;
//						tb.Text = savedPath;
//					}

//					// 如果你有 Users 对象，设置 AvatarPath 字段
//					// currentUser.AvatarPath = savedPath;
//				}
//				catch (Exception ex) {
//					MessageBox.Show( "加载已保存图片失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error );
//				}
//			}
//		}
//	}
//}

// 示例：把上面方法绑定到某按钮点击事件（在 InitializeComponent 或设计器中绑定）：
//private void btnSelectAvatar_Click(object sender, EventArgs e)
//{
//	OpenImageSelectorAndSetAvatar();
//}
#endregion
