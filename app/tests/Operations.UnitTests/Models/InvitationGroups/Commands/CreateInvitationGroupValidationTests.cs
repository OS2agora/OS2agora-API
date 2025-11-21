using Agora.Models.Models;
using Agora.Operations.Models.InvitationGroups.Commands.CreateInvitationGroup;
using Agora.Operations.UnitTests.Common.Behaviours;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.InvitationGroups.Commands
{
    public class CreateInvitationGroupValidationTests
    {
        private readonly CreateInvitationGroupCommandValidator _validator = new();

        [Test]
        public async Task CreateInvitationGroup_ValidRequest_ShouldNotThrowException()
        {
            var command = new CreateInvitationGroupCommand
            {
                InvitationGroup = new InvitationGroup { Name = "Valid Group Name" }
            };
            await ValidationTestFramework
                .For<CreateInvitationGroupCommand, InvitationGroup>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task CreateInvitationGroup_NameExactlyMaxLength_ShouldNotThrowException()
        {
            var command = new CreateInvitationGroupCommand
            {
                InvitationGroup = new InvitationGroup { Name = new string('a', 50) }
            };
            await ValidationTestFramework
                .For<CreateInvitationGroupCommand, InvitationGroup>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task CreateInvitationGroup_EmptyName_ShouldThrowValidationException()
        {
            var command = new CreateInvitationGroupCommand
            {
                InvitationGroup = new InvitationGroup { Name = "" }
            };

            await ValidationTestFramework
                .For<CreateInvitationGroupCommand, InvitationGroup>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(CreateInvitationGroupCommand.InvitationGroup) + "." + nameof(InvitationGroup.Name));
        }

        [Test]
        public async Task CreateInvitationGroup_NullName_ShouldThrowValidationException()
        {
            var command = new CreateInvitationGroupCommand
            {
                InvitationGroup = new InvitationGroup { Name = null }
            };

            await ValidationTestFramework
                .For<CreateInvitationGroupCommand, InvitationGroup>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(CreateInvitationGroupCommand.InvitationGroup) + "." + nameof(InvitationGroup.Name));
        }

        [Test]
        public async Task CreateInvitationGroup_NameTooLong_ShouldThrowValidationException()
        {
            var command = new CreateInvitationGroupCommand
            {
                InvitationGroup = new InvitationGroup { Name = new string('a', 51) }
            };

            await ValidationTestFramework
                .For<CreateInvitationGroupCommand, InvitationGroup>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(CreateInvitationGroupCommand.InvitationGroup) + "." + nameof(InvitationGroup.Name));
        }

        [Test]
        public async Task CreateInvitationGroup_NullInvitationGroup_ShouldThrowException()
        {
            var command = new CreateInvitationGroupCommand
            {
                InvitationGroup = null
            };

            await ValidationTestFramework
                .For<CreateInvitationGroupCommand, InvitationGroup>()
                .WithValidators(_validator)
                .ShouldThrowNullReferenceException(command);
        }
    }
}