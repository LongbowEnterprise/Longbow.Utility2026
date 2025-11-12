// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Longbow.Logging;

/// <summary>
/// 日志文件操作类，内部使用单件模式，所以只能记录在一个文件中
/// </summary>
[ProviderAlias("LgbFile")]
public class FileLoggerProvider : LoggerProvider
{
    private readonly IDisposable? _optionsReloadToken;
    private readonly IConfiguration? _config;
    private FileLoggerOptions _options;

    /// <summary>
    /// 
    /// </summary>
    protected FileLoggerWriter _writer;

    /// <summary>
    /// 默认构造函数
    /// </summary>
    /// <param name="options">IFileLoggerOptions 实例</param>
    /// <param name="filter">日志过滤回调函数</param>
    public FileLoggerProvider(FileLoggerOptions options, Func<string, LogLevel, bool>? filter = null) : base(filter)
    {
        _options = options;
        _writer = new FileLoggerWriter(_options);
    }

    /// <summary>
    /// 通过注入方式监听配置文件初始化 FileProvider，此构造函数被 IoC 调用
    /// </summary>
    /// <param name="optionsMonitor"></param>
    /// <param name="configuration"></param>
    public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> optionsMonitor, IConfiguration configuration) : this(optionsMonitor.CurrentValue)
    {
        _config = configuration;
        _optionsReloadToken = optionsMonitor.OnChange(op =>
        {
            _options = op;
            _writer.Dispose();
            _writer = new FileLoggerWriter(_options);
        });
    }

    /// <summary>
    /// 创建 ILogger 实例方法
    /// </summary>
    /// <param name="categoryName">分类名称</param>
    /// <returns>ILogger 实例</returns>
    public override ILogger CreateLogger(string categoryName)
    {
        var scopeProvider = _options.IncludeScopes ? new LoggerExternalScopeProvider() : null;
        return new FileLogger(categoryName, Filter, scopeProvider, _config, _writer.WriteMessage);
    }

    /// <summary>
    /// Dispose 方法
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _optionsReloadToken?.Dispose();
            _writer?.Dispose();
        }
    }
}
