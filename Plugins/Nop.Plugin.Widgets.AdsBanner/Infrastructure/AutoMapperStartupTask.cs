using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Nop.Plugin.Widgets.AdsBanner.Domain;
using Nop.Plugin.Widgets.AdsBanner.Models;
using TinyCms.Core.Infrastructure;

namespace Nop.Plugin.Widgets.AdsBanner.Infrastructure
{
    public class AutoMapperStartupTask : IStartupTask
    {
        public void Execute()
        {
            //category template
            Mapper.CreateMap<AdsBannerRecord, AdsBannerModel>();
            Mapper.CreateMap<AdsBannerModel, AdsBannerRecord>();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
