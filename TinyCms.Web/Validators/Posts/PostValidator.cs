using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using TinyCms.Services.Localization;
using TinyCms.Web.Framework.Validators;
using TinyCms.Web.Models.Posts;

namespace TinyCms.Web.Validators.Posts
{
    public class PostValidator : BaseNopValidator<NewPostModel>
    {
        public PostValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage(localizationService.GetResource("Tiêu đề không được để trống"));
            RuleFor(x => x.Body).NotEmpty().WithMessage(localizationService.GetResource("Nội dung không được để trống"));
        }
    }
}