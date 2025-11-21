using Agora.Models.Common.CustomResponse;
using Agora.Models.Models;
using Agora.Models.Models.Invitations;
using Agora.Operations.Models.Hearings.Command.DeleteInvitationSource;
using Agora.Operations.UnitTests.Common.Behaviours;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.Hearings.Commands
{
    public class DeleteInvitationSourceValidationTests
    {
        private readonly DeleteInvitationSourceCommandValidator _validator = new();

        [Test]
        public async Task DeleteInvitationSource_ValidRequest_ShouldNotThrowException()
        {
            var command = new DeleteInvitationSourceCommand
            {
                HearingId = 1,
                InvitationSourceName = "Test Source"
            };
            await ValidationTestFramework
                .For<DeleteInvitationSourceCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task DeleteInvitationSource_ZeroHearingId_ShouldThrowValidationException()
        {
            var command = new DeleteInvitationSourceCommand
            {
                HearingId = 0,
                InvitationSourceName = "Test Source"
            };

            await ValidationTestFramework
                .For<DeleteInvitationSourceCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(DeleteInvitationSourceCommand.HearingId));
        }

        [Test]
        public async Task DeleteInvitationSource_EmptyInvitationSourceName_ShouldThrowValidationException()
        {
            var command = new DeleteInvitationSourceCommand
            {
                HearingId = 1,
                InvitationSourceName = ""
            };

            await ValidationTestFramework
                .For<DeleteInvitationSourceCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(DeleteInvitationSourceCommand.InvitationSourceName));
        }

        [Test]
        public async Task DeleteInvitationSource_NullInvitationSourceName_ShouldThrowValidationException()
        {
            var command = new DeleteInvitationSourceCommand
            {
                HearingId = 1,
                InvitationSourceName = null
            };

            await ValidationTestFramework
                .For<DeleteInvitationSourceCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(DeleteInvitationSourceCommand.InvitationSourceName));
        }

        [Test]
        public async Task DeleteInvitationSource_MultipleValidationErrors_ShouldThrowValidationException()
        {
            var command = new DeleteInvitationSourceCommand
            {
                HearingId = 0,
                InvitationSourceName = ""
            };

            await ValidationTestFramework
                .For<DeleteInvitationSourceCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidation(command, 
                    nameof(DeleteInvitationSourceCommand.HearingId), 
                    nameof(DeleteInvitationSourceCommand.InvitationSourceName));
        }
    }
}