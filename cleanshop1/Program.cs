using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistent.Ef;
using Microsoft.Identity.Web;
using Domain.ProductAgg.Repository;
using Domain.UserAgg.Repository;
using Infrastructure.Repositories;
using Infrastucture.Tools;
using Domain;
using Domain.RoleAgg.Repository;
using Domain.Permission.Repository;
using System;
using Domain.UserAppAgg;
using Domain.RoleAgg;
using System.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Identity.Web.UI;
using Domain.Utility;
using Org.BouncyCastle.Ocsp;
using StackExchange.Redis;
using Infrastructure.Tools;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();



        ///////// identity ////////

        builder.Services.AddDbContext<MyDBContext>(optionsAction: Options =>
        Options.UseSqlServer(builder.Configuration.GetConnectionString(name: "DefaultConnection")));

   

        builder.Services.AddScoped<IUserRepository, UserRepositories>();
        builder.Services.AddScoped<IRoleRepository, RoleRepositories>();
        builder.Services.AddScoped<IPermissionRepository, Infrastructure.Repositories.PermissionRepositories>();
        builder.Services.AddScoped<IProductRepository, ProductRepositories>();
        builder.Services.AddTransient<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect("localhost:9191"));

        builder.Services.AddRazorPages();



        //////   identity  /////////



        //////////////////////////////////////jwt////////////////////////////////////
        
        var JwtSettingSection = builder.Configuration.GetSection("JWTSettings");
        builder.Services.Configure<JWTSettings>(JwtSettingSection);
        var JwtSetting = JwtSettingSection.Get<JWTSettings>();


        var key = Encoding.ASCII.GetBytes(JwtSetting.Secret);

        //builder.Services.AddIdentity<UserApp,Role>()
        //    .AddEntityFrameworkStores<MyDBContext>()
        //    .AddDefaultTokenProviders();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x=>
        {
            x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidIssuer = JwtSetting.Issuer,
                ValidateIssuer = true,
                ValidAudience = JwtSetting.Audience,
                ValidateAudience = true,
                ValidateLifetime = true,
            };
        });

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost:9191,password=123456";
            options.InstanceName = "";
        });



        builder.Services.AddIdentity<UserApp, Role>(options =>
        {
            // User Options
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            // Signin Options
            options.SignIn.RequireConfirmedEmail = false; //تایید ایمیل
            options.SignIn.RequireConfirmedPhoneNumber = false;//تایید شماره موبایل
            // Password Options
            options.Password.RequireUppercase = false;
            options.Password.RequiredUniqueChars = 0;
            options.Password.RequireLowercase = false;
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 2;
            // LockOut
            options.Lockout.AllowedForNewUsers = false;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
            // options.Lockout.MaxF ailedAccessAttempts = 3;
            // Stores Options
            //options.Stores.MaxLengthForKeys = 10;
            options.Stores.ProtectPersonalData = false;

            options.Tokens.AuthenticatorTokenProvider = "";

            options.ClaimsIdentity.UserNameClaimType = "ClaimTypes.Name";
        }).AddEntityFrameworkStores<MyDBContext>()
          .AddRoleManager<RoleManager<Role>>()
          .AddDefaultTokenProviders(); 


        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/LogOut";
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(3);
        });


        builder.Services.AddMvc();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
        });






        // ////////////////        identity ui       ////////////////

        //Add services to the container.
        //builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        //    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

        // builder.Services.AddControllersWithViews(options =>
        // {
        //     var policy = new AuthorizationPolicyBuilder()
        //         .RequireAuthenticatedUser()
        //         .Build();
        //     options.Filters.Add(new AuthorizeFilter(policy));
        // });
        // builder.Services.AddRazorPages()
        //     .AddMicrosoftIdentityUI();


        // ////////////////////////        identity ui       ///////////////////////////



        var app = builder.Build();

        //Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        app.UseHttpsRedirection();
        app.UseSession();
        
        //app.UseMvc();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.UseCookiePolicy();

        app.Run();//
    }
}