using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TinyCms.Admin.Extensions;
using TinyCms.Admin.Models.Customers;
using TinyCms.Core;
using TinyCms.Core.Domain.Customers;
using TinyCms.Services.Customers;
using TinyCms.Services.Localization;
using TinyCms.Services.Logging;
using TinyCms.Services.Security;
using TinyCms.Web.Framework.Controllers;
using TinyCms.Web.Framework.Kendoui;

namespace TinyCms.Admin.Controllers
{
    public partial class CustomerRoleController : BaseAdminController
	{
		#region Fields

		private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

		#endregion

		#region Constructors

        public CustomerRoleController(ICustomerService customerService,
            ILocalizationService localizationService, 
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService,
            IWorkContext workContext)
		{
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._workContext = workContext;
		}

		#endregion 

        #region Utilities

        [NonAction]
        protected CustomerRoleModel PrepareCustomerRoleModel(CustomerRole customerRole)
        {
            var model = customerRole.ToModel();
            return model;
        }

        #endregion

        #region Customer roles

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

		public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
			return View();
		}

		[HttpPost]
		public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
            var customerRoles = _customerService.GetAllCustomerRoles(true);
            var gridModel = new DataSourceResult
			{
                Data = customerRoles.Select(PrepareCustomerRoleModel),
                Total = customerRoles.Count()
			};
			return new JsonResult
			{
				Data = gridModel
			};
		}

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
            var model = new CustomerRoleModel();
            //default values
            model.Active = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(CustomerRoleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
            if (ModelState.IsValid)
            {
                var customerRole = model.ToEntity();
                _customerService.InsertCustomerRole(customerRole);

                //activity log
                _customerActivityService.InsertActivity("AddNewCustomerRole", _localizationService.GetResource("ActivityLog.AddNewCustomerRole"), customerRole.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = customerRole.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

		public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
            var customerRole = _customerService.GetCustomerRoleById(id);
            if (customerRole == null)
                //No customer role found with the specified id
                return RedirectToAction("List");
		    
            var model = PrepareCustomerRoleModel(customerRole);
            return View(model);
		}

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(CustomerRoleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
            var customerRole = _customerService.GetCustomerRoleById(model.Id);
            if (customerRole == null)
                //No customer role found with the specified id
                return RedirectToAction("List");

            try
            {
                if (ModelState.IsValid)
                {
                    if (customerRole.IsSystemRole && !model.Active)
                        throw new NopException(_localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.Active.CantEditSystem"));

                    if (customerRole.IsSystemRole && !customerRole.SystemName.Equals(model.SystemName, StringComparison.InvariantCultureIgnoreCase))
                        throw new NopException(_localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.SystemName.CantEditSystem"));

                    if (SystemCustomerRoleNames.Registered.Equals(customerRole.SystemName, StringComparison.InvariantCultureIgnoreCase) &&
                        model.PurchasedWithProductId > 0)
                        throw new NopException(_localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct.Registered"));
                    
                    customerRole = model.ToEntity(customerRole);
                    _customerService.UpdateCustomerRole(customerRole);

                    //activity log
                    _customerActivityService.InsertActivity("EditCustomerRole", _localizationService.GetResource("ActivityLog.EditCustomerRole"), customerRole.Name);

                    SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Updated"));
                    return continueEditing ? RedirectToAction("Edit", new { id = customerRole.Id}) : RedirectToAction("List");
                }

                //If we got this far, something failed, redisplay form
                return View(model);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = customerRole.Id });
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            
            var customerRole = _customerService.GetCustomerRoleById(id);
            if (customerRole == null)
                //No customer role found with the specified id
                return RedirectToAction("List");

            try
            {
                _customerService.DeleteCustomerRole(customerRole);

                //activity log
                _customerActivityService.InsertActivity("DeleteCustomerRole", _localizationService.GetResource("ActivityLog.DeleteCustomerRole"), customerRole.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerRoles.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = customerRole.Id });
            }

		}


		#endregion
    }
}
