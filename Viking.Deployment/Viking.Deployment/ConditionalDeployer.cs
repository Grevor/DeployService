namespace Viking.Deployment
{
    public delegate bool DeploymentCondition(string tag, Version version);
    public class ConditionalDeployer : IDeployer
    {
        private IDeployer Deployer { get; }
        private DeploymentCondition Condition { get; }

        public ConditionalDeployer(IDeployer deployer, DeploymentCondition condition)
        {
            Deployer = deployer;
            Condition = condition;
        }

        public DeploymentResult Deploy(Version version, string tag)
        {
            if (!Condition(tag, version))
                return new DeploymentResult(DeploymentResultType.Skipped);
            return Deployer.Deploy(version, tag);
        }
    }
}
