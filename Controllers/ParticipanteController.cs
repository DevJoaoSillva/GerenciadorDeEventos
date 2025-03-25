using GerenciadorEventos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace GerenciadorEventos.Controllers
{
    public class ParticipanteController : Controller
    {
        private readonly Context _context;

        public ParticipanteController(Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var participantes = _context.Participante.Include(p => p.Evento).ToList();
            return View(participantes);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var eventos = _context.Evento.OrderBy(x => x.Nome).ToList();
            foreach (var evento in eventos)
            {
                Console.WriteLine($"Evento ID: {evento.EventoId}, Nome: {evento.Nome}");
            }
            ViewBag.EventoId = new SelectList(eventos, "EventoId", "Nome");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Participantes participante)
        {
            ViewBag.EventoId = new SelectList(_context.Evento.OrderBy(x => x.Nome), "EventoId", "Nome");
            _context.Participante.Add(participante);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            var participante = _context.Participante.Find(id);
            if (participante == null)
                return NotFound();

            ViewBag.EventoId = new SelectList(_context.Evento.OrderBy(x => x.Nome), "EventoId", "Nome", participante.EventoId);
            return View(participante);
        }

        [HttpPost]
        public IActionResult Edit(Participantes participante)
        {
            if (ModelState.IsValid)
            {
                _context.Participante.Update(participante);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EventoId = new SelectList(_context.Evento.OrderBy(x => x.Nome), "EventoId", "Nome", participante.EventoId);
            return View(participante);
        }

        public IActionResult Details(int id)
        {
            var participante = _context.Participante.Include(p => p.Evento).FirstOrDefault(p => p.ParticipanteId == id);
            if (participante == null)
                return NotFound();

            return View(participante);
        }
        public IActionResult Delete(int id)
        {
            var participante = _context.Participante.Find(id);
            if (participante == null)
                return NotFound();

            return View(participante);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var participante = _context.Participante.Find(id);
            if (participante != null)
            {
                _context.Participante.Remove(participante);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public IActionResult Candidatar(int eventoId)
        {
            var evento = _context.Evento.Find(eventoId);

            if (evento == null)
            {
                return NotFound("Evento não encontrado."); // Retorna erro se o evento não existir
            }

            ViewBag.Evento = evento; // Passa o evento para a View
            return View(new Participantes { EventoId = evento.EventoId });
        }

        [HttpPost]
        public IActionResult Candidatar(Participantes participante)
        {
            if (ModelState.IsValid)
            {
                _context.Participante.Add(participante);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Evento = _context.Evento.Find(participante.EventoId);
            return View(participante);
        }
        public IActionResult ConfirmarParticipacao(int id)
        {
            var participante = _context.Participante.Find(id);
            if (participante == null)
                return NotFound();

            participante.Participacao = true;
            _context.SaveChanges();

            return RedirectToAction("Index"); // Redireciona para a lista de participantes
        }


        public IActionResult GerarCertificado(int id)
        {
            var participante = _context.Participante.Include(p => p.Evento).FirstOrDefault(p => p.ParticipanteId == id);

            if (participante == null || !participante.Participacao)
                return NotFound("Participação não confirmada!");

            using (var stream = new MemoryStream())
            {
                // Definir documento na horizontal
                var pdf = new Document(PageSize.A4.Rotate(), 50, 50, 50, 50);
                PdfWriter writer = PdfWriter.GetInstance(pdf, stream);
                pdf.Open();

                // Criar fontes estilizadas
                Font titleFont = new Font(Font.FontFamily.HELVETICA, 30, Font.BOLD, BaseColor.BLACK);
                Font textFont = new Font(Font.FontFamily.HELVETICA, 18, Font.NORMAL, BaseColor.BLACK);
                Font boldFont = new Font(Font.FontFamily.HELVETICA, 20, Font.BOLD, BaseColor.BLACK);

                // Criar borda decorativa
                PdfContentByte canvas = writer.DirectContent;
                Rectangle border = new Rectangle(pdf.PageSize);
                border.Left += pdf.LeftMargin - 20;
                border.Right -= pdf.RightMargin - 20;
                border.Top -= pdf.TopMargin - 20;
                border.Bottom += pdf.BottomMargin - 20;
                border.BorderWidth = 5;
                border.BorderColor = BaseColor.BLACK;
                border.Border = Rectangle.BOX;
                canvas.Rectangle(border);

                // Adicionar título centralizado
                Paragraph title = new Paragraph("CERTIFICADO DE PARTICIPAÇÃO", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 40
                };
                pdf.Add(title);

                // Adicionar conteúdo do certificado
                Paragraph corpo = new Paragraph($"Certificamos que ", textFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                corpo.Add(new Chunk(participante.Nome, boldFont)); // Nome em negrito
                corpo.Add(new Chunk($" participou do evento \"{participante.Evento.Nome}\" fornecido pela Universidade, realizado em {participante.Evento.Data.ToShortDateString()}.", textFont));
                corpo.SpacingAfter = 50;
                pdf.Add(corpo);

                // Espaço para assinatura
                Paragraph assinatura = new Paragraph("_________________________\nOrganização Responsável", textFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingBefore = 50
                };
                pdf.Add(assinatura);

                pdf.Close();
                return File(stream.ToArray(), "application/pdf", "certificado.pdf");
            }
        }
    }
}
