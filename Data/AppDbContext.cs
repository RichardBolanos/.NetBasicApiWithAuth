using GestionTareasApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace GestionTareasApi.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<User> Users { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }

}
