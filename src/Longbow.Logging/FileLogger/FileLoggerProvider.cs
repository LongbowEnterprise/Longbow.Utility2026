// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Longbow.Logging;

/// <summary>
/// 日志文件操作类，内部使用单件模式，所以只能记录在一个文件中
/// </summary>
[ProviderAlias("LgbFile")]
public class FileLoggerProvider : LoggerProvider
{
    private static readonly string ProviderName = typeof(FileLoggerProvider).FullName!;
    private static readonly string? ProviderAlias = typeof(FileLoggerProvider).GetCustomAttribute<ProviderAliasAttribute>()?.Alias;

    private readonly IDisposable? _optionsReloadToken;
    private readonly IDisposable? _loggerFilterReloadToken;
    private readonly IConfiguration? _config;
    private readonly Func<string, LogLevel, bool>? _customFilter;
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
        _customFilter = filter;
        _options = options;
        _writer = new FileLoggerWriter(_options);
    }

    /// <summary>
    /// 通过注入方式监听配置文件初始化 FileProvider，此构造函数被 IoC 调用
    /// </summary>
    /// <param name="optionsMonitor"></param>
    /// <param name="configuration"></param>
    /// <param name="loggerFilterOptionsMonitor"></param>
    public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> optionsMonitor, IConfiguration configuration, IOptionsMonitor<LoggerFilterOptions> loggerFilterOptionsMonitor)
        : this(optionsMonitor.CurrentValue)
    {
        _config = configuration;
        UpdateFilter(loggerFilterOptionsMonitor.CurrentValue);

        _optionsReloadToken = optionsMonitor.OnChange(op =>
        {
            _options = op;
            _writer.Dispose();
            _writer = new FileLoggerWriter(_options);
        });

        _loggerFilterReloadToken = loggerFilterOptionsMonitor.OnChange(UpdateFilter);
    }

    /// <summary>
    /// 创建 ILogger 实例方法
    /// </summary>
    /// <param name="categoryName">分类名称</param>
    /// <returns>ILogger 实例</returns>
    public override ILogger CreateLogger(string categoryName)
    {
        var scopeProvider = _options.IncludeScopes ? new LoggerExternalScopeProvider() : null;
        return new FileLogger(categoryName, (category, logLevel) => Filter?.Invoke(category, logLevel) ?? true, scopeProvider, _config, _writer.WriteMessage);
    }

    private void UpdateFilter(LoggerFilterOptions options)
    {
        Filter = (category, logLevel) =>
        {
            if (_customFilter != null && !_customFilter(category, logLevel))
            {
                return false;
            }

            return MatchFilterRule(options, category, logLevel);
        };
    }

    private static bool MatchFilterRule(LoggerFilterOptions options, string category, LogLevel logLevel)
    {
        if (logLevel == LogLevel.None)
        {
            return false;
        }

        var rule = SelectRule(options, category);
        if (rule?.Filter != null)
        {
            return rule.Filter(ProviderName, category, logLevel);
        }

        var minLevel = rule?.LogLevel ?? options.MinLevel;
        return logLevel >= minLevel;
    }

    private static LoggerFilterRule? SelectRule(LoggerFilterOptions options, string category)
    {
        LoggerFilterRule? selectedRule = null;
        var selectedProviderSpecificity = -1;
        var selectedCategoryLength = -1;

        foreach (var rule in options.Rules)
        {
            if (!IsProviderMatch(rule.ProviderName) || !IsCategoryMatch(rule.CategoryName, category))
            {
                continue;
            }

            var providerSpecificity = string.IsNullOrEmpty(rule.ProviderName) ? 0 : 1;
            var categoryLength = rule.CategoryName?.Length ?? 0;

            if (providerSpecificity > selectedProviderSpecificity
                || providerSpecificity == selectedProviderSpecificity && categoryLength > selectedCategoryLength
                || providerSpecificity == selectedProviderSpecificity && categoryLength == selectedCategoryLength)
            {
                selectedRule = rule;
                selectedProviderSpecificity = providerSpecificity;
                selectedCategoryLength = categoryLength;
            }
        }

        return selectedRule;
    }

    private static bool IsProviderMatch(string? providerName)
    {
        if (string.IsNullOrEmpty(providerName))
        {
            return true;
        }

        return string.Equals(providerName, ProviderName, StringComparison.OrdinalIgnoreCase)
            || !string.IsNullOrEmpty(ProviderAlias) && string.Equals(providerName, ProviderAlias, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsCategoryMatch(string? ruleCategory, string category) => string.IsNullOrEmpty(ruleCategory)
        || category.StartsWith(ruleCategory, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Dispose 方法
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _loggerFilterReloadToken?.Dispose();
        _optionsReloadToken?.Dispose();
        _writer.Dispose();
    }
}
