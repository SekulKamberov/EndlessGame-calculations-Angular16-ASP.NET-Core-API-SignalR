using Microsoft.EntityFrameworkCore;

namespace EndlessGame.Entities
{
  public class EndlessGameDBContext : DbContext
  {
    public EndlessGameDBContext(DbContextOptions<EndlessGameDBContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
  }
}
