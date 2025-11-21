using Agora.Models.Common.CustomResponse;
using Agora.Models.Models;
using Agora.Models.Models.Invitations;
using Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromInvitationGroup;
using Agora.Operations.UnitTests.Common.Behaviours;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.Hearings.Commands
{
    public class UpdateInviteesFromInvitationGroupValidationTests
    {
        private readonly UpdateInviteesFromInvitationGroupCommandValidator _validator = new();

        [Test]
        public async Task UpdateInviteesFromInvitationGroup_ValidRequest_ShouldNotThrowException()
        {
            var command = new UpdateInviteesFromInvitationGroupCommand
            {
                HearingId = 1,
                InvitationSourceId = 1,
                InvitationGroupId = 1
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromInvitationGroupCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task UpdateInviteesFromInvitationGroup_ZeroHearingId_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromInvitationGroupCommand
            {
                HearingId = 0,
                InvitationSourceId = 1,
                InvitationGroupId = 1
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromInvitationGroupCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInviteesFromInvitationGroupCommand.HearingId));
        }

        [Test]
        public async Task UpdateInviteesFromInvitationGroup_ZeroInvitationSourceId_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromInvitationGroupCommand
            {
                HearingId = 1,
                InvitationSourceId = 0,
                InvitationGroupId = 1
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromInvitationGroupCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInviteesFromInvitationGroupCommand.InvitationSourceId));
        }

        [Test]
        public async Task UpdateInviteesFromInvitationGroup_ZeroInvitationGroupId_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromInvitationGroupCommand
            {
                HearingId = 1,
                InvitationSourceId = 1,
                InvitationGroupId = 0
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromInvitationGroupCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInviteesFromInvitationGroupCommand.InvitationGroupId));
        }

        [Test]
        public async Task UpdateInviteesFromInvitationGroup_MultipleValidationErrors_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromInvitationGroupCommand
            {
                HearingId = 0,
                InvitationSourceId = 0,
                InvitationGroupId = 0
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromInvitationGroupCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidation(command,
                    nameof(UpdateInviteesFromInvitationGroupCommand.HearingId),
                    nameof(UpdateInviteesFromInvitationGroupCommand.InvitationSourceId),
                    nameof(UpdateInviteesFromInvitationGroupCommand.InvitationGroupId));
        }
    }
}