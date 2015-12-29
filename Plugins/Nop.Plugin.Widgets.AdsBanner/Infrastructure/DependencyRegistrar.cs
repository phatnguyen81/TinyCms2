using Autofac;
using Autofac.Core;
using Nop.Plugin.Widgets.AdsBanner.Controllers;
using Nop.Plugin.Widgets.AdsBanner.Data;
using Nop.Plugin.Widgets.AdsBanner.Domain;
using Nop.Plugin.Widgets.AdsBanner.Services;
using TinyCms.Core.Caching;
using TinyCms.Core.Configuration;
using TinyCms.Core.Data;
using TinyCms.Core.Infrastructure;
using TinyCms.Core.Infrastructure.DependencyManagement;
using TinyCms.Data;
using TinyCms.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.AdsBanner.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {

        private const string CONTEXT_NAME = "cms_object_context_adsbanner";

        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<AdsBannerService>().As<IAdsBannerService>().InstancePerLifetimeScope();

            //datacontext
            this.RegisterPluginDataContext<AdsBannerObjectContext>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<AdsBannerRecord>>()
                .As<IRepository<AdsBannerRecord>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<AdsBannerService>().As<IAdsBannerService>()
               .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
               .InstancePerLifetimeScope();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 1; }
        }
    }
}
