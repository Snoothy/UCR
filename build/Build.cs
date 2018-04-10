using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Gitter;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using Nuke.Core;
using Nuke.Core.Tooling;
using static Nuke.Common.Tools.Git.GitTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Core.IO.FileSystemTasks;
using static Nuke.Core.IO.PathConstruction;
using static Nuke.Core.EnvironmentInfo;

class Build : NukeBuild
{
    // Console application entry. Also defines the default target.
    public static int Main() => Execute<Build>(x => x.Test);

    // Auto-injection fields:

    // [GitVersion] readonly GitVersion GitVersion;
    // Semantic versioning. Must have 'GitVersion.CommandLine' referenced.

    // [GitRepository] readonly GitRepository GitRepository;
    // Parses origin, branch name and head from git config.

    // [Parameter] readonly string MyGetApiKey;
    // Returns command-line arguments and environment variables.

    string IoWrapper => "IOWrapper";
    AbsolutePath IoWrapperDirectory => RootDirectory / "submodules" / IoWrapper;
    AbsolutePath IoWrapperSolution => IoWrapperDirectory / (IoWrapper + ".sln");

    MSBuildSettings DefaultIoWrapperMSBuild => DefaultMSBuild
        .SetWorkingDirectory(IoWrapperDirectory)
        .SetSolutionFile(IoWrapperSolution);


    Target Clean => _ => _
        .OnlyWhen(() => false) // Disabled for safety.
        .Executes(() =>
        {
            DeleteDirectories(GlobDirectories(SourceDirectory, "**/bin", "**/obj"));
            EnsureCleanDirectory(OutputDirectory);
        });

    Target RestoreSubmodules => _ => _
        .Executes(() =>
        {
            Git("submodule init");
            Git("submodule update");
            MSBuild(s => DefaultIoWrapperMSBuild
                .SetTargets("Restore")
            );
            NuGetTasks.NuGetRestore(s => s.SetTargetPath(IoWrapperSolution));
        });

    Target CompileSubmodules => _ => _
        .DependsOn(RestoreSubmodules)
        .Executes(() =>
        {
            MSBuild(s => DefaultIoWrapperMSBuild
                .SetTargets("Rebuild")
            );
        });

    Target InitProject => _ => _
        .DependsOn(RestoreSubmodules)
        .DependsOn(CompileSubmodules)
        .Executes(() =>
    {
        CopyRecursively(IoWrapperDirectory / "Artifacts", SolutionDirectory / "dependencies", FileExistsPolicy.Overwrite);
    });

    Target Restore => _ => _
            .DependsOn(Clean)
            .Executes(() =>
            {
                MSBuild(s => DefaultMSBuildRestore);
            });

    Target Compile => _ => _
            .DependsOn(Restore)
            .Executes(() =>
            {
                MSBuild(s => DefaultMSBuildCompile);
            });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            MSBuild(s => DefaultMSBuildCompile);
        });
}
