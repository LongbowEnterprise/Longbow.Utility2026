// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Longbow.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 文本日志配置服务注入方法扩展类
/// </summary>
public static class FileLoggerFactoryExtensions
{
    /// <summary>
    /// 添加文件日志功能到服务容器中
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure"></param>
    public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, Action<FileLoggerOptions>? configure = null)
    {
        builder.Services.AddOptions();
        builder.Services.AddSingleton<IOptionsChangeTokenSource<FileLoggerOptions>, ConfigurationChangeTokenSource<FileLoggerOptions>>();
        builder.Services.AddSingleton<IConfigureOptions<FileLoggerOptions>, ConfigureOptions<FileLoggerOptions>>();

        builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
        builder.Services.Configure<FileLoggerOptions>(op =>
        {
            op.FileName = GetOSPlatformPath(op.FileName);
            configure?.Invoke(op);
        });
        return builder;
    }

    static string GetOSPlatformPath(string originalString)
    {
        var sp = Path.DirectorySeparatorChar;
        var win = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        return win ? originalString.Replace('/', sp) : originalString.Replace('\\', sp);
    }
}
