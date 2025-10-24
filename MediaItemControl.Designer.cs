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
        //    if(disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
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
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnail)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBoxThumbnail
			// 
			this.pictureBoxThumbnail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBoxThumbnail.BackColor = System.Drawing.Color.Transparent;
			this.pictureBoxThumbnail.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureBoxThumbnail.Location = new System.Drawing.Point(1, 0);
			this.pictureBoxThumbnail.Margin = new System.Windows.Forms.Padding(1);
			this.pictureBoxThumbnail.Name = "pictureBoxThumbnail";
			this.pictureBoxThumbnail.Size = new System.Drawing.Size(170, 92);
			this.pictureBoxThumbnail.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBoxThumbnail.TabIndex = 2;
			this.pictureBoxThumbnail.TabStop = false;
			this.pictureBoxThumbnail.WaitOnLoad = true;
			this.pictureBoxThumbnail.Click += new System.EventHandler(this.pictureBoxThumbnail_Click);
			// 
			// lblFileName
			// 
			this.lblFileName.Font = new System.Drawing.Font("微软雅黑", 9F);
			this.lblFileName.Location = new System.Drawing.Point(7, 169);
			this.lblFileName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lblFileName.Name = "lblFileName";
			this.lblFileName.Size = new System.Drawing.Size(227, 38);
			this.lblFileName.TabIndex = 1;
			// 
			// btnPlay
			// 
			this.btnPlay.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnPlay.Location = new System.Drawing.Point(0, 0);
			this.btnPlay.Margin = new System.Windows.Forms.Padding(4);
			this.btnPlay.Name = "btnPlay";
			this.btnPlay.Size = new System.Drawing.Size(160, 120);
			this.btnPlay.TabIndex = 0;
			// 
			// LTimeLength
			// 
			this.LTimeLength.BackColor = System.Drawing.Color.Transparent;
			this.LTimeLength.ForeColor = System.Drawing.Color.Black;
			this.LTimeLength.Location = new System.Drawing.Point(102, 8);
			this.LTimeLength.Name = "LTimeLength";
			this.LTimeLength.Size = new System.Drawing.Size(50, 20);
			this.LTimeLength.TabIndex = 3;
			// 
			// MediaItemControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.LTimeLength);
			this.Controls.Add(this.lblFileName);
			this.Controls.Add(this.pictureBoxThumbnail);
			this.Controls.Add(this.btnPlay);
			this.ForeColor = System.Drawing.SystemColors.ActiveCaption;
			this.Margin = new System.Windows.Forms.Padding(5, 1, 8, 1);
			this.Name = "MediaItemControl";
			this.Size = new System.Drawing.Size(160, 120);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnail)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxThumbnail;
        public System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Button btnPlay;
		private System.Windows.Forms.Label LTimeLength;
	}
}