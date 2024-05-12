using Auth.api.Repository;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Utility;
using Utility.Encryption;
using Utility.Models;

namespace Auth.api
{
    public class Startup(IConfiguration configuration)
    {
        private IConfiguration Configuration { get; } = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMongoDb(Configuration);
            services.AddJwt(Configuration);
            services.AddTransient<IEncryptor, Encryptor>();
            services.AddSingleton<IUserRepository>(sp =>
              new UserRepository(sp.GetService<IMongoDatabase>() ?? throw new Exception("IMongoDatabase not found"))
            );
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "User", Version = "v1" });
            });
            services.AddHealthChecks()
                .AddMongoDb(
                    mongodbConnectionString: (
                        Configuration.GetSection("mongo").Get<MongoOptions>()
                        ?? throw new Exception("mongo configuration section not found")
                    ).ConnectionString,
                    name: "mongo",
                    failureStatus: HealthStatus.Unhealthy
                );
            services.AddHealthChecksUI().AddInMemoryStorage();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog V1");
            });

            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);

            app.UseHealthChecks("/healthz", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecksUI();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
