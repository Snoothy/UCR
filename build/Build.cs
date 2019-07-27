using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Nuke.Common;
using Nuke.Common.BuildServers;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Tools.NUnit;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tooling;

using static Nuke.Common.Tools.Git.GitTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.ChangeLog.ChangelogTasks;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.GitVersion.GitVersionTasks;
using static Nuke.Common.Tools.NUnit.NUnitTasks;
using static Nuke.Common.IO.CompressionTasks;


class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Test);

    [Parameter] string Configuration { get; } = IsLocalBuild ? "Debug" : "Release";
    [Parameter] string CodeAnalysis;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    string ChangeLogFile => RootDirectory / "CHANGELOG.md";

    string IoWrapper => "IOWrapper";
    AbsolutePath IoWrapperDirectory => RootDirectory / "submodules" / IoWrapper;
    AbsolutePath IoWrapperSolution => IoWrapperDirectory / (IoWrapper + ".sln");
    AbsolutePath UcrOutputDirectory => RootDirectory / "UCR" / "bin" / Configuration;
    AbsolutePath TestDirectory => RootDirectory / "UCR.Tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath DependencyDirectory => Solution.Directory / "dependencies";

    Target CleanArtifacts => _ => _
        .Executes(() =>
        {
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Clean => _ => _
        .DependsOn(CleanArtifacts)
        .Executes(() =>
        {
            var outputDirectories = GlobDirectories(RootDirectory, "**/bin", "**/obj").ToList();
            outputDirectories.RemoveAll(s => s.StartsWith(RootDirectory / "build"));
            outputDirectories.ForEach(DeleteDirectory);

            EnsureExistingDirectory(RootDirectory / "Providers");
            EnsureExistingDirectory(RootDirectory / "Plugins");
        });

    Target InitSubmodules => _ => _
        .Executes(() =>
        {
            Git("submodule init");
            Git("submodule update");
        });

    Target RestoreSubmodules => _ => _
        .Executes(() =>
        {
            NuGetTasks.NuGetRestore(s => s.SetTargetPath(IoWrapperSolution));
        });

    Target CompileSubmodules => _ => _
        .DependsOn(RestoreSubmodules)
        .Executes(() =>
        {
            MSBuild(s => s
                    .SetWorkingDirectory(IoWrapperDirectory)
                    .SetSolutionFile(IoWrapperSolution)
                    .SetVerbosity(MSBuildVerbosity.Quiet)
                    .SetTargets("Restore","Rebuild")
                    .SetConfiguration(Configuration)
            );

            EnsureCleanDirectory(DependencyDirectory);
            CopyDirectoryRecursively(IoWrapperDirectory / "Artifacts", DependencyDirectory, DirectoryExistsPolicy.Merge);
        });

    Target InitProject => _ => _
        .DependsOn(InitSubmodules)
        .DependsOn(RestoreSubmodules)
        .DependsOn(CompileSubmodules)
        .Executes(() =>
        {
            
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            NuGetTasks.NuGetRestore(s => s.SetTargetPath(Solution));
            MSBuild(s => s
                .SetTargetPath(Solution)
                .SetTargets("Restore")
            );
        });

    Target StartCodeAnalysis => _ => _
        .OnlyWhenStatic(() => !string.IsNullOrEmpty(CodeAnalysis))
        .Executes(() =>
        {
            var sonarScanner =
                $"sonarscanner begin /k:\"Snoothy_UCR\" /o:\"snoothy-github\" /d:sonar.host.url=\"https://sonarcloud.io\" /d:sonar.login=\"cad188647aee521b62439577ebe235d6a61e750c\" /v:\"{GetFullSemanticVersion()}\" ";
            if (AppVeyor.Instance != null && AppVeyor.Instance.PullRequestNumber != 0)
            {
                DotNet(sonarScanner + $"/d:sonar.pullrequest.provider=GitHub /d:sonar.pullrequest.base=develop /d:sonar.pullrequest.github.repository=\"Snoothy/UCR\" /d:sonar.pullrequest.branch=\"{GitRepository.Branch}\" /d:sonar.pullrequest.key={AppVeyor.Instance.PullRequestNumber}");
            }
            else
            {
                if (GitRepository.Branch == "master")
                {
                    DotNet(sonarScanner + $"/d:sonar.branch.name=\"{GitRepository.Branch}\"");
                }
                else
                {
                    DotNet(sonarScanner + $"/d:sonar.branch.name=\"{GitRepository.Branch}\" /d:sonar.branch.target=\"develop\"");
                }
            }
        });

    Target Versioning => _ => _
        .Executes(() =>
        {
            const string readmePath = "./README.md";
            File.WriteAllText(readmePath, Regex.Replace(File.ReadAllText(readmePath), @"release-v([0-9]+\.[0-9]+\.[0-9]+)-blue.svg", $"release-v{GitVersion.MajorMinorPatch}-blue.svg"));
            File.WriteAllText(readmePath, Regex.Replace(File.ReadAllText(readmePath), @"releases/tag/v([0-9]+\.[0-9]+\.[0-9]+)", $"releases/tag/v{GitVersion.MajorMinorPatch}"));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .DependsOn(StartCodeAnalysis)
        .Executes(() =>
        {
            MSBuild(s => s
                .SetTargetPath(Solution)
                .SetTargets("Rebuild")
                .SetConfiguration(Configuration)
                .SetVerbosity(MSBuildVerbosity.Normal)
                // TODO This doesn't set all assembly versions
                .SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
                .SetFileVersion(GitVersion.GetNormalizedFileVersion())
                .SetInformationalVersion(GetFullSemanticVersion())
            );
        });

    Target EndCodeAnalysis => _ => _ 
    .OnlyWhenStatic(() => !string.IsNullOrEmpty(CodeAnalysis))
    .Executes(() =>
    {
        DotNet($"sonarscanner end /d:sonar.login=\"cad188647aee521b62439577ebe235d6a61e750c\"");
    });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            NUnit3(s => s
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
            EnsureDirectory(ArtifactsDirectory);
            CopyDirectoryRecursively(UcrOutputDirectory, ArtifactsDirectory, DirectoryExistsPolicy.Merge);
            
            CompressZip(ArtifactsDirectory, $"artifacts/UCR_{GetFullSemanticVersion()}.zip");
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

    string GetCurrentVersion()
    {
        return GitVersion.MajorMinorPatch;
    }

    string GetFullSemanticVersion()
    {
        var additionalVersion = "";
        if (AppVeyor.Instance != null) additionalVersion = $"+{AppVeyor.Instance.BuildNumber}";

        return $"v{GetCurrentVersion()}-{GitRepository.Branch}{additionalVersion}".Replace('/', '.');
    }

    void EnsureDirectory(AbsolutePath path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }
}
