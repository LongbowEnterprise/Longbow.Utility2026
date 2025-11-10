// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

namespace Longbow.SerialPorts;

/// <summary>
/// 串口通讯接口
/// </summary>
public interface ISerialPortClient : IAsyncDisposable
{
    /// <summary>
    /// 获得 端口是否打开
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// 获得 当前配置 <see cref="SerialPortOptions"/> 实例
    /// </summary>
    SerialPortOptions Options { get; }

    /// <summary>
    /// 打开端口方法
    /// </summary>
    /// <returns></returns>
    ValueTask<bool> OpenAsync(CancellationToken token = default);

    /// <summary>
    /// 发送数据方法
    /// </summary>
    /// <param name="data"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<bool> SendAsync(ReadOnlyMemory<byte> data, CancellationToken token = default);

    /// <summary>
    /// 接收数据方法
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    ValueTask<ReadOnlyMemory<byte>> ReceiveAsync(CancellationToken token = default);

    /// <summary>
    /// 关闭端口方法
    /// </summary>
    /// <returns></returns>
    ValueTask CloseAsync();
}
