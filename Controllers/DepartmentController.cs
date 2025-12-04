using AutoMapper;
using CompanyEmployees.Contract;
using CompanyEmployees.DTOObject;
using CompanyEmployees.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        public DepartmentController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
         _repository = repository;
           _logger =   logger;
            _mapper = mapper;
        }
        [HttpGet]
        
        public IActionResult getDepartments()
        {
         // throw  new Exception("Error Finding Departments");
            var departmentDb =  _repository.Department.GetDepartments(trackChanges: false);
          var departmentDto =  _mapper.Map<IEnumerable<DeparmentDto>>(departmentDb);
            return Ok(departmentDto);
        }
    }
}
