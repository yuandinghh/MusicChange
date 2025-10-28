namespace MusicChange
{
	partial class RegistrationForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent( )
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegistrationForm));
			this.btnCancel = new DevComponents.DotNetBar.ButtonX();
			this.btnRegister = new DevComponents.DotNetBar.ButtonX();
			this.txtUsername = new DevComponents.DotNetBar.Controls.TextBoxX();
			this.txtEmail = new DevComponents.DotNetBar.Controls.TextBoxX();
			this.txtPassword = new DevComponents.DotNetBar.Controls.TextBoxX();
			this.txtConfirm = new DevComponents.DotNetBar.Controls.TextBoxX();
			this.txtFullName = new DevComponents.DotNetBar.Controls.TextBoxX();
			this.txtPhone = new DevComponents.DotNetBar.Controls.TextBoxX();
			this.SetStatus = new DevComponents.DotNetBar.LabelX();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
			this.btnCancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
			this.btnCancel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnCancel.Location = new System.Drawing.Point(313, 542);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(93, 33);
			this.btnCancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
			this.btnCancel.TabIndex = 0;
			this.btnCancel.Text = "取消";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnRegister
			// 
			this.btnRegister.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
			this.btnRegister.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
			this.btnRegister.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnRegister.Location = new System.Drawing.Point(76, 542);
			this.btnRegister.Name = "btnRegister";
			this.btnRegister.Size = new System.Drawing.Size(96, 33);
			this.btnRegister.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
			this.btnRegister.TabIndex = 1;
			this.btnRegister.Text = "注册";
			this.btnRegister.Click += new System.EventHandler(this.BtnRegister_Click);
			// 
			// txtUsername
			// 
			this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// 
			// 
			this.txtUsername.Border.Class = "TextBoxBorder";
			this.txtUsername.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
			this.txtUsername.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.txtUsername.Location = new System.Drawing.Point(143, 98);
			this.txtUsername.Name = "txtUsername";
			this.txtUsername.PreventEnterBeep = true;
			this.txtUsername.Size = new System.Drawing.Size(263, 30);
			this.txtUsername.TabIndex = 2;
			this.txtUsername.WatermarkColor = System.Drawing.SystemColors.ButtonHighlight;
			this.txtUsername.WatermarkImage = global::MusicChange.Properties.Resources.required;
			this.txtUsername.WatermarkImageAlignment = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtEmail
			// 
			this.txtEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// 
			// 
			this.txtEmail.Border.Class = "TextBoxBorder";
			this.txtEmail.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
			this.txtEmail.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.txtEmail.Location = new System.Drawing.Point(143, 156);
			this.txtEmail.Name = "txtEmail";
			this.txtEmail.PreventEnterBeep = true;
			this.txtEmail.Size = new System.Drawing.Size(263, 30);
			this.txtEmail.TabIndex = 3;
			this.txtEmail.WatermarkColor = System.Drawing.SystemColors.ButtonHighlight;
			this.txtEmail.WatermarkImage = global::MusicChange.Properties.Resources.required;
			this.txtEmail.WatermarkImageAlignment = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtPassword
			// 
			this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// 
			// 
			this.txtPassword.Border.Class = "TextBoxBorder";
			this.txtPassword.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
			this.txtPassword.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.txtPassword.Location = new System.Drawing.Point(143, 219);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PreventEnterBeep = true;
			this.txtPassword.Size = new System.Drawing.Size(263, 30);
			this.txtPassword.TabIndex = 4;
			this.txtPassword.WatermarkColor = System.Drawing.SystemColors.ButtonHighlight;
			this.txtPassword.WatermarkImage = global::MusicChange.Properties.Resources.required;
			this.txtPassword.WatermarkImageAlignment = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtConfirm
			// 
			this.txtConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// 
			// 
			this.txtConfirm.Border.Class = "TextBoxBorder";
			this.txtConfirm.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
			this.txtConfirm.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.txtConfirm.Location = new System.Drawing.Point(143, 279);
			this.txtConfirm.Name = "txtConfirm";
			this.txtConfirm.PreventEnterBeep = true;
			this.txtConfirm.Size = new System.Drawing.Size(263, 30);
			this.txtConfirm.TabIndex = 5;
			this.txtConfirm.WatermarkColor = System.Drawing.SystemColors.ButtonHighlight;
			this.txtConfirm.WatermarkImage = global::MusicChange.Properties.Resources.required;
			this.txtConfirm.WatermarkImageAlignment = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtFullName
			// 
			this.txtFullName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// 
			// 
			this.txtFullName.Border.Class = "TextBoxBorder";
			this.txtFullName.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
			this.txtFullName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.txtFullName.Location = new System.Drawing.Point(143, 340);
			this.txtFullName.Name = "txtFullName";
			this.txtFullName.PreventEnterBeep = true;
			this.txtFullName.Size = new System.Drawing.Size(263, 30);
			this.txtFullName.TabIndex = 6;
			this.txtFullName.WatermarkColor = System.Drawing.SystemColors.ButtonHighlight;
			this.txtFullName.WatermarkImageAlignment = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtPhone
			// 
			this.txtPhone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// 
			// 
			this.txtPhone.Border.Class = "TextBoxBorder";
			this.txtPhone.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
			this.txtPhone.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.txtPhone.Location = new System.Drawing.Point(143, 406);
			this.txtPhone.Name = "txtPhone";
			this.txtPhone.PreventEnterBeep = true;
			this.txtPhone.Size = new System.Drawing.Size(263, 30);
			this.txtPhone.TabIndex = 7;
			this.txtPhone.WatermarkColor = System.Drawing.SystemColors.ButtonHighlight;
			this.txtPhone.WatermarkImageAlignment = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// SetStatus
			// 
			// 
			// 
			// 
			this.SetStatus.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
			this.SetStatus.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.SetStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
			this.SetStatus.Location = new System.Drawing.Point(63, 467);
			this.SetStatus.Name = "SetStatus";
			this.SetStatus.Size = new System.Drawing.Size(343, 59);
			this.SetStatus.TabIndex = 0;
			this.SetStatus.WordWrap = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(27, 108);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 20);
			this.label1.TabIndex = 8;
			this.label1.Text = "用户名:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label2.ForeColor = System.Drawing.Color.White;
			this.label2.Location = new System.Drawing.Point(27, 166);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(59, 20);
			this.label2.TabIndex = 9;
			this.label2.Text = "邮箱:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label3.ForeColor = System.Drawing.Color.White;
			this.label3.Location = new System.Drawing.Point(27, 229);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(59, 20);
			this.label3.TabIndex = 10;
			this.label3.Text = "密码:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label4.ForeColor = System.Drawing.Color.White;
			this.label4.Location = new System.Drawing.Point(27, 289);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(99, 20);
			this.label4.TabIndex = 11;
			this.label4.Text = "确认密码:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label5.ForeColor = System.Drawing.Color.White;
			this.label5.Location = new System.Drawing.Point(27, 350);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(59, 20);
			this.label5.TabIndex = 12;
			this.label5.Text = "姓名:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label6.ForeColor = System.Drawing.Color.White;
			this.label6.Location = new System.Drawing.Point(27, 408);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(59, 20);
			this.label6.TabIndex = 13;
			this.label6.Text = "手机:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label7.ForeColor = System.Drawing.Color.Green;
			this.label7.Location = new System.Drawing.Point(158, 35);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(124, 27);
			this.label7.TabIndex = 14;
			this.label7.Text = "用户注册";
			// 
			// RegistrationForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(464, 605);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.SetStatus);
			this.Controls.Add(this.txtPhone);
			this.Controls.Add(this.txtFullName);
			this.Controls.Add(this.txtConfirm);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.txtEmail);
			this.Controls.Add(this.txtUsername);
			this.Controls.Add(this.btnRegister);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "RegistrationForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "用户注册";
			this.Load += new System.EventHandler(this.RegistrationForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private DevComponents.DotNetBar.ButtonX btnCancel;
		private DevComponents.DotNetBar.ButtonX btnRegister;
		private DevComponents.DotNetBar.Controls.TextBoxX txtUsername;
		private DevComponents.DotNetBar.Controls.TextBoxX txtEmail;
		private DevComponents.DotNetBar.Controls.TextBoxX txtPassword;
		private DevComponents.DotNetBar.Controls.TextBoxX txtConfirm;
		private DevComponents.DotNetBar.Controls.TextBoxX txtFullName;
		private DevComponents.DotNetBar.Controls.TextBoxX txtPhone;
		private DevComponents.DotNetBar.LabelX SetStatus;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
	}
}