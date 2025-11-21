using Agora.Models.Common.CustomResponse.SortAndFilter;
using Agora.Operations.Common.Exceptions;
using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Agora.DTOs.Common.CustomResponseDto.Converter
{
    public class FilterParametersConverter : ITypeConverter<FilterParametersDto, FilterParameters>
    {
        public FilterParameters Convert(FilterParametersDto source, FilterParameters destination,
            ResolutionContext context)
        {
            List<Filter> filters = new List<Filter>();
            var errors = new Dictionary<string, string[]>();
            if (!String.IsNullOrEmpty(source?.Filters))
            {
                try
                {
                    var filtersAsList = JsonConvert.DeserializeObject<List<Filter>>(source.Filters);
                    filters.AddRange(filtersAsList);
                }
                catch (Exception)
                {
                    errors.Add(SortAndFilterExceptionTypes.ProcessingError, new[] { "Error reading filters" });
                }
            }

            if (errors.Any())
            {
                throw new SortAndFilterException(errors);
            }

            destination = new FilterParameters
            {
                Filters = filters
            };

            return destination;
        }
    }
}
