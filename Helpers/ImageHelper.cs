using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing.Imaging;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Gnoss.ApiWrapper.Helpers;
using Gnoss.ApiWrapper.Exceptions;
using System.Runtime.Serialization;
using System.Drawing;

namespace Gnoss.ApiWrapper.Helpers
{
    /// <summary>
    /// Utilities to use images
    /// </summary>
    public static class ImageHelper
    {
        #region Methods

        /// <summary>
        /// Resize keeping the aspect ratio to width
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="widthInPixels">Width to resize</param>
        /// <returns>New image with width = <paramref name="widthInPixels"/></returns>
        public static Bitmap ResizeImageToWidth(Bitmap image, float widthInPixels, bool pResizeAlways = false)
        {
            try
            {
                float width = image.Height;
                float height = image.Width;
                float aspectRatio = height / width;
                Bitmap resultImage = null;

                if (pResizeAlways || widthInPixels <= height)
                {
                    float newHeight = widthInPixels / aspectRatio;
                    Size size = new SizeF(widthInPixels, newHeight).ToSize();
                    resultImage = new Bitmap(image, size);
                }
                else
                {
                    resultImage = image;
                    //if (_logHelper != null)
                    //{
                    //    _logHelper.Info("The original image has less width than widthInPixels. Return the original image", ClassName);
                    //}
                }
                return resultImage;
            }
            catch (Exception ex)
            {

                throw new GnossAPIException($"Error in resize: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Resize the image keeping the aspect ratio without exceeding the width or height indicated
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="widthInPixels" >Width to resize</param>
        /// <param name="heightInPixels">Height to resize</param>
        /// <returns>New image with width = <paramref name="widthInPixels"/> and height = <paramref name="heightInPixels"/></returns>
        public static Bitmap ResizeImageToHeightAndWidth(Bitmap image, float widthInPixels, float heightInPixels)
        {
            float height = image.Height;
            float width = image.Width;
            float aspectRatio = width / height;
            Bitmap resultImage = null;
            if ((widthInPixels <= width))
            {
                try
                {
                    float newHeiht = widthInPixels / aspectRatio;
                    Size size = new SizeF(widthInPixels, newHeiht).ToSize();
                    resultImage = new Bitmap(image, size);
                    height = resultImage.Height;
                    width = resultImage.Width;
                    if (heightInPixels <= height)
                    {
                        aspectRatio = width / height;
                        float nuevoAncho = heightInPixels * aspectRatio;
                        size = new SizeF(nuevoAncho, heightInPixels).ToSize();
                        resultImage = new Bitmap(image, size);
                    }

                }
                catch (Exception ex)
                {
                    throw new GnossAPIException($"Error in resize: {ex.Message}");
                }
            }
            else
            {
                if (heightInPixels <= height)
                {
                    try
                    {
                        float newWidth = heightInPixels * aspectRatio;
                        Size size = new SizeF(newWidth, heightInPixels).ToSize();
                        resultImage = new Bitmap(image, size);
                    }
                    catch (Exception ex)
                    {
                        throw new GnossAPIException($"Error in resize: {ex.Message}");
                    }
                }
                else
                {
                    resultImage = image;
                }
            }
            return resultImage;
        }

        /// <summary>
        /// Resize keeping the aspect ratio to height
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="heightInPixels">Height to resize</param>
        /// <returns>New image with height = <paramref name="heightInPixels"/></returns>
        public static Bitmap ResizeImageToHeight(Bitmap image, float heightInPixels, bool pResizeAlways = false)
        {
            float height = image.Height;
            float width = image.Width;
            float aspectRatio = width / height;
            Bitmap resultImage = null;

            if (pResizeAlways || heightInPixels <= height)
            {
                float newWidth = heightInPixels * aspectRatio;
                Size size = new SizeF(newWidth, heightInPixels).ToSize();
                resultImage = new Bitmap(image, size);
            }
            else
            {
                resultImage = image;
            }
            return resultImage;
        }

        /// <summary>
        /// Resize to the indicated size, crop the image and take the top of the image if it is vertical, or the central part if its horizontal
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="squareSize">Size in pixels of the width and height of the result image</param>
        /// <returns>Square image</returns>
        public static Bitmap CropImageToSquare(Bitmap image, float squareSize)
        {
            Bitmap resultImage = null;
            float height = image.Height;
            float width = image.Width;
            if (height <= squareSize && width <= squareSize)
            {
                resultImage = image;
                return resultImage;
            }
            else
            {
                bool isVertical = false;
                bool isHorizontal = false;
                float aspcetRatio = width / height;
                if (aspcetRatio < 1)
                {
                    isVertical = true;
                }
                else if (aspcetRatio > 1)
                {
                    isHorizontal = true;
                }

                if (isVertical)
                {
                    if (width >= squareSize)
                    {
                        Bitmap resizedImage = ResizeImageToWidth(image, squareSize);
                        Point origin = new Point(0, 0);
                        Size size = new SizeF(squareSize, squareSize).ToSize();
                        Rectangle rectangle = new Rectangle(origin, size);
                        Bitmap cropedImage = resizedImage.Clone(rectangle, resizedImage.PixelFormat);
                        resultImage = cropedImage;
                    }
                    else if (height > squareSize)
                    {
                        Point origin = new Point(0, 0);
                        Size size = new SizeF(width, squareSize).ToSize();
                        Rectangle rectangle = new Rectangle(origin, size);
                        Bitmap cropedImage = image.Clone(rectangle, image.PixelFormat);
                        resultImage = cropedImage;
                    }
                }
                else if (isHorizontal)
                {
                    if (height >= squareSize)
                    {
                        Bitmap resizedImage = ResizeImageToHeight(image, squareSize);
                        Point origin = new Point(Convert.ToInt32(resizedImage.Width - squareSize) / 2, 0);
                        Size size = new SizeF(squareSize, squareSize).ToSize();
                        Rectangle rectangle = new Rectangle(origin, size);
                        Bitmap cropedImage = resizedImage.Clone(rectangle, resizedImage.PixelFormat);
                        resultImage = cropedImage;
                    }
                    else if (width > squareSize)
                    {
                        Point origin = new Point(Convert.ToInt32(image.Width - squareSize) / 2, 0);
                        Size size = new SizeF(squareSize, height).ToSize();
                        Rectangle rectangle = new Rectangle(origin, size);
                        Bitmap cropedImage = image.Clone(rectangle, image.PixelFormat);
                        resultImage = cropedImage;
                    }
                }
                else
                {
                    //If the image isn't vertical and isn't horizontal, have to be a square
                    Bitmap squareImage = ResizeImageToWidth(image, squareSize);
                    resultImage = squareImage;
                }

                return resultImage;
            }
        }

        /// <summary>
        /// Resize to the indicated size, crop the image and take the top of the image if it is vertical, or the central part if its horizontal
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="squareSize">Size in pixels of the width and height of the result image</param>
        /// <returns>Square image</returns>
        public static Bitmap CropImageToHeightAndWidth(Bitmap image, float pHeight, float pWidth)
        {
            float aspcetRatioDeseado = pHeight / pWidth;


            Bitmap resultImage = null;
            float height = image.Height;
            float width = image.Width;
            float aspcetRatio = height / width;

            if (aspcetRatio < aspcetRatioDeseado)
            {
                Bitmap resizedImage = ResizeImageToHeight(image, pHeight, true);
                Point origin = new Point(Convert.ToInt32(resizedImage.Width - pWidth) / 2, 0);
                Size size = new SizeF(pWidth, pHeight).ToSize();
                Rectangle rectangle = new Rectangle(origin, size);
                Bitmap cropedImage = resizedImage.Clone(rectangle, resizedImage.PixelFormat);
                resultImage = cropedImage;
            }
            else
            {
                Bitmap resizedImage = ResizeImageToWidth(image, pWidth, true);
                Point origin = new Point(0, Convert.ToInt32(resizedImage.Height - pHeight) / 2);
                Size size = new SizeF(pWidth, pHeight).ToSize();
                Rectangle rectangle = new Rectangle(origin, size);
                Bitmap cropedImage = resizedImage.Clone(rectangle, resizedImage.PixelFormat);
                resultImage = cropedImage;
            }
            return resultImage;

        }

        /// <summary>
        /// Converts Bitmap to byte[]
        /// </summary>
        /// <param name="bitmap">Bitmap to convert to byte[]</param>
        /// <returns>byte[] converted from <paramref name="bitmap"/></returns>
        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            byte[] buffer = null;

            try
            {
                ImageConverter converter = new ImageConverter();
                buffer = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
            }
            catch (Exception ex)
            {
                throw new GnossAPIException($"Imposible to convert the image to byte[]: {ex.Message}");
            }
            return buffer;
        }

        /// <summary>
        /// Converts Bitmap to byte[], with a minimum quality
        /// </summary>
        /// <param name="bitmap">Bitmap to convert to byte[]</param>
        /// <param name="quality">Minimum quality for the converted image</param>
        /// <returns>byte[] converted from <paramref name="bitmap"/></returns>
        public static byte[] BitmapToByteArray(Bitmap bitmap, long quality)
        {
            byte[] buffer = null;

            if (quality == long.MinValue)
            {
                // Convert without minimum quality
                buffer = BitmapToByteArray(bitmap);
            }
            else
            {
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

                // Create new EncoderPrameters with one EncoderParameters object in the array
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
                myEncoderParameters.Param[0] = myEncoderParameter;

                try
                {
                    MemoryStream ms = new MemoryStream();
                    bitmap.Save(ms, jpgEncoder, myEncoderParameters);
                    buffer = ms.ToArray();
                    ms.Dispose();
                }
                catch (Exception ex)
                {
                    throw new GnossAPIException($"Imposible to convert the image to byte[]: {ex.Message}");
                }
            }
            return buffer;
        }

        /// <summary>
        /// Download a image from a url
        /// </summary>
        /// <param name="imageUrl">Url of the image</param>
        /// <returns>Image</returns>
        public static Bitmap DownloadImageFromUrl(string imageUrl)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(imageUrl);
            Bitmap image = new Bitmap(stream);
            stream.Flush();
            stream.Close();
            client.Dispose();
            return image;
        }

        /// <summary>
        /// Gets a specific encoder
        /// </summary>
        /// <param name="format">Format of the encoder</param>
        /// <returns>Encoder</returns>
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        /// <summary>
        /// Converts ByteArray to bitmap
        /// </summary>
        /// <param name="byteArray">Bytearray of the image</param>
        /// <returns>Image</returns>
        public static Bitmap ByteArrayToBitmap(byte[] byteArray)
        {
            try
            {
                MemoryStream imagestream = new MemoryStream(byteArray);
                Bitmap bitmap1 = new Bitmap(imagestream, true);
                return bitmap1;
            }
            catch (Exception ex)
            {
                throw new GnossAPIException($"Imposible to convert the byte[] to image : {ex.Message}");
            }
        }

        /// <summary>
        /// Download a image from a url or a local path
        /// </summary>
        /// <param name="imageUrlOrPath">Url or local path of the image</param>
        /// <returns>Image</returns>
        internal static Bitmap ReadImageFromUrlOrLocalPath(string imageUrlOrPath)
        {
            Bitmap image = null;
            if (Uri.IsWellFormedUriString(imageUrlOrPath, UriKind.Absolute))
            {
                image = ImageHelper.DownloadImageFromUrl(imageUrlOrPath);
            }
            else
            {
                if (File.Exists(imageUrlOrPath))
                {
                    try
                    {
                        image = new Bitmap(imageUrlOrPath);
                    }
                    catch (Exception ex)
                    {
                        throw new GnossAPIException($"Error reading the image {imageUrlOrPath}: {ex.Message}");
                    }
                }
                else
                {
                    throw new GnossAPIException($"The image { imageUrlOrPath } doesn't exists or the application couldn't access");
                }
            }
            return image;
        }

        /// <summary>
        /// Property assigned to the EXIF sRGB Color Space
        /// <param name="image">Image</param>
        /// <returns>Image with color space</returns>
        public static Bitmap AssignEXIFPropertyColorSpaceSRGB(Bitmap image)
        {
            // See http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/EXIF.html for information about Exif tags
            // See https://msdn.microsoft.com/en-us/library/system.drawing.imaging.propertyitem.id(v=vs.110).aspx for information about how Visual Studio manages Exif properties

            DateTime time = DateTime.Now;
            var prop = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem));
            // Color space assignment. Values: 65535 for uncalibrated,  1 for sRGB.
            SetProperty(ref prop, 40961, "1");
            image.SetPropertyItem(prop);
            return image;
        }

        /// <summary>
        /// Method of writing image Exif tags.
        /// </summary>
        /// <param name="prop">Image property list</param>
        /// <param name="iId">Property ID</param>
        /// <param name="sTxt">Property value</param>
        private static void SetProperty(ref PropertyItem prop, int iId, string sTxt)
        {
            int iLen = sTxt.Length + 1;
            byte[] bTxt = new Byte[iLen];
            for (int i = 0; i < iLen - 1; i++)
                bTxt[i] = (byte)sTxt[i];
            bTxt[iLen - 1] = 0x00;
            prop.Id = iId;
            prop.Type = 2;
            prop.Value = bTxt;
            prop.Len = iLen;
        }

        #endregion

        #region Properties

        private static string ClassName
        {
            get
            {
                return typeof(ImageHelper).Name;
            }
        }

        #endregion
    }
}
