// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Longbow.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Specialized;
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
        builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
        builder.Services.AddSingleton<IConfigureOptions<FileLoggerOptions>, LoggerProviderConfigureOptions<FileLoggerOptions, FileLoggerProvider>>();
        builder.Services.AddSingleton<IOptionsChangeTokenSource<FileLoggerOptions>, LoggerProviderOptionsChangeTokenSource<FileLoggerOptions, FileLoggerProvider>>();

        builder.Services.Configure<FileLoggerOptions>(op =>
        {
            op.FileName = GetOSPlatformPath(op.FileName);
            configure?.Invoke(op);
        });
        return builder;
    }

    /// <summary>
    /// 添加文件日志功能到服务容器中
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="action">日志操作类回调方法，使用者在此回调中将异常记录到数据库中</param>
    public static ILoggingBuilder AddDBLogger(this ILoggingBuilder builder, Action<IServiceProvider, EventId, Exception?, NameValueCollection> action)
    {
        builder.Services.AddSingleton<ILoggerProvider>(provider => new DBLoggerProvider(provider, action));
        return builder;
    }

    static string GetOSPlatformPath(string originalString)
    {
        var sp = Path.DirectorySeparatorChar;
        var win = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        return win ? originalString.Replace('/', sp) : originalString.Replace('\\', sp);
    }
}
