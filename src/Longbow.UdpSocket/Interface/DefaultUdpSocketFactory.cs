// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Longbow.UdpSocket;

sealed class DefaultUdpSocketFactory(IServiceProvider provider) : IUdpSocketFactory
{
    private readonly ConcurrentDictionary<string, IUdpSocketClient> _pool = new();

    public IUdpSocketClient GetOrCreate(string name, Action<UdpSocketClientOptions>? configureOptions = null) => string.IsNullOrEmpty(name)
        ? CreateClient(configureOptions)
        : _pool.GetOrAdd(name, key => CreateClient(configureOptions));

    public IUdpSocketClient GetOrCreate(Action<UdpSocketClientOptions>? configureOptions = null) => CreateClient(configureOptions);

    private IUdpSocketClient CreateClient(Action<UdpSocketClientOptions>? configureOptions = null)
    {
        var client = provider.GetRequiredService<IUdpSocketClient>();
        configureOptions?.Invoke(client.Options);
        return client;
    }

    public IUdpSocketClient? Remove(string name)
    {
        IUdpSocketClient? client = null;
        if (_pool.TryRemove(name, out var c))
        {
            client = c;
        }
        return client;
    }

    private async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            // 释放托管资源
            foreach (var socket in _pool.Values)
            {
                await socket.DisposeAsync();
            }
            _pool.Clear();
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }
}
