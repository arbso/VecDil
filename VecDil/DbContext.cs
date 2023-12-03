using Microsoft.EntityFrameworkCore;
using VecDil.Models.Bar;
using VecDil.Models.User;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Bar> Bars { get; set; }
}