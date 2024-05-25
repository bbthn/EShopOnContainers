using OrderService.Api.Extensions;
using OrderService.Api.Extensions.Registration;
using OrderService.Api.Extensions.Registration.ServiceDiscovery;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureRegistration(builder.Configuration);
builder.Host.UseDefaultServiceProvider(configure =>
{
    configure.ValidateOnBuild = false;
    configure.ValidateScopes = false;
});

var app = builder.Build();



app.MigrateDbContext<OrderDbContext>((context, services) =>
{
    var logger = services.GetService<ILogger<OrderDbContext>>();

    new OrderDbContextSeed()
        .SeedAsync(context, logger)
        .Wait();
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.RegisterWithConsul(app.Lifetime,app.Configuration);
app.ConfigureSubscription();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
