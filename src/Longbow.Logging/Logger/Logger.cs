// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Longbow.Logging;

internal class Logger(string name, Func<string, LogLevel, bool>? filter, IExternalScopeProvider? scopeProvider, IConfiguration? config) : LoggerBase(name, filter, scopeProvider, config)
{
    protected override void WriteMessageCore(string content) => Console.Write(content);
}
