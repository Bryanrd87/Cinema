using Application.Data;
using Domain.Auditorium;
using Domain.Movie;
using Domain.Showtime;
using Domain.Ticket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddDbContext<CinemaContext>(options =>
            {
                options.UseInMemoryDatabase("CinemaDb")
                    .EnableSensitiveDataLogging()
                    .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            services.AddScoped<ICinemaContext>(sp =>
                sp.GetRequiredService<CinemaContext>());

            services.AddScoped<IUnitOfWork>(sp =>
                sp.GetRequiredService<CinemaContext>());

            services.AddScoped<IShowtimesRepository, ShowtimesRepository>();

            services.AddScoped<IAuditoriumsRepository, AuditoriumsRepository>();

            services.AddScoped<ITicketsRepository, TicketsRepository>();

            services.AddScoped<IMovieRepository, MovieRepository>();

            return services;
        }
    }
}
