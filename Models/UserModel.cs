namespace CadastroAlunosMVC.Models;

public class UserModel
{
    public Guid Id { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}