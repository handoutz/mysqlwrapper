using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MySQLWrapper
{
    public class MySQLReaderClosure : IDisposable
    {
        public MySqlDataReader Reader { get; set; }
        public MySQLConnectionClosure Connection { get; set; }

        public bool HasRows { get; set; }

        public MySQLReaderClosure(MySqlDataReader reader, MySQLConnectionClosure con)
        {
            if (reader != null)
            {
                Reader = reader;
                HasRows = reader.HasRows;
                Connection = con;
            }
            else
            {
                Connection = con;
                HasRows = false;
            }
        }

        public object this[string col]
        {
            get { return Reader[col]; }
        }

        public T Get<T>(string col)
        {
            var obj = this[col];
            if (obj is T)
                return (T)obj;
            return default(T);
        }

        public bool Read()
        {
            return Reader.Read();
        }

        public void Dispose()
        {
            if (Reader != null)
                Reader.Dispose();
            Connection.Dispose();
        }

        public static implicit operator MySqlDataReader(MySQLReaderClosure clos)
        {
            return clos.Reader;
        }
    }
}
