var builder = WebApplication.CreateBuilder(args);

// 添加 OpenAPI/Swagger 支持
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 配置 Swagger JSON 路径（固定为 /openapi.json，方便 Pages 直接访问）
app.UseSwagger(c =>
{
    c.RouteTemplate = "openapi.json";
    c.SerializeAsV2 = false;
});

// 直接在根目录提供 Redoc（docs/index.html 内容见下文）
app.UseStaticFiles(); // 让 docs 文件夹可以被访问

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/openapi.json", "My API V1");
    // 可选：直接用 Swagger UI 访问 /swagger
});

var summaries = new[] { "Sunny", "Cloudy", "Rainy" };

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi()
.WithTags("Weather");

// 最简 Hello 示例
app.MapGet("/hello", () => "Hello from Minimal API!")
   .WithName("Hello")
   .WithOpenApi()
   .WithTags("Demo");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}