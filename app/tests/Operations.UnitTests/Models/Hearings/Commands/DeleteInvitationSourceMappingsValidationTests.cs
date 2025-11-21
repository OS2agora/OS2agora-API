using Agora.Models.Common.CustomResponse;
using Agora.Models.Models;
using Agora.Models.Models.Invitations;
using Agora.Operations.Models.Hearings.Command.DeleteInvitationSourceMappings;
using Agora.Operations.UnitTests.Common.Behaviours;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.Hearings.Commands
{
    public class DeleteInvitationSourceMappingsValidationTests
    {
        private readonly DeleteInvitationSourceMappingsCommandValidator _validator = new();

        [Test]
        public async Task DeleteInvitationSourceMappings_ValidRequest_ShouldNotThrowException()
        {
            var command = new DeleteInvitationSourceMappingsCommand
            {
                HearingId = 1,
                InvitationSourceMappingIds = new List<int> { 1, 2, 3 },
                DeleteFromAllInvitationSources = false
            };
            await ValidationTestFramework
                .For<DeleteInvitationSourceMappingsCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task DeleteInvitationSourceMappings_WithDeleteFromAllFlag_ShouldNotThrowException()
        {
            var command = new DeleteInvitationSourceMappingsCommand
            {
                HearingId = 1,
                InvitationSourceMappingIds = new List<int> { 1 },
                DeleteFromAllInvitationSources = true
            };
            await ValidationTestFramework
                .For<DeleteInvitationSourceMappingsCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task DeleteInvitationSourceMappings_ZeroHearingId_ShouldThrowValidationException()
        {
            var command = new DeleteInvitationSourceMappingsCommand
            {
                HearingId = 0,
                InvitationSourceMappingIds = new List<int> { 1, 2, 3 },
                DeleteFromAllInvitationSources = false
            };

            await ValidationTestFramework
                .For<DeleteInvitationSourceMappingsCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(DeleteInvitationSourceMappingsCommand.HearingId));
        }

        [Test]
        public async Task DeleteInvitationSourceMappings_EmptyInvitationSourceMappingIds_ShouldThrowValidationException()
        {
            var command = new DeleteInvitationSourceMappingsCommand
            {
                HearingId = 1,
                InvitationSourceMappingIds = new List<int>(),
                DeleteFromAllInvitationSources = false
            };

            await ValidationTestFramework
                .For<DeleteInvitationSourceMappingsCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(DeleteInvitationSourceMappingsCommand.InvitationSourceMappingIds));
        }

        [Test]
        public async Task DeleteInvitationSourceMappings_NullInvitationSourceMappingIds_ShouldThrowValidationException()
        {
            var command = new DeleteInvitationSourceMappingsCommand
            {
                HearingId = 1,
                InvitationSourceMappingIds = null,
                DeleteFromAllInvitationSources = false
            };

            await ValidationTestFramework
                .For<DeleteInvitationSourceMappingsCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(DeleteInvitationSourceMappingsCommand.InvitationSourceMappingIds));
        }

        [Test]
        public async Task DeleteInvitationSourceMappings_MultipleValidationErrors_ShouldThrowValidationException()
        {
            var command = new DeleteInvitationSourceMappingsCommand
            {
                HearingId = 0,
                InvitationSourceMappingIds = new List<int>(),
                DeleteFromAllInvitationSources = false
            };

            await ValidationTestFramework
                .For<DeleteInvitationSourceMappingsCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidation(command, 
                    nameof(DeleteInvitationSourceMappingsCommand.HearingId),
                    nameof(DeleteInvitationSourceMappingsCommand.InvitationSourceMappingIds));
        }
    }
}