using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Web.Models.Customer
{
    public class UserInfoZoneModel:BaseNopModel
    {
        public bool IsAuthenticated { get; set; }
        public string AvatarUrl { get; set; }
    }
}