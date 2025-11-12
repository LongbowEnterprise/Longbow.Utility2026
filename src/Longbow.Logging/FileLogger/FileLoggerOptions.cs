// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

namespace Longbow.Logging;

/// <summary>
/// 文件日志配置类
/// </summary>
public class FileLoggerOptions
{
    /// <summary>
    /// 获得/设置 是否开启上下文作用域功能 默认为 true 启用
    /// </summary>
    public bool IncludeScopes { get; set; } = true;

    /// <summary>
    /// 获得/设置 日志文件名 默认为 Logs\\Log.log 文件名追加日期
    /// </summary>
    public string FileName { get; set; } = $"Logs{Path.DirectorySeparatorChar}Log.log";

    /// <summary>
    /// 获得/设置 日志文件保留最多个数 默认为 7 个
    /// </summary>
    public int MaxFileCount { get; set; } = 7;

    /// <summary>
    /// 获得/设置 是否开启批次写入功能 默认为 true 开启
    /// </summary>
    public bool Batch { get; set; } = true;

    /// <summary>
    /// 获得/设置 批次写入时间间隔 默认 1000 毫秒
    /// </summary>
    public int BatchCount { get; set; } = 100;

    /// <summary>
    /// 获得/设置 批次写入时间间隔 默认为 5000 毫秒
    /// </summary>
    public int Interval { get; set; } = 5000;

    /// <summary>
    /// 获得/设置 实例销毁时日志任务等待时间 默认为 2 秒
    /// </summary>
    public TimeSpan TaskWaitTime { get; set; } = TimeSpan.FromSeconds(2);
}
