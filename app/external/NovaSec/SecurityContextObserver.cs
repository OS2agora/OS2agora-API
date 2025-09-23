using NovaSec.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NovaSec
{
    public interface ISecurityContextObserver
    {
        void BeginExpressionCompilation(BaseAttribute attribute);
        void EndExpressionCompilation(BaseAttribute attribute);
        void BeginExpressionEvaluation(BaseAttribute attribute);
        void EndExpressionEvaluation(BaseAttribute attribute);
        void BeginPostFilter();
        void EndPostFilter();
        void BeginPreAuthorize();
        void EndPreAuthorize();
        void OnPostFilterPass(object model);
        void OnPostFilterFail(object model);
        double AccumulatedCompilationDuration { get; }
        double AccumulatedEvaluationDuration { get; }
        double AccumulatedPostFilterDuration { get; }
        double AccumulatedPreAuthorizeDuration { get; }
        long PostFilterTotal { get; }
        long PostFilterPassed { get; }
        long PostFilterFailed { get; }
    }

    public class SecurityContextObserver : ISecurityContextObserver
    {
        private double _accumulatedEvaluationDurationTicks = 0;
        private long _postFilterPassed = 0;
        private long _postFilterFailed = 0;
        private double _accumulatedCompilationDurationTicks = 0;
        private double _accumulatedPostFilterDuration = 0;
        private double _accumulatedPreAuthorizeDuration = 0;
        private readonly Dictionary<BaseAttribute, Stopwatch> _evaluationTimers = new Dictionary<BaseAttribute, Stopwatch>();
        private readonly Dictionary<BaseAttribute, Stopwatch> _compilationTimers = new Dictionary<BaseAttribute, Stopwatch>();
        private readonly Stopwatch _postFilterStopWatch = new Stopwatch();
        private readonly Stopwatch _preAuthorizeStopWatch = new Stopwatch();

        public double AccumulatedCompilationDuration => Math.Round(_accumulatedCompilationDurationTicks / 10000);
        public double AccumulatedEvaluationDuration => Math.Round(_accumulatedEvaluationDurationTicks / 10000);
        public double AccumulatedPostFilterDuration => _accumulatedPostFilterDuration;
        public double AccumulatedPreAuthorizeDuration => _accumulatedPreAuthorizeDuration;
        public long PostFilterTotal => _postFilterFailed + _postFilterPassed;
        public long PostFilterPassed => _postFilterPassed;
        public long PostFilterFailed => _postFilterFailed;


        private static void BeginTimer(Dictionary<BaseAttribute, Stopwatch> timers, BaseAttribute attribute)
        {
            timers.Add(attribute, new Stopwatch());
            timers[attribute].Start();
        }

        private static double EndTimer(Dictionary<BaseAttribute, Stopwatch> timers, BaseAttribute attribute)
        {
            double result = 0;
            if (!timers.ContainsKey(attribute)) return result;
            var sw = timers[attribute];
            sw.Stop();
            result = sw.ElapsedTicks;
            timers.Remove(attribute);

            return result;
        }

        public void BeginExpressionCompilation(BaseAttribute attribute)
        {
            BeginTimer(_compilationTimers, attribute);
        }

        public void EndExpressionCompilation(BaseAttribute attribute)
        {
            _accumulatedCompilationDurationTicks += EndTimer(_compilationTimers, attribute);
        }
        
        public void BeginExpressionEvaluation(BaseAttribute attribute)
        {
            BeginTimer(_evaluationTimers, attribute);
        }

        public void EndExpressionEvaluation(BaseAttribute attribute)
        {
            _accumulatedEvaluationDurationTicks += EndTimer(_evaluationTimers, attribute);
        }

        public void OnPostFilterPass(object model)
        {
            _postFilterPassed++;
        }

        public void OnPostFilterFail(object model)
        {
            _postFilterFailed++;
        }

        public void BeginPostFilter()
        {
            _postFilterStopWatch.Restart();
        }
        public void EndPostFilter()
        {
            _postFilterStopWatch.Stop();
            _accumulatedPostFilterDuration += _postFilterStopWatch.ElapsedMilliseconds;
        }

        public void BeginPreAuthorize()
        {
            _preAuthorizeStopWatch.Restart();
        }

        public void EndPreAuthorize()
        {
            _preAuthorizeStopWatch.Stop();
            _accumulatedPreAuthorizeDuration += _preAuthorizeStopWatch.ElapsedMilliseconds;
        }
    }
}
