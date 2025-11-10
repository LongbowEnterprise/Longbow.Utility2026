// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Longbow.SerialPorts;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Runtime.Versioning;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// ISerialPortFactory 扩展方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 增加 ITcpSocketFactory 服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    [UnsupportedOSPlatform("browser")]
    public static IServiceCollection AddSerialPortFactory(this IServiceCollection services, Action<SerialPortOptions>? configureOptions = null)
    {
        // 添加 ISerialPortsFactory 服务
        services.TryAddSingleton<ISerialPortFactory, DefaultSerialPortFactory>();

        // 增加 ISerialPortClient 服务
        services.TryAddTransient<ISerialPortClient, DefaultSerialPortClient>();

        // 增加全局配置
        services.Configure<SerialPortOptions>(op => configureOptions?.Invoke(op));

        return services;
    }
}
