// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using System.Collections.Concurrent;

namespace Longbow.Logging;

/// <summary>
/// 日志文件内部操作类
/// </summary>
public class FileLoggerWriter : IDisposable
{
    private readonly BlockingCollection<LogMessage> _messageQueue = new(new ConcurrentQueue<LogMessage>());
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly List<LogMessage> _currentBatch = [];

    private string _logFolder;
    private string? _logFileName;
    private readonly Task _logTask;

    /// <summary>
    /// 获得/设置 日志配置项 FileLoggerOptions 实例
    /// </summary>
    public FileLoggerOptions Option { get; set; }

    /// <summary>
    /// 默认构造函数
    /// </summary>
    /// <param name="option">日志配置项 FileLoggerOptions 实例</param>
    public FileLoggerWriter(FileLoggerOptions option)
    {
        Option = option;
        _logFolder = "";
        _logTask = Task.Run(ProcessLogQueue);
    }

    /// <summary>
    /// 写消息操作
    /// </summary>
    /// <param name="message">日志消息内容</param>
    public void WriteMessage(string message)
    {
        var msg = new LogMessage { Message = message, Timestamp = DateTimeOffset.Now };
        _logFolder = Path.Combine(AppContext.BaseDirectory, Path.GetDirectoryName(Option.FileName) ?? string.Empty);

        if (!Directory.Exists(_logFolder))
        {
            Directory.CreateDirectory(_logFolder);
        }

        RollFilesAsync();

        if (Option.Batch && !_messageQueue.IsAddingCompleted)
        {
            _messageQueue.Add(msg, _cancellationTokenSource.Token);
        }
        else
        {
            _logFileName = $"{Path.GetFileNameWithoutExtension(Option.FileName)}-{msg.Timestamp:yyyyMMdd}{Path.GetExtension(Option.FileName)}";
            var fullName = Path.Combine(_logFolder, _logFileName);
            WriteToFile(fullName, msg.Message);
        }
    }

    private async Task ProcessLogQueue()
    {
        var cancelSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, _flushCancellationTokenSource.Token);
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            var limit = Option.BatchCount;
            while ((force || limit > 0) && _messageQueue.TryTake(out var message))
            {
                _currentBatch.Add(message);
                limit--;
            }

            if (_flushCancellationTokenSource.IsCancellationRequested)
            {
                // force flush output
                _flushCancellationTokenSource.Dispose();
                _flushCancellationTokenSource = new CancellationTokenSource();

                // 恢复强制输出开关
                force = false;
                cancelSource.Dispose();
                cancelSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, _flushCancellationTokenSource.Token);
            }

            try
            {
                if (_currentBatch.Count > 0)
                {
                    WriteMessagesToFile(_currentBatch);
                    _currentBatch.Clear();
                }
                await Task.Delay(Math.Max(1000, Option.Interval), cancelSource.Token);
            }
            catch (TaskCanceledException)
            {
                if (_flushCancellationTokenSource.IsCancellationRequested)
                {
                    force = true;
                    continue;
                }
                break;
            }
        }
        cancelSource.Dispose();

        // flush message to file
        while (_messageQueue.TryTake(out var message))
        {
            _currentBatch.Add(message);
        }

        WriteMessagesToFile(_currentBatch);
    }

    private void WriteMessagesToFile(IEnumerable<LogMessage> messages)
    {
        foreach (var group in messages.GroupBy(m => new { m.Timestamp.Year, m.Timestamp.Month, m.Timestamp.Day }))
        {
            _logFileName = $"{Path.GetFileNameWithoutExtension(Option.FileName)}-{group.Key.Year:0000}{group.Key.Month:00}{group.Key.Day:00}{Path.GetExtension(Option.FileName)}";
            if (!string.IsNullOrEmpty(_logFolder))
            {
                var fullName = Path.Combine(_logFolder, _logFileName);
                WriteToFile(fullName, string.Join(string.Empty, group.Select(log => log.Message)));
            }
        }
    }

#if NET9_0_OR_GREATER
    private readonly Lock _writeLocker = new();
#else
    private readonly object _writeLocker = new();
#endif

    private void WriteToFile(string fileName, string content)
    {
        lock (_writeLocker)
        {
            using var streamWriter = File.AppendText(fileName);
            streamWriter.Write(content);
        }
    }

    private void RollFilesAsync()
    {
        if (Option.MaxFileCount > 0)
        {
            Task.Run(() =>
            {
                var rollFile = Directory
                .EnumerateFiles(_logFolder, $"{Path.GetFileNameWithoutExtension(Option.FileName)}*{Path.GetExtension(Option.FileName)}")
                .FirstOrDefault(f =>
                {
                    var fn = Path.GetFileNameWithoutExtension(f);
                    return fn.Length > 8 && fn.AsSpan()[(fn.Length - 8)..].ToString().CompareTo(DateTimeOffset.Now.AddDays(0 - Option.MaxFileCount).ToString("yyyyMMdd")) < 1;
                });
                if (!string.IsNullOrEmpty(rollFile))
                {
                    File.Delete(rollFile);
                }
            });
        }
    }

    private bool force;
    private CancellationTokenSource _flushCancellationTokenSource = new();
    /// <summary>
    /// 强制输出到日志文件中
    /// </summary>
    public void Flush() => _flushCancellationTokenSource.Cancel();

    /// <summary>
    /// Dispose 方法
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool disposedValue;

    /// <summary>
    /// Dispose 重载方法
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;

                _messageQueue.CompleteAdding();
                _cancellationTokenSource.Cancel();
                _logTask.Wait(Option.TaskWaitTime);

                _cancellationTokenSource.Dispose();
                _messageQueue.Dispose();
            }
        }
    }
}
