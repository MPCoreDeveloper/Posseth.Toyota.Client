# Posseth.Toyota

A .NET client library for interacting with Toyota Connected Services API. This project provides a fluent interface to access vehicle data such as location, health status, electric information, and more.

## Projects

- **Posseth.Toyota.Client**: The main client library.
- **Posseth.Toyota.Demo.ConsoleApp**: A console application demonstrating the usage of the client.
- **Posseth.Toyota.Client.Tests**: Unit and integration tests for the client.

## Requirements

- .NET 10
- Toyota Connected Services account

## Installation

Clone the repository and build the solution.

```bash
git clone <repository-url>
cd Posseth.Toyota
dotnet build
```

## Usage

### Console Demo

To run the demo console app:

1. Ensure you have a Toyota Connected Services account.
2. Set your credentials in environment variables or files as described in the console app.
3. Run the console app:

```bash
dotnet run --project Posseth.Toyota.Demo.ConsoleApp
```

### Using the Client Library

```csharp
using Posseth.Toyota.Client.Services;
using Posseth.Toyota.Client.Interfaces;

var client = new MyToyotaClient()
    .UseCredentials("username", "password")
    .UseTimeout(30);

await client.LoginAsync();
var vehicles = await client.GetVehiclesAsync();
// ... use other methods
```

## Testing

The test project includes unit tests for constants and integration tests for API methods.

To run tests:

1. Set environment variables `TOYOTA_USERNAME` and `TOYOTA_PASSWORD` with your credentials.
2. Run:

```bash
dotnet test
```

Note: Integration tests require valid Toyota credentials and will make real API calls.

## Contributing

Please ensure tests pass and follow the code style.

## Credits

This project is based on [Abraham.MyToyotaClient](https://github.com/abraham/MyToyotaClient), which is licensed under the Apache License 2.0. We acknowledge and thank the original author for their work.

## License

MIT License

Copyright (c) [2026] [Michel Posseth A.K.A. MPCoreDeveloper]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## Disclaimer

This is an unofficial client and not affiliated with Toyota. Use at your own risk.