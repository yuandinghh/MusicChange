using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace MusicChange
{
	public class MainRepository
	{
		private readonly string _connectionString;

		public MainRepository(string dbPath)
		{
			if(string.IsNullOrEmpty(dbPath))
				throw new ArgumentNullException(nameof(dbPath));
			Directory.CreateDirectory(Path.GetDirectoryName(dbPath) ?? ".");
			_connectionString = $"Data Source={dbPath};Version=3;Pooling=True;Journal Mode=WAL;";
			EnsureTableExists();
		}

		private void EnsureTableExists()
		{
			const string sql = @"
CREATE TABLE IF NOT EXISTS main (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  curren_user_id INTEGER,
  curren_project_id INTEGER,
  login_time DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
  workofftime DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
  version TEXT,
  first_version TEXT,
  server_website TEXT,
  complaint TEXT,
  complaint_feedback INTEGER,
  complaint_country TEXT,
  complaint_id INTEGER,
  is_locked INTEGER NOT NULL DEFAULT 0,
  current_run INTEGER NOT NULL DEFAULT 0,
  the_next_revision_schedule TEXT,
  version_end_time DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
  registered_user TEXT,
  description TEXT,
  created_at DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
  updated_at DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);";
			using var conn = new SQLiteConnection(_connectionString);
			conn.Open();
			using var cmd = new SQLiteCommand(sql, conn);
			cmd.ExecuteNonQuery();
		}

		public int Create(Main model)
		{
			if(model == null)
				throw new ArgumentNullException(nameof(model));
			const string sql = @"
INSERT INTO main(
  curren_user_id, curren_project_id, login_time, workofftime,
  version, first_version, server_website, complaint, complaint_feedback,
  complaint_country, complaint_id, is_locked, current_run, the_next_revision_schedule,
  version_end_time, registered_user, description, created_at, updated_at
) VALUES (
  @curren_user_id, @curren_project_id, @login_time, @workofftime,
  @version, @first_version, @server_website, @complaint, @complaint_feedback,
  @complaint_country, @complaint_id, @is_locked, @current_run, @the_next_revision_schedule,
  @version_end_time, @registered_user, @description, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
);
SELECT last_insert_rowid();";
			using var conn = new SQLiteConnection(_connectionString);
			conn.Open();
			using var cmd = new SQLiteCommand(sql, conn);
			AddParameters(cmd, model);
			var id = Convert.ToInt32(cmd.ExecuteScalar());
			return id;
		}

		public Main? GetById(int id)
		{
			const string sql = "SELECT * FROM main WHERE id = @id LIMIT 1;";
			using var conn = new SQLiteConnection(_connectionString);
			conn.Open();
			using var cmd = new SQLiteCommand(sql, conn);
			cmd.Parameters.AddWithValue("@id", id);
			using var rdr = cmd.ExecuteReader();
			if(rdr.Read())
				return MapReaderToMain(rdr);
			return null;
		}

		public Main? GetCurrentRunning()
		{
			const string sql = "SELECT * FROM main WHERE current_run = 1 ORDER BY updated_at DESC LIMIT 1;";
			using var conn = new SQLiteConnection(_connectionString);
			conn.Open();
			using var cmd = new SQLiteCommand(sql, conn);
			using var rdr = cmd.ExecuteReader();
			if(rdr.Read())
				return MapReaderToMain(rdr);
			return null;
		}

		public List<Main> GetAll()
		{
			var list = new List<Main>();
			const string sql = "SELECT * FROM main ORDER BY created_at DESC;";
			using var conn = new SQLiteConnection(_connectionString);
			conn.Open();
			using var cmd = new SQLiteCommand(sql, conn);
			using var rdr = cmd.ExecuteReader();
			while(rdr.Read())
				list.Add(MapReaderToMain(rdr));
			return list;
		}

		public bool Update(Main model)
		{
			if(model == null)
				throw new ArgumentNullException(nameof(model));
			const string sql = @"
UPDATE main SET
  curren_user_id=@curren_user_id,
  curren_project_id=@curren_project_id,
  login_time=@login_time,
  workofftime=@workofftime,
  version=@version,
  first_version=@first_version,
  server_website=@server_website,
  complaint=@complaint,
  complaint_feedback=@complaint_feedback,
  complaint_country=@complaint_country,
  complaint_id=@complaint_id,
  is_locked=@is_locked,
  current_run=@current_run,
  the_next_revision_schedule=@the_next_revision_schedule,
  version_end_time=@version_end_time,
  registered_user=@registered_user,
  description=@description,
  updated_at=CURRENT_TIMESTAMP
WHERE id = @id;";
			using var conn = new SQLiteConnection(_connectionString);
			conn.Open();
			using var cmd = new SQLiteCommand(sql, conn);
			cmd.Parameters.AddWithValue("@id", model.Id);
			AddParameters(cmd, model);
			return cmd.ExecuteNonQuery() > 0;
		}

		public bool Delete(int id)
		{
			const string sql = "DELETE FROM main WHERE id = @id;";
			using var conn = new SQLiteConnection(_connectionString);
			conn.Open();
			using var cmd = new SQLiteCommand(sql, conn);
			cmd.Parameters.AddWithValue("@id", id);
			return cmd.ExecuteNonQuery() > 0;
		}

		private static void AddParameters(SQLiteCommand cmd, Main m)
		{
			cmd.Parameters.AddWithValue("@curren_user_id", m.CurrenUserId);
			cmd.Parameters.AddWithValue("@curren_project_id", m.CurrenProjectId);
			cmd.Parameters.AddWithValue("@login_time", m.LoginTime);
			cmd.Parameters.AddWithValue("@workofftime", m.Workofftime);
			cmd.Parameters.AddWithValue("@version", (object?)m.version ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@first_version", (object?)m.first_version ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@server_website", (object?)m.Server_website ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@complaint", (object?)m.complaint ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@complaint_feedback", m.complaint_Feedback);
			cmd.Parameters.AddWithValue("@complaint_country", (object?)m.complaint_country ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@complaint_id", m.complaint_id);
			cmd.Parameters.AddWithValue("@is_locked", m.IsLocked ? 1 : 0);
			cmd.Parameters.AddWithValue("@current_run", m.current_run ? 1 : 0);
			cmd.Parameters.AddWithValue("@the_next_revision_schedule", (object?)m.The_next_revision_schedule ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@version_end_time", m.Version_end_time);
			cmd.Parameters.AddWithValue("@registered_user", (object?)m.registered_user ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@description", (object?)m.Description ?? DBNull.Value);
		}

		private static Main MapReaderToMain(IDataRecord r)
		{
			var m = new Main
			{
				Id = Convert.ToInt32(r["id"]),
				CurrenUserId = r["curren_user_id"] == DBNull.Value ? 0 : Convert.ToInt32(r["curren_user_id"]),
				CurrenProjectId = r["curren_project_id"] == DBNull.Value ? 0 : Convert.ToInt32(r["curren_project_id"]),
				LoginTime = r["login_time"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["login_time"]),
				Workofftime = r["workofftime"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["workofftime"]),
				version = r["version"] as string,
				first_version = r["first_version"] as string,
				Server_website = r["server_website"] as string,
				complaint = r["complaint"] as string,
				complaint_Feedback = r["complaint_feedback"] == DBNull.Value ? 0 : Convert.ToInt32(r["complaint_feedback"]),
				complaint_country = r["complaint_country"] as string,
				complaint_id = r["complaint_id"] == DBNull.Value ? 0 : Convert.ToInt32(r["complaint_id"]),
				IsLocked = r["is_locked"] != DBNull.Value && Convert.ToInt32(r["is_locked"]) == 1,
				current_run = r["current_run"] != DBNull.Value && Convert.ToInt32(r["current_run"]) == 1,
				The_next_revision_schedule = r["the_next_revision_schedule"] as string,
				Version_end_time = r["version_end_time"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["version_end_time"]),
				registered_user = r["registered_user"] as string,
				Description = r["description"] as string
			};
			return m;
		}
	}
}