# Contributing to WMS

Thank you for considering contributing to this project.

## How to Contribute

1. Fork the repository
2. Create a feature branch from `dev`:
```bash
   git checkout -b feature/your-feature
```
3. Make your changes
4. Run tests before submitting:
```bash
   cd backend
   dotnet test
```
5. Commit using [Conventional Commits](https://www.conventionalcommits.org/):
```
   feat: add your feature
   fix: fix a bug
   docs: update documentation
   refactor: code change without feature or fix
   test: add or update tests
   chore: maintenance tasks
```
6. Push and open a Pull Request against `dev`, never directly against `main`

## PR Requirements

- All CI checks must pass
- Review and approval from the maintainer is required
- Keep PRs focused — one feature or fix per PR

## Reporting Bugs

Open an issue and include:
- What you expected to happen
- What actually happened
- Steps to reproduce
- Your environment (OS, .NET version, DB)

## Feature Requests

Open an issue with a clear description of the feature and the problem it solves.

## Code Style

- Follow existing patterns in the codebase
- All code, comments, and documentation must be in English
- No dead code or commented-out blocks