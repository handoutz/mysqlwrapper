using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using MySql.Data.MySqlClient;

namespace MySQLWrapper
{
    public class MySQLConnector
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(MySQLConnector));
        private string ConnectionString { get; set; }
        public ConcurrentBag<MySQLConnectionClosure> Connections { get; set; }

        public MySQLConnector(string connectionString)
        {
            Connections = new ConcurrentBag<MySQLConnectionClosure>();
            ConnectionString = connectionString;
        }

        private MySQLConnectionClosure OpenConnection()
        {
            try
            {
                var conn = new MySqlConnection(ConnectionString);
                conn.Open();
                var closure = new MySQLConnectionClosure(conn, this);
                Connections.Add(closure);
                return closure;
            }
            catch (MySqlException except)
            {
                _log.ErrorFormat("Failed to open connection: {0}", except);
                return null;
            }
        }

        private MySqlCommand Prepare(MySqlConnection conn, string sql, params MySQLParameterPair[] parameters)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            if (parameters.Length > 0)
            {
                foreach (var para in parameters)
                    cmd.Parameters.AddWithValue(para.Name, para.Object);
            }
            cmd.Prepare();
            return cmd;
        }

        public object ExecuteScalar(string sql, params MySQLParameterPair[] paras)
        {
            try
            {
                using (var connection = OpenConnection())
                using (var cmd = Prepare(connection, sql, paras))
                {
                    var result = cmd.ExecuteScalar();
                    return result;
                }
            }
            catch (MySqlException e)
            {
                _log.Error("Error in execscalar", e);
                return null;
            }
        }

        public T ExecuteScalar<T>(string sql, params MySQLParameterPair[] paras)
        {
            try
            {
                using (var connection = OpenConnection())
                using (var cmd = Prepare(connection, sql, paras))
                {
                    var result = cmd.ExecuteScalar();
                    if (result is T)
                        return (T)result;
                    return default(T);
                }
            }
            catch (MySqlException e)
            {
                _log.Error("Error in execscalar<T>", e);
                return default(T);
            }
        }

        public int ExecuteNonQuery(string sql, params MySQLParameterPair[] paras)
        {
            try
            {
                using (var connection = OpenConnection())
                using (var cmd = Prepare(connection, sql, paras))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException e)
            {
                _log.Error("Error in ExecuteNonQuery", e);
                return -1;
            }
        }

        public MySQLReaderClosure ExecuteReader(string sql, params MySQLParameterPair[] paras)
        {
            try
            {
                var conn = OpenConnection();
                var cmd = Prepare(conn, sql, paras);
                return new MySQLReaderClosure(cmd.ExecuteReader(), conn);
            }
            catch (MySqlException e)
            {
                _log.Error("Error in ExecuteReader", e);
                return null;
            }
        }
    }
}
