using DbDiver.Core;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbDiver.DAL
{
    public class SqliteItemsExtractor : IDatabaseItemsExtractor
    {

        string _connectionString;
        SqliteConnection _connection;

        public SqliteItemsExtractor(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new SqliteConnection(_connectionString);
            SQLitePCL.Batteries.Init();
            _connection.Open();
        }


        public IEnumerable<string> FindValues(DbSearchParameter parameter)
        {
            List<string> result = new List<string>();
            var sqlExpression = $"SELECT * FROM {parameter.TableName} " +
                $"WHERE {parameter.ColumnName} LIKE '%{parameter.SearchItem}%'";
            SqliteCommand command = new SqliteCommand(sqlExpression, _connection);
            SqliteDataReader reader = command.ExecuteReader();            
            while (reader.Read())
            {
                StringBuilder rowPresentation = new StringBuilder();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if(!Convert.IsDBNull(reader.GetName(i)) && !Convert.IsDBNull(reader.GetValue(i)))
                        rowPresentation.Append($"[{reader.GetName(i)}]:{reader.GetValue(i)} ");
                }
                result.Add(rowPresentation.ToString());
            }
            return result;
        }
    }
}
