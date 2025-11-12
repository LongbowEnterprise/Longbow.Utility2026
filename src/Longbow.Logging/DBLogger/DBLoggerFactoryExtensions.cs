// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Longbow.Logging;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 数据库日志配置服务注入方法扩展类
/// </summary>
public static class DBLoggerFactoryExtensions
{
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
}
