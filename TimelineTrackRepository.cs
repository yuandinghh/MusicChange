using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using static MusicChange.db;

namespace MusicChange
{
	public class TimelineTrackRepository
	{

		public TimelineTrackRepository(string dbPath)
		{
			_connectionString = $"Data Source={dbPath};Version=3;";
		}

		// 创建时间线轨道
		public int Create(TimelineTrack track)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    INSERT INTO timeline_tracks (
                        project_id, track_type, track_index, name, is_muted, is_locked, volume
                    ) VALUES (
                        @project_id, @track_type, @track_index, @name, @is_muted, @is_locked, @volume
                    );
                    SELECT last_insert_rowid();";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@project_id", track.ProjectId );
					command.Parameters.AddWithValue( "@track_type", track.TrackType );
					command.Parameters.AddWithValue( "@track_index", track.TrackIndex );
					command.Parameters.AddWithValue( "@name", track.Name );
					command.Parameters.AddWithValue( "@is_muted", track.IsMuted );
					command.Parameters.AddWithValue( "@is_locked", track.IsLocked );
					command.Parameters.AddWithValue( "@volume", track.Volume );

					return Convert.ToInt32( command.ExecuteScalar() );
				}
			}
		}

		// 根据ID获取时间线轨道
		public TimelineTrack GetById(int id)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    SELECT id, project_id, track_type, track_index, name, is_muted, is_locked, volume, created_at
                    FROM timeline_tracks 
                    WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );

					using (var reader = command.ExecuteReader()) {
						if (reader.Read()) {
							return new TimelineTrack
							{
								Id = Convert.ToInt32( reader["id"] ),
								ProjectId = Convert.ToInt32( reader["project_id"] ),
								TrackType = reader["track_type"].ToString(),
								TrackIndex = Convert.ToInt32( reader["track_index"] ),
								Name = reader["name"].ToString(),
								IsMuted = Convert.ToBoolean( reader["is_muted"] ),
								IsLocked = Convert.ToBoolean( reader["is_locked"] ),
								Volume = Convert.ToDouble( reader["volume"] ),
								CreatedAt = Convert.ToDateTime( reader["created_at"] )
							};
						}
					}
				}
			}

			return null;
		}

		// 获取项目的所有时间线轨道
		public List<TimelineTrack> GetByProjectId(int projectId)
		{
			var tracks = new List<TimelineTrack>();

			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    SELECT id, project_id, track_type, track_index, name, is_muted, is_locked, volume, created_at
                    FROM timeline_tracks 
                    WHERE project_id = @project_id
                    ORDER BY track_index";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@project_id", projectId );

					using (var reader = command.ExecuteReader()) {
						while (reader.Read()) {
							tracks.Add( new TimelineTrack
							{
								Id = Convert.ToInt32( reader["id"] ),
								ProjectId = Convert.ToInt32( reader["project_id"] ),
								TrackType = reader["track_type"].ToString(),
								TrackIndex = Convert.ToInt32( reader["track_index"] ),
								Name = reader["name"].ToString(),
								IsMuted = Convert.ToBoolean( reader["is_muted"] ),
								IsLocked = Convert.ToBoolean( reader["is_locked"] ),
								Volume = Convert.ToDouble( reader["volume"] ),
								CreatedAt = Convert.ToDateTime( reader["created_at"] )
							} );
						}
					}
				}
			}

			return tracks;
		}

		// 更新时间线轨道
		public bool Update(TimelineTrack track)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    UPDATE timeline_tracks 
                    SET project_id = @project_id, track_type = @track_type, track_index = @track_index, 
                        name = @name, is_muted = @is_muted, is_locked = @is_locked, volume = @volume
                    WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@project_id", track.ProjectId );
					command.Parameters.AddWithValue( "@track_type", track.TrackType );
					command.Parameters.AddWithValue( "@track_index", track.TrackIndex );
					command.Parameters.AddWithValue( "@name", track.Name );
					command.Parameters.AddWithValue( "@is_muted", track.IsMuted );
					command.Parameters.AddWithValue( "@is_locked", track.IsLocked );
					command.Parameters.AddWithValue( "@volume", track.Volume );
					command.Parameters.AddWithValue( "@id", track.Id );

					return command.ExecuteNonQuery() > 0;
				}
			}
		}

		// 删除时间线轨道
		public bool Delete(int id)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = "DELETE FROM timeline_tracks WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );
					return command.ExecuteNonQuery() > 0;
				}
			}
		}
	}
}