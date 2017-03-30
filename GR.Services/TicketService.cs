using GR.Repository;

namespace GR.Services
{
    public class TicketService : Repository<Ticket>, ITicketService
    {
        private readonly IRepository<Ticket> _ticketRepository;

        public TicketService(string connectionString) : base(connectionString)
        {
            _ticketRepository = new Repository<Ticket>(connectionString);
        }
    }
}
