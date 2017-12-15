using System;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using Viking.Deployment;

namespace Viking.DeploymentService
{
    public partial class Service1 : ServiceBase
    {
        private static string DeploymentHistoryPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Viking", "Deployment", "History.txt");
        private CompositionContainer CompositionContainer { get; }

        private DeploymentModule Module { get; set; }

        public Service1()
        {
            InitializeComponent();
            var catalog = new DirectoryCatalog(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            CompositionContainer = new CompositionContainer(catalog);
        }

        public void StartInteractive() => OnStart(new string[] { });
        public void StopInteractive() => OnStop();

        protected override void OnStart(string[] args)
        {
            var hook = CompositionContainer.GetExportedValue<IDeployHook>();
            var deployer = CompositionContainer.GetExportedValue<IDeployer>();
            Module = new DeploymentModule(hook, deployer, DeploymentHistoryPath);
        }

        protected override void OnStop()
        {
            Module.Stop();
            CompositionContainer.Dispose();
        }
    }
}
