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
        }

        public static class HearingJournalizer
        {
            public const string TriggerIdentity = "Hearing Journalizer Trigger";
            public const string TriggerDescription = "Trigger for the Hearing Journalizer Job";
            public const string JobIdentity = "Hearing Journalizer";
            public const string JobDescription = "Journalize hearings";
            public const string CronSchedule = "0 0 * * * ?"; // Every hour on the hour
        }

        public static class InstantNotifications
        {
            public const string TriggerIdentity = "Instant Notifications Trigger";
            public const string TriggerDescription = "Trigger for the Instant Notifications Job";
            public const string JobIdentity = "Instant Notifications";
            public const string JobDescription = "Check for Instant Notifications and create NotificationQueue elements";
            public const string CronSchedule = "0 0/5 * * * ?"; // Every 5 minute on the minute
        }

        public static class DailyNotifications
        {
            public const string TriggerIdentity = "Daily Notifications Trigger";
            public const string TriggerDescription = "Trigger for the Daily Notifications Job";
            public const string JobIdentity = "Daily Notifications";
            public const string JobDescription = "Check for Daily Notifications and create NotificationQueue elements";
            public const string CronSchedule = "0 0 7 * * ?"; // Every day at 07:00
        }

        public static class NotificationQueueSender
        {
            public const string TriggerIdentity = "NotificationQueue Sender Trigger";
            public const string TriggerDescription = "Trigger for the NotificationQueue Sender Job";
            public const string JobIdentity = "NotificationQueue Sender";
            public const string JobDescription = "Check for NotificationQueue elements and try to send them";
            public const string CronSchedule = "0 0/1 * * * ?"; // Every minute on the minute
        }
    }
}