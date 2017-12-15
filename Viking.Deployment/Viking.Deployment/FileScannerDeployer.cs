using System;
using System.Diagnostics;
using System.IO;
using System.Timers;

namespace Viking.Deployment
{
    public class FileScannerDeployer : IDeployer, IDeployHook
    {
        public string Folder { get; }
        private FileSystemWatcher Watcher { get; }
        private TimeSpan GracePeriod { get; }
        private Timer GraceTimer { get; } = new Timer();

        private object Mutex { get; } = new object();
        private Version Version { get; set; }
        private string Tag { get; set; }

        public event DeploymentHook DeploymentReady;

        public FileScannerDeployer(string folder, TimeSpan gracePeriod)
        {
            Folder = folder;
            GracePeriod = gracePeriod;
            Watcher = new FileSystemWatcher(Folder)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents=true,
            };
            Watcher.Created += Created;
            GraceTimer.Enabled = false;
            GraceTimer.AutoReset = false;
            GraceTimer.Interval = GracePeriod.TotalSeconds;
            GraceTimer.Elapsed += Send;
        }

        private void Send(object sender, ElapsedEventArgs e)
        {
            lock (Mutex)
            {
                GraceTimer.Stop();
                DeploymentReady?.Invoke(Version, Tag);
            }
        }

        private void Created(object sender, FileSystemEventArgs e)
        {
            var path = e.FullPath;
            if (!Directory.Exists(path))
                path = Path.GetDirectoryName(path);
            var vs = new DirectoryInfo(path);
            var version = vs.Name;
            var tags = vs.Parent.Name;
            if (!vs.Parent.Parent.FullName.Equals(Folder))
                return;
            lock (Mutex)
            {
                Version = new Version(version);
                Tag = tags;
                if (GraceTimer.Enabled)
                    return;
                GraceTimer.Start();
            }
        }

        public DeploymentResult Deploy(Version version, string tag)
        {
            var path = Path.Combine(Folder, tag, version.ToString());
            foreach(var exe in Directory.EnumerateFiles(path, "*.exe"))
            {
                var proc = new Process()
                {
                    StartInfo = new ProcessStartInfo(Path.GetFullPath(exe))
                };
                proc.WaitForExit();
                if (proc.ExitCode != 0)
                    return new DeploymentResult(DeploymentResultType.Error);
            }
            return new DeploymentResult(DeploymentResultType.Success);
        }

        public void Initialize()
        {
            Watcher.EnableRaisingEvents = true;
        }
    }
}
