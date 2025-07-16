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
	public partial class Form3:Form
	{
		public Form3()
		{
			InitializeComponent();
			// 固定左侧分区宽度
			splitContainer5.FixedPanel = FixedPanel.Panel1;
			splitContainer5.SplitterDistance = 120; // 左侧宽度 200px
			// 可选：限制最小宽度（防止用户拖得过小）
			splitContainer5.Panel1MinSize = 120; // 左侧最小宽度 150px[4,5](@ref)
			// 可选：禁用拖动
			splitContainer5.IsSplitterFixed = true;
			//splitContainer5.Panel2MinSize = 400;

			//int rightPanelWidth = 300;
			//splitContainer5.FixedPanel = FixedPanel.Panel2;
			//splitContainer5.SplitterDistance = splitContainer5.Width - rightPanelWidth - splitContainer5.SplitterWidth;

			//// 可选：禁止拖动分隔条（彻底锁定布局）
			//splitContainer5.IsSplitterFixed = true;
		}


		#region 窗体加载事件
		// 创建 SplitContainer
		//SplitContainer splitContainer = new SplitContainer
		//{
		//	Dock = DockStyle.Fill,  // 使 SplitContainer 填满整个窗体
		//	Orientation = Orientation.Horizontal  // 设置为水平分割（可选，垂直分割为 Orientation.Vertical）
		//};

		//// 在第一个面板中添加一个 ListBox
		//ListBox listBox1 = new ListBox
		//{
		//	Dock = DockStyle.Fill  // 使 ListBox 填满第一个面板
		//};
		//private void Form3_Load(object sender, EventArgs e)
		//{
		//	// 在 ListBox 中添加一些示例项


		//	panel2.Controls.Add(textBox);
		//	panel2.Controls.Add(button);
		//	splitContainer.Panel2.Controls.Add(panel2);

		//	splitContainer.Panel1.Controls.Add(listBox1);
		//	this.Controls.Add(splitContainer);
		//}


		//// 在第二个面板中添加一个 Panel 并在其中放置一个 TextBox 和一个 Button
		//Panel panel2 = new Panel
		//{
		//	Dock = DockStyle.Fill, // 使 Panel 填满第二个面板
		//};

		//TextBox textBox = new TextBox
		//{
		//	Dock = DockStyle.Top,  // 使 TextBox 填满面板的顶部
		//	Height = 30  // 设置 TextBox 的高度
		//};
		//Button button = new Button
		//{
		//	Dock = DockStyle.Bottom,  // 使 Button 填满面板的底部
		//	Text = "点击我"
		//};
		#endregion

		private void splitContainer3_Panel1_Paint(object sender, PaintEventArgs e)
		{

		}
	}
}
