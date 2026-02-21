using System.ComponentModel.Design;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata.Ecma335;
using AutoMapper;
using CompanyEmployees.Contract;
using CompanyEmployees.DTOObject;
using CompanyEmployees.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId, [FromQuery] EmployeeParameters employeeParameters)
        {

            if (!employeeParameters.ValidAgeRange)
            {
                return BadRequest("Max age can't be less than min age.");
            }
            var company =  await _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }
          

            var employeesFromDb = await _repository.Employee.GetEmployees(companyId, employeeParameters, trackChanges: false);

            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

            return Ok(employeesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = await _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employeeDb =await _repository.Employee.GetEmployee(companyId, id, trackChanges:
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
        public async Task<IActionResult> createEmployeAt(Guid companyId, EmployeeForCreatingDto empDto)
        {
            var campany = await _context.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == companyId);
            if (campany == null)
            {
                _logger.LogError("No Campany id Found from Client");
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation Error from Client Dto");
                return UnprocessableEntity(ModelState);
            }
            var emp = new Employee
            {
                Name = empDto.Name,
                Age = empDto.Age,
                Position = empDto.Position,
                CompanyId = companyId
            };
            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();
            return Ok(emp);
        }
        [HttpDelete("id")]
        public async Task<ActionResult> DeleteDepartment(Guid companyId, Guid id)
        {
            var campany = await _repository.Company.GetCompany(companyId, trackChanges: false);
            if (campany == null)
            {
                _logger.LogError("No Company  Id Found To Delete");
                return NotFound("NO Campany ID Found to Delete");
            }
            var department = await _repository.Employee.GetEmployee(companyId, id, trackChanges: false);
            //  double dep=  Convert.(department);
            if (department == null)
            {
                _logger.LogError("No Id Found To Delete");
                return NotFound("NO ID Found to Delete");
            }

            _repository.Employee.DeleteEmployee(department);
           await _repository.Save();
            return Ok("The Employee Id Deleted Successfully");
        }
        [HttpPut("{id}")]
        public async Task <ActionResult> UpdateEmploye(Guid campanyId,Guid id, [FromBody] EmployeeForUpdateDto dto)
        {
            if (dto == null)
            {
                _logger.LogError("No User Data From Clien");
                return NotFound("No User Data From Client");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid Model state from Client Dto");
                return UnprocessableEntity(ModelState);
            }
            var company =await _repository.Company.GetCompany(campanyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {campanyId} doesn't exist in the  database.");
                return NotFound();
            }
            var empdb = _repository.Employee.GetEmployee(campanyId, id, trackChanges: true);
            if (dto == null)
            {
                _logger.LogError("No User ID Found For Update");
                return NotFound("No User ID  Found For Update");
            }
            var emp = _mapper.Map(dto, empdb);
           await _repository.Save();
            return Ok("Employee Is Updated Successfully");

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id,
[FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }
            var company =await _repository.Company.GetCompany(companyId, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the  database."); 
                return NotFound();
            }
            var employeeEntity =await _repository.Employee.GetEmployee(companyId, id, trackChanges:true);
            if (employeeEntity == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);
            patchDoc.ApplyTo(employeeToPatch,ModelState);
            TryValidateModel(employeeToPatch);
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }
            _mapper.Map(employeeToPatch, employeeEntity);

            await _repository.Save();

            return NoContent();
        }
    }
}
