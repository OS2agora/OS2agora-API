using Agora.Models.Models;
using Agora.Models.Models.Files;
using Agora.Models.Models.Multiparts;
using Agora.Operations.ApplicationOptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using HearingStatus = Agora.Models.Enums.HearingStatus;

namespace Agora.Operations.Plugins.Plugins
{
    public class LoggingPlugin : PluginBase
    {
        private readonly ILogger<Hearing> _hearingLogger;
        private readonly ILogger<FileOperation> _fileOperationLogger;
        private readonly ILogger<File> _fileLogger;

        public LoggingPlugin(IServiceProvider serviceProvider, PluginConfiguration pluginConfiguration) : base(serviceProvider, pluginConfiguration)
        {
            _hearingLogger = serviceProvider.GetService<ILogger<Hearing>>();
            _fileOperationLogger = serviceProvider.GetService<ILogger<FileOperation>>();
            _fileLogger = serviceProvider.GetService<ILogger<File>>();
        }

        public override async Task<Hearing> AfterHearingCreate(Hearing model)
        {
            _hearingLogger.LogInformation("Hearing with id: {HearingId} was successfully created and now has status: {HearingStatus}", model.Id, model.HearingStatus.Status);
            return model;
        }

        public override async Task<Hearing> AfterHearingUpdate(Hearing model)
        {
            _hearingLogger.LogInformation("Hearing with id: {HearingId} was successfully updated", model.Id);
            return model;
        }

        public override async Task<Hearing> AfterHearingStatusUpdate(Hearing model, HearingStatus oldStatus)
        {
            _hearingLogger.LogInformation("Hearing status was successfully updated from status {OldHearingStatus} => {NewHearingStatus}", oldStatus, model?.HearingStatus?.Status);
            return model;
        }

        public override async Task<UserHearingRole> AfterUserHearingRoleCreate(UserHearingRole model)
        {
            if (model.HearingRole?.Role != null && model.Hearing?.Id != null)
            {
                _hearingLogger.LogInformation("UserHearingRole with role: {HearingRole} was successfully created on Hearing with id: {HearingId}", model.HearingRole.Role, model.Hearing.Id);
            }

            return model;
        }

        public override async Task AfterHearingDelete(int hearingId)
        {
            _hearingLogger.LogInformation("Successfully deleted hearing with id: {HearingId}", hearingId);
        }

        public override async Task<FileOperation> BeforeFileOperation(FileOperation fileOperation)
        {
            if (fileOperation != null)
            {
                _fileOperationLogger.LogInformation("FileOperation with content Id: {FileOperationContentId} and operation: {FileOperation} has started", fileOperation.ContentId, fileOperation.Operation);
            }

            return fileOperation;
        }

        public override async Task<File> BeforeFileUpload(File file)
        {
            if (file != null)
            {
                _fileLogger.LogInformation("FileOperations on a single file has started.");
            }

            return file;
        }
    }
}