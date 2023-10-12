using System.Drawing;

namespace Utilities.Utilities
{
    public static class ImageUtils
    {
        public static Image ResizeImage(Image imgToResize, Size size)
        {
            return new Bitmap(imgToResize, size);
        }
    }
}
