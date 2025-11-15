// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Longbow.Logging;

public class DBLoggerTest
{
    [Fact]
    public void DBLogger_Exception()
    {
        var sc = new ServiceCollection();
        Assert.Throws<ArgumentNullException>(() => sc.AddLogging(builder => builder.AddDBLogger(null!)));
    }

    [Fact]
    public void ToIPv4String_Ok()
    {
        // 代码覆盖率
        var ipMethod = typeof(DBLoggerProvider).Assembly.GetType("System.Net.InternalIPAddressExtensions");
        Assert.NotNull(ipMethod);

        var method = ipMethod.GetMethod("ToIPv4String", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        Assert.NotNull(method);
        method.Invoke(null, [IPAddress.Parse("::ffff:192.168.0.1")]);
    }
}
