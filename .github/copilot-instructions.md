# TypedRest for .NET - Copilot Instructions

## Repository Overview

TypedRest is a .NET library for building type-safe, fluent-style REST API clients. The repository is a medium-sized C# codebase (~200 source files, ~33MB in src/) targeting multiple .NET platforms: netstandard2.0, netstandard2.1, .NET 8, .NET 9, and .NET 10.

**Key Projects:**
- `TypedRest` - Main library for REST API clients
- `TypedRest.Reactive` - ReactiveX (Rx) streaming support
- `TypedRest.SystemTextJson` - System.Text.Json serialization alternative
- `TypedRest.OAuth` - OAuth 2.0/OpenID Connect authentication
- `TypedRest.CommandLine` - CLI builder for TypedRest clients
- `UnitTests` - Unit tests using xUnit, FluentAssertions, and Moq

## Building and Testing

### Prerequisites
- .NET SDK 10.0+ is required (version 10.0.101 or later)
- The repository uses 0install scripts (`0install.sh`/`0install.ps1`) to auto-download .NET SDK if missing

### Build Commands

**IMPORTANT:** Always build from the `src/` directory, not the repository root.

```bash
cd src
dotnet msbuild -v:Quiet -t:Restore -t:Build -p:Configuration=Release -p:Version=1.0.0-dev
```

- Build takes approximately 13 seconds on a clean build
- Artifacts are placed in `artifacts/Release/` (NuGet packages and assemblies for all target frameworks)
- Use the version parameter to set package version (e.g., `1.0.0-dev`)
- Use `-p:ContinuousIntegrationBuild=True` for deterministic builds in CI

**Alternative:** Use root build script:
```bash
./build.sh 1.0.0-dev  # Runs src/build.sh, src/test.sh, and doc/build.sh
```

### Test Commands

**IMPORTANT:** Tests must be run from the `src/` directory.

```bash
cd src
dotnet test --no-build --logger trx --configuration Release --framework net10.0 UnitTests/UnitTests.csproj
```

- Tests complete in approximately 1 second (114 tests)
- Test results are written to `src/UnitTests/TestResults/*.trx`
- The `--no-build` flag is safe to use because binaries persist after `dotnet clean`
- Unit tests target both .NET Framework 4.8 (`net48`) and .NET 10 (`net10.0`), but CI only runs net10.0 tests

**Note:** If you delete `src/*/bin` and `src/*/obj` directories manually, you must rebuild before running tests.

### Documentation Build

```bash
cd doc
dotnet tool restore  # Installs docfx 2.78.4 (defined in .config/dotnet-tools.json)
dotnet docfx --logLevel=warning --warningsAsErrors docfx.json
```

- Removes `doc/api/` directory and regenerates it
- Outputs to `artifacts/Documentation/`
- Uses default, modern, and custom templates

### Clean Build

To perform a completely clean build:
```bash
cd src
rm -rf */bin */obj
dotnet msbuild -v:Quiet -t:Restore -t:Build -p:Configuration=Release -p:Version=1.0.0-dev
```

Note: `dotnet clean` does NOT fully clean the build outputs - binaries remain in `bin/` directories.

## Project Structure

### Root Files
- `build.sh` / `build.ps1` - Orchestrates build, test, and doc generation
- `0install.sh` / `0install.ps1` - Auto-downloads .NET SDK if missing
- `GitVersion.yml` - Version strategy configuration (ContinuousDeployment mode)
- `.editorconfig` - Code style configuration (4-space indent, LF line endings, extensive C# formatting rules)
- `renovate.json` - Dependency update automation configuration

### Source Layout (`src/`)
- `Directory.Build.props` - Shared MSBuild configuration:
  - Multi-targeting: netstandard2.0, netstandard2.1, net8.0, net9.0, net10.0
  - C# 13.0, implicit usings enabled
  - Treats warnings as errors
  - Generates NuGet packages on build with symbols (`.snupkg`)
  
- `TypedRest/` - Main library
  - `Endpoints/` - Core endpoint implementations (collection, element, action, etc.)
  - `Http/` - HTTP client extensions and helpers
  - `Serializers/` - JSON serialization support
  - `Errors/` - Exception types
  - `Links/` - Link handling

- `UnitTests/` - xUnit tests with FluentAssertions, Moq, and MockHttp

### Configuration Files
- `.config/dotnet-tools.json` - Defines docfx 2.78.4 tool
- `doc/docfx.json` - API documentation generation configuration
- `src/TypedRest.slnx` - Solution file (XML-based .slnx format)

## CI/CD Pipeline

### GitHub Actions Workflow (`.github/workflows/build.yml`)

**Triggers:** Push and pull request events

**Build Process:**
1. Checkout with full history (`fetch-depth: 0`)
2. Setup GitVersion 5.12.x
3. Setup .NET SDK 10.0.x
4. Build with version from GitVersion:
   - Pre-release: `./build.sh {version}-{shortSha}`
   - Release (tags): `./build.sh {version}`
5. Report test results using `dorny/test-reporter@v2` with `dotnet-trx` format from `src/UnitTests/TestResults/*.trx`

**Release Process (tags only):**
1. Create GitHub Release with tag message
2. Publish docs to GitHub Pages at `dotnet.typedrest.net`
3. Publish NuGet packages to GitHub Packages (all pushes except renovate branches)
4. Publish to NuGet.org (tags only)

**Expected Test Results Location:** `src/UnitTests/TestResults/*.trx`

## Code Style and Conventions

### EditorConfig Rules
- **Indentation:** 4 spaces for C#, 2 spaces for JSON/XML/project files
- **Line endings:** LF (Unix-style), except .bat/.cmd files (CRLF)
- **Encoding:** UTF-8
- **C# Style:**
  - Braces on new lines (Allman style)
  - Expression-bodied members for methods, properties, accessors
  - Pattern matching preferred over `as` with null checks
  - Object and collection initializers preferred
  - `var` only when type is apparent
  - No `this.` qualification

### Naming Conventions
- Follow standard .NET conventions
- Use PascalCase for public members
- Prefer predefined types (`int`, `string`) over framework types

### Documentation
- XML documentation is generated for all public APIs
- Missing XML comments are suppressed (NoWarn 1591)
- Namespace documentation in `_Namespace.md` files

## Common Pitfalls and Solutions

### 1. Build Location
**Problem:** Building from repository root fails or produces unexpected results.
**Solution:** Always `cd src` before running `dotnet` build or test commands.

### 2. Test Results Location
**Problem:** CI cannot find test results.
**Solution:** Ensure tests write to `src/UnitTests/TestResults/*.trx` using `--logger trx` flag.

### 3. Clean Builds
**Problem:** `dotnet clean` doesn't fully clean binaries.
**Solution:** Manually delete `src/*/bin` and `src/*/obj` directories for a complete clean.

### 4. Multi-Targeting
**Problem:** Changes work on one target framework but fail on others.
**Solution:** Test all target frameworks. Use conditional compilation or framework-specific code when necessary.

### 5. NuGet Package Version
**Problem:** Local builds produce packages with incorrect versions.
**Solution:** Always specify version: `dotnet msbuild ... -p:Version=1.0.0-dev`

### 6. Documentation Build
**Problem:** docfx tool not found.
**Solution:** Run `dotnet tool restore` from the `doc/` directory first.

## Directory Artifacts

### Generated Directories (In .gitignore)
- `artifacts/` - Build outputs (NuGet packages, documentation)
- `src/.vs/`, `src/.idea/` - IDE caches
- `src/_ReSharper.*/` - ReSharper cache
- `src/*/obj/`, `src/*/bin/` - Build intermediates and outputs
- `doc/obj/`, `doc/api/` - Documentation intermediates
- `**/TestResults/` - Test result files

## Trust These Instructions

**These instructions are comprehensive and tested.** Only search the codebase if:
- You need to understand specific implementation details
- These instructions are incomplete for your task
- You encounter an error not documented here
- You need to locate specific code to modify

For standard build, test, and validation workflows, follow these instructions exactly as written.
