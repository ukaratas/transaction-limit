using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Serilog;

namespace bbt.enterprise_library.transaction_limit
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDataHelperService, DataHelperService>();
            services.AddScoped<IDataService, DataService>();
            services.AddScoped<IBusinessService, BusinessService>();
            services.AddScoped<IBusinessHelperService, BusinessHelperService>();

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddControllers();
            services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            services.AddSwaggerGen(c =>
             {
                 c.SwaggerDoc("v1", new OpenApiInfo
                 {
                     Title = "Transaction Limit API",
                     Version = "v1",
                     Description = "Transaction Limit and availablity managemement API set.",

                     TermsOfService = new Uri("https://hub.burgan.com.tr/api/usage-terms"),

                     Contact = new OpenApiContact
                     {
                         Name = "Hub Portal of Burgan Bank Turkey",
                         Email = "info@hub.burgan.com.tr",
                         Url = new Uri("https://hub.burgan.com.tr"),
                     },

                     License = new OpenApiLicense
                     {
                         Name = "Use under XXX",
                         Url = new Uri("https://hub.burgan.com.tr/api/license"),
                     }

                 });

                 c.MapType<decimal>(() => new OpenApiSchema() { Type = "number", Format = "decimal" });
                 c.MapType<decimal?>(() => new OpenApiSchema() { Type = "number", Format = "decimal", Nullable = true});

                 var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                 var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                 c.IncludeXmlComments(xmlPath);
             });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transaction Limit API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
