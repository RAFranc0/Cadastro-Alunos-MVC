using System.Security.Cryptography;
using System.Text;

namespace CadastroAlunosMVC.Utils;

public static class Criptografia
{
    public static string GerarHash(this string senha)
    {
        if (string.IsNullOrEmpty(senha)) return string.Empty;
        return BCrypt.Net.BCrypt.HashPassword(senha);
    }
    
    public static bool VerificarSenha(this string senhaDigitada, string hashDoBanco)
    {
        return BCrypt.Net.BCrypt.Verify(senhaDigitada, hashDoBanco);
    }
}