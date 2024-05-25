using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();


// Add services to the container.
builder.Services.AddOcelot().AddConsul();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Containerlar aya?a kalkarken f?rlat?lan hatalar? önlemek için.
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

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseOcelot().Wait();
app.MapControllers();

app.Run();
