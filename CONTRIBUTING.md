# Contributing to Posseth.Toyota.Client

First off, thank you for considering contributing! Every contribution helps make this project better for the Toyota Connected Services community.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [How to Contribute](#how-to-contribute)
- [Pull Request Process](#pull-request-process)
- [Coding Standards](#coding-standards)
- [Reporting Bugs](#reporting-bugs)
- [Suggesting Features](#suggesting-features)

## Code of Conduct

This project adheres to the [Contributor Covenant Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code. Please report unacceptable behavior to abuse@posseth.com.

## Getting Started

1. Fork the repository
2. Clone your fork locally
3. Create a branch for your changes
4. Make your changes
5. Submit a pull request

## Development Setup

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2026, VS Code, or JetBrains Rider

### Building

```bash
git clone https://github.com/<your-username>/Posseth.Toyota.Client.git
cd Posseth.Toyota.Client
dotnet build src/Posseth.Toyota.Client/Posseth.Toyota.Client.sln
```

### Running Tests

Unit tests (no credentials required):

```bash
dotnet test --filter "MethodName~IsCorrect|MethodName~HasExpectedValues"
```

Integration tests (requires Toyota credentials):

```bash
$env:TOYOTA_USERNAME = "your_username"
$env:TOYOTA_PASSWORD = "your_password"
dotnet test
```

> **Note:** Integration tests make real API calls to Toyota Connected Services. Use them responsibly and do not run them in CI without appropriate credentials.

## How to Contribute

### Types of Contributions

- **Bug fixes** — Fix an issue and submit a PR
- **New API endpoints** — Add support for additional Toyota API endpoints
- **Response models** — Improve or correct API response model mappings
- **Documentation** — Improve docs, add examples, fix typos
- **Tests** — Add unit tests or improve test coverage

### What We're Looking For

- New Toyota Connected Services API endpoints (see `config.json` for the endpoint pattern)
- Better response model coverage (many payloads use `object?` and could be strongly typed)
- Error handling improvements
- Performance optimizations for high-frequency polling scenarios

## Pull Request Process

1. **Create an issue first** for non-trivial changes to discuss the approach
2. **Fork & branch** — Create a feature branch from `master`
3. **Keep it focused** — One PR per feature or fix
4. **Follow conventions** — Match the existing code style
5. **Add tests** — All new public API methods need at least one test
6. **Update docs** — Update `docs/api-reference.md` if you add new methods
7. **Ensure build passes** — Run `dotnet build` and `dotnet test` before submitting
8. **Write a clear description** — Explain what, why, and how

### PR Checklist

- [ ] Code compiles without warnings
- [ ] New/changed public methods have XML documentation
- [ ] Tests added for new functionality
- [ ] `docs/api-reference.md` updated if API surface changed
- [ ] No secrets, credentials, or personal data in the code
- [ ] Follows existing naming conventions

## Coding Standards

- **Target Framework:** .NET 10 / C# 14
- **Async all the way** — All I/O methods must be async with `Async` suffix
- **CancellationToken** — All async methods must accept `CancellationToken`
- **Nullable reference types** — Enabled; use `?` for nullable types
- **Naming:**
  - Methods: `PascalCase`, async methods end with `Async`
  - Private fields: `_camelCase`
  - Constants: `UPPER_SNAKE_CASE` (matching existing convention)
- **Response models** — Follow the existing pattern in `Models/ResponseModels.cs`
- **No breaking changes** — To the `IMyToyotaClient` interface without discussion

## Reporting Bugs

Use the [Bug Report](https://github.com/MPCoreDeveloper/Posseth.Toyota.Client/issues/new?template=bug_report.md) template and include:

- .NET version (`dotnet --info`)
- Steps to reproduce
- Expected vs. actual behavior
- Relevant logs (with credentials redacted)

## Suggesting Features

Use the [Feature Request](https://github.com/MPCoreDeveloper/Posseth.Toyota.Client/issues/new?template=feature_request.md) template and include:

- Description of the feature
- Use case / motivation
- If it's a new API endpoint, include any documentation or references you have

## License

By contributing, you agree that your contributions will be licensed under the [MIT License](LICENSE).
