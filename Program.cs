using CompanyEmployees;
using CompanyEmployees.Contract;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Configure logging levels (appsettings.json can override)
builder.Logging.ClearProviders(); // optional: remove default providers
builder.Logging.AddConsole();
builder.Logging.AddDebug(); // useful during development
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);

builder.Services.AddControllers();
//builder.Services.ConfigureRepositoryManager();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddSingleton<ILoggerManager, LoggerManager>();
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<globalExceptionHandler>();

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
// IMPORTANT: enable CORS early (before routing/authorization/controllers)
app.UseCors("AllowAngularFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
  
}
app.UseExceptionHandler( _ => { });


app.UseAuthorization();

app.MapControllers();

app.Run();
