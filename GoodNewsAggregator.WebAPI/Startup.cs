using GoodNewsAggregator.DAL.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.DAL.Core.Entities;
using Microsoft.EntityFrameworkCore;
using GoodNewsAggregator.DAL.Repositories.Interfaces;
using GoodNewsAggregator.DAL.Repositories.Implementation.Repositories;
using GoodNewsAggregator.DAL.Repositories.Implementation;
using GoodNewsAggregator.Services.Implementation;
using GoodNewsAggregator.Core.Services.Interfaces;
using GoodNewsAggregator.Core.WebApi.Auth;
using GoodNewsAggregator.DAL.CQRS.QueryHandlers.RssSourceQH;
using GoodNewsAggregator.Services.Implementation.Mapping;
using GoodNewsAggregator.Services.Implementation.Parsers;
using GoodNewsAggregator.WebAPI.Auth;
using Hangfire;
using Hangfire.SqlServer;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GoodNewsAggregator.WebAPI
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
            services.Configure<Jwt>(Configuration.GetSection("JWT"));

            services.AddDbContext<GoodNewsAggregatorContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<User, Role>(options => {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<GoodNewsAggregatorContext>();

            services.AddTransient<IRepository<News>, NewsRepository>();
            services.AddTransient<IRepository<RssSource>, RssSourceRepository>();
            services.AddTransient<IRepository<User>, UserRepository>();
            services.AddTransient<IRepository<Comment>, CommentsRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<INewsService, NewsCqsService>();
            services.AddScoped<IRssSourceService, RssSourceCqsService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IUserService, UserCqsService>();

            services.AddTransient<IWebPageParser, OnlinerParser>();
            //services.AddTransient<IWebPageParser, TutByParser>();
            services.AddTransient<IWebPageParser, DtfParser>();
            services.AddTransient<IWebPageParser, S13Parser>();
            services.AddTransient<IWebPageParser, TJournalParser>();

            services.AddScoped<IJwtGenerator, JwtGenerator>();

            services.AddHangfire(conf => conf
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"),
                    new SqlServerStorageOptions()
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(30),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(30),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    }));
            services.AddHangfireServer();

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddMediatR(typeof(GetRssSourceByIdQueryHandler).GetTypeInfo().Assembly);
            
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.RequireHttpsMetadata = false;
                opt.SaveToken = true;

                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GoodNewsAggregator.WebAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GoodNewsAggregator.WebAPI v1"));
            }


            app.UseHangfireDashboard();
            var newsService = serviceProvider.GetService(typeof(INewsService)) as INewsService;

            RecurringJob.AddOrUpdate(
                () => newsService.Aggregate(),
                "1/10 * * * *");
            RecurringJob.AddOrUpdate(
                () => newsService.GetBodies(),
                "6/10 * * * *");
            RecurringJob.AddOrUpdate(
                () => newsService.Rate30News(),
                "1/2 * * * *");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });
        }
    }
}
