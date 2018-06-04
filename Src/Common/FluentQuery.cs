using System;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Apps72.Dev.Data
{
    public partial class FluentQuery
    {
        private DatabaseCommandBase _databaseCommand;

        protected internal FluentQuery(DatabaseCommandBase databaseCommand)
        {
            _databaseCommand = databaseCommand;
        }

        public virtual FluentQuery WithTransaction(DbTransaction transaction)
        {
            _databaseCommand.Transaction = transaction;
            return this;
        }

        public virtual FluentQuery ForSql(string sqlQuery)
        {
            _databaseCommand.CommandText = new StringBuilder(sqlQuery);
            return this;
        }

        public virtual FluentQuery AddParameter<T>(string name, T value)
        {
            _databaseCommand.AddParameter(name, value);
            return this;
        }

        public virtual FluentQuery AddParameter<T>(string name, T value, DbType type)
        {
            _databaseCommand.AddParameter(name, value, type);
            return this;
        }

        public virtual FluentQuery AddParameter<T>(T values)
        {
            _databaseCommand.AddParameter(values);
            return this;
        }

        public virtual T ExecuteRow<T>()
        {
            return _databaseCommand.ExecuteRow<T>();
        }

        public virtual T ExecuteRow<T>(T itemOftype)
        {
            return _databaseCommand.ExecuteRow<T>(itemOftype);
        }

        public virtual object ExecuteScalar()
        {
            return _databaseCommand.ExecuteScalar();
        }

        public virtual T ExecuteScalar<T>()
        {
            return _databaseCommand.ExecuteScalar<T>();
        }
    }
}
