using Agora.Models.Common.CustomResponse;
using Agora.Models.Models;
using Agora.Models.Models.Files;
using Agora.Models.Models.Invitations;
using Agora.Operations.Models.Hearings.Command.UpdateInvitees.UpdateInviteesFromExcelFile;
using Agora.Operations.UnitTests.Common.Behaviours;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Agora.Operations.UnitTests.Models.Hearings.Commands
{
    public class UpdateInviteesFromExcelFileValidationTests
    {
        private readonly UpdateInviteesFromExcelFileCommandValidator _validator = new();

        [Test]
        public async Task UpdateInviteesFromExcelFile_ValidRequest_ShouldNotThrowException()
        {
            var command = new UpdateInviteesFromExcelFileCommand
            {
                HearingId = 1,
                InvitationSourceId = 1,
                File = new File { Name = "test.xlsx", Extension = ".xlsx" }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromExcelFileCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task UpdateInviteesFromExcelFile_ZeroHearingId_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromExcelFileCommand
            {
                HearingId = 0,
                InvitationSourceId = 1,
                File = new File { Name = "test.xlsx", Extension = ".xlsx" }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromExcelFileCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInviteesFromExcelFileCommand.HearingId));
        }

        [Test]
        public async Task UpdateInviteesFromExcelFile_ZeroInvitationSourceId_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromExcelFileCommand
            {
                HearingId = 1,
                InvitationSourceId = 0,
                File = new File { Name = "test.xlsx", Extension = ".xlsx" }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromExcelFileCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInviteesFromExcelFileCommand.InvitationSourceId));
        }

        [Test]
        public async Task UpdateInviteesFromExcelFile_InvalidFileExtension_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromExcelFileCommand
            {
                HearingId = 1,
                InvitationSourceId = 1,
                File = new File { Name = "test.xls", Extension = ".xls" }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromExcelFileCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidationWithError(command, nameof(UpdateInviteesFromExcelFileCommand.File) + "." + nameof(File.Extension));
        }

        [Test]
        public async Task UpdateInviteesFromExcelFile_UppercaseXlsxExtension_ShouldNotThrowException()
        {
            var command = new UpdateInviteesFromExcelFileCommand
            {
                HearingId = 1,
                InvitationSourceId = 1,
                File = new File { Name = "test.XLSX", Extension = ".XLSX" }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromExcelFileCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldPassValidation(command);
        }

        [Test]
        public async Task UpdateInviteesFromExcelFile_NullFile_ShouldThrowException()
        {
            var command = new UpdateInviteesFromExcelFileCommand
            {
                HearingId = 1,
                InvitationSourceId = 1,
                File = null
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromExcelFileCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldThrowNullReferenceException(command);
        }

        [Test]
        public async Task UpdateInviteesFromExcelFile_MultipleValidationErrors_ShouldThrowValidationException()
        {
            var command = new UpdateInviteesFromExcelFileCommand
            {
                HearingId = 0,
                InvitationSourceId = 0,
                File = new File { Name = "test.pdf", Extension = ".pdf" }
            };

            await ValidationTestFramework
                .For<UpdateInviteesFromExcelFileCommand, MetaDataResponse<Hearing, InvitationMetaData>>()
                .WithValidators(_validator)
                .ShouldFailValidation(command,
                    nameof(UpdateInviteesFromExcelFileCommand.HearingId),
                    nameof(UpdateInviteesFromExcelFileCommand.InvitationSourceId),
                    nameof(UpdateInviteesFromExcelFileCommand.File) + "." + nameof(File.Extension));
        }
    }
}