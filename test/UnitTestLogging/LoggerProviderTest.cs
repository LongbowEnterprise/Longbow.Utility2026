// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.Logging;

namespace Longbow.Logging;

public class LoggerProviderTest
{
    [Fact]
    public void CreateLogger_Ok()
    {
        var provider = new LoggerProvider();
        var logger = provider.CreateLogger<LoggerProviderTest>();
        Assert.NotNull(logger);
        logger.Log(LogLevel.Information, string.Empty);
        logger.Log(LogLevel.None, string.Empty);
        provider.Dispose();

        provider = new LoggerProvider(null, null);
        logger = provider.CreateLogger<LoggerProviderTest>();
        logger.Log(LogLevel.Information, string.Empty);
        logger.Log(LogLevel.None, string.Empty);
        provider.Dispose();
    }
}
