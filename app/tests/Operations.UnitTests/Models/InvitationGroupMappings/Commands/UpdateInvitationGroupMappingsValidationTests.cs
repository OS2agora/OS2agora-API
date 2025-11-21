using Agora.Models.Models;
using Agora.Operations.Models.InvitationGroupMappings.Commands.UpdateInvitationGroupMappings;
using Agora.Operations.UnitTests.Common.Behaviours;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.InvitationGroupMappings.Commands
{
    public class UpdateInvitationGroupMappingsValidationTests
    {
        private readonly UpdateInvitationGroupMappingsCommandValidator _validator = new();

        [Test]
        public async Task UpdateInvitationGroupMappings_ValidRequest_ShouldNotThrowException()
        {
            var command = new UpdateInvitationGroupMappingsCommand
            {
                HearingTypeId = 1,
                InvitationGroupMappings = new List<InvitationGroupMapping>
                {
                    new InvitationGroupMapping { HearingTypeId = 1, InvitationGroupId = 1 },
                    new InvitationGroupMapping { HearingTypeId = 1, InvitationGroupId = 2 }
                }
            };
            await ValidationTestFramework
                .For<UpdateInvitationGroupMappingsCommand, List<InvitationGroupMapping>>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task UpdateInvitationGroupMappings_EmptyList_ShouldNotThrowException()
        {
            var command = new UpdateInvitationGroupMappingsCommand
            {
                HearingTypeId = 1,
                InvitationGroupMappings = new List<InvitationGroupMapping>()
            };
            await ValidationTestFramework
                .For<UpdateInvitationGroupMappingsCommand, List<InvitationGroupMapping>>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task UpdateInvitationGroupMappings_EmptyHearingTypeId_ShouldThrowValidationException()
        {
            var command = new UpdateInvitationGroupMappingsCommand
            {
                HearingTypeId = 0,
                InvitationGroupMappings = new List<InvitationGroupMapping>
                {
                    new InvitationGroupMapping { HearingTypeId = 1, InvitationGroupId = 1 }
                }
            };

            await ValidationTestFramework
                .For<UpdateInvitationGroupMappingsCommand, List<InvitationGroupMapping>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInvitationGroupMappingsCommand.HearingTypeId));
        }

        [Test]
        public async Task UpdateInvitationGroupMappings_NullInvitationGroupMapping_ShouldThrowValidationException()
        {
            var command = new UpdateInvitationGroupMappingsCommand
            {
                HearingTypeId = 1,
                InvitationGroupMappings = new List<InvitationGroupMapping> { null }
            };

            await ValidationTestFramework
                .For<UpdateInvitationGroupMappingsCommand, List<InvitationGroupMapping>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInvitationGroupMappingsCommand.InvitationGroupMappings) + "[0]");
        }

        [Test]
        public async Task UpdateInvitationGroupMappings_EmptyInvitationGroupId_ShouldThrowValidationException()
        {
            var command = new UpdateInvitationGroupMappingsCommand
            {
                HearingTypeId = 1,
                InvitationGroupMappings = new List<InvitationGroupMapping>
                {
                    new InvitationGroupMapping { HearingTypeId = 1, InvitationGroupId = 0 }
                }
            };

            await ValidationTestFramework
                .For<UpdateInvitationGroupMappingsCommand, List<InvitationGroupMapping>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInvitationGroupMappingsCommand.InvitationGroupMappings) + "[0]." + nameof(InvitationGroupMapping.InvitationGroupId));
        }

        [Test]
        public async Task UpdateInvitationGroupMappings_EmptyHearingTypeIdInMapping_ShouldThrowValidationException()
        {
            var command = new UpdateInvitationGroupMappingsCommand
            {
                HearingTypeId = 1,
                InvitationGroupMappings = new List<InvitationGroupMapping>
                {
                    new InvitationGroupMapping { HearingTypeId = 0, InvitationGroupId = 1 }
                }
            };

            await ValidationTestFramework
                .For<UpdateInvitationGroupMappingsCommand, List<InvitationGroupMapping>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInvitationGroupMappingsCommand.InvitationGroupMappings) + "[0]." + nameof(InvitationGroupMapping.HearingTypeId));
        }

        [Test]
        public async Task UpdateInvitationGroupMappings_MultipleValidationErrors_ShouldThrowValidationException()
        {
            var command = new UpdateInvitationGroupMappingsCommand
            {
                HearingTypeId = 0,
                InvitationGroupMappings = new List<InvitationGroupMapping>
                {
                    new InvitationGroupMapping { HearingTypeId = 0, InvitationGroupId = 0 }
                }
            };

            await ValidationTestFramework
                .For<UpdateInvitationGroupMappingsCommand, List<InvitationGroupMapping>>()
                .WithValidators(_validator)
                .ShouldFailValidation(command,
                    nameof(UpdateInvitationGroupMappingsCommand.HearingTypeId),
                    nameof(UpdateInvitationGroupMappingsCommand.InvitationGroupMappings) + "[0]." +
                    nameof(InvitationGroupMapping.HearingTypeId),
                    nameof(UpdateInvitationGroupMappingsCommand.InvitationGroupMappings) + "[0]." +
                    nameof(InvitationGroupMapping.InvitationGroupId));
        }
    }
}