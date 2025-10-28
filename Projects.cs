using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using static MusicChange.db;

namespace MusicChange
{
	public class ProjectRepository
	{
		public ProjectRepository(string dbPath)
		{
			_connectionString = $"Data Source={dbPath};Version=3;";
		}

		// 创建项目
		public int Create(Project project)
		{
			using (var connection = new SqliteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    INSERT INTO projects (
                        user_id, name, description, width, height, framerate, duration, thumbnail_path
                    ) VALUES (
                        @user_id, @name, @description, @width, @height, @framerate, @duration, @thumbnail_path
                    );
                    SELECT last_insert_rowid();";

				using (var command = new SqliteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@user_id", project.UserId );
					command.Parameters.AddWithValue( "@name", project.Name );
					command.Parameters.AddWithValue( "@description", project.Description ?? "" );
					command.Parameters.AddWithValue( "@width", project.Width );
					command.Parameters.AddWithValue( "@height", project.Height );
					command.Parameters.AddWithValue( "@framerate", project.Framerate );
					command.Parameters.AddWithValue( "@duration", project.Duration );
					command.Parameters.AddWithValue( "@thumbnail_path", project.ThumbnailPath ?? "" );

					return Convert.ToInt32( command.ExecuteScalar() );
				}
			}
		}

		// 根据ID获取项目
		public Project GetById(int id)
		{
			using (var connection = new SqliteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    SELECT id, user_id, name, description, width, height, framerate, 
                           duration, thumbnail_path, created_at, updated_at
                    FROM projects 
                    WHERE id = @id";

				using (var command = new SqliteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );

					using (var reader = command.ExecuteReader()) {
						if (reader.Read()) {
							return new Project
							{
								Id = Convert.ToInt32( reader["id"] ),
								UserId = Convert.ToInt32( reader["user_id"] ),
								Name = reader["name"].ToString(),
								Description = reader["description"].ToString(),
								Width = Convert.ToInt32( reader["width"] ),
								Height = Convert.ToInt32( reader["height"] ),
								Framerate = Convert.ToDouble( reader["framerate"] ),
								Duration = Convert.ToDouble( reader["duration"] ),
								ThumbnailPath = reader["thumbnail_path"].ToString(),
								CreatedAt = Convert.ToDateTime( reader["created_at"] ),
								UpdatedAt = Convert.ToDateTime( reader["updated_at"] )
							};
						}
					}
				}
			}

			return null;
		}

		// 获取用户的所有项目
		public List<Project> GetByUserId(int userId)
		{
			var projects = new List<Project>();

			using (var connection = new SqliteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    SELECT id, user_id, name, description, width, height, framerate, 
                           duration, thumbnail_path, created_at, updated_at
                    FROM projects 
                    WHERE user_id = @user_id
                    ORDER BY created_at DESC";

				using (var command = new SqliteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@user_id", userId );

					using (var reader = command.ExecuteReader()) {
						while (reader.Read()) {
							projects.Add( new Project
							{
								Id = Convert.ToInt32( reader["id"] ),
								UserId = Convert.ToInt32( reader["user_id"] ),
								Name = reader["name"].ToString(),
								Description = reader["description"].ToString(),
								Width = Convert.ToInt32( reader["width"] ),
								Height = Convert.ToInt32( reader["height"] ),
								Framerate = Convert.ToDouble( reader["framerate"] ),
								Duration = Convert.ToDouble( reader["duration"] ),
								ThumbnailPath = reader["thumbnail_path"].ToString(),
								CreatedAt = Convert.ToDateTime( reader["created_at"] ),
								UpdatedAt = Convert.ToDateTime( reader["updated_at"] )
							} );
						}
					}
				}
			}

			return projects;
		}

		// 更新项目
		public bool Update(Project project)
		{
			using (var connection = new SqliteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    UPDATE projects 
                    SET user_id = @user_id, name = @name, description = @description, 
                        width = @width, height = @height, framerate = @framerate, 
                        duration = @duration, thumbnail_path = @thumbnail_path
                    WHERE id = @id";

				using (var command = new SqliteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@user_id", project.UserId );
					command.Parameters.AddWithValue( "@name", project.Name );
					command.Parameters.AddWithValue( "@description", project.Description ?? "" );
					command.Parameters.AddWithValue( "@width", project.Width );
					command.Parameters.AddWithValue( "@height", project.Height );
					command.Parameters.AddWithValue( "@framerate", project.Framerate );
					command.Parameters.AddWithValue( "@duration", project.Duration );
					command.Parameters.AddWithValue( "@thumbnail_path", project.ThumbnailPath ?? "" );
					command.Parameters.AddWithValue( "@id", project.Id );

					return command.ExecuteNonQuery() > 0;
				}
			}
		}

		// 删除项目
		public bool Delete(int id)
		{
			using (var connection = new SqliteConnection( _connectionString )) {
				connection.Open();

				string sql = "DELETE FROM projects WHERE id = @id";

				using (var command = new SqliteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );
					return command.ExecuteNonQuery() > 0;
				}
			}
		}
	}
}

