# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| latest  | :white_check_mark: |

## Reporting a Vulnerability

If you discover a security vulnerability in this project, please report it responsibly.

**Do NOT open a public GitHub issue for security vulnerabilities.**

Instead, please send an email to **abuse@posseth.com** with:

- A description of the vulnerability
- Steps to reproduce the issue
- Any potential impact
- Suggested fix (if you have one)

### What to Expect

- **Acknowledgment** within 48 hours of your report
- **Assessment** of the vulnerability within 1 week
- **Fix and disclosure** coordinated with you

## Security Considerations

This library handles authentication tokens for the Toyota Connected Services API. Contributors and users should be aware of:

- **Never commit credentials** — Use environment variables or secure file storage
- **Token caching** — Cached tokens contain access and refresh tokens. The default cache file (`toyota_credentials_cache_contains_secrets.json`) should be added to `.gitignore`
- **SSL validation** — The client has an option to bypass SSL validation for development. Never use this in production
- **API keys** — The `config.json` contains the Toyota API key. This is a publicly known key used by the Toyota OneApp mobile application

## Best Practices for Users

1. Store credentials in environment variables, not in code
2. Add token cache files to `.gitignore`
3. Use short-lived tokens when possible
4. Do not log full access tokens in production
5. Keep the library updated to the latest version
