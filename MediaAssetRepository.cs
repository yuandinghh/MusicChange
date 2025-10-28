using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using static MusicChange.db;

namespace MusicChange
{
	public class MediaAssetRepository
	{
		public MediaAssetRepository(string dbPath)
		{
			_connectionString = $"Data Source={dbPath};Version=3;";
		}

		// 创建媒体资源
		public int Create(MediaAsset mediaAsset)
		{
			using (var connection = new SqliteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    INSERT INTO media_assets (
                        user_id, name, file_path, file_size, media_type, duration, 
                        width, height, framerate, codec
                    ) VALUES (
                        @user_id, @name, @file_path, @file_size, @media_type, @duration, 
                        @width, @height, @framerate, @codec
                    );
                    SELECT last_insert_rowid();";

				using (var command = new SqliteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@user_id", mediaAsset.UserId );
					command.Parameters.AddWithValue( "@name", mediaAsset.Name );
					command.Parameters.AddWithValue( "@file_path", mediaAsset.FilePath );
					command.Parameters.AddWithValue( "@file_size", mediaAsset.FileSize );
					command.Parameters.AddWithValue( "@media_type", mediaAsset.MediaType );
					command.Parameters.AddWithValue( "@duration", mediaAsset.Duration ?? (object)DBNull.Value );
					command.Parameters.AddWithValue( "@width", mediaAsset.Width ?? (object)DBNull.Value );
					command.Parameters.AddWithValue( "@height", mediaAsset.Height ?? (object)DBNull.Value );
					command.Parameters.AddWithValue( "@framerate", mediaAsset.Framerate ?? (object)DBNull.Value );
					command.Parameters.AddWithValue( "@codec", mediaAsset.Codec ?? "" );

					return Convert.ToInt32( command.ExecuteScalar() );
				}
			}
		}

		// 根据ID获取媒体资源
		public MediaAsset GetById(int id)
		{
			using (var connection = new SqliteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    SELECT id, user_id, name, file_path, file_size, media_type, duration, 
                           width, height, framerate, codec, created_at
                    FROM media_assets 
                    WHERE id = @id";

				using (var command = new SqliteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );

					using (var reader = command.ExecuteReader()) {
						if (reader.Read()) {
							return new MediaAsset
							{
								Id = Convert.ToInt32( reader["id"] ),
								UserId = Convert.ToInt32( reader["user_id"] ),
								Name = reader["name"].ToString(),
								FilePath = reader["file_path"].ToString(),
								FileSize = Convert.ToInt64( reader["file_size"] ),
								MediaType = reader["media_type"].ToString(),
								Duration = reader["duration"] as double?,
								Width = reader["width"] as int?,
								Height = reader["height"] as int?,
								Framerate = reader["framerate"] as double?,
								Codec = reader["codec"].ToString(),
								CreatedAt = Convert.ToDateTime( reader["created_at"] )
							};
						}
					}
				}
			}

			return null;
		}

		// 获取用户的所有媒体资源
		public List<MediaAsset> GetByUserId(int userId)
		{
			var mediaAssets = new List<MediaAsset>();

			using (var connection = new SqliteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    SELECT id, user_id, name, file_path, file_size, media_type, duration, 
                           width, height, framerate, codec, created_at
                    FROM media_assets 
                    WHERE user_id = @user_id
                    ORDER BY created_at DESC";

				using (var command = new SqliteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@user_id", userId );

					using (var reader = command.ExecuteReader()) {
						while (reader.Read()) {
							mediaAssets.Add( new MediaAsset
							{
								Id = Convert.ToInt32( reader["id"] ),
								UserId = Convert.ToInt32( reader["user_id"] ),
								Name = reader["name"].ToString(),
								FilePath = reader["file_path"].ToString(),
								FileSize = Convert.ToInt64( reader["file_size"] ),
								MediaType = reader["media_type"].ToString(),
								Duration = reader["duration"] as double?,
								Width = reader["width"] as int?,
								Height = reader["height"] as int?,
								Framerate = reader["framerate"] as double?,
								Codec = reader["codec"].ToString(),
								CreatedAt = Convert.ToDateTime( reader["created_at"] )
							} );
						}
					}
				}
			}

			return mediaAssets;
		}

		// 更新媒体资源
		public bool Update(MediaAsset mediaAsset)
		{
			using (var connection = new SqliteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    UPDATE media_assets 
                    SET user_id = @user_id, name = @name, file_path = @file_path, 
                        file_size = @file_size, media_type = @media_type, duration = @duration, 
                        width = @width, height = @height, framerate = @framerate, codec = @codec
                    WHERE id = @id";

				using (var command = new SqliteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@user_id", mediaAsset.UserId );
					command.Parameters.AddWithValue( "@name", mediaAsset.Name );
					command.Parameters.AddWithValue( "@file_path", mediaAsset.FilePath );
					command.Parameters.AddWithValue( "@file_size", mediaAsset.FileSize );
					command.Parameters.AddWithValue( "@media_type", mediaAsset.MediaType );
					command.Parameters.AddWithValue( "@duration", mediaAsset.Duration ?? (object)DBNull.Value );
					command.Parameters.AddWithValue( "@width", mediaAsset.Width ?? (object)DBNull.Value );
					command.Parameters.AddWithValue( "@height", mediaAsset.Height ?? (object)DBNull.Value );
					command.Parameters.AddWithValue( "@framerate", mediaAsset.Framerate ?? (object)DBNull.Value );
					command.Parameters.AddWithValue( "@codec", mediaAsset.Codec ?? "" );
					command.Parameters.AddWithValue( "@id", mediaAsset.Id );

					return command.ExecuteNonQuery() > 0;
				}
			}
		}

		// 删除媒体资源
		public bool Delete(int id)
		{
			using (var connection = new SqliteConnection( _connectionString )) {
				connection.Open();

				string sql = "DELETE FROM media_assets WHERE id = @id";

				using (var command = new SqliteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );
					return command.ExecuteNonQuery() > 0;
				}
			}
		}
	}
}