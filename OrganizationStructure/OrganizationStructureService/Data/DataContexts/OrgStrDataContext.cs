using Microsoft.EntityFrameworkCore;
using OrganizationStructureService.Data.Models;

namespace OrganizationStructureService.Data.DataContexts
{
    public class OrgStrDataContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Role> Roles { get; set; }

        public OrgStrDataContext(DbContextOptions<OrgStrDataContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    Name = "Head of IT",
                },
                new Role
                {
                    Id = 2,
                    Name = "Security Engineer Manager",
                },
                new Role
                {
                    Id = 3,
                    Name = "Network Engineer",
                },
                new Role
                {
                    Id = 4,
                    Name = "Senior Cloud Architect",
                },
                new Role
                {
                    Id = 5,
                    Name = "Help Desk Manager",
                },
                new Role
                {
                    Id = 6,
                    Name = "Security Engineer",
                },
                new Role
                {
                    Id = 7,
                    Name = "Cloud Architect",
                },
                new Role
                {
                    Id = 8,
                    Name = "IT Help Desk Technician",
                });


            modelBuilder.Entity<Person>()
                  .HasOne(u => u.Manager)
                  .WithMany()
                  .HasForeignKey(u => u.ManagerId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Person>()
                  .HasOne(u => u.Role)
                  .WithMany(r => r.Persons)
                  .HasForeignKey(u => u.RoleId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
