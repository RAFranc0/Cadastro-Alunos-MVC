using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CadastroAlunosMVC.Models;

namespace CadastroAlunosMVC.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        // PARA TESTE: Usuário "admin" e senha "123"
        if (model.Usuario == "admin" && model.Senha == "123")
        {
            // Claims são "afirmações" sobre o usuário
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Usuario),
                new Claim(ClaimTypes.Role, "Administrador"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties { IsPersistent = model.LembrarMe };

            // O comando que cria o Cookie criptografado no navegador
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), 
                authProperties);

            return RedirectToAction("Index", "Alunos");
        }

        ModelState.AddModelError("", "Usuário ou senha inválidos.");
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}