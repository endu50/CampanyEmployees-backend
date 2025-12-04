using System;
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
    public class CompaniesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompaniesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }


        //[HttpGet(Name = "set")]
        //public IActionResult GetAll()
        //{
        //    throw new Exception("Test Exception");
        //    return Ok(new[] { 1, 2, 3 });


        //}
        [HttpGet("{id}", Name = "CompanyById")]
        public IActionResult GetCompany(Guid id)
        {
            var company = _repository.Company.GetCompany(id, trackChanges: false);
            if (company == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            else
            {
                var companyDto = _mapper.Map<CompanyDto>(company);
                return Ok(companyDto);
            }
        }
       

        [HttpGet(Name = "Companies")]
        public IActionResult GetCompanies()
        {

            try
            {

                var companies = _repository.Company.GetAllCompanies(trackChanges: false);

                var companyDto1 = companies.Select(c => new CompanyDto
                {

                    Id = c.Id,
                    Name = c.Name,
                    FullAddress = string.Join(' ', c.Address, c.Country)
                }).ToList();
                return Ok(companyDto1);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong in the {nameof(GetCompanies)} action {ex} ");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost]

        public IActionResult CreateCompany([FromBody] CompanyForCreationDto companyDto)
        {
            if (companyDto == null)
            {
                _logger.LogError("Camapany Not Found Null");
                return BadRequest("Empty Data");
            }
            var response = _mapper.Map<Company>(companyDto);
            _repository.Company.CreateCompany(response);
            _repository.Save();

           // _logger.LogInfo("Campany Created Successfully!");
          //  return Ok("Campany Created Successfully!");

            var companyToReturn = _mapper.Map<CompanyDto>(response);
            return CreatedAtRoute("CompanyById", new { id = companyToReturn.Id },
            companyToReturn);



        }
    }
}
