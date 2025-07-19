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

namespace MusicChange
{
	public partial class LaserEditing:Form
	{
		private bool splitContainer5mouseDown;
		int count = 0;
		public LaserEditing()
		{
			InitializeComponent();
			#endregion

			panel2.Height = 80;// 设置panel1的宽度为300像素
			panel2.AutoSize = false; // 关闭自动调整大小功能，确保宽度固定

		}
		private void LaserEditing_Load(object sender, EventArgs e)
		{
			splitContainer5mouseDown = false;
			splitContainer1.Panel2MinSize = 400;
		}
		private void LaserEditing_MouseDown(object sender, MouseEventArgs e)
		{
			splitContainer5mouseDown = true;
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
