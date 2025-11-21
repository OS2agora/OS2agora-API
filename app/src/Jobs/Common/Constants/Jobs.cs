namespace Jobs.Common.Constants
{
    public static class Jobs
    {
        public static class HearingStatusChecker
        {
            public const string TriggerIdentity = "Hearing Status Checker Trigger";
            public const string TriggerDescription = "Trigger for the Staus Checker Job";
            public const string JobIdentity = "Hearing Status Checker";
            public const string JobDescription = "Check the status of Hearings";
            public const string CronSchedule = "0 0/2 * * * ?"; // Every 2 minute on the minute
            public const int LockTimeout = 600; // Time in seconds before releasing lock
            public const int WaitTimeout = 20; // Time in seconds, until process will stop attempting to acquire lock
            public const int RetryTime = 5; // Time in seconds before retrying to acquire lock
        }

        public static class HearingJournalizer
        {
            public const string TriggerIdentity = "Hearing Journalizer Trigger";
            public const string TriggerDescription = "Trigger for the Hearing Journalizer Job";
            public const string JobIdentity = "Hearing Journalizer";
            public const string JobDescription = "Journalize hearings";
            public const string CronSchedule = "0 0 * * * ?"; // Every hour on the hour
            public const int LockTimeout = 600; // Time in seconds before releasing lock
            public const int WaitTimeout = 20; // Time in seconds, until process will stop attempting to acquire lock
            public const int RetryTime = 5; // Time in seconds before retrying to acquire lock
        }

        public static class CreateNotificationQueue
        {
            public const string TriggerIdentity = "Create Notification Queue Trigger";
            public const string TriggerDescription = "Trigger for the Creare Notification Queue Job";
            public const string JobIdentity = "Create Notification Queue";
            public const string JobDescription = "Check for Notification elements and create NotificationQueue elements";
            public const string CronSchedule = "0 0/5 * * * ?"; // Every 5 minute on the minute
            public const int LockTimeout = 600; // Time in seconds before releasing lock
            public const int WaitTimeout = 20; // Time in seconds, until process will stop attempting to acquire lock
            public const int RetryTime = 5; // Time in seconds before retrying to acquire lock
        }

        public static class CreateDailyStatusNotifications
        {
            public const string TriggerIdentity = "Create Daily Status Notifications Trigger";
            public const string TriggerDescription = "Trigger for the Create Daily Status Notifications Job";
            public const string JobIdentity = "Create Daily Status Notifications";
            public const string JobDescription = "Check for Daily Status Notifications and create Notification/Event elements";
            public const string CronSchedule = "0 0 7 * * ?"; // Every day at 07:00
            public const int LockTimeout = 600; // Time in seconds before releasing lock
            public const int WaitTimeout = 300; // Time in seconds, until process will stop attempting to acquire lock
            public const int RetryTime = 10; // Time in seconds before retrying to acquire lock
        }

        public static class NotificationQueueSender
        {
            public const string TriggerIdentity = "NotificationQueue Sender Trigger";
            public const string TriggerDescription = "Trigger for the NotificationQueue Sender Job";
            public const string JobIdentity = "NotificationQueue Sender";
            public const string JobDescription = "Check for NotificationQueue elements and try to send them";
            public const string CronSchedule = "0 0/1 * * * ?"; // Every minute on the minute
            // The lock timeout is set based on the expected max time to send 120 notifications
            // Average time to send a notification is approximately 1 second per notification
            // We estimate worst case to be 30 seconds per notification
            // NotificationsToHandle default Value is 120
            // LockTimeout = 120 notifications * 30 seconds/notification = 3600 seconds
            public const int LockTimeout = 3600; // Time in seconds before releasing lock
            public const int WaitTimeout = 20; // Time in seconds, until process will stop attempting to acquire lock
            public const int RetryTime = 5; // Time in seconds before retrying to acquire lock
        }

        public static class UpdateNotificationQueueDeliveryStatus
        {
            public const string TriggerIdentity = "Update NotificationQueue Delivery Status Trigger";
            public const string TriggerDescription = "Trigger for the Update Notification Queue Delivery Status Job";
            public const string JobIdentity = "Update Notification Queue Delivery Status";
            public const string JobDescription = "Get Delivery Status Updates and update Notification Queues";
            public const string CronSchedule = "30 2/5 * * * ?"; // Every 5 minute on the minute, starting at minute 2,5
            public const int LockTimeout = 600; // Time in seconds before releasing lock
            public const int WaitTimeout = 20; // Time in seconds, until process will stop attempting to acquire lock
            public const int RetryTime = 5; // Time in seconds before retrying to acquire lock
        }

        public static class DataCleanup
        {
            public const string TriggerIdentity = "Data cleanup Trigger";
            public const string TriggerDescription = "Trigger for the data cleanup Job";
            public const string JobIdentity = "Data cleanup";
            public const string JobDescription = "Check for old data and try to delete it"; 
            public const string CronSchedule = "0 0 2 * * ?"; // Every day at 02:00
            public const int LockTimeout = 600; // Time in seconds before releasing lock
            public const int WaitTimeout = 20; // Time in seconds, until process will stop attempting to acquire lock
            public const int RetryTime = 5; // Time in seconds before retrying to acquire lock
        }
    }
}