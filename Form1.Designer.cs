﻿namespace MusicChange
{
	partial class Form1
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent( )
		{
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem( new string[] {
			"选择",
			"儿童沃尔特",
			"阿第三方广东省房",
			"饿的人太晚"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.Empty, new System.Drawing.Font( "宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)) ) );
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem( new string[] {
			"文件名"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.IndianRed, new System.Drawing.Font( "宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)) ) );
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.listBox2 = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.button4 = new System.Windows.Forms.Button();
			this.listView1 = new System.Windows.Forms.ListView();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 15;
			this.listBox1.Location = new System.Drawing.Point( 12, 56 );
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size( 1384, 469 );
			this.listBox1.TabIndex = 0;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point( 152, 12 );
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size( 157, 25 );
			this.textBox1.TabIndex = 1;
			// 
			// listBox2
			// 
			this.listBox2.FormattingEnabled = true;
			this.listBox2.ItemHeight = 15;
			this.listBox2.Items.AddRange( new object[] {
			"1"} );
			this.listBox2.Location = new System.Drawing.Point( 12, 561 );
			this.listBox2.Name = "listBox2";
			this.listBox2.Size = new System.Drawing.Size( 819, 529 );
			this.listBox2.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point( 25, 15 );
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size( 112, 15 );
			this.label1.TabIndex = 3;
			this.label1.Text = "选择扫描的目录";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font( "宋体", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)) );
			this.label2.ForeColor = System.Drawing.Color.Salmon;
			this.label2.Location = new System.Drawing.Point( 12, 528 );
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size( 102, 28 );
			this.label2.TabIndex = 4;
			this.label2.Text = "label2";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font( "宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)) );
			this.label3.ForeColor = System.Drawing.Color.DarkGreen;
			this.label3.Location = new System.Drawing.Point( 12, 1101 );
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size( 96, 25 );
			this.label3.TabIndex = 5;
			this.label3.Text = "label3";
			// 
			// button1
			// 
			this.button1.Font = new System.Drawing.Font( "宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)) );
			this.button1.ForeColor = System.Drawing.SystemColors.Highlight;
			this.button1.Location = new System.Drawing.Point( 329, 12 );
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size( 129, 36 );
			this.button1.TabIndex = 6;
			this.button1.Text = "视频提取音频";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler( this.Button1_Click );
			// 
			// button2
			// 
			this.button2.Font = new System.Drawing.Font( "宋体", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)) );
			this.button2.ForeColor = System.Drawing.Color.Fuchsia;
			this.button2.Location = new System.Drawing.Point( 496, 12 );
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size( 294, 35 );
			this.button2.TabIndex = 7;
			this.button2.Text = "查找目录文件的重复文件";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler( this.button2_Click );
			// 
			// button3
			// 
			this.button3.Font = new System.Drawing.Font( "宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)) );
			this.button3.ForeColor = System.Drawing.SystemColors.Highlight;
			this.button3.Location = new System.Drawing.Point( 826, 15 );
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size( 129, 36 );
			this.button3.TabIndex = 8;
			this.button3.Text = "查找相同文件";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler( this.button3_Click );
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point( 973, 22 );
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size( 409, 25 );
			this.textBox2.TabIndex = 9;
			this.textBox2.Text = "D:\\老鹰乐队";
			// 
			// button4
			// 
			this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.button4.Font = new System.Drawing.Font( "宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)) );
			this.button4.ForeColor = System.Drawing.Color.Blue;
			this.button4.Location = new System.Drawing.Point( 1301, 1096 );
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size( 95, 38 );
			this.button4.TabIndex = 10;
			this.button4.Text = "退出";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler( this.button4_Click );
			// 
			// listView1
			// 
			this.listView1.Items.AddRange( new System.Windows.Forms.ListViewItem[] {
			listViewItem1,
			listViewItem2} );
			this.listView1.Location = new System.Drawing.Point( 869, 561 );
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size( 513, 529 );
			this.listView1.TabIndex = 11;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font( "宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)) );
			this.label4.ForeColor = System.Drawing.Color.DarkGreen;
			this.label4.Location = new System.Drawing.Point( 12, 1143 );
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size( 96, 25 );
			this.label4.TabIndex = 12;
			this.label4.Text = "label4";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 8F, 15F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 1408, 1217 );
			this.Controls.Add( this.label4 );
			this.Controls.Add( this.listView1 );
			this.Controls.Add( this.button4 );
			this.Controls.Add( this.textBox2 );
			this.Controls.Add( this.button3 );
			this.Controls.Add( this.button2 );
			this.Controls.Add( this.button1 );
			this.Controls.Add( this.label3 );
			this.Controls.Add( this.label2 );
			this.Controls.Add( this.label1 );
			this.Controls.Add( this.listBox2 );
			this.Controls.Add( this.textBox1 );
			this.Controls.Add( this.listBox1 );
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.ListBox listBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Label label4;
	}
}

