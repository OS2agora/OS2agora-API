using Agora.Models.Common.CustomResponse.SortAndFilter;
using Agora.Operations.Common.Exceptions;
using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Agora.DTOs.Common.CustomResponseDto.Converter
{
    public class SortingParametersConverter : ITypeConverter<SortingParametersDto, SortingParameters>
    {
        public SortingParameters Convert(SortingParametersDto source, SortingParameters destination,
            ResolutionContext context)
        {
            var errors = new Dictionary<string, string[]>();

            Sorting sorting = null;

            if (!String.IsNullOrEmpty(source?.OrderBy))
            {
                try
                {
                    sorting = JsonConvert.DeserializeObject<Sorting>(source.OrderBy);
                }
                catch (Exception)
                {
                    errors.Add(SortAndFilterExceptionTypes.ProcessingError, new[] { "Error reading OrderBy" });
                }
            }

            if (errors.Any())
            {
                throw new SortAndFilterException(errors);
            }

            destination = new SortingParameters
            {
                Sorting = sorting
            };

            return destination;
        }
    }
}
