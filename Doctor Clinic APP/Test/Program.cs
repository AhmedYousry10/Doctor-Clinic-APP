using Doctor_CLinic_API.Data;
using Doctor_CLinic_API.IServices;
using Doctor_CLinic_API.MappingProfile;
using Doctor_CLinic_API.Models;
using Doctor_CLinic_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure DbContext
        builder.Services.AddDbContext<appContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Configure Identity with custom User and Role types
        builder.Services.AddIdentity<User , Role>()
            .AddEntityFrameworkStores<appContext>()
            .AddDefaultTokenProviders();

        // Configure AutoMapper
        builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

        // Register Services
        builder.Services.AddScoped<IPatientService, PatientService>();
        builder.Services.AddScoped<IAppointmentService, AppointmentService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<JwtTokenService>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddScoped<IUserService, UserService>();

        // Configure JWT
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        // Add Controllers
        builder.Services.AddControllers();

        // Add CORS policy
        builder.Services.AddCors(Services =>
        {
            Services.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
        });

        // Configure Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter into field the word 'Bearer' followed by a space and the JWT value",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            options.OperationFilter<SecurityRequirementsOperationFilter>();
            options.EnableAnnotations();
        });

        var app = builder.Build();

        // Ensure default roles exist
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var roleService = services.GetRequiredService<IRoleService>();
            await roleService.EnsureDefaultRolesAsync();

            var authService = services.GetRequiredService<IAuthService>();
            await authService.EnsureDefaultAdminAsync();
        }

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Doctor Clinic API v1");
            });
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseCors("CorsPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
