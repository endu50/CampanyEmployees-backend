using System.Linq;
using CompanyEmployees.Models;

namespace CompanyEmployees.Contract
{
    public class DepartmentRepositery : RepositoryBase<Department>, IDepartmentRepositery
    {
        public DepartmentRepositery(RepositoryContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<Department> GetDepartments(bool trackChanges) =>
            FindAll(trackChanges).OrderBy(c => c.Name).ToList();
    }
}
