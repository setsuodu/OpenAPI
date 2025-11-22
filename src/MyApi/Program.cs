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
    await Task.Delay(100);
    var doc = app.Services.GetRequiredService<ISwaggerProvider>().GetSwagger("v1");
    var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "docs", "openapi.json");
    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    //await File.WriteAllTextAsync(path, doc.SerializeAsV3AsJson(null));
    await using var stream = File.Create(path);
    await using var writer = new StreamWriter(stream);
    doc.SerializeAsV3(new Microsoft.OpenApi.Writers.OpenApiJsonWriter(writer));
    Console.WriteLine($"OpenAPI 已生成: {Path.GetFullPath(path)}");
});

app.Run();