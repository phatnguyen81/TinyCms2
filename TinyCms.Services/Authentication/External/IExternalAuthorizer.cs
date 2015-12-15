//Contributor:  Nicholas Mayne


using TinyCms.Services.Authentication.External;

namespace TinyCms.Services.Authentication.External
{
    /// <summary>
    /// External authorizer
    /// </summary>
    public partial interface IExternalAuthorizer
    {
        AuthorizationResult Authorize(OpenAuthenticationParameters parameters);
    }
}