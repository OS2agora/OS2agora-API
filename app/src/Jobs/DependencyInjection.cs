using Jobs.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.IO;

namespace Jobs
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddJobs(this IServiceCollection services, IConfiguration configuration)
        {
            // copy database template
            string databasePath = Path.Combine("wwwroot", "api", "QuartzDatabase");

            if (!File.Exists(databasePath + ".db"))
            {
                File.Copy(databasePath + "_base.db", databasePath + ".db", false);
            }

            // See https://www.quartz-scheduler.net/documentation/quartz-3.x/packages/microsoft-di-integration.html#using
            // And https://github.com/quartznet/quartznet/tree/main/src/Quartz.Examples.AspNetCore
            // For reference
            services.AddQuartz(options =>
            {
                options.SchedulerId = "Hearing-Portal-Scheduler-Core";
                options.SchedulerName = "Hearing Portal Scheduler";

                // This will allow our DI to work.
                options.UseMicrosoftDependencyInjectionScopedJobFactory();

                options.ScheduleJob<HearingStatusChecker>(trigger => trigger
                        .WithIdentity(Common.Constants.Jobs.HearingStatusChecker.TriggerIdentity)
                        .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(7)))
                        .WithCronSchedule(Common.Constants.Jobs.HearingStatusChecker.CronSchedule)
                        .WithDescription(Common.Constants.Jobs.HearingStatusChecker.TriggerDescription),
                    job => job.WithIdentity(Common.Constants.Jobs.HearingStatusChecker.JobIdentity)
                        .WithDescription(Common.Constants.Jobs.HearingStatusChecker.JobDescription));

                options.ScheduleJob<HearingJournalizer>(trigger => trigger
                   .WithIdentity(Common.Constants.Jobs.HearingJournalizer.TriggerIdentity)
                   .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(7)))
                   .WithCronSchedule(Common.Constants.Jobs.HearingJournalizer.CronSchedule)
                   .WithDescription(Common.Constants.Jobs.HearingJournalizer.TriggerDescription),
                   job => job.WithIdentity(Common.Constants.Jobs.HearingJournalizer.JobIdentity)
                       .WithDescription(Common.Constants.Jobs.HearingJournalizer.JobDescription));

                options.ScheduleJob<CreateNotificationQueue>(trigger => trigger
                    .WithIdentity(Common.Constants.Jobs.CreateNotificationQueue.TriggerIdentity)
                    .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(7)))
                    .WithCronSchedule(Common.Constants.Jobs.CreateNotificationQueue.CronSchedule)
                    .WithDescription(Common.Constants.Jobs.CreateNotificationQueue.TriggerDescription),
                    job => job.WithIdentity(Common.Constants.Jobs.CreateNotificationQueue.JobIdentity)
                        .WithDescription(Common.Constants.Jobs.CreateNotificationQueue.JobDescription));

                options.ScheduleJob<CreateDailyStatusNotification>(trigger => trigger
                    .WithIdentity(Common.Constants.Jobs.CreateDailyStatusNotifications.TriggerIdentity)
                    .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(7)))
                    .WithCronSchedule(Common.Constants.Jobs.CreateDailyStatusNotifications.CronSchedule)
                    .WithDescription(Common.Constants.Jobs.CreateDailyStatusNotifications.TriggerDescription),
                    job => job.WithIdentity(Common.Constants.Jobs.CreateDailyStatusNotifications.JobIdentity)
                        .WithDescription(Common.Constants.Jobs.CreateDailyStatusNotifications.JobDescription));

                options.ScheduleJob<NotificationQueueSender>(trigger => trigger
                    .WithIdentity(Common.Constants.Jobs.NotificationQueueSender.TriggerIdentity)
                    .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(7)))
                    .WithCronSchedule(Common.Constants.Jobs.NotificationQueueSender.CronSchedule)
                    .WithDescription(Common.Constants.Jobs.NotificationQueueSender.TriggerDescription),
                    job => job.WithIdentity(Common.Constants.Jobs.NotificationQueueSender.JobIdentity)
                        .WithDescription(Common.Constants.Jobs.NotificationQueueSender.JobDescription));

                options.ScheduleJob<UpdateNotificationQueueDeliveryStatus>(trigger => trigger
                    .WithIdentity(Common.Constants.Jobs.UpdateNotificationQueueDeliveryStatus.TriggerIdentity)
                    .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow).AddSeconds(7)) 
                    .WithCronSchedule(Common.Constants.Jobs.UpdateNotificationQueueDeliveryStatus.CronSchedule)
                    .WithDescription(Common.Constants.Jobs.UpdateNotificationQueueDeliveryStatus.TriggerDescription),
                    job => job.WithIdentity(Common.Constants.Jobs.UpdateNotificationQueueDeliveryStatus.JobIdentity)
                        .WithDescription(Common.Constants.Jobs.UpdateNotificationQueueDeliveryStatus.JobDescription));

                options.ScheduleJob<DataCleanup>(trigger => trigger
                        .WithIdentity(Common.Constants.Jobs.DataCleanup.TriggerIdentity)
                        .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.UtcNow.AddSeconds(7)))
                        .WithCronSchedule(Common.Constants.Jobs.DataCleanup.CronSchedule)
                        .WithDescription(Common.Constants.Jobs.DataCleanup.TriggerDescription),
                        job => job.WithIdentity(Common.Constants.Jobs.DataCleanup.JobIdentity)
                            .WithDescription(Common.Constants.Jobs.DataCleanup.JobDescription));

                // convert time zones using converter that can handle Windows/Linux differences
                options.UseTimeZoneConverter();

                options.UsePersistentStore(storeOptions =>
                {
                    storeOptions.UseProperties = true;
                    storeOptions.RetryInterval = TimeSpan.FromSeconds(15);
                    storeOptions.UseSQLite(sqliteOptions =>
                    {
                        sqliteOptions.ConnectionString = "Data Source=wwwroot/api/QuartzDatabase.db;";
                        sqliteOptions.TablePrefix = "QRTZ_";
                    });
                    storeOptions.UseJsonSerializer();
                });
            });

            // ASP.NET Core hosting
            services.AddQuartzServer(options => { options.WaitForJobsToComplete = true; });

            return services;
        }
    }
}