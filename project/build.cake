string target = Argument<string>("target", "ExecuteBuild");
string config = Argument<string>("config", "Release");
bool VSBuilt = Argument<bool>("vsbuilt", false);

// Cake API Reference: https://cakebuild.net/dsl/
// setup variables
var buildDir = "./Build";
var csprojPaths = GetFiles("./**/DDO.*(Launcher|Launcher.Cli|ModManager|ModManager.Cli).csproj");
var externCsprojPaths = GetFiles("../extern/**/*(CodeAnalyzers).csproj");
var toolsCsprojPaths = GetFiles("../tools/**/*(Diff).csproj");
var delPaths = GetDirectories("./**/*(obj|bin)");
var licenseFile = "../LICENSE";
var publishRuntime = "win-x64";
// var launcherDebugFolders = GetDirectories("./DDO.Resources/*(.DDO|development)");
var launcherDebugFolders = GetDirectories("./*(DDO.Resources|DDO.Resources.Local)");

// Clean build directory and remove obj / bin folder from projects
Task("Clean")
    .WithCriteria(!VSBuilt)
    .Does(() =>
    {
        CleanDirectory(buildDir);
    })
    .DoesForEach(delPaths, (directoryPath) =>
    {
        Information("Found directory: {0}", directoryPath);
        DeleteDirectory(directoryPath, new DeleteDirectorySettings
        {
            Recursive = true,
            Force = true
        });
    });

// Restore, build, and publish selected csproj files
Task("Publish")
    .IsDependentOn("Clean")
    .DoesForEach(externCsprojPaths, (externCsprojFile) => 
    {
        DotNetPublish(externCsprojFile.FullPath, new DotNetPublishSettings 
        {
            NoLogo = true,
            Configuration = config,
            Runtime = publishRuntime,
            // PublishSingleFile = true,
            SelfContained = false,
            OutputDirectory = buildDir
        });
    })
    .DoesForEach(toolsCsprojPaths, (toolsCsprojFile) => 
    {
        DotNetPublish(toolsCsprojFile.FullPath, new DotNetPublishSettings 
        {
            NoLogo = true,
            Configuration = config,
            Runtime = publishRuntime,
            // PublishSingleFile = true,
            SelfContained = false,
            OutputDirectory = buildDir
        });
    })
    .DoesForEach(csprojPaths, (csprojFile) => 
    {
        DotNetPublish(csprojFile.FullPath, new DotNetPublishSettings 
        {
            NoLogo = true,
            Configuration = config,
            Runtime = publishRuntime,
            // PublishSingleFile = true,
            SelfContained = false,
            OutputDirectory = buildDir
        });
    });

// Copy license to build directory
Task("CopyBuildData")
    .IsDependentOn("Publish")
    .DoesForEach(launcherDebugFolders, (launcherDebugFolder) => 
    {
        if (DirectoryExists(launcherDebugFolder))
        {
            Information(launcherDebugFolder);
            CopyDirectory(launcherDebugFolder, buildDir);
        }
    });

// Remove pdb files from build if running in release configuration
Task("RemovePDBs")
    .WithCriteria(config == "Release")
    .IsDependentOn("CopyBuildData")
    .Does(() => 
    {
        DeleteFiles($"{buildDir}/*.pdb");
    });

Task("CopyLicense")
    .IsDependentOn("RemovePDBs")
    .Does(() => 
    {
        CopyFile(licenseFile, $"{buildDir}/LICENSE.txt");
    });

// Runs all build tasks based on dependency and configuration
Task("ExecuteBuild")
    .IsDependentOn("CopyBuildData")
    .IsDependentOn("CopyLicense");

// Runs target task
RunTarget(target);