namespace TinyCms.Services.Tasks
{
    /// <summary>
    ///     Interface that should be implemented by each task
    /// </summary>
    public interface ITask
    {
        /// <summary>
        ///     Executes a task
        /// </summary>
        void Execute();
    }
}