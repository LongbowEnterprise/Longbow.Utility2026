// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

namespace Longbow.SerialPorts;

static class SerialPortOptionsExtensions
{
    public static SerialPortOptions CopyTo(this SerialPortOptions source) => new()
    {
        BaudRate = source.BaudRate,
        DataBits = source.DataBits,
        Parity = source.Parity,
        PortName = source.PortName,
        StopBits = source.StopBits,
        DiscardNull = source.DiscardNull,
        Handshake = source.Handshake,
        ReadBufferSize = source.ReadBufferSize,
        WriteBufferSize = source.WriteBufferSize,
        ReadTimeout = source.ReadTimeout,
        WriteTimeout = source.WriteTimeout,
        RtsEnable = source.RtsEnable,
        DtrEnable = source.DtrEnable
    };
}
