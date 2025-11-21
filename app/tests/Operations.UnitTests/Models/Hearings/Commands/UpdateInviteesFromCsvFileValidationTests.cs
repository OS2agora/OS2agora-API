using Agora.Models.Common.CustomResponse;
using Agora.Models.Models;
using Agora.Models.Models.Files;
using Agora.Models.Models.Invitations;
using Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromCsvFile;
using Agora.Operations.UnitTests.Common.Behaviours;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.Hearings.Commands
{
    public class UpdateInviteesFromCsvFileValidationTests
    {
        private readonly UpdateInviteesFromCsvFileCommandValidator _validator = new();

        [Test]
        public async Task UpdateInviteesFromCsvFile_ValidRequest_ShouldNotThrowException()
        {
            var command = new UpdateInviteesFromCsvFileCommand
            {
                HearingId = 1,
                InvitationSourceId = 1,
                File = new File { Name = "test.csv", Extension = ".csv" }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromCsvFileCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task UpdateInviteesFromCsvFile_ZeroHearingId_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromCsvFileCommand
            {
                HearingId = 0,
                InvitationSourceId = 1,
                File = new File { Name = "test.csv", Extension = ".csv" }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromCsvFileCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInviteesFromCsvFileCommand.HearingId));
        }

        [Test]
        public async Task UpdateInviteesFromCsvFile_ZeroInvitationSourceId_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromCsvFileCommand
            {
                HearingId = 1,
                InvitationSourceId = 0,
                File = new File { Name = "test.csv", Extension = ".csv" }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromCsvFileCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInviteesFromCsvFileCommand.InvitationSourceId));
        }

        [Test]
        public async Task UpdateInviteesFromCsvFile_InvalidFileExtension_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromCsvFileCommand
            {
                HearingId = 1,
                InvitationSourceId = 1,
                File = new File { Name = "test.txt", Extension = ".txt" }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromCsvFileCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInviteesFromCsvFileCommand.File) + "." + nameof(File.Extension));
        }

        [Test]
        public async Task UpdateInviteesFromCsvFile_UppercaseCsvExtension_ShouldNotThrowException()
        {
            var command = new UpdateInviteesFromCsvFileCommand
            {
                HearingId = 1,
                InvitationSourceId = 1,
                File = new File { Name = "test.CSV", Extension = ".CSV" }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromCsvFileCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task UpdateInviteesFromCsvFile_NullFile_ShouldThrowException()
        {
            var command = new UpdateInviteesFromCsvFileCommand
            {
                HearingId = 1,
                InvitationSourceId = 1,
                File = null
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromCsvFileCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldThrowNullReferenceException(command);
        }

        [Test]
        public async Task UpdateInviteesFromCsvFile_MultipleValidationErrors_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromCsvFileCommand
            {
                HearingId = 0,
                InvitationSourceId = 0,
                File = new File { Name = "test.txt", Extension = ".txt" }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromCsvFileCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidation(command,
                    nameof(UpdateInviteesFromCsvFileCommand.HearingId),
                    nameof(UpdateInviteesFromCsvFileCommand.InvitationSourceId),
                    nameof(UpdateInviteesFromCsvFileCommand.File) + "." + nameof(File.Extension));
        }
    }
}