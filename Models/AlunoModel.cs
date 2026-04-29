namespace CadastroAlunosMVC.Models;

public class AlunoModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
}