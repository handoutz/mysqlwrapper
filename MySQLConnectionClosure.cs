using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MySQLWrapper
{
    public class MySQLConnectionClosure : IDisposable
    {
        public MySqlConnection Connection { get; set; }
        public MySQLConnector Connector { get; set; }

        public MySQLConnectionClosure(MySqlConnection connection, MySQLConnector connector)
        {
            Connection = connection;
            Connector = connector;
        }

        public void Dispose()
        {
            Connector.NotifyDisposed(this);
            Connection.Dispose();
        }

        public static implicit operator MySqlConnection(MySQLConnectionClosure c)
        {
            return c.Connection;
        }
    }
}
