//Contributor:  Nicholas Mayne


namespace TinyCms.Services.Authentication.External
{
    /// <summary>
    /// Claims translator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial interface IClaimsTranslator<T>
    {
        UserClaims Translate(T response);
    }
}