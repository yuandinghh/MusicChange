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
		#endregion

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


	}
}
