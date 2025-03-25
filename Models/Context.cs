using Microsoft.EntityFrameworkCore;

namespace GerenciadorEventos.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<Eventos> Evento { get; set; }
        public DbSet<Participantes> Participante { get; set; }

    }
}
