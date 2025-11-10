// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

namespace Longbow.SerialPorts;

/// <summary>
/// ISerialPortFactory Interface
/// </summary>
public interface ISerialPortFactory : IAsyncDisposable
{
    /// <summary>
    /// 生成 <see cref="ISerialPortClient"/> 实例方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    ISerialPortClient GetOrCreate(string name, Action<SerialPortOptions>? configureOptions = null);

    /// <summary>
    /// 生成 <see cref="ISerialPortClient"/> 实例方法
    /// </summary>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    ISerialPortClient GetOrCreate(Action<SerialPortOptions>? configureOptions = null);

    /// <summary>
    /// 移除指定名称 <see cref="ISerialPortClient"/> 实例方法
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    ISerialPortClient? Remove(string name);
}
