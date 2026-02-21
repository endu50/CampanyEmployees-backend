using CompanyEmployees;
using CompanyEmployees.ActionFilters;
using CompanyEmployees.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using AspNetCoreRateLimit;






var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Configure logging levels (appsettings.json can override)
builder.Logging.ClearProviders(); // optional: remove default providers
builder.Logging.AddConsole();
builder.Logging.AddDebug(); // useful during development
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);

builder.Services.AddControllers(options =>
{

    options.RespectBrowserAcceptHeader = true;
    options.ReturnHttpNotAcceptable = true;
    options.CacheProfiles.Add("120SecondsDuration", new CacheProfile
    {
        Duration = 120
    });
})
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

//builder.Services.ConfigureRepositoryManager();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddScoped<ValidateCompanyExistsAttribute>();
builder.Services.AddScoped<ValidateEmployeeForCompanyExistsAttribute>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<globalExceptionHandler>();
//builder.Services.AddProblemDetails();
//builder.Services.AddControllers().AddNewtonsoftJson(); // enables JSON Patch (JsonPatchDocument)

builder.Services.ConfigureVersioning();
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();
builder.Services.AddMemoryCache();
builder.Services.AddInMemoryRateLimiting();
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);

builder.Services.AddScoped<IAuthenticationManager, AuthenticationManager>();

builder.Services.Configure<ApiBehaviorOptions>(options =>  // error when the ModelState is invalid
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddDbContext<RepositoryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b =>
b.MigrationsAssembly("CompanyEmployees"))); ;
//Add Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularFrontend",
        policy => policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod());
});


var app = builder.Build();

app.UseExceptionHandler(options => { });


// IMPORTANT: enable CORS early (before routing/authorization/controllers)
app.UseCors("AllowAngularFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  //  app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
  
}

app.UseResponseCaching();
app.UseHttpCacheHeaders();
app.UseIpRateLimiting();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
