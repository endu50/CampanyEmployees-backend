using CompanyEmployees.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Contract
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }

        public async Task<IEnumerable<Employee>> GetEmployees(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges) =>
 await FindByCondition(e => e.CompanyId.Equals(companyId),trackChanges)
            .FilterEmployes(employeeParameters.MinAge,employeeParameters.MaxAge)
            .Search(employeeParameters.SearchTerm)
.OrderBy(e => e.Name).Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
.Take(employeeParameters.PageSize).ToListAsync();

        public async Task <Employee?>  GetEmployee(Guid companyId, Guid id, bool trackChanges) =>
   await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id),trackChanges)
    .SingleOrDefaultAsync();

        public void CreateEmploye(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee)
        {
            Delete(employee);
        }

      
    }
}
