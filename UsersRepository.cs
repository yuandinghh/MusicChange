using System;
using System.Collections.Generic;
using System.Data.SQLite;
using static MusicChange.db;

namespace MusicChange
{
	/// <summary>
	/// 独立的 Users 表增删改查仓库类
	/// 构造时传入数据库文件路径（例如 db.dbPath），内部会设置 _connectionString。
	/// </summary>
	public class UsersRepository
	{
		private readonly string _connectionString;
		public UsersRepository(string dbPath)
		{
			// 使用与现有 db 类相同的连接字符串变量（保持项目内一致性）
			_connectionString = $"Data Source={dbPath};Version=3;";
		}
//主程序如何在用户注册时将用户信息插入到数据库的 Users 表中？，设计一个 窗口 输入用户信息
		// Create: 插入用户，返回新记录的 id
		public int Create(Users user)
		{
			//_connectionString = $"Data Source={D:\\Documents\\Visual Studio 2022\\MusicChange\\LaserEditing.db};Version=3;";
			using var connection = new SQLiteConnection(_connectionString);
			connection.Open();

			string sql = @"
                INSERT INTO users 
                    (username, email, password_hash, iphone, full_name, avatar_path, is_active, created_at, updated_at, draftposition, is_locked, is_deleted, is_modified)
                VALUES
                    (@username, @email, @password_hash, @iphone, @full_name, @avatar_path, @is_active, @created_at, @updated_at, @draftposition, @is_locked, @is_deleted, @is_modified);
                SELECT last_insert_rowid();";
			using var cmd = new SQLiteCommand( sql, connection );
			cmd.Parameters.AddWithValue( "@username", user.Username ?? "" );
			cmd.Parameters.AddWithValue( "@email", user.Email ?? "" );
			cmd.Parameters.AddWithValue( "@password_hash", user.PasswordHash ?? "" );
			cmd.Parameters.AddWithValue( "@iphone", user.Iphone ?? "" );
			cmd.Parameters.AddWithValue( "@full_name", user.FullName ?? "" );
			cmd.Parameters.AddWithValue( "@avatar_path", user.AvatarPath ?? "" );
			cmd.Parameters.AddWithValue( "@is_active", user.IsActive ? 1 : 0 );
			cmd.Parameters.AddWithValue( "@created_at", user.CreatedAt == default ? DateTime.Now : user.CreatedAt );
			cmd.Parameters.AddWithValue( "@updated_at", user.UpdatedAt == default ? DateTime.Now : user.UpdatedAt );
			cmd.Parameters.AddWithValue( "@draftposition", user.Draftposition ?? "" );
			cmd.Parameters.AddWithValue( "@is_locked", user.IsLocked ? 1 : 0 );
			cmd.Parameters.AddWithValue( "@is_deleted", user.IsDeleted ? 1 : 0 );
			cmd.Parameters.AddWithValue( "@is_modified", user.IsModified ? 1 : 0 );
			var res = cmd.ExecuteScalar();
			return Convert.ToInt32( res );
		}

		// Read: 根据 ID 获取用户（找不到返回 null）
		public Users GetById(int id)
		{
			using var connection = new SQLiteConnection( _connectionString );
			connection.Open();

			string sql = "SELECT * FROM users WHERE id = @id LIMIT 1";
			using var cmd = new SQLiteCommand( sql, connection );
			cmd.Parameters.AddWithValue( "@id", id );

			using var reader = cmd.ExecuteReader();
			if (reader.Read()) {
				return MapReaderToUser( reader );
			}

			return null;
		}

		// Read: 根据用户名获取用户（找不到返回 null）
		public Users GetByUsername(string username)
		{
			using var connection = new SQLiteConnection( _connectionString );
			connection.Open();

			string sql = "SELECT * FROM users WHERE username = @username LIMIT 1";
			using var cmd = new SQLiteCommand( sql, connection );
			cmd.Parameters.AddWithValue( "@username", username ?? "" );

			using var reader = cmd.ExecuteReader();
			if (reader.Read()) {
				return MapReaderToUser( reader );
			}

			return null;
		}

		// Read: 获取所有用户
		public List<Users> GetAll( )
		{
			var list = new List<Users>();
			using var connection = new SQLiteConnection( _connectionString );
			connection.Open();

			string sql = "SELECT * FROM users ORDER BY id";
			using var cmd = new SQLiteCommand( sql, connection );
			using var reader = cmd.ExecuteReader();
			while (reader.Read()) {
				list.Add( MapReaderToUser( reader ) );
			}
			return list;
		}

		// Update: 更新用户信息（返回是否成功）
		public bool Update(Users user)
		{
			using var connection = new SQLiteConnection( _connectionString );
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
                WHERE id = @id";

			using var cmd = new SQLiteCommand( sql, connection );
			cmd.Parameters.AddWithValue( "@username", user.Username ?? "" );
			cmd.Parameters.AddWithValue( "@email", user.Email ?? "" );
			cmd.Parameters.AddWithValue( "@password_hash", user.PasswordHash ?? "" );
			cmd.Parameters.AddWithValue( "@iphone", user.Iphone ?? "" );
			cmd.Parameters.AddWithValue( "@full_name", user.FullName ?? "" );
			cmd.Parameters.AddWithValue( "@avatar_path", user.AvatarPath ?? "" );
			cmd.Parameters.AddWithValue( "@is_active", user.IsActive ? 1 : 0 );
			cmd.Parameters.AddWithValue( "@updated_at", DateTime.Now );
			cmd.Parameters.AddWithValue( "@draftposition", user.Draftposition ?? "" );
			cmd.Parameters.AddWithValue( "@is_locked", user.IsLocked ? 1 : 0 );
			cmd.Parameters.AddWithValue( "@is_deleted", user.IsDeleted ? 1 : 0 );
			cmd.Parameters.AddWithValue( "@is_modified", user.IsModified ? 1 : 0 );
			cmd.Parameters.AddWithValue( "@id", user.Id );
			return cmd.ExecuteNonQuery() > 0;
		}

		// Delete: 按 ID 删除用户（返回是否成功）
		public bool Delete(int id)
		{
			using var connection = new SQLiteConnection( _connectionString );
			connection.Open();

			string sql = "DELETE FROM users WHERE id = @id";
			using var cmd = new SQLiteCommand( sql, connection );
			cmd.Parameters.AddWithValue( "@id", id );

			return cmd.ExecuteNonQuery() > 0;
		}

		// 辅助：检查用户名是否存在
		public bool ExistsByUsername(string username)
		{
			using var connection = new SQLiteConnection( _connectionString );
			connection.Open();

			string sql = "SELECT 1 FROM users WHERE username = @username LIMIT 1";
			using var cmd = new SQLiteCommand( sql, connection );
			cmd.Parameters.AddWithValue( "@username", username ?? "" );

			var res = cmd.ExecuteScalar();
			return res != null;
		}

		// 私有映射方法：将 SQLiteDataReader 映射为 Users 实例
		private Users MapReaderToUser(SQLiteDataReader reader)
		{
			return new Users
			{
				Id = Convert.ToInt32( reader["id"] ),
				Username = reader["username"]?.ToString(),
				Email = reader["email"]?.ToString(),
				PasswordHash = reader["password_hash"]?.ToString(),
				Iphone = reader["iphone"]?.ToString(),
				FullName = reader["full_name"]?.ToString(),
				AvatarPath = reader["avatar_path"]?.ToString(),
				IsActive = reader["is_active"] != DBNull.Value && Convert.ToInt32( reader["is_active"] ) == 1,
				CreatedAt = reader["created_at"] != DBNull.Value ? Convert.ToDateTime( reader["created_at"] ) : DateTime.MinValue,
				UpdatedAt = reader["updated_at"] != DBNull.Value ? Convert.ToDateTime( reader["updated_at"] ) : DateTime.MinValue,
				Draftposition = reader["draftposition"]?.ToString(),
				IsLocked = reader["is_locked"] != DBNull.Value && Convert.ToInt32( reader["is_locked"] ) == 1,
				IsDeleted = reader["is_deleted"] != DBNull.Value && Convert.ToInt32( reader["is_deleted"] ) == 1,
				IsModified = reader["is_modified"] != DBNull.Value && Convert.ToInt32( reader["is_modified"] ) == 1
			};
		}
	}
}