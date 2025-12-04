using CompanyEmployees.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyEmployees.Configuration
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {

        public void Configure(EntityTypeBuilder<Department> builder)
        {
            //throw new NotImplementedException();
            builder.HasData
                (
              new Department
              {
                 DeptId  = 123,
                  Name = "Accounting",
                  Description = "Best Accounting"
              },
              new Department
              {
                  DeptId =  456,
                  Name = "finanance",
                  Description = "Best Finanace"
              },
              new Department
              {
                  DeptId = 789,
                  Name = "Marketing",
                  Description ="Best Marketing"
                 
              }
              
               
                );
        }
    }
}
