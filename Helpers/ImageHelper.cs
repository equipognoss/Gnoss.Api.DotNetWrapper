using System;
using System.IO;
using System.Net;
using Gnoss.ApiWrapper.Exceptions;
using System.Runtime.Serialization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats;
using System.Net.Http;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

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
        /// <param name="pResizeAlways">Indicate if the image will be resized always</param>
        public static void ResizeImageToWidth(Image image, int widthInPixels, bool pResizeAlways = false)
        {
            try
            {
                float aspectRatio = image.Height / image.Width;

                if (pResizeAlways || widthInPixels <= image.Height)
                {
                    float newHeight = widthInPixels / aspectRatio;
                    image.Mutate(image => image.Resize(widthInPixels, (int)newHeight));
                }
            }
            catch (Exception ex)
            {
                throw new GnossAPIException($"Error in resize: {ex.Message}");
            }
        }

        /// <summary>
        /// Resize the image keeping the aspect ratio without exceeding the width or height indicated
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="widthInPixels" >Width to resize</param>
        /// <param name="heightInPixels">Height to resize</param>
        public static void ResizeImageToHeightAndWidth(Image image, int widthInPixels, int heightInPixels)
        {
            float aspectRatio = image.Width / image.Height;
            if (widthInPixels <= image.Width)
            {
                try
                {
                    float newHeight = widthInPixels / aspectRatio;
                    image.Mutate(image => image.Resize(widthInPixels, (int)newHeight));
                    if (heightInPixels <= image.Height)
                    {
                        aspectRatio = widthInPixels / newHeight;
                        float newWidth = heightInPixels * aspectRatio;
                        image.Mutate(image => image.Resize((int)newWidth, heightInPixels));
                    }
                }
                catch (Exception ex)
                {
                    throw new GnossAPIException($"Error in resize: {ex.Message}");
                }
            }
            else
            {
                if (heightInPixels <= image.Height)
                {
                    try
                    {
                        float newWidth = heightInPixels * aspectRatio;
                        image.Mutate(image => image.Resize((int)newWidth, heightInPixels));
                    }
                    catch (Exception ex)
                    {
                        throw new GnossAPIException($"Error in resize: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Resize keeping the aspect ratio to height
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="heightInPixels">Height to resize</param>
        /// <param name="pResizeAlways">Indicate if the image will be resized always</param>
        public static void ResizeImageToHeight(Image image, int heightInPixels, bool pResizeAlways = false)
        {
            float aspectRatio = image.Width / image.Height;

            if (pResizeAlways || heightInPixels <= image.Height)
            {
                float newWidth = heightInPixels * aspectRatio;
                image.Mutate(image => image.Resize((int)newWidth, heightInPixels));
            }
        }

        /// <summary>
        /// Resize to the indicated size, crop the image and take the top of the image if it is vertical, or the central part if its horizontal
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="squareSize">Size in pixels of the width and height of the result image</param>
        public static void CropImageToSquare(Image image, int squareSize)
        {
            if (image.Height > squareSize && image.Width > squareSize)
            {
                bool isVertical = false;
                bool isHorizontal = false;
                float aspectRatio = (float)image.Width / image.Height;
                if (aspectRatio < 1)
                {
                    isVertical = true;
                }
                else if (aspectRatio > 1)
                {
                    isHorizontal = true;
                }

                if (isVertical)
                {
                    if (image.Width >= squareSize)
                    {
                        ResizeImageToWidth(image, squareSize);
                        Point origin = new Point(0, 0);
                        Size size = new Size(squareSize, squareSize);
                        Rectangle rectangle = new Rectangle(origin, size);
                        image.Mutate(image => image.Crop(rectangle));
                    }
                    else if (image.Height > squareSize)
                    {
                        Point origin = new Point(0, 0);
                        Size size = new Size(image.Width, squareSize);
                        Rectangle rectangle = new Rectangle(origin, size);
                        image.Mutate(image => image.Crop(rectangle));
                    }
                }
                else if (isHorizontal)
                {
                    if (image.Height >= squareSize)
                    {
                        ResizeImageToHeight(image, squareSize);
                        Point origin = new Point(Convert.ToInt32(image.Width - squareSize) / 2, 0);
                        Size size = new Size(squareSize, squareSize);
                        Rectangle rectangle = new Rectangle(origin, size);
                        image.Mutate(image => image.Crop(rectangle));
                    }
                    else if (image.Width > squareSize)
                    {
                        Point origin = new Point(Convert.ToInt32(image.Width - squareSize) / 2, 0);
                        Size size = new Size(squareSize, image.Height);
                        Rectangle rectangle = new Rectangle(origin, size);
                        image.Mutate(image => image.Crop(rectangle));
                    }
                }
                else
                {
                    //If the image isn't vertical and isn't horizontal, have to be a square
                    ResizeImageToWidth(image, squareSize);
                }
            }
        }

        /// <summary>
        /// Resize to the indicated size, crop the image and take the top of the image if it is vertical, or the central part if its horizontal
        /// </summary>
        /// <param name="pImage">Image</param>
        /// <param name="pHeight">Height of the image</param>
        /// <param name="pWidth">Width of the image</param>       
        public static void CropImageToHeightAndWidth(Image pImage, int pHeight, int pWidth)
        {
            float aspcetRatioDeseado = pHeight / pWidth;
            float aspcetRatio = pImage.Height / pImage.Width;

            if (aspcetRatio < aspcetRatioDeseado)
            {
                ResizeImageToHeight(pImage, pHeight, true);
                Point origin = new Point(Convert.ToInt32(pImage.Width - pWidth) / 2, 0);
                Size size = new Size(pWidth, pHeight);
                Rectangle rectangle = new Rectangle(origin, size);
                pImage.Mutate(image => image.Crop(rectangle));
            }
            else
            {
                ResizeImageToWidth(pImage, pWidth, true);
                Point origin = new Point(0, Convert.ToInt32(pImage.Height - pHeight) / 2);
                Size size = new Size(pWidth, pHeight);
                Rectangle rectangle = new Rectangle(origin, size);
                pImage.Mutate(image => image.Crop(rectangle));
            }
        }

        /// <summary>
        /// Converts Image to byte[]
        /// </summary>
        /// <param name="pImage">Image to convert to byte[]</param>
        /// <returns>byte[] converted from <paramref name="pImage"/></returns>
        public static byte[] ImageToByteArray(Image pImage)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    pImage.Save(ms, new PngEncoder());
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new GnossAPIException($"Imposible to convert the image to byte[]: {ex.Message}");
            }
        }

        /// <summary>
        /// Converts image to byte[], with a minimum quality
        /// </summary>
        /// <param name="pImage">Image to convert to byte[]</param>
        /// <param name="pQuality">Minimum quality for the converted image</param>
        /// <returns>byte[] converted from <paramref name="pImage"/></returns>
        public static byte[] ImageToByteArray(Image pImage, int pQuality)
        {
            if (pQuality == int.MinValue)
            {
                // Convert without minimum quality
                return ImageToByteArray(pImage);
            }
            else
            {
                JpegEncoder encoder = new JpegEncoder();
                encoder.Quality = pQuality;

                try
                {
                    MemoryStream ms = new MemoryStream();
                    pImage.Save(ms, encoder);
                    return ms.ToArray();
                }
                catch (Exception ex)
                {
                    throw new GnossAPIException($"Imposible to convert the image to byte[]: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Download a image from a url
        /// </summary>
        /// <param name="imageUrl">Url of the image</param>
        /// <returns>Image</returns>
        public static Image DownloadImageFromUrl(string imageUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                byte[] imageContent = httpClient.GetByteArrayAsync(imageUrl).Result;
                return Image.Load(imageContent);
            }
        }

        /// <summary>
        /// Converts ByteArray to Image
        /// </summary>
        /// <param name="byteArray">Bytearray of the image</param>
        /// <returns>Image</returns>
        public static Image ByteArrayToImage(byte[] byteArray)
        {
            try
            {
               return Image.Load(byteArray);
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
        internal static Image ReadImageFromUrlOrLocalPath(string imageUrlOrPath)
        {
            Image image = null;
            if (Uri.IsWellFormedUriString(imageUrlOrPath, UriKind.Absolute))
            {
                image = DownloadImageFromUrl(imageUrlOrPath);
            }
            else
            {
                if (File.Exists(imageUrlOrPath))
                {
                    try
                    {
                        image = Image.Load(imageUrlOrPath);
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
        public static void AssignEXIFPropertyColorSpaceSRGB(Image image)
        {
            image.Metadata.ExifProfile.SetValue<ushort>(ExifTag.ColorSpace, 1);
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
