:: 1. 创建目标文件夹并进入
md OpenAPISample && cd OpenAPISample

:: 2. 创建 src 文件夹
md src

:: 3. 创建 Minimal API 项目（.NET 9）
dotnet new webapi -minimal -n MyApi --use-program-main false --no-https -f net9.0 -o src\MyApi

:: 4. 创建 solution 并把项目加进去
dotnet new sln -n OpenAPISample
dotnet sln OpenAPISample.sln add src\MyApi\MyApi.csproj

:: 5. 完成！现在目录结构如下：
:: OpenAPISample\
::   ├── OpenAPISample.sln
::   ├── src\
::   │   └── MyApi\
::   │       ├── Program.cs
::   │       ├── MyApi.csproj
::   │       └── ...
::   └── tests\          （可选）
::       └── MyApi.Tests\

:: 6. 直接运行项目看看
cd src\MyApi
dotnet run