namespace TinyCms.Core.Infrastructure
{
    /// <summary>
    ///     Interface which should be implemented by tasks run on startup
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        ///     Order
        /// </summary>
        int Order { get; }

        /// <summary>
        ///     Executes a task
        /// </summary>
        void Execute();
    }
}