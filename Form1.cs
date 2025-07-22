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
			splitContainer1.Panel1MinSize = 135;
			splitContainer1.Panel2MinSize = 200;

			Button triggerBtn = new Button { Text = "展开菜单", Location = new Point( 50, 50 ) };
			triggerBtn.Click += triggerButton_Click;
			this.Controls.Add( triggerBtn );
		
		}

		private bool _isShifting = false; // 递归拦截标志

		//private void ShiftButtonsDown(int refY, int offset)
		//{
		//	if (_isShifting)
		//		return; // 正在调整时直接退出
		//	_isShifting = true;

		//	try {
		//		foreach (Control ctrl in Controls) {
		//			if (ctrl is Button && ctrl.Location.Y > refY) {
		//				ctrl.Top += offset; // 移动按钮
		//			}
		//		}
		//	}
		//	finally {
		//		_isShifting = false; // 确保标志复位
		//	}
		//}   // 初始化触发按钮

		private void ShiftButtonsDown(int startY, int offset)
		{
			foreach (Control ctrl in this.Controls) {
				if (ctrl is Button && ctrl.Location.Y > startY) {
					ctrl.Location = new Point( ctrl.Location.X, ctrl.Location.Y + offset );
				}
			}
		}
		private void triggerButton_Click(object sender, EventArgs e)
			{
				Button btn = (Button)sender;
				this.SuspendLayout();

				// 移除旧菜单（若存在）
				if (dynamicMenu != null) {
					int offset = -dynamicMenu.Height;
					ShiftButtonsDown( btn.Location.Y, offset );
					Controls.Remove( dynamicMenu );
					dynamicMenu.Dispose();
				}

				// 创建新菜单
				dynamicMenu = new Panel
				{
					Location = new Point( btn.Left, btn.Bottom + 5 ),
					Size = new Size( 120, 150 ),
					BackColor = Color.WhiteSmoke,
					BorderStyle = BorderStyle.FixedSingle
				};

				// 添加菜单项
				AddMenuItem( "新建文件" );
				AddMenuItem( "保存" );
				AddMenuItem( "退出" );

				Controls.Add( dynamicMenu );
				dynamicMenu.BringToFront();
				ShiftButtonsDown( btn.Location.Y, dynamicMenu.Height );
				this.ResumeLayout();
			}

			private void AddMenuItem(string text)
			{
				Button item = new Button
				{
					Text = text,
					Width = dynamicMenu.Width - 10,
					Dock = DockStyle.Top,
					Margin = new Padding( 5 )
				};
				dynamicMenu.Controls.Add( item );
			}

			//private void ShiftButtonsDown(int refY, int offset)
			//{
			//	foreach (Control ctrl in Controls) {
			//		if (ctrl is Button && ctrl.Location.Y > refY) {
			//			ctrl.Top += offset;
			//		}
			//	}
			
	


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
				//sc.SplitterDistance = sc.Width - panel2MaxSize - sc.SplitterWidth;
			}
		}

		private void CreateDynamicMenu(Point position)
		{
			// 创建菜单容器（Panel）
			Panel menuPanel = new Panel
			{
				Location = new Point( position.X, position.Y + 30 ), // 按钮下方30像素处
				Size = new Size( 100, 100 ),
				BackColor = Color.LightGray,
				Visible = true
			};

			// 添加菜单项（示例按钮）
			Button menuItem1 = new Button { Text = "选项1", Dock = DockStyle.Top };
			Button menuItem2 = new Button { Text = "选项2", Dock = DockStyle.Top };
			menuPanel.Controls.Add( menuItem1 );
			menuPanel.Controls.Add( menuItem2 );

			this.Controls.Add( menuPanel );
			menuPanel.BringToFront(); // 确保菜单显示在最上层
		}

	
	

		private void triggerButton_Click_1(object sender, EventArgs e)
		{

			Button btn = (Button)sender;
			int menuHeight = 100; // 菜单高度

			// 显示菜单
			CreateDynamicMenu( btn.Location );

			// 下移后续按钮
			ShiftButtonsDown( btn.Location.Y, menuHeight );
		}
	}
}
