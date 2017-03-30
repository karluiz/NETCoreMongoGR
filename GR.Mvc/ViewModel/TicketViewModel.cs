using System.Collections.Generic;
using GR.Services;

namespace GR.Mvc.ViewModel
{
    public class TicketViewModel
    {
        public Ticket Ticket { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}
