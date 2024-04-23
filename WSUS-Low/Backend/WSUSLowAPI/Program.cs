using Microsoft.EntityFrameworkCore;
using WSUSLowAPI.Contexts;
using WSUSLowAPI.Models;
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

var optionsbuilder = new DbContextOptionsBuilder<WSUSDbContext>();
optionsbuilder.UseSqlServer(builder.Configuration["ConnectionStrings:OliverConnection"]);
WSUSDbContext context = new WSUSDbContext(optionsbuilder.Options);
builder.Services.AddSingleton<IUpdateDataRepository>( new UpdateDataRepositoryDb(context));

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowAll");

app.Run();
