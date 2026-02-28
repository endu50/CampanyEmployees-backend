using System.Data;
using AutoMapper;
using CompanyEmployees.Contract;
using CompanyEmployees.DTOObject;
using CompanyEmployees.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NLog.Targets.Wrappers;

namespace CompanyEmployees.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly RepositoryContext _context;
        public DepartmentController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper,
            RepositoryContext context)
        {
         _repository = repository;
           _logger =   logger;
            _mapper = mapper;
            _context = context;
        }
        [HttpGet, Authorize(Roles = "Manager")]
        
        public IActionResult getDepartments()
        {
         // throw  new Exception("Error Finding Departments");
            var departmentDb =  _repository.Department.GetDepartments(trackChanges: false);
            if (!departmentDb.Any())
            {
                _logger.LogError("Error Finding Departments Or Null");
                throw new Exception("Error Finding Departments Or Null");
                
            }
          var departmentDto =  _mapper.Map<IEnumerable<DeparmentDto>>(departmentDb);
            return Ok(departmentDto);
        }

     [HttpGet("{deptId}", Name = "deptId")]
        public IActionResult GetDepartmentById(int deptId)
        {
            var deptRep = _repository.Department.GetDepartmentById(deptId, trackChanges: false);
            
            var dept = _mapper.Map<DeparmentDto>(deptRep);
            return Ok(dept);

        }
        [HttpPost("department")]
        public ActionResult CreateDepartment([FromBody] DepartmentForCreation departmentdto)
        {
            var department = new Department
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
        public IActionResult CreateDepartmentOne([FromBody] DepartmentForCreation departmentForCreation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (departmentForCreation == null)
            {
                _logger.LogError("Error-Empty Request From User");
                 throw new Exception("Error-Empty Request From User");
            }
            var department = _mapper.Map<Department>(departmentForCreation);
            _repository.Department.CreateDepartment(department);
            _repository.Save();
           // return Ok("Department Inserted Successfuly!");
            var departmentDto = _mapper.Map<DeparmentDto>(department);
          return  CreatedAtRoute("deptId", new { deptId = departmentDto.DeptId}, departmentDto);
        }

        [HttpDelete("deptId")]
        public ActionResult DeleteDepartment(int deptId)
        {
            var department = _context.Departments.FirstOrDefault(c=> c.DeptId.Equals(deptId));
            if (department == null)
            {
                _logger.LogError("No Id Found to Delete");
                return NotFound();
            }
            _context.Departments.Remove(department);
            _context.SaveChanges();
            return Ok("The Department Id Deleted Successfully");
        }

        [HttpPut("deptId")]
        public ActionResult UpdateDepartment(int deptId, DepartmrntForUpdateDto dto)
        {
            if (dto == null)
            {
                _logger.LogError("No Data Found For Update from Client");
                return NotFound("No Data Found For Update from Client");
            }
            var department = _context.Departments.Find(deptId);
            if (department == null)
            {
                _logger.LogError("Id Not Found For Update");
                return NotFound("Id Not Found For Update");
            }
            department.Name = dto.Name;
            department.Description = dto.Description;
            _context.Update(department);
            _context.SaveChanges();
            return Ok("Department Id Updated Successfully");

        }
        [HttpPut("id")]
        public ActionResult UpdateDept(int id , DepartmrntForUpdateDto dto)
        {

            if (dto == null)
            {
                _logger.LogError("No Data Found For Update from Client");
                return NotFound("No Data Found For Update from Client");
            }
            var department = _repository.Department.GetDepartmentById(id, trackChanges: true);
            _mapper.Map(dto, department);
            _repository.Save();
            return Ok("Department Id Updated Successfully");

        }
    }
}
