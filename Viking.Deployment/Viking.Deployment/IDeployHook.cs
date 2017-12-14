namespace Viking.Deployment
{
    public enum DeploymentResultType
    {
        Success,
        Error,
        Skipped
    }

    public struct DeploymentResult
    {
        public string Result { get; }
        public DeploymentResultType Type { get; }

        public DeploymentResult(DeploymentResultType type) : this(type, null) { }
        public DeploymentResult(DeploymentResultType type, string result)
        {
            Type = type;
            Result = result;
        }
    }

    public delegate void DeploymentHook(Version version, string tag);

    public interface IDeployHook
    {
        event DeploymentHook DeploymentReady;
        void Initialize();
    }
}
