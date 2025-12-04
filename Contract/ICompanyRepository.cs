
using CompanyEmployees.Models;

namespace CompanyEmployees.Contract
{
    public interface ICompanyRepository
    {
        IEnumerable<Company> GetAllCompanies(bool trackChanges);

        Company GetCompany(Guid companyId, bool trackChanges);

        void CreateCompany(Company company);
    }
}
