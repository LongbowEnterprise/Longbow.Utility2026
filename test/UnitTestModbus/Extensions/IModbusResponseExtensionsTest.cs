// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using Microsoft.Extensions.DependencyInjection;

namespace UnitTest.Extensions;

public class IModbusResponseExtensionsTest
{
    [Fact]
    public void ReadFloatABCDValue_Ok()
    {
        AssertFloatValue("01 03 04 41 B6 E6 6C BC 80", "00 01 00 00 00 07 01 03 04 41 B6 E6 6C", static response => response.ReadFloatABCDValue());
    }

    [Fact]
    public void ReadFloatBADCValue_Ok()
    {
        AssertFloatValue("01 03 04 E6 6C 41 B6 BC 80", "00 01 00 00 00 07 01 03 04 E6 6C 41 B6", static response => response.ReadFloatBADCValue());
    }

    [Fact]
    public void ReadFloatCDABValue_Ok()
    {
        AssertFloatValue("01 03 04 E6 6C 41 B6 BC 80", "00 01 00 00 00 07 01 03 04 E6 6C 41 B6", static response => response.ReadFloatCDABValue());
    }

    [Fact]
    public void ReadFloatDCBAValue_Ok()
    {
        AssertFloatValue("01 03 04 6C E6 B6 41 BC 80", "00 01 00 00 00 07 01 03 04 6C E6 B6 41", static response => response.ReadFloatDCBAValue());
    }

    private static void AssertFloatValue(string rtuData, string tcpData, Func<IModbusResponse, float> reader)
    {
        var sc = new ServiceCollection();
        sc.AddModbusFactory();

        var provider = sc.BuildServiceProvider();
        var expected = BitConverter.Int32BitsToSingle(unchecked((int)0x41B6E66C));

        var rtuBuilder = provider.GetRequiredService<IModbusRtuMessageBuilder>();
        IModbusResponse response = new TestModbusResponse(HexConverter.ToBytes(rtuData, " "), rtuBuilder);
        Assert.Equal(expected, reader(response));

        var tcpBuilder = provider.GetRequiredService<IModbusTcpMessageBuilder>();
        response = new TestModbusResponse(HexConverter.ToBytes(tcpData, " "), tcpBuilder);
        Assert.Equal(expected, reader(response));
    }

    sealed class TestModbusResponse(ReadOnlyMemory<byte> buffer, IModbusMessageBuilder builder) : IModbusResponse
    {
        public ReadOnlyMemory<byte> Buffer => buffer;

        public IModbusMessageBuilder Builder => builder;

        public Exception? Exception => null;

        public bool IsSuccess => true;
    }
}
