namespace Viking.Deployment
{
    public interface IDeployer
    {
        DeploymentResult Deploy(Version version, string tag);
    }
}
