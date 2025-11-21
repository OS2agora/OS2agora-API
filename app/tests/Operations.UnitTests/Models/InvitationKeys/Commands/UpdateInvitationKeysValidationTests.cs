using Agora.Models.Models;
using Agora.Operations.Models.InvitationKeys.Commands.UpdateInvitationKeys;
using Agora.Operations.UnitTests.Common.Behaviours;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.InvitationKeys.Commands
{
    public class UpdateInvitationKeysValidationTests
    {
        private readonly UpdateInvitationKeysCommandValidator _validator = new();

        [Test]
        public async Task UpdateInvitationKeys_ValidRequest_ShouldNotThrowException()
        {
            var command = new UpdateInvitationKeysCommand
            {
                InvitationGroupId = 1,
                InvitationKeys = new List<InvitationKey>
                {
                    new InvitationKey { InvitationGroupId = 1, Cpr = "1234567890" },
                    new InvitationKey { InvitationGroupId = 1, Email = "test@example.com" }
                }
            };
            await ValidationTestFramework
                .For<UpdateInvitationKeysCommand, List<InvitationKey>>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task UpdateInvitationKeys_EmptyList_ShouldNotThrowException()
        {
            var command = new UpdateInvitationKeysCommand
            {
                InvitationGroupId = 1,
                InvitationKeys = new List<InvitationKey>()
            };
            await ValidationTestFramework
                .For<UpdateInvitationKeysCommand, List<InvitationKey>>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task UpdateInvitationKeys_EmptyInvitationGroupId_ShouldThrowValidationException()
        {
            var command = new UpdateInvitationKeysCommand
            {
                InvitationGroupId = 0,
                InvitationKeys = new List<InvitationKey>
                {
                    new InvitationKey { InvitationGroupId = 1, Cpr = "1234567890" }
                }
            };

            await ValidationTestFramework
                .For<UpdateInvitationKeysCommand, List<InvitationKey>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInvitationKeysCommand.InvitationGroupId));
        }

        [Test]
        public async Task UpdateInvitationKeys_MultipleIdentifiers_ShouldThrowValidationException()
        {
            var command = new UpdateInvitationKeysCommand
            {
                InvitationGroupId = 1,
                InvitationKeys = new List<InvitationKey>
                {
                    new InvitationKey
                    {
                        InvitationGroupId = 1,
                        Cpr = "1234567890",
                        Email = "test@example.com"
                    }
                }
            };

            await ValidationTestFramework
                .For<UpdateInvitationKeysCommand, List<InvitationKey>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInvitationKeysCommand.InvitationKeys) + "[0]");
        }

        [Test]
        public async Task UpdateInvitationKeys_MultipleValidationErrors_ShouldThrowValidationException()
        {
            var command = new UpdateInvitationKeysCommand
            {
                InvitationGroupId = 0,
                InvitationKeys = new List<InvitationKey>
                {
                    new InvitationKey
                    {
                        InvitationGroupId = 0,
                    }
                }
            };

            await ValidationTestFramework
                .For<UpdateInvitationKeysCommand, List<InvitationKey>>()
                .WithValidators(_validator)
                .ShouldFailValidation(command,
                    nameof(UpdateInvitationKeysCommand.InvitationGroupId),
                    nameof(UpdateInvitationKeysCommand.InvitationKeys) + "[0]",
                    nameof(UpdateInvitationKeysCommand.InvitationKeys) + "[0]." + nameof(InvitationKey.InvitationGroupId));
        }
    }
}