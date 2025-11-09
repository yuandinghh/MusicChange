////csharp Data/MainRepository.cs
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SQLite;

//namespace MusicChange
//{


//    public class MainRepository
//    {
//        private readonly string _connectionString;

//        public MainRepository(string dbPath)
//        {
//            _connectionString = $"Data Source={dbPath};Version=3;";
//        }

//        public void EnsureTableExists()
//        {
//            const string sql = @"
//CREATE TABLE IF NOT EXISTS main (
//  id INTEGER PRIMARY KEY AUTOINCREMENT,
//  curren_user_id INTEGER,
//  curren_project_id INTEGER,
//  login_time DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
//  workofftime DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
//  version TEXT,
//  first_version TEXT,
//  server_website TEXT,
//  complaint_count INTEGER,
//  complaint_id INTEGER,
//  is_locked INTEGER NOT NULL DEFAULT 0,
//  current_run INTEGER NOT NULL DEFAULT 0,
//  the_next_revision_schedule INTEGER,
//  version_end_time DATETIME,
//  registered_user TEXT,
//  description TEXT,
//  created_at DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP)
//);";
//            using var conn = new SQLiteConnection(_connectionString);
//            conn.Open();
//            using var cmd = new SQLiteCommand(sql, conn);
//            cmd.ExecuteNonQuery();
//        }

//        public int Create(Main model)
//        {
//            const string sql = @"
//INSERT INTO main (
//  curren_user_id, curren_project_id, login_time, workofftime, version,
//  first_version, server_website, complaint_count, complaint_id, is_locked,
//  current_run, the_next_revision_schedule, version_end_time, registered_user, description
//) VALUES (
//  @curren_user_id, @curren_project_id, @login_time, @workofftime, @version,
//  @first_version, @server_website, @complaint_count, @complaint_id, @is_locked,
//  @current_run, @the_next_revision_schedule, @version_end_time, @registered_user, @description
//);
//SELECT last_insert_rowid();";

//            using var conn = new SQLiteConnection(_connectionString);
//            conn.Open();
//            using var cmd = new SQLiteCommand(sql, conn);
//            AddParameters(cmd, model);
//            var id = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
//            return id;
//        }

//        public Main? GetById(int id)
//        {
//            const string sql = "SELECT * FROM main WHERE id = @id LIMIT 1;";
//            using var conn = new SQLiteConnection(_connectionString);
//            conn.Open();
//            using var cmd = new SQLiteCommand(sql, conn);
//            cmd.Parameters.AddWithValue("@id", id);
//            using var rdr = cmd.ExecuteReader();
//            if(rdr.Read())
//                return MapReaderToMain(rdr);
//            return null;
//        }

//        public Main? GetCurrentRunning()
//        {
//            const string sql = "SELECT * FROM main WHERE current_run = 1 ORDER BY id DESC LIMIT 1;";
//            using var conn = new SQLiteConnection(_connectionString);
//            conn.Open();
//            using var cmd = new SQLiteCommand(sql, conn);
//            using var rdr = cmd.ExecuteReader();
//            if(rdr.Read())
//                return MapReaderToMain(rdr);
//            return null;
//        }

//        public List<Main> GetAll()
//        {
//            var list = new List<Main>();
//            const string sql = "SELECT * FROM main ORDER BY id DESC;";
//            using var conn = new SQLiteConnection(_connectionString);
//            conn.Open();
//            using var cmd = new SQLiteCommand(sql, conn);
//            using var rdr = cmd.ExecuteReader();
//            while(rdr.Read())
//                list.Add(MapReaderToMain(rdr));
//            return list;
//        }

//        public bool Update(Main model)
//        {
//            const string sql = @"
//UPDATE main SET
//  curren_user_id = @curren_user_id,
//  curren_project_id = @curren_project_id,
//  login_time = @login_time,
//  workofftime = @workofftime,
//  version = @version,
//  first_version = @first_version,
//  server_website = @server_website,
//  complaint_count = @complaint_count,
//  complaint_id = @complaint_id,
//  is_locked = @is_locked,
//  current_run = @current_run,
//  the_next_revision_schedule = @the_next_revision_schedule,
//  version_end_time = @version_end_time,
//  registered_user = @registered_user,
//  description = @description
//WHERE id = @id;
//";
//            using var conn = new SQLiteConnection(_connectionString);
//            conn.Open();
//            using var cmd = new SQLiteCommand(sql, conn);
//            AddParameters(cmd, model);
//            cmd.Parameters.AddWithValue("@id", model.Id);
//            return cmd.ExecuteNonQuery() > 0;
//        }

//        public bool Delete(int id)
//        {
//            const string sql = "DELETE FROM main WHERE id = @id;";
//            using var conn = new SQLiteConnection(_connectionString);
//            conn.Open();
//            using var cmd = new SQLiteCommand(sql, conn);
//            cmd.Parameters.AddWithValue("@id", id);
//            return cmd.ExecuteNonQuery() > 0;
//        }

//        internal static void AddParameters(SQLiteCommand cmd, Main m)
//        {
//            cmd.Parameters.AddWithValue("@curren_user_id", (object?)m.CurrenUserId ?? DBNull.Value);
//            cmd.Parameters.AddWithValue("@curren_project_id", (object?)m.CurrenProjectId ?? DBNull.Value);
//            cmd.Parameters.AddWithValue("@login_time", m.LoginTime);
//            cmd.Parameters.AddWithValue("@workofftime", m.Workofftime);
//            cmd.Parameters.AddWithValue("@version", (object?)m.version ?? DBNull.Value);
//            cmd.Parameters.AddWithValue("@first_version", (object?)m.first_version ?? DBNull.Value);
//            cmd.Parameters.AddWithValue("@server_website", (object?)m.Server_website ?? DBNull.Value);
//            cmd.Parameters.AddWithValue("@complaint_count", (object?)m.complaint_count ?? DBNull.Value);
//            cmd.Parameters.AddWithValue("@complaint_id", (object?)m.complaint_id ?? DBNull.Value);
//            cmd.Parameters.AddWithValue("@is_locked", m.IsLocked ? 1 : 0);
//            cmd.Parameters.AddWithValue("@current_run", m.current_run ? 1 : 0);
//            cmd.Parameters.AddWithValue("@the_next_revision_schedule", (object?)m.The_next_revision_schedule ?? DBNull.Value);
//            cmd.Parameters.AddWithValue("@version_end_time", (object?)m.Version_end_time ?? DBNull.Value);
//            cmd.Parameters.AddWithValue("@registered_user", (object?)m.registered_user ?? DBNull.Value);
//            cmd.Parameters.AddWithValue("@description", (object?)m.Description ?? DBNull.Value);
//        }

//        private static Main MapReaderToMain(IDataRecord r)
//        {
//            var m = new Main
//            {
//                Id = Convert.ToInt32(r["id"]),
//                CurrenUserId =Convert.ToInt32(r["curren_user_id"]),
//                CurrenProjectId = Convert.ToInt32(r["curren_project_id"]),
//                //LoginTime =Convert.ToDateTime(r["login_time"]),
//                //Workofftime =  Convert.ToDateTime(r["workofftime"]),
//                //version =  Convert.ToString(r["version"]),
//                //first_version =Convert.ToString(r["first_version"]),
//                //Server_website =  Convert.ToString(r["server_website"]),
//                //complaint_count = Convert.ToInt32(r["complaint_count"]),
//                //complaint_id = Convert.ToInt32(r["complaint_id"]),
//                //IsLocked = Convert.ToInt32(r["is_locked"]) != 0,
//                //current_run = Convert.ToInt32(r["current_run"]) != 0,
//                //The_next_revision_schedule = Convert.ToInt32(r["the_next_revision_schedule"]),
//                //Version_end_time =Convert.ToDateTime(r["version_end_time"]),
//                //registered_user = Convert.ToString(r["registered_user"]),
//                //Description = Convert.ToString(r["description"]),
//                //CreatedAt = Convert.ToDateTime(r["created_at"])

//            };
//            return m;

//        }
//    }
//}

//csharp Data/MainRepository.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace MusicChange
{
    public class MainRepository
    {
        private readonly string _connectionString;

        public MainRepository(string dbPath)
        {
            _connectionString = $"Data Source={dbPath};Version=3;";
        }

        public void EnsureTableExists()  //创建表
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
                      complaint_count INTEGER,
                      complaint_id INTEGER,
                      is_locked INTEGER NOT NULL DEFAULT 0,
                      current_run BOOL NOT NULL DEFAULT 0,
                      the_next_revision_schedule INTEGER,
                      version_end_time INTEGER,
                      registered_user TEXT,
                      description TEXT,
                      created_at DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP)
                    );";
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        public int Create(Main model)   //创插入 表
        {
            const string sql = @"
                    INSERT INTO main (
                      curren_user_id, curren_project_id, login_time, workofftime, version,
                      first_version, server_website, complaint_count, complaint_id, is_locked,
                      current_run, the_next_revision_schedule, version_end_time, registered_user, description
                    ) VALUES (
                      @curren_user_id, @curren_project_id, @login_time, @workofftime, @version,
                      @first_version, @server_website, @complaint_count, @complaint_id, @is_locked,
                      @current_run, @the_next_revision_schedule, @version_end_time, @registered_user, @description
                    );
                    SELECT last_insert_rowid();";

            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            AddParameters(cmd, model);
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result ?? 0);
        }

        public Main? GetById(int id)  //查询 
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

        public Main? GetCurrentRunning()   //查询当前运行
        {
            const string sql = "SELECT * FROM main WHERE current_run = 1 ORDER BY id DESC LIMIT 1;";
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            using var rdr = cmd.ExecuteReader();
            if(rdr.Read())
                return MapReaderToMain(rdr);
            return null;
        }

        public List<Main> GetAll()          //查询所有
        {
            var list = new List<Main>();
            const string sql = "SELECT * FROM main ORDER BY id DESC;";
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            using var rdr = cmd.ExecuteReader();
            while(rdr.Read())
                list.Add(MapReaderToMain(rdr));
            return list;
        }

        public bool Update(Main model)   //修改
        {
            const string sql = @"
                UPDATE main SET
                  curren_user_id = @curren_user_id,
                  curren_project_id = @curren_project_id,
                  login_time = @login_time,
                  workofftime = @workofftime,
                  version = @version,
                  first_version = @first_version,
                  server_website = @server_website,
                  complaint_count = @complaint_count,
                  complaint_id = @complaint_id,
                  is_locked = @is_locked,
                  current_run = @current_run,
                  the_next_revision_schedule = @the_next_revision_schedule,
                  version_end_time = @version_end_time,
                  registered_user = @registered_user,
                  description = @description
                WHERE id = @id;
                ";
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand(sql, conn);
            AddParameters(cmd, model);
            cmd.Parameters.AddWithValue("@id", model.Id);
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

        internal static void AddParameters(SQLiteCommand cmd, Main m)   //添加参数
        {
            cmd.Parameters.AddWithValue("@curren_user_id", (object?)m.CurrenUserId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@curren_project_id", (object?)m.CurrenProjectId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@login_time", m.LoginTime);
            cmd.Parameters.AddWithValue("@workofftime", m.Workofftime);
            cmd.Parameters.AddWithValue("@version", (object?)m.version ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@first_version", (object?)m.first_version ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@server_website", (object?)m.Server_website ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@complaint_count", (object?)m.complaint_count ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@complaint_id", (object?)m.complaint_id ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@is_locked", m.IsLocked ? 1 : 0);
            cmd.Parameters.AddWithValue("@current_run", m.current_run ? 1 : 0);
            cmd.Parameters.AddWithValue("@the_next_revision_schedule", (object?)m.The_next_revision_schedule ?? DBNull.Value);
			cmd.Parameters.AddWithValue("@version_end_time", (object?)m.Version_end_time ??  DBNull.Value);
            cmd.Parameters.AddWithValue("@registered_user", (object?)m.registered_user ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@description", (object?)m.Description ?? DBNull.Value);
        }

        private static Main MapReaderToMain(IDataRecord r)   //映射
        {
            return new Main
            {
                Id = Convert.ToInt32(r["id"]),
                CurrenUserId = r["curren_user_id"] == DBNull.Value ? 0 : Convert.ToInt32(r["curren_user_id"]),
                CurrenProjectId = r["curren_project_id"] == DBNull.Value ? 0 : Convert.ToInt32(r["curren_project_id"]),
                LoginTime = r["login_time"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["login_time"]),
                Workofftime = r["workofftime"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["workofftime"]),
                version = r["version"] == DBNull.Value ? string.Empty : Convert.ToString(r["version"]),
                first_version = r["first_version"] == DBNull.Value ? string.Empty : Convert.ToString(r["first_version"]),
                Server_website = r["server_website"] == DBNull.Value ? string.Empty : Convert.ToString(r["server_website"]),
                complaint_count = r["complaint_count"] == DBNull.Value ? 0 : Convert.ToInt32(r["complaint_count"]),
                complaint_id = r["complaint_id"] == DBNull.Value ? 0 : Convert.ToInt32(r["complaint_id"]),
                IsLocked = r["is_locked"] != DBNull.Value && Convert.ToInt32(r["is_locked"]) != 0,
                current_run = r["current_run"] != DBNull.Value && Convert.ToInt32(r["current_run"]) != 0,
                The_next_revision_schedule = r["the_next_revision_schedule"] == DBNull.Value ? 0 : Convert.ToInt32(r["the_next_revision_schedule"]),
                Version_end_time = r["version_end_time"] == DBNull.Value ? 0 : Convert.ToInt32(r["version_end_time"]),
                registered_user = r["registered_user"] == DBNull.Value ? string.Empty : Convert.ToString(r["registered_user"]),
                Description = r["description"] == DBNull.Value ? string.Empty : Convert.ToString(r["description"]),
                CreatedAt = r["created_at"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["created_at"])
            };
        }
    }
}
