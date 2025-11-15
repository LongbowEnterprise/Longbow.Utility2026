// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.Logging;

namespace Longbow.Logging;

public class FileLoggerWriterTest : IDisposable
{
    private ILogger logger;
    private ILoggerProvider provider;

    public FileLoggerWriterTest()
    {
        var fileName = $"Logging{Path.DirectorySeparatorChar}Logger.log";
        provider = new FileLoggerProvider(new FileLoggerOptions()
        {
            FileName = fileName,
            Interval = 1000,
            Batch = true,
            MaxFileCount = 1,
            BatchCount = 100
        });

        logger = provider.CreateLogger(nameof(FileLoggerWriterTest));
    }

    [Fact]
    public void Task_Wait()
    {
        var t = Task.Delay(2000);
        var token = new CancellationToken(true);
        Assert.ThrowsAny<OperationCanceledException>(() => t.Wait(token));
    }

    [Fact]
    public void Task_Cancel()
    {
        var cts = new CancellationTokenSource(500);
        var task = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(1000, cts.Token);
            }
            catch (TaskCanceledException) { }

            //
            await Task.Delay(2000);
            var foo = DateTime.Now;
        }, cts.Token);
        var r = task.Wait(20000);
        Assert.True(r);
    }

    [Fact]
    public async void RollFile_Ok()
    {
        var fileName = $"Logging{Path.DirectorySeparatorChar}Log.log";
        var provider = new FileLoggerProvider(new FileLoggerOptions()
        {
            FileName = fileName,
            Interval = 1000,
            Batch = true,
            MaxFileCount = 1
        });
        var logger = provider.CreateLogger(nameof(FileLoggerWriterTest));

        // 生成过期文件
        var file = CreateOldLogFile(fileName);
        var folder = file.Folder;
        var rollFileName = file.RollFileName;

        // 过期日志文件存在
        Assert.True(File.Exists(rollFileName));
        logger.LogInformation("Test - RollFile");
        await Task.Delay(500);

        provider.Dispose();
    }

    private static (string Folder, string RollFileName) CreateOldLogFile(string fileName)
    {
        // 清空文件夹
        var folder = Path.Combine(AppContext.BaseDirectory, Path.GetDirectoryName(fileName));
        Directory.CreateDirectory(folder);

        // 生产长度不满足内部判断条件的干扰文件
        var fn = $"Log01.log";
        using (var fs = File.AppendText(Path.Combine(folder, fn))) { }

        // 准备文件，测试RollFile
        var rollFileName = Path.Combine(folder, $"{Path.GetFileNameWithoutExtension(fileName)}-{DateTime.Today.AddDays(0 - 1):yyyyMMdd}.log");
        var sw = File.AppendText(rollFileName);
        sw.Close();
        sw.Dispose();
        return (folder, rollFileName);
    }

    [Fact]
    public async void ManyLog_Ok()
    {
        // 超过100条记录
        for (var i = 0; i < 110; i++) logger.LogDebug($"Test - {i:D2}");

        // 等待批次写入线程启动
        await Task.Delay(1100);
    }

    [Fact]
    public void InternalError_Ok()
    {
        // 代码覆盖率
        provider.Dispose();
        provider = new FileLoggerProvider(new FileLoggerOptions() { Batch = false });
        logger = provider.CreateLogger("UnitTest");
        var _writer = provider.GetType().GetField("_logWriter", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(provider);

        // 设置内部 Token Cancel
        var _cancellationTokenSource = _writer.GetType().GetField("_cancellationTokenSource", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(_writer) as CancellationTokenSource;
        _cancellationTokenSource.Cancel();

        // 再写日志 导致内部错误
        logger.LogError("Test");
        provider.Dispose();
    }

    public void Dispose() => provider.Dispose();
}
