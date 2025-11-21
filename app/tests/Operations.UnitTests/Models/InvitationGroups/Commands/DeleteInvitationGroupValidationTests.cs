using Agora.Operations.Models.InvitationGroups.Commands.DeleteInvitationGroup;
using Agora.Operations.UnitTests.Common.Behaviours;
using MediatR;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.InvitationGroups.Commands
{
    public class DeleteInvitationGroupValidationTests
    {
        private readonly DeleteInvitationGroupCommandValidator _validator = new();

        [Test]
        public async Task DeleteInvitationGroup_ValidRequest_ShouldNotThrowException()
        {
            var command = new DeleteInvitationGroupCommand { Id = 1 };

            await ValidationTestFramework
                .For<DeleteInvitationGroupCommand, Unit>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task DeleteInvitationGroup_NegativeId_ShouldNotThrowException()
        {
            var command = new DeleteInvitationGroupCommand { Id = -1 };

            await ValidationTestFramework
                .For<DeleteInvitationGroupCommand, Unit>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task DeleteInvitationGroup_ZeroId_ShouldThrowValidationException()
        {
            var command = new DeleteInvitationGroupCommand { Id = 0 };

            await ValidationTestFramework
                .For<DeleteInvitationGroupCommand, Unit>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(DeleteInvitationGroupCommand.Id));
        }
    }
}