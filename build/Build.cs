using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using Nuke.Core.Tooling;
using static Nuke.Common.Tools.Git.GitTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using ToolSettingsExtensions = Nuke.Common.Tooling.ToolSettingsExtensions;

class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Test);

    [Solution] readonly Solution Solution;

    string IoWrapper => "IOWrapper";
    AbsolutePath IoWrapperDirectory => RootDirectory / "submodules" / IoWrapper;
    AbsolutePath IoWrapperSolution => IoWrapperDirectory / (IoWrapper + ".sln");

    MSBuildSettings DefaultIoWrapperMSBuild => ToolSettingsExtensions.SetWorkingDirectory(DefaultMSBuildCompile, IoWrapperDirectory).SetSolutionFile(IoWrapperSolution);


    Target Clean => _ => _
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
            CopyDirectoryRecursively(IoWrapperDirectory / "Artifacts", SolutionDirectory / "dependencies", FileExistsPolicy.Overwrite);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            NuGetTasks.NuGetRestore(s => s.SetTargetPath(SolutionFile));
            MSBuild(s => s
                .SetTargetPath(SolutionFile)
                .SetTargets("Restore")
            );
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
