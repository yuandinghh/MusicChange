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
	public partial class Form2 : Form
	{
		private bool isResizing = true; // 是否正在调整大小
		private Point lastMousePosition; // 上一次鼠标位置
		public Form2( )
		{
			InitializeComponent();
			InitializeDynamicLayout(); // 初始化动态分区布局
									   // 设置列和行的百分比大小
									   //tableLayoutPanel1.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 60F ) ); // 第一列占 50%
									   //tableLayoutPanel1.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 40F ) ); // 第二列占 50%
									   //tableLayoutPanel1.RowStyles.Add( new RowStyle( SizeType.Percent, 70F ) );       // 第一行占 50%
									   //tableLayoutPanel1.RowStyles.Add( new RowStyle( SizeType.Percent, 30F ) );       // 第二行占 50%
									   // 将 TableLayoutPanel 添加到窗体
									   //tableLayoutPanel1.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 50F ) );
									   //tableLayoutPanel1.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 50F ) );
									   //tableLayoutPanel1.ColumnCount = 2; // Define the number of columns

			this.Controls.Add( tableLayoutPanel1 );
		}

		private bool IsMouseOnEdge(Point mouseLocation)
		{
			const int edgeThreshold = 10; // 边缘检测阈值
			return mouseLocation.X >= tableLayoutPanel1.Width - edgeThreshold ||
				   mouseLocation.Y >= tableLayoutPanel1.Height - edgeThreshold;
		}
		private void InitializeDynamicLayout( )
		{
			tableLayoutPanel1.ColumnStyles.Clear();
			tableLayoutPanel1.RowStyles.Clear();
			tableLayoutPanel1.ColumnCount = 4; // 设置列数
			tableLayoutPanel1.RowCount = 3; // 设置行数
			tableLayoutPanel1.AutoSize = true; // 自动调整大小
			tableLayoutPanel1.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 50F ) );
			tableLayoutPanel1.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 50F ) );
			tableLayoutPanel1.ColumnStyles.Add( new ColumnStyle( SizeType.Absolute, 200F ) );
			tableLayoutPanel1.ColumnStyles.Add( new ColumnStyle( SizeType.Absolute, 200F ) );
			tableLayoutPanel1.RowStyles.Add( new RowStyle( SizeType.Absolute, 40F ) ); // 第一行高度
			tableLayoutPanel1.RowStyles.Add( new RowStyle( SizeType.Absolute, 40F ) ); // 第二行高度
			tableLayoutPanel1.RowStyles.Add( new RowStyle( SizeType.Percent, 100F ) ); // 第三行占满剩余空间
																					   // 设置行的高度

			// 第一行两个控件
			tableLayoutPanel1.Controls.Add( new Label { Text = "第一行-第一列", BackColor = Color.Red, Dock = DockStyle.Fill }, 0, 0 );
			tableLayoutPanel1.Controls.Add( new Label { Text = "第一行-第二列", BackColor = Color.Green, Dock = DockStyle.Fill }, 1, 0 );

			// 第二行一个控件，横跨两列
			Label secondRowLabel = new Label { Text = "第二行-仅一列", BackColor = Color.Blue, Dock = DockStyle.Fill };
			tableLayoutPanel1.Controls.Add( secondRowLabel, 0, 1 );
			tableLayoutPanel1.SetColumnSpan( secondRowLabel, 2 );


		}

		private void tableLayoutPanel1_MouseMove(object sender, MouseEventArgs e)
		{
			// 检测鼠标是否在边缘
			if (IsMouseOnEdge( e.Location )) {
				isResizing = true;
				lastMousePosition = e.Location;
				Cursor = Cursors.SizeAll; // 更改鼠标样式为调整大小
			}
		}

		private void tableLayoutPanel1_MouseUp(object sender, MouseEventArgs e)
		{
			isResizing = false;
			Cursor = Cursors.Default; // 恢复默认鼠标样式
		}

		private void tableLayoutPanel1_MouseDown_1(object sender, MouseEventArgs e)
		{

			//if (isResizing) {
			//	// 计算鼠标移动的距离
			//	int deltaX = e.Location.X - lastMousePosition.X;
			//	int deltaY = e.Location.Y - lastMousePosition.Y;

			//	// 调整 TableLayoutPanel 的大小
			//	tableLayoutPanel1.Width += deltaX;
			//	tableLayoutPanel1.Height += deltaY;

			//	// 更新鼠标位置
			//	lastMousePosition = e.Location;
			//}
			//else {
			//	// 检测鼠标是否在边缘
			//	if (IsMouseOnEdge( e.Location )) {
			//		Cursor = Cursors.SizeAll; // 更改鼠标样式为调整大小
			//	}
			//	else {
			//		Cursor = Cursors.Default; // 恢复默认鼠标样式
			//	}
			//}
		}

		// 创建 TableLayoutPanel




	}
}
