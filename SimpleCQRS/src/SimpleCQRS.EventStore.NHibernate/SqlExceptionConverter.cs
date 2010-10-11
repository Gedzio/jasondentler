using System;
using NHibernate.Exceptions;

namespace SimpleCQRS.EventStore.NHibernate
{
  public class SqlExceptionConverter : ISQLExceptionConverter 
  {

    public Exception Convert(AdoExceptionContextInfo exInfo)
    {
      var dbException = ADOExceptionHelper.ExtractDbException(exInfo.SqlException);

      var ns = dbException.GetType().Namespace ?? string.Empty;
      if (ns.ToLowerInvariant().StartsWith("system.data.sqlite"))
      {
        // SQLite exception
        switch (dbException.ErrorCode)
        {
          case -2147467259: // Abort due to constraint violation
            throw new ConcurrencyException();
        }
      }

      if (ns.ToLowerInvariant().StartsWith("system.data.sqlclient"))
      {
        // MS SQL Server
        switch (dbException.ErrorCode)
        {
          case -2146232060:
            throw new ConcurrencyException();
        }
      }

      return SQLStateConverter.HandledNonSpecificException(exInfo.SqlException,
          exInfo.Message, exInfo.Sql);
    }

  }
}
