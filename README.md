## DDO.Launcher
A DDOnline launcher for private servers.

> [!Warning]
> #### Under Development
> ###### There is no stable version.

## Privacy
DDO.Launcher is an open source project. Your commit credentials as author of a commit will be visible by anyone. Please make sure you understand this before submitting a PR.
Feel free to use a "fake" username and email on your commits by using the following commands:
```bash
git config --local user.name "USERNAME"
git config --local user.email "USERNAME@SOMETHING.com"
```

## Requirements
- .NET 8 SDK or higher.
- Visual Studio Code (recommended) or alternatives.
- [NetCoreDbg](https://github.com/Samsung/netcoredbg) or alternatives.
- [NativeAOT prerequisites](https://aka.ms/nativeaot-prerequisites)

## Build
1. Install Cake: `dotnet tool install Cake.Tool --version 4.0.0` or run `dotnet tool restore` (from `/project/` root).
2. Run `dotnet-cake build.cake` (from `/project/` root).
3. Output will be located in `project/Build/*`.

If you want to build with an icon, you can place an icon in `{root}/project/DDO.Resources.Local/icon.ico`.

Files placed in `{root}/project/DDO.Resources` or `{root}/project/DDO.Resources.Local` will be copied over to `project/Build`.

## Development
1. Install Roslynator Analyzers: `./scripts/setup.ps1` (from `/DDO.Launcher/` root).
    - Alternatively, download and extract the assemblies to `{root}/.nupkg/roslynator/analyzers/dotnet/roslyn4.7/cs/*.dll`
2. [SonarLint](https://www.sonarsource.com/products/sonarlint/) is used for development, it is optional, but recommended.
3. Use or reference `.vscode/tasks.json` for formatting, code analysis, and building.
4. Follow [build steps](#build) for building.

## Platforms

|        | Windows|Linux (Untested)|Mac OS (Untested)|
|--------|--------|----------------|-----------------|
| x86-64 | ✅ | ❌ | ❌ |
| x86    | ❌ | ❌ | ❌ |
| ARM64  | ❌ | ❌ | ❌ |

## Localization (Translations)
All non-machine translation contributions for any languages are welcome.

Contribution Guidelines are not required for translation contributions.

- English [View](./project/DDO.Resources/.ddo_launcher/localization/localization.en.jsonc)

## Contribution Guidelines
If you would like to contribute to `DDO.Launcher` please take the time to carefully read the guidelines below.

### Commit Workflow
- Run `Roslynator` and ensure ALL diagnostics are fixed.
- Run `CSharpier` to ensure consistent formatting.
- Use `git cz` with the [Commitizen CLI](https://github.com/commitizen/cz-cli#conventional-commit-messages-as-a-global-utility) to prepare commit messages.
- Provide *at least* one short sentence or paragraph in your commit message body to describe your thought process for the changes being committed.

### Pull Requests (PRs) should only contain one feature or fix.
It is very difficult to review pull requests which touch multiple unrelated features and parts of the codebase.

Please do not submit pull requests like this; you will be asked to separate them into smaller PRs that deal only with one feature or bug fix at a time.

### Codebase refactors must have prior approval.
Refactors to the structure of the codebase are not taken lightly and require prior discussion and approval.

Please do not start refactoring the codebase with the expectation of having your changes integrated until you receive an explicit approval or a request to do so.

Similarly, when implementing features and bug fixes, please stick to the structure of the codebase as much as possible and do not take this as an opportunity to do some "refactoring along the way".

It is extremely difficult to review PRs for features and bug fixes if they are lost in sweeping changes to the structure of the codebase.

### License
See [LICENSE](./LICENSE) file.