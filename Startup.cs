using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KalumManagement.Helpers;
using KalumManagement.Models;
using KalumManagement.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using KalumManagement.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace KalumManagement
{
    public class Startup
    {
        private readonly string OriginKalum = "OriginKalum";
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => {
                options.AddPolicy(name: OriginKalum, builder => {
                    builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200");
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey
                    (
                        Encoding.UTF8.GetBytes(this.Configuration["JWT:key"])
                    ),
                    ClockSkew = TimeSpan.Zero
                });

            services.AddTransient<IQueueAspiranteService, QueueAspiranteService>();
            services.AddTransient<IQueueAlumnoService, QueueAlumnoService>();
            services.AddControllers();
            services.AddDbContext<KalumDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));
            services.AddEndpointsApiExplorer();
            services.AddResponseCaching();
            services.AddControllers(options => options.Filters.Add(typeof(ErrorFilterException)));
            services.AddControllers(options => options.Filters.Add(typeof(ErrorFilterResponseException)));
            services.AddSwaggerGen();

            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore);

            //Se agrego esta linea para ignorar el mensaje de serialización cuando traemos muchos elementos
            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            //Servicio para mapear automaticmante los campos en el controlador
            services.AddAutoMapper(typeof(Startup));
            
            //Agregando sección de Logs
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo
            .File("./Logs/KalumManagement.out", Serilog.Events.LogEventLevel.Debug, "{Message:lj}{NewLine}", encoding: Encoding.UTF8)
            .CreateLogger();

            AppLog logApp = new AppLog();
            logApp.ResponseTime = Convert.ToInt16(DateTime.Now.ToString("fff"));
            Utilerias.ImprimirLog(logApp, 0, "Iniciando servicio kalumnManagement", "Debug");

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwagger();
            }
            app.UseCors(this.OriginKalum);
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseResponseCaching();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}