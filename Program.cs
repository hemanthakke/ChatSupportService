using Microsoft.OpenApi;
using ChatSupportService.Services.BackgroundServices;
using ChatSupportService.Services.Implementation;
using ChatSupportService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swgagger Services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Chat Support Service API",
        Version = "v1",
        Description = "API for managing chat support operations, including agent management, chat sessions, and message handling."
    });
});

// Register application services
builder.Services.AddSingleton<IChatQueue>(sp => new InMemoryChatQueue()); // Main Queue
builder.Services.AddSingleton<IChatQueue>(sp => new InMemoryChatQueue()); // Overflow Queue
builder.Services.AddSingleton<ITeamManagementService, TeamManagementService>();
builder.Services.AddSingleton<ITimeProviderService, SystemTimeProviderService>();
builder.Services.AddSingleton<IOfficeHoursService, OfficeHoursService>();
builder.Services.AddSingleton<IChatRequestService, ChatRequestService>();

// Register background services
builder.Services.AddHostedService<QueueMoniterService>();
builder.Services.AddHostedService<ChatAssignmentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chat Support Service API v1");
        c.RoutePrefix = "swagger"; // Access at http://localhost:57555/swagger
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();