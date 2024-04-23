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
    optionsbuilder.UseSqlServer(builder.Configuration["ConnectionStrings:MikkelConnection"]);
    WSUSDbContext context = new(optionsbuilder.Options);
    builder.Services.AddSingleton<IUpdateMetadataRepository>(new UpdateMetadataRepositoryDb(context));
}
else
{
    builder.Services.AddSingleton<IUpdateMetadataRepository>(new UpdateMetadataRepository());
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
