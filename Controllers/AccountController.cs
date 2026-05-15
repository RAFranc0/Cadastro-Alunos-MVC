using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CadastroAlunosMVC.Data;
using CadastroAlunosMVC.Models;
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
            ModelState.AddModelError("", "Usuário não encontrado.");
            return View(model);
        }
        
        bool senhaCorreta = usuarioNoBanco.Senha == model.Senha;

        if (senhaCorreta)
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
        
        ModelState.AddModelError("", "Senha inválida.");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                model.User.Id  = Guid.NewGuid();
                _db.Users.Add(model.User);
                await _db.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                ModelState.AddModelError("", "Não foi possível realizar o cadastro. Tente novamente.");
            }
        }
        return RedirectToAction(nameof(Index), "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}