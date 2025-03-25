using GerenciadorEventos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using X.PagedList.Extensions;

namespace GerenciadorEventos.Controllers
{
    public class EventoController : Controller
    {
        public Context context;
        public EventoController(Context ctx)
        {
            context = ctx;
        }
        public IActionResult Index(int pagina = 1)
        {
            return View(context.Evento
                .Include(p => p.Participantes)
                .ToPagedList(pagina, 10));
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.ParticipanteId = new SelectList(context.Participante.OrderBy(p => p.Nome),
                "ParticipanteId", "Nome");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Eventos evt)
        {
            context.Add(evt);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var evento = context.Evento
                .Include(p => p.Participantes)
                .FirstOrDefault(e => e.EventoId == id);
            return View(evento);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var evento = context.Evento.Find(id);
            ViewBag.ParticipanteId = new SelectList(context.Evento.OrderBy(f => f.Nome),
                "ParticipanteId", "Nome");
            return View(evento);
        }
        [HttpPost]
        public IActionResult Edit(Eventos evt)
        {
            //Avisa a EF que o registo será modificado
            context.Entry(evt).State = EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var evento = context.Evento
                .Include(p => p.Participantes)
                .FirstOrDefault(e => e.EventoId == id);
            return View(evento);
        }
        [HttpPost]
        public IActionResult Delete(Eventos evt)
        {
            context.Evento.Remove(evt);
            context.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
