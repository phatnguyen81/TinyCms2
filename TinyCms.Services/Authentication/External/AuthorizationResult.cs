//Contributor:  Nicholas Mayne

using System.Collections.Generic;

namespace TinyCms.Services.Authentication.External
{
    /// <summary>
    ///     Authorization result
    /// </summary>
    public class AuthorizationResult
    {
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="status"></param>
        public AuthorizationResult(OpenAuthenticationStatus status)
        {
            Errors = new List<string>();
            Status = status;
        }

        /// <summary>
        ///     Gets a value indicating whether request has been completed successfully
        /// </summary>
        public bool Success
        {
            get { return Errors.Count == 0; }
        }

        /// <summary>
        ///     Status
        /// </summary>
        public OpenAuthenticationStatus Status { get; private set; }

        /// <summary>
        ///     Errors
        /// </summary>
        public IList<string> Errors { get; set; }

        /// <summary>
        ///     Add error
        /// </summary>
        /// <param name="error">Error</param>
        public void AddError(string error)
        {
            Errors.Add(error);
        }
    }
}