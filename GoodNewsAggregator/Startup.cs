using GoodNewsAggregator.DAL.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.Services.Interfaces;
using GoodNewsAggregator.DAL.Core.Entities;
using GoodNewsAggregator.DAL.Repositories.Implementation;
using GoodNewsAggregator.DAL.Repositories.Implementation.Repositories;
using GoodNewsAggregator.DAL.Repositories.Interfaces;
using GoodNewsAggregator.Services.Implementation;
using AutoMapper;
using GoodNewsAggregator.Mapping;
using GoodNewsAggregator.Services.Implementation.Mapping;
using GoodNewsAggregator.Services.Implementation.Parsers;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace GoodNewsAggregator
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

            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<IRssSourceService, RssSourceService>();
            services.AddScoped<ICommentService, CommentService>();

            services.AddTransient<IWebPageParser, OnlinerParser>();
            services.AddTransient<IWebPageParser, TutByParser>();
            services.AddTransient<IWebPageParser, DtfParser>();
            services.AddTransient<IWebPageParser, S13Parser>();
            services.AddTransient<IWebPageParser, TJournalParser>();

            services.ConfigureApplicationCookie(config =>
            {
                config.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToLogin = ctx =>
                    {
                        ctx.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                        return Task.FromResult(0);
                    }
                };
            });
                
            //services.AddTransient<ParserResolver>(ServiceProvider => name =>
            //{
            //    switch (name)
            //    {
            //        case "Onliner":
            //            return ServiceProvider.GetService<OnlinerParser>();
            //        case "Tut.by":
            //            return ServiceProvider.GetService<TutByParser>();
            //        default:
            //            return null;
            //    }
            //});

            //services.AddAutoMapper(typeof(Startup));

            //var mapperConfig = new MapperConfiguration(mc =>
            //{
            //    mc.AddProfile(new MappingProfile());
            //});
            //var mapper = mapperConfig.CreateMapper();
            //services.AddSingleton(mapper);
            services.AddAutoMapper(typeof(MappingProfile), typeof(MappingProfile2));

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=News}/{action=Index}/{id?}");
                //endpoints.MapRazorPages();
            });
        }
    }
}
