// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.Logging;
using System.Collections.Specialized;

namespace Longbow.Logging;

/// <summary>
/// ILogger 数据库日志内部实现类
/// </summary>
/// <remarks>
/// 默认构造函数
/// </remarks>
/// <param name="provider">IServiceProvider 实例</param>
/// <param name="action">操作回调函数</param>
internal class DBLogger(IServiceProvider provider, Action<IServiceProvider, EventId, Exception?, NameValueCollection> action) : ILogger
{
    private readonly Action<IServiceProvider, EventId, Exception?, NameValueCollection> _action = action;
    private readonly IServiceProvider _provider = provider;

    /// <summary>
    /// 创建上下文作用域
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state"></param>
    /// <returns></returns>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    /// <summary>
    /// 判断当前 logLevel 是否开启日志
    /// </summary>
    /// <param name="logLevel">LogLevel 实例</param>
    /// <returns>开启日志时返回真</returns>
    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None && logLevel >= LogLevel.Error;

    /// <summary>
    /// 写日志方法
    /// </summary>
    /// <typeparam name="TState">泛型类型</typeparam>
    /// <param name="logLevel">LogLevel 实例</param>
    /// <param name="eventId">eventId 实例</param>
    /// <param name="state">TState 实例</param>
    /// <param name="exception">Exception 实例</param>
    /// <param name="formatter">格式化方法回调函数</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var nv = new NameValueCollection();
        //var accessor = _provider.GetService(typeof(IHttpContextAccessor));
        //if (accessor is IHttpContextAccessor httpContextAccessor)
        //{
        //    if (httpContextAccessor.HttpContext != null)
        //    {
        //        nv["ErrorPage"] = httpContextAccessor.HttpContext.Request.Path;
        //        nv["UserIp"] = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToIPv4String();
        //        nv["UserId"] = httpContextAccessor.HttpContext.User.Identity?.Name;
        //    }
        //}
        if (state is NameValueCollection v)
        {
            nv.Add(v);
        }

        _action?.Invoke(_provider, eventId, exception, nv);
    }
}
