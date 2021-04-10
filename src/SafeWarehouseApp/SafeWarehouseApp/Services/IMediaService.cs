using System.Threading.Tasks;
using SafeWarehouseApp.Models;
using Xamarin.Essentials;

namespace SafeWarehouseApp.Services
{
    public interface IMediaService
    {
        Task SaveFileResultAsync(FileResult fileResult, string path);
        Task<MediaItem> SaveAsMediaItem(FileResult fileResult, string? tag = default);
        Task<MediaItem> CreateMediaItem(FileResult fileResult, string? tag = default);
        Task<string?> GetMediaItemPathAsync(string mediaItemId);
        Task DeleteManyByTagAsync(string tag);
    }
}