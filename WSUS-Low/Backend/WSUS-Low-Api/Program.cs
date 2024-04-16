var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
const string policyName = "AllowAll";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: policyName,
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

// TODO: Connectionstring


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.UseCors(policyName);

app.MapControllers();

app.Run();
