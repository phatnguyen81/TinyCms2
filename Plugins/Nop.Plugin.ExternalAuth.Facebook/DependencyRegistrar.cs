using Autofac;
using Nop.Plugin.ExternalAuth.Facebook.Core;
using TinyCms.Core.Configuration;
using TinyCms.Core.Infrastructure;
using TinyCms.Core.Infrastructure.DependencyManagement;

namespace Nop.Plugin.ExternalAuth.Facebook
{
    /// <summary>
    ///     Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        ///     Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<FacebookProviderAuthorizer>()
                .As<IOAuthProviderFacebookAuthorizer>()
                .InstancePerLifetimeScope();
        }

        /// <summary>
        ///     Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 1; }
        }
    }
}