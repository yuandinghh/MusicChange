using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using System.Windows;
using MusicChange.VideoEditor.Database;

namespace MusicChange
{
	// 数据库操作封装类
	public class db
	{
		// 数据库连接字符串（建议使用绝对路径避免路径问题）
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


		#region ---------创建一个 clip 剪辑数据库的详细表，并给出c# sqlite 程序  ------------

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
		/// InitializeDatabase  创建数据库，创建表
		/// </summary>
		public void InitializeDatabase( )
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
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

				using (var command = new SQLiteCommand( createProjectsTable, connection )) {
					command.ExecuteNonQuery();
				}

				// 创建 clips 表
				string createClipsTable = @"
                    CREATE TABLE IF NOT EXISTS clips (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        name TEXT NOT NULL,
						type TEXT NOT NULL,
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

				using (var command = new SQLiteCommand( createClipsTable, connection )) {
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
				using (var command = new SQLiteCommand( createClipEffectsTable, connection )) {
					command.ExecuteNonQuery();
				}
			}
		}
		/// <summary>
		///  Projects 表操作
		/// </summary>
		/// <param name="project"></param>
		/// <returns></returns>
		public int InsertProject(Project project)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
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

		#region -------剪辑 User 表 处理  ------------

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
	full_name	TEXT					真实姓名
	avatar_path	TEXT					头像路径
	is_active	INTEGER	NOT NULL DEFAULT 1	是否激活
	created_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	创建时间
	updated_at	DATETIME	NOT NULL DEFAULT CURRENT_TIMESTAMP	更新时间
	annotations TEXT										用户注释信息		
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

				CreateUserTable( connection );
				CreateUserProfileTable( connection );
				CreateUserSessionTable( connection );
				CreateUserPreferencesTable( connection );
				CreateUserLoginTable( connection );

				Console.WriteLine( "用户数据库表创建完成" );
			}
		}

		private void CreateUserTable(SQLiteConnection connection)
		{
			string createUsersTable = @"
                CREATE TABLE IF NOT EXISTS users (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    username TEXT NOT NULL UNIQUE,
                    email TEXT NOT NULL UNIQUE,
                    password_hash TEXT NOT NULL,
                    full_name TEXT,
                    avatar_path TEXT,
                    is_active INTEGER NOT NULL DEFAULT 1,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
					annotations TEXT
                )";

			using (var command = new SQLiteCommand( createUsersTable, connection )) {
				command.ExecuteNonQuery();
			}

			// 创建用户名索引
			string createUsernameIndex = @"
                CREATE INDEX IF NOT EXISTS idx_users_username 
                ON users (username)";

			using (var command = new SQLiteCommand( createUsernameIndex, connection )) {
				command.ExecuteNonQuery();
			}

			// 创建邮箱索引
			string createEmailIndex = @"
                CREATE INDEX IF NOT EXISTS idx_users_email 
                ON users (email)";

			using (var command = new SQLiteCommand( createEmailIndex, connection )) {
				command.ExecuteNonQuery();
			}
		}

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

		private void CreateUserPreferencesTable(SQLiteConnection connection)
		{
			string createPreferencesTable = @"
                CREATE TABLE IF NOT EXISTS user_preferences (
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
			string createUserPrefIndex = @"
                CREATE INDEX IF NOT EXISTS idx_user_preferences 
                ON user_preferences (user_id, preference_key)";

			using (var command = new SQLiteCommand( createUserPrefIndex, connection )) {
				command.ExecuteNonQuery();
			}
		}

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
					note TEXT,
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
					updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
					note TEXT,
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
					updated_at DATETIME  DEFAULT CURRENT_TIMESTAMP,
					note TEXT,
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
					note TEXT,
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
					is_redone INTEGER NOT NULL DEFAULT 0,
					created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
					updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
					note TEXT,
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
					note TEXT,
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
					note TEXT

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
					note TEXT,
                    FOREIGN KEY (clip_id) REFERENCES clips (id),
                    FOREIGN KEY (effect_id) REFERENCES effects (id)
                )";

			using (var command = new SQLiteCommand( sql, connection )) {
				command.ExecuteNonQuery();
			}
		}

		private void CreateIndexes(SQLiteConnection connection)
		{
			// 项目索引
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
		//namespace VideoEditor.Database {public class TransactionManager		{
		/* 
		这个事务表设计提供了以下功能：
1.	完整的项目结构管理 - 包括项目、媒体资源、时间线轨道和剪辑片段
2.	事务跟踪 - 记录所有用户操作，支持撤销/重做功能
3.	效果系统 - 支持为剪辑添加各种效果
4.	索引优化 - 为常用查询字段创建索引
5.	自动时间戳 - 使用触发器自动更新时间戳
6.	数据完整性 - 使用外键约束保证数据一致性
通过这些表结构，视频剪辑软件可以完整地记录用户的操作历史，实现_undo/redo_功能，并且可以追踪项目的完整变更历史。
		*/
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

		public void RecordTransactionDetail(int transactionId, string operationType, string tableName,int recordId, string oldValues, string newValues)
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

			using System;
			using VideoEditor.Database;

namespace VideoEditor
	{
		class Program
		{
			static void Main(string[] args)
			{
				try {
					// 初始化数据库
					var dbInitializer = new DatabaseInitializer( "video_editor.db" );

					// 创建所有表
					dbInitializer.InitializeAllTables();

					// 插入默认数据
					dbInitializer.InsertDefaultData();

					Console.WriteLine( "数据库初始化成功！" );

					// 测试数据库连接
					TestDatabaseConnection( "video_editor.db" );
				}
				catch (Exception ex) {
					Console.WriteLine( $"数据库初始化失败: {ex.Message}" );
					Console.WriteLine( $"详细信息: {ex.StackTrace}" );
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
		}
	}using System;
using VideoEditor.Database;

namespace VideoEditor
	{
		class Program
		{
			static void Main(string[] args)
			{
				try {
					// 初始化数据库
					var dbInitializer = new DatabaseInitializer( "video_editor.db" );

					// 创建所有表
					dbInitializer.InitializeAllTables();

					// 插入默认数据
					dbInitializer.InsertDefaultData();

					Console.WriteLine( "数据库初始化成功！" );

					// 测试数据库连接
					TestDatabaseConnection( "video_editor.db" );
				}
				catch (Exception ex) {
					Console.WriteLine( $"数据库初始化失败: {ex.Message}" );
					Console.WriteLine( $"详细信息: {ex.StackTrace}" );
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
		}
	}

			#endregion
			#region  ------------  1.	创建所有事务表 - 按正确的依赖顺序创建所有11个表 -------------------
			using System;
			using System.Data.SQLite;

namespace VideoEditor.Database
	{
		public class DatabaseInitializer
		{
			private readonly string _connectionString;

			public DatabaseInitializer(string databasePath)
			{
				_connectionString = $"Data Source={databasePath};Version=3;";
			}

			/// <summary>
			/// 初始化所有数据库表
			/// </summary>
			public void InitializeAllTables( )
			{
				using (var connection = new SQLiteConnection( _connectionString )) {
					connection.Open();

					// 按依赖顺序创建表
					CreateUserTable( connection );
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

			#region 表创建方法

			private void CreateUserTable(SQLiteConnection connection)
			{
				string sql = @"
                CREATE TABLE IF NOT EXISTS users (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    username TEXT NOT NULL UNIQUE,
                    email TEXT NOT NULL UNIQUE,
                    password_hash TEXT NOT NULL,
					iphone TEXT UNIQUE,
                    full_name TEXT,
                    avatar_path TEXT,
                    is_active INTEGER NOT NULL DEFAULT 1,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
					draftposition   TEXT,   //草稿位置
					lfdm			TEXT,				//素材下载位置  Location for downloading materials
					bm				BOOL,  //buffer management 缓存管理  true 需要缓存
					bms				BOOL,  //self buffer management auto 缓存管理  true 需要缓存	
					bmdate			INT,  //天数 buffer management auto 缓存管理  true 需要缓存	
					bmd			TEXT,		//  缓存管理 路径
					bmsize			TEXT,		//  缓存管理 大小
					autos    BOOL,	//自动保存备份
					pre-setsavelocation TEXT,	//预设保存位置
					sharereview BOOL,	//分享预览
					importtheproject TEXT,  //导入项目位置   导入工程
					significantadjustmentofvalues TEXT,  //数值大幅调节
					defaultdurationoftheimage TEXT,  //图片默认时长
					targetloudness TEXT,  //目标响度  默认 - 2BLUFS
					timelinesound BOOL,  //时间轴声音 开关（当前关闭，描述为 “拖动时间轴时，同步播放声音” ）
					maintracklinkage TEXT,  //主轨联动：显示 “联动设置”，已联动文本、特效、贴纸、滤镜、调节、音效、文本朗读片段
					freedomlevel TEXT,  //自由层级：勾选 “新建草稿时，默认开启自由层级”
					defaultframerate TEXT,  // 默认帧率：30.00 帧 / 秒
					timecodeformat TEXT,  // 时码样式：HH:MM:SS + 帧
					exportingalertsound BOOL,  //导出提示音：开关（当前关闭，描述 “启用后导出成功或失败时有提示音效” ）
					syncJobDreamMaterials TEXT,  // 同步职梦素材：勾选 “启用后自动同步职梦 ai 生成的素材”
					standardizedexpression TEXT,  //规范表达：勾选 “启用规范表达提示，自动识别错别字”
					individuation TEXT,  //个性化：勾选 “启用个性化推荐，自动分析用户偏好”

					note TEXT
                )";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.ExecuteNonQuery();
				}

				// 创建索引
				ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_users_username ON users (username)" );
				ExecuteNonQuery( connection, "CREATE INDEX IF NOT EXISTS idx_users_email ON users (email)" );
			}

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

			#region 索引创建方法

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

			#region 触发器创建方法

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

			#region 辅助方法

			private void ExecuteNonQuery(SQLiteConnection connection, string sql)
			{
				using (var command = new SQLiteCommand( sql, connection )) {
					command.ExecuteNonQuery();
				}
			}

			/// <summary>
			/// 插入默认数据
			/// </summary>
			public void InsertDefaultData( )
			{
				using (var connection = new SQLiteConnection( _connectionString )) {
					connection.Open();

					// 插入默认效果
					InsertDefaultEffects( connection );

					// 插入默认用户（仅用于测试）
					InsertDefaultUser( connection );

					Console.WriteLine( "默认数据插入完成" );
				}
			}

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
                        INSERT INTO users (username, email, password_hash, full_name)
                        VALUES (@username, @email, @password_hash, @full_name)";

						using (var insertCommand = new SQLiteCommand( insertSql, connection )) {
							insertCommand.Parameters.AddWithValue( "@username", "admin" );
							insertCommand.Parameters.AddWithValue( "@email", "admin@example.com" );
							insertCommand.Parameters.AddWithValue( "@password_hash", "hashed_admin_password" );
							insertCommand.Parameters.AddWithValue( "@full_name", "系统管理员" );
							insertCommand.ExecuteNonQuery();
						}
					}
				}
			}

			#endregion
		}
	}

	#endregion

} //class  db



		#region --------- clip 数据模型类（对应数据��表结构） ------------

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
	}
}
	
	#endregion




