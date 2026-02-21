using CompanyEmployees.Models;

namespace CompanyEmployees.DTOObject
{
    public class CampanyForUpdateDto
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public string Country { get; set; }

      //  public IEnumerable<EmployeeForCreatingDto> employees { get; set; }
    }
}
