using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GerenciadorEventos.Models
{
    public class Participantes
    {
        [Key]
        public int ParticipanteId { get; set; }
        [Required]
        public string Nome { get; set; } = "";
        [Required]
        public string Email { get; set; } = "";
        [Required]
        public string Telefone { get; set; } = "";
        [ForeignKey("Evento")]
        public int EventoId { get; set; }
        public Eventos Evento { get; set; }
        public bool Participacao { get; set; } = false;

    }
}
