// Program.cs 顶部加入
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection.Metadata;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApi", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // /swagger/v1/swagger.json
    // 关键：将 openapi.json 写到 wwwroot（供 GitHub Pages 使用）
    app.UseSwaggerUI();
    app.UseStaticFiles(); // 让 wwwroot 下的文件可以直接访问
}

//app.MapGet("/hello", () => Results.Ok(new { message = "Hello from Minimal API!" }))
//   .WithName("Hello")
//   .WithOpenApi()
//   .WithTags("Demo");

// 关键：正确生成 openapi.json 到 ../docs
app.Lifetime.ApplicationStarted.Register(async () =>
{
    await Task.Delay(100);
    var doc = app.Services.GetRequiredService<ISwaggerProvider>().GetSwagger("v1"); // 要和👆SwaggerDoc一致

    //var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "docs", "openapi.json");
    //Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    ////await File.WriteAllTextAsync(path, doc.SerializeAsV3AsJson(null));
    //await using var stream = File.Create(path);
    //await using var writer = new StreamWriter(stream);
    //doc.SerializeAsV3(new Microsoft.OpenApi.Writers.OpenApiJsonWriter(writer));
    //Console.WriteLine($"OpenAPI 已生成: {Path.GetFullPath(path)}");
    var outputPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "openapi.json");
    Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

    using var stream = File.OpenWrite(outputPath);
    await using var writer = new StreamWriter(stream);
    doc.SerializeAsV3(new Microsoft.OpenApi.Writers.OpenApiJsonWriter(writer));
    Console.WriteLine($"OpenAPI 已生成: {Path.GetFullPath(outputPath)}");
    //stream.Close();
});

app.MapGet("/", () => Results.Redirect("/redoc/index.html"));

app.Run();