using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

public class QueryCountInterceptor : DbCommandInterceptor
{
    public int QueryCount { get; private set; }

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        QueryCount++;
        return base.ReaderExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        QueryCount++;
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public void Reset() => QueryCount = 0;
}