namespace Agora.Operations.ApplicationOptions.OperationsOptions
{
    public class NotificationQueueOperationsOptions
    {
        public const string NotificationQueues = "NotificationQueues";

        public SendNotificationQueuesCommandOptions SendNotificationQueues { get; set; }

        public class SendNotificationQueuesCommandOptions
        {
            public int MaxRetryCount { get; set; } = 3;
            public int NotificationQueuesToHandle { get; set; } = 120;
        }
    }
}