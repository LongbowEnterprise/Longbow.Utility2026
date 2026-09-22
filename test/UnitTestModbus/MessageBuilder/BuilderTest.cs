// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using System.Reflection;

namespace UnitTest.Builder;

public class BuilderTest
{
    [Fact]
    public void ValidateNumberOfPoints_Ok()
    {
        Assert.ThrowsAny<ArgumentException>(() => ValidateNumberOfPoints("test", 0, 10));
        ValidateNumberOfPoints("test", 1, 10);
        ValidateNumberOfPoints("test", 10, 10);
        Assert.ThrowsAny<ArgumentException>(() => ValidateNumberOfPoints("test", 11, 10));
    }

    [Fact]
    public void ValidateData_Ok()
    {
        Assert.ThrowsAny<Exception>(() => ValidateData("test", Array.Empty<byte>(), 10));
        ValidateData("test", new byte[] { 0x01, 0x02 }, 2);
        Assert.ThrowsAny<ArgumentException>(() => ValidateData("test", new byte[] { 0x01, 0x02 }, 1));
    }

    [Fact]
    public void WriteMultipleBoolValues_OneValue()
    {
        var buffer = new byte[8];
        var len = WriteValues("WriteMultipleBoolValues", buffer, 0x0012, new[] { true });

        Assert.Equal(6, len);
        Assert.Equal("001200010101", HexConverter.ToString(buffer.AsSpan(0, len), ""));
    }

    [Fact]
    public void WriteMultipleUShortValues_OneValue()
    {
        var buffer = new byte[8];
        var len = WriteValues("WriteMultipleUShortValues", buffer, 0x0012, new ushort[] { 0x1234 });

        Assert.Equal(7, len);
        Assert.Equal("00120001021234", HexConverter.ToString(buffer.AsSpan(0, len), ""));
    }

    private static void ValidateNumberOfPoints(string argumentName, ushort numberOfPoints, ushort maxNumberOfPoints)
    {
        var type = Type.GetType("Longbow.Modbus.MessageBuilder, Longbow.Modbus");
        var method = type?.GetMethod("ValidateNumberOfPoints", BindingFlags.Static | BindingFlags.Public);
        if (method != null)
        {
            try
            {
                method.Invoke(null, [argumentName, numberOfPoints, maxNumberOfPoints]);
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
        }
    }

    private static void ValidateData<T>(string argumentName, T[] data, int maxDataLength)
    {
        var type = Type.GetType("Longbow.Modbus.MessageBuilder, Longbow.Modbus");
        var method = type?.GetMethod("ValidateData", BindingFlags.Static | BindingFlags.Public);
        if (method != null)
        {
            try
            {
                method = method.MakeGenericMethod(typeof(T));
                method.Invoke(null, [argumentName, data, maxDataLength]);
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
        }
    }

    private static int WriteValues<T>(string methodName, Memory<byte> buffer, ushort address, T[] values)
    {
        var type = Type.GetType("Longbow.Modbus.MessageBuilder, Longbow.Modbus") ?? throw new InvalidOperationException();
        var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public) ?? throw new InvalidOperationException();

        return (int)(method.Invoke(null, [buffer, address, values]) ?? throw new InvalidOperationException());
    }
}
