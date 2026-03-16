using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.Helpers;
using Gnoss.ApiWrapper.OAuth;
using NetVips;


namespace Gnoss.Apiwrapper.GenerateTiles
{

    /// <summary>
    /// Generates Deep Zoom tiles from a local image using libvips.
    /// Optimized for speed on large/high-resolution images.
    /// Only levels >= MinLevel are kept (levels 0..MinLevel-1 are deleted after generation).
    /// </summary>
    public class ImageTessellator
    {
        #region Constants

        /// <summary>
        /// Lowest Deep Zoom level that is kept. Levels 0 … MinLevel-1 are deleted.
        /// </summary>
        public const int MinLevel = 8;

        #endregion

        #region Constructor

        private readonly int _tileSize;
        private readonly int _overlap;
        private readonly int _quality;

        /// <summary>
        /// Creates an <see cref="ImageTessellator"/> with default settings
        /// (tileSize=256, overlap=1, quality=85).
        /// </summary>
        public ImageTessellator() : this(256, 1, 85) { }

        /// <summary>
        /// Creates an <see cref="ImageTessellator"/> with custom settings.
        /// </summary>
        public ImageTessellator(int tileSize, int overlap, int quality)
        {
            if (tileSize <= 0) throw new ArgumentOutOfRangeException(nameof(tileSize));
            if (overlap < 0) throw new ArgumentOutOfRangeException(nameof(overlap));
            if (quality < 1 || quality > 100) throw new ArgumentOutOfRangeException(nameof(quality));

            _tileSize = tileSize;
            _overlap = overlap;
            _quality = quality;

            // ── libvips global config ─────────────────────────────────────────
            // Disable operation cache: Dzsave generates thousands of unique tiles,
            // caching them wastes RAM without any reuse benefit.
            Cache.Max = 0;
            Cache.MaxMem = 0;

            // Use all available cores. libvips parallelises Dzsave internally
            // across tile rows — more threads = faster on large images.
            Cache.MaxFiles = 0;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Tiles the image at <paramref name="sourceImagePath"/> and writes the
        /// Deep Zoom output (*.dzi + *_files/) to <paramref name="destinationDirectory"/>.
        /// Output tile format matches the source image format.
        /// Levels 0 to <see cref="MinLevel"/>-1 are removed after generation.
        /// </summary>
        public void GenerateTiles(string sourceImagePath, string destinationDirectory)
        {
            ValidateArguments(sourceImagePath, destinationDirectory);

            Directory.CreateDirectory(destinationDirectory);

            string baseName = Path.GetFileNameWithoutExtension(sourceImagePath);
            string dziBasePath = Path.Combine(destinationDirectory, baseName);

            // ── Access strategy ───────────────────────────────────────────────
            // Sequential: single forward pass, low RAM, best for flat files (JPEG, PNG).
            // Random: lets libvips decode tiles in parallel; faster for pyramidal
            //         formats (TIFF pyramid, JPEG2000) at the cost of more RAM.
            Enums.Access access = IsRandomAccessBeneficial(sourceImagePath)
                ? Enums.Access.Random
                : Enums.Access.Sequential;

            using Image image = Image.NewFromFile(sourceImagePath, access: access);

            string suffix = GetFormatSuffix(image);

            image.Dzsave(
                dziBasePath,
                layout: Enums.ForeignDzLayout.Dz,
                suffix: suffix,
                overlap: _overlap,
                tileSize: _tileSize
            );

            // ── Remove low-resolution levels ──────────────────────────────────
            // Dzsave always generates every level from 0 (1×1 px) up to the full
            // resolution level. We discard levels 0 … MinLevel-1 because they are
            // too coarse to be useful and only waste disk space.
            DeleteLowResolutionLevels(destinationDirectory, baseName);
        }

        /// <summary>
        /// Validates the image at <paramref name="imagePath"/> without fully loading it.
        /// </summary>
        public (bool IsValid, string ErrorMessage) ValidateImage(string imagePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imagePath))
                    return (false, "Ruta inválida");

                if (!File.Exists(imagePath))
                    return (false, "El archivo no existe");

                using Image image = Image.NewFromFile(
                    imagePath,
                    access: Enums.Access.Sequential);

                if (image.Width < 32 || image.Height < 32)
                    return (false, "Imagen demasiado pequeña (mínimo 32×32 px)");

                return (true, string.Empty);
            }
            catch (VipsException ex)
            {
                return (false, $"Formato no soportado: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error al validar la imagen: {ex.Message}");
            }
        }

        #endregion

        #region Private helpers

        private void ValidateArguments(string sourceImagePath, string destinationDirectory)
        {
            if (string.IsNullOrWhiteSpace(sourceImagePath))
                throw new ArgumentException("Ruta de origen inválida", nameof(sourceImagePath));

            if (string.IsNullOrWhiteSpace(destinationDirectory))
                throw new ArgumentException("Ruta de destino inválida", nameof(destinationDirectory));

            if (!Path.IsPathRooted(sourceImagePath) || !Path.IsPathRooted(destinationDirectory))
                throw new ArgumentException("Las rutas deben ser absolutas");

            if (!File.Exists(sourceImagePath))
                throw new FileNotFoundException("La imagen no existe", sourceImagePath);

            var (isValid, errorMessage) = ValidateImage(sourceImagePath);
            if (!isValid)
                throw new InvalidOperationException(errorMessage);
        }

        /// <summary>
        /// Deletes the tile subdirectories for levels 0 … <see cref="MinLevel"/>-1
        /// inside <c>&lt;destinationDirectory&gt;/&lt;baseName&gt;_files/</c>.
        /// Directories that do not exist are silently ignored.
        /// </summary>
        private static void DeleteLowResolutionLevels(string destinationDirectory, string baseName)
        {
            string tilesRoot = Path.Combine(destinationDirectory, $"{baseName}_files");

            if (!Directory.Exists(tilesRoot))
                return;

            for (int level = 0; level < MinLevel; level++)
            {
                string levelDir = Path.Combine(tilesRoot, level.ToString());
                if (Directory.Exists(levelDir))
                    Directory.Delete(levelDir, recursive: true);
            }
        }

        /// <summary>
        /// Returns true for formats where Random access is faster than Sequential
        /// because libvips can decode tiles in parallel (pyramidal TIFF, JPEG2000).
        /// </summary>
        private static bool IsRandomAccessBeneficial(string path)
        {
            string ext = Path.GetExtension(path).ToLowerInvariant();
            return ext is ".tif" or ".tiff" or ".jp2" or ".j2k" or ".j2c";
        }

        /// <summary>
        /// Builds the libvips save suffix from the actual loader used for the image.
        /// </summary>
        private string GetFormatSuffix(Image image)
        {
            string loader = image.Get("vips-loader")?.ToString() ?? string.Empty;

            if (loader.Contains("png", StringComparison.OrdinalIgnoreCase))
                return ".png[compression=9]";

            if (loader.Contains("webp", StringComparison.OrdinalIgnoreCase))
                return $".webp[Q={_quality}]";

            if (loader.Contains("tiff", StringComparison.OrdinalIgnoreCase))
                return $".tif[compression=jpeg,Q={_quality}]";

            if (loader.Contains("gif", StringComparison.OrdinalIgnoreCase))
                return ".gif";

            // jpeg / jp2 / heif / avif / any other lossy → jpg tiles
            return $".jpg[Q={_quality},optimize_coding=true,strip=true]";
        }

        #endregion

    }
}