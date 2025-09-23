using Microsoft.EntityFrameworkCore.Diagnostics;
using DbCommand = System.Data.Common.DbCommand;

namespace BallerupKommune.DAOs.Statistics
{
    internal class CommandCountInterceptor : DbCommandInterceptor
    {
        public ICommandCountStatistics Statistics { get; set; }
        public CommandCountInterceptor(ICommandCountStatistics statistics)
        {
            Statistics = statistics;
        }

        public override DbCommand CommandCreated(CommandEndEventData eventData, DbCommand result)
        {
            Statistics.IncrementCommandCount();
            return base.CommandCreated(eventData, result);
        }
    }
}
