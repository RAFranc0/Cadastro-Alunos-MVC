namespace CadastroAlunosMVC.Models;

public class LoginViewModel
{
    public string Usuario { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public bool LembrarMe { get; set; }
}