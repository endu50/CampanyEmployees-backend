using CompanyEmployees.Contract;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly IRepositoryManager _repository;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ILoggerManager _logger1;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ILoggerManager logger1, IRepositoryManager repository)
        {
            _logger = logger;
            _logger1 = logger1;
            _repository= repository;
        }


        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("Getting weather at {Now}", DateTime.UtcNow);
            _logger1.LogInfo("Here is the log on Endris ");
            _logger1.LogInfo("TEST_LOGGERMANAGER: Weather endpoint hit at " + DateTime.UtcNow);

           // _repository.Company.Equals("");
            //_repository.Employee.AnyMethodFromEmployeeRepository();
           // return new string[] { "value1", "value2" };

            try

            {
                // ...
                return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
            .ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get weather for user {UserId}", User?.Identity?.Name ?? "anon");
                _logger1.LogError(ex+ "Failed to get weather for user");
                throw;
            }

                       


        }
    }
}
