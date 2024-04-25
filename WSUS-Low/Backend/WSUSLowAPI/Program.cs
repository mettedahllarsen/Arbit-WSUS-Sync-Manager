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

bool useSql = true;
if (useSql)
{
    builder.Services.AddDbContext<WSUSDbContext>(options =>
        options.UseSqlServer(builder.Configuration["ConnectionStrings:MikkelConnection"]));
    builder.Services.AddScoped<IUpdateMetadataRepository, UpdateMetadataRepositoryDb>();

}
else
{
    //builder.Services.AddSingleton<IUpdateMetadataRepository>(new UpdateMetadataRepository());
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
