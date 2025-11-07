using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace MusicChange
{
    public class Complaint
    {
        public int Id
        {
            get; set;
        }
        public int? MainId
        {
            get; set;
        }
        public string ComplaintText
        {
            get; set;
        }
        public string Username
        {
            get; set;
        }
        public string ComplaintFeedback
        {
            get; set;
        }
        public DateTime CreatedAt
        {
            get; set;
        }
    }
    public class ComplaintRepository
    {
        private readonly string _connectionString;

        public ComplaintRepository(string dbPath)
        {
            _connectionString = $"Data Source={dbPath};Version=3;";
        }

        public void EnsureTableExists()
        {
            const string sql = @"
CREATE TABLE IF NOT EXISTS complaint (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    Main_id INTEGER,
    complaint TEXT,
    username TEXT,
    complaint_feedback TEXT,
    created_at DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);";
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        public int Create(Complaint model)
        {
            const string sql = @"
INSERT INTO complaint (Main_id, complaint, username, complaint_feedback)
VALUES (@Main_id, @complaint, @username, @complaint_feedback);
SELECT last_insert_rowid();";
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Main_id", (object?)model.MainId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@complaint", model.ComplaintText ?? string.Empty);
            cmd.Parameters.AddWithValue("@username", model.Username ?? string.Empty);
            cmd.Parameters.AddWithValue("@complaint_feedback", model.ComplaintFeedback ?? string.Empty);
            var id = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
            return id;
        }

        public Complaint? GetById(int id)
        {
            const string sql = "SELECT * FROM complaint WHERE id = @id LIMIT 1;";
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var rdr = cmd.ExecuteReader();
            if(rdr.Read())
            {
                return MapReaderToComplaint(rdr);
            }
            return null;
        }

        public List<Complaint> GetAll()
        {
            var list = new List<Complaint>();
            const string sql = "SELECT * FROM complaint ORDER BY created_at DESC;";
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            using var rdr = cmd.ExecuteReader();
            while(rdr.Read())
            {
                list.Add(MapReaderToComplaint(rdr));
            }
            return list;
        }

        public bool UpdateFeedback(int id, int feedback)
        {
            const string sql = "UPDATE complaint SET complaint_feedback = @feedback WHERE id = @id;";
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@feedback", feedback);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        private static Complaint MapReaderToComplaint(IDataRecord r)
        {
            return new Complaint
            {
                Id = Convert.ToInt32(r["id"]),
                MainId = r["Main_id"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["Main_id"]),
                ComplaintText = r["complaint"] == DBNull.Value ? string.Empty : Convert.ToString(r["complaint"]),
                Username = r["username"] == DBNull.Value ? string.Empty : Convert.ToString(r["username"]),
                ComplaintFeedback = r["complaint_feedback"] == DBNull.Value ? string.Empty : Convert.ToString(r["complaint_feedback"]),
                CreatedAt = r["created_at"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["created_at"])
            };
        }
    }
}