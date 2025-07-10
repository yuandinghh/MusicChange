using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicChange {
	public partial class Form2 : Form {
		public Form2( ) {
			InitializeComponent();
			InitializeDynamicLayout(); // 初始化动态分区布局

			// 设置列和行的百分比大小
			tableLayoutPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 90F ) ); // 第一列占 50%
			tableLayoutPanel.ColumnStyles.Add( new ColumnStyle( SizeType.Percent, 90F ) ); // 第二列占 50%
			tableLayoutPanel.RowStyles.Add( new RowStyle( SizeType.Percent, 90F ) );       // 第一行占 50%
			tableLayoutPanel.RowStyles.Add( new RowStyle( SizeType.Percent, 90F ) );       // 第二行占 50%

			// 添加分区控件
			Panel panel1 = new Panel { BackColor = Color.Red };
			Panel panel2 = new Panel { BackColor = Color.Blue };
			Panel panel3 = new Panel { BackColor = Color.Green };
			Panel panel4 = new Panel { BackColor = Color.Yellow };

			tableLayoutPanel.Controls.Add( panel1, 0, 0 ); // 第一行第一列
			tableLayoutPanel.Controls.Add( panel2, 1, 0 ); // 第一行第二列
			tableLayoutPanel.Controls.Add( panel3, 0, 1 ); // 第二行第一列
			tableLayoutPanel.Controls.Add( panel4, 1, 1 ); // 第二行第二列

			// 将 TableLayoutPanel 添加到窗体
			this.Controls.Add( tableLayoutPanel );
		}

		private void InitializeDynamicLayout( ) {
				

	//		throw new NotImplementedException();
		}

		// 创建 TableLayoutPanel
		TableLayoutPanel tableLayoutPanel = new TableLayoutPanel {
			Dock = DockStyle.Fill, // 填充整个窗体
			ColumnCount = 2,       // 两列
			RowCount = 2,          // 两行
			BackColor = Color.LightGray
		};


	}
}
