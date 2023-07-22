using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServicextensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services ,IConfiguration config)
        {
             services.AddDbContext<DataContext>(opt =>
             {
                 opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
             });
            services.AddControllers(); 
            services.AddScoped<ITokenServices , TokenServices>();

            return services;
        }
    }
}