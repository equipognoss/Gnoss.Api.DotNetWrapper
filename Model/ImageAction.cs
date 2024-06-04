using Gnoss.ApiWrapper.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Gnoss.ApiWrapper.Model
{
    /// <summary>
    ///  Represents a transformation to do over an image
    /// </summary>
    public class ImageAction
    {
        #region Constructor
        /// <summary>
        /// Constructor of <see cref="ImageAction"></see>
        /// </summary>
        /// <param name="size">Size in pixels of the width and height of the result image</param>
        /// <param name="imageTransformationType">Transformation to apply to the image</param>
        /// <param name="embedsRGB">Embed color space</param>
        public ImageAction(int size, ImageTransformationType imageTransformationType, bool embedsRGB = false)
        {
            Size = size;
            Width = size;
            Height = size;
            ImageTransformationType = imageTransformationType;
            ImageQualityPercentage = 100;
            EmbedsRGB = embedsRGB;
        }

        /// <summary>
        /// Constructor of <see cref="ImageAction"></see>
        /// </summary>
        /// <param name="size">Size in pixels of the width and height of the result image</param>
        /// <param name="imageTransformationType">Transformation to apply to the image</param>
        /// <param name="imageQualityPercentage">Minimum quality for the converted image (between 0 and 100)</param>
        /// <param name="embedsRGB">Embed color space</param>
        public ImageAction(int size, ImageTransformationType imageTransformationType, long imageQualityPercentage, bool embedsRGB = false)
        {
            Size = size;
            Width = size;
            Height = size;
            ImageTransformationType = imageTransformationType;
            ImageQualityPercentage = imageQualityPercentage;
            EmbedsRGB = embedsRGB;
        }

        /// <summary>
        /// Constructor of <see cref="ImageAction"></see>
        /// </summary>
        /// <param name="width">Width to resize</param>
        /// <param name="height">Height to resize</param>
        /// <param name="imageTransformationType">Transformation to apply to the image</param>
        /// <param name="embedsRGB">Embed color space</param>
        public ImageAction(float width, float height, ImageTransformationType imageTransformationType, bool embedsRGB = false)
        {
            Height = height;
            Width = width;
            Size = (int)width;
            ImageTransformationType = imageTransformationType;
            ImageQualityPercentage = 100;
            EmbedsRGB = embedsRGB;
        }

        /// <summary>
        /// Constructor of <see cref="ImageAction"></see>
        /// </summary>
        /// <param name="width">Width to resize</param>
        /// <param name="height">Height to resize</param>
        /// <param name="imageTransformationType">Transformation to apply to the image</param>
        /// <param name="imageQualityPercentage">Minimum quality for the converted image (between 0 and 100)</param>
        /// <param name="embedsRGB">Embed color space</param>
        public ImageAction(float width, float height, ImageTransformationType imageTransformationType, long imageQualityPercentage, bool embedsRGB = false)
        {
            Height = height;
            Width = width;
            Size = (int)width;
            ImageTransformationType = imageTransformationType;
            ImageQualityPercentage = imageQualityPercentage;
            EmbedsRGB = embedsRGB;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the size in pixels of the width and height of the result image
        /// </summary>
        public int Size
        {
            get; set;
        }

        /// <summary>
        /// Height, in pixels, that must have the image after the transformation
        /// </summary>
        public float Height
        {
            get; set;
        }

        /// <summary>
        /// Width, in pixels, that must have the image after the transformation
        /// </summary>
        public float Width
        {
            get; set;
        }

        /// <summary>
        /// Transformation to apply to the image
        /// </summary>
        public ImageTransformationType ImageTransformationType
        {
            get; set;
        }

        /// <summary>
        /// Minimum quality for the converted image (between 0 and 100)
        /// </summary>
        public long ImageQualityPercentage
        {
            get;
        }

        /// <summary>
        /// Embed color space
        /// </summary>
        public bool EmbedsRGB
        {
            get;
        }

        #endregion
    }
}
