
using System.Windows.Forms;

namespace MusicChange
{
	partial class user
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
			if(disposing && (components != null))
			{
				components.Dispose();
			}

			if(disposing)
			{
				// 释放PictureBox中的图片资源
				if(Controls.Count > 1 && Controls[1] is PictureBox pictureBox && pictureBox.Image != null)
				{
					pictureBox.Image.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(user));
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SetStatus = new DevComponents.DotNetBar.LabelX();
			this.txtPhone = new DevComponents.DotNetBar.Controls.TextBoxX();
			this.txtFullName = new DevComponents.DotNetBar.Controls.TextBoxX();
			this.txtConfirm = new DevComponents.DotNetBar.Controls.TextBoxX();
			this.txtPassword = new DevComponents.DotNetBar.Controls.TextBoxX();
			this.txtEmail = new DevComponents.DotNetBar.Controls.TextBoxX();
			this.txtUsername = new DevComponents.DotNetBar.Controls.TextBoxX();
			this.btnRegister = new DevComponents.DotNetBar.ButtonX();
			this.btnCancel = new DevComponents.DotNetBar.ButtonX();
			this.lblPath = new System.Windows.Forms.Label();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.selectButton = new System.Windows.Forms.Button();
			this.btnConfirm = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.userimage = new System.Windows.Forms.PictureBox();
			this.username = new DevComponents.DotNetBar.LabelX();
			this.labelX1 = new DevComponents.DotNetBar.LabelX();
			this.registertime = new DevComponents.DotNetBar.LabelX();
			this.days = new DevComponents.DotNetBar.LabelX();
			this.versions = new DevComponents.DotNetBar.LabelX();
			this.labelX3 = new DevComponents.DotNetBar.LabelX();
			this.register = new DevComponents.DotNetBar.LabelX();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.userimage)).BeginInit();
			this.SuspendLayout();
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label7.ForeColor = System.Drawing.Color.Green;
			this.label7.Location = new System.Drawing.Point(552, 21);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(124, 27);
			this.label7.TabIndex = 28;
			this.label7.Text = "用户注册";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label6.ForeColor = System.Drawing.Color.White;
			this.label6.Location = new System.Drawing.Point(374, 381);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(59, 20);
			this.label6.TabIndex = 27;
			this.label6.Text = "手机:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label5.ForeColor = System.Drawing.Color.White;
			this.label5.Location = new System.Drawing.Point(374, 323);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(59, 20);
			this.label5.TabIndex = 26;
			this.label5.Text = "姓名:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label4.ForeColor = System.Drawing.Color.White;
			this.label4.Location = new System.Drawing.Point(374, 263);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(99, 20);
			this.label4.TabIndex = 25;
			this.label4.Text = "确认密码:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label3.ForeColor = System.Drawing.Color.White;
			this.label3.Location = new System.Drawing.Point(374, 211);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(59, 20);
			this.label3.TabIndex = 24;
			this.label3.Text = "密码:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label2.ForeColor = System.Drawing.Color.White;
			this.label2.Location = new System.Drawing.Point(374, 148);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(59, 20);
			this.label2.TabIndex = 23;
			this.label2.Text = "邮箱:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(374, 81);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 20);
			this.label1.TabIndex = 22;
			this.label1.Text = "用户名:";
			// 
			// SetStatus
			// 
			// 
			// 
			// 
			this.SetStatus.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
			this.SetStatus.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.SetStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
			this.SetStatus.Location = new System.Drawing.Point(362, 554);
			this.SetStatus.Name = "SetStatus";
			this.SetStatus.Size = new System.Drawing.Size(491, 100);
			this.SetStatus.TabIndex = 15;
			this.SetStatus.WordWrap = true;
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
			this.txtPhone.Location = new System.Drawing.Point(503, 371);
			this.txtPhone.Name = "txtPhone";
			this.txtPhone.PreventEnterBeep = true;
			this.txtPhone.Size = new System.Drawing.Size(326, 30);
			this.txtPhone.TabIndex = 21;
			this.txtPhone.WatermarkColor = System.Drawing.SystemColors.ButtonHighlight;
			this.txtPhone.WatermarkImageAlignment = System.Drawing.ContentAlignment.MiddleRight;
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
			this.txtFullName.Location = new System.Drawing.Point(503, 313);
			this.txtFullName.Name = "txtFullName";
			this.txtFullName.PreventEnterBeep = true;
			this.txtFullName.Size = new System.Drawing.Size(326, 30);
			this.txtFullName.TabIndex = 20;
			this.txtFullName.WatermarkColor = System.Drawing.SystemColors.ButtonHighlight;
			this.txtFullName.WatermarkImageAlignment = System.Drawing.ContentAlignment.MiddleRight;
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
			this.txtConfirm.Location = new System.Drawing.Point(503, 252);
			this.txtConfirm.Name = "txtConfirm";
			this.txtConfirm.PreventEnterBeep = true;
			this.txtConfirm.Size = new System.Drawing.Size(326, 30);
			this.txtConfirm.TabIndex = 19;
			this.txtConfirm.Text = "111111";
			this.txtConfirm.WatermarkColor = System.Drawing.SystemColors.ButtonHighlight;
			this.txtConfirm.WatermarkImage = global::MusicChange.Properties.Resources.required;
			this.txtConfirm.WatermarkImageAlignment = System.Drawing.ContentAlignment.MiddleRight;
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
			this.txtPassword.Location = new System.Drawing.Point(503, 192);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PreventEnterBeep = true;
			this.txtPassword.Size = new System.Drawing.Size(326, 30);
			this.txtPassword.TabIndex = 18;
			this.txtPassword.Text = "111111";
			this.txtPassword.WatermarkColor = System.Drawing.SystemColors.ButtonHighlight;
			this.txtPassword.WatermarkImage = global::MusicChange.Properties.Resources.required;
			this.txtPassword.WatermarkImageAlignment = System.Drawing.ContentAlignment.MiddleRight;
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
			this.txtEmail.Location = new System.Drawing.Point(503, 129);
			this.txtEmail.Name = "txtEmail";
			this.txtEmail.PreventEnterBeep = true;
			this.txtEmail.Size = new System.Drawing.Size(326, 30);
			this.txtEmail.TabIndex = 17;
			this.txtEmail.Text = "yd@qq.com";
			this.txtEmail.WatermarkColor = System.Drawing.SystemColors.ButtonHighlight;
			this.txtEmail.WatermarkImage = global::MusicChange.Properties.Resources.required;
			this.txtEmail.WatermarkImageAlignment = System.Drawing.ContentAlignment.MiddleRight;
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
			this.txtUsername.Location = new System.Drawing.Point(503, 71);
			this.txtUsername.Name = "txtUsername";
			this.txtUsername.PreventEnterBeep = true;
			this.txtUsername.Size = new System.Drawing.Size(326, 30);
			this.txtUsername.TabIndex = 16;
			this.txtUsername.Text = "yyy";
			this.txtUsername.WatermarkColor = System.Drawing.SystemColors.ButtonHighlight;
			this.txtUsername.WatermarkImage = global::MusicChange.Properties.Resources.required;
			this.txtUsername.WatermarkImageAlignment = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnRegister
			// 
			this.btnRegister.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
			this.btnRegister.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
			this.btnRegister.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnRegister.Location = new System.Drawing.Point(413, 682);
			this.btnRegister.Name = "btnRegister";
			this.btnRegister.Size = new System.Drawing.Size(96, 33);
			this.btnRegister.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
			this.btnRegister.TabIndex = 30;
			this.btnRegister.Text = "注册";
			this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
			this.btnCancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
			this.btnCancel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnCancel.Location = new System.Drawing.Point(715, 682);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(93, 33);
			this.btnCancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
			this.btnCancel.TabIndex = 29;
			this.btnCancel.Text = "取消";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// lblPath
			// 
			this.lblPath.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lblPath.ForeColor = System.Drawing.Color.White;
			this.lblPath.Location = new System.Drawing.Point(762, 467);
			this.lblPath.Name = "lblPath";
			this.lblPath.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lblPath.Size = new System.Drawing.Size(117, 30);
			this.lblPath.TabIndex = 34;
			this.lblPath.Text = "保存路径";
			// 
			// pictureBox
			// 
			this.pictureBox.BackColor = System.Drawing.Color.DimGray;
			this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox.Location = new System.Drawing.Point(629, 445);
			this.pictureBox.Margin = new System.Windows.Forms.Padding(30);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(100, 100);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			// 
			// selectButton
			// 
			this.selectButton.AutoSize = true;
			this.selectButton.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.selectButton.Location = new System.Drawing.Point(16, 25);
			this.selectButton.Name = "selectButton";
			this.selectButton.Size = new System.Drawing.Size(119, 30);
			this.selectButton.TabIndex = 32;
			this.selectButton.Text = "选择图片:";
			this.selectButton.UseVisualStyleBackColor = true;
			this.selectButton.Click += new System.EventHandler(this.SelectButton_Click);
			// 
			// btnConfirm
			// 
			this.btnConfirm.AutoSize = true;
			this.btnConfirm.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.btnConfirm.Location = new System.Drawing.Point(28, 84);
			this.btnConfirm.Name = "btnConfirm";
			this.btnConfirm.Size = new System.Drawing.Size(119, 30);
			this.btnConfirm.TabIndex = 33;
			this.btnConfirm.Text = "确认并保存";
			this.btnConfirm.UseVisualStyleBackColor = true;
			this.btnConfirm.Visible = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.selectButton);
			this.panel1.Controls.Add(this.btnConfirm);
			this.panel1.Location = new System.Drawing.Point(362, 434);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(163, 141);
			this.panel1.TabIndex = 35;
			// 
			// userimage
			// 
			this.userimage.BackColor = System.Drawing.Color.DimGray;
			this.userimage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.userimage.Location = new System.Drawing.Point(112, 68);
			this.userimage.Margin = new System.Windows.Forms.Padding(30);
			this.userimage.Name = "userimage";
			this.userimage.Size = new System.Drawing.Size(100, 100);
			this.userimage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.userimage.TabIndex = 37;
			this.userimage.TabStop = false;
			// 
			// username
			// 
			this.username.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			// 
			// 
			// 
			this.username.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
			this.username.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.username.FontBold = true;
			this.username.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
			this.username.Location = new System.Drawing.Point(23, 230);
			this.username.Name = "username";
			this.username.Size = new System.Drawing.Size(318, 29);
			this.username.TabIndex = 38;
			this.username.Text = "你好：";
			// 
			// labelX1
			// 
			this.labelX1.AutoSize = true;
			this.labelX1.BackColor = System.Drawing.Color.White;
			// 
			// 
			// 
			this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
			this.labelX1.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.labelX1.FontBold = true;
			this.labelX1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
			this.labelX1.Location = new System.Drawing.Point(112, 281);
			this.labelX1.Name = "labelX1";
			this.labelX1.Size = new System.Drawing.Size(107, 34);
			this.labelX1.TabIndex = 39;
			this.labelX1.Text = "注册时间";
			// 
			// registertime
			// 
			this.registertime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			// 
			// 
			// 
			this.registertime.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
			this.registertime.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.registertime.FontBold = true;
			this.registertime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
			this.registertime.Location = new System.Drawing.Point(23, 337);
			this.registertime.Name = "registertime";
			this.registertime.Size = new System.Drawing.Size(300, 29);
			this.registertime.TabIndex = 40;
			// 
			// days
			// 
			this.days.BackColor = System.Drawing.Color.Lime;
			// 
			// 
			// 
			this.days.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
			this.days.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.days.FontBold = true;
			this.days.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
			this.days.Location = new System.Drawing.Point(23, 396);
			this.days.Name = "days";
			this.days.Size = new System.Drawing.Size(300, 29);
			this.days.TabIndex = 41;
			// 
			// versions
			// 
			this.versions.BackColor = System.Drawing.Color.Black;
			// 
			// 
			// 
			this.versions.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
			this.versions.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.versions.FontBold = true;
			this.versions.ForeColor = System.Drawing.Color.White;
			this.versions.Location = new System.Drawing.Point(23, 639);
			this.versions.Name = "versions";
			this.versions.Size = new System.Drawing.Size(300, 29);
			this.versions.TabIndex = 42;
			this.versions.Text = "当前版本：V1.0";
			// 
			// labelX3
			// 
			this.labelX3.BackColor = System.Drawing.Color.Black;
			// 
			// 
			// 
			this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
			this.labelX3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.labelX3.FontBold = true;
			this.labelX3.ForeColor = System.Drawing.Color.White;
			this.labelX3.Location = new System.Drawing.Point(23, 677);
			this.labelX3.Name = "labelX3";
			this.labelX3.Size = new System.Drawing.Size(300, 29);
			this.labelX3.TabIndex = 43;
			this.labelX3.Text = "作者Email:110959751@qq.com";
			// 
			// register
			// 
			this.register.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
			// 
			// 
			// 
			this.register.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
			this.register.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.register.FontBold = true;
			this.register.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
			this.register.Location = new System.Drawing.Point(23, 460);
			this.register.Name = "register";
			this.register.Size = new System.Drawing.Size(190, 29);
			this.register.TabIndex = 44;
			// 
			// user
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.ClientSize = new System.Drawing.Size(891, 742);
			this.Controls.Add(this.lblPath);
			this.Controls.Add(this.register);
			this.Controls.Add(this.labelX3);
			this.Controls.Add(this.versions);
			this.Controls.Add(this.days);
			this.Controls.Add(this.registertime);
			this.Controls.Add(this.labelX1);
			this.Controls.Add(this.username);
			this.Controls.Add(this.userimage);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.btnRegister);
			this.Controls.Add(this.btnCancel);
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
			this.Controls.Add(this.pictureBox);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "user";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "用户信息";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.userimage)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private DevComponents.DotNetBar.LabelX SetStatus;
		private DevComponents.DotNetBar.Controls.TextBoxX txtPhone;
		private DevComponents.DotNetBar.Controls.TextBoxX txtFullName;
		private DevComponents.DotNetBar.Controls.TextBoxX txtConfirm;
		private DevComponents.DotNetBar.Controls.TextBoxX txtPassword;
		private DevComponents.DotNetBar.Controls.TextBoxX txtEmail;
		private DevComponents.DotNetBar.Controls.TextBoxX txtUsername;
		private DevComponents.DotNetBar.ButtonX btnRegister;
		private DevComponents.DotNetBar.ButtonX btnCancel;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.Button selectButton;
		private System.Windows.Forms.Button btnConfirm;
		private System.Windows.Forms.Label lblPath;
		private System.Windows.Forms.Panel panel1;
		public	PictureBox userimage;
		public DevComponents.DotNetBar.LabelX username;
		private DevComponents.DotNetBar.LabelX labelX1;
		private DevComponents.DotNetBar.LabelX registertime;
		private DevComponents.DotNetBar.LabelX days;
		private DevComponents.DotNetBar.LabelX versions;
		private DevComponents.DotNetBar.LabelX labelX3;
		private DevComponents.DotNetBar.LabelX register;
	}
}