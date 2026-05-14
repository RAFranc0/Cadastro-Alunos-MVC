using CadastroAlunosMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace CadastroAlunosMVC.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
    
    public DbSet<AlunoModel> Alunos { get; set; }
    public DbSet<UserModel> Users {get; set;}
}