using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence.Setup;
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
            optionsBuilder.UseSqlite("Data Source=../shitForum.db");
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
            await this.Database.MigrateAsync();
            if (!await this.Boards.AnyAsync())
            {
                this.Boards.Add(new Board(new System.Guid("1f5f47db-dd27-4b58-9229-4ae72829621e"), "Random", "b"));
                this.Boards.Add(new Board(new System.Guid("ec9ec01d-d0ed-4b55-a3ad-78bc32b0ebc9"), "International", "int"));
                this.Boards.Add(new Board(new System.Guid("3643c3d5-a917-470d-a41e-14d317ef24a7"), "Politically Suicidal", "pol"));
                this.Boards.Add(new Board(new System.Guid("4fcca824-1986-437f-b844-4373d9b5f855"), "Brighton", "btn"));
                await this.SaveChangesAsync();
            }
        }
    }
}
