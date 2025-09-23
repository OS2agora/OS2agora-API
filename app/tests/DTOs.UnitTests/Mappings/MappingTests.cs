using AutoMapper;
using BallerupKommune.DTOs.Mappings;
using BallerupKommune.DTOs.Models;
using BallerupKommune.DTOs.Models.Multipart;
using BallerupKommune.Models.Models;
using BallerupKommune.Models.Models.Multiparts;
using NUnit.Framework;
using System;
using System.Runtime.Serialization;
using BallerupKommune.DTOs.Common;

namespace BallerupKommune.DTOs.UnitTests.Mappings
{
    public class MappingTests
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public MappingTests()
        {
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
                cfg.AddProfile<Api.Mappings.MappingProfile>();
            });

            _mapper = _configuration.CreateMapper();
        }

        [Test]
        public void ShouldHaveValidConfiguration()
        {
            _configuration.AssertConfigurationIsValid();
        }

        [Test]
        [TestCase(typeof(CommentDto), typeof(Comment))]
        [TestCase(typeof(CommentDeclineInfoDto), typeof(CommentDeclineInfo))]
        [TestCase(typeof(CommentStatusDto), typeof(CommentStatus))]
        [TestCase(typeof(CommentTypeDto), typeof(CommentType))]
        [TestCase(typeof(ConsentDto), typeof(Consent))]
        [TestCase(typeof(ContentDto), typeof(Content))]
        [TestCase(typeof(ContentTypeDto), typeof(ContentType))]
        [TestCase(typeof(FieldDto), typeof(Field))]
        [TestCase(typeof(FieldTemplateDto), typeof(FieldTemplate))]
        [TestCase(typeof(FieldTypeDto), typeof(FieldType))]
        [TestCase(typeof(FieldTypeSpecificationDto), typeof(FieldTypeSpecification))]
        [TestCase(typeof(GlobalContentDto), typeof(GlobalContent))]
        [TestCase(typeof(GlobalContentTypeDto), typeof(GlobalContentType))]
        [TestCase(typeof(HearingDto), typeof(Hearing))]
        [TestCase(typeof(HearingRoleDto), typeof(HearingRole))]
        [TestCase(typeof(HearingStatusDto), typeof(HearingStatusDto))]
        [TestCase(typeof(HearingTemplateDto), typeof(HearingTemplate))]
        [TestCase(typeof(HearingTypeDto), typeof(HearingType))]
        [TestCase(typeof(KleHierarchyDto), typeof(KleHierarchy))]
        [TestCase(typeof(KleMappingDto), typeof(KleMapping))]
        [TestCase(typeof(SubjectAreaDto), typeof(SubjectArea))]
        [TestCase(typeof(UserCapacityDto), typeof(UserCapacity))]
        [TestCase(typeof(UserDto), typeof(User))]
        [TestCase(typeof(UserHearingRoleDto), typeof(UserHearingRole))]
        [TestCase(typeof(ValidationRuleDto), typeof(ValidationRule))]
        [TestCase(typeof(MultiPartFieldDto), typeof(MultiPartField))]
        [TestCase(typeof(FileOperationDto), typeof(FileOperation))]
        [TestCase(typeof(CompanyDto), typeof(Company))]
        [TestCase(typeof(CompanyHearingRoleDto), typeof(CompanyHearingRole))]
        public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
        {
            var sourceInstance = GetInstanceOf(source);
            var destinationSource = GetInstanceOf(destination);

            // Map from DTO to Model
            _mapper.Map(sourceInstance, source, destination);

            // Map from Model to DTO
            _mapper.Map(destinationSource, destination, source);
        }

        [Test]
        public void MapRelationship_ModelToDto_WhenIdIsNullAndObjectIsNull_ResultsInNull()
        {
            var commentModel = new Comment {Id = 42, CommentParrentId = null, CommentParrent = null};
            CommentDto commentDto = _mapper.Map<Comment, CommentDto>(commentModel);
            Assert.That(commentDto.CommentParrent, Is.Null);
        }
        
        [Test]
        public void MapRelationship_ModelToDto_WhenIdIsNotNullAndObjectIsNull_ResultsInBaseDto()
        {
            const int hearingId = 7;
            var commentModel = new Comment {Id = 42, HearingId = hearingId, Hearing = null};
            
            CommentDto commentDto = _mapper.Map<Comment, CommentDto>(commentModel);
            
            Assert.Multiple(() =>
            {
                Assert.That(commentDto.Hearing.GetType(), Is.EqualTo(typeof(BaseDto<HearingDto>)));
                Assert.That(commentDto.Hearing.Id, Is.EqualTo(hearingId));
            });
        }
        
        [Test]
        public void MapRelationship_ModelToDto_WhenIdIsNotNullAndObjectIsNotNull_ResultsInDerivedDto()
        {
            const int hearingId = 7;
            const string contactPersonEmail = "name@mail.dk";
            var hearingModel = new Hearing {Id = hearingId, ContactPersonEmail = contactPersonEmail};
            var commentModel = new Comment {Id = 42, HearingId = hearingId, Hearing = hearingModel};
            
            CommentDto commentDto = _mapper.Map<Comment, CommentDto>(commentModel);
            
            Assert.Multiple(() =>
            {
                Assert.That(commentDto.Hearing.GetType(), Is.EqualTo(typeof(HearingDto)));
                Assert.That(commentDto.Hearing.Id, Is.EqualTo(hearingId));
                Assert.That(((HearingDto)commentDto.Hearing).ContactPersonEmail, Is.EqualTo(contactPersonEmail));
            });
        }
        
        [Test]
        public void MapRelationship_DtoToModel_WhenRelationshipIsNull_ResultsInNull()
        {
            var commentDto = new CommentDto {Id = 42, CommentParrent = null};
            
            Comment commentModel = _mapper.Map<CommentDto, Comment>(commentDto);
            
            Assert.Multiple(() =>
            {
                Assert.That(commentModel.CommentParrentId, Is.Null);
                Assert.That(commentModel.CommentParrent, Is.Null);
            });
        }
        
        [Test]
        public void MapRelationship_DtoToModel_WhenRelationshipIsNotNull_ResultsInIdBeingPopulated()
        {
            const int parentId = 7;
            var commentDto = new CommentDto {Id = 42, CommentParrent = new BaseDto<CommentDto> {Id = parentId}};
            
            Comment commentModel = _mapper.Map<CommentDto, Comment>(commentDto);
            
            Assert.Multiple(() =>
            {
                Assert.That(commentModel.CommentParrentId, Is.EqualTo(parentId));
                Assert.That(commentModel.CommentParrent, Is.Null);
            });
        }

        private object GetInstanceOf(Type type)
        {
            if (type.GetConstructor(Type.EmptyTypes) != null)
            {
                return Activator.CreateInstance(type);
            }

            // Type without parameterless constructor
            return FormatterServices.GetUninitializedObject(type);
        }
    }
}