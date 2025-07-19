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
	using System;
	using System.Windows.Forms;

	public partial class Form1:Form
	{
		// 定义左右面板的最大尺寸
		private int panel1MaxSize = 300;
		private int panel2MaxSize = 400;

		public Form1()
		{
			InitializeComponent();

			// 设置左右面板的最小尺寸
			splitContainer1.Panel1MinSize = 100;
			splitContainer1.Panel2MinSize = 100;

			// 注册分隔条移动事件
			splitContainer1.SplitterMoved += SplitContainer1_SplitterMoved;
			splitContainer1.SplitterMoving += SplitContainer1_SplitterMoving;
		}

		private void SplitContainer1_SplitterMoving(object sender, SplitterCancelEventArgs e)
		{
			// 在分隔条移动过程中检查并限制移动范围
			SplitContainer sc = sender as SplitContainer;

			// 计算面板2的预期大小
			int panel2Size = sc.Width - e.SplitX - sc.SplitterWidth;

			// 检查面板1是否超过最大尺寸
			if(e.SplitX > panel1MaxSize)
			{
				e.Cancel = true; // 取消移动
			}
			// 检查面板2是否超过最大尺寸
			else if(panel2Size > panel2MaxSize)
			{
				e.Cancel = true; // 取消移动
			}
		}

		private void SplitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
		{
			// 在分隔条移动完成后检查并调整，确保不会超过最大尺寸
			SplitContainer sc = sender as SplitContainer;

			// 计算面板2的当前大小
			int panel2Size = sc.Width - sc.SplitterDistance - sc.SplitterWidth;

			// 如果面板1超过最大尺寸，调整分隔条位置
			if(sc.SplitterDistance > panel1MaxSize)
			{
				sc.SplitterDistance = panel1MaxSize;
			}
			// 如果面板2超过最大尺寸，调整分隔条位置
			else if(panel2Size > panel2MaxSize)
			{
				sc.SplitterDistance = sc.Width - panel2MaxSize - sc.SplitterWidth;
			}
		}
	}
}
