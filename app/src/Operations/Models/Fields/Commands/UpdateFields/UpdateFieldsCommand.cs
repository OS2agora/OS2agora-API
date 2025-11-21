using Agora.Models.Enums;
using Agora.Models.Models;
using Agora.Models.Models.Multiparts;
using Agora.Operations.Common.Exceptions;
using Agora.Operations.Common.Interfaces;
using Agora.Operations.Common.Interfaces.DAOs;
using Agora.Operations.Common.Interfaces.Plugins;
using MediatR;
using NovaSec.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Agora.Models.Common;
using ContentType = Agora.Models.Enums.ContentType;
using FieldType = Agora.Models.Enums.FieldType;
using FieldTypeModel = Agora.Models.Models.FieldType;
using HearingStatus = Agora.Models.Enums.HearingStatus;
using InvalidOperationException = Agora.Operations.Common.Exceptions.InvalidOperationException;
using Agora.Operations.Common.Interfaces.Files;

namespace Agora.Operations.Models.Fields.Commands.UpdateFields
{
    [PreAuthorize("@Security.IsHearingOwnerByHearingId(#request.HearingId)")]
    public class UpdateFieldsCommand : IRequest<List<Content>>
    {
        public IEnumerable<MultiPartField> Fields { get; set; }
        public int HearingId { get; set; }
        public int HearingStatusId { get; set; }
        public bool NotifyAboutChanges { get; set; }

        public class UpdateFieldsCommandHandler : IRequestHandler<UpdateFieldsCommand, List<Content>>
        {
            private readonly IHearingDao _hearingDao;
            private readonly IContentDao _contentDao;
            private readonly IContentTypeDao _contentTypeDao;
            private readonly IFileService _fileService;
            private readonly IHearingStatusDao _hearingStatusDao;
            private readonly IPluginService _pluginService;
            private readonly IFieldsValidator _fieldsValidator;
            private readonly IDateTime _dateTimeService;

            public UpdateFieldsCommandHandler(IHearingDao hearingDao, IContentDao contentDao,
                IContentTypeDao contentTypeDao, IFileService fileService, IHearingStatusDao hearingStatusDao, IPluginService pluginService, IFieldsValidator fieldsValidator, IDateTime dateTimeService
                )
            {
                _hearingDao = hearingDao;
                _contentDao = contentDao;
                _contentTypeDao = contentTypeDao;
                _fileService = fileService;
                _hearingStatusDao = hearingStatusDao;
                _pluginService = pluginService;
                _fieldsValidator = fieldsValidator;
                _dateTimeService = dateTimeService;
            }

            public async Task<List<Content>> Handle(UpdateFieldsCommand request, CancellationToken cancellationToken)
            {
                var includes = IncludeProperties.Create<Hearing>(null, new List<string>
                {
                    nameof(Hearing.HearingStatus),
                    nameof(Hearing.HearingType),
                    $"{nameof(Hearing.HearingType)}.{nameof(HearingType.HearingTemplate)}",
                    $"{nameof(Hearing.HearingType)}.{nameof(HearingType.HearingTemplate)}.{nameof(HearingTemplate.Fields)}",
                    $"{nameof(Hearing.HearingType)}.{nameof(HearingType.HearingTemplate)}.{nameof(HearingTemplate.Fields)}.{nameof(Field.FieldType)}",
                    $"{nameof(Hearing.HearingType)}.{nameof(HearingType.HearingTemplate)}.{nameof(HearingTemplate.Fields)}.{nameof(Field.FieldType)}.{nameof(FieldTypeModel.FieldTypeSpecifications)}",
                });
                var currentHearing = await _hearingDao.GetAsync(request.HearingId, includes);
                var newHearingStatus = await _hearingStatusDao.GetAsync(request.HearingStatusId);

                if (currentHearing == null)
                {
                    throw new NotFoundException(nameof(Hearing), request.HearingId);
                }

                if (newHearingStatus == null)
                {
                    throw new NotFoundException(nameof(HearingStatus), request.HearingStatusId);
                }

                if (currentHearing.HearingStatus.Status == HearingStatus.CONCLUDED)
                {
                    throw new InvalidOperationException("Cannot update content on concluded hearing");
                }

                if (currentHearing.HearingStatus.Status == HearingStatus.DRAFT &&
                    newHearingStatus.Status != HearingStatus.DRAFT &&
                    newHearingStatus.Status != HearingStatus.AWAITING_STARTDATE)
                {
                    throw new InvalidOperationException($"Invalid status change from: DRAFT => {newHearingStatus.Status}");
                }
                if (currentHearing.HearingStatus.Status == HearingStatus.AWAITING_CONCLUSION &&
                         newHearingStatus.Status != HearingStatus.AWAITING_CONCLUSION &&
                         newHearingStatus.Status != HearingStatus.CONCLUDED)
                {
                    throw new InvalidOperationException($"Invalid status change from: AWAITING_CONCLUSION => {newHearingStatus.Status}");
                }
                if (currentHearing.HearingStatus.Status != HearingStatus.AWAITING_CONCLUSION &&
                    currentHearing.HearingStatus.Status != HearingStatus.DRAFT &&
                    currentHearing.HearingStatus.Status != newHearingStatus.Status)
                {
                    throw new InvalidOperationException($"Cannot update status from: {newHearingStatus.Status}");
                }

                var requestList = request.Fields.ToList();
                await _fieldsValidator.ValidateMultipartFields(request.HearingId, requestList);

                // Checking if hearing has a conclusion - if not throw an error.
                
                var hearingConclusion = requestList.SingleOrDefault(x => x.FieldType == FieldType.CONCLUSION);
                if (newHearingStatus.Status == HearingStatus.CONCLUDED && (hearingConclusion == null || (hearingConclusion != null && string.IsNullOrEmpty(hearingConclusion.Content))))
                {
                    throw new EmptyConclusionException("Cannot conclude a hearing without a conclusion.");
                }

                var allFileOperations = request.Fields.SelectMany(field => field.FileOperations).ToList();
                allFileOperations = await _pluginService.BeforeFileOperation(allFileOperations);
                if (allFileOperations.Any(fileOperation => fileOperation.MarkedByScanner))
                {
                    var errors = allFileOperations.Where(fileOperation => fileOperation.MarkedByScanner)
                        .Select(fileOperation => fileOperation.File.Name);
                    throw new FileUploadException(errors);
                }

                var result = new List<Content>();
                var shouldNotifyAboutChanges = false;

                foreach (var requestField in request.Fields)
                {
                    var matchingHearingField = currentHearing.HearingType.HearingTemplate.Fields.SingleOrDefault(x => x.FieldType.Type == requestField.FieldType);

                    if (matchingHearingField == null)
                    {
                        continue;
                    }

                    if (matchingHearingField.FieldType.Type == FieldType.CONCLUSION &&
                        currentHearing.HearingStatus.Status != HearingStatus.AWAITING_CONCLUSION)
                    {
                        throw new InvalidOperationException("Cannot update conclusion on hearing. It requires a status of Awaiting Conclusion");
                    }

                    if (matchingHearingField.FieldType.Type != FieldType.CONCLUSION &&
                        currentHearing.HearingStatus.Status == HearingStatus.AWAITING_CONCLUSION)
                    {
                        throw new InvalidOperationException($"Cannot update {matchingHearingField.FieldType.Type} on hearing when status is Awaiting Conclusion");
                    }

                    var createdContent = await UpdateField(requestField, matchingHearingField, currentHearing);
                    result.AddRange(createdContent);

                    if (matchingHearingField.FieldType.Type == FieldType.CONCLUSION)
                    {
                        await _pluginService.NotifyAfterConclusionPublished(currentHearing.Id);
                        currentHearing.PropertiesUpdated.Add(nameof(Hearing.ConcludedDate));
                        currentHearing.ConcludedDate = _dateTimeService.Now;

                    }

                    if (matchingHearingField.FieldType.Type == FieldType.TITLE ||
                        matchingHearingField.FieldType.Type == FieldType.SUMMARY ||
                        matchingHearingField.FieldType.Type == FieldType.BODYINFORMATION)
                    {
                        if (request.NotifyAboutChanges)
                        {
                            shouldNotifyAboutChanges = true;
                        }
                    }
                }

                if (shouldNotifyAboutChanges)
                {
                    await _pluginService.NotifyAfterHearingChanged(currentHearing.Id);
                }

                if (currentHearing.HearingStatus.Status != newHearingStatus.Status)
                {
                    currentHearing.HearingStatusId = newHearingStatus.Id;
                    currentHearing.HearingStatus = newHearingStatus;
                    await _hearingDao.UpdateAsync(currentHearing);
                    await _pluginService.NotifyAfterChangeHearingStatus(currentHearing.Id);
                }

                // Refetch updated/created contents to ensure that changes to all related entities are reflected in the final result
                var resultIncludes = IncludeProperties.Create<Content>(null, new List<string>
                {
                    nameof(Content.Field), 
                    $"{nameof(Content.Field)}.{nameof(Field.FieldType)}", 
                    nameof(Content.ContentType), 
                    nameof(Content.Hearing)
                });
                var resultIds = result.Select(content => content.Id).ToList();
                var finalResult =
                    await _contentDao.GetAllAsync(resultIncludes, content => resultIds.Contains(content.Id));

                return finalResult;
            }

            private async Task<IEnumerable<Content>> UpdateField(MultiPartField multiPartField, Field hearingField, Hearing currentHearing)
            {
                var includes = IncludeProperties.Create<Agora.Models.Models.ContentType>(null,
                    new List<string> {nameof(Agora.Models.Models.ContentType.FieldTypeSpecifications)});
                var contentTypes = await _contentTypeDao.GetAllAsync(includes);
                var textContentType = contentTypes.Single(contentType => contentType.Type == ContentType.TEXT);
                var fileContentType = contentTypes.Single(contentType => contentType.Type == ContentType.FILE);

                var createdTextContent = new List<Content>();
                var createdFileContent = new List<Content>();

                // Determines via the fieldTypeSpecification if the field can have text and/or files
                var fieldHasText = hearingField.FieldType.FieldTypeSpecifications.Any(fieldTypeSpecification =>
                    textContentType.FieldTypeSpecifications.Any(f => f.Id == fieldTypeSpecification.Id));
                var fieldHasFiles = hearingField.FieldType.FieldTypeSpecifications.Any(fieldTypeSpecification =>
                    fileContentType.FieldTypeSpecifications.Any(f => f.Id == fieldTypeSpecification.Id));
                
                if (!fieldHasText && !fieldHasFiles)
                {
                    throw new ArgumentOutOfRangeException();
                }
                if (fieldHasText)
                {
                    var newContent = await HandleTextContent(multiPartField.Content, multiPartField.IsEmptyContent, hearingField, currentHearing, textContentType);
                    createdTextContent.Add(newContent);
                }
                if (fieldHasFiles)
                {
                    var newFileContent = await HandleFileContent(multiPartField.FileOperations, hearingField, currentHearing, fileContentType);
                    createdFileContent.AddRange(newFileContent);
                }

                var createdContent = createdTextContent.Union(createdFileContent);
                return createdContent;
            }

            private async Task<Content> HandleTextContent(string content, bool isEmptyContent, Field hearingField, Hearing currentHearing,
                Agora.Models.Models.ContentType textContentType)
            {
                // There can only be one text FieldType, and that should be updated if it exists
                var includes = IncludeProperties.Create<Content>(null, new List<string> 
                {
                    $"{nameof(Content.Field)}.{nameof(Field.FieldType)}",
                    nameof(Content.ContentType),
                    nameof(Content.Hearing)
                });
                var existingContentFromDb = await _contentDao.GetAllAsync(includes);
                var existingTextContent = existingContentFromDb.SingleOrDefault(x => x.Field != null && x.Field.Id == hearingField.Id && x.ContentType.Type == ContentType.TEXT && x.Hearing.Id == currentHearing.Id);

                if (existingTextContent == null)
                {
                    return await _contentDao.CreateAsync(new Content
                    {
                        ContentTypeId = textContentType.Id,
                        FieldId = hearingField.Id,
                        HearingId = currentHearing.Id,
                        TextContent = content
                    });
                }

                // Text content should only be overwritten if it has a value or explicitly set to be empty
                if (content != null || isEmptyContent)
                {
                    existingTextContent.TextContent = content;
                }

                existingTextContent.PropertiesUpdated = new List<string> { nameof(Content.TextContent) };
                includes = IncludeProperties.Create<Content>(null, new List<string>
                {
                    nameof(Content.Field),
                    nameof(Content.ContentType),
                    nameof(Content.Hearing)
                });
                return await _contentDao.UpdateAsync(existingTextContent, includes);
            }

            private async Task<IEnumerable<Content>> HandleFileContent(List<FileOperation> fileOperations, Field hearingField, Hearing currentHearing, Agora.Models.Models.ContentType fileContentType)
            {
                // There can be many file FieldType, and the request should indicate if they should be deleted or created
                var includes = IncludeProperties.Create<Content>(null, new List<string>
                {
                    $"{nameof(Content.Field)}.{nameof(Field.FieldType)}", 
                    nameof(Content.ContentType)
                });
                var existingContentFromDb = await _contentDao.GetAllAsync(includes);
                var existingFileContent = existingContentFromDb.Where(x => x.Field != null && x.Field.Id == hearingField.Id && x.ContentType.Type == ContentType.FILE).ToList();

                var result = new List<Content>();

                if (fileOperations != null)
                {
                    foreach (var fileOperation in fileOperations.Where(x => x.Operation == FileOperationEnum.DELETE))
                    {
                        var contentToDelete = existingFileContent.SingleOrDefault(x => x.Id == fileOperation.ContentId);

                        if (contentToDelete != null)
                        {
                            _fileService.DeleteFileFromDisk(contentToDelete.FilePath);
                            await _contentDao.DeleteAsync(contentToDelete.Id);
                        }
                    }

                    foreach (var fileOperation in fileOperations.Where(x => x.Operation == FileOperationEnum.ADD))
                    {
                        var filePath = await _fileService.SaveFieldFileToDisk(fileOperation.File.Content, currentHearing.Id, hearingField.Id);
                        includes = IncludeProperties.Create<Content>(null, new List<string>
                        {
                            nameof(Content.Field),
                            nameof(Content.ContentType),
                            nameof(Content.Hearing)
                        });

                        var fileContent = await _contentDao.CreateAsync(new Content
                        {
                            ContentTypeId = fileContentType.Id,
                            FieldId = hearingField.Id,
                            HearingId = currentHearing.Id,
                            FileName = fileOperation.File.Name,
                            FileContentType = fileOperation.File.ContentType,
                            FilePath = filePath
                        }, includes);
                        result.Add(fileContent);
                    }
                }

                return result;
            }
        }
    }
}