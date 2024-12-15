using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class KeyMappingDbContext : DbContext
    {
        public KeyMappingDbContext(DbContextOptions<KeyMappingDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KeyMapping>()
                .HasKey(km => km.UserId);
            modelBuilder.Entity<KeyMapping>()
                .Property(km => km.Key)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasKey(km => km.Id);
            modelBuilder.Entity<User>()
                .Property(km => km.Login)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(km => km.Password)
                .IsRequired();
        }

        public DbSet<KeyMapping> KeyMappings { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
