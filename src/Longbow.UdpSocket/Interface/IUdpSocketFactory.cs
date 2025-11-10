// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

namespace Longbow.UdpSocket;

/// <summary>
/// IUdpSocketFactory Interface
/// </summary>
public interface IUdpSocketFactory : IAsyncDisposable
{
    /// <summary>
    /// Retrieves an existing Udp socket client by name or creates a new one using the specified configuration.
    /// </summary>
    /// <param name="name">The unique name of the Udp socket client to retrieve or create. if value is null or empty do not use cache</param>
    /// <param name="configureOptions">A delegate used to configure the <see cref="UdpSocketClientOptions"/> for the new Udp socket client if it does not
    /// already exist. This delegate is invoked only when a new client is created.</param>
    /// <returns>An instance of <see cref="IUdpSocketClient"/> corresponding to the specified name. If the client already exists,
    /// the existing instance is returned; otherwise, a new instance is created and returned.</returns>
    IUdpSocketClient GetOrCreate(string name, Action<UdpSocketClientOptions>? configureOptions = null);

    /// <summary>
    /// Retrieves an existing Udp socket client by name or creates a new one using the specified configuration.
    /// </summary>
    /// <param name="configureOptions">A delegate used to configure the <see cref="UdpSocketClientOptions"/> for the new Udp socket client if it does not
    /// already exist. This delegate is invoked only when a new client is created.</param>
    /// <returns>An instance of <see cref="IUdpSocketClient"/> corresponding to the specified name. If the client already exists,
    /// the existing instance is returned; otherwise, a new instance is created and returned.</returns>
    IUdpSocketClient GetOrCreate(Action<UdpSocketClientOptions>? configureOptions = null);

    /// <summary>
    /// Removes the Udp socket client associated with the specified name.
    /// </summary>
    /// <param name="name">The name of the Udp socket client to remove. Cannot be <see langword="null"/> or empty.</param>
    /// <returns>The removed <see cref="IUdpSocketClient"/> instance if a client with the specified name exists;  otherwise, <see
    /// langword="null"/>.</returns>
    IUdpSocketClient? Remove(string name);
}
