using GerenciadorEventos.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GerenciadorEventos.Models
{
    public class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            //Criando o escopo de servico para inicializar obter o contexto do bd
            using (var servico = app.ApplicationServices.CreateScope())
            {
                //obtem o conteudo do bd
                var context = servico.ServiceProvider.GetRequiredService<Context>();

                //Aplica migrações pendentes para garantir que o bd esteja atualizado
                context.Database.Migrate();

                //Verifica se já existem registros na tabela de eventos
                if (!context.Evento.Any())
                {
                    //Define o caminho do arquivo JSON de dados iniciais dentro do projeto
                    string jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "eventos.json");

                    //verifica se o arquivo existe, se não existir retorna nula e nao faz nada
                    if (!File.Exists(jsonPath)) return;

                    //Lê o conteúdo do arquivo JSON
                    string json = File.ReadAllText(jsonPath);

                    // Desserializa o JSON para a classe DadosIniciais, transformando-o em objetos C#
                    var dados = JsonSerializer.Deserialize<DadosIniciais>(json);

                    //verifica se os dados foram lidos corretamente
                    if (dados != null)
                    {
                        //Adiciona os eventos do json ao bd
                        context.Evento.AddRange(dados.Eventos);
                        context.SaveChanges();

                        //Associa os participantes aos eventos
                        foreach (var participantes in dados.Participantes)
                        {
                            //Procura o evento pelo nome correspondente fornecido no json
                            var evento = context.Evento.FirstOrDefault(e => e.Nome == participantes.EventoNome);

                            //se o evento for encontrado, adiciona o participante a ele
                            if (evento != null)
                            {
                                context.Participante.Add(new Participantes
                                {
                                    Nome = participantes.Nome,
                                    Email = participantes.Email,
                                    Telefone = participantes.Telefone,
                                    EventoId = evento.EventoId
                                });
                            }
                        }
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}
// Classe usada para desserializar o JSON, contendo listas de eventos e participantes
    public class DadosIniciais
    {
        public List<Eventos> Eventos { get; set; } = new(); 
        public List<ParticipanteDTO> Participantes { get; set; } = new(); 
    }

    // Classe DTO (Data Transfer Object) para representar participantes ao carregar o JSON
    public class ParticipanteDTO
    {
        public string Nome { get; set; } = ""; 
        public string Email { get; set; } = ""; 
        public string Telefone { get; set; } = ""; 
        public string EventoNome { get; set; } = ""; 
    }