using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Models
{
    public class Department
    {
        [Key]
        public int DeptId { get; set; }

        [Required(ErrorMessage ="Department Name is Required")]
        public string Name { get; set; }
        public string Description { get; set; }

        // One-to-many: a department has many employees
         
        public ICollection<Employee> employees { get; set; } = new List<Employee>();

    }
}
