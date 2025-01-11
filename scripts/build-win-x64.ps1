dotnet clean
dotnet restore
dotnet publish project\DDO.Launcher\DDO.Launcher.csproj -c Release -r win-x64 --output bin/win-x64 --self-contained -p:TieredCompilation=true -p:PublishReadyToRun=true -p:PublishSingleFile=true