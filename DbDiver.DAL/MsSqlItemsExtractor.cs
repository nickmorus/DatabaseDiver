using DbDiver.Core;
using System;
using System.Collections.Generic;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Text;

namespace DbDiver.DAL
{
    public class MsSqlItemsExtractor : IDatabaseItemsExtractor
    {

        string _connectionString;
        SqlConnection _connection;

        public MsSqlItemsExtractor(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new SqlConnection(_connectionString);
            
            _connection.Open();
        }

        public IEnumerable<string> FindValues(DbSearchParameter parameter)
        {
            List<string> result = new List<string>();
            var sqlExpression = $"SELECT * FROM {parameter.TableName} WHERE {parameter.ColumnName} LIKE '%{parameter.SearchItem}%'";
            SqlCommand command = new SqlCommand(sqlExpression, _connection);
            SqlDataReader reader = command.ExecuteReader();            
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
