#region  --- old  ---
//namespace MusicChange
//{
//	partial class MediaItemControl
//	{
//		/// <summary> 
//		/// 必需的设计器变量。
//		/// </summary>
//		private System.ComponentModel.IContainer components = null;

//		/// <summary> 
//		/// 清理所有正在使用的资源。
//		/// </summary>
//		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
//		protected override void Dispose(bool disposing)
//		{
//			if(disposing && (components != null))
//			{
//				components.Dispose();
//			}
//			base.Dispose(disposing);
//		}

//		#region 组件设计器生成的代码

//		/// <summary> 
//		/// 设计器支持所需的方法 - 不要修改
//		/// 使用代码编辑器修改此方法的内容。
//		/// </summary>
//		private void InitializeComponent()
//		{
//			this.lblFileName = new System.Windows.Forms.TextBox();
//			this.pictureBoxThumbnail = new System.Windows.Forms.PictureBox();
//			this.btnPlay = new System.Windows.Forms.Button();
//			((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnail)).BeginInit();
//			this.SuspendLayout();
//			// 
//			// lblFileName
//			// 
//			this.lblFileName.Dock = System.Windows.Forms.DockStyle.Bottom;
//			this.lblFileName.Location = new System.Drawing.Point(0, 125);
//			this.lblFileName.Name = "lblFileName";
//			this.lblFileName.Size = new System.Drawing.Size(150, 25);
//			this.lblFileName.TabIndex = 0;
//			// 
//			// pictureBoxThumbnail
//			// 
//			this.pictureBoxThumbnail.Dock = System.Windows.Forms.DockStyle.Fill;
//			this.pictureBoxThumbnail.Location = new System.Drawing.Point(0, 0);
//			this.pictureBoxThumbnail.Name = "pictureBoxThumbnail";
//			this.pictureBoxThumbnail.Size = new System.Drawing.Size(150, 150);
//			this.pictureBoxThumbnail.TabIndex = 1;
//			this.pictureBoxThumbnail.TabStop = false;
//			// 
//			// btnPlay
//			// 
//			this.btnPlay.Dock = System.Windows.Forms.DockStyle.Fill;
//			this.btnPlay.Location = new System.Drawing.Point(0, 0);
//			this.btnPlay.Name = "btnPlay";
//			this.btnPlay.Size = new System.Drawing.Size(150, 125);
//			this.btnPlay.TabIndex = 2;
//			this.btnPlay.UseVisualStyleBackColor = true;
//			// 
//			// MediaItemControl
//			// 
//			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
//			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//			this.Controls.Add(this.btnPlay);
//			this.Controls.Add(this.lblFileName);
//			this.Controls.Add(this.pictureBoxThumbnail);
//			this.Name = "MediaItemControl";
//			((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnail)).EndInit();
//			this.ResumeLayout(false);
//			this.PerformLayout();

//		}

//		//private void InitializeComponent()
//		//{
//		//	this.pictureBoxThumbnail = new System.Windows.Forms.PictureBox();
//		//	this.lblFileName = new System.Windows.Forms.Label();
//		//	this.btnPlay = new System.Windows.Forms.Button();
//		//	this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
//		//	((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnail)).BeginInit();
//		//	this.tableLayoutPanel1.SuspendLayout();
//		//	this.SuspendLayout();

//		//	// 
//		//	// tableLayoutPanel1
//		//	// 
//		//	this.tableLayoutPanel1.ColumnCount = 1;
//		//	this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
//		//	this.tableLayoutPanel1.RowCount = 2;
//		//	this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
//		//	this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
//		//	this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;

//		//	// 
//		//	// pictureBoxThumbnail
//		//	// 
//		//	this.pictureBoxThumbnail.Dock = System.Windows.Forms.DockStyle.Fill;
//		//	this.pictureBoxThumbnail.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;

//		//	// 
//		//	// lblFileName
//		//	// 
//		//	this.lblFileName.Dock = System.Windows.Forms.DockStyle.Fill;
//		//	this.lblFileName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
//		//	this.lblFileName.AutoEllipsis = true;

//		//	// 
//		//	// btnPlay
//		//	// 
//		//	this.btnPlay.Size = new System.Drawing.Size(25, 25);
//		//	this.btnPlay.Text = "▶";

//		//	// 
//		//	// 添加控件到布局
//		//	// 
//		//	this.tableLayoutPanel1.Controls.Add(this.pictureBoxThumbnail, 0, 0);
//		//	this.tableLayoutPanel1.Controls.Add(this.lblFileName, 0, 1);
//		//	this.Controls.Add(this.tableLayoutPanel1);
//		//	this.Controls.Add(this.btnPlay);

//		//	this.Size = new System.Drawing.Size(200, 180);

//		//	((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnail)).EndInit();
//		//	this.tableLayoutPanel1.ResumeLayout(false);
//		//	this.ResumeLayout(false);
//		//}




//		private System.Windows.Forms.TextBox lblFileName;
//		private System.Windows.Forms.PictureBox pictureBoxThumbnail;
//		private System.Windows.Forms.Button btnPlay;
//	}
//}

#endregion

// MediaItemControl.Designer.cs
namespace MusicChange
{
    partial class MediaItemControl
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
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
			this.pictureBoxThumbnail = new System.Windows.Forms.PictureBox();
			this.lblFileName = new System.Windows.Forms.Label();
			this.btnPlay = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnail)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBoxThumbnail
			// 
			this.pictureBoxThumbnail.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.pictureBoxThumbnail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxThumbnail.Image = global::MusicChange.Properties.Resources.DefaultVideoThumbnail;
			this.pictureBoxThumbnail.Location = new System.Drawing.Point(0, 0);
			this.pictureBoxThumbnail.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.pictureBoxThumbnail.Name = "pictureBoxThumbnail";
			this.pictureBoxThumbnail.Size = new System.Drawing.Size(160, 120);
			this.pictureBoxThumbnail.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBoxThumbnail.TabIndex = 2;
			this.pictureBoxThumbnail.TabStop = false;
			this.pictureBoxThumbnail.Click += new System.EventHandler(this.pictureBoxThumbnail_Click);
			// 
			// lblFileName
			// 
			this.lblFileName.Font = new System.Drawing.Font("微软雅黑", 8F);
			this.lblFileName.Location = new System.Drawing.Point(7, 169);
			this.lblFileName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblFileName.Name = "lblFileName";
			this.lblFileName.Size = new System.Drawing.Size(227, 38);
			this.lblFileName.TabIndex = 1;
			// 
			// btnPlay
			// 
			this.btnPlay.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnPlay.Location = new System.Drawing.Point(0, 0);
			this.btnPlay.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnPlay.Name = "btnPlay";
			this.btnPlay.Size = new System.Drawing.Size(160, 120);
			this.btnPlay.TabIndex = 0;
			// 
			// MediaItemControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblFileName);
			this.Controls.Add(this.pictureBoxThumbnail);
			this.Controls.Add(this.btnPlay);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "MediaItemControl";
			this.Size = new System.Drawing.Size(160, 120);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnail)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxThumbnail;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Button btnPlay;
    }
}