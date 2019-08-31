﻿using System.Text;
using Application.Activities;
using Application.Interfaces;
using Domain;
using Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddDbContext<DataContext> (options => {
                options.UseSqlite (Configuration.GetConnectionString ("DefaultConnection"));
            });

            services.AddCors (opt => {
                opt.AddPolicy ("CorsPolicy", policy => {
                    policy
                        .AllowAnyHeader ()
                        .AllowAnyMethod ()
                        .WithOrigins ("http://localhost:3000");
                });
            });

            services.AddMediatR (typeof (List.Handler).Assembly);

            services.AddMvc (opt => {
                    var policy = new AuthorizationPolicyBuilder ().RequireAuthenticatedUser ().Build ();
                    opt.Filters.Add (new AuthorizeFilter (policy));
                })
                .SetCompatibilityVersion (CompatibilityVersion.Version_2_2);

            var builder = services.AddIdentityCore<AppUser> ();
            var identityBuilder = new IdentityBuilder (builder.UserType, builder.Services);
            identityBuilder.AddEntityFrameworkStores<DataContext> ();
            identityBuilder.AddSignInManager<SignInManager<AppUser>> ();

            var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (Configuration["TokenKey"]));

            services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer (opt => {
                    opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateAudience = false,
                    ValidateIssuer = false
                    };
                });

            services.AddScoped<IJwtGenerator, JwtGenerator> ();
            services.AddScoped<IUserAccessor, UserAccessor> ();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            app.UseAuthentication ();
            app.UseCors ("CorsPolicy");

            // app.UseHttpsRedirection();
            app.UseMvc ();
        }
    }
}