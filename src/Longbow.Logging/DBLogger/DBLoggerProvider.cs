// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.Logging;
using System.Collections.Specialized;

namespace Longbow.Logging;

/// <summary>
/// ILoggerProvider 数据库日志提供者实现类
/// </summary>
/// <remarks>
/// 默认构造函数
/// </remarks>
/// <param name="provider">IServiceProvider 实例</param>
/// <param name="action">异常记录操作回调函数</param>
public class DBLoggerProvider(IServiceProvider provider, Action<IServiceProvider, EventId, Exception?, NameValueCollection> action) : ILoggerProvider
{
    private readonly Action<IServiceProvider, EventId, Exception?, NameValueCollection> _action = action;
    private readonly IServiceProvider _provider = provider;

    /// <summary>
    /// 创建 ILogger 实例
    /// </summary>
    /// <param name="categoryName">分类名称</param>
    /// <returns>ILogger 实例</returns>
    public ILogger CreateLogger(string categoryName)
    {
        return new DBLogger(_provider, _action);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
    }

    // This code added to correctly implement the disposable pattern.
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
