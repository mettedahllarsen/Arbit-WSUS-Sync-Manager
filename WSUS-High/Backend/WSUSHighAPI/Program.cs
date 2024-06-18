using WSUSHighAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

var MyCors = "_myCors";

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyCors,
                              policy =>
                              {
                                  policy.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader();
                              });
});

builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Singleton
builder.Services.AddSingleton<ComputersRepository>(new ComputersRepository());

var app = builder.Build();

// Use Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.

app.UseCors(MyCors);

app.UseAuthorization();

app.MapControllers();

app.Run();
