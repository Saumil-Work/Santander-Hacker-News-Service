using Santander_Hacker_News_Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddLogging();

// Configure HttpClient for Hacker News API
builder.Services.AddHttpClient<IHackerNewsService, HackerNewsService>()
    .ConfigureHttpClient(client =>
    {
        client.DefaultRequestHeaders.Add("User-Agent", "Santander-Hacker-News-Service");
        client.Timeout = TimeSpan.FromSeconds(30);
    });

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
