# Longbow.Logging

English README — Longbow.Logging is a lightweight, extensible .NET logging library/wrapper designed to provide a consistent logging interface, structured logging support (message templates + properties), and common extensions for console and file targets.

## Key Features
- Adapter compatible with Microsoft.Extensions.Logging
- Structured logging (templated messages and properties)
- Simple configuration (appsettings.json / DI)
- Examples and extension points for common sinks (Console, File)
- Exception capture and user-friendly exception output formatting

## Quick Start

### Install
```powershell
dotnet add package Longbow.Logging
```

### Register services
```csharp
services.AddLogging(logging => logging.AddFileLogger());
```

## Example configuration (appsettings.json)
```json
{
	"Logging": {
		"LogLevel": {
			"Default": "Information",
			"Microsoft.AspNetCore": "Warning"
		},
		"LgbFile": {
			"IncludeScopes": true,
			"LogLevel": {
				"Default": "Error"
			},
			"FileName": "Error\\Log.log"
		}
	}
}
```

## Contributing
Contributions are welcome. Please follow these steps:
1. Fork the repository and develop on a feature branch
2. Add tests and ensure existing tests pass
3. Open a pull request describing your changes and motivation

## License
MIT — replace with the actual license used by the project if different.

## Contact
If you need help or want to report an issue, please open an Issue in the repository.

Note: Adjust the example code, configuration keys, and names to match the project's actual implementation where necessary.

