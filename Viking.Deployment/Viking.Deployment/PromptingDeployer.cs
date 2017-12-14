using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viking.Deployment
{
    public class PromptingDeployer : IDeployer
    {
        private IDeployer Deployer { get; }
        public DeploymentResult Deploy(Version version, string tag)
        {
            
        }
    }
}
