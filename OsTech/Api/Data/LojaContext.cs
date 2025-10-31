using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LojaContext : DbContext
    {
        public LojaContext(DbContextOptions<LojaContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}