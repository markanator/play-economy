using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Play.Catalog.Service.Entities;
using Play.Common.Identity;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Common.Settings;

namespace Play.Catalog.Service
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private ServiceSettings serviceSettings { get; set; }
        private const string AllowedOriginSetting = "AllowedOrigin";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            serviceSettings = Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

            services.AddMongo() // Mongo setup
                    .AddMongoRepository<Item>("items") // Mongo collection
                    .AddMassTransitWithRabbitMQ() // MassTransit with RabbitMQ setup
                    .AddJwtBearerAuthentication(); // JWT Authentication

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.Read, policyBuilder =>
                {
                    policyBuilder.RequireRole("Admin");
                    policyBuilder.RequireClaim("scope", "Catalog.readAccess", "Catalog.fullAccess");
                });
                options.AddPolicy(Policies.Write, policyBuilder =>
                {
                    policyBuilder.RequireRole("Admin");
                    policyBuilder.RequireClaim("scope", "Catalog.writeAccess", "Catalog.fullAccess");
                });
            });

            services.AddControllers(options =>
            {
                // fix removal of Async from function names
                options.SuppressAsyncSuffixInActionNames = false;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Play.Catalog.Service", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Catalog.Service v1"));

                app.UseCors(builder =>
                {
                    builder.WithOrigins(Configuration[AllowedOriginSetting])
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
