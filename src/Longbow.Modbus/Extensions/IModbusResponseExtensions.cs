// Copyright (c) Argo Zhang (argo@live.ca). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://github.com/LongbowExtensions/

using System.Buffers.Binary;

namespace Longbow.Modbus;

/// <summary>
/// <see cref="IModbusResponse"/> 扩展方法
/// </summary>
public static class IModbusResponseExtensions
{
    private const int TcpPayloadOffset = 9;
    private const int RtuPayloadOffset = 3;

    private static float ReadFloatValue(IModbusResponse response, int index, int byte0, int byte1, int byte2, int byte3)
    {
        var offset = (response.Builder is IModbusTcpMessageBuilder ? TcpPayloadOffset : RtuPayloadOffset) + (index * 2);
        Span<byte> buffer =
        [
            response.Buffer.Span[offset + byte0],
            response.Buffer.Span[offset + byte1],
            response.Buffer.Span[offset + byte2],
            response.Buffer.Span[offset + byte3]
        ];

        return BinaryPrimitives.ReadSingleLittleEndian(buffer);
    }

    /// <summary>
    /// 将 <see cref="IModbusResponse"/> 实例中 <see cref="IModbusResponse.Buffer"/> 转换成布尔数组
    /// </summary>
    /// <param name="response"></param>
    /// <param name="numberOfPoints"></param>
    /// <returns></returns>
    public static bool[] ReadBoolValues(this IModbusResponse response, ushort numberOfPoints)
    {
        return response.Builder is IModbusTcpMessageBuilder
            ? ModbusTcpMessageConverter.ReadBoolValues(response.Buffer, numberOfPoints)
            : ModbusRtuMessageConverter.ReadBoolValues(response.Buffer, numberOfPoints);
    }

    /// <summary>
    /// 将 <see cref="IModbusResponse"/> 实例中 <see cref="IModbusResponse.Buffer"/> 转换成无符号短整型数组
    /// </summary>
    /// <param name="response"></param>
    /// <param name="numberOfPoints"></param>
    /// <returns></returns>
    public static ushort[] ReadUShortValues(this IModbusResponse response, ushort numberOfPoints)
    {
        return response.Builder is IModbusTcpMessageBuilder
            ? ModbusTcpMessageConverter.ReadUShortValues(response.Buffer, numberOfPoints)
            : ModbusRtuMessageConverter.ReadUShortValues(response.Buffer, numberOfPoints);
    }

    /// <summary>
    /// 将 <see cref="IModbusResponse"/> 实例中 <see cref="IModbusResponse.Buffer"/> 按照 ABCD 格式转换成 <see cref="float"/>
    /// </summary>
    /// <param name="response"></param>
    /// <param name="index">float 值索引</param>
    /// <returns></returns>
    public static float ReadFloatABCDValue(this IModbusResponse response, int index = 0) => ReadFloatValue(response, index, 3, 2, 1, 0);

    /// <summary>
    /// 将 <see cref="IModbusResponse"/> 实例中 <see cref="IModbusResponse.Buffer"/> 按照 CDAB 格式转换成 <see cref="float"/>
    /// </summary>
    /// <param name="response"></param>
    /// <param name="index">float 值索引</param>
    /// <returns></returns>
    public static float ReadFloatCDABValue(this IModbusResponse response, int index = 0) => ReadFloatValue(response, index, 1, 0, 3, 2);

    /// <summary>
    /// 将 <see cref="IModbusResponse"/> 实例中 <see cref="IModbusResponse.Buffer"/> 按照 BADC 格式转换成 <see cref="float"/>
    /// </summary>
    /// <param name="response"></param>
    /// <param name="index">float 值索引</param>
    /// <returns></returns>
    public static float ReadFloatBADCValue(this IModbusResponse response, int index = 0) => ReadFloatValue(response, index, 1, 0, 3, 2);

    /// <summary>
    /// 将 <see cref="IModbusResponse"/> 实例中 <see cref="IModbusResponse.Buffer"/> 按照 DCBA 格式转换成 <see cref="float"/>
    /// </summary>
    /// <param name="response"></param>
    /// <param name="index">float 值索引</param>
    /// <returns></returns>
    public static float ReadFloatDCBAValue(this IModbusResponse response, int index = 0) => ReadFloatValue(response, index, 0, 1, 2, 3);

    /// <summary>
    /// 将 <see cref="IModbusResponse"/> 实例中 <see cref="IModbusResponse.Buffer"/> 指定偏移的数据转换成字节数组
    /// </summary>
    /// <param name="response"></param>
    /// <param name="index">数据偏移量</param>
    /// <param name="length">读取长度</param>
    /// <returns></returns>
    public static byte[] ReadBytes(this IModbusResponse response, int index, int length)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(index, nameof(index));
#else
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
#endif
        if (length <= 0 || response.Buffer.IsEmpty)
        {
            return [];
        }

        var payloadOffset = response.Builder is IModbusTcpMessageBuilder ? TcpPayloadOffset : RtuPayloadOffset;
        if (response.Buffer.Length < payloadOffset)
        {
            return [];
        }

        // 得到数据字节数
        var byteCount = response.Buffer.Span[payloadOffset - 1];
        if (index >= byteCount)
        {
            return [];
        }

        var offset = payloadOffset + index;
        var availableLength = Math.Min(length, byteCount - index);
        var bufferLength = Math.Min(availableLength, response.Buffer.Length - offset);
        if (bufferLength <= 0)
        {
            return [];
        }

        var bytes = new byte[bufferLength];
        response.Buffer.Slice(offset, bufferLength).CopyTo(bytes);
        return bytes;
    }
}
