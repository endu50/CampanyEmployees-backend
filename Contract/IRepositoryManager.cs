using System.Security.Cryptography;

namespace CompanyEmployees.Contract
{
    public interface IRepositoryManager
    {
         ICompanyRepository Company { get; }
         IEmployeeRepository Employee { get; }
         IDepartmentRepositery Department { get; }
        void Save();
    }
}
