#region 系统加载部分
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace MusicChange
{
	public partial class LaserEditing : Form
	{
		#endregion
		private bool splitContainer5mouseDown;
		int count = 0;
		[DllImport( "user32.dll" )]
		public static extern bool ReleaseCapture( );
		[DllImport( "user32.dll" )]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		private const int WM_NCLBUTTONDOWN = 0xA1;
		private const int HTCAPTION = 0x2;
		private LibVLC _libVLC;
		private const int HT_CAPTION = 0x2;
		bool Ismaterial = true;   //当前是素材

		private void LaserEditing_MouseDown(object sender, MouseEventArgs e)
		{
			splitContainer5mouseDown = true;
			if (e.Button == MouseButtons.Left) {
				// 释放鼠标捕获并发送消息以模拟拖动窗口
				ReleaseCapture();
				SendMessage( this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0 );
			}
		}
		public LaserEditing( )
		{
			InitializeComponent();
			this.DoubleBuffered = true;


		}

		private void LaserEditing_Load(object sender, EventArgs e)
		{
			splitContainer5mouseDown = false;
			splitContainer1.Panel2MinSize = 400;
			//buttonx8.BackColor = System.Drawing.Color.Gray;
		}
		private void buttonx5_Click(object sender, EventArgs e) //官方素材
		{
		
		}


#region 没用的程序
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
			Application.Exit();
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

		private void LaserEditing_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left) {
				// 计算偏移量并更新窗口位置
				//int deltaX = e.X - _dragStartPoint.X;
				//int deltaY = e.Y - _dragStartPoint.Y;
				//this.Location = new Point( this.Left + deltaX, this.Top + deltaY );
			}
		}

		private void buttonX3_Click(object sender, EventArgs e)  //当前选择素材
		{
			Ismaterial = true;
			this.buttonX3.Visible = true;
			this.buttonX3.BackColor = System.Drawing.Color.Black;
			this.buttonX3.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat;
			this.buttonX3.SymbolColor = System.Drawing.Color.GreenYellow;
			this.buttonX3.SymbolSize = 12F;
			this.buttonX3.TextColor = System.Drawing.Color.White;
			this.buttonX3.ThemeAware = true;  //这个属性很可能用于让按钮能够感知并自动适应应用程序的主题变化。当主题（如浅色 / 深色模式）发生改变时，设置为 ThemeAware=true 的控件会自动更新其外观（如颜色、样式等）以匹配当前主题，而无需手动编写额外的主题切换代码。
			buttonx8.Visible = true;
			this.buttonx8.BackColor = System.Drawing.Color.GreenYellow;
			this.buttonx8.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
			this.buttonx8.TextColor = System.Drawing.Color.Black;

		}

		private void button1_Click(object sender, EventArgs e) //素材热门
		{
			Ismaterial = false;
			this.buttonX3.Visible = false;
			buttonx8.Visible = false;
		}

		private void buttonX2_Click(object sender, EventArgs e) //video 音频
		{

		}


		// 添加边距


	}
	#endregion




}

