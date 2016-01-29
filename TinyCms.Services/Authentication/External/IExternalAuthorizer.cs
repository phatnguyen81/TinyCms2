//Contributor:  Nicholas Mayne


namespace TinyCms.Services.Authentication.External
{
    /// <summary>
    ///     External authorizer
    /// </summary>
    public interface IExternalAuthorizer
    {
        AuthorizationResult Authorize(OpenAuthenticationParameters parameters);
    }
}