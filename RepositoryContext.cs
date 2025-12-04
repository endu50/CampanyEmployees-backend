using CompanyEmployees.Configuration;
using CompanyEmployees.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees
{
    public class RepositoryContext : DbContext
    {
       // public DbContextOptions _options;
        public RepositoryContext(DbContextOptions options) : base (options)
        {
           // _options = options;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
        }
            public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public DbSet<Department> Departments { get; set; }


    }
}
