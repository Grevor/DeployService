using System;
using System.Threading;

namespace Viking.Deployment
{
    public class DeploymentModule
    {
        private IDeployer Deployer { get; }
        private IDeployHook Hook { get; }
        private object Mutex { get; } = new object();
        private VersionHistory History { get; }

        public DeploymentModule(IDeployHook hook, IDeployer deployer, string historyFile)
        {
            Hook = hook;
            Deployer = deployer;
            History = new VersionHistory(historyFile);
            Hook.DeploymentReady += OnAttemptDeploy;
            Hook.Initialize();
        }

        private void OnAttemptDeploy(Version version, string tag)
        {
            lock (Mutex)
            {
                try
                {
                    var currentVersion = History.GetLatestVersion(tag);
                    if (currentVersion >= version)
                        return;
                    var result = Deployer.Deploy(version, tag);
                    // Just return if there was an error.
                    if (result.Type == DeploymentResultType.Error)
                        return;
                    History.SetVersion(tag, version);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public void Stop()
        {
            Monitor.Enter(Mutex);
        }
    }
}
