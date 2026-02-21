using CompanyEmployees.DTOObject;

namespace CompanyEmployees.Contract
{
    public interface IAuthenticationManager
    {
        Task<bool> ValidateUser(UserForAuthenticationDto userForAuth);
        Task<string> CreateToken();
    }
}
