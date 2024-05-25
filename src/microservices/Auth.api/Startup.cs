using Auth.api.Repository;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Utility;
using Utility.Encryption;
using Utility.JWT;
using Utility.Models;
using Utility.RoleManage;

namespace Auth.api
{
    public class Startup(IConfiguration configuration)
    {
        private IConfiguration Configuration { get; } = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddJwtAuthentication(Configuration); // JWT Configuration
            services.AddMongoDb(Configuration);
            services.AddTransient<IEncryptor, Encryptor>();
            services.AddSingleton<IUserRepository>(sp =>
              new UserRepository(sp.GetService<IMongoDatabase>() ?? throw new Exception("IMongoDatabase not found"))
            );

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT token with the prefix Bearer then space into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
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

            services.AddScoped<IAuthorizationHandler, RoleRequirementHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.AddRequirements(new RoleRequirement("Admin"), new RoleRequirement("Accounting")));
                options.AddPolicy("Accounting", policy => policy.AddRequirements(new RoleRequirement("Accounting")));
                options.AddPolicy("Product", policy => policy.AddRequirements(new RoleRequirement("Product")));
                options.AddPolicy("User", policy => policy.AddRequirements(new RoleRequirement("User")));
                options.AddPolicy("SuperAdmin", policy => policy.AddRequirements(new RoleRequirement("SuperAdmin")));
            });
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
            app.UseMiddleware<JwtMiddleware>(); // JWT Middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
            });
        }
    }
}
