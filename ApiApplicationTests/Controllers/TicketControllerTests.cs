using System;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Controllers;
using Application.Features.Ticket;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Xunit;
using Application.Features.Ticket.Commands.Buy;
using Application.Features.Ticket.Commands.Reserve;
using Application.Features.Ticket.Queries.GetTicketDetails;
using Application.Features.Ticket.Queries.GetTickets;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ApiApplication.UnitTests.Controllers
{
    public class TicketControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly TicketController _controller;

        public TicketControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new TicketController(_mediatorMock.Object);
        }

        [Fact]
        public async Task Get_WithValidData_ShouldReturnOkResponse()
        {
            // Arrange
            var tickets = new List<ReservedTicketDTO>();
            _mediatorMock.Setup(mock => mock.Send(It.IsAny<GetTicketsListQuery>(), default))
                .ReturnsAsync(tickets);

            // Act
            var result = await _controller.Get(CancellationToken.None);

            // Assert
            var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
            okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);
            okResult.Value.ShouldBe(tickets);
        }

        [Fact]
        public async Task Get_WithId_ShouldReturnReservedTicketDTO()
        {
            // Arrange
            Guid ticketId = Guid.NewGuid(); 
            var reservedTicket = new ReservedTicketDTO(); 
            _mediatorMock.Setup(mock => mock.Send(It.IsAny<GetTicketDetailQuery>(), CancellationToken.None))
                .ReturnsAsync(reservedTicket);

            // Act
            var result = await _controller.Get(ticketId, CancellationToken.None);

            // Assert
            var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
            var returnedTicket = okResult.Value.ShouldBeAssignableTo<ReservedTicketDTO>();          
        }

        [Fact]
        public async Task ReserveSeats_WithValidData_ShouldReturnOkResponse()
        {
            // Arrange
            var reserveSeatsCommand = new ReserveSeatsCommand
            {
                Reservation = new ReservationDTO()
            };
            var response = new ReservedTicketDTO();
            _mediatorMock.Setup(mock => mock.Send(It.IsAny<ReserveSeatsCommand>(), CancellationToken.None))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.ReserveSeats(reserveSeatsCommand, CancellationToken.None);

            // Assert
            var okResult = result.ShouldBeOfType<OkObjectResult>();
            var returnedTicket = okResult.Value.ShouldBeAssignableTo<ReservedTicketDTO>();
        }

        [Fact]
        public async Task BuySeats_WithValidData_ShouldReturnOkResponse()
        {
            // Arrange
            var ticketId = Guid.NewGuid();

            var response = new ReservedTicketDTO();
            _mediatorMock.Setup(mock => mock.Send(It.IsAny<BuySeatsCommand>(), CancellationToken.None))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.BuySeats(ticketId, CancellationToken.None);

            // Assert
            var okResult = result.ShouldBeOfType<OkObjectResult>();
            var returnedTicket = okResult.Value.ShouldBeAssignableTo<ReservedTicketDTO>();
        }
    }
}
