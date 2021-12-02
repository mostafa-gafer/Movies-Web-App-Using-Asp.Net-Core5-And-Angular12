using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Helper.Helpers;
using Movies.CORE.Context;
using Microsoft.EntityFrameworkCore;
using Movies.EF.Interfaces;
using Movies.EF;
using Movies.CORE.Filters;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using AutoMapper;
using Movies.API.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Movies.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpContextAccessor();
            //to configure connectionstring 
            //services.AddDbContext<ApplicationDbContext>(options =>
            //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
            //b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.UseNetTopologySuite()));  // use point and location

            // to register ibaserepository and define which make implementation to it
            //services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            // to register IUnitof work and define which make implementation to it
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //automapper
            services.AddAutoMapper(typeof(Startup));
            //define mapper to geometry
            services.AddSingleton(provider => new MapperConfiguration(config =>
            {
                var geometryFactory = provider.GetRequiredService<GeometryFactory>();
                config.AddProfile(new AutoMapperProfile(geometryFactory));
            }).CreateMapper());
            //for define geometry
            services.AddSingleton<GeometryFactory>(NtsGeometryServices
                .Instance.CreateGeometryFactory(srid: 4326));

            ////to save file to Azure Storage
            //services.AddScoped<IFileStorageService, AzureStorageService>();

            //to save file locally define instance
            services.AddScoped<IFileStorageService, InAppStorageService>();
            services.AddHttpContextAccessor();

            services.AddControllersWithViews();
           
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddTransient<MyActionFilter>();    //to use custom filter
            //to add cros policy that specific domain can call api
            services.AddCors(options =>
            {   options.AddDefaultPolicy(builder =>
                {
                    //save doman name in appsetting.development.json to avoid hard coding
                    var frontendUrl = Configuration.GetValue<string>("frontend_url");
                    //var x = Configuration.GetSection("frontend_url").Value;
                    builder.WithOrigins(frontendUrl).AllowAnyMethod().AllowAnyHeader()
                           .WithExposedHeaders(new string[] { "totalAmountOfRecords" });
                });
            });

            //to configre Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            //to configre JWT 
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("keyjwt"))),
                        ClockSkew = TimeSpan.Zero
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", policy => policy.RequireClaim("role", "admin"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            //to configre authentication and authorize
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors();   // to put in pipeline

            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
