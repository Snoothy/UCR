using System.IO;
using Nuke.Common;
using Nuke.Common.BuildServers;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Tools.Nunit;
using Nuke.Common.Tools.GitVersion;
using Nuke.Core.Tooling;
using ToolSettingsExtensions = Nuke.Common.Tooling.ToolSettingsExtensions;

using static Nuke.Common.Tools.Git.GitTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.GitVersion.GitVersionTasks;
using static Nuke.Common.Tools.Nunit.NunitTasks;
using static Nuke.Compression.CompressionTasks;


class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Test);

    [Parameter] string CodeAnalysis;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;

    string IoWrapper => "IOWrapper";
    AbsolutePath IoWrapperDirectory => RootDirectory / "submodules" / IoWrapper;
    AbsolutePath IoWrapperSolution => IoWrapperDirectory / (IoWrapper + ".sln");
    AbsolutePath UcrOutputDirectory => RootDirectory / "UCR" / "bin" / Configuration;
    AbsolutePath TestDirectory => RootDirectory / "UCR.Tests";

    MSBuildSettings DefaultIoWrapperMSBuild => ToolSettingsExtensions
        .SetWorkingDirectory(DefaultMSBuildCompile, IoWrapperDirectory)
        .SetSolutionFile(IoWrapperSolution)
        .SetVerbosity(MSBuildVerbosity.Quiet);

    Target CleanArtifacts => _ => _
        .Executes(() =>
        {
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Clean => _ => _
        .DependsOn(CleanArtifacts)
        .Executes(() =>
        {
            DeleteDirectories(GlobDirectories(RootDirectory, "**/bin", "**/obj"));
            EnsureCleanDirectory(OutputDirectory);
        });

    Target RestoreSubmodules => _ => _
        .Executes(() =>
        {
            Git("submodule init");
            Git("submodule update");
            NuGetTasks.NuGetRestore(s => s.SetTargetPath(IoWrapperSolution));
        });

    Target CompileSubmodules => _ => _
        .DependsOn(RestoreSubmodules)
        .Executes(() =>
        {
            MSBuild(s => DefaultIoWrapperMSBuild
                .SetTargets("Restore","Rebuild")
                .SetConfiguration(Configuration)
            );
        });

    Target InitProject => _ => _
        .DependsOn(RestoreSubmodules)
        .DependsOn(CompileSubmodules)
        .Executes(() =>
        {
            CopyDirectoryRecursively(IoWrapperDirectory / "Artifacts", SolutionDirectory / "dependencies", FileExistsPolicy.Overwrite);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            NuGetTasks.NuGetRestore(s => s.SetTargetPath(SolutionFile));
            MSBuild(s => s
                .SetTargetPath(SolutionFile)
                .SetTargets("Restore")
            );
        });

    Target StartCodeAnalysis => _ => _
        .OnlyWhen(() => !string.IsNullOrEmpty(CodeAnalysis))
        .Executes(() =>
        {
            DotNet($"sonarscanner begin /k:\"Snoothy_UCR\" /d:sonar.organization=\"snoothy-github\" /d:sonar.host.url=\"https://sonarcloud.io\" /d:sonar.login=\"cad188647aee521b62439577ebe235d6a61e750c\" /v:sonar.projectVersion=\"{GetFullSemanticVersion()}\"");
        });

    Target Versioning => _ => _
        .Executes(() =>
        {
            // TODO Update readme version

            // TODO Update appveyors version
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .DependsOn(StartCodeAnalysis)
        .Executes(() =>
        {
            MSBuild(s => s
                .SetTargetPath(SolutionFile)
                .SetConfiguration(Configuration)
                .SetTargets("Rebuild")
                .SetVerbosity(MSBuildVerbosity.Minimal)
                // TODO This doesn't set all assembly versions
                .SetAssemblyVersion(GetCurrentVersion())
                .SetFileVersion(GetCurrentVersion())
                .SetInformationalVersion(GetFullSemanticVersion())
            );
        });

    Target EndCodeAnalysis => _ => _
    .OnlyWhen(() => !string.IsNullOrEmpty(CodeAnalysis))
    .Executes(() =>
    {
        DotNet($"sonarscanner end /d:sonar.login=\"cad188647aee521b62439577ebe235d6a61e750c\"");
    });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            EnsureExistingDirectory("Providers");
            EnsureExistingDirectory("Plugins");
            Nunit3(s => s
                .AddInputFiles(GlobFiles(TestDirectory, $"**/bin/{Configuration}/UCR.Tests.dll").NotEmpty())
                .EnableNoResults()
            );
        });
    
    Target Artifacts => _ => _
        .DependsOn(CleanArtifacts)
        .DependsOn(EndCodeAnalysis)
        .DependsOn(Test)
        .Executes(() =>
        {
            if (!Directory.Exists(ArtifactsDirectory)) Directory.CreateDirectory(ArtifactsDirectory);
            CopyDirectoryRecursively(UcrOutputDirectory, ArtifactsDirectory, FileExistsPolicy.Overwrite);
            
            CompressZip(ArtifactsDirectory, $"artifacts/UCR_{GetSemanticVersion()}.zip");
        });

    Target Changelog => _ => _
        .DependsOn(Versioning)
        .Executes(() =>
        {
            // TODO keep a change log update
        });

    Target Publish => _ => _
        .DependsOn(Versioning)
        .DependsOn(Artifacts)
        .DependsOn(Changelog)
        .Executes(() =>
        {
            // TODO
        });

    private string GetCurrentVersion()
    {
        // TODO GitVersion
        return "0.4.0";
    }

    private string GetSemanticVersion()
    {
        return $"v{GetCurrentVersion()}";
    }

    private string GetFullSemanticVersion()
    {
        string additionalVersion = "";
        if (AppVeyor.Instance != null) additionalVersion = $"+{AppVeyor.Instance.BuildNumber}";
        return $"v{GetCurrentVersion()}-{GitRepository.Branch}{additionalVersion}";
    }

}
