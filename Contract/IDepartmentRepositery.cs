using CompanyEmployees.Models;

namespace CompanyEmployees.Contract
{
    public interface IDepartmentRepositery
    {
        IEnumerable<Department> GetDepartments(bool trackChanges);
    }
}
