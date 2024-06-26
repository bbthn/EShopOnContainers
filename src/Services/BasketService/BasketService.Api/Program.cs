using BasketService.Api.Extensions;
using BasketService.Api.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.ConfigureAppConfiguration(i => i.AddConfiguration(ConfigurationSetting.configuration));
builder.Services.AppServiceRegister(builder.Configuration);

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

app.AddSubscriptions();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
 
app.MapControllers();

app.Run();


