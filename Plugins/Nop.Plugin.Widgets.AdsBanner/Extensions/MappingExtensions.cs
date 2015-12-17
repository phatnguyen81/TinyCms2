using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Nop.Plugin.Widgets.AdsBanner.Domain;
using Nop.Plugin.Widgets.AdsBanner.Models;

namespace Nop.Plugin.Widgets.AdsBanner.Extensions
{
    public static class MappingExtensions
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }

        public static AdsBannerModel ToModel(this AdsBannerRecord entity)
        {
            return entity.MapTo<AdsBannerRecord, AdsBannerModel>();
        }

        public static AdsBannerRecord ToEntity(this AdsBannerModel model)
        {
            return model.MapTo<AdsBannerModel, AdsBannerRecord>();
        }

        public static AdsBannerRecord ToEntity(this AdsBannerModel model, AdsBannerRecord destination)
        {
            return model.MapTo(destination);
        }
    }
}
