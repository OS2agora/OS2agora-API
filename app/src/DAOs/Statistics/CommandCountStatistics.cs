using System.Threading;

namespace Agora.DAOs.Statistics
{
    public interface ICommandCountStatistics
    {
        long CommandCount { get; }
        long IncrementCommandCount();
        void ResetCommandCount();
    }

    public class CommandCountStatistics : ICommandCountStatistics
    {
        private long _commandCount = 0;

        public long CommandCount => _commandCount;

        public long IncrementCommandCount()
        {
            return Interlocked.Increment(ref _commandCount);
        }

        public void ResetCommandCount()
        {
            _commandCount = 0;
        }
    }
}
