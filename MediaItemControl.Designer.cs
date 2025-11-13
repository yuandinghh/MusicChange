#region  --- old  ---
#endregion

// MediaItemControl.Designer.cs
using System.Drawing;

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
		//protected override void Dispose(bool disposing)
		//{
		//	if (disposing && (components != null)) {
		//		components.Dispose();
		//	}
		//	base.Dispose( disposing );
		//}

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
			this.LTimeLength = new System.Windows.Forms.Label();
			this.butadd = new DevComponents.DotNetBar.ButtonX();
			this.hadadd = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnail)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBoxThumbnail
			// 
			this.pictureBoxThumbnail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBoxThumbnail.BackColor = System.Drawing.Color.Transparent;
			this.pictureBoxThumbnail.Location = new System.Drawing.Point(0, 0);
			this.pictureBoxThumbnail.Margin = new System.Windows.Forms.Padding(5, 5, 5, 10);
			this.pictureBoxThumbnail.Name = "pictureBoxThumbnail";
			this.pictureBoxThumbnail.Padding = new System.Windows.Forms.Padding(1);
			this.pictureBoxThumbnail.Size = new System.Drawing.Size(180, 120);
			this.pictureBoxThumbnail.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBoxThumbnail.TabIndex = 2;
			this.pictureBoxThumbnail.TabStop = false;
			this.pictureBoxThumbnail.WaitOnLoad = true;
			this.pictureBoxThumbnail.Click += new System.EventHandler(this.pictureBoxThumbnail_Click);
			// 
			// lblFileName
			// 
			this.lblFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblFileName.AutoEllipsis = true;
			this.lblFileName.BackColor = System.Drawing.Color.White;
			this.lblFileName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
			this.lblFileName.ForeColor = System.Drawing.Color.Black;
			this.lblFileName.Location = new System.Drawing.Point(0, 116);
			this.lblFileName.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
			this.lblFileName.Name = "lblFileName";
			this.lblFileName.Size = new System.Drawing.Size(180, 25);
			this.lblFileName.TabIndex = 1;
			this.lblFileName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnPlay
			// 
			this.btnPlay.AutoSize = true;
			this.btnPlay.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnPlay.Location = new System.Drawing.Point(5, 5);
			this.btnPlay.Margin = new System.Windows.Forms.Padding(4);
			this.btnPlay.Name = "btnPlay";
			this.btnPlay.Size = new System.Drawing.Size(170, 135);
			this.btnPlay.TabIndex = 0;
			// 
			// LTimeLength
			// 
			this.LTimeLength.BackColor = System.Drawing.Color.White;
			this.LTimeLength.ForeColor = System.Drawing.Color.Black;
			this.LTimeLength.Location = new System.Drawing.Point(125, 0);
			this.LTimeLength.Margin = new System.Windows.Forms.Padding(0);
			this.LTimeLength.Name = "LTimeLength";
			this.LTimeLength.Size = new System.Drawing.Size(50, 20);
			this.LTimeLength.TabIndex = 3;
			// 
			// butadd
			// 
			this.butadd.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
			this.butadd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.butadd.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
			this.butadd.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.butadd.Location = new System.Drawing.Point(145, 81);
			this.butadd.Margin = new System.Windows.Forms.Padding(5);
			this.butadd.Name = "butadd";
			this.butadd.Size = new System.Drawing.Size(30, 30);
			this.butadd.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
			this.butadd.Symbol = "";
			this.butadd.SymbolColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
			this.butadd.SymbolSize = 12F;
			this.butadd.TabIndex = 4;
			this.butadd.Tooltip = "添加到编辑";
			this.butadd.Click += new System.EventHandler(this.butadd_Click);
			// 
			// hadadd
			// 
			this.hadadd.AutoSize = true;
			this.hadadd.BackColor = System.Drawing.Color.Black;
			this.hadadd.ForeColor = System.Drawing.Color.Transparent;
			this.hadadd.Location = new System.Drawing.Point(5, 5);
			this.hadadd.Margin = new System.Windows.Forms.Padding(0);
			this.hadadd.Name = "hadadd";
			this.hadadd.Size = new System.Drawing.Size(52, 15);
			this.hadadd.TabIndex = 5;
			this.hadadd.Text = "已添加";
			// 
			// MediaItemControl
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Gray;
			this.Controls.Add(this.hadadd);
			this.Controls.Add(this.butadd);
			this.Controls.Add(this.LTimeLength);
			this.Controls.Add(this.lblFileName);
			this.Controls.Add(this.pictureBoxThumbnail);
			this.Controls.Add(this.btnPlay);
			this.ForeColor = System.Drawing.SystemColors.ActiveCaption;
			this.Margin = new System.Windows.Forms.Padding(10, 10, 10, 18);
			this.Name = "MediaItemControl";
			this.Padding = new System.Windows.Forms.Padding(5);
			this.Size = new System.Drawing.Size(180, 145);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MediaItemControl_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MediaItemControl_DragEnter);
			this.MouseEnter += new System.EventHandler(this.MediaItemControl_MouseEnter);
			this.MouseLeave += new System.EventHandler(this.MediaItemControl_MouseLeave);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MediaItemControl_MouseMove);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnail)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxThumbnail;
        public System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Button btnPlay;
		private System.Windows.Forms.Label LTimeLength;
		private DevComponents.DotNetBar.ButtonX butadd;
		private System.Windows.Forms.Label hadadd;
	}
}