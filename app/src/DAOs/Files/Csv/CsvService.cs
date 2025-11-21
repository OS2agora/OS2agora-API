using Agora.DAOs.Files.Csv.Mapping;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.Files;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using File = Agora.Models.Models.Files.File;

namespace Agora.DAOs.Files.Csv
{
    public class CsvService : ICsvService
    {
        public async Task<List<T>> ParseCsv<T>(File file, Dictionary<string, string> columnMappings = null) where T : class
        {
            var extension = file.Extension;
            if (extension == null || extension.ToLowerInvariant() != ".csv")
            {
                throw new GeneralException("File must have the .csv extension");
            }

            List<T> records;

            await using (var ms = new MemoryStream(file.Content))
            {
                using (var reader = new StreamReader(ms))
                {
                    var delimiter = DetectDelimiter(reader);
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = delimiter,
                        HasHeaderRecord = true
                    };

                    StripByteOrderMark(reader);

                    using (var csv = new CsvReader(reader, config))
                    {
                        try
                        {
                            if (columnMappings != null)
                            {
                                csv.Context.RegisterClassMap(new DynamicClassMap<T>(columnMappings));
                            }

                            records = csv.GetRecords<T>().ToList();
                        }
                        catch (Exception e)
                        {
                            throw new InvalidFileContentException($"Failed to read {file.Extension} file '{file.Name}'. Invalid content", e);
                        }
                    }
                }
            };

            return records;
        }

        private void StripByteOrderMark(StreamReader reader)
        {
            // Check if stream contains byte order mark (BOM), and reset position to after if so.
            // The CSVReader fails to parse headers if BOM is present...
            byte[] expectedBom = reader.CurrentEncoding.GetPreamble();
            byte[] bomBuffer = new byte[expectedBom.Length];

            reader.BaseStream.Read(bomBuffer, 0, expectedBom.Length);

            if (bomBuffer.SequenceEqual(expectedBom))
            {
                // bom present, reset position to skip it
                reader.BaseStream.Position = expectedBom.Length;
                reader.DiscardBufferedData();
            }
            else
            {
                // bom not present, reset position to 0
                reader.BaseStream.Position = 0;
                reader.DiscardBufferedData();
            }
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