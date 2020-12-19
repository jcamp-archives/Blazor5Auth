using Microsoft.Extensions.DependencyInjection;
using MongoFramework.AspNetCore.Identity;
using System;
using System.Linq;

namespace Blazor5Auth.Server.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var scopeServices = serviceProvider.CreateScope().ServiceProvider;
            var context = scopeServices.GetRequiredService<ApplicationDbContext>();
            SetupRoles(context);
        }

        private static void SetupRoles(ApplicationDbContext context)
        {
            if (context.Roles.Any()) return;

            var defaultRoles = new MongoIdentityRole[] {
                new MongoIdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new MongoIdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },

            };

            context.Roles.AddRange(defaultRoles);
            context.SaveChanges();
        }

    }
}
