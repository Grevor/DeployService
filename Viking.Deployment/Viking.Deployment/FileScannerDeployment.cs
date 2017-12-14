using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viking.Deployment
{
    public class FileScannerDeployment : IDeployer, IDeployHook
    {
        public string Folder { get; }
        private FileSystemWatcher Watcher { get; }
        private TimeSpan GracePeriod { get; }

        public event DeploymentHook DeploymentReady;

        public FileScannerDeployment()
        {
            Watcher = new FileSystemWatcher(Folder)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents=true,
            };
            Watcher.Created += Created;
        }

        private void Created(object sender, FileSystemEventArgs e)
        {
            var path = e.FullPath;
            if (!Directory.Exists(path))
                path = Path.GetDirectoryName(path);
            var vs = Directory.GetParent(path);
            var version = vs.Name;
            var tags = vs.Parent.Name;
            DeploymentReady?.Invoke(new Version(version), tags);
        }

        public DeploymentResult Deploy(Version version, string tag)
        {
            var path = Path.Combine(Folder, tag, version.ToString());
            foreach(var exe in Directory.EnumerateFiles(path, "*.exe"))
            {
                var proc = new Process()
                {
                    StartInfo = new ProcessStartInfo(Path.GetFullPath(exe))
                    {
                        
                    }
                };
                proc.WaitForExit();
                if (proc.ExitCode != 0)
                    return new DeploymentResult(DeploymentResultType.Error);
            }
            return new DeploymentResult(DeploymentResultType.Success);
        }

        public void Initialize()
        {
        }
    }
}
