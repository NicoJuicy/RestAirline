﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventFlow;
using EventFlow.Queries;
using Microsoft.AspNetCore.Mvc;
using RestAirline.Api.Resources.Booking;
using RestAirline.Api.Resources.Booking.Journey;
using RestAirline.Api.Resources.Booking.Passenger;
using RestAirline.Domain.Booking;
using RestAirline.QueryHandlers.Booking;
using RestAirline.ReadModel.EntityFramework;
using RestAirline.Shared.ModelBuilders;
using AddPassengerCommand = RestAirline.CommandHandlers.Passenger.AddPassengerCommand;
using SelectJourneysCommand = RestAirline.Commands.Journey.SelectJourneysCommand;
using UpdatePassengerNameCommand = RestAirline.CommandHandlers.Passenger.UpdatePassengerNameCommand;

namespace RestAirline.Api.Controllers
{
    [Route("api/booking")]
    public class BookingController : Controller
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryHandler<ReadModelByIdQuery<BookingReadModel>, BookingReadModel> _bookingQueryHandler;
        private readonly BookingQueryHandler _queryHandler;

        public BookingController(ICommandBus commandBus,  
            IQueryHandler<ReadModelByIdQuery<BookingReadModel>, BookingReadModel> bookingQueryHandle,
            BookingQueryHandler queryHandler
            )
        {
            _commandBus = commandBus;
            _bookingQueryHandler = bookingQueryHandle;
            _queryHandler = queryHandler;
        }

        [Route("journeys")]
        [HttpPost]
        public async Task<JourneysSelectedResource> SelectJourneys(List<string> journeyIds)
        {
            var journeys = new JourneysBuilder().BuildJourneys();
            var bookingId = BookingId.New;

            var command = new SelectJourneysCommand(bookingId, journeys);
            await _commandBus.PublishAsync(command, CancellationToken.None);

            return new JourneysSelectedResource(Url, bookingId.Value);
        }

        [Route("{bookingId}")]
        [HttpGet]
        public async Task<BookingResource> GetBooking(string bookingId)
        {
            // Not sure why this does not work
//            var booking = await _bookingQueryHandler.ExecuteQueryAsync(
//                new ReadModelByIdQuery<BookingReadModel>(bookingId),
//                new CancellationToken());
            
            var booking = await _queryHandler.ExecuteQueryAsync(
                new ReadModelByIdQuery<BookingReadModel>(bookingId),
                new CancellationToken());
            
            return new BookingResource(Url, booking);
        }

        [Route("{bookingId}/passenger")]
        [HttpPost]
        public async Task<PassengerAddedResource> AddPassenger(string bookingId, AddPassengerCommand addPassengerCommand)
        {
            var passenger = new PassengerBuilder().CreatePassenger();

            var command = new AddPassengerCommand(new BookingId(bookingId), passenger);
            await _commandBus.PublishAsync(command, CancellationToken.None);
            
            return new PassengerAddedResource(Url, bookingId, passenger.PassengerKey);
        }

        [Route("{bookingId}/passenger/{passengerKey}/name")]
        [HttpPost]
        public async Task<PassengerNameUpdatedResource> UpdatePassengerName(string bookingId, string passengerKey,
            string name)
        {
            name = "new-name";
            var command = new UpdatePassengerNameCommand(new BookingId(bookingId), passengerKey, name);
            await _commandBus.PublishAsync(command, CancellationToken.None);

            return new PassengerNameUpdatedResource(Url, bookingId);
        }
    }
}