using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace MusicChange
{
    public class ProjectsRepository
    {
        private readonly string _connectionString;

        public ProjectsRepository(string dbPath)
        {
            if(string.IsNullOrEmpty(dbPath))
                throw new ArgumentNullException(nameof(dbPath));
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath) ?? ".");
            _connectionString = $"Data Source={dbPath};Version=3;Pooling=True;Journal Mode=WAL;";
            EnsureTableExists();
        }

        public void EnsureTableExists()
        {
            const string sql = @"
                CREATE TABLE IF NOT EXISTS projects(
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    user_id INTEGER NOT NULL,
                    name TEXT NOT NULL,
                    description TEXT,
                    width INTEGER NOT NULL DEFAULT 1920,
                    height INTEGER NOT NULL DEFAULT 1080,
                    framerate REAL NOT NULL DEFAULT 30.0,
                    duration REAL NOT NULL DEFAULT 0.0,
                    thumbnail_path TEXT,
                    number_of_media_files INTEGER NOT NULL DEFAULT 1,
                    created_at DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
                    updated_at DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
                    FOREIGN KEY (user_id) REFERENCES users (id)
                );";
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        public int Create(Project p)
        {
            if(p == null)
                throw new ArgumentNullException(nameof(p));
            const string sql = @"
                    INSERT INTO projects(user_id, name, description, width, height, framerate, duration, thumbnail_path, number_of_media_files, created_at, updated_at)
                    VALUES(@user_id, @name, @description, @width, @height, @framerate, @duration, @thumbnail_path, @number_of_media_files, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);
                    SELECT last_insert_rowid();";
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@user_id", p.UserId);
            cmd.Parameters.AddWithValue("@name", p.Name ?? string.Empty);
            cmd.Parameters.AddWithValue("@description", (object?)p.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@width", p.Width);
            cmd.Parameters.AddWithValue("@height", p.Height);
            cmd.Parameters.AddWithValue("@framerate", p.Framerate);
            cmd.Parameters.AddWithValue("@duration", p.Duration);
            cmd.Parameters.AddWithValue("@thumbnail_path", (object?)p.ThumbnailPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@number_of_media_files", p.NumberOfMediaFiles);
            var id = Convert.ToInt32(cmd.ExecuteScalar());
            return id;
        }

        public Project? GetById(int id)
        {
            const string sql = "SELECT * FROM projects WHERE id = @id LIMIT 1;";
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var rdr = cmd.ExecuteReader();
            if(rdr.Read())
                return MapReaderToProject(rdr);
            return null;
        }

        public List<Project> GetAll()
        {
            var list = new List<Project>();
            const string sql = "SELECT * FROM projects ORDER BY created_at DESC;";
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            using var rdr = cmd.ExecuteReader();
            while(rdr.Read())
                list.Add(MapReaderToProject(rdr));
            return list;
        }

        public List<Project> GetByUserId(int userId)  // GetByUserId 
        {
            var list = new List<Project>();
            const string sql = "SELECT * FROM projects WHERE user_id = @userId ORDER BY created_at DESC;";
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            using var rdr = cmd.ExecuteReader();
            while(rdr.Read())
                list.Add(MapReaderToProject(rdr));
            return list;
        }

        public bool Update(Project p) //`Update`
        {
            if(p == null)
                throw new ArgumentNullException(nameof(p));
            const string sql = @"
                UPDATE projects
                SET user_id = @user_id,
                    name = @name,
                    description = @description,
                    width = @width,
                    height = @height,
                    framerate = @framerate,
                    duration = @duration,
                    thumbnail_path = @thumbnail_path,
                    number_of_media_files = @number_of_media_files,
                    updated_at = CURRENT_TIMESTAMP
                WHERE id = @id;";
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@user_id", p.UserId);
            cmd.Parameters.AddWithValue("@name", p.Name ?? string.Empty);
            cmd.Parameters.AddWithValue("@description", (object?)p.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@width", p.Width);
            cmd.Parameters.AddWithValue("@height", p.Height);
            cmd.Parameters.AddWithValue("@framerate", p.Framerate);
            cmd.Parameters.AddWithValue("@duration", p.Duration);
            cmd.Parameters.AddWithValue("@thumbnail_path", (object?)p.ThumbnailPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@number_of_media_files", p.NumberOfMediaFiles);
            cmd.Parameters.AddWithValue("@id", p.Id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            const string sql = "DELETE FROM projects WHERE id = @id;";
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        private static Project MapReaderToProject(IDataRecord r)  // `MapReaderToProject`
        {
            var p = new Project
            {
                Id = Convert.ToInt32(r["id"]),
                UserId = Convert.ToInt32(r["user_id"]),
                Name = r["name"] as string ?? string.Empty,
                Description = r["description"] as string,
                Width = Convert.ToInt32(r["width"]),
                Height = Convert.ToInt32(r["height"]),
                Framerate = Convert.ToDouble(r["framerate"]),
                Duration = Convert.ToDouble(r["duration"]),
                ThumbnailPath = r["thumbnail_path"] as string,
                NumberOfMediaFiles = Convert.ToInt32(r["number_of_media_files"]),
            };

            try
            {
                var created = r["created_at"];
                p.CreatedAt = created == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(created);
            }
            catch { p.CreatedAt = DateTime.MinValue; }

            try
            {
                var updated = r["updated_at"];
                p.UpdatedAt = updated == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(updated);
            }
            catch { p.UpdatedAt = DateTime.MinValue; }

            return p;
        }
    }
}