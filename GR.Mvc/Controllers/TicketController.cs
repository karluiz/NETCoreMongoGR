using System;
using System.Linq;
using System.Threading.Tasks;
using GR.Mvc.ViewModel;
using GR.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace GR.Mvc.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public async Task<ActionResult> Index()
        {
            var list = await _ticketService.GetAll();
            var model = new TicketViewModel
            {
                Tickets = list.ToList()
            };

            return View(model);
        }

        public ActionResult Add()
        {
            var model = new TicketViewModel {Ticket = new Ticket()};
            return View(model);
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (ModelState.IsValid)
            {
                var ticket = await _ticketService.Get(ObjectId.Parse(id));
                ticket.IdString = ticket.Id.ToString();
                var model = new TicketViewModel
                {
                    Ticket = ticket
                };
                return View(model);
            }

            return RedirectToAction("Index");
        }

        
        public async Task<ActionResult> Delete(string id)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index");
            await _ticketService.Delete(ObjectId.Parse(id));

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> AddOrUpdate(Ticket ticket)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index");
            if (ticket == null) return RedirectToAction("Index");
            if (ticket.Version == 0)
            {
                ticket.Version++;
                ticket.CreatedAt = DateTime.UtcNow;
                await _ticketService.Insert(ticket);
            }
            else
            {
                ticket.Id = ObjectId.Parse(ticket.IdString);
                ticket.Version++;
                ticket.UpdatedAt = DateTime.UtcNow;
                await _ticketService.Update(ticket);
            }

            return RedirectToAction("Index");
        }
    }
}
