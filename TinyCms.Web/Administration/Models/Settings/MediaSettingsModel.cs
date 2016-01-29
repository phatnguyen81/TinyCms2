using TinyCms.Web.Framework;
using TinyCms.Web.Framework.Mvc;

namespace TinyCms.Admin.Models.Settings
{
    public class MediaSettingsModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Configuration.Settings.Media.PicturesStoredIntoDatabase")]
        public bool PicturesStoredIntoDatabase { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.AvatarPictureSize")]
        public int AvatarPictureSize { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.PostThumbPictureSize")]
        public int PostThumbPictureSize { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.PostDetailsPictureSize")]
        public int PostDetailsPictureSize { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.PostThumbPictureSizeOnPostDetailsPage")]
        public int PostThumbPictureSizeOnPostDetailsPage { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.AssociatedPostPictureSize")]
        public int AssociatedPostPictureSize { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.CategoryThumbPictureSize")]
        public int CategoryThumbPictureSize { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.ManufacturerThumbPictureSize")]
        public int ManufacturerThumbPictureSize { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.VendorThumbPictureSize")]
        public int VendorThumbPictureSize { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.CartThumbPictureSize")]
        public int CartThumbPictureSize { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.MiniCartThumbPictureSize")]
        public int MiniCartThumbPictureSize { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.MaximumImageSize")]
        public int MaximumImageSize { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.MultipleThumbDirectories")]
        public bool MultipleThumbDirectories { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Media.DefaultImageQuality")]
        public int DefaultImageQuality { get; set; }
    }
}