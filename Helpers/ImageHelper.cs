using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Gnoss.ApiWrapper.Exceptions;
using NetVips;

namespace Gnoss.ApiWrapper.Helpers
{
    /// <summary>
    /// Utilities to use images. The images are received encoded (JPEG, PNG, GIF or WebP) and the transformed images are returned encoded as PNG
    /// </summary>
    public static class ImageHelper
    {
        #region Members

        /// <summary>
        /// Fragments of the libvips loader names allowed to decode images
        /// </summary>
        private static readonly string[] AllowedLoaders = { "jpeg", "png", "gif", "webp" };

        /// <summary>
        /// libvips metadata field of the EXIF ColorSpace tag
        /// </summary>
        private const string ExifColorSpaceField = "exif-ifd2-ColorSpace";

        /// <summary>
        /// Value of the EXIF ColorSpace tag that represents sRGB
        /// </summary>
        private const string ExifColorSpaceSRGB = "1";

        #endregion

        #region Methods

        /// <summary>
        /// Resize keeping the aspect ratio to width
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="widthInPixels">Width to resize</param>
        /// <param name="pResizeAlways">Indicate if the image will be resized always</param>
        /// <returns>Resized image encoded as PNG</returns>
        public static byte[] ResizeImageToWidth(byte[] image, int widthInPixels, bool pResizeAlways = false)
        {
            return TransformImage(image, source => ResizeToWidth(source, widthInPixels, pResizeAlways), "Error in resize");
        }

        /// <summary>
        /// Resize the image keeping the aspect ratio without exceeding the width or height indicated
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="widthInPixels" >Width to resize</param>
        /// <param name="heightInPixels">Height to resize</param>
        /// <returns>Resized image encoded as PNG</returns>
        public static byte[] ResizeImageToHeightAndWidth(byte[] image, int widthInPixels, int heightInPixels)
        {
            return TransformImage(image, source => ResizeToHeightAndWidth(source, widthInPixels, heightInPixels), "Error in resize");
        }

        /// <summary>
        /// Resize keeping the aspect ratio to height
        /// </summary>
        /// <param name="image">Image to resize</param>
        /// <param name="heightInPixels">Height to resize</param>
        /// <param name="pResizeAlways">Indicate if the image will be resized always</param>
        /// <returns>Resized image encoded as PNG</returns>
        public static byte[] ResizeImageToHeight(byte[] image, int heightInPixels, bool pResizeAlways = false)
        {
            return TransformImage(image, source => ResizeToHeight(source, heightInPixels, pResizeAlways), "Error in resize");
        }

        /// <summary>
        /// Resize to the indicated size, crop the image and take the top of the image if it is vertical, or the central part if its horizontal
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="squareSize">Size in pixels of the width and height of the result image</param>
        /// <returns>Cropped image encoded as PNG</returns>
        public static byte[] CropImageToSquare(byte[] image, int squareSize)
        {
            return TransformImage(image, source => CropToSquare(source, squareSize), "Error in crop");
        }

        /// <summary>
        /// Resize to the indicated size, crop the image and take the top of the image if it is vertical, or the central part if its horizontal
        /// </summary>
        /// <param name="pImage">Image</param>
        /// <param name="pHeight">Height of the image</param>
        /// <param name="pWidth">Width of the image</param>
        /// <returns>Cropped image encoded as PNG</returns>
        public static byte[] CropImageToHeightAndWidth(byte[] pImage, int pHeight, int pWidth)
        {
            return TransformImage(pImage, source => CropToHeightAndWidth(source, pHeight, pWidth), "Error in crop");
        }

        /// <summary>
        /// Converts an image to PNG
        /// </summary>
        /// <param name="pImage">Image to convert</param>
        /// <returns>Image encoded as PNG</returns>
        public static byte[] ConvertToPng(byte[] pImage)
        {
            return TransformImage(pImage, source => source, "Imposible to convert the image to byte[]");
        }

        /// <summary>
        /// Converts an image to PNG, with a minimum quality
        /// </summary>
        /// <param name="pImage">Image to convert</param>
        /// <param name="pQuality">Minimum quality for the converted image</param>
        /// <returns>Image encoded as PNG</returns>
        public static byte[] ConvertToPng(byte[] pImage, int pQuality)
        {
            if (pQuality == int.MinValue)
            {
                // Convert without minimum quality
                return ConvertToPng(pImage);
            }
            else
            {
                return TransformImage(pImage, ToRgbWithAlpha, "Imposible to convert the image to byte[]");
            }
        }

        /// <summary>
        /// Download a image from a url
        /// </summary>
        /// <param name="imageUrl">Url of the image</param>
        /// <returns>Downloaded image</returns>
        public static byte[] DownloadImageFromUrl(string imageUrl)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("UserAgent", GnossApiWrapper.GenerarUserAgent());
                byte[] imageContent = httpClient.GetByteArrayAsync(imageUrl).Result;
                EnsureSupportedFormat(imageContent);
                return imageContent;
            }
        }

        /// <summary>
        /// Download a image from a url or a local path
        /// </summary>
        /// <param name="imageUrlOrPath">Url or local path of the image</param>
        /// <returns>Image</returns>
        internal static byte[] ReadImageFromUrlOrLocalPath(string imageUrlOrPath)
        {
            byte[] image = null;
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
                        image = File.ReadAllBytes(imageUrlOrPath);
                        EnsureSupportedFormat(image);
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
        /// Gets the width of an image without decoding its pixels
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns>Width in pixels</returns>
        internal static int GetImageWidth(byte[] image)
        {
            using (Image source = LoadImage(image))
            {
                return source.Width;
            }
        }

        /// <summary>
        /// Property assigned to the EXIF sRGB Color Space
        /// </summary>
        /// <param name="image">Image</param>
        /// <returns>Image with color space encoded as PNG</returns>
        public static byte[] AssignEXIFPropertyColorSpaceSRGB(byte[] image)
        {
            return TransformImage(image, source => source.Mutate(mutableImage => mutableImage.Set(GValue.GStrType, ExifColorSpaceField, ExifColorSpaceSRGB)), "Error assigning the EXIF color space");
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Decodes an image, applies a transformation and encodes the result as PNG
        /// </summary>
        /// <param name="image">Image to transform</param>
        /// <param name="transformation">Transformation to apply</param>
        /// <param name="errorMessage">Message of the exception if the transformation fails</param>
        /// <returns>Transformed image encoded as PNG</returns>
        private static byte[] TransformImage(byte[] image, Func<Image, Image> transformation, string errorMessage)
        {
            using (Image source = LoadImage(image))
            {
                try
                {
                    return EncodePng(transformation(source));
                }
                catch (Exception ex)
                {
                    throw new GnossAPIException($"{errorMessage}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Encodes an image as PNG
        /// </summary>
        /// <param name="image">Image to encode</param>
        /// <returns>Image encoded as PNG</returns>
        private static byte[] EncodePng(Image image)
        {
            // libvips adds a default EXIF block when saving, so it is only kept if the image already has EXIF metadata
            bool hasExif = image.GetFields().Any(field => field.Equals("exif-data") || field.StartsWith("exif-ifd"));
            Enums.ForeignKeep keep = hasExif ? Enums.ForeignKeep.All : Enums.ForeignKeep.All & ~Enums.ForeignKeep.Exif;

            return image.WriteToBuffer(".png", new VOption { { "keep", keep } });
        }

        /// <summary>
        /// Decodes an image, only if its format is allowed
        /// </summary>
        /// <param name="image">Image to decode</param>
        /// <returns>Decoded image</returns>
        private static Image LoadImage(byte[] image)
        {
            EnsureSupportedFormat(image);

            try
            {
                return Image.NewFromBuffer(image);
            }
            catch (Exception ex)
            {
                throw new GnossAPIException($"Imposible to convert the byte[] to image : {ex.Message}");
            }
        }

        /// <summary>
        /// Checks the format of the image by its signature, before decoding it
        /// </summary>
        /// <param name="image">Image to check</param>
        private static void EnsureSupportedFormat(byte[] image)
        {
            string loader = image != null && image.Length > 0 ? Image.FindLoadBuffer(image) : null;

            if (string.IsNullOrEmpty(loader) || !AllowedLoaders.Any(allowedLoader => loader.Contains(allowedLoader, StringComparison.OrdinalIgnoreCase)))
            {
                throw new GnossAPIException("The image format is not supported. The allowed formats are JPEG, PNG, GIF and WebP");
            }
        }

        private static Image ResizeToWidth(Image image, int widthInPixels, bool resizeAlways)
        {
            if (resizeAlways || widthInPixels <= image.Width)
            {
                float aspectRatio = (float)image.Width / (float)image.Height;
                float newHeight = widthInPixels / aspectRatio;
                return Resize(image, widthInPixels, (int)newHeight);
            }
            return image;
        }

        private static Image ResizeToHeight(Image image, int heightInPixels, bool resizeAlways)
        {
            if (resizeAlways || heightInPixels <= image.Height)
            {
                float aspectRatio = (float)image.Width / (float)image.Height;
                float newWidth = heightInPixels * aspectRatio;
                return Resize(image, (int)newWidth, heightInPixels);
            }
            return image;
        }

        private static Image ResizeToHeightAndWidth(Image image, int widthInPixels, int heightInPixels)
        {
            float aspectRatio = (float)image.Width / (float)image.Height;
            if (widthInPixels <= image.Width)
            {
                float newHeight = widthInPixels / aspectRatio;
                if (heightInPixels <= (int)newHeight)
                {
                    aspectRatio = widthInPixels / newHeight;
                    float newWidth = heightInPixels * aspectRatio;
                    return Resize(image, (int)newWidth, heightInPixels);
                }
                return Resize(image, widthInPixels, (int)newHeight);
            }
            else if (heightInPixels <= image.Height)
            {
                float newWidth = heightInPixels * aspectRatio;
                return Resize(image, (int)newWidth, heightInPixels);
            }
            return image;
        }

        private static Image CropToSquare(Image image, int squareSize)
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
                        image = ResizeToWidth(image, squareSize, false);
                        return image.Crop(0, 0, squareSize, squareSize);
                    }
                    else if (image.Height > squareSize)
                    {
                        return image.Crop(0, 0, image.Width, squareSize);
                    }
                }
                else if (isHorizontal)
                {
                    if (image.Height >= squareSize)
                    {
                        image = ResizeToHeight(image, squareSize, false);
                        return image.Crop((image.Width - squareSize) / 2, 0, squareSize, squareSize);
                    }
                    else if (image.Width > squareSize)
                    {
                        return image.Crop((image.Width - squareSize) / 2, 0, squareSize, image.Height);
                    }
                }
                else
                {
                    //If the image isn't vertical and isn't horizontal, have to be a square
                    return ResizeToWidth(image, squareSize, false);
                }
            }
            return image;
        }

        private static Image CropToHeightAndWidth(Image image, int height, int width)
        {
            float desiredAspectRatio = (float)height / width;
            float aspectRatio = (float)image.Height / image.Width;

            if (aspectRatio < desiredAspectRatio)
            {
                image = ResizeToHeight(image, height, true);
                return image.Crop((image.Width - width) / 2, 0, width, height);
            }
            else
            {
                image = ResizeToWidth(image, width, true);
                return image.Crop(0, (image.Height - height) / 2, width, height);
            }
        }

        /// <summary>
        /// Resize the image to the exact width and height indicated
        /// </summary>
        private static Image Resize(Image image, int width, int height)
        {
            double horizontalScale = (double)width / image.Width;
            double verticalScale = (double)height / image.Height;

            if (!image.HasAlpha())
            {
                return image.Resize(horizontalScale, Enums.Kernel.Cubic, vscale: verticalScale);
            }

            // Premultiply the alpha channel to avoid dark halos around the transparent areas
            return image.Premultiply()
                .Resize(horizontalScale, Enums.Kernel.Cubic, vscale: verticalScale)
                .Unpremultiply()
                .Cast(image.Format);
        }

        /// <summary>
        /// Converts the image to RGB with alpha channel
        /// </summary>
        private static Image ToRgbWithAlpha(Image image)
        {
            if (image.Interpretation != Enums.Interpretation.Srgb && image.Interpretation != Enums.Interpretation.Rgb16)
            {
                image = image.Colourspace(Enums.Interpretation.Srgb);
            }

            if (!image.HasAlpha())
            {
                image = image.Bandjoin(image.Format == Enums.BandFormat.Ushort ? 65535 : 255);
            }

            return image;
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
