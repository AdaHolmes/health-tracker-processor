using HealthTrackerProcessor.Class;
using HealthTrackerProcessor.Controllers;
using HealthTrackerProcessor.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace HealthTrackerProcessor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            using (var clinet = new ConfigContext(new DbContextOptions<ConfigContext>()))
            {
                clinet.Database.EnsureCreated();
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(x =>
                {
                    x.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                });

            services.AddTransient<Project>();
            services.AddTransient<User>();
            services.AddTransient<ProjectController>();
            services.AddTransient<UserController>();
            services.AddDbContext<ConfigContext>(x => x.UseMySql("Server=127.0.0.1;port=3306;Database=configdb;uid=root;pwd=yang12"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //var DB = app.ApplicationServices.GetRequiredService<ConfigContext>();
            //DB.Database.EnsureCreated();

            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "My Web App API V1");
            }
                );
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseEndpoints(endpoints => 
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("OK!"); 
                }); 
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
