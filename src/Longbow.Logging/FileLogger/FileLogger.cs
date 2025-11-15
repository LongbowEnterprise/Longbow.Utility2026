// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Longbow.Logging;

internal class FileLogger(string name, Func<string, LogLevel, bool>? filter, IExternalScopeProvider? scopeProvider, IConfiguration? config, Action<string> writeCallback) : LoggerBase(name, filter, scopeProvider, config)
{
    private readonly Action<string> _writeCallback = writeCallback;

    /// <summary>
    /// 写入日志到文件方法
    /// </summary>
    /// <param name="message"></param>
    protected override void WriteMessageCore(string message) => _writeCallback(message);
}
