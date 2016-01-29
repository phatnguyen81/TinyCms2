using System.Collections.Generic;
using System.Web.Mvc;

namespace TinyCms.Web.Framework.Mvc
{
    /// <summary>
    ///     Base nopCommerce model
    /// </summary>
    [ModelBinder(typeof (NopModelBinder))]
    public class BaseNopModel
    {
        public BaseNopModel()
        {
            CustomProperties = new Dictionary<string, object>();
            PostInitialize();
        }

        /// <summary>
        ///     Use this property to store any custom value for your models.
        /// </summary>
        public Dictionary<string, object> CustomProperties { get; set; }

        public virtual void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
        }

        /// <summary>
        ///     Developers can override this method in custom partial classes
        ///     in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {
        }
    }

    /// <summary>
    ///     Base nopCommerce entity model
    /// </summary>
    public class BaseNopEntityModel : BaseNopModel
    {
        public virtual int Id { get; set; }
    }
}