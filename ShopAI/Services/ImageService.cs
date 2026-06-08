using Microsoft.Extensions.Options;
using ShopAI.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace ShopAI.Services;

public sealed class ImageService(
    IWebHostEnvironment environment,
    IOptions<ImageUploadOptions> options,
    ILogger<ImageService> logger) : IImageService
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    /// <inheritdoc />
    public async Task<string> UploadProductImageAsync(IFormFile file, int storeId)
    {
        if (file.Length == 0)
        {
            throw new InvalidOperationException("The uploaded image is empty.");
        }

        if (!AllowedContentTypes.Contains(file.ContentType))
        {
            throw new InvalidOperationException("Only JPEG, PNG, GIF, and WebP images are supported.");
        }

        var basePath = options.Value.BasePath.Trim('/', '\\');
        var relativeDirectory = Path.Combine(basePath, storeId.ToString());
        var absoluteDirectory = Path.Combine(environment.WebRootPath, relativeDirectory);
        Directory.CreateDirectory(absoluteDirectory);

        var fileName = $"{Guid.NewGuid():N}.webp";
        var thumbnailName = Path.GetFileNameWithoutExtension(fileName) + "_thumb.webp";
        var absolutePath = Path.Combine(absoluteDirectory, fileName);
        var thumbnailPath = Path.Combine(absoluteDirectory, thumbnailName);

        try
        {
            await using var stream = file.OpenReadStream();
            using var image = await Image.LoadAsync(stream);

            image.Mutate(context => context.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(1200, 1200)
            }));

            await image.SaveAsWebpAsync(absolutePath, new WebpEncoder { Quality = 82 });

            using var thumbnail = image.Clone(context => context.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Crop,
                Size = new Size(400, 400)
            }));

            await thumbnail.SaveAsWebpAsync(thumbnailPath, new WebpEncoder { Quality = 78 });
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to process product image {FileName} for store {StoreId}.", file.FileName, storeId);
            throw;
        }

        return "/" + Path.Combine(relativeDirectory, fileName).Replace('\\', '/');
    }
}
