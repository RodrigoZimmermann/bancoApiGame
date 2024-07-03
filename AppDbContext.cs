using Microsoft.EntityFrameworkCore;

namespace bancoApiGame
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Pergunta> Perguntas { get; set; }
        public DbSet<Resposta> Respostas { get; set; }
    }

    public class Pergunta
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string ChemicalElement { get; set; }
    }

    public class Resposta
    {
        public int Id { get; set; }
        public int Answer { get; set; }
        public int AlunoId { get; set; }
    }
}
