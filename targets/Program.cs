using System.IO;
using static Bullseye.Targets;
using static SimpleExec.Command;

internal class Program
{
    public static void Main(string[] args)
    {
        var sdk = new DotnetSdkManager();

        Target("default", DependsOn("test"));

        Target("restore",
            Directory.EnumerateFiles("src", "*.csproj", SearchOption.AllDirectories),
            proj => Run(sdk.GetDotnetCliPath(), $"restore \"{proj}\""));
        
        Target("build", DependsOn("restore"),
            Directory.EnumerateFiles("src", "*.sln", SearchOption.AllDirectories),
            solution => Run(sdk.GetDotnetCliPath(), $"msbuild \"{solution}\" -p:Configuration=Release"));

        Target("test", DependsOn("build"),
            Directory.EnumerateFiles("src", "*Tests.csproj", SearchOption.AllDirectories),
            proj => Run(sdk.GetDotnetCliPath(), $"test \"{proj}\" --configuration Release --no-build"));
        
        RunTargets(args);
    }
}
