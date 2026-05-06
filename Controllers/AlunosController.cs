using CadastroAlunosMVC.Data;
using CadastroAlunosMVC.Models;
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

    public async Task<IActionResult> ListarAlunosAsync()
    {
        var alunos = await _db.Alunos.AsNoTracking().ToListAsync();
        
        return View(alunos);
    }
    
    //===============================================
    //REGIÃO: Cadastro de Alunos
    //===============================================

    [HttpGet]
    public IActionResult CadastrarAluno()
    {
        return  View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAlunoAsync(AlunoModel alunoModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _db.Alunos.Add(alunoModel);
                await _db.SaveChangesAsync();
                return View("ListarAlunos");
            }
            catch(DbUpdateException)
            {
                ModelState.AddModelError("", "Não foi possível salvar as alterações. Tente novamente.");
            }
        }
        return View("CadastrarAluno", alunoModel);
    }
    
    //===============================================
    //REGIÃO: Edição de Alunos
    //===============================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAlunoAsync(AlunoModel alunoModel)
    {
        if (!ModelState.IsValid) return View("ViewEditarAluno", alunoModel);

        var alunoExistente = await _db.Alunos.FindAsync(alunoModel.Id);
        if (alunoExistente == null) return NotFound();

        try
        {
            alunoExistente.Nome = alunoModel.Nome;
            alunoExistente.DataNascimento = alunoModel.DataNascimento;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ListarAlunosAsync));
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Erro ao atualizar banco de dados.");
        }
        
        return View("ViewEditarAluno", alunoModel);
    }
    
    //===============================================
    //REGIÃO: Exclusão de Alunos
    //===============================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletarAlunoAsync(int id)
    {
        var aluno = await _db.Alunos.FindAsync(id);
        if (aluno == null) return NotFound();

        try
        {
            _db.Alunos.Remove(aluno);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(ListarAlunosAsync));
        }
        catch (DbUpdateException)
        {
            return BadRequest("Erro ao excluir aluno.");
        }
    }
}