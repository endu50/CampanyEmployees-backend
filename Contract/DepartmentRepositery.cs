using System.Linq;
using System.Security.Cryptography.X509Certificates;
using CompanyEmployees.Models;

namespace CompanyEmployees.Contract
{
    public class DepartmentRepositery : RepositoryBase<Department>, IDepartmentRepositery
    {
        public DepartmentRepositery(RepositoryContext repositoryContext) : base(repositoryContext) { }

        public void CreateDepartment(Department department) => Create(department);

        public Department GetDepartmentById(int DeptId, bool trackChanges) =>
            FindByCondition(c => c.DeptId.Equals(DeptId), trackChanges).OrderBy(c => c.Name).SingleOrDefault();
        

        public IEnumerable<Department> GetDepartments(bool trackChanges) =>
            FindAll(trackChanges).OrderBy(c => c.Name).ToList();

        public void UpdateDepartment(Department department)
        {
            Update(department);
        }
    }
}
