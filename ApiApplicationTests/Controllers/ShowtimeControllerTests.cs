using ApiApplication.Controllers;
using Application.Features.Showtime.Commands;
using Application.Features.Showtime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Xunit;
using Shouldly;
using Application.Features.Showtime.Queries.GetShowtimeDetails;
using Application.Features.Showtime.Queries.GetShowtimes;

namespace ApiApplication.UnitTests.Controllers
{
    public class ShowtimeControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ShowtimeController _controller;

        public ShowtimeControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ShowtimeController(_mediatorMock.Object);
        }

        [Fact]
        public async Task Get_ShouldReturnListOfShowtimeDTO()
        {
            // Arrange
            var showtimes = new List<ShowtimeDTO>();
            _mediatorMock.Setup(mock => mock.Send(It.IsAny<GetShowtimeListQuery>(), CancellationToken.None))
                .ReturnsAsync(showtimes);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
            var returnedShowtimes = okResult.Value.ShouldBeAssignableTo<List<ShowtimeDTO>>();
        }

        [Fact]
        public async Task Get_WithId_ShouldReturnShowtimeDetailsDTO()
        {
            // Arrange
            int showtimeId = 1;
            var showtimeDetails = new ShowtimeDetailsDTO();
            _mediatorMock.Setup(mock => mock.Send(It.IsAny<GetShowtimeDetailsQuery>(), CancellationToken.None))
                .ReturnsAsync(showtimeDetails);

            // Act
            var result = await _controller.Get(showtimeId);

            // Assert
            var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
            var returnedShowtime = okResult.Value.ShouldBeAssignableTo<ShowtimeDetailsDTO>();
        }

        [Fact]
        public async Task Post_WithValidData_ShouldReturnCreatedResponse()
        {
            // Arrange
            var createShowtimeCommand = new CreateShowtimeCommand(); 
            var createdShowtimeId = 1; 
            _mediatorMock.Setup(mock => mock.Send(It.IsAny<CreateShowtimeCommand>(), CancellationToken.None))
                .ReturnsAsync(createdShowtimeId);

            // Act
            var result = await _controller.Post(createShowtimeCommand, CancellationToken.None);

            // Assert
            var createdAtActionResult = result.ShouldBeOfType<CreatedAtActionResult>();
            createdAtActionResult.ActionName.ShouldBe(nameof(ShowtimeController.Get));
        }
    }
}
