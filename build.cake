//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////
#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
//////////////////////////////////////////////////////////////////////
var artifactsDir = "./artifacts";
var localPackagesDir = "../LocalPackages";
var nugetVersion = string.Empty;
GitVersion gitVersionInfo;

Task("GetVersion")
    .Does(() => 
    {
        gitVersionInfo = GitVersion(new GitVersionSettings {
            OutputType = GitVersionOutput.Json
        });
        nugetVersion = gitVersionInfo.NuGetVersion;

        if(BuildSystem.IsRunningOnTeamCity)
            BuildSystem.TeamCity.SetBuildNumber(nugetVersion);

        Information($"Building Seq.App.VictorOps v{0}", nugetVersion);
        Information("Informational Version {0}", gitVersionInfo.InformationalVersion);
        Verbose("GitVersion:\n{0}", gitVersionInfo);
    });

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDir);
        CleanDirectories("./source/**/bin");
        CleanDirectories("./source/**/obj");
    });

Task("Restore")
    .IsDependentOn("Clean")
    .IsDependentOn("GetVersion")
    .Does(() => DotNetCoreRestore("source", new DotNetCoreRestoreSettings
        {
            ArgumentCustomization = args => args.Append($"/p:Version={nugetVersion}")
        }));

Task("Build")
    .IsDependentOn("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreBuild("./source", new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            ArgumentCustomization = args => args.Append($"/p:Version={nugetVersion} --no-restore")
        });
    });

Task("Pack")
    .IsDependentOn("Build")
    .Does(() => 
    {
        DotNetCorePack(
            "./source/Seq.App.VictorOps",
            new DotNetCorePackSettings 
            {
                Configuration = configuration,
                OutputDirectory = artifactsDir,
                ArgumentCustomization = args => args.Append($"/p:Version={nugetVersion}")
            }
        );
    });

Task("PushPackage")
    .WithCriteria(BuildSystem.IsRunningOnTeamCity)
    .IsDependentOn("Pack")
    .Does(() =>
    {
        NuGetPush($"{artifactsDir}/Seq.App.VictorOps.{nugetVersion}.nupkg", new NuGetPushSettings {
            Source = "https://f.feedz.io/octopus-deploy/dependencies/nuget",
            ApiKey = EnvironmentVariable("FeedzIoApiKey")
        });
    });

Task("CopyToLocalPackages")
    .WithCriteria(BuildSystem.IsLocalBuild)
    .IsDependentOn("Pack")
    .Does(() =>
{
    CreateDirectory(localPackagesDir);
    CopyFileToDirectory($"{artifactsDir}/Seq.App.VictorOps.{nugetVersion}.nupkg", localPackagesDir);
});

Task("Push")
    .IsDependentOn("PushPackage")
    .IsDependentOn("CopyToLocalPackages");


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
Task("Default")
    .IsDependentOn("Push");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(target);