using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CadastroAlunosMVC.Data;
using CadastroAlunosMVC.Models;
using CadastroAlunosMVC.Utils;
using Microsoft.EntityFrameworkCore;

namespace CadastroAlunosMVC.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;
    public AccountController(AppDbContext db) => _db = db;
    
    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var usuarioNoBanco = await _db.Users
            .FirstOrDefaultAsync(u => u.Usuario == model.Usuario);
        
        if (usuarioNoBanco == null)
        {
            ModelState.AddModelError("LoginErro", "Usuário não encontrado.");
            return View(model);
        }

        if (model.Senha.VerificarSenha(usuarioNoBanco.Senha))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuarioNoBanco.Usuario),
                new Claim(ClaimTypes.Role, "Administrador"),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = model.LembrarMe };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), 
                authProperties);

            return RedirectToAction("Index", "Alunos");
        }
        
        ModelState.AddModelError("LoginErro", "Senha inválida.");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var jaExiste = await _db.Users.AnyAsync(u => u.Usuario.ToUpper() == model.User.Usuario.ToUpper());
                if (jaExiste) 
                {
                    ModelState.AddModelError("CadastroErro", "Este usuário já existe.");
                    return View("Login", model);
                }
                
                model.User.Id  = Guid.NewGuid();
                model.User.Senha = model.User.Senha.GerarHash();
                _db.Users.Add(model.User);
                await _db.SaveChangesAsync();
                
                return RedirectToAction(nameof(Login));
            }
            catch(DbUpdateException)
            {
                ModelState.AddModelError("CadastroErro", "Não foi possível realizar o cadastro. Tente novamente.");
            }
        }

        return View("Login", model);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}