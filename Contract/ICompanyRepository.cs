
using CompanyEmployees.Models;

namespace CompanyEmployees.Contract
{
    public interface ICompanyRepository
    {
       Task <IEnumerable<Company> >GetAllCompanies(bool trackChanges);

       Task<Company?> GetCompany(Guid companyId, bool trackChanges);
       Task< IEnumerable<Company>> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
        void CreateCompany(Company company);

        void DeleteCampany(Company company);
    }
}
