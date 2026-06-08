namespace ShopAI.Services;

public interface IImageService
{
    /// <summary>
    /// Uploads a product image, stores optimized WebP assets, and returns the public URL for the main image.
    /// </summary>
    Task<string> UploadProductImageAsync(IFormFile file, int storeId);
}
