// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Longbow.Logging;

/// <summary>
/// 后台服务管理日志操作类
/// </summary>
/// <param name="filter"></param>
/// <param name="config"></param>
public class LoggerProvider(Func<string, LogLevel, bool>? filter = null, IConfiguration? config = null) : ILoggerProvider
{
    /// <summary>
    /// 获得/设置 日志过滤器
    /// </summary>
    protected Func<string, LogLevel, bool>? Filter { get; set; } = filter;

    /// <summary>
    /// 获得/设置  IConfiguration 实例
    /// </summary>
    protected IConfiguration? Configuration { get; set; } = config;

    /// <summary>
    /// 创建 ILogger 实例方法
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns></returns>
    public virtual ILogger CreateLogger(string categoryName) => new Logger(categoryName, Filter, null, Configuration);

    /// <summary>
    /// 创建 ILogger 实例方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public virtual ILogger CreateLogger<T>() => CreateLogger(typeof(T).Name);

    /// <summary>
    /// Dispose
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {

    }

    /// <summary>
    /// Dispose 方法
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
