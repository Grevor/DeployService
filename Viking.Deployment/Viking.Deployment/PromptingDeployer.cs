using System;
using System.Diagnostics;

namespace Viking.Deployment
{
    public delegate string MessageComposer(string tag, Version version);
    public class PromptingDeployer : IDeployer
    {
        private static bool IsRunning => PromptProcess != null;
        private static readonly object mutex = new object();
        private static Process PromptProcess { get; set; }

        private IDeployer Deployer { get; }
        private IPCPipe Pipe { get; }
        private MessageComposer MessageComposer { get; }

        public PromptingDeployer(IDeployer inner, MessageComposer composer)
        {
            Deployer = inner;
            MessageComposer = composer;
            StartPromptProgram();
            Pipe = new IPCPipe(true);
        }

        private void StartPromptProgram()
        {
            lock (mutex)
            {
                if (IsRunning)
                    return;
                PromptProcess = Process.Start("UserPrompt.exe");
            }
        }

        public DeploymentResult Deploy(Version version, string tag)
        {
            Pipe.Send(MessageComposer(tag, version));
            var promptResults = Pipe.ReadBool();
            if (!promptResults)
                return new DeploymentResult(DeploymentResultType.Skipped);
            return Deployer.Deploy(version, tag);
        }

        ~PromptingDeployer()
        {
            PromptProcess.Kill();
        }
    }
}
