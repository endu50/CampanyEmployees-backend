using System.ComponentModel.Design;
using AutoMapper;
using CompanyEmployees.Contract;
using CompanyEmployees.DTOObject;
using CompanyEmployees.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CompanyEmployees.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;
        private readonly RepositoryContext _context;
        public ValuesController(ILogger<ValuesController> logger, IMapper mapper,
            IRepositoryManager repository, RepositoryContext context)
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
            _context = context;
        }

        [HttpGet("")]
        public Task< IActionResult> Get()
        {
            var result =  _repository.Company.GetAllCompanies(trackChanges:false);
            if(result == null)
            {
                _logger.LogInformation("No Campany data found");
            }
            var response = _mapper.Map<IEnumerable<CompanyDto>>(result);

            return Task.FromResult<IActionResult>(Ok(response));
      
        }
        [HttpGet("Department")]
        public async Task< ActionResult <IEnumerable<Department>>> GetDepartmentsIN()
        {
            //var dep = _context.Departments.FirstOrDefault(c => c.DeptId.Equals(dto.DeptId));
            //if (dep== null)
            //{
            //    _logger.LogError("No Dept Id Found");
            //}
            var department = await _context.Departments.ToListAsync();

            if (department == null)
            {
                _logger.LogError("empty departments");
            }

            return  Ok(department);
        }

        [HttpPost("department")]  
        public ActionResult  CreateDepartment( [FromBody] DepartmentForCreation departmentdto)
        {
           var  department = new Department
           {
               Name = departmentdto.Name,
               Description = departmentdto.Description
           };
            _context.Departments.Add(department);
            _context.SaveChanges();
            // return Ok ("Department Created Successfully!!");
            return CreatedAtRoute("deptId", new { deptId = department.DeptId }, department);

        }

        [HttpPost]
        public IActionResult CreateEmployee( Guid companyId, [FromBody] EmployeeForCreatingDto employeeDto)
        {
            if(employeeDto == null)
            {
                _logger.LogError("Error No Input for creating Employee");
                return BadRequest("No Data Found for creating Employee");
            }
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if(company == null)
            {
                _logger.LogError("Camapany Id Not Found");
                return NotFound("Camapany Id Not Found");
            }

            var response = _mapper.Map<Employee>(employeeDto);
            //  return Ok(response);
            _repository.Employee.CreateEmploye( companyId,response);
            _repository.Save();
            //return Ok("Employee Created Successfully!");
            var employeeToReturn = _mapper.Map<EmployeeDto>(response);
            return CreatedAtRoute("GetEmployeeForCompany", new
            {
                companyId, id = employeeToReturn.Id },
                employeeToReturn);

        }
    }
}
