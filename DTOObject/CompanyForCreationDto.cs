using System.ComponentModel.DataAnnotations;

namespace CompanyEmployees.DTOObject
{
    public class CompanyForCreationDto
    {
        [Required(ErrorMessage ="Requied Name")]
        [MinLength(10,ErrorMessage ="Company Name Length bust be 10")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Requied Address")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Requied Country")]
        public string Country { get; set; }

       // public IEnumerable<EmployeeForCreatingDto> Employees { get; set; }
    }
}
