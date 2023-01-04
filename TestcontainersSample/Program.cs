using Microsoft.EntityFrameworkCore;
using TestcontainersSample.DatabaseContext;

namespace TestcontainersSample;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration["MySqlConnectionString"];

        // Replace 'YourDbContext' with the name of your own DbContext derived class.
        builder.Services.AddDbContext<TodoContext>(
            dbContextOptions =>
            {
                dbContextOptions
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

                if (builder.Environment.IsDevelopment())
                {
                    dbContextOptions
                        .LogTo(Console.WriteLine, LogLevel.Information)
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors();
                }
            }
        );

        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
