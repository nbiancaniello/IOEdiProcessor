using System;
using System.IO;
using IOEdiProcessor.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace IOEdiProcessor.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            //services.AddMvc(o =>
            //{
            //    AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
            //        .RequireAuthenticatedUser()
            //        .Build();

            //    o.Filters.Add(new AuthorizeFilter(policy));
            //}).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddMvcCore().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddCors(o => o.AddPolicy("CORS", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfiguration configuration = builder.Build();

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = true,
            //            ValidIssuer = Configuration["Jwt:Issuer"],
            //            ValidAudience = Configuration["Jwt:Issuer"],
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
            //        };
            //    });

            // Add reference to admin database
            //services.AddDbContext<PrismContext>(option =>
            services.AddDbContext<IOEdiProcessorContext>(option =>
                option.UseSqlServer(configuration.GetConnectionString("TestConnection")));

            // Add reference to the client database
            //services.AddDbContext<IOEdiProcessorContext>((provider, options) =>
            //{
            //    // Get organization ID from the request headers
            //    string organization = provider.GetService<IHttpContextAccessor>()
            //        .HttpContext.Request.Headers["X-Organization"];
            //    // Get the connection string from the admin database
            //    string connectionString = provider.GetService<PrismContext>().Organizations
            //        .Where(o => o.ID == organization)
            //        .Select(o => o.ConnectionString)
            //        .Single();
            //    options.UseSqlServer(connectionString);
            //});

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Prism API", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                string basePath = AppContext.BaseDirectory;
                string xmlPath = Path.Combine(basePath, "IOEdiProcessor.Api.xml");
                //c.IncludeXmlComments(xmlPath);
            });

            services.AddMvcCore()
                    .AddApiExplorer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseCors("CORS");

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Prism API V1"));

            //app.UseAuthentication();

            app.UseMvc();
        }
    }
}
