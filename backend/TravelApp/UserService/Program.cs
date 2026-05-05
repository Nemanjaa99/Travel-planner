using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ServiceFabric.Services.Runtime;
using UserService.Data;
using UserService.Helpers;
using UserService;
internal static class Program
{
    private static void Main()
    {
        ServiceRuntime.RegisterServiceAsync("UserServiceType", context =>
        {
            var builder = WebApplication.CreateBuilder();

            // DB
            builder.Services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // JWT
            var jwtSecret = builder.Configuration["Jwt:Secret"];
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
                    };
                });

            builder.Services.AddAuthorization();
            builder.Services.AddScoped<JwtHelper>();
            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
                options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

            var app = builder.Build();

            // Auto-migrate
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                db.Database.Migrate();
            }

            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run($"http://+:{context.CodePackageActivationContext.GetEndpoint("ServiceEndpoint").Port}");

            return new UserServiceFabric(context, app);
        }).GetAwaiter().GetResult();

        Thread.Sleep(Timeout.Infinite);
    }
}