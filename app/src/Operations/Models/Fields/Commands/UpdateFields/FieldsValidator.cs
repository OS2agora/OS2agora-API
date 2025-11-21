using Agora.Models.Enums;
using Agora.Models.Models;
using Agora.Models.Models.Multiparts;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agora.Models.Common;
using Agora.Operations.Common.Interfaces;
using ContentType = Agora.Models.Enums.ContentType;

namespace Agora.Operations.Models.Fields.Commands.UpdateFields
{
    public class FieldsValidator : IFieldsValidator
    {
        private readonly IHearingDao _hearingDao;

        public FieldsValidator(IHearingDao hearingDao)
        {
            _hearingDao = hearingDao;
        }

        public async Task ValidateMultipartFields(int hearingId, List<MultiPartField> multiPartFields)
        {
            var systemIncludes = new List<string>
            {
                nameof(Hearing.HearingType),
                $"{nameof(Hearing.HearingType)}.{nameof(HearingType.HearingTemplate)}",
                $"{nameof(Hearing.HearingType)}.{nameof(HearingType.HearingTemplate)}.{nameof(HearingTemplate.Fields)}",
                $"{nameof(Hearing.HearingType)}.{nameof(HearingType.HearingTemplate)}.{nameof(HearingTemplate.Fields)}.{nameof(Field.FieldType)}",
                $"{nameof(Hearing.HearingType)}.{nameof(HearingType.HearingTemplate)}.{nameof(HearingTemplate.Fields)}.{nameof(Field.ValidationRule)}",
                $"{nameof(Hearing.HearingType)}.{nameof(HearingType.HearingTemplate)}.{nameof(HearingTemplate.Fields)}.{nameof(Field.Contents)}.{nameof(Content.ContentType)}",
                $"{nameof(Hearing.HearingType)}.{nameof(HearingType.HearingTemplate)}.{nameof(HearingTemplate.Fields)}.{nameof(Field.Contents)}.{nameof(Content.Hearing)}"
            };

            var hearing = await _hearingDao.GetAsync(hearingId,IncludeProperties.Create<Hearing>(null, systemIncludes));

            var fields = hearing.HearingType.HearingTemplate.Fields;
            var errors = new Dictionary<string, string[]>();

            foreach (var multiPartField in multiPartFields)
            {
                var field = fields.SingleOrDefault(f => f.FieldType.Type == multiPartField.FieldType);
                if (field?.ValidationRule == null)
                {
                    continue;
                }

                var rules = field.ValidationRule;

                var errorsForField = new List<string>();

                if (rules.CanBeEmpty.HasValue)
                {
                    if (!rules.CanBeEmpty.Value && string.IsNullOrEmpty(multiPartField.Content))
                    {
                        errorsForField.Add($"Validation error on field {multiPartField.FieldType} - content cannot be empty");
                    }
                }

                if (rules.MaxLength.HasValue && !string.IsNullOrEmpty(multiPartField.Content))
                {
                    if (multiPartField.Content?.Length > rules.MaxLength)
                    {
                        errorsForField.Add($"Validation error on field {multiPartField.FieldType} - content length exceeds the limit");
                    }
                }

                if (rules.MinLength.HasValue && !string.IsNullOrEmpty(multiPartField.Content))
                {
                    if (multiPartField.Content?.Length < rules.MinLength)
                    {
                        errorsForField.Add($"Validation error on field {multiPartField.FieldType} - content length is smaller than minimum");
                    }
                }

                if (rules.MaxFileSize.HasValue)
                {
                    var maxByteArrayLength = rules.MaxFileSize.Value;
                    foreach (var fileOperation in multiPartField.FileOperations)
                    {
                        if (fileOperation.Operation == FileOperationEnum.ADD)
                        {
                            if (fileOperation.File.Content.Length > maxByteArrayLength)
                            {
                                errorsForField.Add($"Validation error on field {multiPartField.FieldType} - file size exceeds limit");
                            }
                        }    
                    }
                }

                if (rules.MaxFileCount.HasValue)
                {
                    // Get how many files are currently present
                    var currentFiles = field.Contents.Where(content => content.Hearing.Id == hearingId && content.ContentType.Type == ContentType.FILE).ToList();
                    var fileCount = currentFiles.Count();

                    // Compute how many files after delete operation
                    var filesToDelete = currentFiles.Where(content => multiPartField.FileOperations.Any(fileOperation =>
                        fileOperation.Operation == FileOperationEnum.DELETE && fileOperation.ContentId == content.Id));
                    fileCount -= filesToDelete.Count();

                    // Compute how many files after add operations
                    fileCount += multiPartField.FileOperations.Count(fileOperation => fileOperation.Operation == FileOperationEnum.ADD);
                    
                    // Check against MaxFileCount
                    if (fileCount > rules.MaxFileCount.Value)
                    {
                        errorsForField.Add($"Validation error on field {multiPartField.FieldType} - file count exceeded limit");
                    }
                }

                if (rules.AllowedFileTypes != null && rules.AllowedFileTypes.Length > 0)
                {
                    var filesToValidate = multiPartField.FileOperations
                        .Where(op => op.Operation == FileOperationEnum.ADD).Select(op => op.File);

                    foreach (var file in filesToValidate)
                    {
                        if (rules.AllowedFileTypes.All(fileType => fileType != file.ContentType))
                        {
                            errorsForField.Add("Validation error on field {multiPartField.FieldType} - file type was not found in list of valid file types for this field");
                        }
                    }
                }

                if (errorsForField.Any())
                {
                    errors.Add(field.FieldType.Type.ToString(), errorsForField.ToArray());
                }
            }

            if (errors.Any())
            {
                throw new ValidationException(errors);
            }
        }
    }
}
