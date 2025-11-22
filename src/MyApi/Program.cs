// Program.cs 顶部加入
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApi", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/hello", () => Results.Ok(new { message = "Hello from Minimal API!" }))
   .WithName("Hello")
   .WithOpenApi()
   .WithTags("Demo");

// 关键：正确生成 openapi.json 到 ../docs
app.Lifetime.ApplicationStarted.Register(async () =>
{
    await Task.Yield();

    // 正确获取 SwaggerGenerator（这是 Swashbuckle 内部的实现）
    var swaggerGenerator = app.Services.GetRequiredService<ISwaggerProvider>();
    var swaggerDoc = swaggerGenerator.GetSwagger("v1");

    var docsDir = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "docs");
    Directory.CreateDirectory(docsDir);
    var filePath = Path.Combine(docsDir, "openapi.json");

    await using var stream = File.Create(filePath);
    await using var writer = new StreamWriter(stream);
    swaggerDoc.SerializeAsV3(new Microsoft.OpenApi.Writers.OpenApiJsonWriter(writer));

    Console.WriteLine($"OpenAPI 已生成: {Path.GetFullPath(filePath)}");
});

app.Run();