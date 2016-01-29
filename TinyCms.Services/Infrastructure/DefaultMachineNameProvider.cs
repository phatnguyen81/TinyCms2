//Contribution: Orchard project (http://www.orchardproject.net/)

using System;

namespace TinyCms.Services.Infrastructure
{
    /// <summary>
    ///     Default machine name provider
    /// </summary>
    public class DefaultMachineNameProvider : IMachineNameProvider
    {
        /// <summary>
        ///     Returns the name of the machine (instance) running the application.
        /// </summary>
        public string GetMachineName()
        {
            return Environment.MachineName;
        }
    }
}