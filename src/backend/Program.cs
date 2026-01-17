using SimpleKsef.Infrastructure.Web;

var builder = WebApplication.CreateBuilder(args);
builder.AddApi();

var app = builder.Build();
app.UseApi();

app.Run();
