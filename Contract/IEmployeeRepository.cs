using CompanyEmployees.Models;

namespace CompanyEmployees.Contract
{
    public interface IEmployeeRepository
    {

        IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges);
        Employee GetEmployee(Guid companyId, Guid id, bool trackChanges);

        void CreateEmploye(Guid companyId, Employee employee);
    }
}
