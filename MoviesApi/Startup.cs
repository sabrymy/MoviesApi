using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoviesApi.Filters;
using MoviesApi.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddControllers(options=> options.Filters.Add(typeof(MyExceptionFilter) ));
            //add MyActionFilte as global filter
            //support xml format
            
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))) ;
            services.AddTransient<IHostedService, MovieInTheatersService>();

            services.AddControllers(options => options.Filters.Add(typeof(MyExceptionFilter))).AddNewtonsoftJson().
                AddXmlDataContractSerializerFormatters();

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("V1", new OpenApiInfo{Version ="V1",Title="MoviesApi" });
                config.ResolveConflictingActions(opt => opt.First());
            });


            services.AddDataProtection();
            services.AddCors(options =>  options.AddPolicy("AllowPIRequestIO", builder => builder.WithOrigins("https://apirequest.io").WithMethods("GET", "POST")) );
            //adding automapper to  return view from entity
            services.AddAutoMapper(typeof(Startup));
            services.AddTransient<HashService>();

            //save to azure
            services.AddTransient<IFileStorageService, AzureStorageService>();
            //save to folder on wwwroot
            // services.AddTransient<IFileStorageService, InAppStorageService>();
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = false,
                       ValidateAudience = false,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(
                   Encoding.UTF8.GetBytes(Configuration["jwt:key"])),
                       ClockSkew = TimeSpan.Zero
                   }
               );


            services.AddHttpContextAccessor();
          //  services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
            //api service
            //services.AddSingleton<Irepository, InMetmoryRepository>();
            

            //custom filter service
            // services.AddTransient(typeof(MyActionFilter));

            services.AddTransient<IHostedService, WriteToFileHostedService>();
         //AddScoped is used Actually in most wep applications
        //    services.AddScoped<Irepository, InMemoryRepository>();
        }

       

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) 
        {



            //  config.RoutePrefix = string.Empty;


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(c => { c.RouteTemplate = "/swagger/{documentName}/swagger.json"; });
                app.UseSwaggerUI(config =>
                {
                    config.SwaggerEndpoint("/swagger/v1/swagger.json", "MoviesApi");


                });

            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

           
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors();
            //allow the specified site to call our api using middle ware usecors
          //  app.UseCors(builder => builder.WithOrigins("https://apirequest.io").WithMethods("GET","POST").AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
