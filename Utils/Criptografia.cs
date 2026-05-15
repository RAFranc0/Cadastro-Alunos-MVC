using System.Security.Cryptography;
using System.Text;

namespace CadastroAlunosMVC.Utils;

public static class Criptografia
{
    public static string GerarHash(this string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        
        using var sha256 = SHA256.Create();
        
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = sha256.ComputeHash(bytes);

        return Convert.ToHexString(hash);
    }
}