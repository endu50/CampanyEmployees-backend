using CompanyEmployees.Models;

namespace CompanyEmployees.Contract
{
    public interface IEmployeeRepository
    {

       Task <IEnumerable<Employee>> GetEmployees(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges);
       Task <Employee?> GetEmployee(Guid companyId, Guid id, bool trackChanges);

        void CreateEmploye(Guid companyId, Employee employee);

        void DeleteEmployee(Employee employee);
    }
}
