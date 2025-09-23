using System.Collections.Generic;
using System.Linq;

namespace BallerupKommune.Models.Common
{
    /// <summary>
    /// Composite object for include data.
    /// </summary>
    public class IncludeProperties
    {
        private readonly List<string> _requestIncludes;
        private readonly List<string> _systemIncludes;
        private readonly List<string> _defaultIncludes;

        public IEnumerable<string> RequestIncludes => _requestIncludes;
        public IEnumerable<string> AllIncludes =>
            _requestIncludes.Concat(_systemIncludes).Concat(_defaultIncludes).Distinct();

        public IncludeProperties(List<string> requestIncludes = null, List<string> systemIncludes = null, List<string> defaultIncludes = null)
        {
            _requestIncludes = requestIncludes ?? new List<string>();
            _systemIncludes = systemIncludes ?? new List<string>();
            _defaultIncludes = defaultIncludes ?? new List<string>();
        }

        /// <summary>
        /// Create <see cref="IncludeProperties"/> with default includes.
        /// </summary>
        /// <typeparam name="TModel">The model to get the default includes from.</typeparam>
        /// <returns>Include properties with default includes.</returns>
        public static IncludeProperties Create<TModel>() where TModel : BaseModel
        {
            return new IncludeProperties(null, null, GetDefaultIncludePropertyNames<TModel>());
        }

        /// <summary>
        /// Create <see cref="IncludeProperties"/> with default includes and supplied request and system includes.
        /// </summary>
        /// <param name="requestIncludes">The request includes.</param>
        /// <param name="systemIncludes">The system includes.</param>
        /// <typeparam name="TModel">The model to get the default includes from.</typeparam>
        /// <returns>Include properties.</returns>
        public static IncludeProperties Create<TModel>(List<string> requestIncludes, List<string> systemIncludes)
            where TModel : BaseModel
        {
            return new IncludeProperties(requestIncludes, systemIncludes, GetDefaultIncludePropertyNames<TModel>());
        }

        public bool IsSystemInclude(string include) => _systemIncludes.Contains(include);
        public void AddSystemInclude(string include)
        {
            _systemIncludes.Add(include);
        }
        public void AddSystemIncludes(IEnumerable<string> includes)
        {
            _systemIncludes.AddRange(includes);
        }

        private static List<string> GetDefaultIncludePropertyNames<TModel>() where TModel : BaseModel
        {
            return typeof(TModel).GetProperty("DefaultIncludes")?.GetValue(null) as List<string> ?? new List<string>();
        }
    }
}
