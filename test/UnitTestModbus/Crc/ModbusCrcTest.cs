// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Longbow.Modbus.Algorithm;

namespace UnitTest.Crc;

public class ModbusCrcTest
{
    [Fact]
    public void Computer_Ok()
    {
        // 06 00 00 01 7F C9 BA
        var data = new byte[] { 0x01, 0x06, 0x00, 0x00, 0x01, 0x7F };

        var crc = ModbusCrc16.Compute(data);
        Assert.Equal("BAC9", crc.ToString("X4"));
        Assert.Equal("01060000017FC9BA", HexConverter.ToString(ModbusCrc16.Append(data), ""));

        data = [0x01, 0x03, 0x00, 0x12, 0x00, 0x02];
        crc = ModbusCrc16.Compute(data);
        Assert.Equal("0E64", crc.ToString("X4"));
        Assert.Equal("010300120002640E", HexConverter.ToString(ModbusCrc16.Append(data), ""));
    }

    [Fact]
    public void AppendMemory_Ok()
    {
        Memory<byte> buffer = new byte[16];
        byte[] data = [0x01, 0x03, 0x00, 0x12, 0x00, 0x02];
        data.CopyTo(buffer);

        var len = ModbusCrc16.Append(buffer, 6);

        Assert.Equal(8, len);
        Assert.Equal("010300120002640E", HexConverter.ToString(buffer[..len].Span, ""));
    }

    [Fact]
    public void Validate_Ok()
    {
        var result = ModbusCrc16.Validate([0x01]);
        Assert.False(result);

        result = ModbusCrc16.Validate([0x01, 0x06, 0x00, 0x00, 0x01, 0x7F, 0xC9, 0xBA]);
        Assert.True(result);

        result = false;
        var data = Enumerable.Range(0, 300).Select(i => (byte)Random.Shared.Next(0, 255));
        result = ModbusCrc16.Validate(ModbusCrc16.Append(data.ToArray()));
        Assert.True(result);
    }
}
