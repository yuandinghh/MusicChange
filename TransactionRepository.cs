using System;
using System.Collections.Generic;
using System.Data.SQLite;
using static MusicChange.db;

namespace MusicChange
{
	public class TransactionRepository
	{
		public TransactionRepository(string dbPath)
		{
			_connectionString = $"Data Source={dbPath};Version=3;";
		}
		// 创建事务
		public int Create(Transaction transaction)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    INSERT INTO transactions (
                        project_id, user_id, transaction_type, description, is_undone
                    ) VALUES (
                        @project_id, @user_id, @transaction_type, @description, @is_undone
                    );
                    SELECT last_insert_rowid();";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@project_id", transaction.ProjectId );
					command.Parameters.AddWithValue( "@user_id", transaction.UserId );
					command.Parameters.AddWithValue( "@transaction_type", transaction.TransactionType );
					command.Parameters.AddWithValue( "@description", transaction.Description );
					command.Parameters.AddWithValue( "@is_undone", transaction.IsUndone );

					return Convert.ToInt32( command.ExecuteScalar() );
				}
			}
		}

		// 根据ID获取事务
		public Transaction GetById(int id)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    SELECT id, project_id, user_id, transaction_type, description, timestamp, is_undone
                    FROM transactions 
                    WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );

					using (var reader = command.ExecuteReader()) {
						if (reader.Read()) {
							return new Transaction
							{
								Id = Convert.ToInt32( reader["id"] ),
								ProjectId = Convert.ToInt32( reader["project_id"] ),
								UserId = Convert.ToInt32( reader["user_id"] ),
								TransactionType = reader["transaction_type"].ToString(),
								Description = reader["description"].ToString(),
								Timestamp = Convert.ToDateTime( reader["timestamp"] ),
								IsUndone = Convert.ToBoolean( reader["is_undone"] )
							};
						}
					}
				}
			}

			return null;
		}

		// 获取项目的所有事务
		public List<Transaction> GetByProjectId(int projectId)
		{
			var transactions = new List<Transaction>();

			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    SELECT id, project_id, user_id, transaction_type, description, timestamp, is_undone
                    FROM transactions 
                    WHERE project_id = @project_id
                    ORDER BY timestamp DESC";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@project_id", projectId );

					using (var reader = command.ExecuteReader()) {
						while (reader.Read()) {
							transactions.Add( new Transaction
							{
								Id = Convert.ToInt32( reader["id"] ),
								ProjectId = Convert.ToInt32( reader["project_id"] ),
								UserId = Convert.ToInt32( reader["user_id"] ),
								TransactionType = reader["transaction_type"].ToString(),
								Description = reader["description"].ToString(),
								Timestamp = Convert.ToDateTime( reader["timestamp"] ),
								IsUndone = Convert.ToBoolean( reader["is_undone"] )
							} );
						}
					}
				}
			}

			return transactions;
		}

		// 获取用户的事务历史
		public List<Transaction> GetByUserId(int userId)
		{
			var transactions = new List<Transaction>();

			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    SELECT id, project_id, user_id, transaction_type, description, timestamp, is_undone
                    FROM transactions 
                    WHERE user_id = @user_id
                    ORDER BY timestamp DESC";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@user_id", userId );

					using (var reader = command.ExecuteReader()) {
						while (reader.Read()) {
							transactions.Add( new Transaction
							{
								Id = Convert.ToInt32( reader["id"] ),
								ProjectId = Convert.ToInt32( reader["project_id"] ),
								UserId = Convert.ToInt32( reader["user_id"] ),
								TransactionType = reader["transaction_type"].ToString(),
								Description = reader["description"].ToString(),
								Timestamp = Convert.ToDateTime( reader["timestamp"] ),
								IsUndone = Convert.ToBoolean( reader["is_undone"] )
							} );
						}
					}
				}
			}

			return transactions;
		}

		// 更新事务（标记为已撤销）
		public bool MarkAsUndone(int id)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = "UPDATE transactions SET is_undone = 1 WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );
					return command.ExecuteNonQuery() > 0;
				}
			}
		}

		// 删除事务
		public bool Delete(int id)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = "DELETE FROM transactions WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );
					return command.ExecuteNonQuery() > 0;
				}
			}
		}
	}
}