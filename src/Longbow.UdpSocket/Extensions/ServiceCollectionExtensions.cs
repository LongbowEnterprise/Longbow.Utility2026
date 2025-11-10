// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Runtime.Versioning;

namespace Longbow.UdpSocket;

/// <summary>
/// TcpSocket 扩展方法
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
    public static IServiceCollection AddUdpSocketFactory(this IServiceCollection services, Action<UdpSocketClientOptions>? configureOptions = null)
    {
        // 添加 IUdpSocketFactory 服务
        services.TryAddSingleton<IUdpSocketFactory, DefaultUdpSocketFactory>();

        // 增加 ISocketClientProvider 服务
        services.TryAddTransient<IUdpSocketClient, DefaultUdpSocketClient>();

        // 增加全局配置
        services.Configure<UdpSocketClientOptions>(op => configureOptions?.Invoke(op));

        return services;
    }
}
