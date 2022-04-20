using MassTransit;
using MassTransit_Saga_AzureSB;
using MassTransit_Saga_AzureSB.Data;
using MassTransit_Saga_AzureSB.Saga;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<DbContext, StateDbContext>()
                .AddScoped<SqlServerStateInstance, MyStateInstance>();
var connectionString = builder.Configuration.GetConnectionString("StateDB");
builder.Services.RegisterStateMachine<MySagaStateMachine, MyStateInstance, DbContext, StateDbContext>(connectionString, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();


app.MapGet("/getstate", async (IRequestClient<IBaseEventData> _requestClient, Guid correlationId) =>
{
    var response = await _requestClient.GetResponse<CurrentStateMessage>(new
    {
        CorrelationId = correlationId
    });
    return response.Message;
})
.WithName("GetCurrentState");

app.MapPost("/state-collectemail", async (IRequestClient<CollectEmailData> _requestClient, Guid correlationId) =>
{
    var sagaResponse = await _requestClient.GetResponse<CurrentStateMessage>(new
    {
        CorrelationId = correlationId,
        ActiveAccountExists = false
    });
    return sagaResponse.Message;

}).WithName("UpdateState");

//app.ApplyMigrations();

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}