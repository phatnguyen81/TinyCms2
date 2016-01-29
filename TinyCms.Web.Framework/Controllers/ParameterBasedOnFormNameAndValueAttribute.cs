using System;
using System.Web.Mvc;

namespace TinyCms.Web.Framework.Controllers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ParameterBasedOnFormNameAndValueAttribute : FilterAttribute, IActionFilter
    {
        private readonly string _actionParameterName;
        private readonly string _name;
        private readonly string _value;

        public ParameterBasedOnFormNameAndValueAttribute(string name, string value, string actionParameterName)
        {
            _name = name;
            _value = value;
            _actionParameterName = actionParameterName;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var formValue = filterContext.RequestContext.HttpContext.Request.Form[_name];
            filterContext.ActionParameters[_actionParameterName] = !string.IsNullOrEmpty(formValue) &&
                                                                   formValue.ToLower().Equals(_value.ToLower());
        }
    }
}