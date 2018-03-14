// Copyright (c) MadDonkeySoftware

namespace Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Api.Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;

    /// <summary>
    /// The class containing the startup configuration for the web api.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration used by this <see cref="Startup"/> instance.
        /// </summary>
        /// <returns>The <see cref="IConfiguration"/> for this instance.</returns>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the services for this web api.
        /// </summary>
        /// <param name="services">A references to the <see cref="IServiceCollection"/> for this web api.</param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </remarks>
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(opts =>
            {
                opts.AddPolicy("AllowAllOrigins", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            });
            services.AddMvc();

            // Initialize application services to pre-warm the application
            var connString = new MongoUrl(Environment.GetEnvironmentVariable("MONGO_CONNECTION"));
            DbFactory.SetClient(new MongoClient(connString));

            // Add application services
            services.AddSingleton<IDbFactory, DbFactory>();
        }

        /// <summary>
        /// Configures the HTTP request pipeline for this web api.
        /// </summary>
        /// <param name="app">The web api application.</param>
        /// <param name="env">The web api environment.</param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </remarks>
        public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAllOrigins");
            app.UseMvc();
        }
    }
}