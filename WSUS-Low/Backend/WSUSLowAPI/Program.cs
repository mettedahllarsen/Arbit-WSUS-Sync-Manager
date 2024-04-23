using Microsoft.EntityFrameworkCore;
using WSUSLowAPI.Contexts;
using WSUSLowAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

bool useSql = false;
if (useSql)
{
    var optionsbuilder = new DbContextOptionsBuilder<WSUSDbContext>();
    optionsbuilder.UseSqlServer(builder.Configuration["ConnectionStrings:OliverConnection"]);
    WSUSDbContext context = new WSUSDbContext(optionsbuilder.Options);
    builder.Services.AddSingleton<IUpdateDataRepository>(new UpdateDataRepositoryDb(context));
}
else
{
    builder.Services.AddSingleton<IUpdateDataRepository>(new UpdateDataRepository());
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowAll");

app.Run();
