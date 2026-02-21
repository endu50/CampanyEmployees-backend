using CompanyEmployees.Models;

namespace CompanyEmployees.Contract
{
    public interface IDepartmentRepositery
    {
        IEnumerable<Department> GetDepartments(bool trackChanges);
        Department GetDepartmentById(int DeptId, bool trackChanges);
         void CreateDepartment(Department department);
        void UpdateDepartment(Department department);
    }
}
