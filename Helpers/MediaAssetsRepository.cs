//C:\Project\MusicChange\Repositories\MediaAssetsRepository.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace MusicChange
{


	public class MediaAssetsRepository
	{
		private readonly string _dbPath;
		private readonly string _connectionString;

		public MediaAssetsRepository(string dbPath)
		{
			_dbPath = dbPath ?? throw new ArgumentNullException(nameof(dbPath));
			_connectionString = $"Data Source={_dbPath};Version=3;Foreign Keys=True;";
		}

		/// <summary>
		/// 确保 media_assets 表存在（若不存在则创建）
		/// </summary>
		public void EnsureTableExists()
		{
			const string sql = @"
CREATE TABLE IF NOT EXISTS media_assets(
	id INTEGER PRIMARY KEY AUTOINCREMENT,
	projects_id INTEGER NOT NULL,
	name TEXT NOT NULL,
	file_path TEXT NOT NULL,
	file_size INTEGER NOT NULL,
	media_type TEXT NOT NULL,
	duration TEXT,
	width INTEGER,
	height INTEGER,
	framerate REAL,
	codec TEXT,
	created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	updated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
	FOREIGN KEY (projects_id) REFERENCES projects (id)
);
";
			using var conn = new SQLiteConnection(_connectionString);
			conn.Open();
			using var cmd = new SQLiteCommand(sql, conn);
			cmd.ExecuteNonQuery();
		}

		/// <summary>
		/// 插入新记录，返回新记录 id
		/// </summary>
		public int Create(MediaAsset asset)
		{
			if(asset == null)
				throw new ArgumentNullException(nameof(asset));

			const string sql = @"
INSERT INTO media_assets (projects_id, name, file_path, file_size, media_type, duration, width, height, framerate, codec, created_at, updated_at)
VALUES (@projects_id, @name, @file_path, @file_size, @media_type, @duration, @width, @height, @framerate, @codec, @created_at, @updated_at);
";
			using var conn = new SQLiteConnection(_connectionString);
			conn.Open();
			using var tran = conn.BeginTransaction();
			using var cmd = new SQLiteCommand(sql, conn, tran);
			AddParameters(cmd, asset);
			// 如果调用方没有设置 CreatedAt / UpdatedAt，使用当前时间
			if(asset.CreatedAt == default)
				asset.CreatedAt = DateTime.Now;
			cmd.Parameters.AddWithValue("@created_at", asset.CreatedAt);
			cmd.Parameters.AddWithValue("@updated_at", asset.UpdatedAt.HasValue ? (object)asset.UpdatedAt.Value : DBNull.Value);

			cmd.ExecuteNonQuery();

			// 获取 last_insert_rowid
			using var idCmd = new SQLiteCommand("SELECT last_insert_rowid()", conn, tran);
			long last = (long)idCmd.ExecuteScalar();
			tran.Commit();
			return (int)last;
		}

		/// <summary>
		/// 根据 id 查询
		/// </summary>
		public MediaAsset GetById(int id)
		{
			const string sql = "SELECT * FROM media_assets WHERE id = @id LIMIT 1;";
			using var conn = new SQLiteConnection(_connectionString);
			conn.Open();
			using var cmd = new SQLiteCommand(sql, conn);
			cmd.Parameters.AddWithValue("@id", id);
			using var r = cmd.ExecuteReader();
			if(r.Read())
			{
				return MapReaderToMediaAsset(r);
			}
			return null;
		}

		/// <summary>
		/// 根据项目 id 查询所有媒体资源
		/// </summary>
		public List<MediaAsset> GetByProjectId(int projectId)
		{
			var list = new List<MediaAsset>();
			const string sql = "SELECT * FROM media_assets WHERE projects_id = @pid ORDER BY id;";
			using var conn = new SQLiteConnection(_connectionString);
			conn.Open();
			using var cmd = new SQLiteCommand(sql, conn);
			cmd.Parameters.AddWithValue("@pid", projectId);
			using var r = cmd.ExecuteReader();
			while(r.Read())
			{
				list.Add(MapReaderToMediaAsset(r));
			}
			return list;
		}

		/// <summary>
		/// 查询全部
		/// </summary>
		public List<MediaAsset> GetAll()
		{
			var list = new List<MediaAsset>();
			const string sql = "SELECT * FROM media_assets ORDER BY id;";
			using var conn = new SQLiteConnection(_connectionString);
			conn.Open();
			using var cmd = new SQLiteCommand(sql, conn);
			using var r = cmd.ExecuteReader();
			while(r.Read())
			{
				list.Add(MapReaderToMediaAsset(r));
			}
			return list;
		}

		/// <summary>
		/// 更新现有记录（根据 Id）
		/// </summary>
		public bool Update(MediaAsset asset)
		{
			if(asset == null)
				throw new ArgumentNullException(nameof(asset));
			if(asset.Id <= 0)
				return false;

			const string sql = @"
UPDATE media_assets
SET projects_id = @projects_id,
    name = @name,
    file_path = @file_path,
    file_size = @file_size,
    media_type = @media_type,
    duration = @duration,
    width = @width,
    height = @height,
    framerate = @framerate,
    codec = @codec,
    updated_at = CURRENT_TIMESTAMP
WHERE id = @id;
";
			using var conn = new SQLiteConnection(_connectionString);
			conn.Open();
			using var cmd = new SQLiteCommand(sql, conn);
			AddParameters(cmd, asset);
			cmd.Parameters.AddWithValue("@id", asset.Id);
			int rows = cmd.ExecuteNonQuery();
			return rows > 0;
		}

		/// <summary>
		/// 删除记录
		/// </summary>
		public bool Delete(int id)
		{
			const string sql = "DELETE FROM media_assets WHERE id = @id;";
			using var conn = new SQLiteConnection(_connectionString);
			conn.Open();
			using var cmd = new SQLiteCommand(sql, conn);
			cmd.Parameters.AddWithValue("@id", id);
			int rows = cmd.ExecuteNonQuery();
			return rows > 0;
		}

		#region Helpers

		internal static void AddParameters(SQLiteCommand cmd, MediaAsset asset)
		{
			cmd.Parameters.AddWithValue("@projects_id", asset.ProjectId);
			cmd.Parameters.AddWithValue("@name", asset.Name ?? string.Empty);
			cmd.Parameters.AddWithValue("@file_path", asset.FilePath ?? string.Empty);
			cmd.Parameters.AddWithValue("@file_size", asset.FileSize);
			cmd.Parameters.AddWithValue("@media_type", asset.MediaType ?? string.Empty);
			cmd.Parameters.AddWithValue("@duration", asset.Duration ?? string.Empty);
			cmd.Parameters.AddWithValue("@width", asset.Width.HasValue ? (object)asset.Width.Value : DBNull.Value);
			cmd.Parameters.AddWithValue("@height", asset.Height.HasValue ? (object)asset.Height.Value : DBNull.Value);
			cmd.Parameters.AddWithValue("@framerate", asset.Framerate.HasValue ? (object)asset.Framerate.Value : DBNull.Value);
			cmd.Parameters.AddWithValue("@codec", asset.Codec ?? string.Empty);
		}

		private static MediaAsset MapReaderToMediaAsset(IDataRecord r)
		{
			var a = new MediaAsset
			{
				Id = r.GetInt32(r.GetOrdinal("id")),
				ProjectId = r.GetInt32(r.GetOrdinal("projects_id")),
				Name = r["name"] as string,
				FilePath = r["file_path"] as string,
				FileSize = r["file_size"] != DBNull.Value ? Convert.ToInt64(r["file_size"]) : 0,
				MediaType = r["media_type"] as string,
				Duration = r["duration"]  as string,
				Width = r["width"] != DBNull.Value ? (int?)Convert.ToInt32(r["width"]) : null,
				Height = r["height"] != DBNull.Value ? (int?)Convert.ToInt32(r["height"]) : null,
				Framerate = r["framerate"] != DBNull.Value ? (double?)Convert.ToDouble(r["framerate"]) : null,
				Codec = r["codec"] as string,
				CreatedAt = r["created_at"] != DBNull.Value ? Convert.ToDateTime(r["created_at"]) : DateTime.MinValue,
				UpdatedAt = r["updated_at"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(r["updated_at"]) : null
			};
			return a;
		}

		#endregion
	}
}