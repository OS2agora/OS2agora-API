using AutoMapper;
using Agora.Api;
using Agora.DAOs.Identity;
using Agora.DAOs.Persistence;
using Agora.Operations.Common.Enums;
using Agora.Operations.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InvalidOperationException = Agora.Operations.Common.Exceptions.InvalidOperationException;


namespace Agora.Operations.IntegrationTests
{
    [SetUpFixture]
    public class TestSetup
    {
        private static IConfigurationRoot _configuration;
        private static SqliteConnection _sqLiteConnection;
        public static IServiceScopeFactory ScopeFactory;
        private static string _currentUserId;
        private static AuthenticationMethod? _currentUserAuthenticationMethod;

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();

            var startup = new Startup(_configuration);

            var services = new ServiceCollection();

            _sqLiteConnection = new SqliteConnection("DataSource=:memory:");
            _sqLiteConnection.Open();
            services.AddDbContext<ApplicationDbContext>(o =>
            {
                o.UseSqlite(_sqLiteConnection,
                    op => op.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            });

            services.AddSingleton(Mock.Of<IWebHostEnvironment>(w =>
                w.EnvironmentName == "Development" &&
                w.ApplicationName == "Agora.Api"));

            startup.ConfigureServices(services);

            // Replace service registration for ICurrentUserService
            var currentUserServiceDescriptor = services.FirstOrDefault(d =>
                d.ServiceType == typeof(ICurrentUserService));

            services.Remove(currentUserServiceDescriptor);

            // Register testing version
            services.AddTransient(provider =>
                Mock.Of<ICurrentUserService>(s =>
                                                    s.UserId == _currentUserId &&
                                                    s.AuthenticationMethod == _currentUserAuthenticationMethod));

            ScopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();

            EnsureDatabase();
        }

        private static void EnsureDatabase()
        {
            using var scope = ScopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

            context.Database.EnsureCreated();

            SeedDefaultDatabase().GetAwaiter().GetResult();
        }

        public static async Task SeedDefaultDatabase()
        {
            using var scope = ScopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            var kleService = scope.ServiceProvider.GetService<IKleService>();
            var mapper = scope.ServiceProvider.GetService<IMapper>();
            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

            await ApplicationDbContextSeed.SeedDefaultRolesAsync(roleManager);
            await ApplicationDbContextSeed.SeedDefaultUsers(userManager);
            await ApplicationDbContextSeed.SeedSampleDataAsync(context, userManager, kleService, mapper);
        }

        public static async Task RunAsUserWithRole(string role)
        {
            if (role != "Administrator" && role != "HearingCreator")
            {
                throw new InvalidOperationException("Invalid role");
            }

            var newId = await RunAsUserAsync("test", "Test@3ma1l");
            await AddRoleToUserAsync(newId, role);
        }

        public static async Task RunAsUserWithAuthenticationMethod(AuthenticationMethod? authenticationMethod)
        {
            _currentUserAuthenticationMethod = authenticationMethod;
        }

        public static async Task<string> RunAsUserAsync(string userName, string password)
        {
            using var scope = ScopeFactory.CreateScope();

            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

            var user = new ApplicationUser { UserName = userName, Email = userName };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                _currentUserId = user.Id;

                return _currentUserId;
            }
            throw new Exception($"Unable to create {userName}.");
        }

        public static async Task AddRoleToUserAsync(string id, string role)
        {
            using var scope = ScopeFactory.CreateScope();

            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(id);

            user = await userManager.FindByNameAsync(user.UserName);

            await userManager.AddToRoleAsync(user, role);
        }

        public static async Task ResetState()
        {
            _currentUserId = null;
            _currentUserAuthenticationMethod = AuthenticationMethod.NONE;

            await _sqLiteConnection.CloseAsync();
            await _sqLiteConnection.OpenAsync();
            EnsureDatabase();
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            _sqLiteConnection.Close();
        }
    }
}