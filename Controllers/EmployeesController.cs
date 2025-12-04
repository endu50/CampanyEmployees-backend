using System.Diagnostics.Metrics;
using System.Reflection.Metadata.Ecma335;
using AutoMapper;
using CompanyEmployees.Contract;
using CompanyEmployees.DTOObject;
using CompanyEmployees.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {

        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly RepositoryContext _context;
        public EmployeesController(IRepositoryManager repository, ILoggerManager logger,
        IMapper mapper, RepositoryContext context)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _context = context;
        }
        [HttpGet]
        public IActionResult GetEmployeesForCompany(Guid companyId)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }
            var employeesFromDb = _repository.Employee.GetEmployees(companyId, trackChanges: false);

            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

            return Ok(employeesDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employeeDb = _repository.Employee.GetEmployee(companyId, id, trackChanges:
        false);
            if (employeeDb == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            var employee = _mapper.Map<EmployeeDto>(employeeDb);


            return Ok(employee);
        }
        // [HttpPost]
        //public IActionResult createEmployeAt(Guid companyId, EmployeeForCreatingDto empDto)
        //{
        //    if(empDto == null)
        //    {
        //        _logger.LogError("Error No Client Data");
        //        return NotFound();
        //    }

        //    var camp = _repository.Company.GetCompany(companyId, trackChanges: false);
        //    if(camp == null)
        //    {
        //        _logger.LogError("No Campany Id Found");
        //        return NotFound();
        //    }
        //    var respnse = _mapper.Map<Employee>(empDto);

        //    _repository.Employee.CreateEmploye(companyId, respnse);
        //    _repository.Save();
        //    return Ok("Succesfully Created Employee!");
        //}
        [HttpPost]
        public async Task <IActionResult> createEmployeAt(Guid companyId, EmployeeForCreatingDto empDto)
        {
            var campany =await _context.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == companyId);
            if(campany == null)
            {
                _logger.LogError("No Campany id Found from Client");
                return NotFound();
            }
            var emp = new Employee
            {
                Name = empDto.Name,
                Age= empDto.Age,
                Position =empDto.Position,
                CompanyId = companyId
            };
            _context.Employees.Add(emp);
         await _context.SaveChangesAsync();
            return Ok(emp);
        }
    }
}
