// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Longbow.Logging;

public class FileLoggerTest
{
    [Theory]
    [InlineData(LogLevel.Trace, true)]
    [InlineData(LogLevel.Debug, true)]
    [InlineData(LogLevel.Information, true)]
    [InlineData(LogLevel.Warning, true)]
    [InlineData(LogLevel.Error, true)]
    [InlineData(LogLevel.Critical, true)]
    [InlineData(LogLevel.None, false)]
    public void IsEnabled_FileLogger_Ok(LogLevel level, bool result)
    {
        var sc = new ServiceCollection();
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json");
        var config = builder.Build();
        sc.AddSingleton<IConfiguration>(config);

        sc.AddLogging(builder =>
        {
            builder.AddProvider(new FileLoggerProvider())
        });

        var provider = sc.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<FileLoggerTest>>();

        // 直接调用FileLogger内部 IsEnable 方法
        //var logger = _fileLoggerProvider.CreateLogger(nameof(FileLoggerTest));
        //Assert.Equal(result, logger.IsEnabled(level));
    }

    //[Theory]
    //[InlineData(LogLevel.Trace, false)]
    //[InlineData(LogLevel.Debug, false)]
    //[InlineData(LogLevel.Information, true)]
    //[InlineData(LogLevel.Warning, true)]
    //[InlineData(LogLevel.Error, true)]
    //[InlineData(LogLevel.Critical, true)]
    //[InlineData(LogLevel.None, false)]
    //public void IsEnabled_ILogger_Ok(LogLevel level, bool result)
    //{
    //    // 由于 Logging:0:LogLevel LogLevel = Information
    //    // 大于 Information 的配置项进入 FileLogger中判断
    //    Assert.Equal(result, _logger.IsEnabled(level));
    //}

    //[Theory]
    //[InlineData(LogLevel.Trace, false)]
    //[InlineData(LogLevel.Debug, false)]
    //[InlineData(LogLevel.Information, true)]
    //[InlineData(LogLevel.Warning, true)]
    //[InlineData(LogLevel.Error, true)]
    //[InlineData(LogLevel.Critical, true)]
    //[InlineData(LogLevel.None, false)]
    //public void LogLevel_Ok(LogLevel level, bool result)
    //{
    //    // 由于 Logging:0:LogLevel LogLevel = Information
    //    // 大于 Information 的配置项进入 FileLogger中判断
    //    var logContent = string.Empty;
    //    string callback(object state, Exception ex) => logContent = (string)state;
    //    _logger.Log(level, new EventId(0), level.ToString(), null, callback);
    //    Assert.Equal(result, logContent == level.ToString());
    //}

    //[Theory]
    //[InlineData(LogLevel.Trace)]
    //[InlineData(LogLevel.Debug)]
    //[InlineData(LogLevel.Information)]
    //[InlineData(LogLevel.Warning)]
    //[InlineData(LogLevel.Error)]
    //[InlineData(LogLevel.Critical)]
    //public void GetLogLevelString_Ok(LogLevel level)
    //{
    //    var logger = _fileLoggerProvider.CreateLogger(nameof(FileLoggerTest));

    //    var method = logger.GetType().GetMethod("GetLogLevelString", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

    //    Assert.NotNull(method.Invoke(logger, new object[] { level }));
    //}

    //[Fact]
    //public void ReloadLoggerOptions_Ok()
    //{
    //    var logger = _fileLoggerProvider.CreateLogger(nameof(FileLoggerTest));

    //    var method = _fileLoggerProvider.GetType().GetMethod("ReloadLoggerOptions", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

    //    var para = new FileLoggerOptions()
    //    {
    //        Batch = true,
    //        IncludeScopes = false,
    //        Interval = 1000,
    //        MaxFileCount = 2
    //    };
    //    method.Invoke(_fileLoggerProvider, new object[] { para });
    //}

    //[Fact]
    //public void Config_Change()
    //{
    //    var locker = new AutoResetEvent(false);
    //    TestHelper.TriggerAppSettingsFileChange(_config, () => locker.Set());
    //    locker.WaitOne(500);
    //}

    //[Fact]
    //public void BatchLog_Ok()
    //{
    //    var loggerProvider = new FileLoggerProvider(new FileLoggerOptions()
    //    {
    //        Batch = true,
    //        FileName = $"Logging{Path.DirectorySeparatorChar}Batch.log",
    //        Interval = 5000,
    //        MaxFileCount = 1,
    //        BatchCount = 100,
    //        TaskWaitTime = TimeSpan.FromSeconds(2)
    //    });
    //    var logger = loggerProvider.CreateLogger(nameof(FileLoggerTest));
    //    logger.LogDebug("Message1");
    //    logger.LogDebug("Message2");
    //    loggerProvider.Dispose();
    //}

    //[Fact]
    //public void BeginScope_Ok()
    //{
    //    var logger = _fileLoggerProvider.CreateLogger(nameof(FileLoggerTest));

    //    // 调用泛型string方法
    //    var t = logger.GetType();
    //    var method = t.GetMethod("BeginScope").MakeGenericMethod(typeof(string));
    //    method.Invoke(logger, new object[] { "Test1" });
    //    method.Invoke(logger, new object[] { "Test2" });

    //    // GetScopeInformation 调用
    //    var gm = t.GetMethod("GetScopeInformation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    //    gm.Invoke(logger, new object[] { new StringBuilder(), "-" });

    //    // 设置为Null
    //    var prop = t.GetProperty("ScopeProvider");
    //    var oldValue = prop.GetValue(logger);
    //    prop.SetValue(logger, null);
    //    method.Invoke(logger, new object[] { "Test2" });
    //}

    //[Fact]
    //public async void LogHttpException_Ok()
    //{
    //    await _client.GetAsync("/Home/Index");
    //}

    //[Fact]
    //public void LogCallback_Null()
    //{
    //    // 代码覆盖率
    //    // 设置内部 LogCallback 属性为 Null
    //    var provider = new FileLoggerProvider(new FileLoggerOptions());
    //    var logger = provider.CreateLogger("LogCallback");
    //    var logCallback = logger.GetType().GetProperty("LogCallback", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    //    logCallback.SetValue(logger, null);
    //    logger.LogInformation("Test");
    //}

    //[Fact]
    //public void Option_Null()
    //{
    //    var provider = new FileLoggerProvider(null, filter: null);
    //    Assert.NotNull(provider);
    //    provider.Dispose();
    //}
}
