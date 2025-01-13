using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using ThreadIt.Models;

namespace ThreadIt.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Models.Thread> Threads { get; set; }
        public DbSet<Subscribe> Subscribes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Subscribe>()
                .HasKey(e => new { e.Id, e.UserId, e.CategoryId });

            modelBuilder.Entity<Subscribe>()
                .HasOne(e => e.User)
                .WithMany(e => e.Subs)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Subscribe>()
                .HasOne(e => e.Category);

            modelBuilder.Entity<Comment>()
                .HasOne<Models.Thread>(e => e.Thread)
                .WithMany(e => e.Comments)
                .HasForeignKey(e => e.ThreadId);

            modelBuilder.Entity<Comment>()
                .HasOne<Comment>(e => e.Parent)
                .WithMany(e => e.Replies)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.Thread>()
                .HasOne<Category>(e => e.Category)
                .WithMany(e => e.Threads)
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<Models.Thread>()
                .HasOne<AppUser>(e => e.User)
                .WithMany(e => e.Threads)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Comment>()
               .HasOne<AppUser>(e => e.User)
               .WithMany(e => e.Comments)
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.ParentId)
                .HasDatabaseName("Index_Comment_ParentID");

            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.ThreadId)
                .HasDatabaseName("Index_Comment_ThreadId");

            modelBuilder.Entity<Models.Thread>()
                .HasIndex(t => t.CreateTime)
                .HasDatabaseName("Index_Thread_Date");

        }
    }

    
}
