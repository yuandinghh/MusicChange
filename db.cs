using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using MusicChange.Database.Models;
using System.Windows.Controls;

namespace MusicChange
{
	#region --------- User 表 处理  ------------
	// 数据库操作封装类
	public class db
	{
		// 数据库连接字符串（建议使用绝对路径避免路径问题）
		private string _connectionString;

		public db(string dbPath)
		{
			_connectionString = $"Data Source={dbPath};Version=3;";
		}

		// 1. 初始化数据库（创建表）
		public void InitDatabase()
		{
			//先判断数据库是否存在，如果最存在，看看是否已经有相同的表
			if(!System.IO.File.Exists(_connectionString.Split('=')[1].Split(';')[0]))
			{
				MessageBox.Show("数据库文件不存在，正在创建...");
			}
			else
			{
				MessageBox.Show("数据库文件已存在，正在检查表结构...");
				using(var connection = new SQLiteConnection(_connectionString))
				{
					connection.Open();
					string checkTableSql = "SELECT name FROM sqlite_master WHERE type='table' AND name='Users'";
					using(var command = new SQLiteCommand(checkTableSql, connection))
					{
						var result = command.ExecuteScalar();
						if(result != null)
						{
							MessageBox.Show("表已存在，无需创建。");
							return; // 表已存在，直接返回
						}
					}
				}
			}


			try
			{
				using(var connection = new SQLiteConnection(_connectionString))
				{
					connection.Open();
					string createTableSql = @"
                        CREATE TABLE IF NOT EXISTS Users (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT NOT NULL,
                            Age INTEGER CHECK(Age > 0),  -- 年龄必须为正数
                            Address TEXT
                        )";
					using(var command = new SQLiteCommand(createTableSql, connection))
					{
						command.ExecuteNonQuery();
					}

					MessageBox.Show("数据库初始化成功");
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show($"初始化失败：{ex.Message}");
			}
		}

		// 2. 新增数据（Create）
		public bool InsertUser(User user)
		{
			try
			{
				using(var connection = new SQLiteConnection(_connectionString))
				{
					connection.Open();
					string insertSql = "INSERT INTO Users (Name, Age, Address) VALUES (@Name, @Age, @Address)";
					using(var command = new SQLiteCommand(insertSql, connection))
					{
						// 参数化查询（防止SQL注入）
						command.Parameters.AddWithValue("@Name", user.Name);
						command.Parameters.AddWithValue("@Age", user.Age);
						command.Parameters.AddWithValue("@Address", user.Address);
						int rowsAffected = command.ExecuteNonQuery();
						return rowsAffected > 0; // 插入成功返回true
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show($"插入失败：{ex.Message}");
				return false;
			}
		}

		// 3. 查询所有数据（Read All）
		public void GetAllUsers()
		{
			try
			{
				using(var connection = new SQLiteConnection(_connectionString))
				{
					connection.Open();
					string selectSql = "SELECT Id, Name, Age, Address FROM Users";
					using(var command = new SQLiteCommand(selectSql, connection))
					{
						using(var reader = command.ExecuteReader())
						{
							MessageBox.Show("\n所有用户数据：");
							while(reader.Read())
							{
								var user = new User
								{
									Id = reader.GetInt32(0),
									Name = reader.GetString(1),
									Age = reader.GetInt32(2),
									Address = reader.IsDBNull(3) ? "未填写" : reader.GetString(3)
								};
								MessageBox.Show($"ID: {user.Id}, 姓名: {user.Name}, 年龄: {user.Age}, 地址: {user.Address}");
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show($"查询失败：{ex.Message}");
			}
		}
		/// <summary>
		///  4. 查询单条数据（Read Single）
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public User GetUserById(int id)
		{
			try
			{
				using(var connection = new SQLiteConnection(_connectionString))
				{
					connection.Open();
					string selectSql = "SELECT Id, Name, Age, Address FROM Users WHERE Id = @Id";
					using(var command = new SQLiteCommand(selectSql, connection))
					{
						command.Parameters.AddWithValue("@Id", id);
						using(var reader = command.ExecuteReader())
						{
							if(reader.Read())
							{
								return new User
								{
									Id = reader.GetInt32(0),
									Name = reader.GetString(1),
									Age = reader.GetInt32(2),
									Address = reader.IsDBNull(3) ? "未填写" : reader.GetString(3)
								};
							}
							return null; // 未找到数据
						}
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show($"查询失败：{ex.Message}");
				return null;
			}
		}
		/// <summary>
		///  5. 更新数据（Update）
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public bool UpdateUser(User user)
		{
			try
			{
				using(var connection = new SQLiteConnection(_connectionString))
				{
					connection.Open();
					string updateSql = "UPDATE Users SET Name = @Name, Age = @Age, Address = @Address WHERE Id = @Id";
					using(var command = new SQLiteCommand(updateSql, connection))
					{
						command.Parameters.AddWithValue("@Id", user.Id);
						command.Parameters.AddWithValue("@Name", user.Name);
						command.Parameters.AddWithValue("@Age", user.Age);
						command.Parameters.AddWithValue("@Address", user.Address);
						int rowsAffected = command.ExecuteNonQuery();
						return rowsAffected > 0; // 更新成功返回true
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show($"更新失败：{ex.Message}");
				return false;
			}
		}

		// 6. 删除数据（Delete）
		public bool DeleteUser(int id)
		{
			try
			{
				using(var connection = new SQLiteConnection(_connectionString))
				{
					connection.Open();
					string deleteSql = "DELETE FROM Users WHERE Id = @Id";
					using(var command = new SQLiteCommand(deleteSql, connection))
					{
						command.Parameters.AddWithValue("@Id", id);
						int rowsAffected = command.ExecuteNonQuery();
						return rowsAffected > 0; // 删除成功返回true
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show($"删除失败：{ex.Message}");
				return false;
			}
		}
		/// <summary>
		///  主程序入口
		/// </summary>
		public void RunData()
		{

			// 数据库文件路径（建议使用绝对路径，例如：@"C:\Data\TestDB.db"）
			string dbPath = $"D:\\Documents\\Visual Studio 2022\\MusicChange\\LaserEditing.db";
			var dbAccess = new db(dbPath);

			// 初始化数据库
			dbAccess.InitDatabase();

			// 测试新增
			var user1 = new User { Name = "张三", Age = 28, Address = "广州" };
			bool isInserted = dbAccess.InsertUser(user1);
			MessageBox.Show($"新增结果：{(isInserted ? "成功" : "失败")}", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
			var user2 = new User { Name = "李四", Age = 32, Address = "成都" };
			dbAccess.InsertUser(user2);

			// 测试查询所有
			dbAccess.GetAllUsers();

			// 测试查询单条
			var user = dbAccess.GetUserById(1);
			if(user != null)
			{
				MessageBox.Show($"\n查询ID=1的用户：{user.Name}，{user.Age}岁");
			}

			// 测试更新
			if(user != null)
			{
				user.Age = 29; // 修改年龄
				user.Address = "深圳"; // 修改地址
				bool isUpdated = dbAccess.UpdateUser(user);
				MessageBox.Show($"更新结果：{(isUpdated ? "成功" : "失败")}");
			}

			// 再次查询所有（查看更新后的数据）
			dbAccess.GetAllUsers();

			// 测试删除
			bool isDeleted = dbAccess.DeleteUser(2);
			MessageBox.Show($"删除ID=2的结果：{(isDeleted ? "成功" : "失败")}");

			// 最终数据
			dbAccess.GetAllUsers();

			MessageBox.Show("\n操作完成，按任意键退出...");
		}
		#endregion

		/*
        1.	clips - 存储音频剪辑的基本信息
		2.	projects - 存储项目信息
		3.	clip_effects - 存储剪辑效果设置

        1.	创建(Create): InsertProject, InsertClip, InsertClipEffect
        2.	读取(Read): GetProjectById, GetAllProjects, GetClipById, GetClipsByProjectId, GetAllClips, GetClipEffectById, GetClipEffectsByClipId
        3.	更新(Update): UpdateProject, UpdateClip, UpdateClipEffect
        4.	删除(Delete): DeleteProject, DeleteClip, DeleteClipEffect
        */

#region ---------创建一个 剪辑数据库的详细表，并给出c# sqlite 程序  ------------
		/// <summary>
		/// InitializeDatabase  创建数据库，创建表
		/// </summary>
		public void InitializeDatabase()
		{
			using(var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				// 创建 projects 表
				string createProjectsTable = @"
                    CREATE TABLE IF NOT EXISTS projects (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        name TEXT NOT NULL,
                        description TEXT,
                        created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
                    )";

				using(var command = new SQLiteCommand(createProjectsTable, connection))
				{
					command.ExecuteNonQuery();
				}

				// 创建 clips 表
				string createClipsTable = @"
                    CREATE TABLE IF NOT EXISTS clips (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        name TEXT NOT NULL,
                        file_path TEXT NOT NULL,
                        project_id INTEGER NOT NULL,
                        start_position REAL NOT NULL DEFAULT 0,
                        end_position REAL NOT NULL,
                        speed REAL NOT NULL DEFAULT 1.0,
                        pitch REAL NOT NULL DEFAULT 0.0,
                        created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (project_id) REFERENCES projects (id)
                    )";

				using(var command = new SQLiteCommand(createClipsTable, connection))
				{
					command.ExecuteNonQuery();
				}

				// 创建 clip_effects 表
				string createClipEffectsTable = @"
                    CREATE TABLE IF NOT EXISTS clip_effects (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        clip_id INTEGER NOT NULL,
                        effect_type TEXT NOT NULL,
                        value REAL NOT NULL,
                        created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (clip_id) REFERENCES clips (id)
                    )";

				using(var command = new SQLiteCommand(createClipEffectsTable, connection))
				{
					command.ExecuteNonQuery();
				}
			}
		}

		// Projects 表操作
		public int InsertProject(Project project)
		{
			using(var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				string sql = @"
                    INSERT INTO projects (name, description, created_at, updated_at)
                    VALUES (@name, @description, @created_at, @updated_at);
                    SELECT last_insert_rowid();";

				using(var command = new SQLiteCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@name", project.Name);
					command.Parameters.AddWithValue("@description", project.Description ?? "");
					command.Parameters.AddWithValue("@created_at", project.CreatedAt);
					command.Parameters.AddWithValue("@updated_at", project.UpdatedAt);

					return Convert.ToInt32(command.ExecuteScalar());
				}
			}
		}

		public Project GetProjectById(int id)
		{
			using(var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				string sql = "SELECT * FROM projects WHERE id = @id";

				using(var command = new SQLiteCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@id", id);

					using(var reader = command.ExecuteReader())
					{
						if(reader.Read())
						{
							return new Project
							{
								Id = Convert.ToInt32(reader["id"]),
								Name = reader["name"].ToString(),
								Description = reader["description"].ToString(),
								CreatedAt = Convert.ToDateTime(reader["created_at"]),
								UpdatedAt = Convert.ToDateTime(reader["updated_at"])
							};
						}
					}
				}
			}

			return null;
		}

		public List<Project> GetAllProjects()
		{
			var projects = new List<Project>();

			using(var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				string sql = "SELECT * FROM projects ORDER BY created_at DESC";

				using(var command = new SQLiteCommand(sql, connection))
				using(var reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						projects.Add(new Project
						{
							Id = Convert.ToInt32(reader["id"]),
							Name = reader["name"].ToString(),
							Description = reader["description"].ToString(),
							CreatedAt = Convert.ToDateTime(reader["created_at"]),
							UpdatedAt = Convert.ToDateTime(reader["updated_at"])
						});
					}
				}
			}

			return projects;
		}

		public bool UpdateProject(Project project)
		{
			using(var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				string sql = @"
                    UPDATE projects 
                    SET name = @name, description = @description, updated_at = @updated_at
                    WHERE id = @id";

				using(var command = new SQLiteCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@name", project.Name);
					command.Parameters.AddWithValue("@description", project.Description ?? "");
					command.Parameters.AddWithValue("@updated_at", DateTime.Now);
					command.Parameters.AddWithValue("@id", project.Id);

					return command.ExecuteNonQuery() > 0;
				}
			}
		}

		public bool DeleteProject(int id)
		{
			using(var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				string sql = "DELETE FROM projects WHERE id = @id";

				using(var command = new SQLiteCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@id", id);
					return command.ExecuteNonQuery() > 0;
				}
			}
		}

		// Clips 表操作
		public int InsertClip(Clip clip)
		{
			using(var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				string sql = @"
                    INSERT INTO clips (name, file_path, project_id, start_position, end_position, speed, pitch, created_at, updated_at)
                    VALUES (@name, @file_path, @project_id, @start_position, @end_position, @speed, @pitch, @created_at, @updated_at);
                    SELECT last_insert_rowid();";

				using(var command = new SQLiteCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@name", clip.Name);
					command.Parameters.AddWithValue("@file_path", clip.FilePath);
					command.Parameters.AddWithValue("@project_id", clip.ProjectId);
					command.Parameters.AddWithValue("@start_position", clip.StartPosition);
					command.Parameters.AddWithValue("@end_position", clip.EndPosition);
					command.Parameters.AddWithValue("@speed", clip.Speed);
					command.Parameters.AddWithValue("@pitch", clip.Pitch);
					command.Parameters.AddWithValue("@created_at", clip.CreatedAt);
					command.Parameters.AddWithValue("@updated_at", clip.UpdatedAt);

					return Convert.ToInt32(command.ExecuteScalar());
				}
			}
		}

		public Clip GetClipById(int id)
		{
			using(var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				string sql = "SELECT * FROM clips WHERE id = @id";

				using(var command = new SQLiteCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@id", id);

					using(var reader = command.ExecuteReader())
					{
						if(reader.Read())
						{
							return new Clip
							{
								Id = Convert.ToInt32(reader["id"]),
								Name = reader["name"].ToString(),
								FilePath = reader["file_path"].ToString(),
								ProjectId = Convert.ToInt32(reader["project_id"]),
								StartPosition = Convert.ToDouble(reader["start_position"]),
								EndPosition = Convert.ToDouble(reader["end_position"]),
								Speed = Convert.ToDouble(reader["speed"]),
								Pitch = Convert.ToDouble(reader["pitch"]),
								CreatedAt = Convert.ToDateTime(reader["created_at"]),
								UpdatedAt = Convert.ToDateTime(reader["updated_at"])
							};
						}
					}
				}
			}

			return null;
		}

		public List<Clip> GetClipsByProjectId(int projectId)
		{
			var clips = new List<Clip>();

			using(var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				string sql = "SELECT * FROM clips WHERE project_id = @project_id ORDER BY created_at DESC";

				using(var command = new SQLiteCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@project_id", projectId);

					using(var reader = command.ExecuteReader())
					{
						while(reader.Read())
						{
							clips.Add(new Clip
							{
								Id = Convert.ToInt32(reader["id"]),
								Name = reader["name"].ToString(),
								FilePath = reader["file_path"].ToString(),
								ProjectId = Convert.ToInt32(reader["project_id"]),
								StartPosition = Convert.ToDouble(reader["start_position"]),
								EndPosition = Convert.ToDouble(reader["end_position"]),
								Speed = Convert.ToDouble(reader["speed"]),
								Pitch = Convert.ToDouble(reader["pitch"]),
								CreatedAt = Convert.ToDateTime(reader["created_at"]),
								UpdatedAt = Convert.ToDateTime(reader["updated_at"])
							});
						}
					}
				}
			}

			return clips;
		}

		public List<Clip> GetAllClips()
		{
			var clips = new List<Clip>();

			using(var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				string sql = "SELECT * FROM clips ORDER BY created_at DESC";

				using(var command = new SQLiteCommand(sql, connection))
				using(var reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						clips.Add(new Clip
						{
							Id = Convert.ToInt32(reader["id"]),
							Name = reader["name"].ToString(),
							FilePath = reader["file_path"].ToString(),
							ProjectId = Convert.ToInt32(reader["project_id"]),
							StartPosition = Convert.ToDouble(reader["start_position"]),
							EndPosition = Convert.ToDouble(reader["end_position"]),
							Speed = Convert.ToDouble(reader["speed"]),
							Pitch = Convert.ToDouble(reader["pitch"]),
							CreatedAt = Convert.ToDateTime(reader["created_at"]),
							UpdatedAt = Convert.ToDateTime(reader["updated_at"])
						});
					}
				}
			}

			return clips;
		}

		public bool UpdateClip(Clip clip)
		{
			using(var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				string sql = @"
                    UPDATE clips 
                    SET name = @name, file_path = @file_path, project_id = @project_id, 
                        start_position = @start_position, end_position = @end_position,
                        speed = @speed, pitch = @pitch, updated_at = @updated_at
                    WHERE id = @id";

				using(var command = new SQLiteCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@name", clip.Name);
					command.Parameters.AddWithValue("@file_path", clip.FilePath);
					command.Parameters.AddWithValue("@project_id", clip.ProjectId);
					command.Parameters.AddWithValue("@start_position", clip.StartPosition);
					command.Parameters.AddWithValue("@end_position", clip.EndPosition);
					command.Parameters.AddWithValue("@speed", clip.Speed);
					command.Parameters.AddWithValue("@pitch", clip.Pitch);
					command.Parameters.AddWithValue("@updated_at", DateTime.Now);
					command.Parameters.AddWithValue("@id", clip.Id);

					return command.ExecuteNonQuery() > 0;
				}
			}
		}

		public bool DeleteClip(int id)
		{
			using(var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				// 先删除相关的clip_effects
				string deleteEffectsSql = "DELETE FROM clip_effects WHERE clip_id = @clip_id";
				using(var command = new SQLiteCommand(deleteEffectsSql, connection))
				{
					command.Parameters.AddWithValue("@clip_id", id);
					command.ExecuteNonQuery();
				}

				// 再删除clip
				string deleteClipSql = "DELETE FROM clips WHERE id = @id";
				using(var command = new SQLiteCommand(deleteClipSql, connection))
				{
					command.Parameters.AddWithValue("@id", id);
					return command.ExecuteNonQuery() > 0;
				}
			}
		}

		// ClipEffects 表操作
		public int InsertClipEffect(ClipEffect effect)
		{
			using(var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				string sql = @"
                    INSERT INTO clip_effects (clip_id, effect_type, value, created_at)
                    VALUES (@clip_id, @effect_type, @value, @created_at);
                    SELECT last_insert_rowid();";

				using(var command = new SQLiteCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@clip_id", effect.ClipId);
					command.Parameters.AddWithValue("@effect_type", effect.EffectType);
					command.Parameters.AddWithValue("@value", effect.Value);
					command.Parameters.AddWithValue("@created_at", effect.CreatedAt);

					return Convert.ToInt32(command.ExecuteScalar());
				}
			}
		}

		public ClipEffect GetClipEffectById(int id)
		{
			using(var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				string sql = "SELECT * FROM clip_effects WHERE id = @id";

				using(var command = new SQLiteCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@id", id);

					using(var reader = command.ExecuteReader())
					{
						if(reader.Read())
						{
							return new ClipEffect
							{
								Id = Convert.ToInt32(reader["id"]),
								ClipId = Convert.ToInt32(reader["clip_id"]),
								EffectType = reader["effect_type"].ToString(),
								Value = Convert.ToDouble(reader["value"]),
								CreatedAt = Convert.ToDateTime(reader["created_at"])
							};
						}
					}
				}
			}

			return null;
		}

		public List<ClipEffect> GetClipEffectsByClipId(int clipId)
		{
			var effects = new List<ClipEffect>();

			using(var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				string sql = "SELECT * FROM clip_effects WHERE clip_id = @clip_id ORDER BY created_at DESC";

				using(var command = new SQLiteCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@clip_id", clipId);

					using(var reader = command.ExecuteReader())
					{
						while(reader.Read())
						{
							effects.Add(new ClipEffect
							{
								Id = Convert.ToInt32(reader["id"]),
								ClipId = Convert.ToInt32(reader["clip_id"]),
								EffectType = reader["effect_type"].ToString(),
								Value = Convert.ToDouble(reader["value"]),
								CreatedAt = Convert.ToDateTime(reader["created_at"])
							});
						}
					}
				}
			}

			return effects;
		}

		public bool UpdateClipEffect(ClipEffect effect)
		{
			using(var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				string sql = @"
                    UPDATE clip_effects 
                    SET clip_id = @clip_id, effect_type = @effect_type, value = @value
                    WHERE id = @id";

				using(var command = new SQLiteCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@clip_id", effect.ClipId);
					command.Parameters.AddWithValue("@effect_type", effect.EffectType);
					command.Parameters.AddWithValue("@value", effect.Value);
					command.Parameters.AddWithValue("@id", effect.Id);

					return command.ExecuteNonQuery() > 0;
				}
			}
		}

		public bool DeleteClipEffect(int id)
		{
			using(var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();

				string sql = "DELETE FROM clip_effects WHERE id = @id";

				using(var command = new SQLiteCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@id", id);
					return command.ExecuteNonQuery() > 0;
				}
			}
		}
	}
}


#endregion

#region --------- 数据模型类（对应数据库表结构） ------------
public class User
{
	public int Id
	{
		get; set;
		}
	public string Name
	{
		get; set;

	}
	public int Age
	{
		get; set;
	}
	public string Address
	{
		get; set;
	}
}

public class Project
{
	public int Id
	{
		get; set;
	}
	public string Name
	{
		get; set;
	}
	public string Description
	{
		get; set;
	}
	public DateTime CreatedAt
	{
		get; set;
	}
	public DateTime UpdatedAt
	{
		get; set;
	}
}

public class Clip
{
	public int Id
	{
		get; set;
	}
	public string Name
	{
		get; set;
	}
	public string FilePath
	{
		get; set;
	}
	public int ProjectId
	{
		get; set;
	}
	public double StartPosition
	{
		get; set;
	}
	public double EndPosition
	{
		get; set;
	}
	public double Speed
	{
		get; set;
	}
	public double Pitch
	{
		get; set;
	}
	public DateTime CreatedAt
	{
		get; set;
	}
	public DateTime UpdatedAt
	{
		get; set;
	}
}

public class ClipEffect
{
	public int Id
	{
		get; set;
	}
	public int ClipId
	{
		get; set;
	}
	public string EffectType
	{
		get; set;
	}
	public double Value
	{
		get; set;
	}
	public DateTime CreatedAt
	{
		get; set;
	}
}
#endregion




