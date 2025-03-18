using Microsoft.EntityFrameworkCore;

namespace WebAppWithHighSecurity.Data
{
    public class CprTodoDbContext : DbContext
    {
        public CprTodoDbContext(DbContextOptions<CprTodoDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserCprNumber> UserCprNumbers { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserCprNumber>()
                .HasKey(u => u.UserId);
        }
    }
}