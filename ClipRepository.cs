using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
//using static MusicChange.db;

namespace MusicChange
{
	public class ClipRepository
	{
		private readonly string _connectionString;

		public ClipRepository(string dbPath)
		{
			_connectionString = $"Data Source={dbPath};Version=3;";
		}

		// 创建剪辑片段
		public int Create(Clip clip)
		{
			using (var connection = new SqliteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    INSERT INTO clips (
                        project_id, track_id, media_asset_id, name, start_time, end_time, 
                        media_start_time, media_end_time, position_x, position_y, 
                        scale_x, scale_y, rotation, volume, is_muted
                    ) VALUES (
                        @project_id, @track_id, @media_asset_id, @name, @start_time, @end_time, 
                        @media_start_time, @media_end_time, @position_x, @position_y, 
                        @scale_x, @scale_y, @rotation, @volume, @is_muted
                    );
                    SELECT last_insert_rowid();";

				using (var command = new SqliteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@project_id", clip.ProjectId );
					command.Parameters.AddWithValue( "@track_id", clip.TrackId );
					command.Parameters.AddWithValue( "@media_asset_id", clip.MediaAssetId ?? (object)DBNull.Value );
					command.Parameters.AddWithValue( "@name", clip.Name );
					command.Parameters.AddWithValue( "@start_time", clip.StartTime );
					command.Parameters.AddWithValue( "@end_time", clip.EndTime );
					command.Parameters.AddWithValue( "@media_start_time", clip.MediaStartTime );
					command.Parameters.AddWithValue( "@media_end_time", clip.MediaEndTime );
					command.Parameters.AddWithValue( "@position_x", clip.PositionX );
					command.Parameters.AddWithValue( "@position_y", clip.PositionY );
					command.Parameters.AddWithValue( "@scale_x", clip.ScaleX );
					command.Parameters.AddWithValue( "@scale_y", clip.ScaleY );
					command.Parameters.AddWithValue( "@rotation", clip.Rotation );
					command.Parameters.AddWithValue( "@volume", clip.Volume );
					command.Parameters.AddWithValue( "@is_muted", clip.IsMuted );

					return Convert.ToInt32( command.ExecuteScalar() );
				}
			}
		}

		// 根据ID获取剪辑片段
		public Clip GetById(int id)
		{
			using (var connection = new SqliteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    SELECT id, project_id, track_id, media_asset_id, name, start_time, end_time, 
                           media_start_time, media_end_time, position_x, position_y, 
                           scale_x, scale_y, rotation, volume, is_muted, created_at, updated_at
                    FROM clips 
                    WHERE id = @id";

				using (var command = new SqliteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );

					using (var reader = command.ExecuteReader()) {
						if (reader.Read()) {
							return new Clip
							{
								Id = Convert.ToInt32( reader["id"] ),
								ProjectId = Convert.ToInt32( reader["project_id"] ),
								TrackId = Convert.ToInt32( reader["track_id"] ),
								MediaAssetId = reader["media_asset_id"] as int?,
								Name = reader["name"].ToString(),
								StartTime = Convert.ToDouble( reader["start_time"] ),
								EndTime = Convert.ToDouble( reader["end_time"] ),
								MediaStartTime = Convert.ToDouble( reader["media_start_time"] ),
								MediaEndTime = Convert.ToDouble( reader["media_end_time"] ),
								PositionX = Convert.ToDouble( reader["position_x"] ),
								PositionY = Convert.ToDouble( reader["position_y"] ),
								ScaleX = Convert.ToDouble( reader["scale_x"] ),
								ScaleY = Convert.ToDouble( reader["scale_y"] ),
								Rotation = Convert.ToDouble( reader["rotation"] ),
								Volume = Convert.ToDouble( reader["volume"] ),
								IsMuted = Convert.ToBoolean( reader["is_muted"] ),
								CreatedAt = Convert.ToDateTime( reader["created_at"] ),
								UpdatedAt = Convert.ToDateTime( reader["updated_at"] )
							};
						}
					}
				}
			}

			return null;
		}

		// 获取轨道的所有剪辑片段
		public List<Clip> GetByTrackId(int trackId)
		{
			var clips = new List<Clip>();

			using (var connection = new SqliteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    SELECT id, project_id, track_id, media_asset_id, name, start_time, end_time, 
                           media_start_time, media_end_time, position_x, position_y, 
                           scale_x, scale_y, rotation, volume, is_muted, created_at, updated_at
                    FROM clips 
                    WHERE track_id = @track_id
                    ORDER BY start_time";

				using (var command = new SqliteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@track_id", trackId );

					using (var reader = command.ExecuteReader()) {
						while (reader.Read()) {
							clips.Add( new Clip
							{
								Id = Convert.ToInt32( reader["id"] ),
								ProjectId = Convert.ToInt32( reader["project_id"] ),
								TrackId = Convert.ToInt32( reader["track_id"] ),
								MediaAssetId = reader["media_asset_id"] as int?,
								Name = reader["name"].ToString(),
								StartTime = Convert.ToDouble( reader["start_time"] ),
								EndTime = Convert.ToDouble( reader["end_time"] ),
								MediaStartTime = Convert.ToDouble( reader["media_start_time"] ),
								MediaEndTime = Convert.ToDouble( reader["media_end_time"] ),
								PositionX = Convert.ToDouble( reader["position_x"] ),
								PositionY = Convert.ToDouble( reader["position_y"] ),
								ScaleX = Convert.ToDouble( reader["scale_x"] ),
								ScaleY = Convert.ToDouble( reader["scale_y"] ),
								Rotation = Convert.ToDouble( reader["rotation"] ),
								Volume = Convert.ToDouble( reader["volume"] ),
								IsMuted = Convert.ToBoolean( reader["is_muted"] ),
								CreatedAt = Convert.ToDateTime( reader["created_at"] ),
								UpdatedAt = Convert.ToDateTime( reader["updated_at"] )
							} );
						}
					}
				}
			}

			return clips;
		}

		// 获取项目的所有剪辑片段
		public List<Clip> GetByProjectId(int projectId)
		{
			var clips = new List<Clip>();

			using (var connection = new SqliteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    SELECT id, project_id, track_id, media_asset_id, name, start_time, end_time, 
                           media_start_time, media_end_time, position_x, position_y, 
                           scale_x, scale_y, rotation, volume, is_muted, created_at, updated_at
                    FROM clips 
                    WHERE project_id = @project_id
                    ORDER BY track_id, start_time";

				using (var command = new SqliteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@project_id", projectId );

					using (var reader = command.ExecuteReader()) {
						while (reader.Read()) {
							clips.Add( new Clip
							{
								Id = Convert.ToInt32( reader["id"] ),
								ProjectId = Convert.ToInt32( reader["project_id"] ),
								TrackId = Convert.ToInt32( reader["track_id"] ),
								MediaAssetId = reader["media_asset_id"] as int?,
								Name = reader["name"].ToString(),
								StartTime = Convert.ToDouble( reader["start_time"] ),
								EndTime = Convert.ToDouble( reader["end_time"] ),
								MediaStartTime = Convert.ToDouble( reader["media_start_time"] ),
								MediaEndTime = Convert.ToDouble( reader["media_end_time"] ),
								PositionX = Convert.ToDouble( reader["position_x"] ),
								PositionY = Convert.ToDouble( reader["position_y"] ),
								ScaleX = Convert.ToDouble( reader["scale_x"] ),
								ScaleY = Convert.ToDouble( reader["scale_y"] ),
								Rotation = Convert.ToDouble( reader["rotation"] ),
								Volume = Convert.ToDouble( reader["volume"] ),
								IsMuted = Convert.ToBoolean( reader["is_muted"] ),
								CreatedAt = Convert.ToDateTime( reader["created_at"] ),
								UpdatedAt = Convert.ToDateTime( reader["updated_at"] )
							} );
						}
					}
				}
			}

			return clips;
		}

		// 更新剪辑片段
		public bool Update(Clip clip)
		{
			using (var connection = new SqliteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    UPDATE clips 
                    SET project_id = @project_id, track_id = @track_id, media_asset_id = @media_asset_id, 
                        name = @name, start_time = @start_time, end_time = @end_time, 
                        media_start_time = @media_start_time, media_end_time = @media_end_time, 
                        position_x = @position_x, position_y = @position_y, scale_x = @scale_x, 
                        scale_y = @scale_y, rotation = @rotation, volume = @volume, is_muted = @is_muted
                    WHERE id = @id";

				using (var command = new SqliteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@project_id", clip.ProjectId );
					command.Parameters.AddWithValue( "@track_id", clip.TrackId );
					command.Parameters.AddWithValue( "@media_asset_id", clip.MediaAssetId ?? (object)DBNull.Value );
					command.Parameters.AddWithValue( "@name", clip.Name );
					command.Parameters.AddWithValue( "@start_time", clip.StartTime );
					command.Parameters.AddWithValue( "@end_time", clip.EndTime );
					command.Parameters.AddWithValue( "@media_start_time", clip.MediaStartTime );
					command.Parameters.AddWithValue( "@media_end_time", clip.MediaEndTime );
					command.Parameters.AddWithValue( "@position_x", clip.PositionX );
					command.Parameters.AddWithValue( "@position_y", clip.PositionY );
					command.Parameters.AddWithValue( "@scale_x", clip.ScaleX );
					command.Parameters.AddWithValue( "@scale_y", clip.ScaleY );
					command.Parameters.AddWithValue( "@rotation", clip.Rotation );
					command.Parameters.AddWithValue( "@volume", clip.Volume );
					command.Parameters.AddWithValue( "@is_muted", clip.IsMuted );
					command.Parameters.AddWithValue( "@id", clip.Id );

					return command.ExecuteNonQuery() > 0;
				}
			}
		}

		// 删除剪辑片段
		public bool Delete(int id)
		{
			using (var connection = new SqliteConnection( _connectionString )) {
				connection.Open();

				string sql = "DELETE FROM clips WHERE id = @id";

				using (var command = new SqliteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );
					return command.ExecuteNonQuery() > 0;
				}
			}
		}
	}
}


//public class Clip
//{
//	public int Id { get; set; }
//	public int ProjectId { get; set; }
//	public int TrackId { get; set; }
//	public int? MediaAssetId { get; set; }
//	public string Name { get; set; }
//	public double StartTime { get; set; }
//	public double EndTime { get; set; }
//	public double MediaStartTime { get; set; }
//	public double MediaEndTime { get; set; }
//	public double PositionX { get; set; }
//	public double PositionY { get; set; }
//	public double ScaleX { get; set; }
//	public double ScaleY { get; set; }
//	public double Rotation { get; set; }
//	public double Volume { get; set; } = 1.0;
//	public bool IsMuted { get; set; } = false;
//	public DateTime CreatedAt { get; set; } = DateTime.Now;
//	public DateTime UpdatedAt { get; set; } = DateTime.Now;
//}
