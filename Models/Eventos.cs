using System.ComponentModel.DataAnnotations;

namespace GerenciadorEventos.Models
{
    public class Eventos
    {
        [Key]
        public int EventoId { get; set; }
        [Required]
        public string Nome { get; set; } = "";
        [Required]
        public string Local { get; set; } = "";
        [Required]
        public DateTime Data { get; set; }
        public string? Descricao { get; set; }
        public List<Participantes> Participantes { get; set; } = new();

    }
}
