using System;
using System.Diagnostics;
using System.Reflection;
using System.Web.Mvc;

namespace TinyCms.Web.Framework.Controllers
{
    public class FormValueRequiredAttribute : ActionMethodSelectorAttribute
    {
        private readonly FormValueRequirement _requirement;
        private readonly string[] _submitButtonNames;

        public FormValueRequiredAttribute(params string[] submitButtonNames) :
            this(FormValueRequirement.Equal, submitButtonNames)
        {
        }

        public FormValueRequiredAttribute(FormValueRequirement requirement, params string[] submitButtonNames)
        {
            //at least one submit button should be found
            _submitButtonNames = submitButtonNames;
            _requirement = requirement;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            foreach (var buttonName in _submitButtonNames)
            {
                try
                {
                    var value = "";
                    switch (_requirement)
                    {
                        case FormValueRequirement.Equal:
                        {
                            //do not iterate because "Invalid request" exception can be thrown
                            value = controllerContext.HttpContext.Request.Form[buttonName];
                        }
                            break;
                        case FormValueRequirement.StartsWith:
                        {
                            foreach (var formValue in controllerContext.HttpContext.Request.Form.AllKeys)
                            {
                                if (formValue.StartsWith(buttonName, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    value = controllerContext.HttpContext.Request.Form[formValue];
                                    break;
                                }
                            }
                        }
                            break;
                    }
                    if (!String.IsNullOrEmpty(value))
                        return true;
                }
                catch (Exception exc)
                {
                    //try-catch to ensure that 
                    Debug.WriteLine(exc.Message);
                }
            }
            return false;
        }
    }

    public enum FormValueRequirement
    {
        Equal,
        StartsWith
    }
}