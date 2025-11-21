using Agora.Models.Common.CustomResponse;
using Agora.Models.Models;
using Agora.Models.Models.Invitations;
using Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromPersonalInviteeIdentifiers;
using Agora.Operations.UnitTests.Common.Behaviours;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.Hearings.Commands
{
    public class UpdateInviteesFromPersonalInviteeIdentifiersValidationTests
    {
        private readonly UpdateInviteesFromPersonalInviteeIdentifiersCommandValidator _validator = new();

        [Test]
        public async Task UpdateInviteesFromPersonalInviteeIdentifiers_ValidRequest_ShouldNotThrowException()
        {
            var command = new UpdateInviteesFromPersonalInviteeIdentifiersCommand
            {
                HearingId = 1,
                InvitationSourceId = 1,
                InviteeIdentifiers = new List<InviteeIdentifiers>
                {
                    new InviteeIdentifiers { Cpr = "1234567890" },
                    new InviteeIdentifiers { Email = "test@example.com" },
                    new InviteeIdentifiers { Cvr = "12345678" }
                }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromPersonalInviteeIdentifiersCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task UpdateInviteesFromPersonalInviteeIdentifiers_ZeroHearingId_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromPersonalInviteeIdentifiersCommand
            {
                HearingId = 0,
                InvitationSourceId = 1,
                InviteeIdentifiers = new List<InviteeIdentifiers>
                {
                    new InviteeIdentifiers { Cpr = "1234567890" }
                }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromPersonalInviteeIdentifiersCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInviteesFromPersonalInviteeIdentifiersCommand.HearingId));
        }

        [Test]
        public async Task UpdateInviteesFromPersonalInviteeIdentifiers_ZeroInvitationSourceId_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromPersonalInviteeIdentifiersCommand
            {
                HearingId = 1,
                InvitationSourceId = 0,
                InviteeIdentifiers = new List<InviteeIdentifiers>
                {
                    new InviteeIdentifiers { Cpr = "1234567890" }
                }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromPersonalInviteeIdentifiersCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInviteesFromPersonalInviteeIdentifiersCommand.InvitationSourceId));
        }

        [Test]
        public async Task UpdateInviteesFromPersonalInviteeIdentifiers_EmptyInviteeIdentifiers_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromPersonalInviteeIdentifiersCommand
            {
                HearingId = 1,
                InvitationSourceId = 1,
                InviteeIdentifiers = new List<InviteeIdentifiers>()
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromPersonalInviteeIdentifiersCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInviteesFromPersonalInviteeIdentifiersCommand.InviteeIdentifiers));
        }

        [Test]
        public async Task UpdateInviteesFromPersonalInviteeIdentifiers_NullInviteeIdentifiers_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromPersonalInviteeIdentifiersCommand
            {
                HearingId = 1,
                InvitationSourceId = 1,
                InviteeIdentifiers = null
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromPersonalInviteeIdentifiersCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInviteesFromPersonalInviteeIdentifiersCommand.InviteeIdentifiers));
        }

        [Test]
        public async Task UpdateInviteesFromPersonalInviteeIdentifiers_MultipleIdentifiersInSingleItem_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromPersonalInviteeIdentifiersCommand
            {
                HearingId = 1,
                InvitationSourceId = 1,
                InviteeIdentifiers = new List<InviteeIdentifiers>
                {
                    new InviteeIdentifiers
                    {
                        Cpr = "1234567890",
                        Email = "test@example.com"
                    }
                }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromPersonalInviteeIdentifiersCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInviteesFromPersonalInviteeIdentifiersCommand.InviteeIdentifiers) + "[0]");
        }

        [Test]
        public async Task UpdateInviteesFromPersonalInviteeIdentifiers_NoIdentifierProvided_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromPersonalInviteeIdentifiersCommand
            {
                HearingId = 1,
                InvitationSourceId = 1,
                InviteeIdentifiers = new List<InviteeIdentifiers>
                {
                    new InviteeIdentifiers
                    {
                    }
                }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromPersonalInviteeIdentifiersCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInviteesFromPersonalInviteeIdentifiersCommand.InviteeIdentifiers) + "[0]");
        }

        [Test]
        public async Task UpdateInviteesFromPersonalInviteeIdentifiers_NullInviteeIdentifier_ShouldThrowException()
        {
            var command = new UpdateInviteesFromPersonalInviteeIdentifiersCommand
            {
                HearingId = 1,
                InvitationSourceId = 1,
                InviteeIdentifiers = new List<InviteeIdentifiers> { null }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromPersonalInviteeIdentifiersCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldThrowNullReferenceException(command);
        }

        [Test]
        public async Task UpdateInviteesFromPersonalInviteeIdentifiers_AllThreeIdentifiersProvided_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromPersonalInviteeIdentifiersCommand
            {
                HearingId = 1,
                InvitationSourceId = 1,
                InviteeIdentifiers = new List<InviteeIdentifiers>
                {
                    new InviteeIdentifiers
                    {
                        Cpr = "1234567890",
                        Email = "test@example.com",
                        Cvr = "12345678" // All three provided - violates single identifier rule
                    }
                }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromPersonalInviteeIdentifiersCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInviteesFromPersonalInviteeIdentifiersCommand.InviteeIdentifiers) + "[0]");
        }

        [Test]
        public async Task UpdateInviteesFromPersonalInviteeIdentifiers_MultipleValidationErrors_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromPersonalInviteeIdentifiersCommand
            {
                HearingId = 0,
                InvitationSourceId = 0,
                InviteeIdentifiers = new List<InviteeIdentifiers>()
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromPersonalInviteeIdentifiersCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidation(command,
                    nameof(UpdateInviteesFromPersonalInviteeIdentifiersCommand.HearingId),
                    nameof(UpdateInviteesFromPersonalInviteeIdentifiersCommand.InvitationSourceId),
                    nameof(UpdateInviteesFromPersonalInviteeIdentifiersCommand.InviteeIdentifiers));
        }
    }
}