using System.Web;

namespace TinyCms.Services.Media
{
    /// <summary>
    ///     Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        ///     Gets the download binary array
        /// </summary>
        /// <param name="postedFile">Posted file</param>
        /// <returns>Download binary array</returns>
        public static byte[] GetDownloadBits(this HttpPostedFileBase postedFile)
        {
            var fs = postedFile.InputStream;
            var size = postedFile.ContentLength;
            var binary = new byte[size];
            fs.Read(binary, 0, size);
            return binary;
        }

        /// <summary>
        ///     Gets the picture binary array
        /// </summary>
        /// <param name="postedFile">Posted file</param>
        /// <returns>Picture binary array</returns>
        public static byte[] GetPictureBits(this HttpPostedFileBase postedFile)
        {
            var fs = postedFile.InputStream;
            var size = postedFile.ContentLength;
            var img = new byte[size];
            fs.Read(img, 0, size);
            return img;
        }
    }
}