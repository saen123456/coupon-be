// Models/SampleDBContext.cs

using Microsoft.EntityFrameworkCore;

namespace DotnetWebApiWithEFCodeFirst.Models
{
    public partial class SampleDBContext : DbContext
    {
        public SampleDBContext(DbContextOptions
        <SampleDBContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<Coupons> Coupons { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(k => k.id);
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.HasKey(k => k.id);
            });

            modelBuilder.Entity<Coupons>(entity =>
            {
                entity.HasKey(k => k.id);
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}