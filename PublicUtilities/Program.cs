using Microsoft.EntityFrameworkCore;
using PublicUtilities.Data;
using PublicUtilities.Extention;
using PublicUtilities.Middlewares;

namespace PublicUtilities
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAppServices(); // Add Project-Services

            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
            });

            var app = builder.Build();

            app.UseMiddleware<MiddleWare>();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();

            app.Run();
        }
    }
}
