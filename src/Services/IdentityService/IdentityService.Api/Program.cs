using IdentityService.Api.Application;
using IdentityService.Api.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IIdentityService,IdentityService.Api.Application.Services.IdentityService>();
builder.Services.ConfigureConsul(builder.Configuration);

builder.Host.UseDefaultServiceProvider(configure =>
{
    configure.ValidateOnBuild = false;
    configure.ValidateScopes = false;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.RegisterWithConsul(app.Lifetime,app.Configuration);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
