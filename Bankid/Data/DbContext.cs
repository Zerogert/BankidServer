using Bankid.Data.Mappings;
using Bankid.Models.Entities;
using Bankid.Models.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bankid.Data {
    public class AppDbContext : IdentityDbContext<User, Role, int> {
        private readonly DbConfiguration _dbConfiguration;

        public DbSet<Course> Courses { get; set; }

        public AppDbContext(DbConfiguration dbConfiguration) {
            _dbConfiguration = dbConfiguration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite(_dbConfiguration.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CourseMap());
        }
    }
}
