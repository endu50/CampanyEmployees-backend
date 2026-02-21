using System;
using System.Threading.Tasks;
using AutoMapper;
using CompanyEmployees.ActionFilters;
using CompanyEmployees.Contract;
using CompanyEmployees.DTOObject;
using CompanyEmployees.ModelBinders;
using CompanyEmployees.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;


namespace CompanyEmployees.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
  //  [ResponseCache(CacheProfileName = "120SecondsDuration")]
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
        //   [ResponseCache(Duration = 60)]

        //[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        //[HttpCacheValidation(MustRevalidate = false)]
        [ResponseCache( Duration = 60,Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task <IActionResult> GetCompany(Guid id)
        {
            var company =await _repository.Company.GetCompany(id, trackChanges: false);
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


        [HttpGet(Name = "Companies"), Authorize(Roles = "Manager")]
      
        public async Task<IActionResult>GetCompanies()
        {

            try
            {

                var companies = await _repository.Company.GetAllCompanies(trackChanges: false);

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
        [ServiceFilter(typeof(ValidationFilterAttribute))] 
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto companyDto)
        {
            //if (companyDto == null)
            //{
            //    _logger.LogError("Camapany Not Found Null");
            //    return BadRequest("Empty Data");
            //}
            var response = _mapper.Map<Company>(companyDto);
            _repository.Company.CreateCompany(response);
           await _repository.Save();

            // _logger.LogInfo("Campany Created Successfully!");
            //  return Ok("Campany Created Successfully!");

            var companyToReturn = _mapper.Map<CompanyDto>(response);
            return CreatedAtRoute("CompanyById", new { id = companyToReturn.Id },
            companyToReturn);



        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task< IActionResult> GetCompanyCollection([ModelBinder(BinderType =
typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");

            }
            var companyEntities =await _repository.Company.GetByIds(ids, trackChanges: false);
            if (ids.Count() != companyEntities.Count())
            {
                _logger.LogError("Some ids are not valid in a collection");
                return NotFound();
            }
            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            return Ok(companiesToReturn);
        }
        [HttpPost("collection")]
        public async Task< IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection == null)
            {
                _logger.LogError("Company collection sent from client is null.");
                return BadRequest("Company collection is null");
            }
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }
          await  _repository.Save();
            var companyCollectionToReturn =
            _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("CompanyCollection", new { ids },
            companyCollectionToReturn);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        public async Task<ActionResult> DeleteCampany(Guid id)
        {
            //var campany = await _repository.Company.GetCompany(campanyId, trackChanges: false);
            //if(campany == null)
            //{
            //    _logger.LogError("No campany id Found");
            //    return NotFound("No Campany Id Found ");
            //}
            var company = HttpContext.Items["company"] as Company;
            _repository.Company.DeleteCampany(company);
           await _repository.Save();
            // return Ok("The Campany Id Delete Successfully");
            return NoContent();

        }
        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))] //1
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))] //2
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CampanyForUpdateDto company)
        {
            //if (company == null)  //1
            //{
            //    _logger.LogError("CompanyForUpdateDto object sent from client is null.");
            //    return BadRequest("CompanyForUpdateDto object is null");
            //}

            //var companyEntity =await _repository.Company.GetCompany(id, trackChanges: true);
            //if (companyEntity == null)  //2
            //{
            //    _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
            //    return NotFound();
            //}
            var companyentity = HttpContext.Items["company"] as Company;

            _mapper.Map(company, companyentity);
            await _repository.Save();

            return NoContent();
        }
        [HttpPatch("{id}")]
        public async Task <ActionResult> CampanyPatch(Guid id, [FromBody]
        JsonPatchDocument<CampanyForUpdateDto> jsonPatch)
        {
            if(jsonPatch == null)
            {
                _logger.LogError("No Client Data For Update");
                return NotFound();
            }
            var campany =await _repository.Company.GetCompany(id, trackChanges: true);
            if(campany == null)
            {
                _logger.LogError("No CampanyId found for Update");
                return NotFound();
            }

            var pathvalue= _mapper.Map<CampanyForUpdateDto>(campany);
            jsonPatch.ApplyTo(pathvalue);
            _mapper.Map(pathvalue, campany);
           await _repository.Save();
            return NoContent();


        }

       
    }
}
