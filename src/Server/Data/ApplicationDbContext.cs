using System;
using Blazor5Auth.Server.Models;
using MongoFramework;
using MongoFramework.AspNetCore.Identity;

namespace Blazor5Auth.Server.Data
{
    public class ApplicationDbContext : MongoIdentityDbContext<ApplicationUser, MongoIdentityRole>
    {
        public ApplicationDbContext(IMongoDbConnection connection) : base(connection) { }
    }

}
