var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var MyCors = "_myCors";

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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.UseCors(MyCors);

app.Run();
