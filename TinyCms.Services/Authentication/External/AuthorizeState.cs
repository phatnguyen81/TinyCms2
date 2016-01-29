//Contributor:  Nicholas Mayne

using System.Collections.Generic;
using System.Web.Mvc;

namespace TinyCms.Services.Authentication.External
{
    /// <summary>
    ///     Authorize state
    /// </summary>
    public class AuthorizeState
    {
        public AuthorizeState(string returnUrl, OpenAuthenticationStatus openAuthenticationStatus)
        {
            Errors = new List<string>();
            AuthenticationStatus = openAuthenticationStatus;

            //in way SEO friendly language URLs will be persisted
            if (AuthenticationStatus == OpenAuthenticationStatus.Authenticated)
                Result = new RedirectResult(!string.IsNullOrEmpty(returnUrl) ? returnUrl : "~/");
        }

        public AuthorizeState(string returnUrl, AuthorizationResult authorizationResult)
            : this(returnUrl, authorizationResult.Status)
        {
            Errors = authorizationResult.Errors;
        }

        public OpenAuthenticationStatus AuthenticationStatus { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether request has been completed successfully
        /// </summary>
        public bool Success
        {
            get { return (Errors.Count == 0); }
        }

        /// <summary>
        ///     Errors
        /// </summary>
        public IList<string> Errors { get; set; }

        /// <summary>
        ///     Result
        /// </summary>
        public ActionResult Result { get; set; }

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