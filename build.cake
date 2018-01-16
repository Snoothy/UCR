#tool "nuget:?package=GitReleaseNotes"
#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=gitlink"
#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#addin nuget:?package=Cake.Git
#addin "Cake.FileHelpers"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

var ucrVersion = "0.1.1";
var iowrapperVersion = "v0.2.17";
var outputDir = "./artifacts/";
var dependencyDir = "./dependencies/";
var iowrapperDir = dependencyDir + "IOWrapper";
var iowrapperSolutionPath = iowrapperDir + "/IOWrapper/IOWrapper.sln";
var solutionPath = "./UCR.sln";
var targetPath = "./UCR/bin/" + configuration;
var projectJson = "./UCR/project.json";
var projectCoreJson = "./UCR.Core/project.json";

Task("Clean")
	.Does(() => {
		if (DirectoryExists(outputDir))
		{
			DeleteDirectory(outputDir, recursive:true);
		}
		
		CreateDirectory(outputDir);
	});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
	{
		NuGetRestore(solutionPath);
	});
	
GitVersion versionInfo = null;
Task("Version")
	.Does(() => {
		GitVersion(new GitVersionSettings{
			UpdateAssemblyInfo = true,
			OutputType = GitVersionOutput.BuildServer
		});
		versionInfo = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });
		
		// Set IOWrapper version in README
		ReplaceRegexInFiles("./README.md", @"IOWrapper-v([0-9]+\.[0-9]+\.[0-9]+)-blue.svg", "IOWrapper-" + iowrapperVersion + "-blue.svg");

		// Set UCR release badge in README
		ReplaceRegexInFiles("./README.md", @"release-v([0-9]+\.[0-9]+\.[0-9]+)-blue.svg", "release-v" + ucrVersion + "-blue.svg");
		ReplaceRegexInFiles("./README.md", @"releases/tag/v([0-9]+\.[0-9]+\.[0-9]+)", "releases/tag/v" + ucrVersion);
		
		// Update project.json
		//VersionProject(projectJson, versionInfo);
		//VersionProject(projectCoreJson, versionInfo);
	});

Task("Dependencies")
	.Does(() => {
		CreateDirectory("dependencies");
	});	
	
Task("CloneIOWrapper")
	.IsDependentOn("Dependencies")
	.Does(() => {
		if (!DirectoryExists(iowrapperDir)){
			GitClone("https://github.com/evilC/IOWrapper.git", iowrapperDir);
		}
		
		var filePaths = new FilePath[] { };
		GitCheckout(iowrapperDir, "tags/" + iowrapperVersion, filePaths);
	});	

Task("BuildIOWrapper")
	.IsDependentOn("CloneIOWrapper")
	.Does(() => {
		NuGetRestore(iowrapperSolutionPath);
		
		var msbuildSettings = new MSBuildSettings 
		{
			Verbosity = Verbosity.Minimal,
			ToolVersion = MSBuildToolVersion.VS2015,
			Configuration = configuration,
			PlatformTarget = PlatformTarget.MSIL
		};
		
		msbuildSettings.WithProperty("VersionNumber", ucrVersion + ".0");
		
		MSBuild(iowrapperSolutionPath, msbuildSettings);
	});	

Task("BuiltProviders")
	.IsDependentOn("BuildIOWrapper")
	.Does(() => {
		CopyDirectory(iowrapperDir + "/Built Providers", targetPath + "/Providers");
	});	
	
Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Version")
	.IsDependentOn("Restore")
	.IsDependentOn("BuildIOWrapper")
	.Does(() => {
		MSBuild(solutionPath, new MSBuildSettings 
		{
			Verbosity = Verbosity.Minimal,
			ToolVersion = MSBuildToolVersion.VS2015,
			Configuration = configuration,
			PlatformTarget = PlatformTarget.MSIL
		});
	});

Task("Artifacts")
	.IsDependentOn("Build")
	.IsDependentOn("BuiltProviders")
	.Does(() => {
		CopyDirectory(targetPath, outputDir);
	});
	
Task("Test")
	.IsDependentOn("Build")
	.Does(() => {
		var contextFilePath = "artifacts/context.xml";
		CreateDirectory("artifacts/Providers");
		CreateDirectory("artifacts/Plugins");
		
		NUnit3("./UCR.Tests/**/bin/" + configuration + "/*.Tests.dll", new NUnit3Settings {
			NoResults = true,
			WorkingDirectory = new DirectoryPath("artifacts")
        });
		if (FileExists(contextFilePath)) DeleteFile(contextFilePath);
	});

Task("Package")
	.IsDependentOn("Test")
	.IsDependentOn("Artifacts")
	.Does(() => {
		//GitLink("./", new GitLinkSettings { ArgumentCustomization = args => args.Append("-include Specify,Specify.Autofac") });
        
        //GenerateReleaseNotes();

		//if (AppVeyor.IsRunningOnAppVeyor)
		//{
		//	foreach (var file in GetFiles(outputDir + "**/*"))
		//		AppVeyor.UploadArtifact(file.FullPath);
		//}
	});

private void VersionProject(string projectJsonPath, GitVersion versionInfo)
{
	var updatedProjectJson = System.IO.File.ReadAllText(projectJsonPath)
		.Replace("1.0.0-*", versionInfo.NuGetVersion);
	System.IO.File.WriteAllText(projectJsonPath, updatedProjectJson);
} 

private void GenerateReleaseNotes()
{
	var releaseNotesExitCode = StartProcess(
		@"tools\GitReleaseNotes\tools\gitreleasenotes.exe", 
		new ProcessSettings { Arguments = ". /o artifacts/releasenotes.md" });
	if (string.IsNullOrEmpty(System.IO.File.ReadAllText("./artifacts/releasenotes.md")))
		System.IO.File.WriteAllText("./artifacts/releasenotes.md", "No issues closed since last release");

	if (releaseNotesExitCode != 0) throw new Exception("Failed to generate release notes");
}

Task("Default")
	.IsDependentOn("Package");

RunTarget(target);