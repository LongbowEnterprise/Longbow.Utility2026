// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

namespace Longbow.UdpSocket;

static class TcpSocketClientOptionsExtensions
{
    public static UdpSocketClientOptions CopyTo(this UdpSocketClientOptions source) => new()
    {
        ReceiveBufferSize = source.ReceiveBufferSize,
        IsAutoReceive = source.IsAutoReceive,
        LocalEndPoint = source.LocalEndPoint,
        NoDelay = source.NoDelay
    };
}
