using System.ComponentModel.Design;
using AutoMapper;
using CompanyEmployees.Contract;
using CompanyEmployees.DTOObject;
using CompanyEmployees.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repository;
        public ValuesController(ILogger<ValuesController> logger, IMapper mapper,IRepositoryManager repository)
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;           
        }

        [HttpGet("")]
        public async Task< IActionResult> Get()
        {
            var result =  _repository.Company.GetAllCompanies(trackChanges:false);
            if(result == null)
            {
                _logger.LogInformation("No Campany data found");
            }
            var response = _mapper.Map<IEnumerable<CompanyDto>>(result);

            return Ok(response);
      
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
