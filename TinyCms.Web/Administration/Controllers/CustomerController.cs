using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using TinyCms.Admin.Extensions;
using TinyCms.Admin.Models.Customers;
using TinyCms.Core;
using TinyCms.Core.Domain.Catalog;
using TinyCms.Core.Domain.Customers;
using TinyCms.Core.Domain.Messages;
using TinyCms.Services.Authentication.External;
using TinyCms.Services.Common;
using TinyCms.Services.Customers;
using TinyCms.Services.ExportImport;
using TinyCms.Services.Helpers;
using TinyCms.Services.Localization;
using TinyCms.Services.Logging;
using TinyCms.Services.Messages;
using TinyCms.Services.Security;
using TinyCms.Web.Framework.Controllers;
using TinyCms.Web.Framework.Kendoui;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Controllers
{
    public partial class CustomerController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly IWorkContext _workContext;
        private readonly IExportManager _exportManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly IWorkflowMessageService _workflowMessageService;

        #endregion

        #region Constructors

        public CustomerController(ICustomerService customerService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IGenericAttributeService genericAttributeService,
            ICustomerRegistrationService customerRegistrationService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService, 
            DateTimeSettings dateTimeSettings,
            CustomerSettings customerSettings,
            IWorkContext workContext,
            IExportManager exportManager,
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService, 
            IQueuedEmailService queuedEmailService,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService, 
            IOpenAuthenticationService openAuthenticationService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            IWorkflowMessageService workflowMessageService)
        {
            this._customerService = customerService;
            this._newsLetterSubscriptionService = newsLetterSubscriptionService;
            this._genericAttributeService = genericAttributeService;
            this._customerRegistrationService = customerRegistrationService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._dateTimeSettings = dateTimeSettings;
            this._customerSettings = customerSettings;
            this._workContext = workContext;
            this._exportManager = exportManager;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._queuedEmailService = queuedEmailService;
            this._emailAccountSettings = emailAccountSettings;
            this._emailAccountService = emailAccountService;
            this._openAuthenticationService = openAuthenticationService;
            this._customerAttributeParser = customerAttributeParser;
            this._customerAttributeService = customerAttributeService;
            this._workflowMessageService = workflowMessageService;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual string GetCustomerRolesNames(IList<CustomerRole> customerRoles, string separator = ",")
        {
            var sb = new StringBuilder();
            for (int i = 0; i < customerRoles.Count; i++)
            {
                sb.Append(customerRoles[i].Name);
                if (i != customerRoles.Count - 1)
                {
                    sb.Append(separator);
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        [NonAction]
        protected virtual IList<CustomerModel.AssociatedExternalAuthModel> GetAssociatedExternalAuthRecords(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var result = new List<CustomerModel.AssociatedExternalAuthModel>();
            foreach (var record in _openAuthenticationService.GetExternalIdentifiersFor(customer))
            {
                var method = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName(record.ProviderSystemName);
                if (method == null)
                    continue;

                result.Add(new CustomerModel.AssociatedExternalAuthModel
                {
                    Id = record.Id,
                    Email = record.Email,
                    ExternalIdentifier = record.ExternalIdentifier,
                    //AuthMethodName = method.PluginDescriptor.FriendlyName
                });
            }

            return result;
        }

        [NonAction]
        protected virtual CustomerModel PrepareCustomerModelForList(Customer customer)
        {
            return new CustomerModel
            {
                Id = customer.Id,
                Email = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                Username = customer.Username,
                FullName = customer.GetFullName(),
                Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company),
                Phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone),
                ZipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode),
                CustomerRoleNames = GetCustomerRolesNames(customer.CustomerRoles.ToList()),
                Active = customer.Active,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc),
                LastActivityDate = _dateTimeHelper.ConvertToUserTime(customer.LastActivityDateUtc, DateTimeKind.Utc),
            };
        }

        [NonAction]
        protected virtual string ValidateCustomerRoles(IList<CustomerRole> customerRoles)
        {
            if (customerRoles == null)
                throw new ArgumentNullException("customerRoles");

            //ensure a customer is not added to both 'Guests' and 'Registered' customer roles
            //ensure that a customer is in at least one required role ('Guests' and 'Registered')
            bool isInGuestsRole = customerRoles.FirstOrDefault(cr => cr.SystemName == SystemCustomerRoleNames.Guests) != null;
            bool isInRegisteredRole = customerRoles.FirstOrDefault(cr => cr.SystemName == SystemCustomerRoleNames.Registered) != null;
            if (isInGuestsRole && isInRegisteredRole)
                return "The customer cannot be in both 'Guests' and 'Registered' customer roles";
            if (!isInGuestsRole && !isInRegisteredRole)
                return "Add the customer to 'Guests' or 'Registered' customer role";

            //no errors
            return "";
        }

       
        [NonAction]
        protected virtual void PrepareCustomerAttributeModel(CustomerModel model, Customer customer)
        {
            var customerAttributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in customerAttributes)
            {
                var attributeModel = new CustomerModel.CustomerAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new CustomerModel.CustomerAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }


                //set already selected attributes
                if (customer != null)
                {
                    var selectedCustomerAttributes = customer.GetAttribute<string>(SystemCustomerAttributeNames.CustomCustomerAttributes, _genericAttributeService);
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                        {
                            if (!String.IsNullOrEmpty(selectedCustomerAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = _customerAttributeParser.ParseCustomerAttributeValues(selectedCustomerAttributes);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                            break;
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                        {
                            if (!String.IsNullOrEmpty(selectedCustomerAttributes))
                            {
                                var enteredText = _customerAttributeParser.ParseValues(selectedCustomerAttributes, attribute.Id);
                                if (enteredText.Count > 0)
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                            break;
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.Datepicker:
                        case AttributeControlType.FileUpload:
                        default:
                            //not supported attribute control types
                            break;
                    }
                }

                model.CustomerAttributes.Add(attributeModel);
            }
        }

        [NonAction]
        protected virtual string ParseCustomCustomerAttributes(Customer customer, FormCollection form)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (form == null)
                throw new ArgumentNullException("form");

            string attributesXml = "";
            var customerAttributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in customerAttributes)
            {
                string controlId = string.Format("customer_attribute_{0}", attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.FileUpload:
                    //not supported customer attributes
                    default:
                        break;
                }
            }

            return attributesXml;
        }

        [NonAction]
        protected virtual void PrepareCustomerModel(CustomerModel model, Customer customer, bool excludeProperties)
        {
            if (customer != null)
            {
                model.Id = customer.Id;
                if (!excludeProperties)
                {
                    model.Email = customer.Email;
                    model.Username = customer.Username;
                    model.AdminComment = customer.AdminComment;
                    model.Active = customer.Active;

                    model.TimeZoneId = customer.GetAttribute<string>(SystemCustomerAttributeNames.TimeZoneId);
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc);
                    model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(customer.LastActivityDateUtc, DateTimeKind.Utc);
                    model.LastIpAddress = customer.LastIpAddress;
                    model.LastVisitedPage = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastVisitedPage);

                    model.SelectedCustomerRoleIds = customer.CustomerRoles.Select(cr => cr.Id).ToArray();

                    //newsletter subscriptions
                    if (!String.IsNullOrEmpty(customer.Email))
                    {
                        var newsletterSubscriptionStoreIds = new List<int>();
                        var newsletterSubscription = _newsLetterSubscriptionService
                               .GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email);
                        model.SelectedNewsletterSubscriptionStoreIds = newsletterSubscriptionStoreIds.ToArray();
                    }

                    //form fields
                    model.FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
                    model.LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName);
                    model.Gender = customer.GetAttribute<string>(SystemCustomerAttributeNames.Gender);
                    model.DateOfBirth = customer.GetAttribute<DateTime?>(SystemCustomerAttributeNames.DateOfBirth);
                    model.Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company);
                    model.StreetAddress = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress);
                    model.StreetAddress2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2);
                    model.ZipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode);
                    model.City = customer.GetAttribute<string>(SystemCustomerAttributeNames.City);
                    model.CountryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId);
                    model.StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId);
                    model.Phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                    model.Fax = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax);
                }
            }

            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _customerSettings.AllowUsersToChangeUsernames;
            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == model.TimeZoneId) });
          
            //customer attributes
            PrepareCustomerAttributeModel(model, customer);

            model.GenderEnabled = _customerSettings.GenderEnabled;
            model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _customerSettings.CompanyEnabled;
            model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
            model.CityEnabled = _customerSettings.CityEnabled;
            model.CountryEnabled = _customerSettings.CountryEnabled;
            model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
            model.PhoneEnabled = _customerSettings.PhoneEnabled;
            model.FaxEnabled = _customerSettings.FaxEnabled;

          
            //customer roles
            model.AvailableCustomerRoles = _customerService
                .GetAllCustomerRoles(true)
                .Select(cr => cr.ToModel())
                .ToList();
      
            //external authentication records
            if (customer != null)
            {
                model.AssociatedExternalAuthRecords = GetAssociatedExternalAuthRecords(customer);
            }
            //sending of the welcome message:
            //1. "admin approval" registration method
            //2. already created customer
            //3. registered
            model.AllowSendingOfWelcomeMessage = _customerSettings.UserRegistrationType == UserRegistrationType.AdminApproval &&
                customer != null &&
                customer.IsRegistered();
            //sending of the activation message
            //1. "email validation" registration method
            //2. already created customer
            //3. registered
            //4. not active
            model.AllowReSendingOfActivationMessage = _customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation &&
                customer != null &&
                customer.IsRegistered() &&
                !customer.Active;
        }

      
        #endregion

        #region Customers

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //load registered customers by default
            var defaultRoleIds = new[] {_customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered).Id};
            var model = new CustomerListModel
            {
                UsernamesEnabled = _customerSettings.UsernamesEnabled,
                DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled,
                CompanyEnabled = _customerSettings.CompanyEnabled,
                PhoneEnabled = _customerSettings.PhoneEnabled,
                ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled,
                AvailableCustomerRoles = _customerService.GetAllCustomerRoles(true).Select(cr => cr.ToModel()).ToList(),
                SearchCustomerRoleIds = defaultRoleIds,
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult CustomerList(DataSourceRequest command, CustomerListModel model,
            [ModelBinder(typeof(CommaSeparatedModelBinder))] int[] searchCustomerRoleIds)
        {
            //we use own own binder for searchCustomerRoleIds property 
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var searchDayOfBirth = 0;
            int searchMonthOfBirth = 0;
            if (!String.IsNullOrWhiteSpace(model.SearchDayOfBirth))
                searchDayOfBirth = Convert.ToInt32(model.SearchDayOfBirth);
            if (!String.IsNullOrWhiteSpace(model.SearchMonthOfBirth))
                searchMonthOfBirth = Convert.ToInt32(model.SearchMonthOfBirth);
            
            var customers = _customerService.GetAllCustomers(
                customerRoleIds: searchCustomerRoleIds,
                email: model.SearchEmail,
                username: model.SearchUsername,
                firstName: model.SearchFirstName,
                lastName: model.SearchLastName,
                dayOfBirth: searchDayOfBirth,
                monthOfBirth: searchMonthOfBirth,
                company: model.SearchCompany,
                phone: model.SearchPhone,
                zipPostalCode: model.SearchZipPostalCode,
                loadOnlyWithShoppingCart: false,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = customers.Select(PrepareCustomerModelForList),
                Total = customers.TotalCount
            };

            return Json(gridModel);
        }
        
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var model = new CustomerModel();
            PrepareCustomerModel(model, null, false);
            //default value
            model.Active = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        [ValidateInput(false)]
        public ActionResult Create(CustomerModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            if (!String.IsNullOrWhiteSpace(model.Email))
            {
                var cust2 = _customerService.GetCustomerByEmail(model.Email);
                if (cust2 != null)
                    ModelState.AddModelError("", "Email is already registered");
            }
            if (!String.IsNullOrWhiteSpace(model.Username) & _customerSettings.UsernamesEnabled)
            {
                var cust2 = _customerService.GetCustomerByUsername(model.Username);
                if (cust2 != null)
                    ModelState.AddModelError("", "Username is already registered");
            }

            //validate customer roles
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds != null && model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);
            var customerRolesError = ValidateCustomerRoles(newCustomerRoles);
            if (!String.IsNullOrEmpty(customerRolesError))
            {
                ModelState.AddModelError("", customerRolesError);
                ErrorNotification(customerRolesError, false);
            }
            
            if (ModelState.IsValid)
            {
                var customer = new Customer
                {
                    CustomerGuid = Guid.NewGuid(),
                    Email = model.Email,
                    Username = model.Username,
                    AdminComment = model.AdminComment,
                    Active = model.Active,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastActivityDateUtc = DateTime.UtcNow,
                };
                _customerService.InsertCustomer(customer);

                //form fields

                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FullName, model.FullName);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);
                //if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TimeZoneId, model.TimeZoneId);
                //if (_customerSettings.GenderEnabled)
                //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                //_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                //_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                //if (_customerSettings.DateOfBirthEnabled)
                //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, model.DateOfBirth);
                //if (_customerSettings.CompanyEnabled)
                //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Company, model.Company);
                //if (_customerSettings.StreetAddressEnabled)
                //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress, model.StreetAddress);
                //if (_customerSettings.StreetAddress2Enabled)
                //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress2, model.StreetAddress2);
                //if (_customerSettings.ZipPostalCodeEnabled)
                //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ZipPostalCode, model.ZipPostalCode);
                //if (_customerSettings.CityEnabled)
                //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.City, model.City);
                //if (_customerSettings.CountryEnabled)
                //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CountryId, model.CountryId);
                //if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StateProvinceId, model.StateProvinceId);
                //if (_customerSettings.PhoneEnabled)
                //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);
                //if (_customerSettings.FaxEnabled)
                //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Fax, model.Fax);

                //custom customer attributes
                var customerAttributes = ParseCustomCustomerAttributes(customer, form);
                _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CustomCustomerAttributes, customerAttributes);


                //newsletter subscriptions
                //if (!String.IsNullOrEmpty(customer.Email))
                //{
                //    var allStores = _storeService.GetAllStores();
                //    foreach (var store in allStores)
                //    {
                //        var newsletterSubscription = _newsLetterSubscriptionService
                //            .GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, store.Id);
                //        if (model.SelectedNewsletterSubscriptionStoreIds != null &&
                //            model.SelectedNewsletterSubscriptionStoreIds.Contains(store.Id))
                //        {
                //            //subscribed
                //            if (newsletterSubscription == null)
                //            {
                //                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                //                {
                //                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                //                    Email = customer.Email,
                //                    Active = true,
                //                    StoreId = store.Id,
                //                    CreatedOnUtc = DateTime.UtcNow
                //                });
                //            }
                //        }
                //        else
                //        {
                //            //not subscribed
                //            if (newsletterSubscription != null)
                //            {
                //                _newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletterSubscription);
                //            }
                //        }
                //    }
                //}

                //password
                if (!String.IsNullOrWhiteSpace(model.Password))
                {
                    var changePassRequest = new ChangePasswordRequest(model.Email, false, _customerSettings.DefaultPasswordFormat, model.Password);
                    var changePassResult = _customerRegistrationService.ChangePassword(changePassRequest);
                    if (!changePassResult.Success)
                    {
                        foreach (var changePassError in changePassResult.Errors)
                            ErrorNotification(changePassError);
                    }
                }

                //customer roles
                foreach (var customerRole in newCustomerRoles)
                {
                    //ensure that the current customer cannot add to "Administrators" system role if he's not an admin himself
                    if (customerRole.SystemName == SystemCustomerRoleNames.Administrators && 
                        !_workContext.CurrentCustomer.IsAdmin())
                        continue;

                    customer.CustomerRoles.Add(customerRole);
                }
                _customerService.UpdateCustomer(customer);
                

                ////ensure that a customer with a vendor associated is not in "Administrators" role
                ////otherwise, he won't be have access to the other functionality in admin area
                //if (customer.IsAdmin() && customer.VendorId > 0)
                //{
                //    customer.VendorId = 0;
                //    _customerService.UpdateCustomer(customer);
                //    ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.AdminCouldNotbeVendor"));
                //}

                ////ensure that a customer in the Vendors role has a vendor account associated.
                ////otherwise, he will have access to ALL products
                //if (customer.IsVendor() && customer.VendorId == 0)
                //{
                //    var vendorRole = customer
                //        .CustomerRoles
                //        .FirstOrDefault(x => x.SystemName == SystemCustomerRoleNames.Vendors);
                //    customer.CustomerRoles.Remove(vendorRole);
                //    _customerService.UpdateCustomer(customer);
                //    ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.CannotBeInVendoRoleWithoutVendorAssociated"));
                //}

                //activity log
                _customerActivityService.InsertActivity("AddNewCustomer", _localizationService.GetResource("ActivityLog.AddNewCustomer"), customer.Id);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = customer.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareCustomerModel(model, null, true);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(id);
            if (customer == null || customer.Deleted)
                //No customer found with the specified id
                return RedirectToAction("List");

            var model = new CustomerModel();
            PrepareCustomerModel(model, customer, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        [ValidateInput(false)]
        public ActionResult Edit(CustomerModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null || customer.Deleted)
                //No customer found with the specified id
                return RedirectToAction("List");

            //validate customer roles
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds != null && model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);
            var customerRolesError = ValidateCustomerRoles(newCustomerRoles);
            if (!String.IsNullOrEmpty(customerRolesError))
            {
                ModelState.AddModelError("", customerRolesError);
                ErrorNotification(customerRolesError, false);
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    customer.AdminComment = model.AdminComment;
                    customer.Active = model.Active;
                    //email
                    //if (!String.IsNullOrWhiteSpace(model.Email))
                    //{
                    //    _customerRegistrationService.SetEmail(customer, model.Email);
                    //}
                    //else
                    //{
                    //    customer.Email = model.Email;
                    //}

                    //username
                    if (_customerSettings.UsernamesEnabled && _customerSettings.AllowUsersToChangeUsernames)
                    {
                        if (!String.IsNullOrWhiteSpace(model.Username))
                        {
                            _customerRegistrationService.SetUsername(customer, model.Username);
                        }
                        else
                        {
                            customer.Username = model.Username;
                        }
                    }


                    //form fields
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FullName, model.FullName);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);

                    //if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.TimeZoneId, model.TimeZoneId);
                    //if (_customerSettings.GenderEnabled)
                    //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                    //_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                    //_genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                    //if (_customerSettings.DateOfBirthEnabled)
                    //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, model.DateOfBirth);
                    //if (_customerSettings.CompanyEnabled)
                    //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Company, model.Company);
                    //if (_customerSettings.StreetAddressEnabled)
                    //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress, model.StreetAddress);
                    //if (_customerSettings.StreetAddress2Enabled)
                    //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress2, model.StreetAddress2);
                    //if (_customerSettings.ZipPostalCodeEnabled)
                    //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.ZipPostalCode, model.ZipPostalCode);
                    //if (_customerSettings.CityEnabled)
                    //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.City, model.City);
                    //if (_customerSettings.CountryEnabled)
                    //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CountryId, model.CountryId);
                    //if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StateProvinceId, model.StateProvinceId);
                    ////if (_customerSettings.PhoneEnabled)
                    ////    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);
                    //if (_customerSettings.FaxEnabled)
                    //    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Fax, model.Fax);

                    //custom customer attributes
                    var customerAttributes = ParseCustomCustomerAttributes(customer, form);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.CustomCustomerAttributes, customerAttributes);

                    //newsletter subscriptions
                    //if (!String.IsNullOrEmpty(customer.Email))
                    //{
                    //    var allStores = _storeService.GetAllStores();
                    //    foreach (var store in allStores)
                    //    {
                    //        var newsletterSubscription = _newsLetterSubscriptionService
                    //            .GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, store.Id);
                    //        if (model.SelectedNewsletterSubscriptionStoreIds != null &&
                    //            model.SelectedNewsletterSubscriptionStoreIds.Contains(store.Id))
                    //        {
                    //            //subscribed
                    //            if (newsletterSubscription == null)
                    //            {
                    //                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                    //                {
                    //                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                    //                    Email = customer.Email,
                    //                    Active = true,
                    //                    StoreId = store.Id,
                    //                    CreatedOnUtc = DateTime.UtcNow
                    //                });
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //not subscribed
                    //            if (newsletterSubscription != null)
                    //            {
                    //                _newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletterSubscription);
                    //            }
                    //        }
                    //    }
                    //}


                    //customer roles
                    foreach (var customerRole in allCustomerRoles)
                    {
                        //ensure that the current customer cannot add/remove to/from "Administrators" system role
                        //if he's not an admin himself
                        if (customerRole.SystemName == SystemCustomerRoleNames.Administrators &&
                            !_workContext.CurrentCustomer.IsAdmin())
                            continue;

                        if (model.SelectedCustomerRoleIds != null &&
                            model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                        {
                            //new role
                            if (customer.CustomerRoles.Count(cr => cr.Id == customerRole.Id) == 0)
                                customer.CustomerRoles.Add(customerRole);
                        }
                        else
                        {
                            //remove role
                            if (customer.CustomerRoles.Count(cr => cr.Id == customerRole.Id) > 0)
                                customer.CustomerRoles.Remove(customerRole);
                        }
                    }
                    _customerService.UpdateCustomer(customer);
                    

                 

                    //activity log
                    _customerActivityService.InsertActivity("EditCustomer", _localizationService.GetResource("ActivityLog.EditCustomer"), customer.Id);

                    SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Updated"));
                    if (continueEditing)
                    {
                        //selected tab
                        SaveSelectedTabIndex();

                        return RedirectToAction("Edit",  new {id = customer.Id});
                    }
                    return RedirectToAction("List");
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc.Message, false);
                }
            }


            //If we got this far, something failed, redisplay form
            PrepareCustomerModel(model, customer, true);
            return View(model);
        }
        
        [HttpPost, ActionName("Edit")]
        [FormValueRequired("changepassword")]
        public ActionResult ChangePassword(CustomerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var changePassRequest = new ChangePasswordRequest(model.Email,
                    false, _customerSettings.DefaultPasswordFormat, model.Password);
                var changePassResult = _customerRegistrationService.ChangePassword(changePassRequest);
                if (changePassResult.Success)
                    SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.PasswordChanged"));
                else
                    foreach (var error in changePassResult.Errors)
                        ErrorNotification(error);
            }

            return RedirectToAction("Edit",  new {id = customer.Id});
        }
      
      
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            try
            {
                _customerService.DeleteCustomer(customer);

                //remove newsletter subscription (if exists)
                //foreach (var store in _storeService.GetAllStores())
                //{
                //    var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, store.Id);
                //    if (subscription != null)
                //        _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);
                //}

                //activity log
                _customerActivityService.InsertActivity("DeleteCustomer", _localizationService.GetResource("ActivityLog.DeleteCustomer"), customer.Id);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = customer.Id });
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("impersonate")]
        public ActionResult Impersonate(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AllowCustomerImpersonation))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            //ensure that a non-admin user cannot impersonate as an administrator
            //otherwise, that user can simply impersonate as an administrator and gain additional administrative privileges
            if (!_workContext.CurrentCustomer.IsAdmin() && customer.IsAdmin())
            {
                ErrorNotification("A non-admin user cannot impersonate as an administrator");
                return RedirectToAction("Edit", customer.Id);
            }


            _genericAttributeService.SaveAttribute<int?>(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.ImpersonatedCustomerId, customer.Id);

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("send-welcome-message")]
        public ActionResult SendWelcomeMessage(CustomerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.SendWelcomeMessage.Success"));

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("resend-activation-message")]
        public ActionResult ReSendActivationMessage(CustomerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            //email validation message
            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());
            _workflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.ReSendActivationMessage.Success"));

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        public ActionResult SendEmail(CustomerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            try
            {
                if (String.IsNullOrWhiteSpace(customer.Email))
                    throw new NopException("Customer email is empty");
                if (!CommonHelper.IsValidEmail(customer.Email))
                    throw new NopException("Customer email is not valid");
                if (String.IsNullOrWhiteSpace(model.SendEmail.Subject))
                    throw new NopException("Email subject is empty");
                if (String.IsNullOrWhiteSpace(model.SendEmail.Body))
                    throw new NopException("Email body is empty");

                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
                if (emailAccount == null)
                    throw new NopException("Email account can't be loaded");

                var email = new QueuedEmail
                {
                    Priority = QueuedEmailPriority.High,
                    EmailAccountId = emailAccount.Id,
                    FromName = emailAccount.DisplayName,
                    From = emailAccount.Email,
                    ToName = customer.GetFullName(),
                    To = customer.Email,
                    Subject = model.SendEmail.Subject,
                    Body = model.SendEmail.Body,
                    CreatedOnUtc = DateTime.UtcNow,
                };
                _queuedEmailService.InsertQueuedEmail(email);
                SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.SendEmail.Queued"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
            }

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        //public ActionResult SendPm(CustomerModel model)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
        //        return AccessDeniedView();

        //    var customer = _customerService.GetCustomerById(model.Id);
        //    if (customer == null)
        //        //No customer found with the specified id
        //        return RedirectToAction("List");

        //    try
        //    {
        //        if (!_forumSettings.AllowPrivateMessages)
        //            throw new NopException("Private messages are disabled");
        //        if (customer.IsGuest())
        //            throw new NopException("Customer should be registered");
        //        if (String.IsNullOrWhiteSpace(model.SendPm.Subject))
        //            throw new NopException("PM subject is empty");
        //        if (String.IsNullOrWhiteSpace(model.SendPm.Message))
        //            throw new NopException("PM message is empty");


        //        var privateMessage = new PrivateMessage
        //        {
        //            StoreId = _storeContext.CurrentStore.Id,
        //            ToCustomerId = customer.Id,
        //            FromCustomerId = _workContext.CurrentCustomer.Id,
        //            Subject = model.SendPm.Subject,
        //            Text = model.SendPm.Message,
        //            IsDeletedByAuthor = false,
        //            IsDeletedByRecipient = false,
        //            IsRead = false,
        //            CreatedOnUtc = DateTime.UtcNow
        //        };

        //        _forumService.InsertPrivateMessage(privateMessage);
        //        SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.SendPM.Sent"));
        //    }
        //    catch (Exception exc)
        //    {
        //        ErrorNotification(exc.Message);
        //    }

        //    return RedirectToAction("Edit", new { id = customer.Id });
        //}
        
        #endregion
        
     
    
    
        #region Activity log

        [HttpPost]
        public ActionResult ListActivityLog(DataSourceRequest command, int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return Content("");

            var activityLog = _customerActivityService.GetAllActivities(null, null, customerId, 0, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = activityLog.Select(x =>
                {
                    var m = new CustomerModel.ActivityLogModel
                    {
                        Id = x.Id,
                        ActivityLogTypeName = x.ActivityLogType.Name,
                        Comment = x.Comment,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc)
                    };
                    return m;

                }),
                Total = activityLog.TotalCount
            };

            return Json(gridModel);
        }

        #endregion

        #region Back in stock subscriptions

      

        #endregion

        #region Export / Import

        [HttpPost, ActionName("List")]
        [FormValueRequired("exportexcel-all")]
        public ActionResult ExportExcelAll(CustomerListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var searchDayOfBirth = 0;
            int searchMonthOfBirth = 0;
            if (!String.IsNullOrWhiteSpace(model.SearchDayOfBirth))
                searchDayOfBirth = Convert.ToInt32(model.SearchDayOfBirth);
            if (!String.IsNullOrWhiteSpace(model.SearchMonthOfBirth))
                searchMonthOfBirth = Convert.ToInt32(model.SearchMonthOfBirth);

            var customers = _customerService.GetAllCustomers(
                customerRoleIds: model.SearchCustomerRoleIds,
                email: model.SearchEmail,
                username: model.SearchUsername,
                firstName: model.SearchFirstName,
                lastName: model.SearchLastName,
                dayOfBirth: searchDayOfBirth,
                monthOfBirth: searchMonthOfBirth,
                company: model.SearchCompany,
                phone: model.SearchPhone,
                zipPostalCode: model.SearchZipPostalCode,
                loadOnlyWithShoppingCart: false);

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    _exportManager.ExportCustomersToXlsx(stream, customers);
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "customers.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public ActionResult ExportExcelSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customers = new List<Customer>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                customers.AddRange(_customerService.GetCustomersByIds(ids));
            }

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _exportManager.ExportCustomersToXlsx(stream, customers);
                bytes = stream.ToArray();
            }
            return File(bytes, "text/xls", "customers.xlsx");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("exportxml-all")]
        public ActionResult ExportXmlAll(CustomerListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var searchDayOfBirth = 0;
            int searchMonthOfBirth = 0;
            if (!String.IsNullOrWhiteSpace(model.SearchDayOfBirth))
                searchDayOfBirth = Convert.ToInt32(model.SearchDayOfBirth);
            if (!String.IsNullOrWhiteSpace(model.SearchMonthOfBirth))
                searchMonthOfBirth = Convert.ToInt32(model.SearchMonthOfBirth);

            var customers = _customerService.GetAllCustomers(
                customerRoleIds: model.SearchCustomerRoleIds,
                email: model.SearchEmail,
                username: model.SearchUsername,
                firstName: model.SearchFirstName,
                lastName: model.SearchLastName,
                dayOfBirth: searchDayOfBirth,
                monthOfBirth: searchMonthOfBirth,
                company: model.SearchCompany,
                phone: model.SearchPhone,
                zipPostalCode: model.SearchZipPostalCode,
                loadOnlyWithShoppingCart: false);

            try
            {
                var xml = _exportManager.ExportCustomersToXml(customers);
                return new XmlDownloadResult(xml, "customers.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public ActionResult ExportXmlSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customers = new List<Customer>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                customers.AddRange(_customerService.GetCustomersByIds(ids));
            }

            var xml = _exportManager.ExportCustomersToXml(customers);
            return new XmlDownloadResult(xml, "customers.xml");
        }

        #endregion
    }
}
