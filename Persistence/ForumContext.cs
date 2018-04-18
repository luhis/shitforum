using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace Persistence
{
    public class ForumContext : DbContext
    {
        public DbSet<Board> Boards { get; private set; }
        public DbSet<Thread> Threads { get; private set; }
        public DbSet<Post> Posts { get; private set; }
        public DbSet<File> Files { get; private set; }
        public DbSet<BannedImage> BannedImages { get; private set; }
        public DbSet<BannedIp> BannedIp { get; private set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=forum.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetupBoardTable.Setup(modelBuilder.Entity<Board>());
            SetupThreadTable.Setup(modelBuilder.Entity<Thread>());
            SetupPostTable.Setup(modelBuilder.Entity<Post>());
            SetupFileTable.Setup(modelBuilder.Entity<File>());
            SetupBannedImage.Setup(modelBuilder.Entity<BannedImage>());
            SetupBannedIp.Setup(modelBuilder.Entity<BannedIp>());
        }

        public async Task SeedData()
        {
            if (!await this.Boards.AnyAsync())
            {
                await this.Boards.AddAsync(new Board(new System.Guid("1f5f47db-dd27-4b58-9229-4ae72829621e"), "Random", "B"));
                await this.SaveChangesAsync();
            }
        }
    }
}