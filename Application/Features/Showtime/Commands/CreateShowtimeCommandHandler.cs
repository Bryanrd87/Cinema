using Application.Exceptions;
using Application.Services;
using Domain.Auditorium;
using Domain.Movie;
using Domain.Showtime;
using MediatR;
using ProtoDefinitions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Showtime.Commands
{
    public class CreateShowtimeCommandHandler : IRequestHandler<CreateShowtimeCommand, int>
    {        
        private readonly IShowtimesRepository _showtimesRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly ApiClientGrpc _grpcClient;
        private readonly IAuditoriumsRepository _auditoriumsRepository;
        public CreateShowtimeCommandHandler(IShowtimesRepository showtimesRepository, IMovieRepository movieRepository, ApiClientGrpc grpcClient, IAuditoriumsRepository auditoriumsRepository)
        {           
            _showtimesRepository = showtimesRepository;
            _movieRepository = movieRepository;
            _grpcClient = grpcClient;
            _auditoriumsRepository = auditoriumsRepository;
        }
        public async Task<int> Handle(CreateShowtimeCommand request, CancellationToken cancellationToken)
        {
            var auditorium = await _auditoriumsRepository.GetAsync(request.AuditoriumId, cancellationToken) 
                ?? throw new InvalidAuditoriumException(nameof(AuditoriumEntity),request.AuditoriumId);

            showResponse showResponse = await _grpcClient.GetById(request.MovieId, cancellationToken)
                ?? throw new NotFoundException(nameof(showResponse), request.MovieId); 
           
            var movieEntityToCreate = new MovieEntity { 
                ImdbId = showResponse.Id,
                Stars = showResponse.Crew,
                ReleaseDate = GenerateRandomDate(showResponse.Year),
                Title = showResponse.Title
            };
           
            var movie = await _movieRepository.CreateMovie(movieEntityToCreate, cancellationToken);

            var showtimeEntityToCreate = new ShowtimeEntity { Movie = movie, SessionDate = request.SessionDate, AuditoriumId = request.AuditoriumId};
           
            await _showtimesRepository.CreateShowtime(showtimeEntityToCreate, cancellationToken);
           
            return showtimeEntityToCreate.Id;
        }

        public DateTime GenerateRandomDate(string year)
        {
            Random random = new Random();
            int dayOfYear = random.Next(1, 366);
            
            DateTime randomDate = new DateTime(Convert.ToInt32(year), 1, 1).AddDays(dayOfYear - 1);

            return randomDate;
        }
    }
}
