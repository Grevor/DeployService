using System;
using System.ComponentModel.Composition;
using Viking.Deployment;

namespace TestLibrary
{
    [Export(typeof(IDeployer))]
    [Export(typeof(IDeployHook))]
    public class Class1 : IDeployer, IDeployHook
    {
        private IDeployer Deployer { get; }
        private IDeployHook Hook { get; }
        public event DeploymentHook DeploymentReady
        {
            add { Hook.DeploymentReady += value; }
            remove { Hook.DeploymentReady -= value; }
        }

        [ImportingConstructor]
        public Class1()
        {
            var fileDeployer = new FileScannerDeployer(@"C:\Temp\Deployment", TimeSpan.FromMinutes(1));
            var promptDeployer = new PromptingDeployer(fileDeployer, ComposeMessage);
            var conditionalDeployer = new ConditionalDeployer(promptDeployer, Condition);
            Deployer = conditionalDeployer;
            Hook = fileDeployer;
        }

        private bool Condition(string tag, Viking.Deployment.Version version) => true;
        private string ComposeMessage(string tag, Viking.Deployment.Version version) => $"Install '{tag} {version}'?";
        public DeploymentResult Deploy(Viking.Deployment.Version version, string tag) => Deployer.Deploy(version, tag);

        public void Initialize() => Hook.Initialize();
    }
}
