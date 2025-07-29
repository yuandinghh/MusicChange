using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using static MusicChange.db;

namespace MusicChange
{
	public class TransactionDetailRepository
	{
		private readonly string _connectionString;

		public TransactionDetailRepository(string dbPath)
		{
			_connectionString = $"Data Source={dbPath};Version=3;";
		}

		//public static string dbPath = "D:\\Documents\\Visual Studio 2022\\MusicChange\\LaserEditing.db";
		//private string _connectionString;
		//public db(string dbPath)
		//{
		//	_connectionString = $"Data Source={dbPath};Version=3;";
		//}
		// 创建事务详情
		public int Create(TransactionDetail detail)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    INSERT INTO transaction_details (
                        transaction_id, operation_type, table_name, record_id, old_values, new_values
                    ) VALUES (
                        @transaction_id, @operation_type, @table_name, @record_id, @old_values, @new_values
                    );
                    SELECT last_insert_rowid();";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@transaction_id", detail.TransactionId );
					command.Parameters.AddWithValue( "@operation_type", detail.OperationType );
					command.Parameters.AddWithValue( "@table_name", detail.TableName );
					command.Parameters.AddWithValue( "@record_id", detail.RecordId );
					command.Parameters.AddWithValue( "@old_values", detail.OldValues ?? "" );
					command.Parameters.AddWithValue( "@new_values", detail.NewValues ?? "" );

					return Convert.ToInt32( command.ExecuteScalar() );
				}
			}
		}

		// 根据ID获取事务详情
		public TransactionDetail GetById(int id)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    SELECT id, transaction_id, operation_type, table_name, record_id, old_values, new_values, created_at
                    FROM transaction_details 
                    WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );

					using (var reader = command.ExecuteReader()) {
						if (reader.Read()) {
							return new TransactionDetail
							{
								Id = Convert.ToInt32( reader["id"] ),
								TransactionId = Convert.ToInt32( reader["transaction_id"] ),
								OperationType = reader["operation_type"].ToString(),
								TableName = reader["table_name"].ToString(),
								RecordId = Convert.ToInt32( reader["record_id"] ),
								OldValues = reader["old_values"].ToString(),
								NewValues = reader["new_values"].ToString(),
								CreatedAt = Convert.ToDateTime( reader["created_at"] )
							};
						}
					}
				}
			}

			return null;
		}

		// 获取事务的所有详情
		public List<TransactionDetail> GetByTransactionId(int transactionId)
		{
			var details = new List<TransactionDetail>();

			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    SELECT id, transaction_id, operation_type, table_name, record_id, old_values, new_values, created_at
                    FROM transaction_details 
                    WHERE transaction_id = @transaction_id
                    ORDER BY created_at";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@transaction_id", transactionId );

					using (var reader = command.ExecuteReader()) {
						while (reader.Read()) {
							details.Add( new TransactionDetail
							{
								Id = Convert.ToInt32( reader["id"] ),
								TransactionId = Convert.ToInt32( reader["transaction_id"] ),
								OperationType = reader["operation_type"].ToString(),
								TableName = reader["table_name"].ToString(),
								RecordId = Convert.ToInt32( reader["record_id"] ),
								OldValues = reader["old_values"].ToString(),
								NewValues = reader["new_values"].ToString(),
								CreatedAt = Convert.ToDateTime( reader["created_at"] )
							} );
						}
					}
				}
			}

			return details;
		}

		// 更新事务详情
		public bool Update(TransactionDetail detail)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = @"
                    UPDATE transaction_details 
                    SET transaction_id = @transaction_id, operation_type = @operation_type, 
                        table_name = @table_name, record_id = @record_id, 
                        old_values = @old_values, new_values = @new_values
                    WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@transaction_id", detail.TransactionId );
					command.Parameters.AddWithValue( "@operation_type", detail.OperationType );
					command.Parameters.AddWithValue( "@table_name", detail.TableName );
					command.Parameters.AddWithValue( "@record_id", detail.RecordId );
					command.Parameters.AddWithValue( "@old_values", detail.OldValues ?? "" );
					command.Parameters.AddWithValue( "@new_values", detail.NewValues ?? "" );
					command.Parameters.AddWithValue( "@id", detail.Id );

					return command.ExecuteNonQuery() > 0;
				}
			}
		}

		// 删除事务详情
		public bool Delete(int id)
		{
			using (var connection = new SQLiteConnection( _connectionString )) {
				connection.Open();

				string sql = "DELETE FROM transaction_details WHERE id = @id";

				using (var command = new SQLiteCommand( sql, connection )) {
					command.Parameters.AddWithValue( "@id", id );
					return command.ExecuteNonQuery() > 0;
				}
			}
		}
	}
}