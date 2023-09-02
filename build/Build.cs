using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using System.IO;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
  /// Support plugins are available for:
  ///   - JetBrains ReSharper        https://nuke.build/resharper
  ///   - JetBrains Rider            https://nuke.build/rider
  ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
  ///   - Microsoft VSCode           https://nuke.build/vscode

  public static int Main() => Execute<Build>(x => x.Publish);

  [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
  readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

  [Solution(GenerateProjects = true)] readonly Solution Solution;

  AbsolutePath SourceDirectory => RootDirectory / "src";
  AbsolutePath TestsDirectory => RootDirectory / "tests";
  AbsolutePath OutputDirectory => RootDirectory / "publish";

  Target Clean => _ => _
    .Before(Restore)
    .Executes(() =>
    {
      SourceDirectory.GlobDirectories("**/bin", "**/obj").DeleteDirectories();
      TestsDirectory.GlobDirectories("**/bin", "**/obj").DeleteDirectories();

      OutputDirectory.DeleteDirectory();
    });

  Target Restore => _ => _
    .Executes(() =>
    {
      DotNetRestore(s => s
        .SetProjectFile(Solution));
    });

  Target Compile => _ => _
    .DependsOn(Restore)
    .Executes(() =>
    {
      DotNetBuild(s => s
        .SetProjectFile(Solution)
        .SetConfiguration(Configuration)
        .EnableNoRestore());
    });

  Target Publish => _ => _
    .DependsOn(Compile)
    .Executes(() =>
    {
      DotNetPublish(s => s
        .SetProject(Solution.GetProject("BeanBot"))
        .SetOutput(OutputDirectory)
        .EnableNoBuild()
        );
    });
}
