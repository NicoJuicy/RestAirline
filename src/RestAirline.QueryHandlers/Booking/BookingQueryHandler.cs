using System.Threading;
using System.Threading.Tasks;
using EventFlow.EntityFramework;
using EventFlow.Queries;
using Microsoft.EntityFrameworkCore;
using RestAirline.Queries.Booking;
using RestAirline.ReadModel.EntityFramework;
using RestAirline.ReadModel.EntityFramework.DBContext;

namespace RestAirline.QueryHandlers.Booking
{
    public class BookingQueryHandler : IQueryHandler<BookingIdQuery, BookingReadModel>
    {
        private readonly IDbContextProvider<RestAirlineReadModelContext> _contextProvider;

        public BookingQueryHandler(IDbContextProvider<RestAirlineReadModelContext> contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task<BookingReadModel> ExecuteQueryAsync(BookingIdQuery query, CancellationToken cancellationToken)
        {
            using (var context = _contextProvider.CreateContext())
            {
                var readModel = await context.Bookings
                    .Include(x => x.Journeys)
                    .ThenInclude(x=>x.Flight)
                    .Include(x => x.Passengers)
                    .SingleAsync(x => x.Id == query.BookingId, cancellationToken);

                return readModel;
            }
        }
    }
}