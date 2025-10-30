
namespace MusicChange
{
	partial class AvatarSelector

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
		private void   InitializeComponent()
		{
			this.btnSelect = new System.Windows.Forms.Button();
			this.picAvatar = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.picAvatar)).BeginInit();
			this.SuspendLayout();
			// 
			// btnSelect
			// 
			this.btnSelect.Location = new System.Drawing.Point(13, 68);
			this.btnSelect.Name = "btnSelect";
			this.btnSelect.Size = new System.Drawing.Size(75, 23);
			this.btnSelect.TabIndex = 1;
			this.btnSelect.Text = "选择头像";
			this.btnSelect.UseVisualStyleBackColor = true;
			// 
			// picAvatar
			// 
			this.picAvatar.Image = global::MusicChange.Properties.Resources.man2;
			this.picAvatar.Location = new System.Drawing.Point(26, 3);
			this.picAvatar.Name = "picAvatar";
			this.picAvatar.Size = new System.Drawing.Size(50, 50);
			this.picAvatar.TabIndex = 0;
			this.picAvatar.TabStop = false;
			// 
			// AvatarSelector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.Controls.Add(this.btnSelect);
			this.Controls.Add(this.picAvatar);
			this.Name = "AvatarSelector";
			this.Size = new System.Drawing.Size(100, 91);
			((System.ComponentModel.ISupportInitialize)(this.picAvatar)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox picAvatar;
		private System.Windows.Forms.Button btnSelect;
	}
}
