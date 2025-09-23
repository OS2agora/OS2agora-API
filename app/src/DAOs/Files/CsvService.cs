using BallerupKommune.Operations.Common.Exceptions;
using BallerupKommune.Operations.Common.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BallerupKommune.DAOs.Files
{
    public class CsvService : ICsvService
    {
        public async Task<List<T>> ParseCsv<T>(BallerupKommune.Models.Models.Files.File file) where T : class
        {
            var extension = file.Extension;
            if (extension == null || extension.ToLower() != ".csv")
            {
                throw new GeneralException("File must have the .csv extension");
            }

            List<T> records;

            await using (var ms = new MemoryStream(file.Content))
            {
                using (var reader = new StreamReader(ms))
                {
                    var delimiter = DetectDelimiter(reader);
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture);
                    // Setting delimiter
                    config.Delimiter = delimiter;
                    config.HasHeaderRecord = true;

                    using (var csv = new CsvReader(reader, config))
                    {
                        try
                        {
                            records = csv.GetRecords<T>().ToList();
                        }
                        catch (Exception e)
                        {
                            throw new GeneralException("File content were invalid", e);
                        }
                    }
                }
            };
            return records;
        }

        private string DetectDelimiter(StreamReader reader)
        {
            // assume one of the following delimiters (English and Danish MS Excel CSV delimiters)
            var possibleDelimiters = new List<string> { ",", ";" };

            var headerLine = reader.ReadLine();

            // reset the reader to initial position for outside reuse
            // Eg. Csv helper won't find header line, because it has been read in the Reader
            reader.BaseStream.Position = 0;
            reader.DiscardBufferedData();

            foreach (var possibleDelimiter in possibleDelimiters)
            {
                if (headerLine.Contains(possibleDelimiter))
                {
                    return possibleDelimiter;
                }
            }

            // if code haven't found a the delimiter, thrown an error
            throw new GeneralException($"Delimiter in csv is not supported. Change delimiter to {string.Join(" or ", possibleDelimiters.ToArray())}");

        }
    }
}