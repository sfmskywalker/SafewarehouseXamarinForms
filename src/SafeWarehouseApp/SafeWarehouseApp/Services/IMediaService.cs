using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SafeWarehouseApp.Models;
using Xamarin.Essentials;

namespace SafeWarehouseApp.Services
{
    public interface IMediaService
    {
        Task SaveFileResultAsync(FileResult fileResult, string path);
        Task SaveBufferAsync(byte[] buffer, string path);
        Task<MediaItem> SaveAsMediaItem(FileResult fileResult, string? tag = default);
        Task<MediaItem> CreateMediaItem(FileResult fileResult, string? tag = default);
        Task<MediaItem> CreateMediaItem(byte[] buffer, string extension, string contentType, string? tag = default);
        Task<string?> GetMediaItemPathAsync(string mediaItemId);
        string GetMediaItemPath(MediaItem mediaItem);
        Task<MediaItem?> GetMediaItemAsync(string mediaItemId);
        Task DeleteManyByTagAsync(string tag);
        Task DeleteManyByIdAsync(IEnumerable<string> ids);
        Task<string> GetImageDataUrlAsync(MediaItem mediaItem, CancellationToken cancellationToken = default);
    }
}