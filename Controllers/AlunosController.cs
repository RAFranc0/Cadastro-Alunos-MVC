using CadastroAlunosMVC.Data;
using CadastroAlunosMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CadastroAlunosMVC.Controllers;

public class AlunosController : Controller
{
    private readonly AppDbContext _db;
    public AlunosController(AppDbContext db) => _db = db;
    
    //===============================================
    //REGIÃO: Listagem de Alunos
    //===============================================

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var alunos = await _db.Alunos.AsNoTracking().ToListAsync();
        
        return View(alunos);
    }
    
    //===============================================
    //REGIÃO: Cadastro de Alunos
    //===============================================

    [HttpGet]
    [Authorize]
    public IActionResult Create()
    {
        return View("CadastrarAluno");
    }
    
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AlunoModel alunoModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _db.Alunos.Add(alunoModel);
                await _db.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                ModelState.AddModelError("", "Não foi possível salvar as alterações. Tente novamente.");
            }
        }
        return RedirectToAction(nameof(Index));
    }
    
    //===============================================
    //REGIÃO: Edição de Alunos
    //===============================================

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AlunoModel alunoModel)
    {
        if (!ModelState.IsValid) return View("ViewEditarAluno", alunoModel);

        var alunoExistente = await _db.Alunos.FindAsync(alunoModel.Id);
        if (alunoExistente == null) return NotFound();

        try
        {
            alunoExistente.Nome = alunoModel.Nome;
            alunoExistente.DataNascimento = alunoModel.DataNascimento;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Erro ao atualizar banco de dados.");
        }
        
        return View("Index", alunoModel);
    }
    
    //===============================================
    //REGIÃO: Exclusão de Alunos
    //===============================================
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var aluno = await _db.Alunos.FindAsync(id);
        if (aluno == null) return NotFound();

        try
        {
            _db.Alunos.Remove(aluno);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            return BadRequest("Erro ao excluir aluno.");
        }
    }
}