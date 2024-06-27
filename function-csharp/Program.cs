using GrpcService.Services;

var builder = WebApplication.CreateSlimBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<RunFunctionService>();

app.Run();
