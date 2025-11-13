# Longbow.Logging

简体中文 README — Longbow.Logging 是一个轻量、可扩展的 .NET 日志库/封装，旨在提供一致的日志接口、结构化日志支持与常用扩展，方便在控制台、文件及第三方接入（如 Seq / ELK / Azure Monitor）中使用。

## 主要特性
- 与 Microsoft.Extensions.Logging 兼容的适配器
- 结构化日志（模板化消息 + 属性）
- 简易配置（appsettings.json / DI）
- 常见目标（Console、File）示例与扩展点
- 异常捕获与友好输出格式

## 快速开始

### 安装
```powershell
dotnet add package Longbow.Logging
```

### 基本用法（自包含）
```csharp
using Longbow.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();
services.AddLongbowLogging(); // 假设库提供的扩展方法
var provider = services.BuildServiceProvider();

var logger = provider.GetRequiredService<ILogger<Program>>();
logger.LogInformation("应用启动，版本：{Version}", "1.0.0");
try
{
    throw new InvalidOperationException("示例异常");
}
catch (Exception ex)
{
    logger.LogError(ex, "发生异常：{Message}", ex.Message);
}
```

### 在 ASP.NET Core 中集成
在 Program.cs / Startup.cs 中：
```csharp
builder.Host.ConfigureLogging((context, logging) =>
{
    logging.ClearProviders();
    logging.AddLongbowLogging(context.Configuration); // 使用配置初始化
});
```

## 配置示例（appsettings.json）
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "LongbowLogging": {
    "EnableConsole": true,
    "EnableFile": true,
    "FilePath": "logs/app.log",
    "MinimumLevel": "Information"
  }
}
```

## 贡献
欢迎提交 issue 与 pull request。请遵循以下流程：
1. Fork 仓库并在 feature 分支开发
2. 添加测试并通过现有测试
3. 提交 PR 描述变更与目的

## 许可证
MIT（若实际项目使用不同许可证，请替换为实际许可证）

## 联系
如需帮助或报告问题，请在仓库中打开 Issue。

备注：请根据项目实际实现、命名与配置键调整示例代码与配置项。
