using System;
using Autofac;
using TinyCms.Core.Configuration;
using TinyCms.Core.Domain.Tasks;
using TinyCms.Core.Infrastructure;
using TinyCms.Services.Infrastructure;
using TinyCms.Services.Logging;

namespace TinyCms.Services.Tasks
{
    /// <summary>
    ///     Task
    /// </summary>
    public class Task
    {
        #region Utilities

        private ITask CreateTask(ILifetimeScope scope)
        {
            ITask task = null;
            if (Enabled)
            {
                var type2 = System.Type.GetType(Type);
                if (type2 != null)
                {
                    object instance;
                    if (!EngineContext.Current.ContainerManager.TryResolve(type2, scope, out instance))
                    {
                        //not resolved
                        instance = EngineContext.Current.ContainerManager.ResolveUnregistered(type2, scope);
                    }
                    task = instance as ITask;
                }
            }
            return task;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Executes the task
        /// </summary>
        /// <param name="throwException">A value indicating whether exception should be thrown if some error happens</param>
        /// <param name="dispose">A value indicating whether all instances should be disposed after task run</param>
        /// <param name="ensureRunOnOneWebFarmInstance">
        ///     A value indicating whether we should ensure this task is run on one farm
        ///     node at a time
        /// </param>
        public void Execute(bool throwException = false, bool dispose = true, bool ensureRunOnOneWebFarmInstance = true)
        {
            //background tasks has an issue with Autofac
            //because scope is generated each time it's requested
            //that's why we get one single scope here
            //this way we can also dispose resources once a task is completed
            var scope = EngineContext.Current.ContainerManager.Scope();
            var scheduleTaskService = EngineContext.Current.ContainerManager.Resolve<IScheduleTaskService>("", scope);
            var scheduleTask = scheduleTaskService.GetTaskByType(Type);

            try
            {
                //task is run on one farm node at a time?
                if (ensureRunOnOneWebFarmInstance)
                {
                    //is web farm enabled (multiple instances)?
                    var nopConfig = EngineContext.Current.ContainerManager.Resolve<NopConfig>("", scope);
                    if (nopConfig.MultipleInstancesEnabled)
                    {
                        var machineNameProvider =
                            EngineContext.Current.ContainerManager.Resolve<IMachineNameProvider>("", scope);
                        var machineName = machineNameProvider.GetMachineName();
                        if (String.IsNullOrEmpty(machineName))
                        {
                            throw new Exception("Machine name cannot be detected. You cannot run in web farm.");
                            //actually in this case we can generate some unique string (e.g. Guid) and store it in some "static" (!!!) variable
                            //then it can be used as a machine name
                        }

                        //lease can't be aquired only if for a different machine and it has not expired
                        if (scheduleTask.LeasedUntilUtc.HasValue &&
                            scheduleTask.LeasedUntilUtc.Value >= DateTime.UtcNow &&
                            scheduleTask.LeasedByMachineName != machineName)
                            return;

                        //lease the task. so it's run on one farm node at a time
                        scheduleTask.LeasedByMachineName = machineName;
                        scheduleTask.LeasedUntilUtc = DateTime.UtcNow.AddMinutes(30);
                        scheduleTaskService.UpdateTask(scheduleTask);
                    }
                }

                //initialize and execute
                var task = CreateTask(scope);
                if (task != null)
                {
                    LastStartUtc = DateTime.UtcNow;
                    if (scheduleTask != null)
                    {
                        //update appropriate datetime properties
                        scheduleTask.LastStartUtc = LastStartUtc;
                        scheduleTaskService.UpdateTask(scheduleTask);
                    }
                    task.Execute();
                    LastEndUtc = LastSuccessUtc = DateTime.UtcNow;
                }
            }
            catch (Exception exc)
            {
                Enabled = !StopOnError;
                LastEndUtc = DateTime.UtcNow;

                //log error
                var logger = EngineContext.Current.ContainerManager.Resolve<ILogger>("", scope);
                logger.Error(string.Format("Error while running the '{0}' schedule task. {1}", Name, exc.Message), exc);
                if (throwException)
                    throw;
            }

            if (scheduleTask != null)
            {
                //update appropriate datetime properties
                scheduleTask.LastEndUtc = LastEndUtc;
                scheduleTask.LastSuccessUtc = LastSuccessUtc;
                scheduleTaskService.UpdateTask(scheduleTask);
            }

            //dispose all resources
            if (dispose)
            {
                scope.Dispose();
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        ///     Ctor for Task
        /// </summary>
        private Task()
        {
            Enabled = true;
        }

        /// <summary>
        ///     Ctor for Task
        /// </summary>
        /// <param name="task">Task </param>
        public Task(ScheduleTask task)
        {
            Type = task.Type;
            Enabled = task.Enabled;
            StopOnError = task.StopOnError;
            Name = task.Name;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Datetime of the last start
        /// </summary>
        public DateTime? LastStartUtc { get; private set; }

        /// <summary>
        ///     Datetime of the last end
        /// </summary>
        public DateTime? LastEndUtc { get; private set; }

        /// <summary>
        ///     Datetime of the last success
        /// </summary>
        public DateTime? LastSuccessUtc { get; private set; }

        /// <summary>
        ///     A value indicating type of the task
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        ///     A value indicating whether to stop task on error
        /// </summary>
        public bool StopOnError { get; private set; }

        /// <summary>
        ///     Get the task name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     A value indicating whether the task is enabled
        /// </summary>
        public bool Enabled { get; set; }

        #endregion
    }
}