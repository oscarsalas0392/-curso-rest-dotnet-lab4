using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Entities;
using ProductsApi.Services;

namespace ProductsApi
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

            //services.AddDbContext();

            //services.AddSingleton<AdventureWorksDbContext>();
            services.AddDbContext<AdventureworksContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString("AdventureWorks");
                
                Console.WriteLine(connectionString);
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<AdventureworksContext>();
            services.AddScoped<IProductService, ProductService>();

            /*
                - Raw db calls -> ADO.NET
                - ORM -> Object Relational Mapping 
                    -  Dapper
                    - Entity Framework
                        * Database First
                        * DBML (Model First) -> Generado de Models
                        * Code First
                            - Data Annotations
                            - Fluent APIS

                    ...

            */

            services.AddCors(options =>
            {
                options.AddPolicy("Policy",
                                builder =>
                                {
                                    builder.AllowAnyOrigin()
                                           .AllowAnyHeader()
                                           .AllowAnyMethod();
                                });
            });
            services.AddControllers();

            // dotnet add package Swashbuckle.AspNetCore
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors("Policy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGet("/", context => {

                    context.Response.Redirect("/swagger");
                    return Task.CompletedTask;
                });
            });
        }
    }
}
