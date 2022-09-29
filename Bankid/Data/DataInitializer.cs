
using Bankid.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bankid.Data {
    internal class DataInitializer : BackgroundService {
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;

        public DataInitializer(ILogger<DataInitializer> logger, IServiceProvider services) {
            _logger = logger;
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            try {
                _logger.LogInformation("Start db init");
                using (var scope = _services.CreateScope())
                using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                using (var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>())
                using (var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>()) {
                    await dbContext.Database.MigrateAsync();
                    foreach (string role in Role.Roles) {
                        if (!await roleManager.RoleExistsAsync(role)) {
                            await roleManager.CreateAsync(new Role() { Name = role });
                        }
                    }
                    try {
                        var user = new Models.Entities.User() { UserName = "admin", Role = Role.AdminRoleName };
                        var result = await userManager.CreateAsync(user, "admin");
                        if (result.Succeeded) {
                            await userManager.AddToRoleAsync(user, Role.AdminRoleName);
                        }
                    } catch (System.Exception) {
                    }
                }
                _logger.LogInformation("Finish db init");
            } catch (Exception e) {
                _logger.LogError(e, "Db init exception");
            }
        }
    }
}
