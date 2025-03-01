using System;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SQLCover.Gateway;
using SQLCover.Objects;
using SQLCover.Source;

namespace SQLCover.Trace
{
    class TraceControllerBuilder
    {
        public TraceController GetTraceController(DatabaseGateway gateway, string databaseName, TraceControllerType type)
        {
            switch(type)
            {
                case TraceControllerType.Azure:
                    return new AzureTraceController(gateway, databaseName);
                case TraceControllerType.Sql:
                    return new SqlTraceController(gateway, databaseName);
                case TraceControllerType.SqlLocalDb:
                    return new SqlLocalDbTraceController(gateway, databaseName);
            }

            var source = new DatabaseSourceGateway(gateway);
            var isAzure = source.IsAzure();

            if(!isAzure)
                return new SqlTraceController(gateway, databaseName);
            
            var version = source.GetVersion();
            if(version < SqlServerVersion.Sql120)
                throw  new Exception("SQL Azure is only supported from Version 12");

            return new AzureTraceController(gateway, databaseName);
        }
    }

    public enum TraceControllerType
    {
        Default,
        Sql,
        Azure,
        Exp,
        SqlLocalDb
    }
}