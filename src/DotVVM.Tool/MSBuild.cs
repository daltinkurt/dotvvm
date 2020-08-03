using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.Setup.Configuration;

namespace DotVVM.Tool
{
    public class MSBuild
    {
        public const string DefaultTargetFramework = "netcoreapp3.1";
        public const string VSRelativePath = "./MSBuild/Current/Bin/MSBuild.exe";

        public string Path { get; } = string.Empty;
        public ImmutableArray<string> PrefixedArgs { get; } = ImmutableArray.Create<string>();

        public MSBuild(string path, ImmutableArray<string> prefixedArgs)
        {
            Path = path;
            PrefixedArgs = prefixedArgs;
        }

        public static MSBuild? CreateFromSdk()
        {
            return new MSBuild(
                path: "dotnet",
                prefixedArgs: ImmutableArray.Create("msbuild"));
        }

        public static MSBuild? CreateFromVS()
        {
            try
            {
                var query = new SetupConfiguration();
                var query2 = (ISetupConfiguration2)query;
                var @enum = query2.EnumAllInstances();
                var instances = new ISetupInstance[1];
                int fetchedCount;
                do
                {
                    @enum.Next(1, instances, out fetchedCount);
                    if (fetchedCount > 0)
                    {
                        var instance2 = (ISetupInstance2)instances[0];
                        var path = instance2.GetInstallationPath();
                        var exe = new FileInfo(System.IO.Path.Combine(path, VSRelativePath));
                        if (exe.Exists)
                        {
                            return new MSBuild(exe.FullName, ImmutableArray.Create<string>());
                        }
                    }
                }
                while (fetchedCount > 0);
                return null;
            }
            catch(PlatformNotSupportedException)
            {
                return null;
            }
        }

        public static MSBuild? Create()
        {
            var vs = CreateFromVS();
            if (vs is null) {
                return CreateFromSdk();
            }
            return vs;
        }

        public bool TryBuild(FileInfo project, string configuration, ILogger? logger = null)
        {
            logger ??= NullLogger.Instance;

            var sb = new StringBuilder();
            sb.AppendJoin(' ', PrefixedArgs);
            sb.Append(" -target:Restore,Build");
            sb.Append($" -property:Configuration={configuration}");
            sb.Append($" {project.FullName}");
            var args = sb.ToString();
            logger.LogDebug($"Invoking MSBuild with args: '{args}'.");

            var startInfo = new ProcessStartInfo
            {
                FileName = Path,
                Arguments = args
            };
            var process = Process.Start(startInfo);
            process.WaitForExit();
            return process.ExitCode == 0;
        }

        public override string ToString()
        {
            return $"({Path} {string.Join(' ', PrefixedArgs)})";
        }
    }
}
