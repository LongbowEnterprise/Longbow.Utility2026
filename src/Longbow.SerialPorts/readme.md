# Longbow.SerialPorts

`Longbow.SerialPorts` is a .NET-based serial port communication extension library. It provides easy-to-use serial port operation interfaces, supports asynchronous data transmission and reception, port management, and is compatible with multiple platforms and frameworks (net6.0, net7.0, net8.0, net9.0).

## Features

- Simple and user-friendly serial port communication interface `ITcpSocketFactory`
- Supports asynchronous open, close, send, and receive operations
- Configurable serial port parameters (port name, baud rate, data bits, parity, stop bits, etc.)
- Dependency injection extension (`AddSerialPortFactory`)
- Compatible with multiple .NET versions

## Getting Started

### Installation

Install via NuGet:

```shell
dotnet add package Longbow.SerialPorts
```

### Usage Example

```csharp
var factory = serviceProvider.GetRequiredService<ITcpSocketFactory>();
var client = factory.GetOrCreate("com1", options => 
{
    options.PortName = "COM1";
	options.BaudRate = 9600,
	options.DataBits = 8,
	options.Parity = Parity.None,
	options.StopBits = StopBits.One
});
await client.OpenAsync();

var data = new byte[] { 0x01, 0x02, 0x03 };
await client.SendAsync(data);

var received = await client.ReceiveAsync();
await client.CloseAsync();
```

### Dependency Injection

```csharp
services.AddSerialPortFactory();
```

## API Overview

- `ISerialPortClient`: Serial port communication interface with async operations
- `SerialPortOptions`: Serial port parameter configuration class

## License

Apache-2.0

## Related Links

- [GitHub Repository](https://github.com/LongbowEnterprise/Longbow.SerialPorts?wt.mc_id=DT-MVP-5004174)
