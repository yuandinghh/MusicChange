using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace MusicChange
{
	// 数据库操作封装类
#pragma warning disable IDE1006 // 命名样式
	public class db
#pragma warning restore IDE1006 // 命名样式
	{   // 数据库连接字符串（建议使用绝对路径避免路径问题）
		public static string dbPath = "D:\\Documents\\Visual Studio 2022\\MusicChange\\LaserEditing.db";
		public static string _connectionString;
		public db(string dbPath)
		{
			_connectionString = $"Data Source={dbPath};Version=3;";
		}


		// 1. 初始化数据库（创建表）
		public void InitDatabase( )
		{
			//先判断数据库是否存在，如果最存在，看看是否已经有相同的表
			if (!System.IO.File.Exists( _connectionString.Split( '=' )[1].Split( ';' )[0] )) {
				MessageBox.Show( "数据库文件不存在，正在创建..." );
			}
			else {
				MessageBox.Show( "数据库文件已存在，正在检查表结构..." );
				using (var connection = new SQLiteConnection( _connectionString )) {
					connection.Open();
					string checkTableSql = "SELECT name FROM sqlite_master WHERE type='table' AND name='Users'";
					using (var command = new SQLiteCommand( checkTableSql, connection )) {
						var result = command.ExecuteScalar();
						if (result != null) {
							MessageBox.Show( "表已存在，无需创建。" );
							return; // 表已存在，直接返回
						}
					}
				}
			}
			try {
				using (var connection = new SQLiteConnection( _connectionString )) {
					connection.Open();
					string createTableSql = @"
                        CREATE TABLE IF NOT EXISTS Users (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT NOT NULL,
                            Age INTEGER CHECK(Age > 0),  -- 年龄必须为正数
                            Address TEXT
                        )";
					using (var command = new SQLiteCommand( createTableSql, connection )) {
						command.ExecuteNonQuery();
					}

					MessageBox.Show( "数据库初始化成功" );
				}
			}
			catch (Exception ex) {
				MessageBox.Show( $"初始化失败：{ex.Message}" );
			}
		}

		#region --------- 应该不用了  创建一个 clip 剪辑数据库的详细表，并给出c# sqlite 程序  ------------

		/*
        1.	clips - 存储音频剪辑的基本信息
		2.	projects - 存储项目信息
		3.	clip_effects - 存储剪辑效果设置

        1.	创建(Create): InsertProject, InsertClip, InsertClipEffect
        2.	读取(Read): GetProjectById, GetAllProjects, GetClipById, GetClipsByProjectId, GetAllClips, GetClipEffectById, GetClipEffectsByClipId
        3.	更新(Update): UpdateProject, UpdateClip, UpdateClipEffect
        4.	删除(Delete): DeleteProject, DeleteClip, DeleteClipEffect
        */
	
		/// <summary>
		///  Projects 表操作
		/// </summary>
		/// <param name="project"></param>
		/// <returns></returns>
		public int InsertProject(Project project)
		{
#pragma warning disable IDE0063 // 使用简单的 "using" 语句
			using (var connection = new SQLiteConnection( _connectionString )) {
#pragma warning restore IDE0063 // 使用简单的 "using" 语句
				connection.Open();
				string sql = @"
                    INSERT INTO projects (name, description, created_at, updated_at)
                    VALUES (@name, @description, @created_at, @updated_at);
                    SELECT last_insert_rowid();";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@name", project.Name );
					command.Parameters.AddWithValue( "@description", project.Description ?? "" );
					command.Parameters.AddWithValue( "@created_at", project.CreatedAt );
					command.Parameters.AddWithValue( "@updated_at", project.UpdatedAt );

					return Convert.ToInt32( command.ExecuteScalar() );
				}
			}
		}
		/// <summary>
		/// Retrieves a project by its unique identifier.
		/// </summary>
		/// <remarks>This method queries the database to find a project with the specified ID.  If no matching project
		/// is found, the method returns <see langword="null"/>.</remarks>
		/// <param name="id">The unique identifier of the project to retrieve. Must be a positive integer.</param>
		/// <returns>The <see cref="Project"/> object corresponding to the specified <paramref name="id"/>,  or <see langword="null"/>
		/// if no project with the given ID exists.</returns>
		public Project GetProjectById(int id)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();
				string sql = "SELECT * FROM projects WHERE id = @id";
				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );
					using (var reader = command.ExecuteReader()) {
						if (reader.Read()) {
							return new Project
							{
								Id = Convert.ToInt32( reader["id"] ),
								Name = reader["name"].ToString(),
								Description = reader["description"].ToString(),
								CreatedAt = Convert.ToDateTime( reader["created_at"] ),
								UpdatedAt = Convert.ToDateTime( reader["updated_at"] )
							};
						}
					}
				}
			}
			return null;
		}
		/// <summary>
		/// 获取所有Project项目列表
		/// </summary>
		/// <returns></returns>
		public List<Project> GetAllProjects( )
		{
			var projects = new List<Project>();
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();
				string sql = "SELECT * FROM projects ORDER BY created_at DESC";
				using (var command = new SQLiteCommand( sql, connection ))
				using (var reader = command.ExecuteReader()) {
					while (reader.Read()) {
						projects.Add( new Project
						{
							Id = Convert.ToInt32( reader["id"] ),
							Name = reader["name"].ToString(),
							Description = reader["description"].ToString(),
							CreatedAt = Convert.ToDateTime( reader["created_at"] ),
							UpdatedAt = Convert.ToDateTime( reader["updated_at"] )
						} );
					}
				}
			}
			return projects;
		}
		/// <summary>
		/// Updates an existing project in the database.
		/// <param name="project"></param>
		/// </summary>
		public bool UpdateProject(Project project)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    UPDATE projects 
                    SET name = @name, description = @description, updated_at = @updated_at
                    WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@name", project.Name );
					command.Parameters.AddWithValue( "@description", project.Description ?? "" );
					command.Parameters.AddWithValue( "@updated_at", DateTime.Now );
					command.Parameters.AddWithValue( "@id", project.Id );

					return command.ExecuteNonQuery() > 0;
				}
			}
		}
		/// <summary>
		/// Deletes a project from the database by its unique identifier.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>

		public bool DeleteProject(int id)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();
				string sql = "DELETE FROM projects WHERE id = @id";
				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );
					return command.ExecuteNonQuery() > 0;
				}
			}
		}
		/// <summary>
		///  Clips 表插入操作
		/// </summary>
		/// <param name="clip"></param>
		/// <returns></returns>
		public int InsertClip(Clipold clip)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    INSERT INTO clips (name, type,file_path, project_id, start_position, end_position, speed, pitch, created_at, updated_at)
                    VALUES (@name, @file_path, @project_id, @start_position, @end_position, @speed, @pitch, @created_at, @updated_at);
                    SELECT last_insert_rowid();";
				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@name", clip.Name );
					command.Parameters.AddWithValue( "@file_path", clip.FilePath );
					command.Parameters.AddWithValue( "@project_id", clip.ProjectId );
					command.Parameters.AddWithValue( "@start_position", clip.StartPosition );
					command.Parameters.AddWithValue( "@end_position", clip.EndPosition );
					command.Parameters.AddWithValue( "@speed", clip.Speed );
					command.Parameters.AddWithValue( "@pitch", clip.Pitch );
					command.Parameters.AddWithValue( "@created_at", clip.CreatedAt );
					command.Parameters.AddWithValue( "@updated_at", clip.UpdatedAt );

					return Convert.ToInt32( command.ExecuteScalar() );
				}
			}
		}
		/// <summary>
		/// 获取 Clip by Id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Clipold GetClipById(int id)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();
				string sql = "SELECT * FROM clips WHERE id = @id";
				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );
					using (var reader = command.ExecuteReader()) {
						if (reader.Read()) {
							return new Clipold
							{
								Id = Convert.ToInt32( reader["id"] ),
								Name = reader["name"].ToString(),
								Type = reader["type"].ToString(),
								FilePath = reader["file_path"].ToString(),
								ProjectId = Convert.ToInt32( reader["project_id"] ),
								StartPosition = Convert.ToDouble( reader["start_position"] ),
								EndPosition = Convert.ToDouble( reader["end_position"] ),
								Speed = Convert.ToDouble( reader["speed"] ),
								Pitch = Convert.ToDouble( reader["pitch"] ),
								CreatedAt = Convert.ToDateTime( reader["created_at"] ),
								UpdatedAt = Convert.ToDateTime( reader["updated_at"] )
							};
						}
					}
				}
			}
			return null;
		}
		/// <summary>
		/// 存储剪辑效果设置 id
		/// </summary>
		/// <param name="projectId"></param>
		/// <returns></returns>
		public List<Clipold> GetClipsByProjectId(int projectId)
		{
			var clips = new List<Clipold>();
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();
				string sql = "SELECT * FROM clips WHERE project_id = @project_id ORDER BY created_at DESC";
				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@project_id", projectId );
					using (var reader = command.ExecuteReader()) {
						while (reader.Read()) {
							clips.Add( new Clipold
							{
								Id = Convert.ToInt32( reader["id"] ),
								Name = reader["name"].ToString(),
								Type = reader["type"].ToString(),
								FilePath = reader["file_path"].ToString(),
								ProjectId = Convert.ToInt32( reader["project_id"] ),
								StartPosition = Convert.ToDouble( reader["start_position"] ),
								EndPosition = Convert.ToDouble( reader["end_position"] ),
								Speed = Convert.ToDouble( reader["speed"] ),
								Pitch = Convert.ToDouble( reader["pitch"] ),
								CreatedAt = Convert.ToDateTime( reader["created_at"] ),
								UpdatedAt = Convert.ToDateTime( reader["updated_at"] )
							} );

						}
					}
				}
			}

			return clips;
		}
		/// <summary>
		/// 获取所有 Clips 列表
		/// </summary>
		/// <returns></returns>
		public List<Clipold> GetAllClips( )
		{
			var clips = new List<Clipold>();

			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();
				string sql = "SELECT * FROM clips ORDER BY created_at DESC";
				using (var command = new SQLiteCommand( sql, connection ))
				using (var reader = command.ExecuteReader()) {
					while (reader.Read()) {
						clips.Add( new Clipold
						{
							Id = Convert.ToInt32( reader["id"] ),
							Name = reader["name"].ToString(),
							Type = reader["name"].ToString(),
							FilePath = reader["file_path"].ToString(),
							ProjectId = Convert.ToInt32( reader["project_id"] ),
							StartPosition = Convert.ToDouble( reader["start_position"] ),
							EndPosition = Convert.ToDouble( reader["end_position"] ),
							Speed = Convert.ToDouble( reader["speed"] ),
							Pitch = Convert.ToDouble( reader["pitch"] ),
							CreatedAt = Convert.ToDateTime( reader["created_at"] ),
							UpdatedAt = Convert.ToDateTime( reader["updated_at"] )
						} );
					}
				}
			}
			return clips;
		}
		/// <summary>
		/// 更新 Clip 信息
		/// </summary>
		/// <param name="clip"></param>
		/// <returns></returns>
		public bool UpdateClip(Clipold clip)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    UPDATE clips 
                    SET name = @name,Type = @type,file_path = @file_path, project_id = @project_id, 
                        start_position = @start_position, end_position = @end_position,
                        speed = @speed, pitch = @pitch, updated_at = @updated_at
                    WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@name", clip.Name );
					command.Parameters.AddWithValue( "@type", clip.Type );
					command.Parameters.AddWithValue( "@file_path", clip.FilePath );
					command.Parameters.AddWithValue( "@project_id", clip.ProjectId );
					command.Parameters.AddWithValue( "@start_position", clip.StartPosition );
					command.Parameters.AddWithValue( "@end_position", clip.EndPosition );
					command.Parameters.AddWithValue( "@speed", clip.Speed );
					command.Parameters.AddWithValue( "@pitch", clip.Pitch );
					command.Parameters.AddWithValue( "@updated_at", DateTime.Now );
					command.Parameters.AddWithValue( "@id", clip.Id );

					return command.ExecuteNonQuery() > 0;
				}
			}
		}
		/// <summary>
		/// 删除 Clip
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool DeleteClip(int id)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				// 先删除相关的clip_effects
				string deleteEffectsSql = "DELETE FROM clip_effects WHERE clip_id = @clip_id";
				using (var command = new SQLiteCommand( deleteEffectsSql, connection )) {
					command.Parameters.AddWithValue( "@clip_id", id );
					command.ExecuteNonQuery();
				}

				// 再删除clip
				string deleteClipSql = "DELETE FROM clips WHERE id = @id";
				using (var command = new SQLiteCommand( deleteClipSql, connection )) {
					command.Parameters.AddWithValue( "@id", id );
					return command.ExecuteNonQuery() > 0;
				}
			}
		}
		/// <summary>
		///  ClipEffects 表操作 存储剪辑效果设置
		/// </summary>
		/// <param name="effect"></param>
		/// <returns></returns>
		// ClipEffects 表操作
		public int InsertClipEffect(ClipEffectold effect)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();
				string sql = @"
                    INSERT INTO clip_effects (clip_id, effect_type, value, created_at)
                    VALUES (@clip_id, @effect_type, @value, @created_at);
                    SELECT last_insert_rowid();";
				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@clip_id", effect.ClipId );
					command.Parameters.AddWithValue( "@effect_type", effect.EffectType );
					command.Parameters.AddWithValue( "@value", effect.Value );
					command.Parameters.AddWithValue( "@created_at", effect.CreatedAt );

					return Convert.ToInt32( command.ExecuteScalar() );
				}
			}
		}
		/// <summary>
		///获取 ClipEffect by Id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ClipEffectold GetClipEffectById(int id)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = "SELECT * FROM clip_effects WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );

					using (var reader = command.ExecuteReader()) {
						if (reader.Read()) {
							return new ClipEffectold
							{
								Id = Convert.ToInt32( reader["id"] ),
								ClipId = Convert.ToInt32( reader["clip_id"] ),
								EffectType = reader["effect_type"].ToString(),
								Value = Convert.ToDouble( reader["value"] ),
								CreatedAt = Convert.ToDateTime( reader["created_at"] )
							};
						}
					}
				}
			}

			return null;
		}

		public List<ClipEffectold> GetClipEffectsByClipId(int clipId)
		{
			var effects = new List<ClipEffectold>();

			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = "SELECT * FROM clip_effects WHERE clip_id = @clip_id ORDER BY created_at DESC";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@clip_id", clipId );

					using (var reader = command.ExecuteReader()) {
						while (reader.Read()) {
							effects.Add( new ClipEffectold
							{
								Id = Convert.ToInt32( reader["id"] ),
								ClipId = Convert.ToInt32( reader["clip_id"] ),
								EffectType = reader["effect_type"].ToString(),
								Value = Convert.ToDouble( reader["value"] ),
								CreatedAt = Convert.ToDateTime( reader["created_at"] )
							} );

						}
					}
				}
			}
			return effects;
		}
		/// <summary>
		///	更新 ClipEffect
		/// </summary>
		/// <param name="effect"></param>
		/// <returns></returns>
		public bool UpdateClipEffect(ClipEffectold effect)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    UPDATE clip_effects 
                    SET clip_id = @clip_id, effect_type = @effect_type, value = @value
                    WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@clip_id", effect.ClipId );
					command.Parameters.AddWithValue( "@effect_type", effect.EffectType );
					command.Parameters.AddWithValue( "@value", effect.Value );
					command.Parameters.AddWithValue( "@id", effect.Id );

					return command.ExecuteNonQuery() > 0;
				}
			}
		}

		public bool DeleteClipEffect(int id)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = "DELETE FROM clip_effects WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );
					return command.ExecuteNonQuery() > 0;
				}
			}
		}

		#endregion

		#region -------剪辑   综合  User 表 处理  ------------

		/* 
		数据库表关系图

	这个设计提供了完整的用户管理功能，包括：
	1.	基础用户信息 (users) - 存储用户的基本账户信息
	2.	用户配置文件 (user_profiles) - 存储用户的个性化设置
	3.	用户会话 (user_sessions) - 管理用户登录会话
	4.	用户偏好 (user_preferences) - 存储用户的详细偏好设置
	5.	 (user_logintime)   - 记录用户登录时间
	特点：
	•	使用外键约束保证数据完整性
	•	创建索引提高查询性能
	•	使用触发器自动更新时间戳
	•	支持级联删除，当用户被删除时相关数据也会被自动清理
	•	提供默认数据插入功能
		数据库表结构设计
	-----------------------------------------------
	1. users 表 - 用户信息表
	列名		类型	约束	描述
	id			INTEGER	PRIMARY KEY AUTOINCREMENT	用户ID
	username	TEXT	NOT NULL UNIQUE	用户名
	email		TEXT	NOT NULL UNIQUE	邮箱
	password_hash	TEXT	NOT NULL	密码哈希值
		   iphone TEXT,	NOT NULL UNIQUE	手机号码
	full_name	TEXT					真实姓名
		 
	avatar_path	TEXT					头像路径
	is_active	INTEGER	NOT NULL DEFAULT 1	是否激活
	created_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
	updated_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间
    draftposition TEXT,            职位草案           
    is_locked INTEGER NOT NULL DEFAULT 0,
    is_deleted INTEGER NOT NULL DEFAULT 0,
    is_modified INTEGER NOT NULL DEFAULT 0,
    note TEXT


		2. user_profiles 表 - 用户详细信息表
	列名	类型	约束	描述
	id	INTEGER	PRIMARY KEY AUTOINCREMENT	配置ID
	user_id	INTEGER	NOT NULL UNIQUE, FOREIGN KEY REFERENCES users(id)	用户ID
	preferred_language	TEXT		偏好语言
	theme	TEXT		主题设置
	default_project_location	TEXT		默认项目保存位置
	auto_save_interval	INTEGER		自动保存间隔（分钟）
	max_undo_steps	INTEGER		最大撤销步数
	created_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
	updated_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间

	3. user_sessions 表 - 用户会话表
	列名	类型	约束	描述
	id	INTEGER	PRIMARY KEY AUTOINCREMENT	会话ID
	user_id	INTEGER	NOT NULL, FOREIGN KEY REFERENCES users(id)	用户ID
	session_token	TEXT	NOT NULL UNIQUE	会话令牌
	ip_address	TEXT		IP地址
	user_agent	TEXT		用户代理
	expires_at	DATETIME	NOT NULL	过期时间
	created_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间

	4. user_preferences 表 - 用户偏好设置表
	列名	类型	约束	描述
	id	INTEGER	PRIMARY KEY AUTOINCREMENT	偏好ID
	user_id	INTEGER	NOT NULL, FOREIGN KEY REFERENCES users(id)	用户ID
	preference_key	TEXT	NOT NULL	偏好键名
	preference_value	TEXT	NOT NULL	偏好值
	created_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
	updated_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间

	5. userslogintime 表 - 用户登录时间表
	列名	类型	约束	描述
	id	INTEGER	PRIMARY KEY AUTOINCREMENT	用户ID
	iduser	INTEGER	NOT NULL UNIQUE, FOREIGN KEY REFERENCES users(id)	用户ID
		login_time	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	登录时间
		end_time	DATETIME		登出时间
		created_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
		count 	INTEGER	NOT NULL DEFAULT 1	登录次数
		timelength	INTEGER	NOT NULL DEFAULT 0	在线时长（秒）

		*/

		/// <summary>
		/// 用户数据库表设置
		/// </summary>
		//public class UserDatabaseSetup
		//{
		//private readonly string _connectionString;
		//private string databasePath = "D:\\Documents\\Visual Studio 2022\\MusicChange\\LaserEditing.db";
		////public UserDatabaseSetup(string databasePath)
		////{
		//	_connectionString = $"Data Source={databasePath};Version=3;";
		//}

		/// <summary>
		/// testuser 测试用户数据库初始化
		/// </summary>
		public void testuser( )
		{
			try {
				// 初始化用户数据库				
				var dbAccess = new db( dbPath );
				// 创建表结构
				dbAccess.CreateDatabaseTables();
				// 创建触发器
				dbAccess.CreateTriggers();
				// 插入默认数据
				dbAccess.InsertDefaultData();
				Console.WriteLine( "用户数据库初始化完成！" );
			}
			catch (Exception ex) {
				Console.WriteLine( $"数据库初始化失败: {ex.Message}" );
			}
		}
		/// <summary>
		/// 创建数据库表
		/// </summary>
		public void CreateDatabaseTables( )
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				//CreateUserTable( connection );
				CreateUserProfileTable( connection );
				CreateUserSessionTable( connection );
				CreateUserPreferencesTable( connection );
				CreateUserLoginTable( connection );

				Console.WriteLine( "用户数据库表创建完成" );
			}
		}

		//private void CreateUserTable(SQLiteConnection connection)
		//{
		//	string createUsersTable = @"
  //              CREATE TABLE IF NOT EXISTS users (
  //                  id INTEGER PRIMARY KEY AUTOINCREMENT,
  //                  username TEXT NOT NULL UNIQUE,
  //                  email TEXT NOT NULL UNIQUE,
  //                  password_hash TEXT NOT NULL,
  //                  full_name TEXT,
  //                  avatar_path TEXT,
  //                  is_active INTEGER NOT NULL DEFAULT 1,
  //                  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  //                  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
		//			annotations TEXT
  //              )";

		//	using (var command = new SQLiteCommand( createUsersTable, connection )) {
		//		command.ExecuteNonQuery();
		//	}

		//	// 创建用户名索引
		//	string createUsernameIndex = @"CREATE INDEX IF NOT EXISTS idx_users_username ON users (username)";

		//	using (var command = new SQLiteCommand( createUsernameIndex, connection )) {
		//		command.ExecuteNonQuery();
		//	}

		//	// 创建邮箱索引
		//	string createEmailIndex = @"
  //              CREATE INDEX IF NOT EXISTS idx_users_email 
  //              ON users (email)";

		//	using (var command = new SQLiteCommand( createEmailIndex, connection )) {
		//		command.ExecuteNonQuery();
		//	}
		//}

		private void CreateUserProfileTable(SQLiteConnection connection)
		{
			string createProfilesTable = @"
                CREATE TABLE IF NOT EXISTS user_profiles (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    user_id INTEGER NOT NULL UNIQUE,
                    preferred_language TEXT,
                    theme TEXT,
                    default_project_location TEXT,
                    auto_save_interval INTEGER,
                    max_undo_steps INTEGER,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
                )";

			using (var command = new SQLiteCommand( createProfilesTable, connection )) {
				command.ExecuteNonQuery();
			}
		}

		private void CreateUserSessionTable(SQLiteConnection connection)
		{
			string createSessionsTable = @"
                CREATE TABLE IF NOT EXISTS user_sessions (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    user_id INTEGER NOT NULL,
                    session_token TEXT NOT NULL UNIQUE,
                    ip_address TEXT,
                    user_agent TEXT,
                    expires_at DATETIME NOT NULL,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
                )";

			using (var command = new SQLiteCommand( createSessionsTable, connection )) {
				command.ExecuteNonQuery();
			}

			// 创建会话令牌索引
			string createSessionTokenIndex = @"
                CREATE INDEX IF NOT EXISTS idx_sessions_token 
                ON user_sessions (session_token)";

			using (var command = new SQLiteCommand( createSessionTokenIndex, connection )) {
				command.ExecuteNonQuery();
			}

			// 创建过期时间索引
			string createExpiresAtIndex = @"
                CREATE INDEX IF NOT EXISTS idx_sessions_expires 
                ON user_sessions (expires_at)";

			using (var command = new SQLiteCommand( createExpiresAtIndex, connection )) {
				command.ExecuteNonQuery();
			}
		}

		//private void CreateUserPreferencesTable(SQLiteConnection connection)
		//{
		//	string createPreferencesTable = @"
		//              CREATE TABLE IF NOT EXISTS user_preferences (
		//                  id INTEGER PRIMARY KEY AUTOINCREMENT,
		//                  user_id INTEGER NOT NULL,
		//                  preference_key TEXT NOT NULL,
		//                  preference_value TEXT NOT NULL,
		//                  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
		//                  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
		//                  FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
		//              )";

		//	using (var command = new SQLiteCommand( createPreferencesTable, connection )) {
		//		command.ExecuteNonQuery();
		//	}

		//	// 创建用户偏好组合索引
		//	string createUserPrefIndex = @"
		//              CREATE INDEX IF NOT EXISTS idx_user_preferences 
		//              ON user_preferences (user_id, preference_key)";

		//	using (var command = new SQLiteCommand( createUserPrefIndex, connection )) {
		//		command.ExecuteNonQuery();
		//	}
		//}

		private void CreateUserLoginTable(SQLiteConnection connection)
		{
			string createPreferencesTable = @"
                CREATE TABLE IF NOT EXISTS user_logintime (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    user_id INTEGER NOT NULL,
                    preference_key TEXT NOT NULL,
                    preference_value TEXT NOT NULL,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
                )";

			using (var command = new SQLiteCommand( createPreferencesTable, connection )) {
				command.ExecuteNonQuery();
			}

			// 创建用户偏好组合索引
			//string createUserPrefIndex = @"
			//             CREATE INDEX IF NOT EXISTS idx_user_preferences 
			//             ON user_preferences (user_id, preference_key)";
			//using (var command = new SQLiteCommand( createUserPrefIndex, connection )) {
			//	command.ExecuteNonQuery();
		}


		/// <summary>
		/// InsertDefaultData 插入默认数据
		/// </summary>
		public void InsertDefaultData( )
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				// 插入默认用户
				string insertDefaultUser = @"
                    INSERT OR IGNORE INTO users (username, email, password_hash, full_name)
                    VALUES (@username, @email, @password_hash, @full_name)";

				using (var command = new SQLiteCommand( insertDefaultUser, connection )) {
					command.Parameters.AddWithValue( "@username", "admin" );
					command.Parameters.AddWithValue( "@email", "admin@example.com" );
					command.Parameters.AddWithValue( "@password_hash", " hashed_password_here " );
					command.Parameters.AddWithValue( "@full_name", "系统管理员" );
					command.ExecuteNonQuery();
				}

				// 获取默认用户ID
				int userId = 0;
				string getUserId = "SELECT id FROM users WHERE username = 'admin'";
				using (var command = new SQLiteCommand( getUserId, connection )) {
					var result = command.ExecuteScalar();
					if (result != null) {
						userId = Convert.ToInt32( result );
					}
				}

				if (userId > 0) {
					// 插入默认用户配置
					string insertDefaultProfile = @"
                        INSERT OR IGNORE INTO user_profiles 
                        (user_id, preferred_language, theme, auto_save_interval, max_undo_steps)
                        VALUES (@user_id, @language, @theme, @auto_save, @undo_steps)";

					using (var command = new SQLiteCommand( insertDefaultProfile, connection )) {
						command.Parameters.AddWithValue( "@user_id", userId );
						command.Parameters.AddWithValue( "@language", "zh-CN" );
						command.Parameters.AddWithValue( "@theme", "dark" );
						command.Parameters.AddWithValue( "@auto_save", 5 );
						command.Parameters.AddWithValue( "@undo_steps", 50 );
						command.ExecuteNonQuery();
					}

					// 插入默认偏好设置
					InsertDefaultPreference( connection, userId, "video_quality", "high" );
					InsertDefaultPreference( connection, userId, "audio_bitrate", "320" );
					InsertDefaultPreference( connection, userId, "export_format", "mp4" );
					InsertDefaultPreference( connection, userId, "timeline_snap", "true" );
				}

				Console.WriteLine( "默认数据插入完成" );
			}
		}

		private void InsertDefaultPreference(SQLiteConnection connection, int userId, string key, string value)
		{
			string insertPreference = @"
                INSERT OR IGNORE INTO user_preferences (user_id, preference_key, preference_value)
                VALUES (@user_id, @key, @value)";

			using (var command = new SQLiteCommand( insertPreference, connection )) {
				command.Parameters.AddWithValue( "@user_id", userId );
				command.Parameters.AddWithValue( "@key", key );
				command.Parameters.AddWithValue( "@value", value );
				command.ExecuteNonQuery();
			}
		}

		public void CreateTriggers( )
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				// 创建更新时间触发器
				string createUsersTrigger = @"
                    CREATE TRIGGER IF NOT EXISTS update_users_timestamp 
                    AFTER UPDATE ON users
                    BEGIN
                        UPDATE users SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
                    END";

				using (var command = new SQLiteCommand( createUsersTrigger, connection )) {
					command.ExecuteNonQuery();
				}

				string createProfilesTrigger = @"
                    CREATE TRIGGER IF NOT EXISTS update_profiles_timestamp 
                    AFTER UPDATE ON user_profiles
                    BEGIN
                        UPDATE user_profiles SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
                    END";

				using (var command = new SQLiteCommand( createProfilesTrigger, connection )) {
					command.ExecuteNonQuery();
				}

				string createPreferencesTrigger = @"
                    CREATE TRIGGER IF NOT EXISTS update_preferences_timestamp 
                    AFTER UPDATE ON user_preferences
                    BEGIN
                        UPDATE user_preferences SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
                    END";

				using (var command = new SQLiteCommand( createPreferencesTrigger, connection )) {
					command.ExecuteNonQuery();
				}

				Console.WriteLine( "触发器创建完成" );
			}
		}
		#endregion

		#region -----------   视频剪辑软件的 事务表 如何定义    ------------
		/*
		视频剪辑软件的 事务表 如何定义
		视频剪辑软件中的事务表主要用于记录用户的操作历史、项目变更、_undo/redo_操作等。以下是如何定义视频剪辑软件的事务表结构：
事务表结构设计
1. ----------------------- projects 表 - 项目信息表
列名			类型	约束	描述
id				INTEGER	PRIMARY KEY AUTOINCREMENT				项目ID
user_id			INTEGER	NOT NULL, FOREIGN KEY REFERENCES users(id)	用户ID
name			TEXT	NOT NULL								项目名称
description		TEXT		项目描述
width			INTEGER	NOT NULL DEFAULT 1920				项目宽度
height			INTEGER	NOT NULL DEFAULT 1080				项目高度
framerate		REAL	NOT NULL DEFAULT 30.0				帧率
duration		REAL	NOT NULL DEFAULT 0.0				项目时长
thumbnail_path		TEXT		缩略图路径
created_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间
note			TEXT		备注信息
		约束：PRIMARY KEY(id)
		约束：FOREIGN KEY REFERENCES users(id) ON DELETE CASCADE
		索引：UNIQUE(name, user_id)

2. ------------------- media_assets 表 - 媒体资源表
列名			类型	约束	描述
id				INTEGER	PRIMARY KEY AUTOINCREMENT	资源ID
user_id			INTEGER	NOT NULL, FOREIGN KEY REFERENCES users(id)	用户ID
name			TEXT	NOT NULL					资源名称
file_path			TEXT	NOT NULL			文件路径
file_size			INTEGER	NOT NULL				文件大小(字节)
media_type			TEXT	NOT NULL				媒体类型(video/audio/image)
duration			REAL		时长(秒)
width				INTEGER		宽度
height				INTEGER		高度
framerate			REAL		帧率
codec				TEXT		编解码器
created_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at			DATETIME	 DEFAULT CURRENT_TIMESTAMP	更新时间
note				TEXT		备注信息

3. ----------------------------- timeline_tracks 表 - 时间线轨道表
列名			类型	约束	描述
id				INTEGER	PRIMARY KEY AUTOINCREMENT	轨道ID
project_id			INTEGER	NOT NULL, FOREIGN KEY REFERENCES projects(id)	项目ID
track_type			TEXT	NOT NULL	轨道类型(video/audio)
track_index			INTEGER	NOT NULL	轨道索引
name				TEXT	NOT NULL	轨道名称
is_muted			INTEGER	NOT NULL DEFAULT 0	是否静音
is_locked			INTEGER	NOT NULL DEFAULT 0	是否锁定
volume				REAL	NOT NULL DEFAULT 1.0	音量(0.0-1.0)
created_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间
note				TEXT		备注信息

4. ----------------------------- clips 表 - 剪辑片段表
列名				类型	约束	描述
id					INTEGER	PRIMARY KEY AUTOINCREMENT	剪辑ID
project_id			INTEGER	NOT NULL, FOREIGN KEY REFERENCES projects(id)	项目ID
track_id			INTEGER	NOT NULL, FOREIGN KEY REFERENCES timeline_tracks(id)	轨道ID
media_asset_id		INTEGER	FOREIGN KEY REFERENCES media_assets(id)	媒体资源ID
name				TEXT	NOT NULL		剪辑名称
start_time			REAL	NOT NULL		在时间线上的开始时间
end_time			REAL	NOT NULL		在时间线上的结束时间
media_start_time	REAL	NOT NULL DEFAULT 0.0	在媒体中的开始时间
media_end_time		REAL	NOT NULL				在媒体中的结束时间
position_x			REAL	NOT NULL DEFAULT 0.0	X位置
position_y			REAL	NOT NULL DEFAULT 0.0	Y位置
scale_x				REAL	NOT NULL DEFAULT 1.0	X缩放
scale_y				REAL	NOT NULL DEFAULT 1.0	Y缩放
rotation			REAL	NOT NULL DEFAULT 0.0	旋转角度
volume				REAL	NOT NULL DEFAULT 1.0	音量
is_muted			INTEGER	NOT NULL DEFAULT 0		是否静音
created_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间
note				TEXT		备注信息


5. --------------------------- transactions 表 - 事务表
列名				类型	约束	描述
id					INTEGER	PRIMARY KEY AUTOINCREMENT	事务ID
project_id			INTEGER	NOT NULL, FOREIGN KEY REFERENCES projects(id)	项目ID
user_id				INTEGER	NOT NULL, FOREIGN KEY REFERENCES users(id)	用户ID
transaction_type	TEXT	NOT NULL									事务类型
description			TEXT	NOT NULL							事务描述
timestamp			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	时间戳

is_undone			INTEGER	NOT NULL DEFAULT 0					是否已撤销
is_redone			INTEGER	NOT NULL DEFAULT 0					是否已重做
created_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
updated_at			DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间
note				TEXT		备注信息
		约束：PRIMARY KEY(id)
		约束：FOREIGN KEY REFERENCES projects(id) ON DELETE CASCADE
		约束：FOREIGN KEY REFERENCES users(id) ON DELETE CASCADE

6. ----------------------- transaction_details 表 - 事务详情表
列名			类型	约束	描述
id	INTEGER	PRIMARY KEY AUTOINCREMENT	详情ID
transaction_id	INTEGER	NOT NULL, FOREIGN KEY REFERENCES transactions(id)	事务ID
operation_type	TEXT	NOT NULL	操作类型(create/update/delete)
table_name		TEXT	NOT NULL	表名
record_id		INTEGER	NOT NULL	记录ID
old_values		TEXT		旧值(JSON格式)
new_values		TEXT		新值(JSON格式)
created_at		DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
note				TEXT		备注信息

7. ----------------------------effects 表 - 效果表
列名			类型	约束	描述
id				INTEGER	PRIMARY KEY AUTOINCREMENT	效果ID
name			TEXT	NOT NULL	效果名称
effect_type		TEXT	NOT NULL	效果类型
description		TEXT		效果描述
parameters		TEXT		参数定义(JSON格式)
created_at		DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
note				TEXT		备注信息

8. -----------------clip_effects 表 - 剪辑效果关联表
列名			类型	约束	描述
id				INTEGER	PRIMARY KEY AUTOINCREMENT	关联ID
clip_id			INTEGER	NOT NULL, FOREIGN KEY REFERENCES clips(id)	剪辑ID
effect_id		INTEGER	NOT NULL, FOREIGN KEY REFERENCES effects(id)	效果ID
parameters		TEXT	NOT NULL	参数值(JSON格式)
start_time		REAL	NOT NULL DEFAULT 0.0	开始时间
end_time		REAL	NOT NULL	结束时间
order_index		INTEGER	NOT NULL	排序索引
is_enabled		INTEGER	NOT NULL DEFAULT 1	是否启用
created_at		DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
note				TEXT		备注信息

C# SQLite 事务表创建程序
 
事务操作示例类
 
这个事务表设计提供了以下功能：
1.	完整的项目结构管理 - 包括项目、媒体资源、时间线轨道和剪辑片段
2.	事务跟踪 - 记录所有用户操作，支持撤销/重做功能
3.	效果系统 - 支持为剪辑添加各种效果
4.	索引优化 - 为常用查询字段创建索引
5.	自动时间戳 - 使用触发器自动更新时间戳
6.	数据完整性 - 使用外键约束保证数据一致性
通过这些表结构，视频剪辑软件可以完整地记录用户的操作历史，实现_undo/redo_功能，并且可以追踪项目的完整变更历史。
		*/

		//namespace VideoEditor.Database  	{	public class TransactionDatabaseSetup		{		
		/// <summary>
		/// 创建11个  clip  表
		/// </summary>
		public void CreateTransactionTables( )
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				// 创建项目表
				CreateProjectsTable( connection );

				// 创建媒体资源表
				CreateMediaAssetsTable( connection );

				// 创建时间线轨道表
				CreateTimelineTracksTable( connection );

				// 创建剪辑片段表
				CreateClipsTable( connection );

				// 创建事务表
				CreateTransactionsTable( connection );

				// 创建事务详情表
				CreateTransactionDetailsTable( connection );

				// 创建效果表
				CreateEffectsTable( connection );

				// 创建剪辑效果关联表
				CreateClipEffectsTable( connection );

				// 创建索引
				CreateIndexes( connection );

				// 创建触发器
				CreateTriggers( connection );

				Console.WriteLine( "视频剪辑事务数据库表创建完成" );
			}
		}
		#region  -----------  old  clip  creat table -----------
		//private void CreateProjectsTable(SQLiteConnection connection)
		//{
		//	string sql = @"
		//              CREATE TABLE IF NOT EXISTS projects (
		//                  id INTEGER PRIMARY KEY AUTOINCREMENT,
		//                  user_id INTEGER NOT NULL,
		//                  name TEXT NOT NULL,
		//                  description TEXT,
		//                  width INTEGER NOT NULL DEFAULT 1920,
		//                  height INTEGER NOT NULL DEFAULT 1080,
		//                  framerate REAL NOT NULL DEFAULT 30.0,
		//                  duration REAL NOT NULL DEFAULT 0.0,
		//                  thumbnail_path TEXT,
		//                  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
		//                  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
		//			note TEXT,
		//                  FOREIGN KEY (user_id) REFERENCES users (id)
		//              )";

		//	using (var command = new SQLiteCommand( sql, connection )) {
		//		command.ExecuteNonQuery();
		//	}
		//}

		//private void CreateMediaAssetsTable(SQLiteConnection connection)
		//{
		//	string sql = @"
		//              CREATE TABLE IF NOT EXISTS media_assets (
		//                  id INTEGER PRIMARY KEY AUTOINCREMENT,
		//                  user_id INTEGER NOT NULL,
		//                  name TEXT NOT NULL,
		//                  file_path TEXT NOT NULL,
		//                  file_size INTEGER NOT NULL,
		//                  media_type TEXT NOT NULL,
		//                  duration REAL,
		//                  width INTEGER,
		//                  height INTEGER,
		//                  framerate REAL,
		//                  codec TEXT,
		//                  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
		//			updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
		//			note TEXT,
		//                  FOREIGN KEY (user_id) REFERENCES users (id)
		//              )";

		//	using (var command = new SQLiteCommand( sql, connection )) {
		//		command.ExecuteNonQuery();
		//	}
		//}

		//private void CreateTimelineTracksTable(SQLiteConnection connection)
		//{
		//	string sql = @"
		//              CREATE TABLE IF NOT EXISTS timeline_tracks (
		//                  id INTEGER PRIMARY KEY AUTOINCREMENT,
		//                  project_id INTEGER NOT NULL,
		//                  track_type TEXT NOT NULL,
		//                  track_index INTEGER NOT NULL,
		//                  name TEXT NOT NULL,
		//                  is_muted INTEGER NOT NULL DEFAULT 0,
		//                  is_locked INTEGER NOT NULL DEFAULT 0,
		//                  volume REAL NOT NULL DEFAULT 1.0,
		//                  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
		//			updated_at DATETIME  DEFAULT CURRENT_TIMESTAMP,
		//			note TEXT,
		//                  FOREIGN KEY (project_id) REFERENCES projects (id)
		//              )";

		//	using (var command = new SQLiteCommand( sql, connection )) {
		//		command.ExecuteNonQuery();
		//	}
		//}

		//private void CreateClipsTable(SQLiteConnection connection)
		//{
		//	string sql = @"
		//              CREATE TABLE IF NOT EXISTS clips (
		//                  id INTEGER PRIMARY KEY AUTOINCREMENT,
		//                  project_id INTEGER NOT NULL,
		//                  track_id INTEGER NOT NULL,
		//                  media_asset_id INTEGER,
		//                  name TEXT NOT NULL,
		//                  start_time REAL NOT NULL,
		//                  end_time REAL NOT NULL,
		//                  media_start_time REAL NOT NULL DEFAULT 0.0,
		//                  media_end_time REAL NOT NULL,
		//                  position_x REAL NOT NULL DEFAULT 0.0,
		//                  position_y REAL NOT NULL DEFAULT 0.0,
		//                  scale_x REAL NOT NULL DEFAULT 1.0,
		//                  scale_y REAL NOT NULL DEFAULT 1.0,
		//                  rotation REAL NOT NULL DEFAULT 0.0,
		//                  volume REAL NOT NULL DEFAULT 1.0,
		//                  is_muted INTEGER NOT NULL DEFAULT 0,
		//                  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
		//                  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
		//			note TEXT,
		//                  FOREIGN KEY (project_id) REFERENCES projects (id),
		//                  FOREIGN KEY (track_id) REFERENCES timeline_tracks (id),
		//                  FOREIGN KEY (media_asset_id) REFERENCES media_assets (id)
		//              )";

		//	using (var command = new SQLiteCommand( sql, connection )) {
		//		command.ExecuteNonQuery();
		//	}
		//}

		//private void CreateTransactionsTable(SQLiteConnection connection)
		//{
		//	string sql = @"
		//              CREATE TABLE IF NOT EXISTS transactions (
		//                  id INTEGER PRIMARY KEY AUTOINCREMENT,
		//                  project_id INTEGER NOT NULL,
		//                  user_id INTEGER NOT NULL,
		//                  transaction_type TEXT NOT NULL,
		//                  description TEXT NOT NULL,
		//                  timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
		//                  is_undone INTEGER NOT NULL DEFAULT 0,
		//			is_redone INTEGER NOT NULL DEFAULT 0,
		//			created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
		//			updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
		//			note TEXT,
		//                  FOREIGN KEY (project_id) REFERENCES projects (id),
		//                  FOREIGN KEY (user_id) REFERENCES users (id)
		//              )";

		//	using (var command = new SQLiteCommand( sql, connection )) {
		//		command.ExecuteNonQuery();
		//	}
		//}

		//private void CreateTransactionDetailsTable(SQLiteConnection connection)
		//{
		//	string sql = @"
		//              CREATE TABLE IF NOT EXISTS transaction_details (
		//                  id INTEGER PRIMARY KEY AUTOINCREMENT,
		//                  transaction_id INTEGER NOT NULL,
		//                  operation_type TEXT NOT NULL,
		//                  table_name TEXT NOT NULL,
		//                  record_id INTEGER NOT NULL,
		//                  old_values TEXT,
		//                  new_values TEXT,
		//                  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
		//			note TEXT,
		//                  FOREIGN KEY (transaction_id) REFERENCES transactions (id)
		//              )";

		//	using (var command = new SQLiteCommand( sql, connection )) {
		//		command.ExecuteNonQuery();
		//	}
		//}

		//private void CreateEffectsTable(SQLiteConnection connection)
		//{
		//	string sql = @"
		//              CREATE TABLE IF NOT EXISTS effects (
		//                  id INTEGER PRIMARY KEY AUTOINCREMENT,
		//                  name TEXT NOT NULL,
		//                  effect_type TEXT NOT NULL,
		//                  description TEXT,
		//                  parameters TEXT,
		//                  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
		//			note TEXT

		//              )";

		//	using (var command = new SQLiteCommand( sql, connection )) {
		//		command.ExecuteNonQuery();
		//	}
		//}

		//private void CreateClipEffectsTable(SQLiteConnection connection)
		//{
		//	string sql = @"
		//              CREATE TABLE IF NOT EXISTS clip_effects (
		//                  id INTEGER PRIMARY KEY AUTOINCREMENT,
		//                  clip_id INTEGER NOT NULL,
		//                  effect_id INTEGER NOT NULL,
		//                  parameters TEXT NOT NULL,
		//                  start_time REAL NOT NULL DEFAULT 0.0,
		//                  end_time REAL NOT NULL,
		//                  order_index INTEGER NOT NULL,
		//                  is_enabled INTEGER NOT NULL DEFAULT 1,
		//                  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
		//			note TEXT,
		//                  FOREIGN KEY (clip_id) REFERENCES clips (id),
		//                  FOREIGN KEY (effect_id) REFERENCES effects (id)
		//              )";

		//	using (var command = new SQLiteCommand( sql, connection )) {
		//		command.ExecuteNonQuery();
		//	}
		//}
		#endregion
		/// <summary>
		/// 项目索引
		/// </summary>
		/// <param name="connection"></param>
		private void CreateIndexes(SQLiteConnection connection)
		{
			
			string projectIndex = "CREATE INDEX IF NOT EXISTS idx_projects_user ON projects (user_id)";
			using (var command = new SQLiteCommand( projectIndex, connection )) {
				command.ExecuteNonQuery();
			}

			// 媒体资源索引
			string mediaIndex = "CREATE INDEX IF NOT EXISTS idx_media_assets_user ON media_assets (user_id)";
			using (var command = new SQLiteCommand( mediaIndex, connection )) {
				command.ExecuteNonQuery();
			}

			// 轨道索引
			string trackIndex = "CREATE INDEX IF NOT EXISTS idx_timeline_tracks_project ON timeline_tracks (project_id)";
			using (var command = new SQLiteCommand( trackIndex, connection )) {
				command.ExecuteNonQuery();
			}

			// 剪辑索引
			string clipIndex1 = "CREATE INDEX IF NOT EXISTS idx_clips_project ON clips (project_id)";
			using (var command = new SQLiteCommand( clipIndex1, connection )) {
				command.ExecuteNonQuery();
			}

			string clipIndex2 = "CREATE INDEX IF NOT EXISTS idx_clips_track ON clips (track_id)";
			using (var command = new SQLiteCommand( clipIndex2, connection )) {
				command.ExecuteNonQuery();
			}

			// 事务索引
			string transactionIndex1 = "CREATE INDEX IF NOT EXISTS idx_transactions_project ON transactions (project_id)";
			using (var command = new SQLiteCommand( transactionIndex1, connection )) {
				command.ExecuteNonQuery();
			}

			string transactionIndex2 = "CREATE INDEX IF NOT EXISTS idx_transactions_user ON transactions (user_id)";
			using (var command = new SQLiteCommand( transactionIndex2, connection )) {
				command.ExecuteNonQuery();
			}

			// 事务详情索引
			string detailIndex = "CREATE INDEX IF NOT EXISTS idx_transaction_details_transaction ON transaction_details (transaction_id)";
			using (var command = new SQLiteCommand( detailIndex, connection )) {
				command.ExecuteNonQuery();
			}

			// 剪辑效果索引
			string clipEffectIndex1 = "CREATE INDEX IF NOT EXISTS idx_clip_effects_clip ON clip_effects (clip_id)";
			using (var command = new SQLiteCommand( clipEffectIndex1, connection )) {
				command.ExecuteNonQuery();
			}

			string clipEffectIndex2 = "CREATE INDEX IF NOT EXISTS idx_clip_effects_effect ON clip_effects (effect_id)";
			using (var command = new SQLiteCommand( clipEffectIndex2, connection )) {
				command.ExecuteNonQuery();
			}
		}

		private void CreateTriggers(SQLiteConnection connection)
		{
			// 项目更新时间触发器
			string projectTrigger = @"
                CREATE TRIGGER IF NOT EXISTS update_projects_timestamp 
                AFTER UPDATE ON projects
                BEGIN
                    UPDATE projects SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
                END";
			using (var command = new SQLiteCommand( projectTrigger, connection )) {
				command.ExecuteNonQuery();
			}

			// 剪辑更新时间触发器
			string clipTrigger = @"
                CREATE TRIGGER IF NOT EXISTS update_clips_timestamp 
                AFTER UPDATE ON clips
                BEGIN
                    UPDATE clips SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
                END";
			using (var command = new SQLiteCommand( clipTrigger, connection )) {
				command.ExecuteNonQuery();
			}
		}
		#endregion
		
		#region  -----------  事务操作示例类  ----------------

		// 记录事务
		public int RecordTransaction(int projectId, int userId, string transactionType, string description)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    INSERT INTO transactions (project_id, user_id, transaction_type, description)
                    VALUES (@project_id, @user_id, @transaction_type, @description);
                    SELECT last_insert_rowid();";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@project_id", projectId );
					command.Parameters.AddWithValue( "@user_id", userId );
					command.Parameters.AddWithValue( "@transaction_type", transactionType );
					command.Parameters.AddWithValue( "@description", description );

					return Convert.ToInt32( command.ExecuteScalar() );
				}
			}
		}
		// 记录事务详情
		//当前时间 怎么输入表中
		public void RecordTransactionDetail(int transactionId, string operationType, string tableName, int recordId, string oldValues, string newValues)
		//当前时间 怎么输入表中

		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();
				string sql = @"
                    INSERT INTO transaction_details 
                    (transaction_id, operation_type, table_name, record_id, old_values, new_values, created_at,note)
                    VALUES (@transaction_id, @operation_type, @table_name, @record_id, @old_values, @new_values,@created_at, @note)";
				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@transaction_id", transactionId );
					command.Parameters.AddWithValue( "@operation_type", operationType );
					command.Parameters.AddWithValue( "@table_name", tableName );
					command.Parameters.AddWithValue( "@record_id", recordId );
					command.Parameters.AddWithValue( "@old_values", oldValues ?? "" );
					command.Parameters.AddWithValue( "@new_values", newValues ?? "" );
					command.Parameters.AddWithValue( "@created_at", newValues ?? "" );
					command.Parameters.AddWithValue( "@note", newValues ?? "注释" );
					command.ExecuteNonQuery();
				}
			}
		}

		// 获取项目事务历史
		public void GetProjectTransactions(int projectId)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();
				string sql = @"
                    SELECT t.*, u.username 
                    FROM transactions t
                    JOIN users u ON t.user_id = u.id
                    WHERE t.project_id = @project_id
                    ORDER BY t.timestamp DESC
                    LIMIT 100";
				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@project_id", projectId );
					using (var reader = command.ExecuteReader()) {
						while (reader.Read()) {
							Console.WriteLine( $"事务: {reader["description"]}, 时间: {reader["timestamp"]}, 用户: {reader["username"]}" );
						}
					}
				}
			}
		}
		// 撤销事务
		public bool UndoTransaction(int transactionId)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();
				using (var transaction = connection.BeginTransaction()) {
					try {
						// 标记事务为已撤销
						string updateSql = "UPDATE transactions SET is_undone = 1 WHERE id = @id";
						using (var command = new SQLiteCommand( updateSql, connection )) {
							command.Parameters.AddWithValue( "@id", transactionId );
							command.ExecuteNonQuery();
						}
						// 这里可以添加实际的撤销逻辑
						// 例如根据transaction_details中的信息恢复数据
						transaction.Commit();
						return true;
					}
					catch {
						transaction.Rollback();
						return false;
					}
				}
			}
		}
		/// <summary>
		/// 第一次 初始化事务  已经不用
		/// </summary>
		public void transactioninit( )
		{

			// 初始化各个仓库
			var projectRepo = new ProjectRepository( _connectionString );
			var mediaAssetRepo = new MediaAssetRepository( _connectionString );
			var trackRepo = new TimelineTrackRepository( _connectionString );
			var clipRepo = new ClipRepository( _connectionString );
			var transactionRepo = new TransactionRepository( _connectionString );
			var transactionDetailRepo = new TransactionDetailRepository( _connectionString );

			try {
				// 创建项目
				var project = new Project
				{
					UserId = 1,
					Name = "我的视频项目",
					Description = "测试项目",
					Width = 1920,
					Height = 1080,
					Framerate = 30.0,
					Duration = 0.0,
					ThumbnailPath = @"D:\Documents\ResourceFolder\D:\Documents\ResourceFolder.jpg",
					CreatedAt = DateTime.Now,
					UpdatedAt = DateTime.Now,
					Note = "这是一个测试项目"
				};

				int projectId = projectRepo.Create( project );
				Console.WriteLine( $"创建项目，ID: {projectId}" );

				// 获取项目
				var retrievedProject = projectRepo.GetById( projectId );
				Console.WriteLine( $"项目名称: {retrievedProject.Name}" );

				// 更新项目
				retrievedProject.Description = "更新后的项目描述";
				projectRepo.Update( retrievedProject );
				Console.WriteLine( "项目已更新" );

				// 创建媒体资源
				var mediaAsset = new MediaAsset
				{
					UserId = 1,
					Name = "测试视频.mp4",
					FilePath = @"C:\Videos\test.mp4",
					FileSize = 1024000,
					MediaType = "video",
					Duration = 60.0,
					Width = 1920,
					Height = 1080,
					Framerate = 30.0,
					Codec = "H.264"
				};

				int mediaAssetId = mediaAssetRepo.Create( mediaAsset );
				Console.WriteLine( $"创建媒体资源，ID: {mediaAssetId}" );

				// 创建时间线轨道
				var track = new TimelineTrack
				{
					ProjectId = projectId,
					TrackType = "video",
					TrackIndex = 0,
					Name = "视频轨道1",
					IsMuted = false,
					IsLocked = false,
					Volume = 1.0
				};

				int trackId = trackRepo.Create( track );
				Console.WriteLine( $"创建轨道，ID: {trackId}" );

				// 创建剪辑片段
				var clip = new Clip
				{
					ProjectId = projectId,
					TrackId = trackId,
					MediaAssetId = mediaAssetId,
					Name = "剪辑片段1",
					StartTime = 0.0,
					EndTime = 10.0,
					MediaStartTime = 0.0,
					MediaEndTime = 10.0,
					PositionX = 0.0,
					PositionY = 0.0,
					ScaleX = 1.0,
					ScaleY = 1.0,
					Rotation = 0.0,
					Volume = 1.0,
					IsMuted = false
				};

				int clipId = clipRepo.Create( clip );
				Console.WriteLine( $"创建剪辑片段，ID: {clipId}" );
				// 记录事务
				//int transactionId = transactionRepo.RecordTransaction( projectId, 1, "create_clip", "创建剪辑片段" );
				//Console.WriteLine( $"记录事务，ID: {transactionId}" );
				//// 记录事务详情
				//transactionDetailRepo.RecordTransactionDetail( transactionId, "create", "clips", clipId, null, clip.ToString() );
				Console.WriteLine( "记录事务详情" );
			}
			catch (Exception ex) {
				Console.WriteLine( $"发生错误: {ex.Message}" );
			}
		}

	
			static void TestDatabaseConnection(string databasePath)
			{
			try {
				string connectionString = $"Data Source={databasePath};Version=3;";
				using (var connection = new SQLiteConnection( connectionString )) {
					connection.Open();

					// 测试查询
					string sql = "SELECT name FROM sqlite_master WHERE type='table'";
					using (var command = new SQLiteCommand( sql, connection ))
					using (var reader = command.ExecuteReader()) {
						Console.WriteLine( "\n数据库中的表:" );
						while (reader.Read()) {
							Console.WriteLine( $"  - {reader["name"]}" );
						}
					}
				}
			}
			catch (Exception ex) {
				Console.WriteLine( $"数据库连接测试失败: {ex.Message}" );
			}
		}

		#endregion
		#region  ------------ new 	创建所有事务表 - 按正确的依赖顺序创建所有11个表 -------------------
		/// <summary>
		/// 初始化所有数据库表  不能在运行  2025-7-30
		/// </summary>
		public void InitializeAllTables( )
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();
				// 按依赖顺序创建表
				CreateUserTable( connection );
				CreateSettingsTable( connection );
				CreateProjectsTable( connection );
				CreateMediaAssetsTable( connection );
				CreateTimelineTracksTable( connection );
				CreateClipsTable( connection );
				CreateTransactionsTable( connection );
				CreateTransactionDetailsTable( connection );
				CreateEffectsTable( connection );
				CreateClipEffectsTable( connection );
				CreateUserProfilesTable( connection );
				CreateUserSessionsTable( connection );
				CreateUserPreferencesTable( connection );
				// 创建索引
				CreateAllIndexes( connection );
				// 创建触发器
				CreateAllTriggers( connection );
				Console.WriteLine( "所有数据库表初始化完成" );
			}
		}
		#endregion

		#region -------------最终 事务 表创建方法 --------------------


		private void CreateProjectsTable(SQLiteConnection connection)
		{
			string sql = @"
                CREATE TABLE IF NOT EXISTS projects (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    user_id INTEGER NOT NULL,
                    name TEXT NOT NULL,
                    description TEXT,
                    width INTEGER NOT NULL DEFAULT 1920,
                    height INTEGER NOT NULL DEFAULT 1080,
                    framerate REAL NOT NULL DEFAULT 30.0,
                    duration REAL NOT NULL DEFAULT 0.0,
                    thumbnail_path TEXT,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (user_id) REFERENCES users (id)
                )";

			using (var command = new SQLiteCommand( sql, connection )) {
				command.ExecuteNonQuery();
			}
		}

		private void CreateMediaAssetsTable(SQLiteConnection connection)
		{
			string sql = @"
                CREATE TABLE IF NOT EXISTS media_assets (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    user_id INTEGER NOT NULL,
                    name TEXT NOT NULL,
                    file_path TEXT NOT NULL,
                    file_size INTEGER NOT NULL,
                    media_type TEXT NOT NULL,
                    duration REAL,
                    width INTEGER,
                    height INTEGER,
                    framerate REAL,
                    codec TEXT,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (user_id) REFERENCES users (id)
                )";

			using (var command = new SQLiteCommand( sql, connection )) {
				command.ExecuteNonQuery();
			}
		}

		private void CreateTimelineTracksTable(SQLiteConnection connection)
		{
			string sql = @"
                CREATE TABLE IF NOT EXISTS timeline_tracks (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    project_id INTEGER NOT NULL,
                    track_type TEXT NOT NULL,
                    track_index INTEGER NOT NULL,
                    name TEXT NOT NULL,
                    is_muted INTEGER NOT NULL DEFAULT 0,
                    is_locked INTEGER NOT NULL DEFAULT 0,
                    volume REAL NOT NULL DEFAULT 1.0,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (project_id) REFERENCES projects (id)
                )";

			using (var command = new SQLiteCommand( sql, connection )) {
				command.ExecuteNonQuery();
			}
		}

		private void CreateClipsTable(SQLiteConnection connection)
		{
			string sql = @"
                CREATE TABLE IF NOT EXISTS clips (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    project_id INTEGER NOT NULL,
                    track_id INTEGER NOT NULL,
                    media_asset_id INTEGER,
                    name TEXT NOT NULL,
                    start_time REAL NOT NULL,
                    end_time REAL NOT NULL,
                    media_start_time REAL NOT NULL DEFAULT 0.0,
                    media_end_time REAL NOT NULL,
                    position_x REAL NOT NULL DEFAULT 0.0,
                    position_y REAL NOT NULL DEFAULT 0.0,
                    scale_x REAL NOT NULL DEFAULT 1.0,
                    scale_y REAL NOT NULL DEFAULT 1.0,
                    rotation REAL NOT NULL DEFAULT 0.0,
                    volume REAL NOT NULL DEFAULT 1.0,
                    is_muted INTEGER NOT NULL DEFAULT 0,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (project_id) REFERENCES projects (id),
                    FOREIGN KEY (track_id) REFERENCES timeline_tracks (id),
                    FOREIGN KEY (media_asset_id) REFERENCES media_assets (id)
                )";

			using (var command = new SQLiteCommand( sql, connection )) {
				command.ExecuteNonQuery();
			}
		}
		private void CreateTransactionsTable(SQLiteConnection connection)
		{
			string sql = @"
                CREATE TABLE IF NOT EXISTS transactions (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    project_id INTEGER NOT NULL,
                    user_id INTEGER NOT NULL,
                    transaction_type TEXT NOT NULL,
                    description TEXT NOT NULL,
                    timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    is_undone INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (project_id) REFERENCES projects (id),
                    FOREIGN KEY (user_id) REFERENCES users (id)
                )";

			using (var command = new SQLiteCommand( sql, connection )) {
				command.ExecuteNonQuery();
			}
		}

		private void CreateTransactionDetailsTable(SQLiteConnection connection)
		{
			string sql = @"
                CREATE TABLE IF NOT EXISTS transaction_details (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    transaction_id INTEGER NOT NULL,
                    operation_type TEXT NOT NULL,
                    table_name TEXT NOT NULL,
                    record_id INTEGER NOT NULL,
                    old_values TEXT,
                    new_values TEXT,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (transaction_id) REFERENCES transactions (id)
                )";

			using (var command = new SQLiteCommand( sql, connection )) {
				command.ExecuteNonQuery();
			}
		}

		private void CreateEffectsTable(SQLiteConnection connection)
		{
			string sql = @"
                CREATE TABLE IF NOT EXISTS effects (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    effect_type TEXT NOT NULL,
                    description TEXT,
                    parameters TEXT,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
                )";

			using (var command = new SQLiteCommand( sql, connection )) {
				command.ExecuteNonQuery();
			}
		}

		private void CreateClipEffectsTable(SQLiteConnection connection)
		{
			string sql = @"
                CREATE TABLE IF NOT EXISTS clip_effects (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    clip_id INTEGER NOT NULL,
                    effect_id INTEGER NOT NULL,
                    parameters TEXT NOT NULL,
                    start_time REAL NOT NULL DEFAULT 0.0,
                    end_time REAL NOT NULL,
                    order_index INTEGER NOT NULL,
                    is_enabled INTEGER NOT NULL DEFAULT 1,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (clip_id) REFERENCES clips (id),
                    FOREIGN KEY (effect_id) REFERENCES effects (id)
                )";

			using (var command = new SQLiteCommand( sql, connection )) {
				command.ExecuteNonQuery();
			}
		}

		private void CreateUserProfilesTable(SQLiteConnection connection)
		{
			string sql = @"
                CREATE TABLE IF NOT EXISTS user_profiles (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    user_id INTEGER NOT NULL UNIQUE,
                    preferred_language TEXT,
                    theme TEXT,
                    default_project_location TEXT,
                    auto_save_interval INTEGER,
                    max_undo_steps INTEGER,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
                )";

			using (var command = new SQLiteCommand( sql, connection )) {
				command.ExecuteNonQuery();
			}
		}

		private void CreateUserSessionsTable(SQLiteConnection connection)
		{
			string sql = @"
                CREATE TABLE IF NOT EXISTS user_sessions (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    user_id INTEGER NOT NULL,
                    session_token TEXT NOT NULL UNIQUE,
                    ip_address TEXT,
                    user_agent TEXT,
                    expires_at DATETIME NOT NULL,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
                )";

			using (var command = new SQLiteCommand( sql, connection )) {
				command.ExecuteNonQuery();
			}
		}
		private void CreateUserPreferencesTable(SQLiteConnection connection)
		{
			string sql = @"
                CREATE TABLE IF NOT EXISTS user_preferences (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    user_id INTEGER NOT NULL,
                    preference_key TEXT NOT NULL,
                    preference_value TEXT NOT NULL,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
                )";

			using (var command = new SQLiteCommand( sql, connection )) {
				command.ExecuteNonQuery();
			}
		}

		#endregion

		#region -------------------  索引创建方法  ---------------------

		private void CreateAllIndexes(SQLiteConnection connection)
		{
			// 用户相关索引
			ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_users_username ON users (username)" );
			ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_users_email ON users (email)" );

			// 项目相关索引
			ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_projects_user ON projects (user_id)" );

			// 媒体资源索引
			ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_media_assets_user ON media_assets (user_id)" );

			// 时间线轨道索引
			ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_timeline_tracks_project ON timeline_tracks (project_id)" );

			// 剪辑索引
			ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_clips_project ON clips (project_id)" );
			ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_clips_track ON clips (track_id)" );

			// 事务索引
			ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_transactions_project ON transactions (project_id)" );
			ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_transactions_user ON transactions (user_id)" );
			ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_transactions_timestamp ON transactions (timestamp)" );

			// 事务详情索引
			ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_transaction_details_transaction ON transaction_details (transaction_id)" );

			// 用户会话索引
			ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_sessions_token ON user_sessions (session_token)" );
			ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_sessions_expires ON user_sessions (expires_at)" );

			// 用户偏好索引
			ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_user_preferences ON user_preferences (user_id, preference_key)" );

			// 剪辑效果索引
			ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_clip_effects_clip ON clip_effects (clip_id)" );
			ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_clip_effects_effect ON clip_effects (effect_id)" );

			Console.WriteLine( "所有索引创建完成" );
		}

		#endregion

		#region --------------------- 触发器创建方法 ------------------------

		private void CreateAllTriggers(SQLiteConnection connection)
		{
			// 用户更新时间触发器
			string userTrigger = @"
                CREATE TRIGGER IF NOT EXISTS update_users_timestamp 
                AFTER UPDATE ON users
                BEGIN
                    UPDATE users SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
                END";
			ExecuteNonQuery( connection, userTrigger );

			// 项目更新时间触发器
			string projectTrigger = @"
                CREATE TRIGGER IF NOT EXISTS update_projects_timestamp 
                AFTER UPDATE ON projects
                BEGIN
                    UPDATE projects SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
                END";
			ExecuteNonQuery( connection, projectTrigger );

			// 剪辑更新时间触发器
			string clipTrigger = @"
                CREATE TRIGGER IF NOT EXISTS update_clips_timestamp 
                AFTER UPDATE ON clips
                BEGIN
                    UPDATE clips SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
                END";
			ExecuteNonQuery( connection, clipTrigger );

			// 用户配置更新时间触发器
			string profileTrigger = @"
                CREATE TRIGGER IF NOT EXISTS update_profiles_timestamp 
                AFTER UPDATE ON user_profiles
                BEGIN
                    UPDATE user_profiles SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
                END";
			ExecuteNonQuery( connection, profileTrigger );

			// 用户偏好更新时间触发器
			string preferenceTrigger = @"
                CREATE TRIGGER IF NOT EXISTS update_preferences_timestamp 
                AFTER UPDATE ON user_preferences
                BEGIN
                    UPDATE user_preferences SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
                END";
			ExecuteNonQuery( connection, preferenceTrigger );

			Console.WriteLine( "所有触发器创建完成" );
		}



		#endregion

		#region ---------------- 辅助方法  -------------------------

		private void ExecuteNonQuery(SQLiteConnection connection, string sql)
		{
			using (var command = new SQLiteCommand( sql, connection )) {
				command.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// 插入默认数据
		/// </summary>
		//public void InsertDefaultData( )
		//{
		//	using (var connection = new SQLiteConnection( _connectionString )) {
		//		connection.Open();
		//		// 插入默认效果  已做
		//		//InsertDefaultEffects( connection );
		//		// 插入默认用户（仅用于测试）
		//		InsertDefaultUser( connection );
		//		Console.WriteLine( "默认数据插入完成" );
		//	}
		//}

		private void InsertDefaultEffects(SQLiteConnection connection)
		{
			string[] effects = {
				"亮度调整", "对比度调整", "饱和度调整", "色调调整",
				"模糊效果", "锐化效果", "阴影效果", "高光效果",
				"淡入效果", "淡出效果", "缩放效果", "旋转效果"
			};

			string[] effectTypes = {
				"brightness", "contrast", "saturation", "hue",
				"blur", "sharpen", "shadow", "highlight",
				"fade_in", "fade_out", "zoom", "rotate"
			};

			for (int i = 0; i < effects.Length; i++) {
				string checkSql = "SELECT COUNT(*) FROM effects WHERE name = @name";
				using (var checkCommand = new SQLiteCommand( checkSql, connection )) {
					checkCommand.Parameters.AddWithValue( "@name", effects[i] );
					var count = Convert.ToInt32( checkCommand.ExecuteScalar() );

					if (count == 0) {
						string insertSql = @"
                            INSERT INTO effects (name, effect_type, description)
                            VALUES (@name, @effect_type, @description)";

						using (var insertCommand = new SQLiteCommand( insertSql, connection )) {
							insertCommand.Parameters.AddWithValue( "@name", effects[i] );
							insertCommand.Parameters.AddWithValue( "@effect_type", effectTypes[i] );
							insertCommand.Parameters.AddWithValue( "@description", $"{effects[i]}效果" );
							insertCommand.ExecuteNonQuery();
						}
					}
				}
			}
		}

		private void InsertDefaultUser(SQLiteConnection connection)
		{
			string checkSql = "SELECT COUNT(*) FROM users WHERE username = 'admin'";
			using (var checkCommand = new SQLiteCommand( checkSql, connection )) {
				var count = Convert.ToInt32( checkCommand.ExecuteScalar() );

				if (count == 0) {
					string insertSql = @"
                        INSERT INTO users (username, email, passwordhash, fullname)
                        VALUES (@username, @email, @passwordhash, @full_name)";

					using (var insertCommand = new SQLiteCommand( insertSql, connection )) {
						insertCommand.Parameters.AddWithValue( "@username", "admin" );
						insertCommand.Parameters.AddWithValue( "@email", "admin@example.com" );
						insertCommand.Parameters.AddWithValue( "@passwordhash", "hashed_admin_password" );
						insertCommand.Parameters.AddWithValue( "@fullname", "系统管理员" );
						insertCommand.ExecuteNonQuery();
					}
				}
			}
		}

		#endregion
		#region  ------------  user 和 Settings  两个表的创建 初始化 和 增删改查 -------------------

		public void InitializeDatabase( )  //创建两个表  不能运行了
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();
				// 创建 users 表
				CreateUserTable( connection );
				// 创建 settings 表
				CreateSettingsTable( connection );
				Console.WriteLine( "数据库表创建完成" );
			}
		}

		private void CreateUserTable(SQLiteConnection connection)
		{
			string sql = @"
                CREATE TABLE IF NOT EXISTS users (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    username TEXT NOT NULL,
                    email TEXT NOT NULL,
                    password_hash TEXT NOT NULL,
                    iphone TEXT,
                    full_name TEXT,
                    avatar_path TEXT,
                    is_active INTEGER NOT NULL DEFAULT 1,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    draftposition TEXT,
                    is_locked INTEGER NOT NULL DEFAULT 0,
                    is_deleted INTEGER NOT NULL DEFAULT 0,
                    is_modified INTEGER NOT NULL DEFAULT 0,
                    note TEXT
                )";

			using (var command = new SQLiteCommand( sql, connection )) {
				command.ExecuteNonQuery();
			}
		}
		private void CreateSettingsTable(SQLiteConnection connection)
		{
			string sql = @"
                CREATE TABLE IF NOT EXISTS settings (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    lfdm TEXT,
                    bm INTEGER NOT NULL DEFAULT 0,
                    bms INTEGER NOT NULL DEFAULT 0,
                    bmdate INTEGER,
                    bmd TEXT,
                    bmsize TEXT,
                    autos INTEGER NOT NULL DEFAULT 0,
                    pre_setsavelocation TEXT,
                    sharereview INTEGER NOT NULL DEFAULT 0,
                    importtheproject TEXT,
                    significantadjustmentofvalues TEXT,
                    defaultdurationoftheimage TEXT,
                    targetloudness TEXT,
                    timelinesound INTEGER NOT NULL DEFAULT 0,
                    maintracklinkage TEXT,
                    freedomlevel TEXT,
                    defaultframerate TEXT,
                    timecodeformat TEXT,
                    exportingalertsound INTEGER NOT NULL DEFAULT 0,
                    sync_job_dream_materials TEXT,
                    standardizedexpression TEXT,
                    individuation TEXT,
                    codesettings INTEGER NOT NULL DEFAULT 0,
                    codesettings1 INTEGER NOT NULL DEFAULT 0,
                    interfacedesign INTEGER NOT NULL DEFAULT 0,
                    automaticre_sealing INTEGER NOT NULL DEFAULT 0,
                    locationofthepackagedfile TEXT,
                    sizeofthepackagedfile TEXT,
                    agencymode TEXT,
                    agencylocation TEXT,
                    agencysize TEXT,
                    audio_outputdevice TEXT,
                    renderfilelocation TEXT,
                    renderfilesize INTEGER,
                    startup_cleaner INTEGER,
                    automaticrendering INTEGER NOT NULL DEFAULT 0,
                    autoupdate INTEGER NOT NULL DEFAULT 0,
                    notification_settings TEXT,
                    note TEXT
                )";

			using (var command = new SQLiteCommand( sql, connection )) {
				command.ExecuteNonQuery();
			}
		}
		#endregion

		#region ----------------Users 表操作 ----------------

		// 插入用户
		public int InsertUser(Users user)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();
				/*INSERT OR IGNORE INTO 是 SQLite 特有的语法（其他数据库有类似但语法不同的实现），作用是：
				当插入的记录违反表中的唯一约束（如主键、唯一索引）时，数据库会忽略本次插入操作，不执行任何变更，也不返回错误；若没有冲突，则正常插入新记录。*/
				string sql = @"
                    INSERT OR IGNORE INTO users (
                        username, email, password_hash, iphone, full_name, avatar_path,
                        is_active, created_at, updated_at, draftposition, is_locked,
                        is_deleted, is_modified, note
                    ) VALUES (
                        @username, @email, @password_hash, @iphone, @full_name, @avatar_path,
                        @is_active, @created_at, @updated_at, @draftposition, @is_locked,
                        @is_deleted, @is_modified, @note
                    );
                    SELECT last_insert_rowid();";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@username", user.Username );
					command.Parameters.AddWithValue( "@email", user.Email );
					command.Parameters.AddWithValue( "@password_hash", user.PasswordHash );
					command.Parameters.AddWithValue( "@iphone", user.Iphone ?? "" );
					command.Parameters.AddWithValue( "@full_name", user.FullName ?? "" );
					command.Parameters.AddWithValue( "@avatar_path", user.AvatarPath ?? "" );
					command.Parameters.AddWithValue( "@is_active", user.IsActive ? 1 : 0 );
					command.Parameters.AddWithValue( "@created_at", user.CreatedAt );
					command.Parameters.AddWithValue( "@updated_at", user.UpdatedAt );
					command.Parameters.AddWithValue( "@draftposition", user.Draftposition ?? "" );
					command.Parameters.AddWithValue( "@is_locked", user.IsLocked ? 1 : 0 );
					command.Parameters.AddWithValue( "@is_deleted", user.IsDeleted ? 1 : 0 );
					command.Parameters.AddWithValue( "@is_modified", user.IsModified ? 1 : 0 );
					command.Parameters.AddWithValue( "@note", user.Note ?? "" );

					return Convert.ToInt32( command.ExecuteScalar() );
				}
			}
		}
		// 获取所有用户
		public List<Users> GetAllUsers( )
		{
			var users = new List<Users>();

			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = "SELECT * FROM users ORDER BY id";

				using (var command = new SQLiteCommand( sql, connection ))
				using (var reader = command.ExecuteReader()) {
					while (reader.Read()) {
						users.Add( new Users
						{
							Id = Convert.ToInt32( reader["id"] ),
							Username = reader["username"].ToString(),
							Email = reader["email"].ToString(),
							PasswordHash = reader["password_hash"].ToString(),
							Iphone = reader["iphone"].ToString(),
							FullName = reader["full_name"].ToString(),
							AvatarPath = reader["avatar_path"].ToString(),
							IsActive = Convert.ToInt32( reader["is_active"] ) == 1,
							CreatedAt = Convert.ToDateTime( reader["created_at"] ),
							UpdatedAt = Convert.ToDateTime( reader["updated_at"] ),
							Draftposition = reader["draftposition"].ToString(),
							IsLocked = Convert.ToInt32( reader["is_locked"] ) == 1,
							IsDeleted = Convert.ToInt32( reader["is_deleted"] ) == 1,
							IsModified = Convert.ToInt32( reader["is_modified"] ) == 1,
							Note = reader["note"].ToString()
						} );
					}
				}
			}

			return users;
		}
		// 根据ID获取用户
		public Users GetUserById(int id)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = "SELECT * FROM users WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );

					using (var reader = command.ExecuteReader()) {
						if (reader.Read()) {
							return new Users
							{
								Id = Convert.ToInt32( reader["id"] ),
								Username = reader["username"].ToString(),
								Email = reader["email"].ToString(),
								PasswordHash = reader["password_hash"].ToString(),
								Iphone = reader["iphone"].ToString(),
								FullName = reader["full_name"].ToString(),
								AvatarPath = reader["avatar_path"].ToString(),
								IsActive = Convert.ToInt32( reader["is_active"] ) == 1,
								CreatedAt = Convert.ToDateTime( reader["created_at"] ),
								UpdatedAt = Convert.ToDateTime( reader["updated_at"] ),
								Draftposition = reader["draftposition"].ToString(),
								IsLocked = Convert.ToInt32( reader["is_locked"] ) == 1,
								IsDeleted = Convert.ToInt32( reader["is_deleted"] ) == 1,
								IsModified = Convert.ToInt32( reader["is_modified"] ) == 1,
								Note = reader["note"].ToString()
							};
						}
					}
				}
			}

			return null;
		}
		// 更新用户
		public bool UpdateUser(Users user)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    UPDATE users SET
                        username = @username,
                        email = @email,
                        password_hash = @password_hash,
                        iphone = @iphone,
                        full_name = @full_name,
                        avatar_path = @avatar_path,
                        is_active = @is_active,
                        updated_at = @updated_at,
                        draftposition = @draftposition,
                        is_locked = @is_locked,
                        is_deleted = @is_deleted,
                        is_modified = @is_modified,
                        note = @note
                    WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", user.Id );
					command.Parameters.AddWithValue( "@username", user.Username );
					command.Parameters.AddWithValue( "@email", user.Email );
					command.Parameters.AddWithValue( "@password_hash", user.PasswordHash );
					command.Parameters.AddWithValue( "@iphone", user.Iphone ?? "" );
					command.Parameters.AddWithValue( "@full_name", user.FullName ?? "" );
					command.Parameters.AddWithValue( "@avatar_path", user.AvatarPath ?? "" );
					command.Parameters.AddWithValue( "@is_active", user.IsActive ? 1 : 0 );
					command.Parameters.AddWithValue( "@updated_at", DateTime.Now );
					command.Parameters.AddWithValue( "@draftposition", user.Draftposition ?? "" );
					command.Parameters.AddWithValue( "@is_locked", user.IsLocked ? 1 : 0 );
					command.Parameters.AddWithValue( "@is_deleted", user.IsDeleted ? 1 : 0 );
					command.Parameters.AddWithValue( "@is_modified", user.IsModified ? 1 : 0 );
					command.Parameters.AddWithValue( "@note", user.Note ?? "" );

					return command.ExecuteNonQuery() > 0;
				}
			}
		}
		// 删除用户
		public bool DeleteUser(int id)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = "DELETE FROM users WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );
					return command.ExecuteNonQuery() > 0;
				}
			}
		}

		#endregion

		#region---------------- Settings 表操作 ----------------

		// 插入设置
		public int InsertSettings(Settings settings)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    INSERT INTO settings (
                        lfdm, bm, bms, bmdate, bmd, bmsize, autos, pre_setsavelocation,
                        sharereview, importtheproject, significantadjustmentofvalues,
                        defaultdurationoftheimage, targetloudness, timelinesound,
                        maintracklinkage, freedomlevel, defaultframerate, timecodeformat,
                        exportingalertsound, sync_job_dream_materials, standardizedexpression,
                        individuation, codesettings, codesettings1, interfacedesign,
                        automaticre_sealing, locationofthepackagedfile, sizeofthepackagedfile,
                        agencymode, agencylocation, agencysize, audio_outputdevice,
                        renderfilelocation, renderfilesize, startup_cleaner,
                        automaticrendering, autoupdate, notification_settings, note
                    ) VALUES (
                        @lfdm, @bm, @bms, @bmdate, @bmd, @bmsize, @autos, @pre_setsavelocation,
                        @sharereview, @importtheproject, @significantadjustmentofvalues,
                        @defaultdurationoftheimage, @targetloudness, @timelinesound,
                        @maintracklinkage, @freedomlevel, @defaultframerate, @timecodeformat,
                        @exportingalertsound, @sync_job_dream_materials, @standardizedexpression,
                        @individuation, @codesettings, @codesettings1, @interfacedesign,
                        @automaticre_sealing, @locationofthepackagedfile, @sizeofthepackagedfile,
                        @agencymode, @agencylocation, @agencysize, @audio_outputdevice,
                        @renderfilelocation, @renderfilesize, @startup_cleaner,
                        @automaticrendering, @autoupdate, @notification_settings, @note
                    );
                    SELECT last_insert_rowid();";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@lfdm", settings.Lfdm ?? "" );
					command.Parameters.AddWithValue( "@bm", settings.Bm ? 1 : 0 );
					command.Parameters.AddWithValue( "@bms", settings.Bms ? 1 : 0 );
					command.Parameters.AddWithValue( "@bmdate", settings.Bmdate );
					command.Parameters.AddWithValue( "@bmd", settings.Bmd ?? "" );
					command.Parameters.AddWithValue( "@bmsize", settings.Bmsize ?? "" );
					command.Parameters.AddWithValue( "@autos", settings.Autos ? 1 : 0 );
					command.Parameters.AddWithValue( "@pre_setsavelocation", settings.PreSetsavelocation ?? "" );
					command.Parameters.AddWithValue( "@sharereview", settings.Sharereview ? 1 : 0 );
					command.Parameters.AddWithValue( "@importtheproject", settings.Importtheproject ?? "" );
					command.Parameters.AddWithValue( "@significantadjustmentofvalues", settings.Significantadjustmentofvalues ?? "" );
					command.Parameters.AddWithValue( "@defaultdurationoftheimage", settings.Defaultdurationoftheimage ?? "" );
					command.Parameters.AddWithValue( "@targetloudness", settings.Targetloudness ?? "" );
					command.Parameters.AddWithValue( "@timelinesound", settings.Timelinesound ? 1 : 0 );
					command.Parameters.AddWithValue( "@maintracklinkage", settings.Maintracklinkage ?? "" );
					command.Parameters.AddWithValue( "@freedomlevel", settings.Freedomlevel ?? "" );
					command.Parameters.AddWithValue( "@defaultframerate", settings.Defaultframerate ?? "" );
					command.Parameters.AddWithValue( "@timecodeformat", settings.Timecodeformat ?? "" );
					command.Parameters.AddWithValue( "@exportingalertsound", settings.Exportingalertsound ? 1 : 0 );
					command.Parameters.AddWithValue( "@sync_job_dream_materials", settings.SyncJobDreamMaterials ?? "" );
					command.Parameters.AddWithValue( "@standardizedexpression", settings.Standardizedexpression ?? "" );
					command.Parameters.AddWithValue( "@individuation", settings.Individuation ?? "" );
					command.Parameters.AddWithValue( "@codesettings", settings.Codesettings ? 1 : 0 );
					command.Parameters.AddWithValue( "@codesettings1", settings.Codesettings1 ? 1 : 0 );
					command.Parameters.AddWithValue( "@interfacedesign", settings.Interfacedesign ? 1 : 0 );
					command.Parameters.AddWithValue( "@automaticre_sealing", settings.AutomaticreSealing ? 1 : 0 );
					command.Parameters.AddWithValue( "@locationofthepackagedfile", settings.Locationofthepackagedfile ?? "" );
					command.Parameters.AddWithValue( "@sizeofthepackagedfile", settings.Sizeofthepackagedfile ?? "" );
					command.Parameters.AddWithValue( "@agencymode", settings.Agencymode ?? "" );
					command.Parameters.AddWithValue( "@agencylocation", settings.Agencylocation ?? "" );
					command.Parameters.AddWithValue( "@agencysize", settings.Agencysize ?? "" );
					command.Parameters.AddWithValue( "@audio_outputdevice", settings.AudioOutputdevice ?? "" );
					command.Parameters.AddWithValue( "@renderfilelocation", settings.Renderfilelocation ?? "" );
					command.Parameters.AddWithValue( "@renderfilesize", settings.Renderfilesize );
					command.Parameters.AddWithValue( "@startup_cleaner", settings.StartupCleaner );
					command.Parameters.AddWithValue( "@automaticrendering", settings.Automaticrendering ? 1 : 0 );
					command.Parameters.AddWithValue( "@autoupdate", settings.Autoupdate ? 1 : 0 );
					command.Parameters.AddWithValue( "@notification_settings", settings.NotificationSettings ?? "" );
					command.Parameters.AddWithValue( "@note", settings.Note ?? "" );

					return Convert.ToInt32( command.ExecuteScalar() );
				}
			}
		}

		// 获取所有设置
		public List<Settings> GetAllSettings( )
		{
			var settingsList = new List<Settings>();

			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = "SELECT * FROM settings ORDER BY id";

				using (var command = new SQLiteCommand( sql, connection ))
				using (var reader = command.ExecuteReader()) {
					while (reader.Read()) {
						settingsList.Add( CreateSettingsFromReader( reader ) );
					}
				}
			}

			return settingsList;
		}

		// 根据ID获取设置
		public Settings GetSettingsById(int id)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = "SELECT * FROM settings WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );

					using (var reader = command.ExecuteReader()) {
						if (reader.Read()) {
							return CreateSettingsFromReader( reader );
						}
					}
				}
			}

			return null;
		}

		// 更新设置
		public bool UpdateSettings(Settings settings)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    UPDATE settings SET
                        lfdm = @lfdm,
                        bm = @bm,
                        bms = @bms,
                        bmdate = @bmdate,
                        bmd = @bmd,
                        bmsize = @bmsize,
                        autos = @autos,
                        pre_setsavelocation = @pre_setsavelocation,
                        sharereview = @sharereview,
                        importtheproject = @importtheproject,
                        significantadjustmentofvalues = @significantadjustmentofvalues,
                        defaultdurationoftheimage = @defaultdurationoftheimage,
                        targetloudness = @targetloudness,
                        timelinesound = @timelinesound,
                        maintracklinkage = @maintracklinkage,
                        freedomlevel = @freedomlevel,
                        defaultframerate = @defaultframerate,
                        timecodeformat = @timecodeformat,
                        exportingalertsound = @exportingalertsound,
                        sync_job_dream_materials = @sync_job_dream_materials,
                        standardizedexpression = @standardizedexpression,
                        individuation = @individuation,
                        codesettings = @codesettings,
                        codesettings1 = @codesettings1,
                        interfacedesign = @interfacedesign,
                        automaticre_sealing = @automaticre_sealing,
                        locationofthepackagedfile = @locationofthepackagedfile,
                        sizeofthepackagedfile = @sizeofthepackagedfile,
                        agencymode = @agencymode,
                        agencylocation = @agencylocation,
                        agencysize = @agencysize,
                        audio_outputdevice = @audio_outputdevice,
                        renderfilelocation = @renderfilelocation,
                        renderfilesize = @renderfilesize,
                        startup_cleaner = @startup_cleaner,
                        automaticrendering = @automaticrendering,
                        autoupdate = @autoupdate,
                        notification_settings = @notification_settings,
                        note = @note
                    WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", settings.Id );
					command.Parameters.AddWithValue( "@lfdm", settings.Lfdm ?? "" );
					command.Parameters.AddWithValue( "@bm", settings.Bm ? 1 : 0 );
					command.Parameters.AddWithValue( "@bms", settings.Bms ? 1 : 0 );
					command.Parameters.AddWithValue( "@bmdate", settings.Bmdate );
					command.Parameters.AddWithValue( "@bmd", settings.Bmd ?? "" );
					command.Parameters.AddWithValue( "@bmsize", settings.Bmsize ?? "" );
					command.Parameters.AddWithValue( "@autos", settings.Autos ? 1 : 0 );
					command.Parameters.AddWithValue( "@pre_setsavelocation", settings.PreSetsavelocation ?? "" );
					command.Parameters.AddWithValue( "@sharereview", settings.Sharereview ? 1 : 0 );
					command.Parameters.AddWithValue( "@importtheproject", settings.Importtheproject ?? "" );
					command.Parameters.AddWithValue( "@significantadjustmentofvalues", settings.Significantadjustmentofvalues ?? "" );
					command.Parameters.AddWithValue( "@defaultdurationoftheimage", settings.Defaultdurationoftheimage ?? "" );
					command.Parameters.AddWithValue( "@targetloudness", settings.Targetloudness ?? "" );
					command.Parameters.AddWithValue( "@timelinesound", settings.Timelinesound ? 1 : 0 );
					command.Parameters.AddWithValue( "@maintracklinkage", settings.Maintracklinkage ?? "" );
					command.Parameters.AddWithValue( "@freedomlevel", settings.Freedomlevel ?? "" );
					command.Parameters.AddWithValue( "@defaultframerate", settings.Defaultframerate ?? "" );
					command.Parameters.AddWithValue( "@timecodeformat", settings.Timecodeformat ?? "" );
					command.Parameters.AddWithValue( "@exportingalertsound", settings.Exportingalertsound ? 1 : 0 );
					command.Parameters.AddWithValue( "@sync_job_dream_materials", settings.SyncJobDreamMaterials ?? "" );
					command.Parameters.AddWithValue( "@standardizedexpression", settings.Standardizedexpression ?? "" );
					command.Parameters.AddWithValue( "@individuation", settings.Individuation ?? "" );
					command.Parameters.AddWithValue( "@codesettings", settings.Codesettings ? 1 : 0 );
					command.Parameters.AddWithValue( "@codesettings1", settings.Codesettings1 ? 1 : 0 );
					command.Parameters.AddWithValue( "@interfacedesign", settings.Interfacedesign ? 1 : 0 );
					command.Parameters.AddWithValue( "@automaticre_sealing", settings.AutomaticreSealing ? 1 : 0 );
					command.Parameters.AddWithValue( "@locationofthepackagedfile", settings.Locationofthepackagedfile ?? "" );
					command.Parameters.AddWithValue( "@sizeofthepackagedfile", settings.Sizeofthepackagedfile ?? "" );
					command.Parameters.AddWithValue( "@agencymode", settings.Agencymode ?? "" );
					command.Parameters.AddWithValue( "@agencylocation", settings.Agencylocation ?? "" );
					command.Parameters.AddWithValue( "@agencysize", settings.Agencysize ?? "" );
					command.Parameters.AddWithValue( "@audio_outputdevice", settings.AudioOutputdevice ?? "" );
					command.Parameters.AddWithValue( "@renderfilelocation", settings.Renderfilelocation ?? "" );
					command.Parameters.AddWithValue( "@renderfilesize", settings.Renderfilesize );
					command.Parameters.AddWithValue( "@startup_cleaner", settings.StartupCleaner );
					command.Parameters.AddWithValue( "@automaticrendering", settings.Automaticrendering ? 1 : 0 );
					command.Parameters.AddWithValue( "@autoupdate", settings.Autoupdate ? 1 : 0 );
					command.Parameters.AddWithValue( "@notification_settings", settings.NotificationSettings ?? "" );
					command.Parameters.AddWithValue( "@note", settings.Note ?? "" );

					return command.ExecuteNonQuery() > 0;
				}
			}
		}

		// 删除设置
		public bool DeleteSettings(int id)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = "DELETE FROM settings WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );
					return command.ExecuteNonQuery() > 0;
				}
			}
		}

		// 从DataReader创建Settings对象
		private Settings CreateSettingsFromReader(SQLiteDataReader reader)
		{
			return new Settings
			{
				Id = Convert.ToInt32( reader["id"] ),
				Lfdm = reader["lfdm"].ToString(),
				Bm = Convert.ToInt32( reader["bm"] ) == 1,
				Bms = Convert.ToInt32( reader["bms"] ) == 1,
				Bmdate = Convert.ToInt32( reader["bmdate"] ),
				Bmd = reader["bmd"].ToString(),
				Bmsize = reader["bmsize"].ToString(),
				Autos = Convert.ToInt32( reader["autos"] ) == 1,
				PreSetsavelocation = reader["pre_setsavelocation"].ToString(),
				Sharereview = Convert.ToInt32( reader["sharereview"] ) == 1,
				Importtheproject = reader["importtheproject"].ToString(),
				Significantadjustmentofvalues = reader["significantadjustmentofvalues"].ToString(),
				Defaultdurationoftheimage = reader["defaultdurationoftheimage"].ToString(),
				Targetloudness = reader["targetloudness"].ToString(),
				Timelinesound = Convert.ToInt32( reader["timelinesound"] ) == 1,
				Maintracklinkage = reader["maintracklinkage"].ToString(),
				Freedomlevel = reader["freedomlevel"].ToString(),
				Defaultframerate = reader["defaultframerate"].ToString(),
				Timecodeformat = reader["timecodeformat"].ToString(),
				Exportingalertsound = Convert.ToInt32( reader["exportingalertsound"] ) == 1,
				SyncJobDreamMaterials = reader["sync_job_dream_materials"].ToString(),
				Standardizedexpression = reader["standardizedexpression"].ToString(),
				Individuation = reader["individuation"].ToString(),
				Codesettings = Convert.ToInt32( reader["codesettings"] ) == 1,
				Codesettings1 = Convert.ToInt32( reader["codesettings1"] ) == 1,
				Interfacedesign = Convert.ToInt32( reader["interfacedesign"] ) == 1,
				AutomaticreSealing = Convert.ToInt32( reader["automaticre_sealing"] ) == 1,
				Locationofthepackagedfile = reader["locationofthepackagedfile"].ToString(),
				Sizeofthepackagedfile = reader["sizeofthepackagedfile"].ToString(),
				Agencymode = reader["agencymode"].ToString(),
				Agencylocation = reader["agencylocation"].ToString(),
				Agencysize = reader["agencysize"].ToString(),
				AudioOutputdevice = reader["audio_outputdevice"].ToString(),
				Renderfilelocation = reader["renderfilelocation"].ToString(),
				Renderfilesize = Convert.ToInt32( reader["renderfilesize"] ),
				StartupCleaner = Convert.ToInt32( reader["startup_cleaner"] ),
				Automaticrendering = Convert.ToInt32( reader["automaticrendering"] ) == 1,
				Autoupdate = Convert.ToInt32( reader["autoupdate"] ) == 1,
				NotificationSettings = reader["notification_settings"].ToString(),
				Note = reader["note"].ToString()
			};
		}

		#endregion

		public void dbinit( )  // 创建所有表  已做
		{
			try {
				var dbInitializer = new db( dbPath );
				//dbInitializer.InsertDefaultData(); 	// 插入默认数据   		//dbInitializer.InitializeDatabase();  InsertDefaultData  插入 users
				//Console.WriteLine( "数据库初始化成功！" );  
				Users user = new Users();
				user.Username = "YuanDing";
				dbInitializer.InsertUser( user );
				// 测试数据库连接  				TestDatabaseConnection( "video_editor.db" );
			}
			catch (Exception ex) {
				Console.WriteLine( $"数据库初始化失败: {ex.Message}" );
				Console.WriteLine( $"详细信息: {ex.StackTrace}" );
			}
		}
	} //class  db

	#region --------- clip 数据模型类（对应数据 表结构  ） ------------

	//namespace VideoEditor.Database.Models {
	public class Project
	{
		public int Id
		{
			get; set;
		}
		public int UserId
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
		public int Width
		{
			get; set;
		}
		public int Height
		{
			get; set;
		}
		public double Framerate
		{
			get; set;
		}
		public double Duration
		{
			get; set;
		}
		public string ThumbnailPath    //缩略图路径
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
		public string Note
		{
			get; set;
		}


	}
	public class MediaAsset
	{
		public int Id
		{
			get; set;
		}
		public int UserId
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
		public long FileSize
		{
			get; set;
		}
		public string MediaType
		{
			get; set;
		}
		public double? Duration
		{
			get; set;
		}
		public int? Width
		{
			get; set;
		}
		public int? Height
		{
			get; set;
		}
		public double? Framerate
		{
			get; set;
		}
		public string Codec
		{
			get; set;
		}
		public DateTime CreatedAt
		{
			get; set;
		}
		public DateTime Updated_at
		{
			get; set;
		}
		public string Note
		{
			get; set;
		}
	}
	public class TimelineTrack
	{
		public int Id
		{
			get; set;
		}
		public int ProjectId
		{
			get; set;
		}
		public string TrackType
		{
			get; set;
		}
		public int TrackIndex
		{
			get; set;
		}
		public string Name
		{
			get; set;
		}
		public bool IsMuted
		{
			get; set;
		}
		public bool IsLocked
		{
			get; set;
		}
		public double Volume
		{
			get; set;
		}
		public DateTime CreatedAt
		{
			get; set;
		}
		public DateTime Updated_at
		{
			get; set;
		}
		public string Note
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
		public int ProjectId
		{
			get; set;
		}
		public int TrackId
		{
			get; set;
		}
		public int? MediaAssetId
		{
			get; set;
		}
		public string Name
		{
			get; set;
		}
		public double StartTime
		{
			get; set;
		}
		public double EndTime
		{
			get; set;
		}
		public double MediaStartTime
		{
			get; set;
		}
		public double MediaEndTime
		{
			get; set;
		}
		public double PositionX
		{
			get; set;
		}
		public double PositionY
		{
			get; set;
		}
		public double ScaleX
		{
			get; set;
		}
		public double ScaleY
		{
			get; set;
		}
		public double Rotation
		{
			get; set;
		}
		public double Volume
		{
			get; set;
		}
		public bool IsMuted
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
		public string Note
		{
			get; set;
		}
	}
	public class Transaction
	{
		public int Id
		{
			get; set;
		}
		public int ProjectId
		{
			get; set;
		}
		public int UserId
		{
			get; set;
		}
		public string TransactionType
		{
			get; set;
		}
		public string Description
		{
			get; set;
		}
		public DateTime Timestamp
		{
			get; set;
		}
		public bool IsUndone
		{
			get; set;
		}
		public bool is_redone
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
		public string Note
		{
			get; set;
		}
	}
	public class TransactionDetail
	{
		public int Id
		{
			get; set;
		}
		public int TransactionId
		{
			get; set;
		}
		public string OperationType
		{
			get; set;
		}
		public string TableName
		{
			get; set;
		}
		public int RecordId
		{
			get; set;
		}
		public string OldValues
		{
			get; set;
		}
		public string NewValues
		{
			get; set;
		}
		public DateTime CreatedAt
		{
			get; set;
		}
		public string Note
		{
			get; set;
		}
	}
	public class Effect
	{
		public int Id
		{
			get; set;
		}
		public string Name
		{
			get; set;
		}
		public string EffectType
		{
			get; set;
		}
		public string Description
		{
			get; set;
		}
		public string Parameters
		{
			get; set;
		}
		public DateTime CreatedAt
		{
			get; set;
		}
		public string Note
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
		public int EffectId
		{
			get; set;
		}
		public string Parameters
		{
			get; set;
		}
		public double StartTime
		{
			get; set;
		}
		public double EndTime
		{
			get; set;
		}
		public int OrderIndex
		{
			get; set;
		}
		public bool IsEnabled
		{
			get; set;
		}
		public DateTime CreatedAt
		{
			get; set;
		}
		public string Note
		{
			get; set;
		}
	}
	public class Projectold
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
	public class Clipold
	{
		public int Id
		{
			get; set;
		}
		public string Name
		{
			get; set;
		}
		public string Type  // mp3, wav, etc.
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
	public class ClipEffectold
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

	public class Users
	{
		public int Id
		{
			get; set;
		}
		public string Username
		{
			get; set;
		}
		public string Email
		{
			get; set;
		}
		public string PasswordHash
		{
			get; set;
		}
		public string Iphone
		{
			get; set;
		}
		public string FullName
		{
			get; set;
		}
		public string AvatarPath
		{
			get; set;
		}
		public bool IsActive { get; set; } = true;
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime UpdatedAt { get; set; } = DateTime.Now;
		public string Draftposition
		{
			get; set;
		}
		public bool IsLocked
		{
			get; set;
		}
		public bool IsDeleted
		{
			get; set;
		}
		public bool IsModified
		{
			get; set;
		}
		public string Note
		{
			get; set;
		}
	}
	public class Settings
	{
		public int Id
		{
			get; set;
		}
		public string Lfdm
		{
			get; set;
		}
		public bool Bm
		{
			get; set;
		}
		public bool Bms
		{
			get; set;
		}
		public int Bmdate
		{
			get; set;
		}
		public string Bmd
		{
			get; set;
		}
		public string Bmsize
		{
			get; set;
		}
		public bool Autos
		{
			get; set;
		}
		public string PreSetsavelocation
		{
			get; set;
		}
		public bool Sharereview
		{
			get; set;
		}
		public string Importtheproject
		{
			get; set;
		}
		public string Significantadjustmentofvalues
		{
			get; set;
		}
		public string Defaultdurationoftheimage
		{
			get; set;
		}
		public string Targetloudness
		{
			get; set;
		}
		public bool Timelinesound
		{
			get; set;
		}
		public string Maintracklinkage
		{
			get; set;
		}
		public string Freedomlevel
		{
			get; set;
		}
		public string Defaultframerate
		{
			get; set;
		}
		public string Timecodeformat
		{
			get; set;
		}
		public bool Exportingalertsound
		{
			get; set;
		}
		public string SyncJobDreamMaterials
		{
			get; set;
		}
		public string Standardizedexpression
		{
			get; set;
		}
		public string Individuation
		{
			get; set;
		}
		public bool Codesettings
		{
			get; set;
		}
		public bool Codesettings1
		{
			get; set;
		}
		public bool Interfacedesign
		{
			get; set;
		}
		public bool AutomaticreSealing
		{
			get; set;
		}
		public string Locationofthepackagedfile
		{
			get; set;
		}
		public string Sizeofthepackagedfile
		{
			get; set;
		}
		public string Agencymode
		{
			get; set;
		}
		public string Agencylocation
		{
			get; set;
		}
		public string Agencysize
		{
			get; set;
		}
		public string AudioOutputdevice
		{
			get; set;
		}
		public string Renderfilelocation
		{
			get; set;
		}
		public int Renderfilesize
		{
			get; set;
		}
		public int StartupCleaner
		{
			get; set;
		}
		public bool Automaticrendering
		{
			get; set;
		}
		public bool Autoupdate
		{
			get; set;
		}
		public string NotificationSettings
		{
			get; set;
		}
		public string Note
		{
			get; set;
		}
	}
}



#endregion




