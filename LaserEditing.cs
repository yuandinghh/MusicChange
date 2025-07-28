
#region ------------- 系统加载部分  无需改变的变量 -------------
using System;
//using System.Windows.Forms.Keys;
//using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using Application = System.Windows.Application;
using Point = System.Drawing.Point;
//using MusicChange.SqliteDataAccess;

namespace MusicChange
{
	public partial class LaserEditing : Form
	{       // Windows API 函数
		[DllImport( "user32.dll" )]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
		[DllImport( "user32.dll" )]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
		// 快捷键ID
		private const int HOTKEY_ID = 1;
		// 修饰符常量
		private const uint MOD_ALT = 0x0001;
		private const uint MOD_CONTROL = 0x0002;
		private const uint MOD_SHIFT = 0x0004;
		// 鼠标状态变量
		private bool isDragging = false;
		private System.Drawing.Point dragStartPoint;
		private const int borderSize = 10;
		private FormWindowState previousWindowState;
		[DllImport( "user32.dll" )]
		public static extern bool ReleaseCapture( );
		[DllImport( "user32.dll" )]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		private const int WM_NCLBUTTONDOWN = 0xA1;
		private const int HTCAPTION = 0x2;
		private LibVLC _libVLC;
		private const int HT_CAPTION = 0x2;
	#endregion

		private bool splitContainer5mouseDown;
		int count = 0;
		bool IsOfficialMaterialSwitch = false; //官方素材开关
		bool Ismaterial = true;
		string importfile;  //imøport a file

		public LaserEditing( )
		{
			//AutoScaleMode = AutoScaleMode.Dpi; // 根据系统DPI自动缩放
			InitializeComponent();  			this.DoubleBuffered = true;
			button2.FlatAppearance.BorderSize = 0;    // 边框大小设为 0
			qrcode1.FlatAppearance.BorderSize = 0;    // 边框大小设为 0

		}
		private void LaserEditing_Load(object sender, EventArgs e)
		{
			//splitContainer5mouseDown = false;
			//splitContainer1.Panel2MinSize = 400;
			//buttonx8.BackColor = System.Drawing.Color.Gray;
			Ismaterial = true;  // 默认选择当前素材
			buttonX3_Click( null, null ); // 设置当前素材按钮样式	this.ClientSize = new System.Drawing.Size( 1900, 1080 );

			// 注册快捷键	RegisterHotKey( this.Handle, HOTKEY_ID, MOD_CONTROL | MOD_ALT, (uint)Keys.F1 );
			// 初始化 LibVLC			Core.Initialize();			_libVLC = new LibVLC();
			// 设置初始状态
			this.Size = new System.Drawing.Size( 1500, 900 ); // 设置主窗口初始大小
			splitContainer5mouseDown = false;
			OfficialMaterialSwitch(); // 初始化官方素材开关状态
			sC4.SplitterDistance = 500; //上中
			sC3.SplitterDistance = 750; // 上左 宽度


		}
		#region ----------- 鼠标拖动窗口和改变大小问题 快捷键  还没解决-------------------
		private void panelEx4_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) {
				ReleaseCapture();
				SendMessage( this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0 );
			}

		}

		private void LaserEditing_MouseDown(object sender, MouseEventArgs e)
		{
			splitContainer5mouseDown = true;
			if (e.Button == MouseButtons.Left) {
				// 释放鼠标捕获并发送消息以模拟拖动窗口
				ReleaseCapture();
				SendMessage( this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0 );
			}
			//// 只有在正常状态下才能拖动和调整大小
			//if(this.WindowState == FormWindowState.Normal)
			//{
			//	// 鼠标左键按下时开始拖动（排除边缘区域，避免与缩放冲突）
			if (e.Button == MouseButtons.Left) {  //&& !IsInResizeArea( e.Location )
				isDragging = true;
				dragStartPoint = new Point( e.X, e.Y );
				this.Cursor = Cursors.Hand;
			}
		}

		//if(e.Button == MouseButtons.Left)
		//{
		//	ReleaseCapture();
		//	SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0); // 欺骗系统认为在标题栏按下
		//}
		private void LaserEditing_MouseMove(object sender, MouseEventArgs e)
		{

			// 处理拖动
			if (isDragging) {
				this.Location = new Point( this.Location.X + e.X - dragStartPoint.X, this.Location.Y + e.Y - dragStartPoint.Y );
				//  Point newLocation = this.Location;
				//newLocation.X += e.X - dragStartPoint.X;
				//newLocation.Y += e.Y - dragStartPoint.Y;
				//this.Location = newLocation;
			}
			//// 处理边框鼠标样式
			//else if (this.WindowState == FormWindowState.Normal) {
			//	SetCursorBasedOnPosition( e.Location );
			//}
		}

		private void LaserEditing_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) {
				isDragging = false;
				//this.Cursor = Cursors.Default;
			}
		}

		private void LaserEditing_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			// 双击标题栏切换最大化/正常状态
			if (e.Button == MouseButtons.Left &&
				e.Y < 50) // 假设标题栏高度为50
			{
				ToggleMaximize();
			}
		}
		private void ToggleMaximize( )
		{
			if (this.WindowState == FormWindowState.Normal) {
				previousWindowState = FormWindowState.Normal;
				this.WindowState = FormWindowState.Maximized;
			}
			else {
				this.WindowState = previousWindowState;
			}
		}
		protected override void WndProc(ref Message m)
		{
			const int WM_NCHITTEST = 0x0084;
			const int HTCLIENT = 0x1;
			const int HTLEFT = 0xA;
			const int HTRIGHT = 0xB;
			const int HTTOP = 0xC;
			const int HTTOPLEFT = 0xD;
			const int HTTOPRIGHT = 0xE;
			const int HTBOTTOM = 0xF;
			const int HTBOTTOMLEFT = 0x10;
			const int HTBOTTOMRIGHT = 0x11;

			const int RESIZE_HANDLE_SIZE = 10; // 可调整大小的边缘宽度

			if (m.Msg == WM_NCHITTEST && this.WindowState == FormWindowState.Normal) {
				Point cursorPos = this.PointToClient( Cursor.Position );
				int hitTestResult = HTCLIENT;

				// 判断鼠标是否在窗体边缘
				if (cursorPos.X <= RESIZE_HANDLE_SIZE) {
					if (cursorPos.Y <= RESIZE_HANDLE_SIZE)
						hitTestResult = HTTOPLEFT; // 左上角
					else if (cursorPos.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
						hitTestResult = HTBOTTOMLEFT; // 左下角
					else
						hitTestResult = HTLEFT; // 左边
				}
				else if (cursorPos.X >= this.ClientSize.Width - RESIZE_HANDLE_SIZE) {
					if (cursorPos.Y <= RESIZE_HANDLE_SIZE)
						hitTestResult = HTTOPRIGHT; // 右上角
					else if (cursorPos.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
						hitTestResult = HTBOTTOMRIGHT; // 右下角
					else
						hitTestResult = HTRIGHT; // 右边
				}
				else if (cursorPos.Y <= RESIZE_HANDLE_SIZE)
					hitTestResult = HTTOP; // 上边
				else if (cursorPos.Y >= this.ClientSize.Height - RESIZE_HANDLE_SIZE)
					hitTestResult = HTBOTTOM; // 下边

				m.Result = (IntPtr)hitTestResult;
				return;
			}

			base.WndProc( ref m );
		}
	
		// 处理快捷键
		private void LaserEditing_KeyDown(object sender, KeyEventArgs e)
		{
		Keys key = e.KeyCode;
			//  if (e.Control != true)//如果没按Ctrl键      return;
			switch (key) {
				case Keys.F1:

					break;
				case Keys.F2:

					break;
				case Keys.F3:

					break;
				case Keys.F4:             //减少 图片显示时间5秒
				case Keys.F6:             //停音乐
					break;
				case Keys.F5:             //增加图片显示时间
				case Keys.F10:
					break;
				case Keys.F11:             //关闭计算机
					break;
				case Keys.F12:
					//application.Exit();
					this.Close();
					break;

			}
		}
		#endregion

		#region ------------- 没用的程序  -------------

		private void splitContainer5_MouseDown(object sender, MouseEventArgs e)
		{
			splitContainer5mouseDown = true;
		}


		private void splitContainer3_Panel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void LaserEditing_Resize(object sender, EventArgs e)
		{
			splitContainer1.Panel2MinSize = 400;
		}

		private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void 修改图片_Click(object sender, EventArgs e)
		{
			ChangePictuer form = new ChangePictuer();
			form.Show();

		}

		private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
		{

		}
		//private void SplitContainer5_SplitterMoving(object sender, SplitterCancelEventArgs e)
		//{
		//	// 仅处理水平分隔条（Orientation=Vertical）
		//	if (splitContainer1.Orientation == Orientation.Vertical) {
		//		// 计算 Panel2 当前宽度：总宽度 - 分隔条位置
		//		int panel2Width = splitContainer1.Width - e.SplitX;
		//		// 若 Panel2 宽度 < 400px，则取消移动
		//		if (panel2Width < 400) {
		//			e.Cancel = true;  // 阻止移动
		//			splitContainer1.SplitterDistance = splitContainer1.Width - 400; // 重置位置
		//		}
		//	}
		//}
		#endregion
		#region  -------------  窗口 close, maximize, minimize  -----------------
		private void button8_Click(object sender, EventArgs e)  //退出当前窗口
		{
			//Application.Exit();
			this.Close();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Maximized) {
				WindowState = FormWindowState.Normal; // 恢复到正常状态
			}
			else {
				WindowState = FormWindowState.Maximized; // 最大化窗口
			}

		}

		private void button42_Click(object sender, EventArgs e)
		{           //minimize
			if (WindowState == FormWindowState.Minimized) {
				WindowState = FormWindowState.Normal; // 恢复到正常状态
			}
			else {
				WindowState = FormWindowState.Minimized; // 最小化窗口
			}
		}

		private void buttonx6_Click(object sender, EventArgs e)
		{       //show cut 
			Cut form = new Cut();
			form.Show();
		}

		private bool IsInResizeArea(System.Drawing.Point point)
		{
			return point.X <= borderSize ||  // 左边界
				   point.X >= this.ClientSize.Width - borderSize ||  // 右边界
				   point.Y <= borderSize ||  // 上边界
				   point.Y >= this.ClientSize.Height - borderSize;   // 下边界
		}

		private void SetCursorBasedOnPosition(System.Drawing.Point point)
		{
			if (this.WindowState == FormWindowState.Maximized) {
				this.Cursor = Cursors.Default;
				return;
			}

			// 右下角
			if (point.X >= this.ClientSize.Width - borderSize &&
				point.Y >= this.ClientSize.Height - borderSize) {
				this.Cursor = Cursors.SizeNWSE;
			}
			// 左下角
			else if (point.X <= borderSize &&
					 point.Y >= this.ClientSize.Height - borderSize) {
				this.Cursor = Cursors.SizeNESW;
			}
			// 右上角
			else if (point.X >= this.ClientSize.Width - borderSize &&
					 point.Y <= borderSize) {
				this.Cursor = Cursors.SizeNESW;
			}
			// 左上角
			else if (point.X <= borderSize &&
					 point.Y <= borderSize) {
				this.Cursor = Cursors.SizeNWSE;
			}
			// 左边框
			else if (point.X <= borderSize) {
				this.Cursor = Cursors.SizeWE;
			}
			// 右边框
			else if (point.X >= this.ClientSize.Width - borderSize) {
				this.Cursor = Cursors.SizeWE;
			}
			// 上边框
			else if (point.Y <= borderSize) {
				this.Cursor = Cursors.SizeNS;
			}
			// 下边框
			else if (point.Y >= this.ClientSize.Height - borderSize) {
				this.Cursor = Cursors.SizeNS;
			}
			// 窗体内部
			else {
				this.Cursor = Cursors.Default;
			}
		}

		private void splitContainer6_MouseMove(object sender, MouseEventArgs e)
		{
			//SetCursorBasedOnPosition( e.Location );
		}
		private void panelEx1_MouseMove(object sender, MouseEventArgs e)
		{
			SetCursorBasedOnPosition( e.Location );
		}

		/// <summary>
		/// QRCode 二维码 
		/// </summary>
		private void buttonX12_Click(object sender, EventArgs e)
		{
			// 显示二维码生成界面
			QRCode form = new QRCode();
			form.Show();
			// 隐藏当前界面			this.Hide();

		}
		#endregion

		#region ----------  素材  Material  -------------------
		private void buttonX3_Click(object sender, EventArgs e)  //当前选择素材
		{
			Ismaterial = true;
			AllGray();
			//this.buttonX3.BackColor = System.Drawing.Color.Black;
			//this.buttonX3.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat;
			this.buttonX3.SymbolColor = System.Drawing.Color.GreenYellow;
			//this.buttonX3.ThemeAware = true;  //这个属性很可能用于让按钮能够感知并自动适应应用程序的主题变化。当主题（如浅色 / 深色模式）发生改变时，设置为 ThemeAware=true 的控件会自动更新其外观（如颜色、样式等）以匹配当前主题，而无需手动编写额外的主题切换代码。
			buttonx8.Visible = true;
			buttonx4.Visible = true;
			buttonx5.Visible = true;
			this.buttonx8.BackColor = System.Drawing.Color.GreenYellow;


		}
		private void buttonX2_Click(object sender, EventArgs e) //video 音频
		{
			Ismaterial = false;
			AllGray();
			this.buttonX2.SymbolColor = System.Drawing.Color.YellowGreen;
			buttonx8.Visible = false;
			buttonx4.Visible = false;
			buttonx5.Visible = false;
		}
		private void AllGray( )
		{
			this.buttonX3.SymbolColor = System.Drawing.Color.Gray;
			this.buttonX2.SymbolColor = System.Drawing.Color.Gray;
		}


		//官方素材 开关
		private void OfficialMaterialSwitch( )
		{
			if (IsOfficialMaterialSwitch) {
				button1.Visible = true;
			}
			else {
				button1.Visible = false;
			}
		}
		private void buttonx5_Click(object sender, EventArgs e) //官方素材
		{
			IsOfficialMaterialSwitch = true;
			OfficialMaterialSwitch();

		}


		private void button1_Click(object sender, EventArgs e) //素材热门
		{

		}

		//导入素材  选项
		private void buttonx8_Click(object sender, EventArgs e)
		{
			IsOfficialMaterialSwitch = false;
			OfficialMaterialSwitch();
		}
		//个人收藏
		private void buttonx4_Click(object sender, EventArgs e)
		{
			IsOfficialMaterialSwitch = false;
			OfficialMaterialSwitch();
		}

		private void panel2_Paint(object sender, PaintEventArgs e)
		{

		}
		//导入素材  Importing the materials
		private void buttonx7_Click(object sender, EventArgs e)
		{
			panel4.Visible = true;
			button2.Visible = true;
		}

		#endregion

		#region ------------  导入：视频、音频、图片  -------------
		/// <summary>
		/// 导入：视频、音频、图片
		/// </summary>
		private void button2_Click(object sender, EventArgs e)
		{
			//button2.Visible = false;
			//qrcode1.Visible = false;
			panel4.Visible = false;
			// 获取默认文档目录路径
			string documentsPath = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
			temp.Text = "默认文档目录: " + documentsPath;
			string subDirectory = Path.Combine( documentsPath, "ResourceFolder" );
			//判断是否目录存在
			if (Directory.Exists( subDirectory )) {
				temp.Text = "子目录已存在: " + subDirectory;
			}
			else {
				temp1.Text = "子目录不存在，将创建: " + subDirectory;
				// 创建子目录
				try {
					Directory.CreateDirectory( subDirectory );
					temp1.Text = "子目录创建成功: " + subDirectory;
				}
				catch (Exception ex) {
					temp1.Text = "创建子目录失败: " + ex.Message;
				}
			}
			//选择目录
			OpenFileDialog ofd = new OpenFileDialog();
			//所有音频文件
			ofd.Title = "请选择要导入的音频、视频或图片文件";
			//设置缺省文档目录
			ofd.InitialDirectory = subDirectory;			//ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); //默认打开音乐文件夹
			ofd.Multiselect = true; //允许多选
			ofd.Filter = "素材|*.mp3;*.wav;*.wma;*.flac;*.aac;*.ogg;*.mp4;*.avi;*.wmv;*.mov;*.jpg;*.jpeg;*.png;*.bmp;*.gif";
			if (ofd.ShowDialog() == DialogResult.OK) { //获取所有的选中文件存入string数组
				string[] selectedFiles = ofd.FileNames;
			}
			if (ofd.FileNames.Length > 0) {
				// 遍历选中的文件
				foreach (string file in ofd.FileNames) {
					try {
						// 获取文件名
						string fileName = Path.GetFileName( file );
						// 构建目标路径
						string targetPath = Path.Combine( subDirectory, fileName );
						// 复制文件到目标路径
						File.Copy( file, targetPath, true ); // true表示覆盖同名文件
						temp1.Text += $"\n已导入: {fileName}";
					}
					catch (Exception ex) {
						temp1.Text += $"\n导入失败: {ex.Message}";
					}
				}
			}
			else {
				temp1.Text = "未选择任何文件";
				//button2.Visible = true;				qrcode1.Visible = true;
				panel4.Visible = true;

			}

		}
		/*// 递归删除子目录及其所有内容
try
{
    if (Directory.Exists(subDirectory))
    {
        Directory.Delete(subDirectory, recursive: true); // recursive: true 表示删除所有子文件和子目录
        MessageBox.Show("子目录删除成功");
    }
}
catch (Exception ex)
{
    MessageBox.Show("删除子目录失败: " + ex.Message);
}*/
		private void panel4_SizeChanged(object sender, EventArgs e)
		{
			// 调整按钮位置
			button2.Left = (panel4.Width - button2.Width) / 2; // 水平居中
			button2.Top = (panel4.Height - button2.Height) / 2; // 垂直居中
			qrcode1.Left = button2.Left + 250; // 水平居中	


		}
		#endregion

		//全局设置
		private void buttonx11_Click(object sender, EventArgs e)
		{
			string dbPath = $"D:\\Documents\\Visual Studio 2022\\MusicChange\\LaserEditing.db";
			db dbAccess = new db( dbPath );

			//dbAccess.RunData();
			//dbAccess.InitDatabase();   //创建用户表
			//dbAccess.InitializeDatabase();
			//dbAccess.CreateTable();   //创建用户表
			dbAccess.testuser(); //测试用户表是否存在

		}

	
	}


	

}








