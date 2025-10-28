using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MusicChange
{
	public partial class user:Form
	{
        private UsersRepository _usersRepo;
		private bool usernamenull = false;
		private int newId = 0;
		public user()
		{
			// 使用 db.dbPath（确保已初始化）
			_usersRepo = new UsersRepository(db.dbPath);
			InitializeComponent();
		}

		private void btnRegister_Click(object sender, EventArgs e)
		{
			try
			{
				if(!ValidateInput(out string username, out string email, out string password))
				{
					if(usernamenull)
					{
						SetStatus.Text = "未注册";
						var userw = new Users
						{
							Username = username,
							Email = "110959751@qq.com",
							PasswordHash = "123456789",
							Iphone = txtPhone.Text.Trim(),
							FullName = txtFullName.Text.Trim(),
							AvatarPath = "",
							IsActive = true,
							CreatedAt = DateTime.Now,
							UpdatedAt = DateTime.Now
						};
						int newIdw = _usersRepo.Create(userw);

						if(newIdw > 0)
						{
							//SetStatus.Text = "未注册用户成功";
							MessageBox.Show("未注册用户成功。", "注册完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
							this.DialogResult = DialogResult.OK;
							this.Close();
						}
						else
						{
							SetStatus.Text = "注册失败，请重试";
						}

					}
					return;
				}
				// 检查用户名是否存在
				if(_usersRepo.ExistsByUsername(username))
				{
					SetStatus.Text = $"用户名已存在，请更换";
					return;
				}

				// 生成密码哈希（格式： iterations.salt.hash）
				string passwordHash = PasswordHelper.HashPassword(password);

				var user = new Users
				{
					Username = username,
					Email = email,
					PasswordHash = passwordHash,
					Iphone = txtPhone.Text.Trim(),
					FullName = txtFullName.Text.Trim(),
					AvatarPath = "",
					IsActive = true,
					CreatedAt = DateTime.Now,
					UpdatedAt = DateTime.Now
				};

				newId = _usersRepo.Create(user);
				if(newId > 0)
				{
					SetStatus.Text = "注册成功";
					MessageBox.Show("用户注册成功。", "注册完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
					this.DialogResult = DialogResult.OK;
					this.Close();
				}
				else
				{
					SetStatus.Text = "注册失败，请重试";
				}
			}
			catch(Exception ex)
			{
				SetStatus.Text = "发生错误: " + ex.Message;
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
			this.Dispose();
		}

		private bool ValidateInput(out string username, out string email, out string password)
		{
			username = txtUsername.Text.Trim();
			email = txtEmail.Text.Trim();
			password = txtPassword.Text;
			usernamenull = false;

			if(string.IsNullOrEmpty(username))
			{
				SetStatus.Text = "用户名不能为空";
				usernamenull = true;
				txtUsername.Focus();
				return false;
			}

			if(username.Length < 3)
			{
				SetStatus.Text = "用户名至少 3 个字符";
				txtUsername.Focus();
				return false;
			}

			if(string.IsNullOrEmpty(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
			{
				SetStatus.Text = "请输入有效邮箱地址";
				txtEmail.Focus();
				return false;
			}

			if(string.IsNullOrEmpty(password) || password.Length < 6)
			{
				SetStatus.Text = "密码至少 6 个字符";
				txtPassword.Focus();
				return false;
			}

			if(password != txtConfirm.Text)
			{
				SetStatus.Text = "两次输入密码不匹配";
				txtConfirm.Focus();
				return false;
			}
			return true;
		}

	}
}
