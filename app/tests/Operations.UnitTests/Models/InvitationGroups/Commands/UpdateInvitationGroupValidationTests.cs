using Agora.Models.Models;
using Agora.Operations.Models.InvitationGroups.Commands.UpdateInvitationGroup;
using Agora.Operations.UnitTests.Common.Behaviours;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.InvitationGroups.Commands
{
    public class UpdateInvitationGroupValidationTests
    {
        private readonly UpdateInvitationGroupCommandValidator _validator = new();

        [Test]
        public async Task UpdateInvitationGroup_ValidRequest_ShouldNotThrowException()
        {
            var command = new UpdateInvitationGroupCommand
            {
                InvitationGroup = new InvitationGroup { Id = 1, Name = "Updated Group Name" }
            };
            await ValidationTestFramework
                .For<UpdateInvitationGroupCommand, InvitationGroup>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task UpdateInvitationGroup_NameExactlyMaxLength_ShouldNotThrowException()
        {
            var command = new UpdateInvitationGroupCommand
            {
                InvitationGroup = new InvitationGroup { Id = 1, Name = new string('a', 50) }
            };
            await ValidationTestFramework
                .For<UpdateInvitationGroupCommand, InvitationGroup>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task UpdateInvitationGroup_ZeroId_ShouldThrowValidationException()
        {
            var command = new UpdateInvitationGroupCommand
            {
                InvitationGroup = new InvitationGroup { Id = 0, Name = "Valid Name" }
            };

            await ValidationTestFramework
                .For<UpdateInvitationGroupCommand, InvitationGroup>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInvitationGroupCommand.InvitationGroup) + "." + nameof(InvitationGroup.Id));
        }

        [Test]
        public async Task UpdateInvitationGroup_EmptyName_ShouldThrowValidationException()
        {
            var command = new UpdateInvitationGroupCommand
            {
                InvitationGroup = new InvitationGroup { Id = 1, Name = "" }
            };

            await ValidationTestFramework
                .For<UpdateInvitationGroupCommand, InvitationGroup>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInvitationGroupCommand.InvitationGroup) + "." + nameof(InvitationGroup.Name));
        }

        [Test]
        public async Task UpdateInvitationGroup_NullName_ShouldThrowValidationException()
        {
            var command = new UpdateInvitationGroupCommand
            {
                InvitationGroup = new InvitationGroup { Id = 1, Name = null }
            };

            await ValidationTestFramework
                .For<UpdateInvitationGroupCommand, InvitationGroup>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInvitationGroupCommand.InvitationGroup) + "." + nameof(InvitationGroup.Name));
        }

        [Test]
        public async Task UpdateInvitationGroup_NameTooLong_ShouldThrowValidationException()
        {
            var command = new UpdateInvitationGroupCommand
            {
                InvitationGroup = new InvitationGroup { Id = 1, Name = new string('a', 51) }
            };

            await ValidationTestFramework
                .For<UpdateInvitationGroupCommand, InvitationGroup>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInvitationGroupCommand.InvitationGroup) + "." + nameof(InvitationGroup.Name));
        }

        [Test]
        public async Task UpdateInvitationGroup_MultipleValidationErrors_ShouldThrowValidationException()
        {
            var command = new UpdateInvitationGroupCommand
            {
                InvitationGroup = new InvitationGroup { Id = 0, Name = "" }
            };

            await ValidationTestFramework
                .For<UpdateInvitationGroupCommand, InvitationGroup>()
                .WithValidators(_validator)
                .ShouldFailValidation(command,
                    nameof(UpdateInvitationGroupCommand.InvitationGroup) + "." + nameof(InvitationGroup.Id),
                    nameof(UpdateInvitationGroupCommand.InvitationGroup) + "." + nameof(InvitationGroup.Name));
        }
    }
}