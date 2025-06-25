namespace cnd
{
    using cnd.Models;
    using Microsoft.EntityFrameworkCore;

    public class JournalDbContext : DbContext
    {
        public JournalDbContext(DbContextOptions<JournalDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<Song> Songs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Entries)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<JournalEntry>()
                .HasOne(e => e.Song)
                .WithOne(s => s.JournalEntry)
                .HasForeignKey<Song>(s => s.JournalEntryId);

            base.OnModelCreating(modelBuilder);
        }
    }

}
