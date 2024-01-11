using Domain.ProductAgg;
using Domain.UserAgg;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Domain.Login;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Domain.RoleAgg;
using Domain.Permission;
using Domain;
using Domain.UserAppAgg;

namespace Infrastructure.Persistent.Ef
{
    public class MyDBContext : IdentityDbContext<UserApp,Role,string>
    {


        public MyDBContext(DbContextOptions<MyDBContext> options) : base(options)
        {

        }

        public DbSet<Product> product { get; set; }
        public DbSet<User> user { get; set; }
        public DbSet<Permission> permissions { get; set; }
     
    }
    #region comment
    //public class IDesignTimeFactory : IDesignTimeDbContextFactory<MyDBContex>
    //{
    //    public MyDBContex CreateDbContext(string[] args)
    //    {
    //        var optionBuilder = new DbContextOptionsBuilder<MyDBContex>();
    //        optionBuilder.UseSqlServer(@"Server=DESKTOP-MFT5R3I;Database=CleanProject11;integrated security=true; TrustServerCertificate = True;Trusted_Connection=True;MultipleActiveResultSets=true");
    //        return new MyDBContex(optionBuilder.Options);
    //    }


    //    //    //public MyDBContex CreateDbContext(string[] args)
    //    //    //{
    //    //    //    IConfigurationRoot configuration = new ConfigurationBuilder()
    //    //    //        .SetBasePath(Directory.GetCurrentDirectory())
    //    //    //        .AddJsonFile("appsetting.json")
    //    //    //        .Build();
    //    //    //    var builder = new DbContextOptionsBuilder<MyDBContex>();
    //    //    //    var connectionString = configuration.GetConnectionString("DefaultConnection");
    //    //    //    builder.UseSqlServer(connectionString);
    //    //    //    return new MyDBContex(builder.Options);
    //    //    //}
    //}
    #endregion
}
