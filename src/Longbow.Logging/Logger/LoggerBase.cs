// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Text;

namespace Longbow.Logging;

/// <summary>
/// 
/// </summary>
/// <param name="name"></param>
/// <param name="filter"></param>
/// <param name="scopeProvider"></param>
/// <param name="config"></param>
public abstract class LoggerBase(string name, Func<string, LogLevel, bool>? filter, IExternalScopeProvider? scopeProvider, IConfiguration? config) : ILogger
{
    /// <summary>
    /// 
    /// </summary>
    protected string LogName { get; } = name;

    /// <summary>
    /// 
    /// </summary>
    protected Func<string, LogLevel, bool> Filter { get; } = filter ?? ((category, logLevel) => true);

    /// <summary>
    /// 判断当前 logLevel 是否开启日志
    /// </summary>
    /// <param name="logLevel">LogLevel 实例</param>
    /// <returns>开启日志时返回真</returns>
    public virtual bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None && Filter(LogName, logLevel);

    /// <summary>
    /// 获得/设置 IConfiguration 实例
    /// </summary>
    public IConfiguration? Configuration { get; set; } = config;

    /// <summary>
    /// 获得/设置 IExternalScopeProvider 实例
    /// </summary>
    public IExternalScopeProvider? ScopeProvider { get; set; } = scopeProvider;

    /// <summary>
    /// 开始创建日志上下文作用域
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state"></param>
    /// <returns></returns>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => ScopeProvider?.Push(state);

    /// <summary>
    /// 写日志方法
    /// </summary>
    /// <typeparam name="TState">泛型类型</typeparam>
    /// <param name="logLevel">LogLevel 实例</param>
    /// <param name="eventId">eventId 实例</param>
    /// <param name="state">TState 实例</param>
    /// <param name="exception">Exception 实例</param>
    /// <param name="formatter">格式化方法回调函数</param>
    public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (IsEnabled(logLevel))
        {
            var message = formatter.Invoke(state, exception);
            if (exception != null)
            {
                var nv = GetEnvironmentInformation(state);
                if (!string.IsNullOrEmpty(message))
                {
                    nv["Message"] = message;
                }
                message = exception.FormatException(nv);
            }
            if (!string.IsNullOrEmpty(message))
            {
                WriteMessage(logLevel, eventId.Id, message);
            }
        }
    }

    private void GetScopeInformation(StringBuilder stringBuilder, string messagePadding)
    {
        if (ScopeProvider != null)
        {
            var initialLength = stringBuilder.Length;

            ScopeProvider.ForEachScope((scope, state) =>
            {
                var (builder, length) = state;
                var first = length == builder.Length;
                builder.Append(first ? "=> " : " => ").Append(scope);
            }, (stringBuilder, initialLength));

            if (stringBuilder.Length > initialLength)
            {
                stringBuilder.Insert(initialLength, messagePadding);
                stringBuilder.AppendLine();
            }
        }
    }

    /// <summary>
    /// 写入日志
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="eventId"></param>
    /// <param name="message"></param>
    protected virtual void WriteMessage(LogLevel logLevel, int eventId, string? message)
    {
        var _logBuilder = new StringBuilder();
        var messagePadding = string.Empty;

        // 根据配置是否输出 Scope 信息
        if (ScopeProvider != null)
        {
            var logLevelString = GetLogLevelString(logLevel);

            // Example:
            // INFO: ConsoleApp.Program[10]
            //       Request received

            // category and event id
            _logBuilder.AppendFormat(string.Format("{0}: {1}[{2}]", logLevelString, LogName, eventId));
            _logBuilder.AppendLine();

            // scope information
            messagePadding = new string(' ', logLevelString.Length + 2);

            GetScopeInformation(_logBuilder, messagePadding);
        }

        if (!string.IsNullOrEmpty(message))
        {
            // add message padding
            if (ScopeProvider != null)
            {
                message = $"{messagePadding}{message}".Replace(Environment.NewLine, $"{Environment.NewLine}{messagePadding}");
            }

            // write message
            _logBuilder.AppendLine(message);
        }

        if (_logBuilder.Length > 0)
        {
            WriteMessageCore(_logBuilder.ToString());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    protected abstract void WriteMessageCore(string content);

    /// <summary>
    /// 获得环境变量信息
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state"></param>
    /// <returns></returns>
    protected virtual NameValueCollection GetEnvironmentInformation<TState>(TState state)
    {
        if (state is not NameValueCollection nv)
        {
            nv = [];
        }

        // 收集环境变量信息
        nv["OS"] = RuntimeInformation.OSDescription;
        nv["Framework"] = RuntimeInformation.FrameworkDescription;

        // 当前用户
        var userName = Configuration?.GetUserName();
        if (!string.IsNullOrEmpty(userName))
        {
            nv["UserName"] = userName;
        }

        // 当前环境
        var env = Configuration?.GetEnvironmentName();
        if (!string.IsNullOrEmpty(env))
        {
            nv["EnvironmentName"] = env;
        }

        // IIS Root 路径
        var iis = Configuration?.GetIISPath();
        if (!string.IsNullOrEmpty(iis))
        {
            nv["IISRootPath"] = iis;
        }

        // VisualStudio Version
        var vs = Configuration?.GetVisualStudioVersion();
        if (!string.IsNullOrEmpty(vs))
        {
            nv["VSIDE"] = vs;
        }

        return nv;
    }

    private static string GetLogLevelString(LogLevel logLevel) => logLevel switch
    {
        LogLevel.Trace => "trce",
        LogLevel.Debug => "dbug",
        LogLevel.Information => "info",
        LogLevel.Warning => "warn",
        LogLevel.Error => "fail",
        _ => "crit",
    };
}
