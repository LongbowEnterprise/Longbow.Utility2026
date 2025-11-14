# Longbow.Logging

简体中文 README — Longbow.Logging 是一个轻量、可扩展的 .NET 日志库/封装，旨在提供一致的日志接口、结构化日志支持与常用扩展，方便在控制台、文件。

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

### 注入服务
```csharp
services.AddLogging(logging => logging.AddFileLogger());
```

## 配置示例（appsettings.json）
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    },
    "LgbFile": {
      "IncludeScopes": true,
      "LogLevel": {
        "Default": "Error"
      },
      "FileName": "Error\\Log.log"
    }
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
